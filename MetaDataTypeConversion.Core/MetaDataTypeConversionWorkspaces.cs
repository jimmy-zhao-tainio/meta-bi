using Meta.Core.Domain;
using Meta.Core.Services;
using MetaDataTypeConversion.Instance;

namespace MetaDataTypeConversion.Core;

public static class MetaDataTypeConversionWorkspaces
{
    public static Workspace CreateMetaDataTypeConversionWorkspace(string workspaceRootPath)
    {
        MetaDataTypeConversionInstance.Default.SaveToXmlWorkspace(workspaceRootPath);
        return new WorkspaceService().LoadAsync(workspaceRootPath, searchUpward: false).GetAwaiter().GetResult();
    }

    public static Workspace CreateEmptyMetaDataTypeConversionWorkspace(string workspaceRootPath)
    {
        return MetaDataTypeConversionWorkspaceFactory.CreateEmptyWorkspace(
            workspaceRootPath,
            MetaDataTypeConversionModels.CreateMetaDataTypeConversionModel());
    }
}
