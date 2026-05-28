using Meta.Core.Domain;

namespace MetaDataWarehouse.Core;

public static class MetaDataWarehouseWorkspaces
{
    public static Workspace CreateEmptyMetaDataWarehouseWorkspace(string workspaceRootPath)
    {
        return MetaDataWarehouseWorkspaceFactory.CreateEmptyWorkspace(
            workspaceRootPath,
            MetaDataWarehouseModels.CreateMetaDataWarehouseModel());
    }

}
