#nullable enable
using System;
using System.Collections.Generic;

namespace MetaSqlScript;
public sealed partial class SqlScript
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string SqlText { get; set; } = null !;
}

public sealed partial class MetaSqlScriptModel
{
    public static MetaSqlScriptModel CreateEmpty() => new();
    public List<SqlScript> SqlScriptList { get; set; } = new();
}

public static partial class MetaSqlScriptInstance
{
    private static readonly MetaSqlScriptModel _builtIn = CreateBuiltIn();
    public static MetaSqlScriptModel BuiltIn => _builtIn;

    public static MetaSqlScriptModel CreateBuiltIn()
    {
        var model = MetaSqlScriptModel.CreateEmpty();
        return model;
    }
}