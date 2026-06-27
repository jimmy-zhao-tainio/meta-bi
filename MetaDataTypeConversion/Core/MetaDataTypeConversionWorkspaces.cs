using MetaDataTypeConversion;
using MetaDataTypeConversion.Instance;

namespace MetaDataTypeConversion.Core;

public static class MetaDataTypeConversionWorkspaces
{
    public static MetaDataTypeConversionModel CreateMetaDataTypeConversionWorkspace(string workspaceRootPath)
    {
        MetaDataTypeConversionInstance.Default.SaveToXmlWorkspace(workspaceRootPath);
        return MetaDataTypeConversionModel.LoadFromXmlWorkspace(workspaceRootPath, searchUpward: false);
    }

    public static MetaDataTypeConversionModel CreateEmptyMetaDataTypeConversionWorkspace(string workspaceRootPath)
    {
        var model = MetaDataTypeConversionModel.CreateEmpty();
        model.SaveToXmlWorkspace(workspaceRootPath);
        return model;
    }
}
