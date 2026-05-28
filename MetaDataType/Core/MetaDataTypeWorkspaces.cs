using Meta.Core.Domain;
using Meta.Core.Services;
using MetaDataType.Instance;

namespace MetaDataType.Core;

public static class MetaDataTypeWorkspaces
{
    public static Workspace CreateMetaDataTypeWorkspace(string workspaceRootPath)
    {
        MetaDataTypeInstance.Default.SaveToXmlWorkspace(workspaceRootPath);
        return new WorkspaceService().LoadAsync(workspaceRootPath, searchUpward: false).GetAwaiter().GetResult();
    }
}
