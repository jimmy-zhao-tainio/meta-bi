using MetaSchema;
using MetaTransformBinding;
using MetaTransformScript;

namespace MetaTransform.Binding;

public sealed class TransformBindingService
{
    public MetaTransformBindingModel BindSingleTransformModel(
        MetaTransformScriptModel model,
        MetaSchemaModel sourceSchema,
        string? activeLanguageProfileIdOverride = null)
    {
        var bound = BindSingleTransform(model, sourceSchema, activeLanguageProfileIdOverride);
        return TransformBindingModelBuilder.Create(bound);
    }

    public MetaTransformBindingModel BindTransformModel(
        MetaTransformScriptModel model,
        TransformScript transformScript,
        MetaSchemaModel sourceSchema,
        string? activeLanguageProfileIdOverride = null)
    {
        var bound = BindTransform(model, transformScript, sourceSchema, activeLanguageProfileIdOverride);
        return TransformBindingModelBuilder.Create(bound);
    }

    public BoundTransform BindSingleTransform(
        MetaTransformScriptModel model,
        MetaSchemaModel sourceSchema,
        string? activeLanguageProfileIdOverride = null)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(sourceSchema);

        if (model.TransformScriptList.Count != 1)
        {
            throw new InvalidOperationException(
                $"Expected exactly one TransformScript row but found {model.TransformScriptList.Count}.");
        }

        return BindTransform(model, model.TransformScriptList[0], sourceSchema, activeLanguageProfileIdOverride);
    }

    public BoundTransform BindTransform(
        MetaTransformScriptModel model,
        TransformScript transformScript,
        MetaSchemaModel sourceSchema,
        string? activeLanguageProfileIdOverride = null)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(transformScript);
        ArgumentNullException.ThrowIfNull(sourceSchema);

        var session = new TransformBindingSession(model, sourceSchema);
        return session.BindTransform(transformScript, activeLanguageProfileIdOverride);
    }
}
