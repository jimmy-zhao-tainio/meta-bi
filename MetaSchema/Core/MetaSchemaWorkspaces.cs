using Meta.Core.Domain;

namespace MetaSchema.Core;

public static class MetaSchemaWorkspaces
{
    public static InMemoryWorkspace CreateEmptyMetaSchemaWorkspace()
    {
        var model = MetaSchemaModels.CreateMetaSchemaModel();
        return new InMemoryWorkspace(
            model,
            new GenericInstance
            {
                ModelName = model.Name,
            });
    }
}
