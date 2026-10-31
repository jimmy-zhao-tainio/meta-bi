using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using MetaDataType;
using MetaTransformScript.Sql;

namespace MetaDataTypeConversion.Core;

public sealed record SqlValueConversionRequest(
    string SourceDataTypeId,
    string TargetDataTypeSystemName,
    string InputExpression,
    SqlServerTypeFacets TargetFacets,
    SqlServerTypeFacets? SourceFacets = null);

public sealed record SqlValueConversionResult(DataTypeMappingResolution Mapping, SqlScalarExpression Expression);

public sealed partial class MetaDataTypeConversionService
{
    /// <summary>Instantiate explicitly authored conversion metadata; existing BI consumers do not call this helper.</summary>
    public SqlValueConversionResult GenerateSqlExpression(
        MetaDataTypeConversionModel model, MetaDataTypeModel dataTypes, SqlValueConversionRequest request)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(dataTypes);
        ArgumentNullException.ThrowIfNull(request);
        var check = Check(model, dataTypes);
        if (check.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, check.Errors));
        var resolution = Resolve(model, dataTypes, request.SourceDataTypeId, request.TargetDataTypeSystemName);
        var syntax = new MetaTransformScriptSqlService();
        if (resolution.ConversionImplementationId == "MetaDataTypeConversion:implementation:direct")
            return new(resolution, syntax.ImportScalarExpression(request.InputExpression));
        if (resolution.ConversionImplementationId == "MetaDataTypeConversion:implementation:structural")
            throw new InvalidOperationException($"Mapping '{resolution.MappingId}' is Structural; it has no scalar SQL value conversion.");

        var implementation = model.ConversionImplementationList.Single(row => row.Id == resolution.ConversionImplementationId);
        var snippet = implementation.SqlConversionSnippet
            ?? throw new InvalidOperationException($"Mapping '{resolution.MappingId}', implementation '{implementation.Id}' has no SQL conversion snippet.");
        // Only input preparation is code. SQL function choice, composition and fixed arguments are data.
        // Definitions are lazy: a source-only snippet must not require destination metadata or facets.
        var inputs = new Dictionary<string, Lazy<string>>(StringComparer.Ordinal)
        {
            ["MetaDataTypeConversion:parameter:source-expression"] =
                new(() => "(" + syntax.ExportScalarExpression(syntax.ImportScalarExpression(request.InputExpression)) + ")"),
            ["MetaDataTypeConversion:parameter:destination-type"] = new(() =>
            {
                ArgumentNullException.ThrowIfNull(request.TargetFacets);
                var target = dataTypes.DataTypeList.SingleOrDefault(row => row.Id == resolution.TargetDataTypeId)
                    ?? throw new InvalidOperationException($"Mapping '{resolution.MappingId}' target '{resolution.TargetDataTypeId}' is absent from MetaDataType.");
                if (!string.Equals(target.DataTypeSystem?.Name, "SqlServer", StringComparison.OrdinalIgnoreCase))
                {
                    var candidates = (from mapping in model.DataTypeMappingList
                                      where mapping.SourceDataTypeId == target.Id
                                      join dataType in dataTypes.DataTypeList on mapping.TargetDataTypeId equals dataType.Id
                                      where string.Equals(dataType.DataTypeSystem?.Name, "SqlServer", StringComparison.OrdinalIgnoreCase)
                                      select dataType).ToArray();
                    if (candidates.Length != 1)
                        throw new InvalidOperationException($"Target '{target.Id}' requires exactly one mapped SqlServer physical type; found {candidates.Length}.");
                    target = candidates[0];
                }
                var facets = ResolveLengthPolicy(model, resolution.MappingId, request.SourceFacets, request.TargetFacets);
                return syntax.RenderSqlServerDataType(target.Name, facets);
            })
        };
        var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var binding in model.SqlConversionParameterBindingList.Where(row => row.SqlConversionSnippet.Id == snippet.Id))
        {
            if (!inputs.TryGetValue(binding.SqlConversionParameter.Id, out var input))
                throw new InvalidOperationException($"Binding '{binding.Id}' requires unsupported conversion input '{binding.SqlConversionParameter.Id}'.");
            if (parameters.ContainsKey(binding.Name))
                throw new InvalidOperationException($"Snippet '{snippet.Id}' binds placeholder '{binding.Name}' more than once.");
            parameters.Add(binding.Name, input.Value);
        }
        return new(resolution, syntax.ImportScalarExpression(SubstituteSnippet(snippet, parameters)));
    }

    private static SqlServerTypeFacets ResolveLengthPolicy(
        MetaDataTypeConversionModel model, string mappingId,
        SqlServerTypeFacets? source, SqlServerTypeFacets destination)
    {
        var rules = model.DataTypeMappingLengthRuleList
            .Where(rule => rule.DataTypeMapping.Id == mappingId)
            .Select(rule => (Rule: rule, Maximum: ParseRuleLength(rule.SourceMaximumLength, rule.Id)))
            .ToArray();
        if (rules.Length == 0) return destination;
        if (source is null || (source.IsMax == source.Length.HasValue) || (source.Length.HasValue && source.Length <= 0))
            throw new InvalidOperationException($"Mapping '{mappingId}' requires an explicit positive source length or MAX to select its length rule.");
        if (rules.Select(rule => rule.Maximum).Distinct().Count() != rules.Length)
            throw new InvalidOperationException($"Mapping '{mappingId}' has duplicate source maximum lengths.");
        var sourceLength = source.IsMax ? (long)int.MaxValue + 1 : source.Length!.Value;
        var selected = rules.Where(rule => sourceLength <= rule.Maximum).OrderBy(rule => rule.Maximum).FirstOrDefault();
        if (selected.Rule is null)
            throw new InvalidOperationException($"Mapping '{mappingId}' has no length rule for source length {(source.IsMax ? "MAX" : source.Length!.Value.ToString(CultureInfo.InvariantCulture))}.");
        var targetLength = ParseRuleLength(selected.Rule.TargetLength, selected.Rule.Id);
        return targetLength > int.MaxValue
            ? destination with { Length = null, IsMax = true }
            : destination with { Length = (int)targetLength, IsMax = false };
    }

    private static long ParseRuleLength(string value, string ruleId)
    {
        if (string.Equals(value?.Trim(), "MAX", StringComparison.OrdinalIgnoreCase)) return (long)int.MaxValue + 1;
        if (int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var length) && length > 0) return length;
        throw new InvalidOperationException($"Length rule '{ruleId}' has invalid length '{value}'; use a positive integer or MAX.");
    }

    private static string SubstituteSnippet(SqlConversionSnippet snippet, IReadOnlyDictionary<string, string> parameters)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(snippet.TransformScript);
        var result = new StringBuilder();
        var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var text = snippet.TransformScript;
        for (var index = 0; index < text.Length;)
        {
            // Same $(name), optional whitespace and $$( escaping as the pattern text syntax.
            if (text.AsSpan(index).StartsWith("$$(", StringComparison.Ordinal))
            {
                result.Append("$(");
                index += 3;
            }
            else if (text.AsSpan(index).StartsWith("$(", StringComparison.Ordinal))
            {
                var end = text.IndexOf(')', index + 2);
                if (end < 0)
                    throw new InvalidOperationException($"Snippet '{snippet.Id}' has an unclosed placeholder at offset {index}.");
                var name = text[(index + 2)..end].Trim();
                if (!SnippetParameterName().IsMatch(name))
                    throw new InvalidOperationException($"Snippet '{snippet.Id}' has invalid placeholder '{name}'; use $(name).");
                if (!parameters.TryGetValue(name, out var value))
                    throw new InvalidOperationException($"Snippet '{snippet.Id}' has no binding for placeholder '{name}'.");
                result.Append(value); // Inserted SQL is never rescanned for placeholders.
                used.Add(name);
                index = end + 1;
            }
            else result.Append(text[index++]);
        }
        var unused = parameters.Keys.FirstOrDefault(name => !used.Contains(name));
        if (unused is not null)
            throw new InvalidOperationException($"Snippet '{snippet.Id}' binds unused placeholder '{unused}'.");
        return result.ToString();
    }

    [GeneratedRegex("^[\\p{L}_][\\p{L}\\p{N}_.-]*$", RegexOptions.CultureInvariant)]
    private static partial Regex SnippetParameterName();
}
