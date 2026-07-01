# CLI Interaction Wall

This is a taste file for the `meta` / `meta-bi` command line.

It is not a catalog.
It is not a requirements list.
It is not a set of strings to copy blindly.

It is a wall of terminal moments that should add up to a flavour: quiet, exact, almost bare, but not cold. The user should feel that the tool is awake, careful, and withholding everything that does not help.

The command line already contains the user's sentence.
The response should be the smallest useful change in the room.

## The Feeling

The surface should feel like this:

```text
> meta-data-warehouse --new-workspace Warehouse
Ok

> meta-data-warehouse add-dimension --workspace Warehouse --warehouse Commerce --dimension Customer
Ok

> meta-transform-binding bind --transform-workspace Transform --schema-workspace Schema --out Binding
Binding...|
Binding...Ok

> meta-orchestration inspect-run-plan --workspace Orchestration
DefaultRunPlan
+-- LoadStageCustomer.load-stage-customer
+-- LoadStageOrder.load-stage-order
+-- LoadDimCustomer.load-dim-customer
`-- LoadFactSales.load-fact-sales
```

Nothing here performs.
Nothing explains itself twice.
Nothing leaks an internal id just because the code has one.

The flavour is in the relation between the parts:

- authoring answers with a small acknowledgement
- waiting answers with motion
- movement answers with a quiet meter only when the meter matters
- inspection opens the shape itself
- failure says what cannot continue and what to do next
- density appears only when invited

## Acknowledgement

Successful authoring is almost silent.

```text
> meta-data-warehouse --new-workspace Warehouse
Ok
```

```text
> meta-data-warehouse add-dimension --workspace Warehouse --warehouse Commerce --dimension Customer
Ok
```

```text
> meta-data-warehouse add-fact --workspace Warehouse --warehouse Commerce --fact Sales
Ok
```

```text
> meta-pipeline new-workspace Pipeline
Ok
```

```text
> meta-pipeline add-pipeline --workspace Pipeline --name CustomerLoad
Ok
```

```text
> meta-pipeline add-step --workspace Pipeline --pipeline CustomerLoad --step-name load-customers --script dbo.v_customer_load --binding customer-load --execution-connection-env CUSTOMER_SQL
Ok
```

The command is the action.
`Ok` is not a status report.
It is a small physical acknowledgement that the action landed.

Not this:

```text
OK: Created Warehouse
```

Not this:

```text
Created workspace Warehouse.
```

Not this:

```text
Customer
```

Those all make the tool feel like it is narrating its own machinery.

## Motion

When the tool is busy and the user does not need measurements, the output is one activity line.

```text
> meta-transform-binding bind --transform-workspace Transform --schema-workspace Schema --out Binding
Binding...|
Binding...Ok
```

```text
> meta-transform-script --session --workspace Transform < import.commands
Importing...|
Importing...Ok
```

```text
> meta-sql deploy --deploy-plan-workspace DeployPlan --connection-env META_SQL
Deploying...|
Deploying...Ok
```

The motion is the ordinary terminal spinner:

```text
|
/
-
\
```

But the user should experience one living line, not four printed frames.
It rewrites the same terminal line.
It appears after a short delay, so instant work stays clean.
It disappears into the final result.

The caption is not an object label.
It is one verb for the work currently in flight.
It should be reusable and calm, not improvised per object or command.

Not this:

```text
Binding  |
```

Not this:

```text
Running CustomerLoad  |
```

Not this:

```text
Deploying Commerce  |
```

Those create a different visual object for every command. The label, spacing, and spinner start fighting each other. The activity line says only the class of work and whether it is still alive.

## Meter

A meter is motion with a readout.

It is for work where the live quantity changes the user's confidence: rows moving, bytes moving, tasks running.

```text
> meta-pipeline execute --workspace Pipeline --pipeline CustomerLoad --transform-workspace Transform --binding-workspace Binding --pipeline-db-connection-env META_PIPELINE
|  [================----] 5 of 6  10 rows  155 B/s
[====================] 6 of 6  10 rows  155 B/s
```

```text
> meta-orchestration execute --workspace Orchestration --pipeline-workspace Pipeline --pipeline-db-connection-env META_PIPELINE --max-degree-of-parallelism 2
|  [=============-------] 12 of 18  4 running
[====================] 18 of 18
```

```text
> meta-tabular deploy --workspace Tabular --server localhost\TABULAR --database-name Commerce --drop-existing
|  processing
Deploying...Ok
```

The meter begins with the same spinner in column 1.
The readout is short, factual, and unsentimental.
It is not a sentence.
It is not a diagnostic stream.
It is not the final report.

If the readout cannot be useful, use plain motion.

## Shape

Inspection commands should not acknowledge themselves.
The output is the object.

```text
> meta-orchestration inspect-run-plan --workspace Orchestration
DefaultRunPlan
+-- CleanupScratch.cleanup-scratch
+-- LoadStageCustomer.load-stage-customer
+-- LoadStageOrder.load-stage-order
+-- LoadDimCustomer.load-dim-customer
+-- RefreshExchangeRates.load-work-rates
`-- LoadFactSales.load-fact-sales
```

