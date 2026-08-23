# MetaMultiDimensionalHierarchyQueryCliIntegration

This demo authors a small `MetaAnalytics` hierarchy model, converts it to
`MetaMultiDimensional`, deploys it to a local SSAS multidimensional instance,
and queries it with MDX.

The mesh uses the original demo names:

- source SQL database: `MetaBiMultiDimensionalHierarchySource`
- SSAS server: `localhost\MULTI`
- SSAS database: `MetaBiMultiDimensionalHierarchyDemo`
- SSAS SQL login: `NT Service\MSOLAP$MULTI`

Set the SQL connection environment variables:

```powershell
$env:META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_ADMIN_SQL = "Data Source=localhost;Initial Catalog=master;Integrated Security=True;TrustServerCertificate=True"
$env:COMMERCE_DW = "Data Source=localhost;Initial Catalog=MetaBiMultiDimensionalHierarchySource;Integrated Security=True;TrustServerCertificate=True"
```

Run from the mesh folder:

```powershell
cd Demos\MetaMultiDimensionalHierarchyQueryCliIntegration\MetaMultiDimensionalHierarchyQueryCliIntegration.MetaMesh
meta-mesh validate --operation cleanup
meta-mesh validate --operation deploy-and-query-hierarchy
meta-mesh run --operation cleanup
meta-mesh run --operation deploy-and-query-hierarchy
```
