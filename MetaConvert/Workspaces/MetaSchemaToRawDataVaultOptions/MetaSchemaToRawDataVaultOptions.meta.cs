#nullable enable
using System;
using System.Collections.Generic;

namespace MetaSchemaToRawDataVaultOptions;
public sealed partial class ConversionOptions
{
    public string Id { get; set; } = null !;
}

public sealed partial class IgnoredFieldName
{
    public string Id { get; set; } = null !;
    public string Value { get; set; } = null !;
    public ConversionOptions ConversionOptions { get; set; } = null !;
}

public sealed partial class IgnoredFieldSuffix
{
    public string Id { get; set; } = null !;
    public string Value { get; set; } = null !;
    public ConversionOptions ConversionOptions { get; set; } = null !;
}

public sealed partial class IncludeViewsOption
{
    public string Id { get; set; } = null !;
    public ConversionOptions ConversionOptions { get; set; } = null !;
}

public sealed partial class MetaSchemaToRawDataVaultOptionsModel
{
    public static MetaSchemaToRawDataVaultOptionsModel CreateEmpty() => new();
    public List<ConversionOptions> ConversionOptionsList { get; set; } = new();
    public List<IgnoredFieldName> IgnoredFieldNameList { get; set; } = new();
    public List<IgnoredFieldSuffix> IgnoredFieldSuffixList { get; set; } = new();
    public List<IncludeViewsOption> IncludeViewsOptionList { get; set; } = new();
}

public static partial class MetaSchemaToRawDataVaultOptionsInstance
{
    private static readonly MetaSchemaToRawDataVaultOptionsModel _builtIn = CreateBuiltIn();
    public static MetaSchemaToRawDataVaultOptionsModel BuiltIn => _builtIn;

    public static MetaSchemaToRawDataVaultOptionsModel CreateBuiltIn()
    {
        var model = MetaSchemaToRawDataVaultOptionsModel.CreateEmpty();
        var record0 = new ConversionOptions
        {
            Id = "options"
        };
        model.ConversionOptionsList.Add(record0);
        return model;
    }
}