```text
> meta-transform-binding inspect --workspace Binding
dbo.CustomerLoad
  source dbo.RawCustomer
  target dbo.Customer

dbo.OrderLoad
  source dbo.RawOrder
  target dbo.Order
```

```text
> meta-sql deploy-plan --source-workspace Sql --connection-env META_SQL --out DeployPlan
Deploy plan
  create table dbo.Customer
  alter table dbo.Order add CustomerId
  create index IX_Order_CustomerId
```

```text
> meta-data-quality inspect --workspace DataQuality
Candidates
  Join orphan: dbo.Order.CustomerId -> dbo.Customer.CustomerId
  Outer join null expansion: dbo.Customer -> dbo.Order
  Output duplicate risk: dbo.v_customer_orders
```

No `Ok`.
No "Ready".
No "Workspace".
No counters unless the count is itself the shape.

Not this:

```text
MetaOrchestration:
  RunPlans: 1
  PlannedTasks: 7
  PlannedTaskLocks: 13
```

Those numbers may be true, but the user cannot hold them as a shape.
They are inventory, not interface.

## Cannot Continue

Failure should be plain and useful.

```text
> meta-orchestration inspect-run-plan --workspace Orchestration
Cannot build run plan.

dbo.WorkExchangeRate has ambiguous writes.
Next: add an explicit order or scoped lock policy.
```

```text
> meta-orchestration execute --workspace Orchestration --pipeline-workspace Pipeline --pipeline-db-connection-env META_PIPELINE
Cannot execute orchestration.

Connection environment variable 'META_PIPELINE' was not found.

Next: set the named connection environment variable and retry.
```

```text
> meta-pipeline execute --workspace Pipeline --pipeline CustomerLoad --transform-workspace Transform --binding-workspace Binding --pipeline-db-connection-env META_PIPELINE
|

Cannot complete CustomerLoad.

load-customers could not write dbo.Customer.
Next: inspect the pipeline run, fix the target, then run CustomerLoad again.
```

```text
> meta-tabular deploy --workspace Tabular --server localhost\TABULAR --database-name Commerce --drop-existing
|

Cannot process Commerce.

Sales partition failed against the configured data source.
Next: check credentials and source availability, then deploy again.
```

The first line names what cannot continue.
The middle names the actual obstruction.
The last line gives the next useful move when one exists.

The first line belongs to the command, not to the exception.

Prefer:

```text
Cannot convert analytics to tabular.
```

```text
Cannot deploy tabular database.
```

```text
Cannot update pipeline workspace.
```

The caught exception is the reason underneath.
It should not become the headline unless it already names the command-level intent.

Failure is allowed to speak more than success.

## Continuation

Large systems fail in branches.
The CLI should show that without turning into a dashboard.

```text
> meta-orchestration execute --workspace Orchestration --pipeline-workspace Pipeline --pipeline-db-connection-env META_PIPELINE --max-degree-of-parallelism 2
|  [===============-----] 18 of 24  3 running

Cannot complete orchestration.

DefaultRunPlan stopped with unresolved paths.
  20 succeeded
  1 failed
  3 skipped

First failed
  HRLoad.load-hr
  SQL Server timeout expired.

Skipped
  HRMart.load-hr-mart

Next: inspect the failed pipeline run, then rerun the affected path.
```

This is not a success banner.
It is not a stack trace.
It is the shape of what survived.

## Help

Help is a shape too.

```text
> meta-transform-script --help

Usage:
  meta-transform-script <command> [options]

Commands:
  from
  to

Next: meta-transform-script from --help
```

```text
> meta-orchestration --help

Usage:
  meta-orchestration <command> [options]

Commands:
  add-dependency
  add-order
  allow-concurrent-append
  execute
  explain-issue
  infer
  inspect
  inspect-run-plan
  list-issues
  refresh-run-plan
  set-lock-policy

Next: meta-orchestration help <command>
```

Help should orient, not advertise.

## Explanation

Explanation is opt-in.

```text
> meta-orchestration inspect-run-plan --workspace Orchestration --why
LoadStageCustomer.load-stage-customer
  no predecessors

LoadStageOrder.load-stage-order
  no predecessors

LoadDimCustomer.load-dim-customer
  after LoadStageCustomer.load-stage-customer

RefreshExchangeRates.load-work-rates
  after RefreshExchangeRates.reset-work-rates

LoadFactSales.load-fact-sales
  after LoadDimCustomer.load-dim-customer
  after LoadStageOrder.load-stage-order
```

