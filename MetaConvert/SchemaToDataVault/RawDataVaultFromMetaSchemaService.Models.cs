using MS = global::MetaSchema;
using MRDV = global::MetaRawDataVault;

namespace MetaConvert.SchemaToDataVault;

internal sealed record FromMetaSchemaOptions(
    ISet<string> IgnoredFieldNames,
    ISet<string> IgnoredFieldSuffixes,
    bool IncludeViews);

internal sealed record CandidateKeySelection(
    MS.TableKey TableKey,
    IReadOnlyList<MS.TableKeyField> OrderedKeyFields);

internal sealed record TableKeyAssessment(
    IReadOnlyList<CandidateKeySelection> CandidateKeys,
    CandidateKeySelection? SelectedKey,
    string SkipReason);

internal sealed record TableMaterializationReportRow(
    MS.Table Table,
    TableKeyAssessment? KeyAssessment,
    bool HubCreated,
    int SatelliteAttributeCount);

internal sealed record RelationshipMaterializationReportRow(
    MS.TableRelationship Relationship,
    MS.Table SourceTable,
    MS.Table TargetTable,
    string? RawLinkName,
    bool LinkCreated,
    string? SkipReason);

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

internal sealed class SourceIndex
{
    public required IReadOnlyList<MS.System> IncludedSystems { get; init; }
    public required IReadOnlyList<MS.Schema> IncludedSchemas { get; init; }
    public required IReadOnlyList<MS.Table> IncludedTables { get; init; }
    public required IReadOnlyList<MS.Field> IncludedFields { get; init; }
    public required IReadOnlyList<MS.FieldDataTypeDetail> IncludedFieldDetails { get; init; }
    public required IReadOnlyList<MS.TableRelationship> IncludedRelationships { get; init; }
    public required IReadOnlyList<MS.TableRelationshipField> IncludedRelationshipFields { get; init; }
    public required IReadOnlyDictionary<string, MS.Schema> SchemaById { get; init; }
    public required IReadOnlyDictionary<string, MS.Table> TableById { get; init; }
    public required IReadOnlyDictionary<string, MS.Field> FieldById { get; init; }
    public required IReadOnlyDictionary<string, IReadOnlyList<MS.Field>> FieldsByTableId { get; init; }
    public required IReadOnlyDictionary<string, IReadOnlyList<MS.TableRelationshipField>> RelationshipFieldsByRelationshipId { get; init; }
    public required ISet<string> RelationshipSourceFieldIds { get; init; }
}

internal sealed class FromMetaSchemaDraft
{
    public List<MRDV.SourceSystem> SourceSystems { get; } = new();
    public List<MRDV.SourceSchema> SourceSchemas { get; } = new();
    public List<MRDV.SourceTable> SourceTables { get; } = new();
    public List<MRDV.SourceField> SourceFields { get; } = new();
    public List<MRDV.SourceFieldDataTypeDetail> SourceFieldDetails { get; } = new();
    public List<MRDV.SourceTableRelationship> SourceRelationships { get; } = new();
    public List<MRDV.SourceTableRelationshipField> SourceRelationshipFields { get; } = new();
    public List<MRDV.RawHub> RawHubs { get; } = new();
    public List<MRDV.RawHubKeyPart> RawHubKeyParts { get; } = new();
    public List<MRDV.RawHubSatellite> RawHubSatellites { get; } = new();
    public List<MRDV.RawHubSatelliteAttribute> RawHubSatelliteAttributes { get; } = new();
    public List<MRDV.RawLink> RawLinks { get; } = new();
    public List<MRDV.RawLinkHub> RawLinkHubs { get; } = new();
    public List<MRDV.RawLinkSatellite> RawLinkSatellites { get; } = new();
    public List<MRDV.RawLinkSatelliteAttribute> RawLinkSatelliteAttributes { get; } = new();

    public Dictionary<string, MRDV.SourceSystem> SourceSystemsById { get; } = new(StringComparer.Ordinal);
    public Dictionary<string, MRDV.SourceSchema> SourceSchemasById { get; } = new(StringComparer.Ordinal);
    public Dictionary<string, MRDV.SourceTable> SourceTablesById { get; } = new(StringComparer.Ordinal);
    public Dictionary<string, MRDV.SourceField> SourceFieldsById { get; } = new(StringComparer.Ordinal);
    public Dictionary<string, MRDV.SourceTableRelationship> SourceRelationshipsById { get; } = new(StringComparer.Ordinal);
    public Dictionary<string, MRDV.RawHub> RawHubsById { get; } = new(StringComparer.Ordinal);
    public Dictionary<string, string> RawHubIdsBySourceTableId { get; } = new(StringComparer.Ordinal);
}
