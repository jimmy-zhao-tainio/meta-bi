using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using MTP = global::MetaTransformPattern;

namespace MetaTransformPattern.Core;

public sealed partial class TransformPatternAuthoringService
{
    private static readonly StringComparer IdentityComparer = StringComparer.OrdinalIgnoreCase;

    public MTP.MetaTransformPatternModel CreateWorkspace() => MTP.MetaTransformPatternModel.CreateEmpty();

    public MTP.TransformPattern AddPattern(
        MTP.MetaTransformPatternModel model,
        string id,
        string name,
        string? description,
        string patternText)
    {
        ArgumentNullException.ThrowIfNull(model);
        var normalizedId = RequireText(id, "Pattern id");
        var normalizedName = RequireText(name, "Pattern name");
        if (model.TransformPatternList.Any(pattern => IdentityComparer.Equals(pattern.Id, normalizedId)))
        {
            throw new InvalidOperationException($"Transform pattern '{normalizedId}' already exists.");
        }

        var syntax = Parse(patternText);
        var pattern = new MTP.TransformPattern
        {
            Id = normalizedId,
            Name = normalizedName,
            Description = NormalizeOptional(description),
        };
        model.TransformPatternList.Add(pattern);
        MaterializeSyntax(model, pattern, syntax);
        return pattern;
    }

    public MTP.TransformPattern UpdatePattern(
        MTP.MetaTransformPatternModel model,
        string patternId,
        string patternText)
    {
        ArgumentNullException.ThrowIfNull(model);
        var pattern = RequirePattern(model, patternId);
        var syntax = Parse(patternText);
        var oldItems = model.TransformPatternItemList
            .Where(item => ReferenceEquals(item.TransformPattern, pattern))
            .ToHashSet();
        var oldPlaceholders = model.TransformPatternPlaceholderList
            .Where(placeholder => ReferenceEquals(placeholder.TransformPattern, pattern))
            .ToHashSet();
        model.TransformPatternTextList.RemoveAll(text => oldItems.Contains(text.TransformPatternItem));
        model.TransformPatternPlaceholderItemList.RemoveAll(item => oldItems.Contains(item.TransformPatternItem));
        model.TransformPatternItemList.RemoveAll(oldItems.Contains);
        model.TransformPatternPlaceholderList.RemoveAll(oldPlaceholders.Contains);
        MaterializeSyntax(model, pattern, syntax);
        return pattern;
    }

    public string EmitPattern(MTP.MetaTransformPatternModel model, string patternId)
    {
        ArgumentNullException.ThrowIfNull(model);
        var pattern = RequirePattern(model, patternId);
        var items = OrderedItems(model, pattern);
        var builder = new StringBuilder();
        foreach (var item in items)
        {
            var texts = model.TransformPatternTextList
                .Where(text => ReferenceEquals(text.TransformPatternItem, item))
                .ToArray();
            var placeholders = model.TransformPatternPlaceholderItemList
                .Where(reference => ReferenceEquals(reference.TransformPatternItem, item))
                .ToArray();
            if (texts.Length + placeholders.Length != 1)
            {
                throw new InvalidOperationException(
                    $"Pattern item '{item.Id}' must have exactly one text or placeholder shape.");
            }

            if (texts.Length == 1)
            {
                builder.Append(texts[0].SqlText.Replace("$(", "$$(", StringComparison.Ordinal));
                continue;
            }

            var placeholder = placeholders[0].TransformPatternPlaceholder;
            builder.Append("$(").Append(placeholder.Name).Append(')');
        }

        return builder.ToString();
    }

    public MTP.TransformPattern RequirePattern(
        MTP.MetaTransformPatternModel model,
        string patternId)
    {
        ArgumentNullException.ThrowIfNull(model);
        var normalizedId = RequireText(patternId, "Pattern id");
        return model.TransformPatternList.SingleOrDefault(pattern =>
                   IdentityComparer.Equals(pattern.Id, normalizedId))
               ?? throw new InvalidOperationException($"Transform pattern '{normalizedId}' was not found.");
    }

    public MTP.TransformPatternPlaceholder RequirePlaceholder(
        MTP.MetaTransformPatternModel model,
        string patternId,
        string placeholderIdentity)
    {
        var pattern = RequirePattern(model, patternId);
        var normalizedIdentity = RequireText(placeholderIdentity, "Placeholder identity");
        return model.TransformPatternPlaceholderList.SingleOrDefault(placeholder =>
                   ReferenceEquals(placeholder.TransformPattern, pattern) &&
                   (IdentityComparer.Equals(placeholder.Id, normalizedIdentity) ||
                    IdentityComparer.Equals(placeholder.Name, normalizedIdentity)))
               ?? throw new InvalidOperationException(
                   $"Placeholder '{normalizedIdentity}' was not found in pattern '{pattern.Id}'.");
    }

