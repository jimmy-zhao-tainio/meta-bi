namespace MetaConvert.SchemaToDataVault;

public sealed record RawDataVaultFromMetaSchemaReport(
    RawDataVaultFromMetaSchemaSummary Summary,
    IReadOnlyList<RawDataVaultFromMetaSchemaTableReport> Tables,
    IReadOnlyList<RawDataVaultFromMetaSchemaRelationshipReport> Relationships);

public sealed record RawDataVaultFromMetaSchemaSummary(
    int SourceSystemCount,
    int SourceSchemaCount,
    int SourceTableCount,
    int SourceRelationshipCount,
    int RawHubCount,
    int RawHubKeyPartCount,
    int RawLinkCount,
    int RawHubSatelliteCount,
    int RawHubSatelliteAttributeCount,
    IReadOnlyList<string> IgnoredFieldNames,
    IReadOnlyList<string> IgnoredFieldSuffixes,
    bool IncludeViews);

public sealed record RawDataVaultFromMetaSchemaSelectedKeyReport(
    string KeyType,
    string? KeyName,
    IReadOnlyList<string> FieldNames);

public sealed record RawDataVaultFromMetaSchemaTableReport(
    string QualifiedTableName,
    RawDataVaultFromMetaSchemaSelectedKeyReport? SelectedKey,
    bool HubCreated,
    int SatelliteAttributeCount,
    string? Reason);

public sealed record RawDataVaultFromMetaSchemaRelationshipReport(
    string RawLinkName,
    string SourceTableName,
    string TargetTableName,
    bool LinkCreated,
    bool NameWasDisambiguated,
    string? Reason);
