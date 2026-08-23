#nullable enable
using System;
using System.Collections.Generic;

namespace MetaRawDataVault;
public sealed partial class Field
{
    public string Id { get; set; } = null !;
    public string DataTypeId { get; set; } = null !;
    public string Name { get; set; } = null !;
}

public sealed partial class FieldDataTypeDetail
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string Value { get; set; } = null !;
    public Field Field { get; set; } = null !;
}

public sealed partial class RawHub
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
}

public sealed partial class RawHubKeyPart
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public Field Field { get; set; } = null !;
    public RawHub RawHub { get; set; } = null !;
}

public sealed partial class RawHubSatellite
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string SatelliteKind { get; set; } = null !;
    public RawHub RawHub { get; set; } = null !;
}

public sealed partial class RawHubSatelliteAttribute
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public Field Field { get; set; } = null !;
    public RawHubSatellite RawHubSatellite { get; set; } = null !;
}

public sealed partial class RawLink
{
    public string Id { get; set; } = null !;
    public string LinkKind { get; set; } = null !;
    public string Name { get; set; } = null !;
}

public sealed partial class RawLinkRole
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public RawHub RawHub { get; set; } = null !;
    public RawLink RawLink { get; set; } = null !;
}

public sealed partial class RawLinkSatellite
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string SatelliteKind { get; set; } = null !;
    public RawLink RawLink { get; set; } = null !;
}

public sealed partial class RawLinkSatelliteAttribute
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public Field Field { get; set; } = null !;
    public RawLinkSatellite RawLinkSatellite { get; set; } = null !;
}

public sealed partial class MetaRawDataVaultModel
{
    public static MetaRawDataVaultModel CreateEmpty() => new();
    public List<Field> FieldList { get; set; } = new();
    public List<FieldDataTypeDetail> FieldDataTypeDetailList { get; set; } = new();
    public List<RawHub> RawHubList { get; set; } = new();
    public List<RawHubKeyPart> RawHubKeyPartList { get; set; } = new();
    public List<RawHubSatellite> RawHubSatelliteList { get; set; } = new();
    public List<RawHubSatelliteAttribute> RawHubSatelliteAttributeList { get; set; } = new();
    public List<RawLink> RawLinkList { get; set; } = new();
    public List<RawLinkRole> RawLinkRoleList { get; set; } = new();
    public List<RawLinkSatellite> RawLinkSatelliteList { get; set; } = new();
    public List<RawLinkSatelliteAttribute> RawLinkSatelliteAttributeList { get; set; } = new();
}

public static partial class MetaRawDataVaultInstance
{
    private static readonly MetaRawDataVaultModel _builtIn = CreateBuiltIn();
    public static MetaRawDataVaultModel BuiltIn => _builtIn;

    public static MetaRawDataVaultModel CreateBuiltIn()
    {
        var model = MetaRawDataVaultModel.CreateEmpty();
        return model;
    }
}