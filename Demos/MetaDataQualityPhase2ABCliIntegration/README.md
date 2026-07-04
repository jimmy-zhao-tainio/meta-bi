# MetaDataQuality Phase 2A/2B CLI Integration

This demo exercises Phase 2A/2B corpus inference from a generated
`MetaTransformScript` workspace into a `MetaDataQuality` workspace.

Run from the mesh folder:

```powershell
cd Demos\MetaDataQualityPhase2ABCliIntegration\MetaDataQualityPhase2ABCliIntegration.MetaMesh
meta-mesh run --operation cleanup
meta-mesh run --operation build-phase2ab-data-quality
```

The build operation:

- imports repeated transform-script examples that establish dominant and minority relationship evidence;
- generates a data-quality workspace from the transform workspace;
- generates a pre-promotion SQL pack with no candidate views;
- promotes all candidates once and generates the full SQL pack;
- regenerates a second data-quality workspace and promotes only the supported implied candidate families:
  - `ImpliedForeignKeyMissingReference`
  - `ImpliedUniqueKeyViolation`
- generates `DataQualityViews.sql` from the supported implied candidates.

Generated outputs are ignored:

- `TransformWS`
- `DataQualityWS`
- `DataQualityWS_Supported`
- `BeforePromote.sql`
- `AllPromoted.sql`
- `DataQualityViews.sql`
