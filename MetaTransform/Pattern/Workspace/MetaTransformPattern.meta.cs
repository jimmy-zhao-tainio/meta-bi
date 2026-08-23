#nullable enable
using System;
using System.Collections.Generic;

namespace MetaTransformPattern;
public sealed partial class TransformPattern
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
}

public sealed partial class TransformPatternItem
{
    public string Id { get; set; } = null !;
    public TransformPatternItem? PreviousItem { get; set; }
    public TransformPattern TransformPattern { get; set; } = null !;
}

public sealed partial class TransformPatternPlaceholder
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public TransformPattern TransformPattern { get; set; } = null !;
}

public sealed partial class TransformPatternPlaceholderItem
{
    public string Id { get; set; } = null !;
    public TransformPatternItem TransformPatternItem { get; set; } = null !;
    public TransformPatternPlaceholder TransformPatternPlaceholder { get; set; } = null !;
}

public sealed partial class TransformPatternText
{
    public string Id { get; set; } = null !;
    public string SqlText { get; set; } = null !;
    public TransformPatternItem TransformPatternItem { get; set; } = null !;
}

public sealed partial class MetaTransformPatternModel
{
    public static MetaTransformPatternModel CreateEmpty() => new();
    public List<TransformPattern> TransformPatternList { get; set; } = new();
    public List<TransformPatternItem> TransformPatternItemList { get; set; } = new();
    public List<TransformPatternPlaceholder> TransformPatternPlaceholderList { get; set; } = new();
    public List<TransformPatternPlaceholderItem> TransformPatternPlaceholderItemList { get; set; } = new();
    public List<TransformPatternText> TransformPatternTextList { get; set; } = new();
}

public static partial class MetaTransformPatternInstance
{
    private static readonly MetaTransformPatternModel _builtIn = CreateBuiltIn();
    public static MetaTransformPatternModel BuiltIn => _builtIn;

    public static MetaTransformPatternModel CreateBuiltIn()
    {
        var model = MetaTransformPatternModel.CreateEmpty();
        return model;
    }
}
