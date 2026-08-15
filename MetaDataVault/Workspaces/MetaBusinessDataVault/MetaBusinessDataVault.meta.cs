#nullable enable
using System;
using System.Collections.Generic;

namespace MetaBusinessDataVault;
public sealed partial class BusinessBridge
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public BusinessHub BusinessHub { get; set; } = null !;
}

public sealed partial class BusinessBridgeTraversal
{
    public string Id { get; set; } = null !;
    public BusinessBridge BusinessBridge { get; set; } = null !;
    public BusinessBridgeTraversal? PreviousTraversal { get; set; }
    public BusinessLinkRole SourceRole { get; set; } = null !;
    public BusinessLinkRole TargetRole { get; set; } = null !;
}

public sealed partial class BusinessHierarchicalLink
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public BusinessHub ChildHub { get; set; } = null !;
    public BusinessHub ParentHub { get; set; } = null !;
}

public sealed partial class BusinessHierarchicalLinkSatellite
{
    public string Id { get; set; } = null !;
    public BusinessHierarchicalLink BusinessHierarchicalLink { get; set; } = null !;
    public BusinessSatellite BusinessSatellite { get; set; } = null !;
}

public sealed partial class BusinessHub
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
}

public sealed partial class BusinessHubKeyPart
{
    public string Id { get; set; } = null !;
    public string DataTypeId { get; set; } = null !;
    public string Name { get; set; } = null !;
    public BusinessHub BusinessHub { get; set; } = null !;
    public BusinessHubKeyPart? PreviousKeyPart { get; set; }
}

public sealed partial class BusinessHubKeyPartDataTypeDetail
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string Value { get; set; } = null !;
    public BusinessHubKeyPart BusinessHubKeyPart { get; set; } = null !;
}

public sealed partial class BusinessHubSatellite
{
    public string Id { get; set; } = null !;
    public BusinessHub BusinessHub { get; set; } = null !;
    public BusinessSatellite BusinessSatellite { get; set; } = null !;
}

public sealed partial class BusinessLink
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
}

public sealed partial class BusinessLinkRole
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public BusinessHub BusinessHub { get; set; } = null !;
    public BusinessLink BusinessLink { get; set; } = null !;
}

public sealed partial class BusinessLinkSatellite
{
    public string Id { get; set; } = null !;
    public BusinessLink BusinessLink { get; set; } = null !;
    public BusinessSatellite BusinessSatellite { get; set; } = null !;
}

public sealed partial class BusinessPointInTime
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public BusinessHub BusinessHub { get; set; } = null !;
}

public sealed partial class BusinessPointInTimeHubSatellite
{
    public string Id { get; set; } = null !;
    public BusinessHubSatellite BusinessHubSatellite { get; set; } = null !;
    public BusinessPointInTime BusinessPointInTime { get; set; } = null !;
}

public sealed partial class BusinessPointInTimeLinkSatellite
{
    public string Id { get; set; } = null !;
    public BusinessLinkSatellite BusinessLinkSatellite { get; set; } = null !;
    public BusinessPointInTime BusinessPointInTime { get; set; } = null !;
}

public sealed partial class BusinessPointInTimeStamp
{
    public string Id { get; set; } = null !;
    public string DataTypeId { get; set; } = null !;
    public string Name { get; set; } = null !;
    public BusinessPointInTime BusinessPointInTime { get; set; } = null !;
}

public sealed partial class BusinessPointInTimeStampDataTypeDetail
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string Value { get; set; } = null !;
    public BusinessPointInTimeStamp BusinessPointInTimeStamp { get; set; } = null !;
}

public sealed partial class BusinessReference
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
}

public sealed partial class BusinessReferenceKeyPart
{
    public string Id { get; set; } = null !;
    public string DataTypeId { get; set; } = null !;
    public string Name { get; set; } = null !;
    public BusinessReference BusinessReference { get; set; } = null !;
    public BusinessReferenceKeyPart? PreviousKeyPart { get; set; }
}

public sealed partial class BusinessReferenceKeyPartDataTypeDetail
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string Value { get; set; } = null !;
    public BusinessReferenceKeyPart BusinessReferenceKeyPart { get; set; } = null !;
}

