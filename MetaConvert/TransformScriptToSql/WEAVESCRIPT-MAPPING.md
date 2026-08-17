# TransformScript to SQL Weave Mapping

This ledger maps the retired forward C# conversion to the sanctioned
`MetaTransformScript` to `MetaSql` weave. The weave is the executable product
definition. A frozen copy of the former C# conversion remains in the test
project only as a temporary equivalence oracle.

The forward call chain is:

1. `TransformScriptToSqlConverter` loads the source workspace and the packaged
   sanctioned weave.
2. MetaWeave executes the `forward` direction against an empty `MetaSql`
   target contract with the supplied database name.
3. `meta-convert` writes the resulting workspace through the selected Meta
   surface.

The product path does not invoke the former C# emitter or place a pre-rendered
SQL definition in an intermediate workspace.

## Conversion order

| Step | C# responsibility | WeaveScript equivalent | State |
| --- | --- | --- | --- |
| 1 | Require a bound, exportable transform workspace | Source-contract validation plus direction requirements | Mapped below |
| 2 | Create `Database` from `databaseName` | One `Database` transformation | Ready |
| 3 | Classify scripts and resolve module identities | Direction relations shared by requirements and target transformations | Implemented |
| 4 | Reject missing parameters or modules, raw statements, duplicate module identities, ambiguous object kinds, and malformed render events | Seven direction requirements over invocation, classification, identity, renderer coverage, and renderer protocol evidence | Implemented for structured module identities |
| 5 | Create distinct `Schema` rows | One `Schema` transformation over `modules` | Implemented and equivalent |
| 6 | Render identifiers, literals, names, types, expressions, predicates, rowsets, queries, and statements | Composed render-event relations plus a shared recursive walk | Implemented across the established emitter surface; `RendererCoverage` retains malformed and unknown-subtype rejection |
| 7 | Add view, TVF, scalar-function, or stored-procedure envelope | Root render events selected by module kind | View, including explicit output columns, TVF, scalar-function, and stored-procedure envelopes implemented for the covered renderer domain |
| 8 | Assign stable deployment ordinals | `ROW_NUMBER() OVER (ORDER BY SchemaName, ObjectName, ModuleKind)` | Implemented |
| 9 | Create `View`, `Function`, and `StoredProcedure` | Three target transformations using the rendered module relation | Implemented for the covered renderer domain |
| 10 | Compare complete target workspaces | Weave-versus-C# equivalence tests over the reference corpus | Mixed-module and focused renderer witnesses are permanent tests; a combined workspace containing all 61 claimed corpus modules matched the C# target |

Target materialization itself is already ordered by the `MetaSql` model:
`Database`, then `Schema`, then the schema-owned module populations.
`DeployOrdinal` records SQL-module deployment order; it is not MetaWeave
execution order.

## First direct translations

The database translation is exact:

```sql
SELECT
    @databaseName AS Id,
    @databaseName AS Name,
    NULL AS Collation;
```

The module catalog is a relational form of `ResolveScriptObjectType` and
`ResolveSqlModuleIdentity`. CREATE FUNCTION and CREATE PROCEDURE imports carry
typed script-object rows. A normal CREATE VIEW is instead identified by its
modeled `SelectStatement`; `ScriptObjectView` is optional target evidence, not
the sole view discriminator. Classification therefore follows the C#
precedence exactly: explicit TVF, scalar function, stored procedure, explicit
view, function-parameter inference, select-statement inference, then raw
statement.

All CREATE-module forms carry structured schema and object identifier links.
After classification, the common catalog joins those links once:

