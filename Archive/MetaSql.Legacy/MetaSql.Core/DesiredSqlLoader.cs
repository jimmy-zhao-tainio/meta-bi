using System.Text.RegularExpressions;

namespace MetaSql.Core;

public sealed class DesiredSqlLoader
{
    public DesiredSqlModel LoadFromDirectory(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            throw new InvalidOperationException("preflight requires --desired-sql <path>.");
        }

        var fullPath = Path.GetFullPath(directoryPath);
        if (!Directory.Exists(fullPath))
        {
            throw new InvalidOperationException($"desired sql directory '{fullPath}' does not exist.");
        }

        var sqlFiles = Directory.EnumerateFiles(fullPath, "*.sql", SearchOption.AllDirectories)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ThenBy(path => path, StringComparer.Ordinal)
            .ToList();
        if (sqlFiles.Count == 0)
        {
            throw new InvalidOperationException($"desired sql directory '{fullPath}' does not contain any .sql files.");
        }

        var tables = new Dictionary<string, DesiredTable>(StringComparer.OrdinalIgnoreCase);
        foreach (var sqlFile in sqlFiles)
        {
            var script = File.ReadAllText(sqlFile);
            var batches = SplitBatches(script);
            foreach (var batch in batches)
            {
                if (TryParseCreateTable(batch, out var table))
                {
                    tables[table.ObjectKey] = table;
                    continue;
                }

                if (TryParseAlterConstraint(batch, out var tableKey, out var constraint))
                {
                    if (!tables.TryGetValue(tableKey, out var existingTable))
                    {
                        throw new InvalidOperationException(
                            $"sql file '{sqlFile}' contains ALTER TABLE constraint for '{tableKey}' before its CREATE TABLE was seen.");
                    }

                    tables[tableKey] = existingTable with
                    {
                        AlterConstraints = existingTable.AlterConstraints.Concat([constraint]).ToList()
                    };
                    continue;
                }

                if (TryParseCreateIndex(batch, out tableKey, out var index))
                {
                    if (!tables.TryGetValue(tableKey, out var existingTable))
                    {
                        throw new InvalidOperationException(
                            $"sql file '{sqlFile}' contains CREATE INDEX for '{tableKey}' before its CREATE TABLE was seen.");
                    }

                    tables[tableKey] = existingTable with
                    {
                        Indexes = existingTable.Indexes.Concat([index]).ToList()
                    };
                }
            }
        }

        return new DesiredSqlModel(tables.Values
            .OrderBy(item => item.SchemaName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(item => item.SchemaName, StringComparer.Ordinal)
            .ThenBy(item => item.TableName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(item => item.TableName, StringComparer.Ordinal)
            .ToList());
    }

    private static IReadOnlyList<string> SplitBatches(string script)
    {
        return Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)
            .Select(RemoveCommentOnlyLines)
            .Select(item => item.Trim())
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .ToList();
    }

