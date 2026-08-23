using Tom = Microsoft.AnalysisServices.Tabular;

namespace MetaTabular.Core.Deploy;

public sealed class MetaTabularProcessService
{
    public Task<MetaTabularProcessResult> ProcessAsync(MetaTabularProcessRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Server);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.DatabaseName);

        var refreshType = ParseRefreshType(request.RefreshType);
        var tableName = request.TableName?.Trim();
        var partitionName = request.PartitionName?.Trim();
        if (!string.IsNullOrWhiteSpace(partitionName) && string.IsNullOrWhiteSpace(tableName))
        {
            throw new InvalidOperationException("Tabular partition processing requires --table <name>.");
        }

        using var server = new Tom.Server();
        server.Connect($"Data Source={request.Server}");
        var database = FindDatabase(server, request.DatabaseName)
            ?? throw new InvalidOperationException($"Tabular database '{request.DatabaseName}' does not exist on '{request.Server}'.");

        if (string.IsNullOrWhiteSpace(tableName))
        {
            database.Model.RequestRefresh(refreshType);
            database.Model.SaveChanges();
            return Task.FromResult(new MetaTabularProcessResult
            {
                Server = request.Server,
                DatabaseName = database.Name,
                RefreshType = refreshType.ToString(),
                TargetKind = "Database",
            });
        }

        var table = FindTable(database.Model, tableName)
            ?? throw new InvalidOperationException($"Tabular table '{tableName}' does not exist in database '{database.Name}'.");
        if (string.IsNullOrWhiteSpace(partitionName))
        {
            table.RequestRefresh(refreshType);
            database.Model.SaveChanges();
            return Task.FromResult(new MetaTabularProcessResult
            {
                Server = request.Server,
                DatabaseName = database.Name,
                RefreshType = refreshType.ToString(),
                TargetKind = "Table",
                TableName = table.Name,
            });
        }

        var partition = FindPartition(table, partitionName)
            ?? throw new InvalidOperationException($"Tabular partition '{partitionName}' does not exist on table '{table.Name}' in database '{database.Name}'.");
        partition.RequestRefresh(refreshType);
        database.Model.SaveChanges();
        return Task.FromResult(new MetaTabularProcessResult
        {
            Server = request.Server,
            DatabaseName = database.Name,
            RefreshType = refreshType.ToString(),
            TargetKind = "Partition",
            TableName = table.Name,
            PartitionName = partition.Name,
        });
    }

    private static Tom.RefreshType ParseRefreshType(string? value)
    {
        var text = string.IsNullOrWhiteSpace(value) ? "Full" : value.Trim();
        return Enum.TryParse<Tom.RefreshType>(text, ignoreCase: true, out var refreshType)
            ? refreshType
            : throw new InvalidOperationException($"Tabular refresh type '{text}' is not supported. Use Full, DataOnly, Calculate, ClearValues, Automatic, Add, or Defragment.");
    }

    private static Tom.Database? FindDatabase(Tom.Server server, string databaseName)
    {
        return server.Databases
            .OfType<Tom.Database>()
            .FirstOrDefault(database =>
                string.Equals(database.ID, databaseName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(database.Name, databaseName, StringComparison.OrdinalIgnoreCase));
    }

    private static Tom.Table? FindTable(Tom.Model model, string tableName)
    {
        return model.Tables
            .OfType<Tom.Table>()
            .FirstOrDefault(table =>
                string.Equals(table.Name, tableName, StringComparison.OrdinalIgnoreCase));
    }

    private static Tom.Partition? FindPartition(Tom.Table table, string partitionName)
    {
        return table.Partitions
            .OfType<Tom.Partition>()
            .FirstOrDefault(partition =>
                string.Equals(partition.Name, partitionName, StringComparison.OrdinalIgnoreCase));
    }
}
