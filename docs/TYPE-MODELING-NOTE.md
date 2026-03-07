# Type Modeling Note

## Problem

Type modeling becomes unstable when one concept is forced to carry four different meanings at once:

- a stable type vocabulary item
- a concrete declaration at one usage site
- variable parameters on that declaration
- a house policy or implementation default

Examples:

- `nvarchar`
- `CustomerName nvarchar(255) null`
- `length=255`
- `HashKey should be binary(16)`

If those are modeled as one thing, the model either explodes into an infinite catalog of concrete variants, or it becomes incomplete and pushes the real logic into tooling.

## Core issue

The mistake is to model concrete declarations as if they were global identities.

That leads to two bad outcomes:

1. a global catalog of pseudo-types such as `nvarchar(1)`, `nvarchar(2)`, ..., `nvarchar(max)`
2. a weak model where the generator still has to guess physical details

The problem is not specific to SQL Server. It appears anywhere a stable type name can be combined with variable parameters.

## Better split

A cleaner modeling strategy is to separate:

- stable type identity
- local type usage
- variable parameters on that usage
- reusable implementation defaults

One useful vocabulary for that split is:

- `TypeSystem`
- `TypeFamily`
- `TypeBinding`
- `TypeParameter`
- `TypePolicy`
- `TypePolicyParameter`

## Proposed concepts

### `TypeSystem`

Represents one family of type names.

Examples:

- SQL Server
- Meta
- SSIS
- Snowflake

### `TypeFamily`

Represents a stable type identity within a type system.

Examples:

- `sqlserver:decimal`
- `sqlserver:nvarchar`
- `meta:String`
- `meta:Decimal`

This is the global vocabulary layer.

### `TypeBinding`

Represents one actual use of a type in one concrete place.

Examples:

- field `CustomerName`
- field `Amount`
- Data Vault column slot `HashKey`
- implementation slot `RecordSource`

A `TypeBinding` references one `TypeFamily`.

This is where type becomes real in context.

### `TypeParameter`

Represents variable detail attached to one `TypeBinding`.

Examples:

- `Length = 255`
- `Precision = 18`
- `Scale = 2`
- `TimePrecision = 7`
- `Unicode = true`

This is the open-ended part. It avoids hardcoding one fixed set of properties into every consuming model.

### `TypePolicy`

Represents a reusable implementation rule or house-style default.

Examples:

- `RawHub.HashKey`
- `RecordSourceColumn`
- `Bridge.Path`

A `TypePolicy` references one `TypeFamily`.

This is not a concrete field. It is a reusable implementation decision.

### `TypePolicyParameter`

Represents variable detail attached to a policy.

Examples:

- `Length = 16`
- `Length = 128`
- `Precision = 18`
- `Scale = 2`

## Example

A source field:

- `Amount decimal(18,2) not null`

Can be understood as:

- `TypeSystem = SQL Server`
- `TypeFamily = decimal`
- `TypeBinding = Field Amount`
- `TypeBinding.IsNullable = false`
- `TypeParameter(Precision, 18)`
- `TypeParameter(Scale, 2)`

A Data Vault implementation rule:

- `HashKey = binary(16)`

Can be understood as:

- `TypePolicy = RawHub.HashKey`
- `TypeFamily = binary`
- `TypePolicyParameter(Length, 16)`

The stable type is reused, but the variable detail stays local to either the usage or the policy.

## Modeling guidance

Use the following rule of thumb:

- if it is stable and shared, model it as `TypeFamily`
- if it exists at one concrete usage site, model it as `TypeBinding`
- if it varies by usage, model it as `TypeParameter`
- if it is a reusable house default, model it as `TypePolicy`
- if it varies within a policy, model it as `TypePolicyParameter`

## Why this matters

This keeps the model finite and explicit:

- no infinite sanctioned `TypeSpec` catalog
- no fake simplification where three scalar properties pretend to capture all type detail
- no hidden generator knowledge for physical details

It also gives different sanctioned models a coherent role:

- type models own stable vocabulary
- schema models own observed source usage
- implementation models own physical defaults and house style

That is the strategy needed when a framework must stay explicit and isomorphic across several technical surfaces.
