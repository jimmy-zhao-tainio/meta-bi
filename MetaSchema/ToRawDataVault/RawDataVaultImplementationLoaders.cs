using DVI = global::MetaDataVaultImplementation;

namespace MetaSchema.ToRawDataVault;

public sealed record RawDataVaultImplementationModel(
    DVI.MetaDataVaultImplementationModel Model,
    DVI.RawHubImplementation RawHubImplementation,
    DVI.RawLinkImplementation RawLinkImplementation,
    DVI.RawHubSatelliteImplementation RawHubSatelliteImplementation,
    DVI.RawLinkSatelliteImplementation RawLinkSatelliteImplementation);

public static class RawDataVaultImplementationLoaders
{
    public static Task<RawDataVaultImplementationModel> LoadImplementationAsync(string workspacePath, CancellationToken cancellationToken = default)
    {
        var model = DVI.MetaDataVaultImplementationModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
        return Task.FromResult(new RawDataVaultImplementationModel(
            model,
            GetSingle(model.RawHubImplementationList, nameof(DVI.RawHubImplementation)),
            GetSingle(model.RawLinkImplementationList, nameof(DVI.RawLinkImplementation)),
            GetSingle(model.RawHubSatelliteImplementationList, nameof(DVI.RawHubSatelliteImplementation)),
            GetSingle(model.RawLinkSatelliteImplementationList, nameof(DVI.RawLinkSatelliteImplementation))));
    }

    private static T GetSingle<T>(IReadOnlyList<T> rows, string entityName)
    {
        if (rows.Count != 1)
        {
            throw new InvalidOperationException($"Workspace requires exactly one '{entityName}' row, found {rows.Count}.");
        }

        return rows[0];
    }
}
