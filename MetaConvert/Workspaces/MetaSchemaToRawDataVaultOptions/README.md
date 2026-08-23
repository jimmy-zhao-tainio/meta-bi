# MetaSchema to Raw Data Vault Options

This C# workspace is the typed option contract used by the sanctioned
`MetaSchema` to `MetaRawDataVault` weave.

`ConversionOptions` is the single option-set root. An `IncludeViewsOption`
record enables view inclusion. `IgnoredFieldName` and `IgnoredFieldSuffix`
records carry repeatable values without encoding collections into scalar
parameters. Their values are compared case-insensitively and are otherwise
preserved verbatim.

The sanctioned instance contains only the root and therefore represents the
default conversion: tables are included, views are not, and no fields are
ignored. `meta-convert` creates an invocation-specific instance of this same
contract from its command-line options.
