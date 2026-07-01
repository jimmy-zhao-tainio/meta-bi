# MetaDataQuality Phase 2A/2B CLI Integration Sample

This demo exercises the implemented Phase 2A/2B corpus inference scope.

Run:

```cmd
call run.cmd
```

What this demo proves:

- corpus relationship inference is materialized from a TransformScript workspace.
- explicit promotion is still required before SQL conversion.
- conversion fails fast when unsupported promoted families are present.
- conversion succeeds when promotion is limited to supported implied families:
  - `ImpliedForeignKeyMissingReference`
  - `ImpliedUniqueKeyViolation`

Artifacts:

- `TransformWS`
- `DataQualityWS` (all promoted path; expected converter fail-fast)
- `DataQualityWS_Supported` (implied-only promoted path; expected converter success)
- `DataQualityViews.sql`
- `before-promote.output`
- `unsupported.output`

Cleanup:

```cmd
call cleanup.cmd
```
