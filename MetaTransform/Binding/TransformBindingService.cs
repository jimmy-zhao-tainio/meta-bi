using MetaTransformBinding;
using MetaTransformScript;
using MetaSchema;

namespace MetaTransform.Binding;

public sealed class TransformBindingService
{
    public MetaTransformBindingModel BindSingleTransformModel(
        MetaTransformScriptModel model,
        MetaSchemaModel sourceSchema)
    {
        ArgumentNullException.ThrowIfNull(sourceSchema);
        var bound = BindSingleTransform(model, sourceSchema);
        return TransformBindingModelBuilder.Create(bound);
    }

    public MetaTransformBindingModel BindSingleTransformModel(
        MetaTransformScriptModel model)
    {
        var bound = BindSingleTransform(model);
        return TransformBindingModelBuilder.Create(bound);
    }

    public MetaTransformBindingModel BindTransformModel(
        MetaTransformScriptModel model,
        TransformScript transformScript,
        MetaSchemaModel sourceSchema)
    {
        ArgumentNullException.ThrowIfNull(sourceSchema);
        var bound = BindTransform(model, transformScript, sourceSchema);
        return TransformBindingModelBuilder.Create(bound);
    }

    public MetaTransformBindingModel BindTransformModel(
        MetaTransformScriptModel model,
        TransformScript transformScript)
    {
        var bound = BindTransform(model, transformScript);
        return TransformBindingModelBuilder.Create(bound);
    }

    public TransformBindingResult BindSingleTransform(
        MetaTransformScriptModel model,
        MetaSchemaModel sourceSchema)
    {
        ArgumentNullException.ThrowIfNull(sourceSchema);
        ArgumentNullException.ThrowIfNull(model);

        if (model.TransformScriptList.Count != 1)
        {
            throw new InvalidOperationException(
                $"Expected exactly one TransformScript row but found {model.TransformScriptList.Count}.");
        }

        return BindTransform(model, model.TransformScriptList[0], sourceSchema);
    }

    public TransformBindingResult BindSingleTransform(
        MetaTransformScriptModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (model.TransformScriptList.Count != 1)
        {
            throw new InvalidOperationException(
                $"Expected exactly one TransformScript row but found {model.TransformScriptList.Count}.");
        }

        return BindTransform(model, model.TransformScriptList[0]);
    }

    public TransformBindingResult BindTransform(
        MetaTransformScriptModel model,
        TransformScript transformScript,
        MetaSchemaModel sourceSchema)
    {
        ArgumentNullException.ThrowIfNull(sourceSchema);
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(transformScript);

        var session = new TransformBindingSession(model, sourceSchema);
        return session.BindTransform(transformScript);
    }

    public TransformBindingResult BindTransform(
        MetaTransformScriptModel model,
        TransformScript transformScript)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(transformScript);

        var session = new TransformBindingSession(model);
        return session.BindTransform(transformScript);
    }
}
