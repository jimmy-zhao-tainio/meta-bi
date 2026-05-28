using Amo = Microsoft.AnalysisServices;

namespace MetaMultiDimensional.Core.Deploy;

public sealed class MetaMultiDimensionalDropService
{
    public Task<MetaMultiDimensionalDropResult> DropAsync(MetaMultiDimensionalDropRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Server);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.DatabaseName);

        using var server = new Amo.Server();
        server.Connect($"Data Source={request.Server}");
        var database = FindDatabase(server, request.DatabaseName)
            ?? throw new InvalidOperationException($"Multidimensional database '{request.DatabaseName}' does not exist on '{request.Server}'.");

        var databaseName = database.Name;
        database.Drop();
        server.Refresh();

        return Task.FromResult(new MetaMultiDimensionalDropResult
        {
            Server = request.Server,
            DatabaseName = databaseName,
        });
    }

    private static Amo.Database? FindDatabase(Amo.Server server, string databaseName)
    {
        return server.Databases
            .OfType<Amo.Database>()
            .FirstOrDefault(database =>
                string.Equals(database.ID, databaseName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(database.Name, databaseName, StringComparison.OrdinalIgnoreCase));
    }
}
