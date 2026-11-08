# Apply authored conversions to source-column reads

```powershell
meta-transform-script apply-conversions `
  --workspace Transform `
  --source-schema SourceSchema `
  --data-type-workspace Types `
  --data-type-conversion-workspace Conversions `
  --output-xml ConvertedTransform
```

The operation processes the collection of scripts in a TransformScript workspace.
Source MetaSchema evidence identifies each original source-column read and its
type. Repeat `--source-schema` for statements reading multiple databases, including
existing destination rows. These are read contracts, not destination requirements.
A matching authored DataTypeMapping supplies the destination type and
conversion implementation. There is no target-schema input or requirements file.
A source type without a rule is unchanged; multiple rules for a source type in
the selected conversion workspace are ambiguous and fail.

The CLI also supports `--output-csharp` and `--output-sql` (with
`--output-connection-env`). Exactly one output representation is required. The
output must be new and separate from all inputs. Conversion completes in memory
before output materialization; rejected conversions leave the inputs intact.

## Ownership and call chain

1. The modeled MetaTransformScript CLI loads the selected workspaces through its
   workspace runtime and calls `TransformScriptConversionWorkspaceService.Convert`
   in MetaConvert. The programmatic `Apply` wrapper loads and writes XML workspaces.
2. MetaSchema validates SQL Server source types through the selected MetaDataType
   catalog and its modeled DataTypeSystem relationships. Arbitrary catalog IDs work.
3. Binding's read-only source analysis resolves column references, including SQL
   aliases and JOIN predicates. It selects no conversion rule and knows no snippets.
4. MetaConvert selects the source-type rule. MetaDataTypeConversion owns its
   reusable snippet and parameter bindings; `SqlConversionSnippetService`
   instantiates the authored expression using the existing substitution syntax.
5. TransformScript.Sql parses and structurally composes the expression around each
   original source-column occurrence, including occurrences inside projection
   expressions, predicates, joins and ordering. Parentheses preserve precedence.
   Inserted references are not recursively converted. Generic workspace mapping
   copies the input graph, retaining script identities, module envelopes, declared
   procedure contracts and metadata. Structural substitution preserves targets,
   column order and aliases, including implicit direct-column output names.
   Output-alias references remain output-alias references. Schema-resolved stars
   containing applicable source reads become explicit ordered projections.
6. The caller independently runs schema-aware Binding against its destination
   contract and executes the validated result through Pipeline. Conversion does
   not approve compatibility or validate the output against a target schema.

## Authored snippet data

`ConversionImplementation.SqlConversionSnippet` references a reusable
`SqlConversionSnippet` whose `TransformScript` contains one scalar expression.
Function choice, composition, fixed arguments, facets and NULL policy are authored
data. For example:

```sql
CAST($(source) AS $(destination)(25))
CONVERT($(destination), COALESCE($(source), N'0'))
```

Each `SqlConversionParameterBinding` belongs to the snippet, names a placeholder,
and references a `SqlConversionParameter`. Supported parameter names are:

| Parameter name | Supplied value |
| --- | --- |
| `SourceExpression` | The original source-column reference, parenthesized |
| `DestinationType` | The mapping's catalog-resolved SQL Server type name |

Facet arguments such as `(25)` belong in the authored snippet. They are not
invented from the source or supplied by a separate destination-requirements API.
Placeholder names are case-insensitive and permit surrounding whitespace.
`$(name)` substitutes; `$$(name)` emits literal `$(name)`. Inserted text is never
rescanned for placeholders. Missing, duplicate and unused bindings fail clearly.

## Supported boundaries

- Statement coverage follows TransformScript and its existing Binding source
  analysis: SELECT, INSERT SELECT (including CTEs), UPDATE, DELETE and MERGE,
  with joins, nested/set queries, grouping, windows and schema-resolved stars.
  Reads in expressions and predicates are converted; write destinations are not.
  Statements with no applicable reads, such as TRUNCATE and literal INSERT VALUES,
  remain unchanged. Function envelopes and opaque stored-procedure invocation
  contracts are preserved; this does not inspect or rewrite external procedure bodies.
  Column-valued method call targets remain explicitly unsupported for substitution.
- Source and mapped destination types must resolve to SQL Server through the
  selected catalog. Physical destination names must be supported SQL Server syntax
  names; arbitrary SQL alias/user-defined type names are unsupported.
- MAX-length snippets and `time`, `datetime2`, `datetimeoffset` and `float`
  destinations remain explicitly unsupported before output is written. Binding's
  existing result-facet limits are unchanged and belong to a separate task.
- Supported source analysis is bounded by existing Binding syntax support. This
  operation adds no general function/arithmetic result typing. Final Binding may
  reject a resulting expression whose contract it cannot establish.
- No conversion graph, Pipeline coercion or DV/DW physical schema generation is
  introduced. Type/facet validity of the final expression remains Binding's job.

`TransformScriptConversionIntegrationTests` exercises persisted XML/C# workspaces,
the modeled CLI, changed authored snippets, arbitrary catalog IDs, independent
Binding and actual SQL Server Pipeline execution with exact values and NULLs.
