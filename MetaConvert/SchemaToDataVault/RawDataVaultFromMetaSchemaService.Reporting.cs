using MRDV = global::MetaRawDataVault;

namespace MetaConvert.SchemaToDataVault;

public sealed partial class RawDataVaultFromMetaSchemaService
{
    private static RawDataVaultFromMetaSchemaReport BuildReport(
        MRDV.MetaRawDataVaultModel target,
        OptionsInput options,
        SchemaToRawDataVaultEvidence evidence)
    {
        var rawHubIds = target.RawHubList
            .Select(hub => hub.Id)
            .ToHashSet(StringComparer.Ordinal);
        var selectedKeysByTable = evidence.SelectedKeys
            .ToDictionary(key => key.TableId, StringComparer.Ordinal);
        var keyAssessmentsByTable = evidence.KeyAssessments
            .ToDictionary(assessment => assessment.TableId, StringComparer.Ordinal);
        var selectedKeyFieldsByTable = evidence.SelectedKeyFields
            .GroupBy(field => field.TableId, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<SelectedKeyFieldEvidence>)group
                    .OrderBy(field => field.KeyFieldNumber)
                    .ThenBy(field => field.KeyFieldId, StringComparer.Ordinal)
                    .ToList(),
                StringComparer.Ordinal);

        var tableReports = evidence.IncludedTables
            .Select(table => BuildTableReport(
                target,
                table,
                selectedKeysByTable,
                selectedKeyFieldsByTable,
                keyAssessmentsByTable))
            .OrderBy(report => report.QualifiedTableName, StringComparer.OrdinalIgnoreCase)
            .ToList();
        var relationshipReports = evidence.IncludedRelationships
            .Select(relationship => BuildRelationshipReport(target, rawHubIds, relationship))
            .OrderBy(
                report => $"{report.RawLinkName} ({report.SourceTableName} -> {report.TargetTableName})",
                StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new RawDataVaultFromMetaSchemaReport(
            new RawDataVaultFromMetaSchemaSummary(
                SourceSystemCount: evidence.IncludedTables
                    .Select(table => table.SystemId)
                    .Distinct(StringComparer.Ordinal)
                    .Count(),
                SourceSchemaCount: evidence.IncludedTables
                    .Select(table => table.SchemaId)
                    .Distinct(StringComparer.Ordinal)
                    .Count(),
                SourceTableCount: evidence.IncludedTables.Count,
                SourceRelationshipCount: evidence.IncludedRelationships.Count,
                RawHubCount: target.RawHubList.Count,
                RawHubKeyPartCount: target.RawHubKeyPartList.Count,
                RawLinkCount: target.RawLinkList.Count,
                RawHubSatelliteCount: target.RawHubSatelliteList.Count,
                RawHubSatelliteAttributeCount: target.RawHubSatelliteAttributeList.Count,
                IgnoredFieldNames: MaterializeOptionList(options.IgnoredFieldNames),
                IgnoredFieldSuffixes: MaterializeOptionList(options.IgnoredFieldSuffixes),
                IncludeViews: options.IncludeViews),
            tableReports,
            relationshipReports);
    }

