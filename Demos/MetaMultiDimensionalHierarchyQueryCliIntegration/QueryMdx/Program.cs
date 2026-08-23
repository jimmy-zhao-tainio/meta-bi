using System.Globalization;
using Microsoft.AnalysisServices.AdomdClient;

const string defaultMdx = """
SELECT
  {[Measures].[Sales Amount]} ON COLUMNS,
  NON EMPTY [Date].[Calendar].[Month].MEMBERS ON ROWS
FROM [Commerce]
""";

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: QueryMdx <server> <database> [mdx]");
    return 2;
}

var server = args[0];
var database = args[1];
var mdx = args.Length > 2 ? args[2] : defaultMdx;
var connectionString = $"Data Source={server};Catalog={database};";

using var connection = new AdomdConnection(connectionString);
connection.Open();

using var command = connection.CreateCommand();
command.CommandText = mdx;

using var reader = command.ExecuteReader();
var rowCount = 0;
while (reader.Read())
{
    rowCount++;
    if (rowCount <= 20)
    {
        Console.WriteLine(FormatRow(reader));
    }
}

if (rowCount == 0)
{
    throw new InvalidOperationException("The hierarchy MDX query returned no rows.");
}

Console.WriteLine($"Rows: {rowCount}");
return 0;

static string FormatRow(AdomdDataReader reader)
{
    var values = new string[reader.FieldCount];
    for (var i = 0; i < reader.FieldCount; i++)
    {
        values[i] = Convert.ToString(reader.GetValue(i), CultureInfo.InvariantCulture) ?? string.Empty;
    }

    return string.Join("\t", values);
}
