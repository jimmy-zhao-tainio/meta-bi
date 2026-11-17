using Meta.Operations.Domain;

namespace MetaSql;

/// <summary>Limits a manifest to one physical SQL Server schema when explicitly selected.</summary>
public static class MetaSqlDeploymentScope
{
    public static string? ValidateWorkspace(InMemoryWorkspace source, string? schemaName, string role)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (schemaName is null)
        {
            return null;
        }

        var name = schemaName.Trim();
        if (name.Length is 0 or > 128)
        {
            throw new InvalidOperationException("Deployment schema scope must name one SQL Server schema (1–128 characters).");
        }

        var model = new MetaSqlWorkspaceBinding().Read(source);
        var objects = model.TableList.Select(row => (Schema: row.Schema.Name, Name: row.Name))
            .Concat(model.ViewList.Select(row => (Schema: row.Schema.Name, Name: row.Name)))
            .Concat(model.FunctionList.Select(row => (Schema: row.Schema.Name, Name: row.Name)))
            .Concat(model.StoredProcedureList.Select(row => (Schema: row.Schema.Name, Name: row.Name)));
        foreach (var row in objects)
        {
            if (!string.Equals(row.Schema, name, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"{role} object '{row.Schema}.{row.Name}' is outside deployment schema scope '{name}'.");
            }
        }

        // SQL workspaces include the database's implicit dbo schema even when it owns no objects.
        foreach (var schema in model.SchemaList)
        {
            if (schema.Name != name && schema.Name != "dbo")
            {
                throw new InvalidOperationException(
                    $"{role} schema '{schema.Name}' is outside deployment schema scope '{name}'.");
            }
        }

        return name;
    }
}
