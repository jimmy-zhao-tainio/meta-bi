using System.Text;

namespace MetaDataVault.Core;

public sealed record RawDataVaultSqlGenerationResult(
    string OutputPath,
    int FileCount,
    int TableCount,
    int SourceSystemCount,
    int SourceSchemaCount,
    int SourceTableCount,
    int SourceFieldCount,
    int RawHubCount,
    int RawLinkCount,
    int RawLinkHubCount,
    int RawHubSatelliteCount,
    int RawLinkSatelliteCount);

public interface IRawDataVaultSqlGenerator
{
    Task<RawDataVaultSqlGenerationResult> GenerateAsync(
        string rawDataVaultWorkspacePath,
        string implementationWorkspacePath,
        string dataTypeConversionWorkspacePath,
        string outputPath,
        CancellationToken cancellationToken = default);
}

public sealed class RawDataVaultSqlGenerator : IRawDataVaultSqlGenerator
{
    public async Task<RawDataVaultSqlGenerationResult> GenerateAsync(
        string rawDataVaultWorkspacePath,
        string implementationWorkspacePath,
        string dataTypeConversionWorkspacePath,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rawDataVaultWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(implementationWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(dataTypeConversionWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        var rdv = await RawDataVaultSqlModelLoaders.LoadRawDataVaultAsync(rawDataVaultWorkspacePath, cancellationToken).ConfigureAwait(false);
        var implementation = await RawDataVaultSqlModelLoaders.LoadImplementationAsync(implementationWorkspacePath, cancellationToken).ConfigureAwait(false);
        var conversions = await RawDataVaultSqlModelLoaders.LoadDataTypeConversionAsync(dataTypeConversionWorkspacePath, cancellationToken).ConfigureAwait(false);

        var outputRoot = Path.GetFullPath(outputPath);
        Directory.CreateDirectory(outputRoot);

        var context = new RawDataVaultSqlGenerationContext(rdv, implementation, conversions);
        context.EnsureRequiredRawImplementation();

        var scripts = new List<(string FileName, string Sql)>();

        foreach (var hub in rdv.RawHubList.OrderBy(row => row.Name, StringComparer.Ordinal))
        {
            var table = RawHubDdlConverter.Build(hub, context);
            scripts.Add(($"{table.Name}.sql", context.RenderTable(table)));
        }

        foreach (var link in rdv.RawLinkList.OrderBy(row => row.Name, StringComparer.Ordinal))
        {
            var table = RawLinkDdlConverter.Build(link, context);
            scripts.Add(($"{table.Name}.sql", context.RenderTable(table)));
        }

        foreach (var satellite in rdv.RawHubSatelliteList.OrderBy(row => row.Name, StringComparer.Ordinal))
        {
            var table = RawHubSatelliteDdlConverter.Build(satellite, context);
            scripts.Add(($"{table.Name}.sql", context.RenderTable(table)));
        }

        foreach (var satellite in rdv.RawLinkSatelliteList.OrderBy(row => row.Name, StringComparer.Ordinal))
        {
            var table = RawLinkSatelliteDdlConverter.Build(satellite, context);
            scripts.Add(($"{table.Name}.sql", context.RenderTable(table)));
        }

        foreach (var script in scripts)
        {
            var path = Path.Combine(outputRoot, script.FileName);
            await File.WriteAllTextAsync(path, script.Sql, new UTF8Encoding(false), cancellationToken).ConfigureAwait(false);
        }

        return new RawDataVaultSqlGenerationResult(
            outputRoot,
            scripts.Count,
            scripts.Count,
            rdv.SourceSystemList.Count,
            rdv.SourceSchemaList.Count,
            rdv.SourceTableList.Count,
            rdv.SourceFieldList.Count,
            rdv.RawHubList.Count,
            rdv.RawLinkList.Count,
            rdv.RawLinkHubList.Count,
            rdv.RawHubSatelliteList.Count,
            rdv.RawLinkSatelliteList.Count);
    }
}
