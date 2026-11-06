using System.Text;
using System.Text.RegularExpressions;

namespace MetaDataTypeConversion.Core;

/// <summary>Instantiates authored $(name) bindings using the TransformPattern text syntax.</summary>
public static partial class SqlConversionSnippetService
{
    public static string Instantiate(MetaDataTypeConversionModel model, DataTypeMapping mapping,
        string sourceExpression, string destinationType)
    {
        var implementation = model.ConversionImplementationList.SingleOrDefault(row => row.Id == mapping.ConversionImplementation?.Id)
            ?? throw new InvalidOperationException($"Mapping '{mapping.Id}' references a missing implementation.");
        var snippet = model.SqlConversionSnippetList.SingleOrDefault(row => row.Id == implementation.SqlConversionSnippet?.Id)
            ?? throw new InvalidOperationException($"Mapping '{mapping.Id}', implementation '{implementation.Id}' has no authored SQL conversion snippet.");
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var binding in model.SqlConversionParameterBindingList.Where(row => row.SqlConversionSnippet?.Id == snippet.Id))
        {
            var parameter = model.SqlConversionParameterList.SingleOrDefault(row => row.Id == binding.SqlConversionParameter?.Id)
                ?? throw new InvalidOperationException($"Binding '{binding.Id}' references a missing parameter.");
            var value = parameter.Name switch
            {
                "SourceExpression" => sourceExpression,
                "DestinationType" => destinationType,
                _ => throw new InvalidOperationException($"Binding '{binding.Id}' requests unsupported input '{parameter.Name}'.")
            };
            if (!values.TryAdd(binding.Name, value))
                throw new InvalidOperationException($"Snippet '{snippet.Id}' has ambiguous bindings for '{binding.Name}'.");
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(snippet.TransformScript);
        var text = snippet.TransformScript;
        var result = new StringBuilder();
        var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var index = 0; index < text.Length;)
        {
            // One pass: inserted SQL is never rescanned. $$( escapes a literal $(.
            if (text.AsSpan(index).StartsWith("$$(", StringComparison.Ordinal))
            {
                result.Append("$(");
                index += 3;
            }
            else if (text.AsSpan(index).StartsWith("$(", StringComparison.Ordinal))
            {
                var end = text.IndexOf(')', index + 2);
                if (end < 0) throw new InvalidOperationException($"Snippet '{snippet.Id}' has an unclosed placeholder at offset {index}.");
                var name = text[(index + 2)..end].Trim();
                if (!ParameterName().IsMatch(name) || !values.TryGetValue(name, out var value))
                    throw new InvalidOperationException($"Snippet '{snippet.Id}' has no valid binding for placeholder '{name}'.");
                result.Append(value);
                used.Add(name);
                index = end + 1;
            }
            else result.Append(text[index++]);
        }
        var unused = values.Keys.FirstOrDefault(name => !used.Contains(name));
        if (unused is not null) throw new InvalidOperationException($"Snippet '{snippet.Id}' binds unused placeholder '{unused}'.");
        return result.ToString();
    }

    [GeneratedRegex("^[\\p{L}_][\\p{L}\\p{N}_.-]*$", RegexOptions.CultureInvariant)]
    private static partial Regex ParameterName();
}
