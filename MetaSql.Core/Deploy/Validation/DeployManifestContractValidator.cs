using System.Xml.Linq;

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
        "BlockTableDifference",
        "BlockTableColumnDifference",
        "BlockPrimaryKeyDifference",
        "BlockForeignKeyDifference",
        "BlockIndexDifference",
    ];

    public void Validate(string manifestWorkspacePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(manifestWorkspacePath);

        var modelPath = Path.Combine(manifestWorkspacePath, "metadata", "model.xml");
        if (!File.Exists(modelPath))
        {
            throw new InvalidOperationException(
                $"Deploy manifest metadata model was not found at '{modelPath}'.");
        }

        XDocument document;
        try
        {
            document = XDocument.Load(modelPath);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Deploy manifest metadata model could not be parsed at '{modelPath}'.",
                ex);
        }

        var root = document.Root;
        if (root is null)
        {
            throw new InvalidOperationException(
                $"Deploy manifest metadata model is empty at '{modelPath}'.");
        }

        var modelName = GetAttribute(root, "name");
        if (!string.Equals(modelName, "MetaSqlDeployManifest", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Deploy manifest model must be 'MetaSqlDeployManifest'. Actual model: '{modelName}'.");
        }

        var entityList = root.Element("EntityList");
        if (entityList is null)
        {
            throw new InvalidOperationException(
                "Deploy manifest metadata model is missing EntityList.");
        }

        var declaredEntityNames = entityList
            .Elements("Entity")
            .Select(row => GetAttribute(row, "name"))
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

    private static string GetAttribute(XElement element, string attributeName)
    {
        var value = (string?)element.Attribute(attributeName);
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var alternateName = char.ToUpperInvariant(attributeName[0]) + attributeName[1..];
        return (string?)element.Attribute(alternateName) ?? string.Empty;
    }
}