```text
> meta-orchestration inspect-run-plan --workspace Orchestration --locks
LoadStageCustomer.load-stage-customer
  read   dbo.RawCustomer
  append dbo.StageCustomer

LoadStageOrder.load-stage-order
  read   dbo.RawOrder
  append dbo.StageOrder

LoadDimCustomer.load-dim-customer
  read   dbo.StageCustomer
  append dbo.DimCustomer
```

```text
> meta-pipeline execute --workspace Pipeline --pipeline CustomerLoad --details ...
|

Cannot complete CustomerLoad.

load-customers could not write dbo.Customer.

SQL Server
  Cannot insert the value NULL into column CustomerId.

Pipeline run
  2ddf1870-ffef-4283-a96c-8f27b8fb3dfd

Next: inspect the pipeline run, fix the target, then run CustomerLoad again.
```

Details, locks, ids, row counts, and storage-adjacent facts belong here.
They should not leak into default output.

## Dense Output

Dense output is a drawer the user opens.

```text
> meta-orchestration inspect --workspace Orchestration --details
Plan
  Default

Status
  DAG complete
  deterministic
  synchronization constrained

Rows
  pipelines 6
  objects 9
  task effects 18
  planned tasks 7
```

The same information would be ugly by default.
It is acceptable here because the user asked for the drawer.

## SQL And Deploy

Planning opens a shape.
Deploying does work.

```text
> meta-sql deploy-plan --source-workspace Sql --connection-env META_SQL --out DeployPlan
Deploy plan
  create table dbo.Customer
  alter table dbo.Order add CustomerId
  create index IX_Order_CustomerId
```

```text
> meta-sql deploy-plan --source-workspace Sql --connection-env META_SQL --out DeployPlan
No changes.
```

```text
> meta-sql deploy --deploy-plan-workspace DeployPlan --connection-env META_SQL
Deploying...|
Deploying...Ok
```

```text
> meta-sql deploy-plan --source-workspace Sql --connection-env META_SQL --out DeployPlan
Cannot create deploy plan.

dbo.LegacyCustomer would be dropped.
Next: approve the destructive change explicitly or update the model.
```

## Analytics And SSAS

Conversion is authoring.
Deploy is motion.
Drop is a sharp state change.

```text
> meta-convert analytics-to-tabular --source-workspace Analytics --out Tabular
Ok
```

```text
> meta-tabular deploy --workspace Tabular --server localhost\TABULAR --database-name Commerce --drop-existing
Deploying...|
Deploying...Ok
```

```text
> meta-tabular deploy --workspace Tabular --server localhost\TABULAR --database-name Commerce --drop-existing --no-process
Deploying...|
Deploying...Ok
```

```text
> meta-tabular drop --server localhost\TABULAR --database-name CommerceScratch
Ok
```

```text
> meta-tabular drop --server localhost\TABULAR --database-name CommerceScratch
Cannot drop CommerceScratch.

The database does not exist.
```

## Whole Session

A good session should have rhythm.

```text
> meta-transform-script from sql-file --path customer.sql --target dbo.Customer --workspace Transform
Ok

> meta-transform-script from sql-file --path order.sql --target dbo.Order --workspace Transform
Ok

> meta-transform-binding bind --transform-workspace Transform --schema-workspace Schema --out Binding
Binding...|
Binding...Ok

> meta-pipeline execute --workspace Pipeline --pipeline CustomerLoad --transform-workspace Transform --binding-workspace Binding --pipeline-db-connection-env META_PIPELINE
|  [================----] 5 of 6  10 rows  155 B/s
[====================] 6 of 6  10 rows  155 B/s

> meta-orchestration inspect-run-plan --workspace Orchestration
DefaultRunPlan
+-- LoadStageCustomer.load-stage-customer
+-- LoadDimCustomer.load-dim-customer
`-- LoadFactSales.load-fact-sales
```

That rhythm is the product:

```text
ask
Ok

ask
activity
activity...Ok

ask
shape

ask
cannot continue
Next
```

## Mechanics That Protect The Flavour

The shared CLI presentation layer should make the desired surface easier than the wrong one.

It should expose named moves, not free-form habits:

```text
Ok()
Activity(work)
Meter(work, readout)
Shape(lines)
CannotContinue(intent, reason, next)
Explain(lines)
```

The spinner should be owned by a dedicated renderer/timer so backend work cannot casually starve it.
If the terminal is redirected, animation is suppressed and the final activity line remains.
If work completes before the delay threshold, no spinner appears.
If work continues, the spinner must remain smooth enough that the user never wonders whether the tool froze.

The presentation API should make these hard:

```text
OK: Executed pipeline CustomerLoad
Workspace: C:\...
Created workspace Warehouse.
Binding  |
Binding Customer...|
Customer
```

Those strings are not merely verbose.
They are a different product.
