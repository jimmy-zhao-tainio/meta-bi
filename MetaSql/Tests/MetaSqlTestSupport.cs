using Meta.Integration;
using Meta.Surfaces;

namespace MetaSql.Tests;

internal static class MetaSqlTestSupport
{
    public static void SaveXml<TModel>(TModel model, string workspacePath)
        where TModel : class
    {
        WorkspaceSurface.CreateAsync(
                TypedWorkspaceModelMapper.ToInMemoryWorkspace(model),
                workspacePath,
                "xml")
            .GetAwaiter()
            .GetResult();
    }

    public static Task SaveXmlAsync<TModel>(TModel model, string workspacePath)
        where TModel : class
    {
        return WorkspaceSurface.CreateAsync(
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(model),
            workspacePath,
            "xml");
    }
}
