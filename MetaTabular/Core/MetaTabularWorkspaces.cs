using Meta.Core.Domain;

namespace MetaTabular.Core;

public static class MetaTabularWorkspaces
{
    public static Workspace CreateEmptyMetaTabularWorkspace(string workspaceRootPath)
    {
        return MetaTabularWorkspaceFactory.CreateEmptyWorkspace(
            workspaceRootPath,
            MetaTabularModels.CreateMetaTabularModel());
    }
}