    private static ParsedPattern Parse(string patternText)
    {
        ArgumentNullException.ThrowIfNull(patternText);
        if (string.IsNullOrWhiteSpace(patternText))
        {
            throw new InvalidOperationException("Pattern text cannot be blank.");
        }

        var items = new List<ParsedItem>();
        var placeholders = new Dictionary<string, ParsedPlaceholder>(IdentityComparer);
        var text = new StringBuilder();
        var index = 0;
        while (index < patternText.Length)
        {
            if (patternText.AsSpan(index).StartsWith("$$(", StringComparison.Ordinal))
            {
                text.Append("$(");
                index += 3;
                continue;
            }

            if (!patternText.AsSpan(index).StartsWith("$(", StringComparison.Ordinal))
            {
                text.Append(patternText[index++]);
                continue;
            }

            FlushText(items, text);
            var markerStart = index;
            var markerEnd = patternText.IndexOf(')', markerStart + 2);
            if (markerEnd < 0)
            {
                throw new InvalidOperationException(
                    $"Placeholder beginning at offset {markerStart} is missing its closing ')'.");
            }

            var marker = patternText[(markerStart + 2)..markerEnd];
            var match = PlaceholderPattern().Match(marker);
            if (!match.Success)
            {
                throw new InvalidOperationException(
                    $"Invalid placeholder at offset {markerStart}. Use $(name).");
            }

            var name = match.Groups["name"].Value;
            placeholders.TryAdd(name, new ParsedPlaceholder(name));
            items.Add(new ParsedItem(Text: null, PlaceholderName: name));
            index = markerEnd + 1;
        }

        FlushText(items, text);
        if (items.Count == 0)
        {
            throw new InvalidOperationException("Pattern text did not produce any pattern items.");
        }

        return new ParsedPattern(items, placeholders);
    }

    private static void FlushText(List<ParsedItem> items, StringBuilder text)
    {
        if (text.Length == 0)
        {
            return;
        }

        items.Add(new ParsedItem(text.ToString(), PlaceholderName: null));
        text.Clear();
    }

    private static void MaterializeSyntax(
        MTP.MetaTransformPatternModel model,
        MTP.TransformPattern pattern,
        ParsedPattern syntax)
    {
        var placeholders = new Dictionary<string, MTP.TransformPatternPlaceholder>(IdentityComparer);
        foreach (var parsed in syntax.Placeholders.Values)
        {
            var placeholder = new MTP.TransformPatternPlaceholder
            {
                Id = $"{pattern.Id}:placeholder:{parsed.Name}",
                Name = parsed.Name,
                TransformPattern = pattern,
            };
            model.TransformPatternPlaceholderList.Add(placeholder);
            placeholders.Add(parsed.Name, placeholder);
        }

        MTP.TransformPatternItem? previous = null;
        for (var index = 0; index < syntax.Items.Count; index++)
        {
            var parsed = syntax.Items[index];
            var item = new MTP.TransformPatternItem
            {
                Id = $"{pattern.Id}:item:{index.ToString(CultureInfo.InvariantCulture)}",
                PreviousItem = previous,
                TransformPattern = pattern,
            };
            model.TransformPatternItemList.Add(item);
            if (parsed.Text is not null)
            {
                model.TransformPatternTextList.Add(new MTP.TransformPatternText
                {
                    Id = $"{item.Id}:text",
                    SqlText = parsed.Text,
                    TransformPatternItem = item,
                });
            }
            else
            {
                model.TransformPatternPlaceholderItemList.Add(new MTP.TransformPatternPlaceholderItem
                {
                    Id = $"{item.Id}:placeholder",
                    TransformPatternItem = item,
                    TransformPatternPlaceholder = placeholders[parsed.PlaceholderName!],
                });
            }

            previous = item;
        }
    }

    private static IReadOnlyList<MTP.TransformPatternItem> OrderedItems(
        MTP.MetaTransformPatternModel model,
        MTP.TransformPattern pattern)
    {
        var items = model.TransformPatternItemList
            .Where(item => ReferenceEquals(item.TransformPattern, pattern))
            .ToArray();
        var roots = items.Where(item => item.PreviousItem is null).ToArray();
        if (roots.Length != 1)
        {
            throw new InvalidOperationException(
                $"Transform pattern '{pattern.Id}' must have exactly one root item.");
        }

        var ordered = new List<MTP.TransformPatternItem>(items.Length);
        var visited = new HashSet<MTP.TransformPatternItem>();
        var current = roots[0];
        while (visited.Add(current))
        {
            ordered.Add(current);
            var successors = items.Where(item => ReferenceEquals(item.PreviousItem, current)).ToArray();
            if (successors.Length > 1)
            {
                throw new InvalidOperationException(
                    $"Pattern item '{current.Id}' has more than one successor.");
            }

            if (successors.Length == 0)
            {
                break;
            }

            current = successors[0];
        }

        if (ordered.Count != items.Length)
        {
            throw new InvalidOperationException(
                $"Transform pattern '{pattern.Id}' contains unreachable or cyclic items.");
        }

        return ordered;
    }

    private static string RequireText(string value, string label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return value.Trim();
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    [GeneratedRegex(
        "^\\s*(?<name>[\\p{L}_][\\p{L}\\p{N}_.-]*)\\s*$",
        RegexOptions.CultureInvariant)]
    private static partial Regex PlaceholderPattern();

    private sealed record ParsedPattern(
        IReadOnlyList<ParsedItem> Items,
        IReadOnlyDictionary<string, ParsedPlaceholder> Placeholders);

    private sealed record ParsedItem(string? Text, string? PlaceholderName);

    private sealed record ParsedPlaceholder(string Name);
}