public sealed partial class BusinessReferenceSatellite
{
    public string Id { get; set; } = null !;
    public BusinessReference BusinessReference { get; set; } = null !;
    public BusinessSatellite BusinessSatellite { get; set; } = null !;
}

public sealed partial class BusinessSameAsLink
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
    public BusinessHub EquivalentHub { get; set; } = null !;
    public BusinessHub PrimaryHub { get; set; } = null !;
}

public sealed partial class BusinessSameAsLinkSatellite
{
    public string Id { get; set; } = null !;
    public BusinessSameAsLink BusinessSameAsLink { get; set; } = null !;
    public BusinessSatellite BusinessSatellite { get; set; } = null !;
}

public sealed partial class BusinessSatellite
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
}

public sealed partial class BusinessSatelliteAttribute
{
    public string Id { get; set; } = null !;
    public string DataTypeId { get; set; } = null !;
    public string Name { get; set; } = null !;
    public BusinessSatellite BusinessSatellite { get; set; } = null !;
}

public sealed partial class BusinessSatelliteAttributeDataTypeDetail
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string Value { get; set; } = null !;
    public BusinessSatelliteAttribute BusinessSatelliteAttribute { get; set; } = null !;
}

public sealed partial class MetaBusinessDataVaultModel
{
    public static MetaBusinessDataVaultModel CreateEmpty() => new();
    public List<BusinessBridge> BusinessBridgeList { get; set; } = new();
    public List<BusinessBridgeTraversal> BusinessBridgeTraversalList { get; set; } = new();
    public List<BusinessHierarchicalLink> BusinessHierarchicalLinkList { get; set; } = new();
    public List<BusinessHierarchicalLinkSatellite> BusinessHierarchicalLinkSatelliteList { get; set; } = new();
    public List<BusinessHub> BusinessHubList { get; set; } = new();
    public List<BusinessHubKeyPart> BusinessHubKeyPartList { get; set; } = new();
    public List<BusinessHubKeyPartDataTypeDetail> BusinessHubKeyPartDataTypeDetailList { get; set; } = new();
    public List<BusinessHubSatellite> BusinessHubSatelliteList { get; set; } = new();
    public List<BusinessLink> BusinessLinkList { get; set; } = new();
    public List<BusinessLinkRole> BusinessLinkRoleList { get; set; } = new();
    public List<BusinessLinkSatellite> BusinessLinkSatelliteList { get; set; } = new();
    public List<BusinessPointInTime> BusinessPointInTimeList { get; set; } = new();
    public List<BusinessPointInTimeHubSatellite> BusinessPointInTimeHubSatelliteList { get; set; } = new();
    public List<BusinessPointInTimeLinkSatellite> BusinessPointInTimeLinkSatelliteList { get; set; } = new();
    public List<BusinessPointInTimeStamp> BusinessPointInTimeStampList { get; set; } = new();
    public List<BusinessPointInTimeStampDataTypeDetail> BusinessPointInTimeStampDataTypeDetailList { get; set; } = new();
    public List<BusinessReference> BusinessReferenceList { get; set; } = new();
    public List<BusinessReferenceKeyPart> BusinessReferenceKeyPartList { get; set; } = new();
    public List<BusinessReferenceKeyPartDataTypeDetail> BusinessReferenceKeyPartDataTypeDetailList { get; set; } = new();
    public List<BusinessReferenceSatellite> BusinessReferenceSatelliteList { get; set; } = new();
    public List<BusinessSameAsLink> BusinessSameAsLinkList { get; set; } = new();
    public List<BusinessSameAsLinkSatellite> BusinessSameAsLinkSatelliteList { get; set; } = new();
    public List<BusinessSatellite> BusinessSatelliteList { get; set; } = new();
    public List<BusinessSatelliteAttribute> BusinessSatelliteAttributeList { get; set; } = new();
    public List<BusinessSatelliteAttributeDataTypeDetail> BusinessSatelliteAttributeDataTypeDetailList { get; set; } = new();
}

public static partial class MetaBusinessDataVaultInstance
{
    private static readonly MetaBusinessDataVaultModel _builtIn = CreateBuiltIn();
    public static MetaBusinessDataVaultModel BuiltIn => _builtIn;

    public static MetaBusinessDataVaultModel CreateBuiltIn()
    {
        var model = MetaBusinessDataVaultModel.CreateEmpty();
        return model;
    }
}