# Deployment ownership scope

`meta-sql deploy-plan --schema dw` declares one physical SQL Server schema as the
desired-state deployment boundary. Source tables, views, functions and stored
procedures must all belong to that schema. The implicit empty `dbo` schema in SQL
workspaces is permitted. Schema spelling must agree with the source contract.

The manifest stores `DeployManifest.ManagedSchemaName`. Live extraction, difference
planning and live fingerprint validation use that schema. `deploy` reads the scope
from the saved manifest; it has no independent scope override. Changing objects in
other schemas does not invalidate the scoped live fingerprint or schedule their
removal. Changes inside the owned schema still invalidate a stale manifest.

Without `--schema`, the manifest retains the existing whole-database reconciliation
contract. A manifest without a selected schema must not be used as a partial owner
of a database shared with separately deployed objects.

Scoped deployment rejects inbound and outbound cross-schema foreign keys during
planning and again before execution. Filtered extraction cannot represent those
dependencies completely. Use a database-wide contract when such dependencies must
be coordinated. Dependencies in SQL module bodies are not a new deployment graph;
SQL Server still enforces its module dependency rules. Deployment retains its
existing statement-by-statement execution and is not atomic across statements.

Regenerate older deploy manifests after updating the manifest model. Destructive
table/column approvals and source/catalog fingerprint checks remain unchanged.
