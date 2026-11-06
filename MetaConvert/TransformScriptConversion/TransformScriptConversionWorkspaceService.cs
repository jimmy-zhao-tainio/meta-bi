using Meta.Integration;
using MetaDataType;
using MetaDataTypeConversion;
using MetaDataTypeConversion.Core;
using MetaSchema;
using MetaTransform.Binding;
using MetaTransformScript;
using MetaTransformScript.Sql;

namespace MetaConvert.TransformScriptConversion;

/// <param name="ProjectionId">SelectScalarExpression identity in the input workspace.</param>
public sealed record ProjectionConversionRequirement(string ProjectionId, string DestinationDataTypeId,
    SqlServerTypeFacets Facets);

public sealed record TransformScriptConversionRequest(string InputWorkspace, string DataTypeWorkspace,
    string ConversionWorkspace, string SourceSchemaWorkspace,
    IReadOnlyList<ProjectionConversionRequirement> Requirements, string OutputWorkspace);

public sealed record TransformScriptConversionResult(string WorkspacePath, int ConvertedProjectionCount);

/// <summary>Produces a separate script. Callers own destination requirements; metadata owns conversion expressions.</summary>
public sealed class TransformScriptConversionWorkspaceService
{
    public TransformScriptConversionResult Apply(TransformScriptConversionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Requirements);
        var output = Path.GetFullPath(request.OutputWorkspace);
        foreach (var input in new[] { request.InputWorkspace, request.DataTypeWorkspace, request.ConversionWorkspace, request.SourceSchemaWorkspace })
        {
            var path = Path.TrimEndingDirectorySeparator(Path.GetFullPath(input));
            if (output.Equals(path, StringComparison.OrdinalIgnoreCase) ||
                output.StartsWith(path + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("The output workspace must be separate from every input workspace.");
        }
        if (Directory.Exists(output) || File.Exists(output)) throw new InvalidOperationException($"Output '{output}' already exists.");
        var model = TypedWorkspaceModelMapper.Load<MetaTransformScriptModel>(request.InputWorkspace, searchUpward: false);
        var catalog = TypedWorkspaceModelMapper.Load<MetaDataTypeModel>(request.DataTypeWorkspace, searchUpward: false);
        var conversions = TypedWorkspaceModelMapper.Load<MetaDataTypeConversionModel>(request.ConversionWorkspace, searchUpward: false);
        var source = TypedWorkspaceModelMapper.Load<MetaSchemaModel>(request.SourceSchemaWorkspace, searchUpward: false);
        var types = new SqlServerSchemaTypeValidator(catalog);
        types.Validate(source);
        if (model.TransformScriptList.Count != 1) throw new InvalidOperationException("Conversion supports exactly one SELECT script.");
        var script = model.TransformScriptList.Single();
        var target = model.ScriptObjectViewList.SingleOrDefault(row => row.TransformScript.Id == script.Id)
            ?? throw new InvalidOperationException("Conversion supports a SELECT script with a view/target contract.");
        if (model.TransformScriptStatementLinkList.Count != 1 || model.SelectStatementList.Count != 1 || model.QuerySpecificationList.Count != 1 ||
            model.StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList.Count != 0)
            throw new InvalidOperationException("Conversion supports one SELECT projection without CTEs, set queries or nested queries.");
        if (request.Requirements.Count == 0 || request.Requirements.Select(row => row.ProjectionId).Distinct().Count() != request.Requirements.Count)
            throw new InvalidOperationException("Supply distinct, explicit projection requirements.");
        var syntax = new MetaTransformScriptSqlService();
        var analysis = Analyze(model, script, source, catalog);
        var expected = new List<(int Ordinal, string Name, string TypeId, SqlServerTypeFacets Facets)>();
        foreach (var requirement in request.Requirements)
        {
            var projection = model.SelectScalarExpressionList.SingleOrDefault(row => row.Id == requirement.ProjectionId)
                ?? throw new InvalidOperationException($"Unsupported or missing scalar projection '{requirement.ProjectionId}'.");
            var column = analysis.TopLevelRowset?.Columns.SingleOrDefault(row => row.Id == projection.SelectElement.Id + ":output")
                ?? throw new InvalidOperationException($"Projection '{projection.Id}' is not a top-level output expression.");
            var contract = column.DataType
                ?? throw new InvalidOperationException($"Unsupported expression typing for projection '{projection.Id}' ('{column.Name}').");
            types.Resolve(contract.MetaDataTypeId, $"Source expression '{projection.Id}'");
            var destination = types.Resolve(requirement.DestinationDataTypeId, $"Destination requirement '{projection.Id}'");
            var mappings = conversions.DataTypeMappingList.Where(row => row.SourceDataTypeId == contract.MetaDataTypeId && row.TargetDataTypeId == destination.Id).ToArray();
            if (mappings.Length != 1)
                throw new InvalidOperationException($"Projection '{projection.Id}': expected one authored conversion '{contract.MetaDataTypeId}' → '{destination.Id}'; found {mappings.Length} (missing or ambiguous mapping).");
            var typeSql = syntax.RenderSqlServerDataType(destination.Name, requirement.Facets);
            var expression = model.SelectScalarExpressionExpressionLinkList.Single(row => row.SelectScalarExpression.Id == projection.Id).ScalarExpression;
            var sourceSql = "(" + syntax.ExportScalarExpression(new(model, expression)) + ")";
            var instantiated = SqlConversionSnippetService.Instantiate(conversions, mappings[0], sourceSql, typeSql);
            // The scalar parser rejects statement injection and unsupported syntax before composition.
            try { syntax.ImportScalarExpression(instantiated); }
            catch (MetaTransformScriptSqlImportException ex)
            {
                throw new InvalidOperationException($"Mapping '{mappings[0].Id}', projection '{projection.Id}': invalid or unsupported scalar snippet. {ex.Message}", ex);
            }
            syntax.ReplaceProjectionExpression(model, projection.Id, instantiated, column.Name);
            expected.Add((column.Ordinal, column.Name, destination.Id, requirement.Facets));
        }
        // Re-import the sanctioned SQL surface to discard detached old operands and normalize the graph.
        var normalized = syntax.ImportSqlCode(MetaTransformScriptModel.CreateEmpty(), syntax.ExportToSqlCode(model),
            target.TargetSqlIdentifier, script.Name).Model;
        var result = Analyze(normalized, normalized.TransformScriptList.Single(), source, catalog);
        foreach (var requirement in expected)
        {
            var actual = result.TopLevelRowset!.Columns.Single(row => row.Ordinal == requirement.Ordinal).DataType;
            var facets = requirement.Facets;
            if (actual is null || actual.MetaDataTypeId != requirement.TypeId ||
                actual.Length != (facets.IsMax ? -1 : facets.Length) || actual.Precision != facets.Precision || actual.Scale != facets.Scale)
                throw new InvalidOperationException($"Generated expression '{requirement.Name}' does not resolve to the exact destination type/facets '{requirement.TypeId}'. Binding expression typing may not support this snippet.");
        }
        TypedWorkspaceModelMapper.Create(normalized, output, "xml");
        return new(output, expected.Count);
    }

    private static TransformBindingResult Analyze(MetaTransformScriptModel model, TransformScript script,
        MetaSchemaModel source, MetaDataTypeModel catalog)
    {
        var result = new TransformBindingService().BindTransform(model, script, source, catalog);
        if (result.HasErrors) throw new InvalidOperationException("Unsupported or unresolved input/generated expression: " +
            string.Join("; ", result.Issues.Select(issue => issue.ToString())));
        return result;
    }
}
