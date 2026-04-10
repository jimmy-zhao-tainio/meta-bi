using MetaTransformBinding;
using MetaTransformScript;
using MetaSchema;

namespace MetaTransform.Binding;

public sealed class TransformBindingService
{
    public MetaTransformBindingModel BindSingleTransformModel(
        MetaTransformScriptModel model,
        MetaSchemaModel sourceSchema,
        string? activeLanguageProfileIdOverride = null)
    {
        ArgumentNullException.ThrowIfNull(sourceSchema);
        return BindSingleTransformModel(model, activeLanguageProfileIdOverride);
    }

    public MetaTransformBindingModel BindSingleTransformModel(
        MetaTransformScriptModel model,
        string? activeLanguageProfileIdOverride = null)
    {
        var bound = BindSingleTransform(model, activeLanguageProfileIdOverride);
        return TransformBindingModelBuilder.Create(bound);
    }

    public MetaTransformBindingModel BindTransformModel(
        MetaTransformScriptModel model,
        TransformScript transformScript,
        MetaSchemaModel sourceSchema,
        string? activeLanguageProfileIdOverride = null)
    {
        ArgumentNullException.ThrowIfNull(sourceSchema);
        return BindTransformModel(model, transformScript, activeLanguageProfileIdOverride);
    }

    public MetaTransformBindingModel BindTransformModel(
        MetaTransformScriptModel model,
        TransformScript transformScript,
        string? activeLanguageProfileIdOverride = null)
    {
        var bound = BindTransform(model, transformScript, activeLanguageProfileIdOverride);
        return TransformBindingModelBuilder.Create(bound);
    }

    public TransformBindingResult BindSingleTransform(
        MetaTransformScriptModel model,
        MetaSchemaModel sourceSchema,
        string? activeLanguageProfileIdOverride = null)
    {
        ArgumentNullException.ThrowIfNull(sourceSchema);
        return BindSingleTransform(model, activeLanguageProfileIdOverride);
    }

    public TransformBindingResult BindSingleTransform(
        MetaTransformScriptModel model,
        string? activeLanguageProfileIdOverride = null)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (model.TransformScriptList.Count != 1)
        {
            throw new InvalidOperationException(
                $"Expected exactly one TransformScript row but found {model.TransformScriptList.Count}.");
        }

        return BindTransform(model, model.TransformScriptList[0], activeLanguageProfileIdOverride);
    }

    public TransformBindingResult BindTransform(
        MetaTransformScriptModel model,
        TransformScript transformScript,
        MetaSchemaModel sourceSchema,
        string? activeLanguageProfileIdOverride = null)
    {
        ArgumentNullException.ThrowIfNull(sourceSchema);
        return BindTransform(model, transformScript, activeLanguageProfileIdOverride);
    }

    public TransformBindingResult BindTransform(
        MetaTransformScriptModel model,
        TransformScript transformScript,
        string? activeLanguageProfileIdOverride = null)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(transformScript);

        var session = new TransformBindingSession(model);
        return session.BindTransform(transformScript, activeLanguageProfileIdOverride);
    }
}
