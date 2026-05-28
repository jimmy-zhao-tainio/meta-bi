using Meta.Core.Domain;
using MetaWorkspaceConfig = Meta.Core.WorkspaceConfig.Generated.MetaWorkspace;

namespace MetaDataWarehouse.Core;

public static class MetaDataWarehouseWorkspaceFactory
{
    public static Workspace CreateEmptyWorkspace(string workspaceRootPath, GenericModel model)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspaceRootPath);
        ArgumentNullException.ThrowIfNull(model);

        var rootPath = Path.GetFullPath(workspaceRootPath);

        return new Workspace
        {
            WorkspaceRootPath = rootPath,
            MetadataRootPath = rootPath,
            WorkspaceConfig = MetaWorkspaceConfig.CreateDefault(),
            Model = model,
            Instance = new GenericInstance
            {
                ModelName = model.Name,
            },
            IsDirty = true,
        };
    }
}
