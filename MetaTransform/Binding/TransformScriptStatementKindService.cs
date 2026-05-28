using MetaTransformScript;

namespace MetaTransform.Binding;

public sealed class TransformScriptStatementKindService
{
    public BoundStatementKind GetStatementKind(
        MetaTransformScriptModel model,
        TransformScript transformScript)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(transformScript);

        return new TransformScriptNavigator(model).GetTransformScriptStatementKind(transformScript);
    }

    public IReadOnlyDictionary<string, BoundStatementKind> GetStatementKindsByTransformScriptId(
        MetaTransformScriptModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var navigator = new TransformScriptNavigator(model);
        return model.TransformScriptList.ToDictionary(
            static item => item.Id,
            item => navigator.GetTransformScriptStatementKind(item),
            StringComparer.Ordinal);
    }
}
