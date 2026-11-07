using Meta.Integration;
using MetaDataType;
using MetaDataTypeConversion;
using MetaDataTypeConversion.Core;
using MetaSchema;
using MetaTransform.Binding;
using MetaTransformScript;
using MetaTransformScript.Sql;

namespace MetaConvert.TransformScriptConversion;

public sealed record TransformScriptConversionRequest(string InputWorkspace, string DataTypeWorkspace,
    string ConversionWorkspace, string SourceSchemaWorkspace, string OutputWorkspace);

public sealed record TransformScriptConversionResult(string WorkspacePath, int ScriptCount, int ConvertedReferenceCount);
public sealed record TransformScriptConversionModelResult(MetaTransformScriptModel Model, int ScriptCount, int ConvertedReferenceCount);

/// <summary>Rewrite source-column reads using authored type rules. Target contracts and Binding results are not inputs.</summary>
public sealed class TransformScriptConversionWorkspaceService
{
    public TransformScriptConversionResult Apply(TransformScriptConversionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        EnsureSeparateOutput(request.OutputWorkspace, request.InputWorkspace, request.DataTypeWorkspace, request.ConversionWorkspace, request.SourceSchemaWorkspace);
        var result = Convert(
            TypedWorkspaceModelMapper.Load<MetaTransformScriptModel>(request.InputWorkspace, searchUpward: false),
            TypedWorkspaceModelMapper.Load<MetaDataTypeModel>(request.DataTypeWorkspace, searchUpward: false),
            TypedWorkspaceModelMapper.Load<MetaDataTypeConversionModel>(request.ConversionWorkspace, searchUpward: false),
            TypedWorkspaceModelMapper.Load<MetaSchemaModel>(request.SourceSchemaWorkspace, searchUpward: false));
        TypedWorkspaceModelMapper.Create(result.Model, Path.GetFullPath(request.OutputWorkspace), "xml");
        return new(Path.GetFullPath(request.OutputWorkspace), result.ScriptCount, result.ConvertedReferenceCount);
    }

