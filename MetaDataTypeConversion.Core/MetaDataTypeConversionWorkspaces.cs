using Meta.Core.Domain;
using MetaDataTypeConversion.Instance;

namespace MetaDataTypeConversion.Core;

public static class MetaDataTypeConversionWorkspaces
{
    public static Workspace CreateMetaDataTypeConversionWorkspace(string workspaceRootPath)
    {
        return MetaDataTypeConversionInstance.Default.ToXmlWorkspace(workspaceRootPath);
    }

    public static Workspace CreateEmptyMetaDataTypeConversionWorkspace(string workspaceRootPath)
    {
        return MetaDataTypeConversionWorkspaceFactory.CreateEmptyWorkspace(
            workspaceRootPath,
            MetaDataTypeConversionModels.CreateMetaDataTypeConversionModel());
    }
}
