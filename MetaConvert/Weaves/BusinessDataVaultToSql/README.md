# Business Data Vault to SQL Weave

This workspace is the sanctioned forward `MetaWeave` correspondence from a
`MetaBusinessDataVault` and its implementation policy to `MetaSql`. Its
WeaveScript relations project hubs, links, references, satellites, same-as and
hierarchical links, point-in-time tables, and bridges as SQL database
structure.

The direction reads four named source workspaces:

- `business` supplies the Business Data Vault model;
- `implementation` supplies SQL schemas, naming patterns, technical columns,
  keys, and physical datatype policy;
- `dataTypes` supplies the sanctioned datatype systems and types;
- `typeConversions` supplies direct Meta-to-SQL Server datatype mappings.

The `databaseName` parameter names the resulting `MetaSql` database.
Requirements cover implementation cardinality, satellite specialization,
key-part and bridge chains, role names, optional implementation columns,
datatype lowering, projected identity, name allocation, and SQL Server's
128-character identifier limit.

Column population follows the converter's authored order. Predecessor chains
remain structural, while ordinary member collections use their declared name
and identity order. Column-name collisions retain the established behavior of
prefixing underscores until the name is free.

Execute the weave into a new target workspace:

```text
meta-weave execute \
  --workspace . \
  --source-workspace business=../../../MetaDataVault/Workspaces/SampleBusinessDataVaultCommerceHelpers \
  --source-workspace implementation=../../../MetaDataVault/Workspaces/MetaDataVaultImplementation \
  --source-workspace dataTypes=../../../MetaDataType/Workspace \
  --source-workspace typeConversions=../../../MetaDataTypeConversion/Workspace \
  --parameter databaseName=BusinessVault \
  --target-workspace ../../../MetaSql/Workspace \
  --xml <new-target-workspace>
```

`forward` is the default direction. Use `--csharp` or `--sql` instead of
`--xml` for another target surface. `emit-relation`, `emit-requirement`, and
`emit-transformation` expose the modeled WeaveScript through the CLI.

The product converter executes this packaged workspace. The former C# path is
kept only in the test reference tree, where the complete commerce-helper and
link-variant witnesses, plus adversarial name allocation, are compared for
structural workspace equivalence.
