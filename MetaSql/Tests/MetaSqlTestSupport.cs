using Meta.Integration;
using Meta.Operations.Domain;
using Meta.Surfaces;
using Meta.TypedModels;

namespace MetaSql.Tests;

internal static class MetaSqlTestSupport
{
    public static void SaveXml(MetaSqlModel model, string path) =>
        SaveXmlAsync(model, path).GetAwaiter().GetResult();

    public static Task SaveXmlAsync(MetaSqlModel model, string path) =>
        SaveXmlAsync(model, path, MetaSqlModels.LoadContract(), new MetaSqlWorkspaceBinding());

    public static Task SaveXmlAsync(MetaRawDataVault.MetaRawDataVaultModel model, string path) =>
        SaveXmlAsync(model, path, MetaDataVault.Core.RawDataVaultModels.LoadContract(),
            new MetaRawDataVault.MetaRawDataVaultWorkspaceBinding());

    public static void SaveXml(MetaSqlDeployManifest.MetaSqlDeployManifestModel model, string path) =>
        SaveXmlAsync(model, path).GetAwaiter().GetResult();

    public static Task SaveXmlAsync(MetaSqlDeployManifest.MetaSqlDeployManifestModel model, string path) =>
        SaveXmlAsync(model, path, MetaSqlDeployManifest.MetaSqlDeployManifestModels.LoadContract(),
            new MetaSqlDeployManifest.MetaSqlDeployManifestWorkspaceBinding());

    private static Task SaveXmlAsync<TModel>(TModel model, string path, GenericModel contract,
        ITypedWorkspaceBinding<TModel> binding) where TModel : class =>
        WorkspaceSurface.CreateAsync(binding.Write(model, contract), path, "xml");
}
