using MetaPipeline;
using MetaSchema;
using MetaTransformBinding;
using MetaTransformScript;

namespace MetaSchemaAdapter;

/// <summary>
/// Identifies the external connection and system name to discover into MetaSchema.
/// </summary>
/// <param name="ConnectionReference">A provider-resolved connection name. It is not a credential.</param>
/// <param name="SystemName">The system name to use in the discovered MetaSchema model.</param>
public sealed record MetaSchemaDiscoveryRequest(
    string ConnectionReference,
    string SystemName);

/// <summary>
/// Supplies a provider with one bound, row-producing transform and its expected output shape.
/// </summary>
/// <param name="ConnectionReference">The provider-resolved source connection name.</param>
/// <param name="Schema">The MetaSchema contract used to bind the transform.</param>
/// <param name="Transforms">The semantic MetaTransformScript model containing the selected transform.</param>
/// <param name="Binding">The MetaTransformBinding evidence for the selected transform.</param>
/// <param name="TransformScriptId">The selected TransformScript identity.</param>
/// <param name="TransformBindingId">The corresponding TransformBinding identity.</param>
/// <param name="OutputShape">The row shape expected by MetaPipeline.</param>
/// <param name="BatchSize">The requested maximum number of rows per produced batch.</param>
/// <param name="TimeoutSeconds">An optional provider operation timeout.</param>
/// <param name="ExecutionContext">Optional MetaPipeline execution context.</param>
public sealed record MetaSchemaRowStreamRequest(
    string ConnectionReference,
    MetaSchemaModel Schema,
    MetaTransformScriptModel Transforms,
    MetaTransformBindingModel Binding,
    string TransformScriptId,
    string TransformBindingId,
    PipelineRowStreamShape OutputShape,
    int BatchSize = 1000,
    int? TimeoutSeconds = null,
    MetaPipelineExecutionContext? ExecutionContext = null);

/// <summary>
/// Supplies a provider with one bound mutating transform.
/// </summary>
/// <param name="ConnectionReference">The provider-resolved connection name.</param>
/// <param name="Schema">The MetaSchema contract used to bind the transform.</param>
/// <param name="Transforms">The semantic MetaTransformScript model containing the selected transform.</param>
/// <param name="Binding">The MetaTransformBinding evidence for the selected transform.</param>
/// <param name="TransformScriptId">The selected TransformScript identity.</param>
/// <param name="TransformBindingId">The corresponding TransformBinding identity.</param>
/// <param name="TimeoutSeconds">An optional provider operation timeout.</param>
/// <param name="ExecutionContext">Optional MetaPipeline execution context.</param>
public sealed record MetaSchemaMutationRequest(
    string ConnectionReference,
    MetaSchemaModel Schema,
    MetaTransformScriptModel Transforms,
    MetaTransformBindingModel Binding,
    string TransformScriptId,
    string TransformBindingId,
    int? TimeoutSeconds = null,
    MetaPipelineExecutionContext? ExecutionContext = null);

/// <summary>
/// Reports evidence returned by a provider after a mutating transform.
/// </summary>
/// <param name="AffectedRowCount">The affected row count when the provider can report it.</param>
public sealed record MetaSchemaMutationResult(
    long? AffectedRowCount);

/// <summary>
/// Supplies a provider with a bound target and row shape for a MetaPipeline insert.
/// </summary>
/// <param name="ConnectionReference">The provider-resolved target connection name.</param>
/// <param name="Schema">The target MetaSchema contract.</param>
/// <param name="TargetIdentifier">The bound target object identifier.</param>
/// <param name="Shape">The incoming MetaPipeline row shape.</param>
/// <param name="TimeoutSeconds">An optional provider operation timeout.</param>
/// <param name="ExecutionContext">Optional MetaPipeline execution context.</param>
public sealed record MetaSchemaInsertRowsRequest(
    string ConnectionReference,
    MetaSchemaModel Schema,
    string TargetIdentifier,
    PipelineRowStreamShape Shape,
    int? TimeoutSeconds = null,
    MetaPipelineExecutionContext? ExecutionContext = null);
