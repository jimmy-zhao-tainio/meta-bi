# Schema to Raw Data Vault Weave

This workspace is the sanctioned forward `MetaWeave` correspondence from a
`MetaSchema` source and typed conversion options to `MetaRawDataVault`.

The `schema` source supplies systems, schemas, tables, views, fields, keys,
relationships, and datatype details. The `options` source uses the
`MetaSchemaToRawDataVaultOptions` contract. Its root identifies one option
set; related records select view inclusion and repeatable ignored field names
and suffixes. The default options workspace includes tables and carries no
field exclusions.

The weave selects one usable key for each included schema object, preferring
primary keys, then unique keys, then other modeled keys. It creates Raw hubs,
key parts, hub satellites and attributes, links and link roles while retaining
the source fields and datatype details required by those structures.

When several relationships have the same structural link name, their ordered
source-field names are used verbatim as the preferred disambiguator. A
preferred name used by more than one relationship receives the stable source
relationship identity. This rule is independent of source enumeration order;
it does not sanitize source names or select one relationship as the collision
winner.

Execute the default conversion into a new workspace:

```text
meta-weave execute \
  --workspace . \
  --source-workspace schema=<meta-schema-workspace> \
  --source-workspace options=../../Workspaces/MetaSchemaToRawDataVaultOptions \
  --target-workspace ../../../MetaDataVault/Workspaces/MetaRawDataVault \
  --xml <new-target-workspace>
```

`emit-relation`, `emit-requirement`, and `emit-transformation` expose every
modeled query through the CLI. The `meta-convert schema-to-raw-datavault`
command constructs the typed options input from its ordinary command-line
options and executes this packaged weave. The former C# materializer remains
only in the test reference tree for compatibility proofs.
