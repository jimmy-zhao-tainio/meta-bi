# Contributing

Thank you for contributing to `meta-bi`. Read
[`docs/META-MODELING-GUARDRAILS.md`](docs/META-MODELING-GUARDRAILS.md) before
substantial work. Product models are reviewed contracts; implementation
convenience is not sufficient reason to change them.

## Foundation packages

The repository consumes development packages produced by a sibling `meta`
checkout. With both repositories in the same parent directory:

```powershell
cd ../meta
./pack-internal.cmd ../meta-bi/artifacts/meta-foundation-feed
cd ../meta-bi
./scripts/build-test-clis.ps1 -FoundationFeed ./artifacts/meta-foundation-feed
```

The package suffix and script name identify the unreleased development channel,
not private source. Keep the package version aligned between the repositories.

## Models and generated artifacts

- Discuss broad product-model changes before implementation.
- Change model or authoring sources first, then use the established workflow to
  regenerate C#, XML, SQL, reference corpora, or demos.
- Do not hand-edit generated workspace instances to conceal a generator defect.
- Keep generated outputs in the same pull request as the source change and
  explain why each generated area changed.

## Tests and pull requests

Build shared dependencies serially. Run the focused test project for the
affected product, its full product suite, and any demo or reference-corpus gate
whose contract changed. Report exact commands and run `git diff --check` before
submission.

Keep changes scoped. Do not commit credentials, local package feeds, transient
run evidence, or machine-specific paths. Third-party corpus changes must retain
their source, license, modification, and fair-use notices.
