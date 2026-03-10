# Business-derived BDV column intent

## The issue

`MetaDataVaultImplementation` already owns the mandatory technical Data Vault columns:

- hash keys
- parent hash keys
- load timestamp
- record source
- hashdiff
- PIT helper columns
- bridge helper columns

That part is coherent.

The unresolved part is different:

- `BusinessHubKeyPart`
- `BusinessHubSatelliteAttribute`
- `BusinessHubSatelliteKeyPart`
- `BusinessLinkSatelliteAttribute`
- `BusinessLinkSatelliteKeyPart`

These rows imply SQL columns in a materialized Business Data Vault, but the current model stack does not yet say what their physical data types are.

That leaves any future SQL emitter hanging.

## What makes this hard

There are two different kinds of column meaning in a Data Vault:

### 1. Standard technical columns

These are house-standardized by table family.

Examples:

- hub hash key
- satellite hashdiff
- load timestamp
- record source

These belong naturally in `MetaDataVaultImplementation`.

### 2. Business-derived columns

These depend on the actual business content being modeled.

Examples:

- the business key part `CustomerNumber`
- the satellite attribute `CustomerName`
- the multi-active qualifier `AddressType`

These are not fixed once per table family. They vary per business row and per vault structure.

## The real design question

Where should the physical typing of those business-derived columns live?

## Option 1. Put it in `MetaBusiness`

Meaning:
- `BusinessKeyPart` and possibly other business-side entities own `DataTypeId` and detail rows.

### Pros
- business keys would be fully specified at the business level
- business identity would be more concrete and actionable

### Cons
- `MetaBusiness` would start carrying physical storage detail
- the same business concept may be realized differently in different downstream models
- satellite attributes are not naturally business-model concepts at all
- it would mix business semantics and physical realization too early

## Option 2. Put it in `MetaBusinessDataVault`

Meaning:
- every BDV row that emits a business-driven SQL column owns its own `DataTypeId`
- and local `...DataTypeDetail` rows if needed

### Pros
- the model that causes the SQL column to exist also owns its physical declaration
- `meta-datavault-business materialize-business` can produce a SQL-ready BDV workspace without guessing
- the future SQL emitter can be deterministic
- this matches the existing `MetaSchema` pattern:
  - base `DataTypeId`
  - local `...DataTypeDetail`

### Cons
- `MetaBusinessDataVault` becomes more implementation-aware
- if you later support several competing physical implementations of the same BDV structure, some duplication may appear

## Option 3. Put it in weave or fabric

Meaning:
- cross-model binding rows also carry physical typing for the resulting BDV columns.

### Pros
- keeps business and BDV models cleaner on paper
- typing could be expressed exactly where business meaning is bound to vault structure

### Cons
- weave/fabric would stop being about correspondence and start owning physical declaration
- authoring would become much more complex
- the same SQL-emitting row would not be self-describing without the binding layer
- this would overload fabric with concerns that are not really about scope or consistency

## Option 4. Introduce another sanctioned model

Meaning:
- add a dedicated sanctioned model for business-derived vault column declarations.

### Pros
- very explicit separation of concerns
- reusable if many downstream models need the same column-declaration layer

### Cons
- another model tax now
- more authoring overhead
- likely premature before there is a second real consumer besides BDV SQL generation

## Current repo decision

This direction is now adopted in `meta-bi`:

- business-derived BDV column rows own `DataTypeId`
- local variability is expressed through `...DataTypeDetail` rows
- `MetaDataVaultImplementation` remains responsible only for standardized technical DV columns

## Recommended direction

For the current stage of `meta-bi`, the best tradeoff is:

1. keep `MetaDataVaultImplementation` for the standardized technical columns only
2. put business-derived SQL column typing directly on `MetaBusinessDataVault`
3. use the same local pattern already used successfully in `MetaSchema`:
   - `DataTypeId`
   - local `...DataTypeDetail` rows
4. keep weave/fabric focused on correspondence and scoped consistency, not physical typing

That means:

- `BusinessHubKeyPart` should own `DataTypeId`
- `BusinessHubSatelliteAttribute` should own `DataTypeId`
- `BusinessHubSatelliteKeyPart` should own `DataTypeId`
- `BusinessLinkSatelliteAttribute` should own `DataTypeId`
- `BusinessLinkSatelliteKeyPart` should own `DataTypeId`
- if exact length / precision / scale / provider-specific detail matters, attach local detail rows there

## Why this is the simplest correct move

It keeps the stack readable:

- `MetaBusiness` owns business meaning
- `MetaBusinessDataVault` owns vault structure and the business-driven columns it causes to exist
- `MetaDataVaultImplementation` owns fixed house conventions for technical columns
- weave/fabric own cross-model alignment and scoped consistency

That is not theoretically perfect, but it is closed enough for real generation and simple enough for users to understand.

## What this avoids

This avoids three bad outcomes:

- forcing physical detail into `MetaBusiness`
- turning fabric into a physical declaration language
- inventing another sanctioned model too early

## Trigger for revisiting this decision

Revisit the decision only if one of these becomes true:

- more than one downstream model needs the same business-derived column declaration layer
- several physical implementations of the same BDV structure must coexist cleanly
- BDV typing starts to require cross-model policy selection rather than local declaration

Until then, the simplest stable answer is:

`MetaBusinessDataVault` should own the physical typing of its business-derived columns.

