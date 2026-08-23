# Transform Pattern to SQL Script Weave

This workspace is the sanctioned forward MetaWeave correspondence from
`MetaTransformPattern` and `MetaTransformPatternInstance` to `MetaSqlScript`.

The pattern source contains reusable definitions: ordered SQL text and
placeholder items. The instance source contains concrete instances and one SQL
fragment for each placeholder. The weave follows the modeled pattern-item
sequence, substitutes each scalar fragment wherever its placeholder occurs, and
creates one SQL script per instance.

The weave validates pattern resolution, item shape, ownership, sequence shape,
and placeholder coverage before materialization. It does not parse SQL.
MetaSqlScript is the small workspace contract at the SQL language boundary; the
existing MetaTransformScript SQL importer parses its statements into the
semantic MetaTransformScript model.

Execute the weave directly:

```text
meta-weave execute \
  --workspace . \
  --source-workspace pattern=<meta-transform-pattern-workspace> \
  --source-workspace instance=<meta-transform-pattern-instance-workspace> \
  --target-workspace ../../../MetaSqlScript/Workspace \
  --xml <new-meta-sql-script-workspace>
```

The packaged path is available through:

```text
meta-convert transform-pattern-to-sql-script \
  --pattern-workspace <meta-transform-pattern-workspace> \
  --instance-workspace <meta-transform-pattern-instance-workspace> \
  --output-xml <new-meta-sql-script-workspace>

meta-transform-script from sql-script-workspace \
  --source-workspace <meta-sql-script-workspace> \
  --output-xml <new-meta-transform-script-workspace>
```
