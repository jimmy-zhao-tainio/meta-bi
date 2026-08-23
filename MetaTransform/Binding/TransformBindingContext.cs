using MetaSchema;
using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed class TransformBindingContext
{
    private readonly MetaSchemaTableResolver? sourceSchemaResolver;
    private readonly MetaSchemaTableResolver? targetSchemaResolver;
    private readonly string executeSystemName;
    private readonly string executeSystemDefaultSchemaName;

    public TransformBindingContext(MetaTransformScriptModel model)
        : this(
            model,
            sourceSchemaResolver: null,
            targetSchemaResolver: null,
            executeSystemName: string.Empty,
            executeSystemDefaultSchemaName: string.Empty)
    {
    }

    public TransformBindingContext(
        MetaTransformScriptModel model,
        MetaSchemaModel sourceSchema)
    {
        ArgumentNullException.ThrowIfNull(sourceSchema);
        Navigator = new TransformScriptNavigator(model);
        sourceSchemaResolver = new MetaSchemaTableResolver(sourceSchema);
        targetSchemaResolver = sourceSchemaResolver;
        executeSystemName = string.Empty;
        executeSystemDefaultSchemaName = string.Empty;
    }

    public TransformBindingContext(
        MetaTransformScriptModel model,
        MetaSchemaTableResolver? sourceSchemaResolver,
        MetaSchemaTableResolver? targetSchemaResolver,
        string? executeSystemName,
        string? executeSystemDefaultSchemaName)
    {
        Navigator = new TransformScriptNavigator(model ?? throw new ArgumentNullException(nameof(model)));
        this.sourceSchemaResolver = sourceSchemaResolver;
        this.targetSchemaResolver = targetSchemaResolver;
        this.executeSystemName = executeSystemName?.Trim() ?? string.Empty;
        this.executeSystemDefaultSchemaName = executeSystemDefaultSchemaName?.Trim() ?? string.Empty;
    }

    public TransformScriptNavigator Navigator { get; }

    public TransformBindingResult Bind(TransformScript transformScript)
    {
        ArgumentNullException.ThrowIfNull(transformScript);
        return new TransformBindingSession(
                Navigator,
                sourceSchemaResolver,
                targetSchemaResolver,
                executeSystemName,
                executeSystemDefaultSchemaName)
            .BindTransform(transformScript);
    }
}