    private static bool TryParseCreateTable(string batch, out DesiredTable table)
    {
        var match = Regex.Match(
            batch,
            @"^CREATE\s+TABLE\s+\[(?<schema>[^\]]+)\]\.\[(?<table>[^\]]+)\]\s*\((?<body>.*)\)\s*;$",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        if (!match.Success)
        {
            table = null!;
            return false;
        }

        var schemaName = match.Groups["schema"].Value;
        var tableName = match.Groups["table"].Value;
        var body = match.Groups["body"].Value;
        var items = SplitTopLevelCommaList(body);
        var columns = new List<DesiredColumn>();
        var constraints = new List<DesiredConstraint>();
        foreach (var rawItem in items)
        {
            var item = rawItem.Trim();
            if (item.StartsWith("CONSTRAINT ", StringComparison.OrdinalIgnoreCase))
            {
                constraints.Add(ParseInlineConstraint(schemaName, tableName, item));
                continue;
            }

            columns.Add(ParseColumn(item));
        }

        table = new DesiredTable(
            schemaName,
            tableName,
            NormalizeSql(batch),
            columns,
            constraints,
            [],
            []);
        return true;
    }

    private static bool TryParseAlterConstraint(string batch, out string tableKey, out DesiredConstraint constraint)
    {
        var match = Regex.Match(
            batch,
            @"^ALTER\s+TABLE\s+\[(?<schema>[^\]]+)\]\.\[(?<table>[^\]]+)\]\s+(WITH\s+CHECK\s+)?ADD\s+CONSTRAINT\s+\[(?<name>[^\]]+)\]\s+(?<body>.*);$",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        if (!match.Success)
        {
            tableKey = string.Empty;
            constraint = null!;
            return false;
        }

        var schemaName = match.Groups["schema"].Value;
        var tableName = match.Groups["table"].Value;
        tableKey = SqlObjectName.Format(schemaName, tableName);
        var body = match.Groups["body"].Value.Trim();
        var referencedMatch = Regex.Match(
            body,
            @"REFERENCES\s+\[(?<schema>[^\]]+)\]\.\[(?<table>[^\]]+)\]",
            RegexOptions.IgnoreCase);
        constraint = new DesiredConstraint(
            match.Groups["name"].Value,
            InferConstraintKind(body),
            NormalizeSql(batch),
            referencedMatch.Success ? referencedMatch.Groups["schema"].Value : null,
            referencedMatch.Success ? referencedMatch.Groups["table"].Value : null);
        return true;
    }

    private static bool TryParseCreateIndex(string batch, out string tableKey, out DesiredIndex index)
    {
        var match = Regex.Match(
            batch,
            @"^CREATE\s+(UNIQUE\s+)?INDEX\s+\[(?<name>[^\]]+)\]\s+ON\s+\[(?<schema>[^\]]+)\]\.\[(?<table>[^\]]+)\]",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        if (!match.Success)
        {
            tableKey = string.Empty;
            index = null!;
            return false;
        }

        tableKey = SqlObjectName.Format(match.Groups["schema"].Value, match.Groups["table"].Value);
        index = new DesiredIndex(match.Groups["name"].Value, NormalizeSql(batch));
        return true;
    }

    private static DesiredConstraint ParseInlineConstraint(string schemaName, string tableName, string item)
    {
        var match = Regex.Match(
            item,
            @"^CONSTRAINT\s+\[(?<name>[^\]]+)\]\s+(?<body>.*)$",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        if (!match.Success)
        {
            throw new InvalidOperationException($"unsupported inline constraint fragment '{item}'.");
        }

        var name = match.Groups["name"].Value;
        var body = match.Groups["body"].Value.Trim();
        return new DesiredConstraint(
            name,
            InferConstraintKind(body),
            NormalizeSql($"ALTER TABLE [{schemaName}].[{tableName}] ADD CONSTRAINT [{name}] {body};"));
    }

    private static DesiredColumn ParseColumn(string item)
    {
        var match = Regex.Match(item, @"^\[(?<name>[^\]]+)\]\s+(?<definition>.+)$", RegexOptions.Singleline);
        if (!match.Success)
        {
            throw new InvalidOperationException($"unsupported column fragment '{item}'.");
        }

        var definition = NormalizeInnerWhitespace(match.Groups["definition"].Value.Trim());
        string typeSql;
        bool isNullable;
        if (definition.EndsWith("NOT NULL", StringComparison.OrdinalIgnoreCase))
        {
            isNullable = false;
            typeSql = definition[..^"NOT NULL".Length].TrimEnd();
        }
        else if (definition.EndsWith("NULL", StringComparison.OrdinalIgnoreCase))
        {
            isNullable = true;
            typeSql = definition[..^"NULL".Length].TrimEnd();
        }
        else
        {
            isNullable = true;
            typeSql = definition;
        }

        return new DesiredColumn(
            match.Groups["name"].Value,
            definition,
            NormalizeToken(typeSql),
            isNullable);
    }

    private static IReadOnlyList<string> SplitTopLevelCommaList(string value)
    {
        var items = new List<string>();
        var start = 0;
        var depth = 0;
        for (var i = 0; i < value.Length; i++)
        {
            switch (value[i])
            {
                case '(':
                    depth++;
                    break;
                case ')':
                    depth--;
                    break;
                case ',' when depth == 0:
                    items.Add(value[start..i]);
                    start = i + 1;
                    break;
            }
        }

        items.Add(value[start..]);
        return items;
    }

    private static string NormalizeSql(string sql)
    {
        return string.Join(Environment.NewLine, sql
            .Split(["\r\n", "\n"], StringSplitOptions.None)
            .Select(line => line.TrimEnd()))
            .Trim();
    }

    private static string NormalizeInnerWhitespace(string value)
    {
        return Regex.Replace(value, @"\s+", " ").Trim();
    }

    private static string RemoveCommentOnlyLines(string value)
    {
        return string.Join(
            Environment.NewLine,
            value.Split(["\r\n", "\n"], StringSplitOptions.None)
                .Where(line => !line.TrimStart().StartsWith("--", StringComparison.Ordinal)));
    }

    private static string NormalizeToken(string value)
    {
        return Regex.Replace(value, @"\s+", string.Empty).ToLowerInvariant();
    }

    private static string InferConstraintKind(string body)
    {
        if (body.Contains("PRIMARY KEY", StringComparison.OrdinalIgnoreCase))
        {
            return "PRIMARY KEY";
        }

        if (body.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase))
        {
            return "FOREIGN KEY";
        }

        if (body.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
        {
            return "UNIQUE";
        }

        return "CONSTRAINT";
    }
}
