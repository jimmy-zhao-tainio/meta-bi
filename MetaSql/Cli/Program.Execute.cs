using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Text;
using Microsoft.Data.SqlClient;
using Meta.Core.Connections;

internal static partial class Program
{
    private static async Task<int> RunExecuteAsync(string[] args)
    {
        if (args.Length > 1 && IsHelpToken(args[1]))
        {
            PrintCommandHelp("execute");
            return 0;
        }

        var parse = ParseExecuteArgs(args, startIndex: 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("execute"));
        }

        try
        {
            var connectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(parse.ConnectionEnvironmentVariableName);
            var sql = !string.IsNullOrWhiteSpace(parse.FilePath)
                ? await File.ReadAllTextAsync(parse.FilePath).ConfigureAwait(false)
                : parse.Query;

            sql = ApplySqlCmdVariables(sql, parse.Variables);
            var batches = SplitSqlBatches(sql);
            if (batches.Count == 0)
            {
                return Fail("SQL input did not contain any executable batches.", HelpCommand("execute"));
            }

            var executedBatchCount = await ExecuteSqlBatchesAsync(
                    connectionString,
                    batches,
                    parse.TimeoutSeconds,
                    parse.Quiet,
                    CancellationToken.None)
                .ConfigureAwait(false);

            if (!parse.Quiet)
            {
                Presenter.WriteOk("Executed SQL", ("Batches", executedBatchCount.ToString(CultureInfo.InvariantCulture)));
            }

            return 0;
        }
        catch (Exception exception)
        {
            return Fail(
                exception.Message,
                HelpCommand("execute"),
                exitCode: 1,
                details: FormatInnerExceptionMessages(exception));
        }
    }

    private static (
        bool Ok,
        string ConnectionEnvironmentVariableName,
        string FilePath,
        string Query,
        IReadOnlyDictionary<string, string> Variables,
        int TimeoutSeconds,
        bool Quiet,
        string ErrorMessage) ParseExecuteArgs(string[] args, int startIndex)
    {
        var connectionEnvironmentVariableName = string.Empty;
        var filePath = string.Empty;
        var query = string.Empty;
        var variables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var timeoutSeconds = 0;
        var quiet = false;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--connection-env", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return ExecuteParseFailure("missing value for --connection-env.");
                if (!string.IsNullOrWhiteSpace(connectionEnvironmentVariableName)) return ExecuteParseFailure("--connection-env can only be provided once.");
                connectionEnvironmentVariableName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--file", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return ExecuteParseFailure("missing value for --file.");
                if (!string.IsNullOrWhiteSpace(filePath)) return ExecuteParseFailure("--file can only be provided once.");
                filePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--query", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return ExecuteParseFailure("missing value for --query.");
                if (!string.IsNullOrWhiteSpace(query)) return ExecuteParseFailure("--query can only be provided once.");
                query = args[++i];
                continue;
            }

            if (string.Equals(arg, "--var", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return ExecuteParseFailure("missing value for --var.");
                var value = args[++i];
                var equalsIndex = value.IndexOf('=');
                if (equalsIndex <= 0)
                {
                    return ExecuteParseFailure($"invalid --var '{value}'. Expected NAME=value.");
                }

                var name = value[..equalsIndex].Trim();
                var variableValue = value[(equalsIndex + 1)..];
                if (string.IsNullOrWhiteSpace(name))
                {
                    return ExecuteParseFailure($"invalid --var '{value}'. Variable name cannot be empty.");
                }

                variables[name] = variableValue;
                continue;
            }

            if (string.Equals(arg, "--timeout-seconds", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return ExecuteParseFailure("missing value for --timeout-seconds.");
                if (!int.TryParse(args[++i], NumberStyles.None, CultureInfo.InvariantCulture, out timeoutSeconds) || timeoutSeconds < 0)
                {
                    return ExecuteParseFailure("--timeout-seconds must be a non-negative integer.");
                }

                continue;
            }

            if (string.Equals(arg, "--quiet", StringComparison.OrdinalIgnoreCase))
            {
                quiet = true;
                continue;
            }

            return ExecuteParseFailure($"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(connectionEnvironmentVariableName)) return ExecuteParseFailure("missing required option --connection-env <name>.");
        if (string.IsNullOrWhiteSpace(filePath) == string.IsNullOrWhiteSpace(query)) return ExecuteParseFailure("provide exactly one of --file <path> or --query <sql>.");
        if (!string.IsNullOrWhiteSpace(filePath) && !File.Exists(filePath)) return ExecuteParseFailure($"SQL file '{filePath}' was not found.");

        return (
            true,
            connectionEnvironmentVariableName,
            filePath,
            query,
            new ReadOnlyDictionary<string, string>(variables),
            timeoutSeconds,
            quiet,
            string.Empty);

        (
            bool Ok,
            string ConnectionEnvironmentVariableName,
            string FilePath,
            string Query,
            IReadOnlyDictionary<string, string> Variables,
            int TimeoutSeconds,
            bool Quiet,
            string ErrorMessage) ExecuteParseFailure(string message)
        {
            return (false, string.Empty, string.Empty, string.Empty, new ReadOnlyDictionary<string, string>(new Dictionary<string, string>()), 0, false, message);
        }
    }

    private static string ApplySqlCmdVariables(string sql, IReadOnlyDictionary<string, string> variables)
    {
        foreach (var (name, value) in variables)
        {
            sql = sql.Replace("$(" + name + ")", value, StringComparison.OrdinalIgnoreCase);
        }

        return sql;
    }

    private static IReadOnlyList<string> SplitSqlBatches(string sql)
    {
        var batches = new List<string>();
        var builder = new StringBuilder();
        using var reader = new StringReader(sql);

        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            if (TryParseGoBatchSeparator(line, out var repeatCount))
            {
                AddBatch(builder.ToString(), repeatCount);
                builder.Clear();
                continue;
            }

            builder.AppendLine(line);
        }

        AddBatch(builder.ToString(), repeatCount: 1);
        return batches;

        void AddBatch(string batchSql, int repeatCount)
        {
            if (string.IsNullOrWhiteSpace(batchSql))
            {
                return;
            }

            for (var i = 0; i < repeatCount; i++)
            {
                batches.Add(batchSql);
            }
        }
    }

    private static bool TryParseGoBatchSeparator(string line, out int repeatCount)
    {
        repeatCount = 1;
        var trimmed = line.Trim();
        if (trimmed.Length == 0)
        {
            return false;
        }

        var pieces = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (pieces.Length is < 1 or > 2)
        {
            return false;
        }

        if (!string.Equals(pieces[0], "GO", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (pieces.Length == 2 &&
            (!int.TryParse(pieces[1], NumberStyles.None, CultureInfo.InvariantCulture, out repeatCount) || repeatCount < 1))
        {
            throw new InvalidOperationException($"Invalid GO repeat count '{pieces[1]}'.");
        }

        return true;
    }

    private static async Task<int> ExecuteSqlBatchesAsync(
        string connectionString,
        IReadOnlyList<string> batches,
        int timeoutSeconds,
        bool quiet,
        CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(connectionString);
        connection.InfoMessage += (_, eventArgs) =>
        {
            if (!quiet)
            {
                foreach (SqlError error in eventArgs.Errors)
                {
                    Console.WriteLine(error.Message);
                }
            }
        };

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        for (var i = 0; i < batches.Count; i++)
        {
            await using var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = batches[i];
            command.CommandTimeout = timeoutSeconds;

            try
            {
                await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                await RenderResultSetsAsync(reader, quiet, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"SQL execution failed at batch {i + 1}.", exception);
            }
        }

        return batches.Count;
    }

    private static async Task RenderResultSetsAsync(SqlDataReader reader, bool quiet, CancellationToken cancellationToken)
    {
        if (quiet)
        {
            do
            {
                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                }
            }
            while (await reader.NextResultAsync(cancellationToken).ConfigureAwait(false));

            return;
        }

        var resultSetIndex = 0;
        do
        {
            if (reader.FieldCount <= 0)
            {
                continue;
            }

            resultSetIndex++;
            Console.WriteLine();
            Console.WriteLine($"Result set {resultSetIndex}");
            Console.WriteLine(string.Join(" | ", Enumerable.Range(0, reader.FieldCount).Select(reader.GetName)));

            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                var values = new string[reader.FieldCount];
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    values[i] = FormatSqlValue(reader.GetValue(i));
                }

                Console.WriteLine(string.Join(" | ", values));
            }
        }
        while (await reader.NextResultAsync(cancellationToken).ConfigureAwait(false));
    }

    private static string FormatSqlValue(object value)
    {
        return value switch
        {
            DBNull => "NULL",
            byte[] bytes => "0x" + Convert.ToHexString(bytes),
            DateTime dateTime => dateTime.ToString("yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture),
            DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz", CultureInfo.InvariantCulture),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture) ?? string.Empty,
            _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty,
        };
    }

}
