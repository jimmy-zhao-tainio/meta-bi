using System.Globalization;
using Microsoft.AnalysisServices.AdomdClient;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: TabularProof <server> <database>");
    return 2;
}

var server = args[0];
var database = args[1];
var dax = """
EVALUATE
ROW(
    "FactRows", COUNTROWS('Fact Sales Order Line'),
    "SalesAmount", [Sales Amount],
    "OrderQuantity", [Order Quantity]
)
""";

using var connection = new AdomdConnection($"Data Source={server};Catalog={database};");
connection.Open();

using var command = connection.CreateCommand();
command.CommandText = dax;

using var reader = command.ExecuteReader();
if (!reader.Read())
{
    throw new InvalidOperationException("The Tabular DAX proof returned no rows.");
}

var factRows = Convert.ToInt64(reader.GetValue(0), CultureInfo.InvariantCulture);
var salesAmount = Convert.ToDecimal(reader.GetValue(1), CultureInfo.InvariantCulture);
var orderQuantity = Convert.ToInt64(reader.GetValue(2), CultureInfo.InvariantCulture);

Console.WriteLine($"FactRows={factRows.ToString(CultureInfo.InvariantCulture)}");
Console.WriteLine($"SalesAmount={salesAmount.ToString(CultureInfo.InvariantCulture)}");
Console.WriteLine($"OrderQuantity={orderQuantity.ToString(CultureInfo.InvariantCulture)}");

if (factRows <= 0)
{
    throw new InvalidOperationException("The Tabular fact row count was zero.");
}

if (salesAmount <= 0)
{
    throw new InvalidOperationException("The Tabular sales amount was zero.");
}

return 0;
