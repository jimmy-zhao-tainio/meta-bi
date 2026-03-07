using Meta.Core.Domain;

namespace MetaDataTypeConversion.Core;

public static class MetaDataTypeConversionWorkspaces
{
    public static Workspace CreateMetaDataTypeConversionWorkspace(string workspaceRootPath)
    {
        var workspace = MetaDataTypeConversionWorkspaceFactory.CreateEmptyWorkspace(
            workspaceRootPath,
            MetaDataTypeConversionModels.CreateMetaDataTypeConversionModel());

        MetaDataTypeConversionSeed.Populate(workspace.Instance);
        return workspace;
    }

    public static Workspace CreateEmptyMetaDataTypeConversionWorkspace(string workspaceRootPath)
    {
        return MetaDataTypeConversionWorkspaceFactory.CreateEmptyWorkspace(
            workspaceRootPath,
            MetaDataTypeConversionModels.CreateMetaDataTypeConversionModel());
    }
}
