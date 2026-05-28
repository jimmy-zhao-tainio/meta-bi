using Meta.Core.Domain;

namespace MetaMultiDimensional.Core;

public static class MetaMultiDimensionalWorkspaces
{
    public static Workspace CreateEmptyMetaMultiDimensionalWorkspace(string workspaceRootPath)
    {
        return MetaMultiDimensionalWorkspaceFactory.CreateEmptyWorkspace(
            workspaceRootPath,
            MetaMultiDimensionalModels.CreateMetaMultiDimensionalModel());
    }
}
