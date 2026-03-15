using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using MetaSql.Core;

namespace MetaSql.App;

public sealed class SqlServerScriptExecutor
{
    public void ExecuteScripts(string connectionString, IEnumerable<string> scriptPaths)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        ArgumentNullException.ThrowIfNull(scriptPaths);

        foreach (var scriptPath in scriptPaths
                     .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(item => item, StringComparer.Ordinal))
        {
            ExecuteSqlScript(connectionString, File.ReadAllText(scriptPath));
        }
    }

    public void ExecutePlan(string connectionString, SqlServerPreflightPlan plan)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        ArgumentNullException.ThrowIfNull(plan);

        foreach (var sql in plan.DropTables)
        {
            ExecuteSqlScript(connectionString, sql);
        }

        foreach (var sql in plan.AddTables)
        {
            ExecuteSqlScript(connectionString, sql);
        }

        foreach (var sql in plan.AddColumns)
        {
            ExecuteSqlScript(connectionString, sql);
        }

        foreach (var sql in plan.AddIndexes)
        {
            ExecuteSqlScript(connectionString, sql);
        }

        foreach (var sql in plan.AddConstraints)
        {
            ExecuteSqlScript(connectionString, sql);
        }
    }

    private static void ExecuteSqlScript(string connectionString, string script)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        foreach (var batch in SplitBatches(script))
        {
            using var command = new SqlCommand(batch, connection);
            command.ExecuteNonQuery();
        }
    }

    private static IReadOnlyList<string> SplitBatches(string script)
    {
        // Temporary and intentionally narrow: execute only plain GO-separated batches.
        return Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)
            .Select(item => item.Trim())
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .ToList();
    }
}
