using Meta.Core.Domain;
using MetaDataType.Instance;

namespace MetaDataType.Core;

public static class MetaDataTypeWorkspaces
{
    public static Workspace CreateMetaDataTypeWorkspace(string workspaceRootPath)
    {
        return MetaDataTypeInstance.Default.ToXmlWorkspace(workspaceRootPath);
    }
}
