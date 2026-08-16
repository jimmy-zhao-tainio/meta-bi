# meta-bi

`meta-bi` is the business-intelligence product layer built on the
[`meta`](https://github.com/jimmy-zhao-tainio/meta) workspace foundation. It
makes source contracts, transformations, bindings, data quality, pipelines,
orchestration, and analytical targets explicit as sanctioned models.

The same modeled structure can be carried through supported XML, SQL, and C#
workspace surfaces. `meta` owns the representation-neutral foundation;
`meta-bi` owns the BI domain models and the tools that compile, validate, and
operate them.

## Documentation

The canonical public reference is [metametabi.com/docs.html](https://metametabi.com/docs.html).
The repository-level [documentation index](docs/README.md) links design notes
and product boundaries that complement the modeled reference. Use
`meta-<tool> help` for local command help.

## Status

The repository is under active pre-release development. Product areas have
explicit, bounded contracts and do not all have the same implementation depth;
the documentation states supported and unsupported surfaces rather than using
a single maturity label for the whole stack.

## Quick start

The current build uses .NET 8 on Windows and consumes development packages from
a sibling `meta` checkout. With both repositories in the same parent directory:

```powershell
cd ../meta
./pack-internal.cmd ../meta-bi/artifacts/meta-foundation-feed
cd ../meta-bi
./scripts/build-test-clis.ps1 -FoundationFeed ./artifacts/meta-foundation-feed
```

The checked-in demos are inspectable without live services. Running complete
SQL Server or Analysis Services scenarios requires the environment described
by the individual demo.

See [CONTRIBUTING.md](CONTRIBUTING.md) for modeling rules, focused tests, and
generated-artifact expectations.

## License and third-party material

The repository is licensed under the [Apache License 2.0](LICENSE). See
[THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) for the corpus derived from
TPC-DS and the externally downloaded AdventureWorks sample database.