```sql
WITH classifications AS
(
    SELECT
        s.Id AS TransformScriptId,
        CASE
            WHEN EXISTS (SELECT o.Id AS Id FROM transform.ScriptObjectTVF AS o WHERE o.TransformScriptId = s.Id) THEN 'InlineTableValuedFunction'
            WHEN EXISTS (SELECT o.Id AS Id FROM transform.ScriptObjectScalarFunction AS o WHERE o.TransformScriptId = s.Id) THEN 'ScalarFunction'
            WHEN EXISTS (SELECT o.Id AS Id FROM transform.ScriptObjectStoredProcedure AS o WHERE o.TransformScriptId = s.Id) THEN 'StoredProcedure'
            WHEN EXISTS (SELECT o.Id AS Id FROM transform.ScriptObjectView AS o WHERE o.TransformScriptId = s.Id) THEN 'View'
            WHEN EXISTS (SELECT p.Id AS Id FROM transform.TransformScriptFunctionParametersItem AS p WHERE p.TransformScriptId = s.Id) THEN 'InlineTableValuedFunction'
            WHEN EXISTS
            (
                SELECT q.Id AS Id
                FROM transform.TransformScriptStatementLink AS l
                INNER JOIN transform.StatementWithCtesAndXmlNamespaces AS b ON b.TSqlStatementId = l.TSqlStatementId
                INNER JOIN transform.SelectStatement AS q ON q.StatementWithCtesAndXmlNamespacesId = b.Id
                WHERE l.TransformScriptId = s.Id
            ) THEN 'View'
            ELSE 'RawStatement'
        END AS ModuleKind
    FROM transform.TransformScript AS s
),
modules AS
(
    SELECT c.TransformScriptId AS TransformScriptId, c.ModuleKind AS ModuleKind,
           si.Value AS SchemaName, oi.Value AS ObjectName
    FROM classifications AS c
    INNER JOIN transform.TransformScriptSchemaIdentifierLink AS sl ON sl.TransformScriptId = c.TransformScriptId
    INNER JOIN transform.Identifier AS si ON si.Id = sl.IdentifierId
    INNER JOIN transform.TransformScriptObjectIdentifierLink AS ol ON ol.TransformScriptId = c.TransformScriptId
    INNER JOIN transform.Identifier AS oi ON oi.Id = ol.IdentifierId
    WHERE c.ModuleKind <> 'RawStatement'
)
SELECT
    CONCAT(@databaseName, '.', m.SchemaName) AS Id,
    m.SchemaName AS Name,
    @databaseName AS DatabaseId
FROM modules AS m
GROUP BY m.SchemaName;
```

The correspondence now keeps that catalog in direction relations rather than
repeating it in every query. Seventeen relations cover classification, module
identity and ordering, names and data types, five semantic render-event
families, their union, module roots, and rendered module bodies. Requirements
and transformations read those relations through the same WeaveScript table
syntax used for source entities.

`RenderEventProtocol` treats the common event projection as an explicit
contract. It rejects events with neither or both payload forms, incomplete
child references, and duplicate `(ParentKind, ParentId, SlotPath)` keys before
target construction. Ordered select, FROM, COALESCE, and multipart-identifier
events derive their slots from deterministic item numbers, so duplicate,
sparse, and non-zero-based source ordinals cannot create ambiguous event keys.

Its `Database`, `Schema`, opaque `StoredProcedure`, `View`, and `Function`
populations are byte-for-byte identical to the frozen C# oracle after canonical
workspace serialization. The stored-procedure transformation copies the
modeled `DefinitionSql` after the same trim and uses the global module row
number as `DeployOrdinal`. View and function transformations share the
rendered module-body relation and add their distinct SQL envelopes. Function
parameters and return types remain function-specific. Multiline scalar returns
receive the same per-line indentation as the C# envelope.

## Semantic renderer translation

The C# emitter is a recursive semantic dispatch: render a node, render its
owned children in their modeled order, and place punctuation and whitespace
around them. The SQL equivalent is an ordered expansion relation rather than
one large `CASE` expression.

Each retained emitter branch contributes rows shaped like:

```text
ParentKind, ParentId, SlotOrdinal, Token, ChildKind, ChildId
```

A slot contains either a literal token or one semantic child. For example, a
binary query expression contributes its first child, a newline plus `UNION
ALL` plus a newline, and its second child. A function call contributes its
rendered call target, opening parenthesis, ordered parameters and separators,
and closing parenthesis.

The five direction relations `query_render_events`,
`table_render_events`, `scalar_render_events`, `boolean_render_events`, and
`leaf_render_events` partition those branches by semantic parent kind. Their
common projection is combined by `render_events`. The shared module renderer
then has this form:

