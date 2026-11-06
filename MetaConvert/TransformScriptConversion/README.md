# Apply authored conversions to SELECT projections

`TransformScriptConversionWorkspaceService.Apply` loads an existing TransformScript,
a selected MetaDataType catalog, a MetaDataTypeConversion workspace, and a source
MetaSchema workspace. Each `ProjectionConversionRequirement` identifies a
`SelectScalarExpression` in the input and supplies its destination type ID and
explicit SQL Server facets. The destination requirement belongs to the caller.

```csharp
var result = new TransformScriptConversionWorkspaceService().Apply(new(
    InputWorkspace: inputPath,
    DataTypeWorkspace: catalogPath,
    ConversionWorkspace: conversionsPath,
    SourceSchemaWorkspace: sourcePath,
    Requirements: [new(projectionId, destinationId, new(Length: 25))],
    OutputWorkspace: newPath));
```

This is a programmatic workspace operation. It does not add a CLI command in this
slice. The output must not exist or be nested in an input workspace. Input
artifacts are left intact; the result is a new XML TransformScript workspace.

## Ownership and call chain

1. MetaConvert owns preparation of the new script. It loads the selected catalog
   and source schema. MetaSchema validates SQL Server type membership through the
   catalog's modeled DataTypeSystem relationships.
2. Binding's read-only analysis resolves source references and expression contracts
   using that same catalog. Unknown/unsupported expression types are rejected.
   Binding selects no mappings and applies no conversions.
3. The producing service selects exactly one authored DataTypeMapping with the
   resolved source ID and the explicitly required destination ID. It follows no
   compatibility graph and supplies no implicit direct conversion.
4. MetaDataTypeConversion owns the implementation/snippet and its parameter
   bindings. `SqlConversionSnippetService` instantiates the authored text.
5. TransformScript.Sql parses the snippet as one scalar expression and composes it
   into the selected projection. The whole original expression is parenthesized;
   aliases (including implicit column names), column order, references and predicates
   are retained. Export/re-import normalizes the graph and removes detached operands.
6. Existing Binding analysis must establish the exact requested resulting type and
   facets before materialization. The caller can then use ordinary schema-aware
   Binding against its destination schema, followed by execution of the exported SQL.

## Authored snippet data

`ConversionImplementation.SqlConversionSnippet` references a reusable
`SqlConversionSnippet` whose `TransformScript` contains the scalar SQL expression.
For example, `CAST($(source) AS $(destination))` and
`CONVERT($(destination), $(source))` require no different C# conversion branches.

Each `SqlConversionParameterBinding` belongs to the snippet, names a placeholder,
and references a `SqlConversionParameter`. Supported parameter **names** are:

| Parameter name | Supplied value |
| --- | --- |
| `SourceExpression` | The complete selected expression, parenthesized |
| `DestinationType` | Catalog-resolved SQL type name with explicit destination facets |

Parameter and snippet identities are authored data. A binding named `source`
references the parameter named `SourceExpression`; a binding named `destination`
references `DestinationType`. Placeholder names are case-insensitive, permit
surrounding whitespace, and use the existing TransformPattern text syntax:
`$(name)` substitutes and `$$(name)` emits a literal `$(name)`. Inserted SQL is
never rescanned for placeholders. Missing, duplicate and unused bindings fail.

Snippet function choice, composition, fixed arguments and NULL policy are data.
For example `CAST(COALESCE($(source), N'0') AS $(destination))` changes NULL's result
to zero without a generator change. A fixed expression producing `nvarchar(100)`
cannot satisfy a requirement for `nvarchar(25)` even though ordinary destination
conformance can accept a narrower expression into a wider target.

## First-slice boundaries

- Exactly one SELECT script with a view/target contract and one query specification;
  no CTEs, nested queries or set queries. Only explicitly selected scalar projections
  are rewritten. Predicates, joins and other occurrences of source columns are not
  rewritten by this operation.
- SQL Server source and destination types must resolve in the selected catalog;
  arbitrary type/system IDs work. Supported physical type names are the sanctioned
  SQL Server syntax names, not arbitrary SQL alias/user-defined types.
- Destination facets are explicit caller inputs in this slice. No source-length
  policy selection or DV/DW physical schema realization is introduced.
- Existing Binding expression inference bounds the accepted inputs and snippets.
  Arithmetic and general function result typing are not added. Unknown facets are
  not evidence that a generated expression meets an exact requested contract.
  In particular, MAX, temporal fractional precision and float precision destinations
  currently fail the exact result-facet check; this operation does not extend
  Binding inference to make them pass.
- No Pipeline coercion, TransformScript interpreter, or conversion-graph change.

Integration coverage is in `TransformScriptConversionIntegrationTests` in the
existing TransformScript Integration-profile project. It persists/reloads real
workspaces, changes snippet data, validates exact resulting contracts, and executes
representative generated SQL on the development SQL Server with NULL verification.
