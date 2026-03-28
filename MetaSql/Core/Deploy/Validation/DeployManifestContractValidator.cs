using Meta.Core.Domain;

namespace MetaSql;

/// <summary>
/// Validates deploy-manifest model contract and supported action kinds.
/// </summary>
internal sealed class DeployManifestContractValidator
{
    private static readonly HashSet<string> SupportedManifestEntityNames =
    [
        "DeployManifest",
        "AddTable",
        "DropTable",
        "AddTableColumn",
        "DropTableColumn",
        "TruncateTableColumnData",
        "AlterTableColumn",
        "AddPrimaryKey",
        "DropPrimaryKey",
        "ReplacePrimaryKey",
        "AddForeignKey",
        "DropForeignKey",
        "ReplaceForeignKey",
        "AddIndex",
        "DropIndex",
        "ReplaceIndex",
        "AddSchema",
        "BlockTableDifference",
        "BlockTableColumnDifference",
        "BlockPrimaryKeyDifference",
        "BlockForeignKeyDifference",
        "BlockIndexDifference",
    ];

    public void Validate(Workspace manifestWorkspace)
    {
        ArgumentNullException.ThrowIfNull(manifestWorkspace);

        if (!string.Equals(manifestWorkspace.Model.Name, "MetaSqlDeployManifest", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Deploy manifest workspace must use the MetaSqlDeployManifest model. Actual model: '{manifestWorkspace.Model.Name}'.");
        }

        var declaredEntityNames = manifestWorkspace.Model.Entities
            .Select(row => row.Name)
            .Where(row => !string.IsNullOrWhiteSpace(row))
            .ToHashSet(StringComparer.Ordinal);

        var unsupportedEntityNames = declaredEntityNames
            .Where(row => !SupportedManifestEntityNames.Contains(row))
            .OrderBy(row => row, StringComparer.Ordinal)
            .ToList();
        if (unsupportedEntityNames.Count > 0)
        {
            throw new InvalidOperationException(
                $"Manifest contains unsupported action kind(s): {string.Join(", ", unsupportedEntityNames)}.");
        }

        var missingRequiredEntityNames = SupportedManifestEntityNames
            .Where(row => !declaredEntityNames.Contains(row))
            .OrderBy(row => row, StringComparer.Ordinal)
            .ToList();
        if (missingRequiredEntityNames.Count > 0)
        {
            throw new InvalidOperationException(
                $"Manifest is missing required action kind(s): {string.Join(", ", missingRequiredEntityNames)}.");
        }
    }

    public MetaSqlDeployManifest.DeployManifest RequireSingleManifestRoot(
        MetaSqlDeployManifest.MetaSqlDeployManifestModel manifestModel)
    {
        ArgumentNullException.ThrowIfNull(manifestModel);

        if (manifestModel.DeployManifestList.Count != 1)
        {
            throw new InvalidOperationException(
                $"Deploy manifest workspace must contain exactly one DeployManifest row. Found {manifestModel.DeployManifestList.Count}.");
        }

        return manifestModel.DeployManifestList[0];
    }
}