```sql
WITH render_events AS
(
    -- UNION ALL branches corresponding to C# emitter cases
),
walk AS
(
    -- one root semantic node for each module

    UNION ALL

    -- expand the preceding iteration through render_events;
    -- append a zero-padded SlotOrdinal to SortPath
)
SELECT
    TransformScriptId,
    STRING_AGG(Token, '') WITHIN GROUP (ORDER BY SortPath) AS DefinitionSql
FROM walk
WHERE Token IS NOT NULL
GROUP BY TransformScriptId;
```

This is the direct SQL counterpart of the C# call stack. Recursive CTE
execution supplies the stack, `SortPath` preserves child order, and ordered
`STRING_AGG` assembles the leaf tokens. No syntax is parsed during conversion.

The emitter was ported bottom-up in this order:

1. identifiers, literals, and parameters;
2. multipart and schema-object names;
3. data-type references;
4. scalar expressions and function calls;
5. boolean expressions;
6. table references and joins;
7. query specifications, query expressions, windows, and CTEs;
8. mutation statements;
9. view and function envelopes;
10. stored-procedure pass-through.

Each completed family is checked against the corresponding C# renderer.
`RendererCoverage` rejects semantic subtypes and options for which no valid
render event can be formed, so conversion cannot silently emit a partial SQL
definition.

The current query boundary includes simple and searched `CASE`, SQL data-type
references, `CAST`/`CONVERT` and their `TRY_` forms, parsing calls, ordinary and
qualified scalar calls, scalar subqueries, comparison, range, membership,
pattern, distinctness, null, and existence predicates, query-level grouping,
having, ordering, `TOP`, and offset/fetch. It also includes `WITHIN GROUP`,
inline and named windows, partitions, ordered expressions, window frames,
binary query expressions, query parentheses, and ordered CTEs. Named tables,
query-derived tables, inline `VALUES` tables, schema-object and global table
functions, XML `nodes()` rowsets, full-text rowsets, pivot and unpivot rowsets,
and their applicable aliases and column lists are rendered. Named-table hints
and `TABLESAMPLE` clauses retain their modeled order and argument styles. A
recursive CTE using an earlier CTE and referring to itself is a focused
equivalence witness. `WITH XMLNAMESPACES` alias and default elements share the
same ordered `WITH` relation and compose with CTEs. Explicit view output-column
lists are rendered by the view envelope rather than by the recursive query
walk.

No C# view/function emitter family is knowingly excluded from the conversion
renderer. `RendererCoverage` still rejects unknown or malformed semantic
subtypes and options that the established C# emitter itself rejects. This is a
conversion-coverage statement, not an expansion of the WeaveScript authoring
surface.

## Deliberate boundaries

Two details remain explicit:

- View classification without `ScriptObjectView` is normal and is now
  reproduced from the modeled select statement. The former C# path also parsed
  a schema-qualified `TransformScript.Name` when structured identifier links
  were absent. The sanctioned weave requires the modeled schema and object
  identifier links. SQL import creates those links.
- The C# emitter uses platform newlines in memory. Meta's canonical XML surface
  writes `\n`, and the C# and weave outputs are byte-for-byte identical there.
  Whitespace-only SQL literals are normalized during import, so the view
  renderer derives its newline indentation token with `SUBSTRING` from a
  nonblank multiline literal. No character-construction function is needed.

The recursive walk also established one execution constraint: a recursive CTE
may depend on an earlier nonrecursive `UNION ALL` CTE. Recursive inspection is
therefore nest-safe; evaluating the event union cannot erase the walk's
self-reference evidence. The TransformScript renderer now uses that capability
for modeled `WITH` clauses and recursive query expressions, rather than only
for its own token walk.

Recursive CTEs, ordered `STRING_AGG`, `ROW_NUMBER`, `CONCAT`, `RIGHT`,
`REPLACE`, `CASE`, and ordinal conversion are already executable in
WeaveScript. No further language form is assumed by this ledger.