    private static RawDataVaultFromMetaSchemaTableReport BuildTableReport(
        MRDV.MetaRawDataVaultModel target,
        IncludedTableEvidence table,
        IReadOnlyDictionary<string, SelectedKeyEvidence> selectedKeysByTable,
        IReadOnlyDictionary<string, IReadOnlyList<SelectedKeyFieldEvidence>> selectedKeyFieldsByTable,
        IReadOnlyDictionary<string, KeyAssessmentEvidence> keyAssessmentsByTable)
    {
        var hubId = "rawhub:" + table.TableId;
        var hubCreated = target.RawHubList.Any(hub =>
            string.Equals(hub.Id, hubId, StringComparison.Ordinal));
        selectedKeysByTable.TryGetValue(table.TableId, out var selectedKey);
        selectedKeyFieldsByTable.TryGetValue(table.TableId, out var selectedKeyFields);
        selectedKeyFields ??= [];
        var keyAssessment = keyAssessmentsByTable[table.TableId];

        if (hubCreated != (selectedKey is not null))
        {
            throw new InvalidOperationException(
                $"The weave evidence and produced RawHub disagree for source table '{table.TableId}'.");
        }
        var selectedKeyReport = selectedKey is null
            ? null
            : new RawDataVaultFromMetaSchemaSelectedKeyReport(
                KeyTypeFromPriority(selectedKey.KeyPriority),
                string.IsNullOrWhiteSpace(selectedKey.KeyName) ? null : selectedKey.KeyName,
                selectedKeyFields.Select(field => field.FieldName).ToList());
        var satelliteIds = target.RawHubSatelliteList
            .Where(satellite => string.Equals(satellite.RawHub.Id, hubId, StringComparison.Ordinal))
            .Select(satellite => satellite.Id)
            .ToHashSet(StringComparer.Ordinal);
        var satelliteAttributeCount = target.RawHubSatelliteAttributeList.Count(attribute =>
            satelliteIds.Contains(attribute.RawHubSatellite.Id));

        return new RawDataVaultFromMetaSchemaTableReport(
            QualifiedTableName: string.IsNullOrWhiteSpace(table.SchemaName)
                ? table.TableName
                : table.SchemaName + "." + table.TableName,
            SelectedKey: selectedKeyReport,
            HubCreated: hubCreated,
            SatelliteAttributeCount: satelliteAttributeCount,
            Reason: hubCreated
                ? null
                : ReasonFromAssessment(keyAssessment.Assessment));
    }

    private static RawDataVaultFromMetaSchemaRelationshipReport BuildRelationshipReport(
        MRDV.MetaRawDataVaultModel target,
        ISet<string> rawHubIds,
        IncludedRelationshipEvidence relationship)
    {
        var rawLinkId = "rawlink:" + relationship.RelationshipId;
        var rawLink = target.RawLinkList.FirstOrDefault(link =>
            string.Equals(link.Id, rawLinkId, StringComparison.Ordinal));
        var missingHubs = new List<string>();
        if (!rawHubIds.Contains("rawhub:" + relationship.SourceTableId))
        {
            missingHubs.Add($"source table `{relationship.SourceTableName}` did not materialize to a hub");
        }
        if (!rawHubIds.Contains("rawhub:" + relationship.TargetTableId))
        {
            missingHubs.Add($"target table `{relationship.TargetTableName}` did not materialize to a hub");
        }

        return new RawDataVaultFromMetaSchemaRelationshipReport(
            RawLinkName: rawLink?.Name ?? relationship.StructuralName,
            SourceTableName: relationship.SourceTableName,
            TargetTableName: relationship.TargetTableName,
            LinkCreated: rawLink is not null,
            NameWasDisambiguated: rawLink is not null &&
                                  !string.Equals(
                                      rawLink.Name,
                                      relationship.StructuralName,
                                      StringComparison.Ordinal),
            Reason: rawLink is null && missingHubs.Count > 0
                ? string.Join("; ", missingHubs)
                : null);
    }

    private static string KeyTypeFromPriority(int keyPriority) => keyPriority switch
    {
        0 => "primary",
        1 => "unique",
        2 => "other",
        _ => throw new InvalidOperationException(
            $"The weave returned unsupported selected-key priority '{keyPriority}'.")
    };

    private static string ReasonFromAssessment(TableKeyAssessment assessment) => assessment switch
    {
        TableKeyAssessment.NoModeledKey => "no source key metadata was available",
        TableKeyAssessment.NoModeledKeyFields => "source key metadata contained no key fields",
        TableKeyAssessment.KeyFieldsExcluded => "all source key fields were excluded by explicit ignore options",
        TableKeyAssessment.Selected => throw new InvalidOperationException(
            "The weave reported a selected key for a table that did not materialize to a RawHub."),
        _ => throw new ArgumentOutOfRangeException(nameof(assessment), assessment, null)
    };

    private static IReadOnlyList<string> MaterializeOptionList(IEnumerable<string> values)
        => values
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
            .ThenBy(value => value, StringComparer.Ordinal)
            .ToList();
}
