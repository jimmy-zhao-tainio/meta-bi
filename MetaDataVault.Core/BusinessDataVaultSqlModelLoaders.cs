using BDV = MetaBusinessDataVault;
using DTC = MetaDataTypeConversion;
using DVI = MetaDataVaultImplementation;

namespace MetaDataVault.Core;

public sealed record DataVaultImplementationModel(
    DVI.MetaDataVaultImplementationModel Model,
    DVI.BusinessHubImplementation BusinessHubImplementation,
    DVI.BusinessLinkImplementation BusinessLinkImplementation,
    DVI.BusinessSameAsLinkImplementation BusinessSameAsLinkImplementation,
    DVI.BusinessHierarchicalLinkImplementation BusinessHierarchicalLinkImplementation,
    DVI.BusinessHubSatelliteImplementation BusinessHubSatelliteImplementation,
    DVI.BusinessLinkSatelliteImplementation BusinessLinkSatelliteImplementation,
    DVI.BusinessSameAsLinkSatelliteImplementation BusinessSameAsLinkSatelliteImplementation,
    DVI.BusinessHierarchicalLinkSatelliteImplementation BusinessHierarchicalLinkSatelliteImplementation,
    DVI.BusinessPointInTimeImplementation BusinessPointInTimeImplementation,
    DVI.BusinessBridgeImplementation BusinessBridgeImplementation);

public sealed record DataTypeConversionModel(
    DTC.MetaDataTypeConversionModel Model,
    IReadOnlyDictionary<string, string> SourceToTargetDataTypeIds);

public static class BusinessDataVaultSqlModelLoaders
{
    public static Task<BDV.MetaBusinessDataVaultModel> LoadBusinessDataVaultAsync(string workspacePath, CancellationToken cancellationToken = default)
    {
        return BDV.MetaBusinessDataVaultModel.LoadFromXmlWorkspaceAsync(workspacePath, searchUpward: false, cancellationToken);
    }

    public static Task<DataVaultImplementationModel> LoadImplementationAsync(string workspacePath, CancellationToken cancellationToken = default)
    {
        var model = DVI.MetaDataVaultImplementationModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
        return Task.FromResult(new DataVaultImplementationModel(
            model,
            GetSingle(model.BusinessHubImplementationList, nameof(DVI.BusinessHubImplementation)),
            GetSingle(model.BusinessLinkImplementationList, nameof(DVI.BusinessLinkImplementation)),
            GetSingle(model.BusinessSameAsLinkImplementationList, nameof(DVI.BusinessSameAsLinkImplementation)),
            GetSingle(model.BusinessHierarchicalLinkImplementationList, nameof(DVI.BusinessHierarchicalLinkImplementation)),
            GetSingle(model.BusinessHubSatelliteImplementationList, nameof(DVI.BusinessHubSatelliteImplementation)),
            GetSingle(model.BusinessLinkSatelliteImplementationList, nameof(DVI.BusinessLinkSatelliteImplementation)),
            GetSingle(model.BusinessSameAsLinkSatelliteImplementationList, nameof(DVI.BusinessSameAsLinkSatelliteImplementation)),
            GetSingle(model.BusinessHierarchicalLinkSatelliteImplementationList, nameof(DVI.BusinessHierarchicalLinkSatelliteImplementation)),
            GetSingle(model.BusinessPointInTimeImplementationList, nameof(DVI.BusinessPointInTimeImplementation)),
            GetSingle(model.BusinessBridgeImplementationList, nameof(DVI.BusinessBridgeImplementation))));
    }

    public static Task<DataTypeConversionModel> LoadDataTypeConversionAsync(string workspacePath, CancellationToken cancellationToken = default)
    {
        var model = DTC.MetaDataTypeConversionModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
        var mappings = model.DataTypeMappingList
            .ToDictionary(row => row.SourceDataTypeId, row => row.TargetDataTypeId, StringComparer.Ordinal);
        return Task.FromResult(new DataTypeConversionModel(model, mappings));
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
