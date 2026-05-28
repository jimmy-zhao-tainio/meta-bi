using System.Text.RegularExpressions;

namespace MetaSql;

internal sealed class SqlServerModuleSqlRenderer
{
    private static readonly Regex GoBatchSeparatorPattern = new(
        @"^\s*GO(?:\s+\d+)?\s*$",
        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);

    public string BuildDropViewSql(View view)
    {
        return $"DROP VIEW {FormatModuleName(view.Schema, view.Name)};";
    }

    public string BuildDeployViewSql(View view)
    {
        return NormalizeModuleDefinition(view.DefinitionSql, "VIEW", view.Id);
    }

    public string BuildDropStoredProcedureSql(StoredProcedure storedProcedure)
    {
        return $"DROP PROCEDURE {FormatModuleName(storedProcedure.Schema, storedProcedure.Name)};";
    }

    public string BuildDeployStoredProcedureSql(StoredProcedure storedProcedure)
    {
        return NormalizeModuleDefinition(storedProcedure.DefinitionSql, "PROCEDURE", storedProcedure.Id);
    }

    private static string NormalizeModuleDefinition(string definitionSql, string expectedModuleKeyword, string moduleId)
    {
        if (string.IsNullOrWhiteSpace(definitionSql))
        {
            throw new InvalidOperationException(
                $"{expectedModuleKeyword} '{moduleId}' has empty DefinitionSql.");
        }

        var trimmed = definitionSql.Trim();
        if (GoBatchSeparatorPattern.IsMatch(trimmed))
        {
            throw new InvalidOperationException(
                $"{expectedModuleKeyword} '{moduleId}' DefinitionSql must not contain GO batch separators.");
        }

        var moduleKeywordPattern = string.Equals(expectedModuleKeyword, "PROCEDURE", StringComparison.Ordinal)
            ? "(?:PROCEDURE|PROC)"
            : Regex.Escape(expectedModuleKeyword);
        var prefixPattern = @"\A\s*(?:CREATE\s+(?:OR\s+ALTER\s+)?|ALTER\s+)" + moduleKeywordPattern + @"\b";
        if (!Regex.IsMatch(trimmed, prefixPattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            throw new InvalidOperationException(
                $"{expectedModuleKeyword} '{moduleId}' DefinitionSql must start with CREATE, CREATE OR ALTER, or ALTER {expectedModuleKeyword}.");
        }

        return trimmed;
    }

    private static string FormatModuleName(Schema schema, string name)
    {
        if (schema is null)
        {
            throw new InvalidOperationException($"SQL module '{name}' is missing Schema relationship.");
        }

        return $"{SqlServerRenderingSupport.EscapeSqlIdentifier(schema.Name)}.{SqlServerRenderingSupport.EscapeSqlIdentifier(name)}";
    }
}
