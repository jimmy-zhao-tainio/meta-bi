# MetaSchemaAdapter

`MetaSchemaAdapter` is the provider boundary between an external system and
meta-bi. An adapter exposes the capabilities it supports through small
interfaces built around existing contracts.

`IMetaSchemaDiscoveryAdapter` discovers an external system as a
`MetaSchemaModel`. That workspace can be reviewed and used by the existing
MetaTransform binding workflow in the same way as any other MetaSchema
workspace.

`IMetaSchemaTransformAdapter` receives a `MetaTransformScriptModel`, its
validated `MetaTransformBindingModel`, the relevant `MetaSchemaModel`, and the
identities of the selected transform and binding. A row-producing transform
supplies MetaPipeline with an `IPipelineRowStreamSource`.

`IMetaSchemaMutationAdapter` executes mutating transforms and returns their
affected-row evidence. It is separate so a read-only provider can support
row-producing transforms without claiming mutation support.

`IMetaSchemaTargetWriteAdapter` creates the existing
`IPipelineTargetWriteOperation` used to insert a bound row stream into an
external target. MetaPipeline continues to own buffering, shape checks,
ordering, progress, and execution evidence.

One adapter class may implement all four capabilities, or only the ones its
external system supports. Connection references are names resolved by the
adapter. Credentials and provider configuration remain outside the modeled
workspaces.

The standard binding path remains unchanged:

```text
IMetaSchemaDiscoveryAdapter
    -> MetaSchemaAdapterWorkspaceService
    -> ordinary MetaSchema workspace

MetaTransformScript workspace + source/target MetaSchema workspaces
    -> existing TransformBindingWorkspaceService
    -> validated MetaTransformBinding workspace

selected transform + binding
    -> MetaSchemaAdapterExecutionService
    -> provider row stream and target operation
    -> existing BufferedPipelineExecutionService
```

External packages own provider protocols, authentication, schema discovery,
translation of the supported MetaTransformScript semantics, and physical read
or write behavior. They do not replace MetaTransformBinding or MetaPipeline.
They should reject semantic forms they do not support with a clear diagnostic.

`MetaSchemaAdapterWorkspaceService` materializes discovery in any workspace
representation supported by the normal model mapper. `MetaSchemaAdapterExecutionService`
loads the selected MetaSchema, MetaTransformScript, and MetaTransformBinding
workspaces, uses MetaPipeline's existing execution resolver, and dispatches to
the provider capability required by the selected transform. Row-producing
transforms run through MetaPipeline; mutating transforms run through
`IMetaSchemaMutationAdapter`.

The application supplies the adapter instances to these services. Selecting,
loading, and configuring external provider packages belongs to the application
host; the contract library does not impose a plugin-loading mechanism.

The contract project is:

```text
MetaSchemaAdapter/MetaSchema.Adapter.csproj
```

The contract test demonstrates the individual capabilities, row delivery
through `BufferedPipelineExecutionService`, and mutation evidence.

A second, test-only witness is written as an external tab-separated-text
provider. Its integration test discovers files into an XML MetaSchema
workspace, imports a MetaTransformScript workspace, creates a validated binding
workspace through `TransformBindingWorkspaceService`, and executes the selected
transform through `MetaSchemaAdapterExecutionService`. The provider uses the
semantic transform entities and their table-source and column-reference binding
evidence, then writes its rows through MetaPipeline. A separate test writes and
reloads discovered MetaSchema in both XML and C# workspace representations.

The tab-separated provider supports a single-table projection of columns and
literals with an optional equality predicate. That limit belongs to the example
provider, not to the adapter contract. Its unsupported-expression test shows
how a provider reports a semantic form that it does not implement.
