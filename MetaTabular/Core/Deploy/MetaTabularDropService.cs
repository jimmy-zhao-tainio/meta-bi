using Tom = Microsoft.AnalysisServices.Tabular;

namespace MetaTabular.Core.Deploy;

public sealed class MetaTabularDropService
{
    public Task<MetaTabularDropResult> DropAsync(MetaTabularDropRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Server);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.DatabaseName);

        using var server = new Tom.Server();
        server.Connect($"Data Source={request.Server}");
        var database = FindDatabase(server, request.DatabaseName)
            ?? throw new InvalidOperationException($"Tabular database '{request.DatabaseName}' does not exist on '{request.Server}'.");

        var databaseName = database.Name;
        database.Drop();
        server.Refresh();

        return Task.FromResult(new MetaTabularDropResult
        {
            Server = request.Server,
            DatabaseName = databaseName,
        });
    }

    private static Tom.Database? FindDatabase(Tom.Server server, string databaseName)
    {
        return server.Databases
            .OfType<Tom.Database>()
            .FirstOrDefault(database =>
                string.Equals(database.ID, databaseName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(database.Name, databaseName, StringComparison.OrdinalIgnoreCase));
    }
}
