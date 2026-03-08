# MetaDataVault materialization note

## Current split

`meta-datavault` has two distinct responsibilities:

1. native Data Vault workspace authoring and management
2. sanctioned materialization from other metadata workspaces

Those are both valid.

## Current implemented contract gate

The first implemented Business Vault materialization step is not generation. It is a sanctioned contract check:

```cmd
meta-datavault check-business-materialization --business-workspace <path> --bdv-workspace <path> --implementation-workspace <path> --weave-workspace <path> [--weave-workspace <path> ...] --fabric-workspace <path> [--fabric-workspace <path> ...]
```

This command currently validates that the input set is coherent and complete enough for future Business Data Vault materialization.

## Current sanctioned inputs

The current Business Data Vault materialization contract consumes:

- one `MetaBusiness` workspace
- one `MetaBusinessDataVault` workspace
- one `MetaDataVaultImplementation` workspace
- one or more `MetaWeave` workspaces
- one or more `MetaFabric` workspaces

## Why each input exists

### MetaBusiness

Owns business-side intent:

- `BusinessObject`
- `BusinessKey`
- `BusinessKeyPart`
- `BusinessRelationship`
- `BusinessRelationshipParticipant`

### MetaBusinessDataVault

Owns the sanctioned Business Data Vault structure family:

- hubs
- links
- satellites
- PIT
- bridge

### MetaDataVaultImplementation

Owns fixed implementation defaults that do not belong in the sanctioned BDV model itself:

- table name patterns
- fixed mandatory technical column names
- datatype defaults for those columns

### MetaWeave

Owns flat direct anchors such as:

- `BusinessHub.Name -> BusinessObject.Name`
- `BusinessLink.Name -> BusinessRelationship.Name`

### MetaFabric

Owns scoped child-row consistency over weave workspaces, such as:

- `BusinessLinkEnd.RoleName -> BusinessRelationshipParticipant.RoleName`
- `BusinessHubKeyPart.Name -> BusinessKeyPart.Name`

where the child binding is only deterministic under a scoped parent binding.

## Current required anchor set

Flat anchors:

- `BusinessHub.Name -> BusinessObject.Name`
- `BusinessLink.Name -> BusinessRelationship.Name`

Scoped anchors:

- `BusinessLinkEnd.RoleName -> BusinessRelationshipParticipant.RoleName`
- `BusinessHubKeyPart.Name -> BusinessKeyPart.Name`

The contract check currently requires all four.

## What this does not do yet

The command does not yet:

- materialize a Business Data Vault workspace
- rewrite or derive BDV rows
- generate SQL or SSDT artifacts

It is a sanctioned gate for future materialization work.

## Why this is still useful

This keeps the future materializer honest.

If the sanctioned input set is incomplete or inconsistent, the failure happens before any workspace generation logic is introduced.

## Current proved sample set

The current sanctioned repeated-key-part sample set is:

- `MetaBusiness.Workspaces/SampleBusinessCommerceRepeatedKeyPart`
- `MetaDataVault.Workspaces/SampleBusinessDataVaultCommerceRepeatedKeyPart`
- `Weaves/Weave-MetaBusiness-MetaBusinessDataVault-HubObject-Commerce-RepeatedKeyPart`
- `Weaves/Weave-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce`
- `Weaves/Weave-MetaBusiness-MetaBusinessDataVault-LinkRelationship-Commerce-RepeatedKeyPart`
- `Weaves/Weave-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce-RepeatedKeyPart`
- `Fabrics/Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce`
- `Fabrics/Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce-RepeatedKeyPart`
