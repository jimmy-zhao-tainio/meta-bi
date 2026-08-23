#nullable enable
using System;
using System.Collections.Generic;

namespace MetaTransformPatternInstance;

public sealed partial class TransformPatternInstance
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string TransformPatternId { get; set; } = null !;
}

public sealed partial class TransformPatternInstancePlaceholder
{
    public string Id { get; set; } = null !;
    public string? SqlText { get; set; }
    public string TransformPatternPlaceholderId { get; set; } = null !;
    public TransformPatternInstance TransformPatternInstance { get; set; } = null !;
}

public sealed partial class MetaTransformPatternInstanceModel
{
    public static MetaTransformPatternInstanceModel CreateEmpty() => new();
    public List<TransformPatternInstance> TransformPatternInstanceList { get; set; } = new();
    public List<TransformPatternInstancePlaceholder> TransformPatternInstancePlaceholderList { get; set; } = new();
}

public static partial class MetaTransformPatternInstanceInstance
{
    private static readonly MetaTransformPatternInstanceModel _builtIn = CreateBuiltIn();
    public static MetaTransformPatternInstanceModel BuiltIn => _builtIn;

    public static MetaTransformPatternInstanceModel CreateBuiltIn()
    {
        var model = MetaTransformPatternInstanceModel.CreateEmpty();
        return model;
    }
}
