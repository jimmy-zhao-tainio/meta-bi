using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaSchema.Core;

public static class MetaSchemaModels
{
    public const string MetaSchemaModelName = "MetaSchema";

    public static GenericModel CreateMetaSchemaModel()
    {
        return TypedWorkspaceModelMapper
            .ToInMemoryWorkspace(MetaSchemaModel.CreateEmpty())
            .Model;
    }
}

