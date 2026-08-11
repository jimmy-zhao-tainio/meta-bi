using Meta.Operations.Domain;

namespace MetaSql;

internal static class MetaSqlCanonicalizer
{
    internal static InMemoryWorkspace Canonicalize(InMemoryWorkspace workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        if (!string.Equals(workspace.Model.Name, "MetaSql", StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"Workspace model must be 'MetaSql', but was '{workspace.Model.Name}'.",
                nameof(workspace));
        }

        var canonical = workspace.Clone();
        NormalizeOptionalText(canonical, "Database", "Collation");

        NormalizeOptionalText(canonical, "TableColumn", "Ordinal");
        NormalizeBoolean(canonical, "TableColumn", "IsNullable", isExplicit: true);
        NormalizeBoolean(canonical, "TableColumn", "IsIdentity", isExplicit: false);
        NormalizeOptionalText(canonical, "TableColumn", "IdentitySeed");
        NormalizeOptionalText(canonical, "TableColumn", "IdentityIncrement");
        NormalizeOptionalText(canonical, "TableColumn", "ExpressionSql");
        NormalizeOptionalText(canonical, "TableColumn", "DefaultExpressionSql");

        NormalizeBoolean(canonical, "PrimaryKey", "IsClustered", isExplicit: false);
        NormalizeBoolean(canonical, "PrimaryKeyColumn", "IsDescending", isExplicit: false);

        NormalizeBoolean(canonical, "Index", "IsUnique", isExplicit: false);
        NormalizeBoolean(canonical, "Index", "IsClustered", isExplicit: false);
        NormalizeOptionalText(canonical, "Index", "FilterSql");
        NormalizeBoolean(canonical, "IndexColumn", "IsDescending", isExplicit: false);
        NormalizeBoolean(canonical, "IndexColumn", "IsIncluded", isExplicit: false);

        NormalizeOptionalText(canonical, "View", "DeployOrdinal");
        NormalizeOptionalText(canonical, "Function", "DeployOrdinal");
        NormalizeOptionalText(canonical, "StoredProcedure", "DeployOrdinal");
        return canonical;
    }

    private static void NormalizeBoolean(
        InMemoryWorkspace workspace,
        string entityName,
        string propertyName,
        bool isExplicit)
    {
        foreach (var record in GetRecords(workspace, entityName))
        {
            if (!record.Values.TryGetValue(propertyName, out var value) || string.IsNullOrWhiteSpace(value))
            {
                SetBoolean(record, propertyName, value: false, isExplicit);
                continue;
            }

            if (string.Equals(value.Trim(), "true", StringComparison.OrdinalIgnoreCase))
            {
                SetBoolean(record, propertyName, value: true, isExplicit);
                continue;
            }

            if (string.Equals(value.Trim(), "false", StringComparison.OrdinalIgnoreCase))
            {
                SetBoolean(record, propertyName, value: false, isExplicit);
                continue;
            }

            throw new InvalidOperationException(
                $"MetaSql {entityName} '{record.Id}' has invalid {propertyName} value '{value}'. Expected true or false.");
        }
    }

    private static void SetBoolean(
        GenericRecord record,
        string propertyName,
        bool value,
        bool isExplicit)
    {
        if (value)
        {
            record.Values[propertyName] = "true";
        }
        else if (isExplicit)
        {
            record.Values[propertyName] = "false";
        }
        else
        {
            record.Values.Remove(propertyName);
        }
    }

    private static void NormalizeOptionalText(
        InMemoryWorkspace workspace,
        string entityName,
        string propertyName)
    {
        foreach (var record in GetRecords(workspace, entityName))
        {
            if (record.Values.TryGetValue(propertyName, out var value) && string.IsNullOrWhiteSpace(value))
            {
                record.Values.Remove(propertyName);
            }
        }
    }

    private static IReadOnlyList<GenericRecord> GetRecords(InMemoryWorkspace workspace, string entityName)
    {
        return workspace.Instance.RecordsByEntity.TryGetValue(entityName, out var records)
            ? records
            : [];
    }
}
