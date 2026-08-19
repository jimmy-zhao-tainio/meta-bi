# MetaTransformPattern

MetaTransformPattern models reusable SQL transform shapes. A pattern is an
ordered sequence of literal SQL text and named placeholders. It belongs in a
clean definition workspace that can be shared and versioned without collecting
project-specific uses.

MetaTransformPatternInstance is the separate model for concrete uses. Each
instance identifies one reusable pattern by stable modeled identity and carries
one SQL fragment for each placeholder. If a placeholder occurs more than once
in a pattern, every occurrence receives the same value.

The split is the working contract:

```text
MetaTransformPattern + MetaTransformPatternInstance
                    TransformPatternToSqlScript weave
                              ↓
                         MetaSqlScript
                              ↓
                   MetaTransformScript SQL parser
                              ↓
                    MetaTransformScript
```

The weave performs ordered rendering and substitution only. MetaSqlScript is a
small text-bearing workspace at the SQL boundary. The existing C# SQL parser
then imports the generated SQL into the semantic MetaTransformScript model used
by binding, pipelines, and execution. SQL parsing is not part of WeaveScript.

This supports recurring transform families: the same load shape can be used for
hundreds of modeled source and target entities while table references, field
lists, expressions, and predicates vary per instance.

## Pattern definitions

`meta-transform-pattern` exposes a reusable pattern as an editable SQL-like text
projection. Placeholder references use `$(name)`. Lists are supplied as one SQL
fragment:

```sql
INSERT INTO $(target) ($(target-fields))
SELECT $(source-expressions)
FROM $(source);
```

The projection is read from standard input:

```text
meta-transform-pattern create --xml <pattern-workspace>
meta-transform-pattern add-pattern \
  --workspace <pattern-workspace> \
  --id insert-select \
  --name "Insert select" < insert-select.pattern.sql

meta-transform-pattern emit-pattern \
  --workspace <pattern-workspace> \
  --id insert-select
```

`update-pattern` replaces the modeled item sequence from the same projection.
`$$(` represents literal `$(` in SQL text.

## Pattern instances

Concrete instances are authored in their own workspace:

```text
meta-transform-pattern create-instance-workspace --xml <instance-workspace>

meta-transform-pattern add-instance \
  --workspace <instance-workspace> \
  --pattern-workspace <pattern-workspace> \
  --id load-customer \
  --name LoadCustomer \
  --pattern insert-select

echo [dbo].[Customer] | meta-transform-pattern set-binding \
  --workspace <instance-workspace> \
  --pattern-workspace <pattern-workspace> \
  --instance load-customer \
  --placeholder target
```

`set-binding` replaces a placeholder with the SQL fragment read from standard
input. `clear-binding` sets that scalar fragment to empty text. `show` reports reusable
definitions; `show-instances` reports concrete instances and placeholder
coverage.

Materialization takes both source workspaces:

```text
meta-convert transform-pattern-to-sql-script \
  --pattern-workspace <pattern-workspace> \
  --instance-workspace <instance-workspace> \
  --output-xml <meta-sql-script-workspace>
```