    public TransformScriptConversionModelResult Convert(MetaTransformScriptModel input, MetaDataTypeModel catalog,
        MetaDataTypeConversionModel conversions, MetaSchemaModel source)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(conversions);
        var types = new SqlServerSchemaTypeValidator(catalog);
        types.Validate(source);
        EnsureSupportedScripts(input);
        var syntax = new MetaTransformScriptSqlService();
        var model = CopyScripts(input, syntax);
        var edits = new Dictionary<string, (string Sql, string Column)>();
        foreach (var script in model.TransformScriptList)
        {
            var analysis = new TransformBindingService().BindTransform(model, script, source, catalog);
            if (analysis.HasErrors)
                throw new InvalidOperationException($"Cannot resolve source reads in '{script.Name}': " + string.Join("; ", analysis.Issues.Select(issue => issue.Message)));
            foreach (var read in analysis.ColumnReferences.Where(read => read.ResolvedTableSource.Rowset.DerivationKind == "Source")
                .DistinctBy(read => read.SyntaxColumnReferenceId))
            {
                var column = read.ResolvedColumn;
                var display = read.ResolvedTableSource.SqlIdentifier + "." + column.Name;
                var sourceType = types.Resolve(column.DataType?.MetaDataTypeId, $"Source column '{display}'");
                var mappings = conversions.DataTypeMappingList.Where(row => row.SourceDataTypeId == sourceType.Id).ToArray();
                if (mappings.Length == 0) continue;
                if (mappings.Length != 1)
                    throw new InvalidOperationException($"Source column '{display}' ({sourceType.Id}) has {mappings.Length} authored conversions; the source type rule is ambiguous.");
                var mapping = mappings[0];
                var destination = types.Resolve(mapping.TargetDataTypeId, $"Conversion '{mapping.Id}' target");
                if (destination.Name.ToLowerInvariant() is "time" or "datetime2" or "datetimeoffset" or "float")
                    throw new NotSupportedException($"Conversion '{mapping.Id}' for '{display}': temporal fractional precision and float precision destinations remain unsupported in this slice.");
                if (!MetaTransformScriptSqlServerDataTypes.TryMapSqlName(destination.Name, out var option) ||
                    !string.Equals(MetaTransformScriptSqlServerDataTypes.RenderSqlName(option), destination.Name, StringComparison.OrdinalIgnoreCase))
                    throw new NotSupportedException($"Conversion '{mapping.Id}' uses unsupported SQL Server type name '{destination.Name}'.");
                var reference = model.ColumnReferenceExpressionList.SingleOrDefault(row => row.Id == read.SyntaxColumnReferenceId)
                    ?? throw new NotSupportedException($"Source read '{display}' in '{script.Name}' is not an ordinary column-reference expression; conversion of method call targets is unsupported.");
                var original = syntax.ExportScalarExpression(new(model, reference.PrimaryExpression.ScalarExpression));
                try
                {
                    // DestinationType is a catalog-resolved type name. Any facets/defaults are authored in the snippet.
                    var sql = SqlConversionSnippetService.Instantiate(conversions, mapping, "(" + original + ")", destination.Name);
                    var parsed = syntax.ImportScalarExpression(sql);
                    if (parsed.Model.MaxLiteralList.Count > 0)
                        throw new NotSupportedException("MAX length destinations remain unsupported in this slice.");
                    edits.Add(reference.Id, ("(" + sql + ")", column.Name));
                }
                catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or NotSupportedException or MetaTransformScriptSqlImportException)
                {
                    throw new InvalidOperationException($"Cannot apply authored conversion '{mapping.Id}' to '{display}' in '{script.Name}': {ex.Message}", ex);
                }
            }
        }
        // Only original source references are selected. Inserted snippet references are never rewritten again.
        foreach (var edit in edits)
            syntax.ReplaceColumnReference(model, model.ColumnReferenceExpressionList.Single(row => row.Id == edit.Key), edit.Value.Sql, edit.Value.Column);
        return new(CopyScripts(model, syntax), model.TransformScriptList.Count, edits.Count);
    }

    public static void EnsureSeparateOutput(string outputWorkspace, params string[] inputWorkspaces)
    {
        var output = Path.TrimEndingDirectorySeparator(Path.GetFullPath(outputWorkspace));
        foreach (var input in inputWorkspaces)
        {
            var path = Path.TrimEndingDirectorySeparator(Path.GetFullPath(input));
            if (output.Equals(path, StringComparison.OrdinalIgnoreCase) || output.StartsWith(path + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("The output workspace must be separate from every input workspace.");
        }
        if (Directory.Exists(output) || File.Exists(output)) throw new InvalidOperationException($"Output '{output}' already exists.");
    }

    private static void EnsureSupportedScripts(MetaTransformScriptModel model)
    {
        var count = model.TransformScriptList.Count;
        if (count == 0 || model.ScriptObjectViewList.Count != count || model.SelectStatementList.Count != count ||
            model.TransformScriptStatementLinkList.Count != count || model.QuerySpecificationList.Count != count ||
            model.StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList.Count != 0)
            throw new NotSupportedException("Source conversion supports a collection of SELECT scripts with explicit projections; CTEs, nested queries, set queries and non-SELECT scripts are unsupported.");
        if (model.SelectStarExpressionList.Count != 0)
            throw new NotSupportedException("SELECT * is unsupported for source conversion; declare explicit column reads.");
    }

    private static MetaTransformScriptModel CopyScripts(MetaTransformScriptModel input, MetaTransformScriptSqlService syntax)
    {
        var copy = MetaTransformScriptModel.CreateEmpty();
        foreach (var script in input.TransformScriptList)
        {
            var target = input.ScriptObjectViewList.Single(row => row.TransformScript.Id == script.Id);
            syntax.ImportSqlCode(copy, syntax.ExportToSqlCode(input, script.Name), target.TargetSqlIdentifier, script.Name);
        }
        return copy;
    }
}
