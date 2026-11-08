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
    string ConversionWorkspace, IReadOnlyList<string> SourceSchemaWorkspaces, string OutputWorkspace);

public sealed record TransformScriptConversionResult(string WorkspacePath, int ScriptCount, int ConvertedReferenceCount);
public sealed record TransformScriptConversionModelResult(MetaTransformScriptModel Model, int ScriptCount, int ConvertedReferenceCount);

/// <summary>Rewrite source-column reads using authored type rules. Target contracts and Binding results are not inputs.</summary>
public sealed class TransformScriptConversionWorkspaceService
{
    public TransformScriptConversionResult Apply(TransformScriptConversionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        EnsureSeparateOutput(request.OutputWorkspace, [request.InputWorkspace, request.DataTypeWorkspace, request.ConversionWorkspace, .. request.SourceSchemaWorkspaces]);
        var result = Convert(
            TypedWorkspaceModelMapper.Load<MetaTransformScriptModel>(request.InputWorkspace, searchUpward: false),
            TypedWorkspaceModelMapper.Load<MetaDataTypeModel>(request.DataTypeWorkspace, searchUpward: false),
            TypedWorkspaceModelMapper.Load<MetaDataTypeConversionModel>(request.ConversionWorkspace, searchUpward: false),
            request.SourceSchemaWorkspaces.Select(path => TypedWorkspaceModelMapper.Load<MetaSchemaModel>(path, searchUpward: false)).ToArray());
        TypedWorkspaceModelMapper.Create(result.Model, Path.GetFullPath(request.OutputWorkspace), "xml");
        return new(Path.GetFullPath(request.OutputWorkspace), result.ScriptCount, result.ConvertedReferenceCount);
    }

    public TransformScriptConversionModelResult Convert(MetaTransformScriptModel input, MetaDataTypeModel catalog,
        MetaDataTypeConversionModel conversions, MetaSchemaModel source)
        => Convert(input, catalog, conversions, new[] { source });

    public TransformScriptConversionModelResult Convert(MetaTransformScriptModel input, MetaDataTypeModel catalog,
        MetaDataTypeConversionModel conversions, IReadOnlyList<MetaSchemaModel> source)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(conversions);
        var types = new SqlServerSchemaTypeValidator(catalog);
        ArgumentNullException.ThrowIfNull(source);
        if (source.Count == 0) throw new ArgumentException("At least one source schema workspace is required.", nameof(source));
        foreach (var schema in source) types.Validate(schema);
        if (input.TransformScriptList.Count == 0) throw new InvalidOperationException("The input workspace contains no TransformScripts.");
        var syntax = new MetaTransformScriptSqlService();
        var model = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(input), MetaTransformScriptModel.CreateEmpty);
        // Expand implicit physical reads using Binding's resolved star evidence, not alias guesses.
        foreach (var script in model.TransformScriptList)
        {
            var analysis = Analyze(model, script, source, catalog);
            foreach (var star in analysis.SelectStarExpansions.DistinctBy(item => item.SyntaxSelectStarId))
                if (star.Columns.Any(read => IsPhysicalRead(read) && conversions.DataTypeMappingList.Any(mapping => mapping.SourceDataTypeId == read.ResolvedColumn.DataType?.MetaDataTypeId)))
                    syntax.ExpandSelectStar(model, star.SyntaxSelectStarId, star.Columns.Select(read => read.IdentifierParts).ToArray());
        }
        var edits = new Dictionary<string, (string Sql, string Column)>();
        foreach (var script in model.TransformScriptList)
        {
            var analysis = Analyze(model, script, source, catalog);
            foreach (var read in analysis.ColumnReferences.Where(IsPhysicalRead)
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
                    var edit = (Sql: "(" + sql + ")", Column: column.Name);
                    if (edits.TryGetValue(reference.Id, out var previous) && previous != edit)
                        throw new InvalidOperationException($"Conflicting source evidence for '{display}'.");
                    edits[reference.Id] = edit;
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
        _ = TypedWorkspaceModelMapper.ToInMemoryWorkspace(model); // Prove graph integrity before materializing output.
        return new(model, model.TransformScriptList.Count, edits.Count);
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

    private static bool IsPhysicalRead(RuntimeColumnReference read) =>
        read.ResolvedTableSource.Rowset.DerivationKind is "Source" or "Target";

    private static TransformBindingResult Analyze(MetaTransformScriptModel model, TransformScript script,
        IReadOnlyList<MetaSchemaModel> source, MetaDataTypeModel catalog)
    {
        var analysis = new TransformBindingService().BindTransform(model, script, source, catalog);
        if (analysis.HasErrors)
            throw new InvalidOperationException($"Cannot resolve source reads in '{script.Name}': " + string.Join("; ", analysis.Issues.Select(issue => issue.Message)));
        return analysis;
    }
}
