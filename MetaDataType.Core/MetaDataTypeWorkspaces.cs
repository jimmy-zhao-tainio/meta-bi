using Meta.Core.Domain;

namespace MetaDataType.Core;

public static class MetaDataTypeWorkspaces
{
    public static Workspace CreateMetaDataTypeWorkspace(string workspaceRootPath)
    {
        var workspace = MetaDataTypeWorkspaceFactory.CreateEmptyWorkspace(
            workspaceRootPath,
            MetaDataTypeModels.CreateMetaDataTypeModel());

        MetaDataTypeSeed.Populate(workspace.Instance);
        return workspace;
    }
}
