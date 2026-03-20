The main move is this:

Do not build one giant fuzzy “migration algorithm.”
Build a strict pipeline of tiny classifiers.

First, classify table presence.
Then classify column presence.
Then, only for matched columns, classify definition drift.
Then, only for type drift, classify the type conversion.

Each stage uses an explicit table.
Each stage returns a closed enum.
No stage is allowed to “be clever.”

That gives you a policy machine instead of prose.

1. Hard contract

Use this as the governing contract.

SOURCE defines required schema.
TARGET keeps existing data unless the user explicitly approves destruction.
Absence in SOURCE never implies automatic deletion.
Automatic execution may only do safe, non-destructive, unambiguous changes.
Potentially safe but data-dependent changes require validation against TARGET data.
Destructive or semantic-changing operations may be suggested, but not auto-executed.
If a case is missing from policy tables, BLOCK.
If classification is ambiguous, BLOCK.
2. Closed action model

This is the execution vocabulary. Keep it small.

public enum ActionKind
{
    Nop,
    CreateTable,
    InformExtraTablePreserved,
    CreateColumn,
    PromptNewOrRename,
    PromptRenameTargetColumn,
    InformExtraColumnPreserved,
    AlterColumnSafe,
    ValidateThenAlter,
    SuggestRemediation,
    Block
}

And the reason codes:

public enum ReasonCode
{
    None,

    TableExistsInBoth,
    TableMissingInTarget,
    TableExtraInTargetPreserved,

    ColumnExistsInBothNoDrift,
    ColumnMissingInTargetNewSafe,
    ColumnMissingInTargetNeedsRenameDecision,
    ColumnExtraInTargetPreserved,

    NewNonNullableColumnOnPopulatedTableWithoutDefault,
    RenameRequestedButNoCandidate,
    RenameRequestedButCollision,
    RenameRequestedButDefinitionIncompatible,

    SafeDefinitionDrift,
    DefinitionDriftNeedsValidation,
    DefinitionDriftNeedsExplicitTransform,

    TypeChangeSafeWidening,
    TypeChangeNeedsValidation,
    TypeChangeExplicitTransformRequired,
    TypeChangeUnsupported,

    NoDecisionTableMatch,
    AmbiguousDecisionTableMatch
}
3. Closed normalization types

Codex should normalize raw schema into these tiny facts first.

public enum PresenceKind
{
    SourceOnly,
    TargetOnly,
    Both
}

public enum RowState
{
    Unknown,
    ZeroRows,
    HasRows
}

public enum DefinitionDriftKind
{
    None,
    NullabilityOnly,
    DefaultOnly,
    TypeOnly,
    TypeAndNullability,
    Multiple
}

public enum TypeFamily
{
    Boolean,
    Integer,
    Decimal,
    String,
    Date,
    DateTime,
    DateTimeOffset,
    Guid,
    Binary,
    Json,
    Xml,
    Unknown
}

public enum TypeChangeClass
{
    SameType,
    SafeWidening,
    NeedsDataValidation,
    ExplicitTransformRequired,
    Unsupported
}

Normalized SQL type:

public sealed record NormalizedType(
    TypeFamily Family,
    bool IsUnicode,
    int? Length,
    byte? Precision,
    byte? Scale
);
4. Stage 1: table decision table

This table is very small.

Key:

public sealed record TableDecisionKey(
    PresenceKind Presence
);

Decision rows:

public sealed record TableDecision(
    TableDecisionKey Key,
    ActionKind Action,
    ReasonCode Reason
);

Literal table:

new TableDecision(new(PresenceKind.Both),       ActionKind.Nop,                     ReasonCode.TableExistsInBoth),
new TableDecision(new(PresenceKind.SourceOnly), ActionKind.CreateTable,             ReasonCode.TableMissingInTarget),
new TableDecision(new(PresenceKind.TargetOnly), ActionKind.InformExtraTablePreserved, ReasonCode.TableExtraInTargetPreserved),

That is it.

No dependency-based branching here, because with no metadata and no auto-delete policy, extra target tables are always preserved.

5. Stage 2: column presence decision table

Only used inside matched tables.

Key:

public sealed record ColumnPresenceDecisionKey(
    PresenceKind Presence,
    bool TargetTableHasUnmatchedColumns,
    RowState TargetRowState,
    bool SourceColumnNullable,
    bool SourceColumnHasDefault
);

Action table:

A. Matched by exact name
Presence = Both
=> defer to matched-column definition classifier

This is not a final action yet.

B. Source-only column

These are the exact rules.

1.
Presence = SourceOnly
TargetTableHasUnmatchedColumns = true
(any row state)
(any nullability)
(any default)
=> Action = PromptNewOrRename
=> Reason = ColumnMissingInTargetNeedsRenameDecision

This is important: if there are unmatched target columns, plain diff cannot know whether the source-only column is truly new or is a rename.

2.
Presence = SourceOnly
TargetTableHasUnmatchedColumns = false
TargetRowState = ZeroRows
(any nullability)
(any default)
=> Action = CreateColumn
=> Reason = ColumnMissingInTargetNewSafe
3.
Presence = SourceOnly
TargetTableHasUnmatchedColumns = false
TargetRowState = HasRows
SourceColumnNullable = true
(any default)
=> Action = CreateColumn
=> Reason = ColumnMissingInTargetNewSafe
4.
Presence = SourceOnly
TargetTableHasUnmatchedColumns = false
TargetRowState = HasRows
SourceColumnNullable = false
SourceColumnHasDefault = true
=> Action = CreateColumn
=> Reason = ColumnMissingInTargetNewSafe
5.
Presence = SourceOnly
TargetTableHasUnmatchedColumns = false
TargetRowState = HasRows
SourceColumnNullable = false
SourceColumnHasDefault = false
=> Action = SuggestRemediation
=> Reason = NewNonNullableColumnOnPopulatedTableWithoutDefault

That last row is where the engine becomes useful. It does not just block dumbly. It can attach remediation options like “add as nullable then backfill,” “provide default,” “truncate if user chooses,” but execution still does not do any of those automatically.

C. Target-only column
6.
Presence = TargetOnly
(any other flags ignored)
=> Action = InformExtraColumnPreserved
=> Reason = ColumnExtraInTargetPreserved

Again, preserved by default.

6. Stage 3: matched-column definition decision table

This stage only runs when source and target columns have the same name, or when the user explicitly selected a rename pairing.

Key:

public sealed record MatchedColumnDecisionKey(
    DefinitionDriftKind DriftKind,
    TypeChangeClass TypeChangeClass,
    bool SourceNullable,
    bool TargetNullable,
    bool SourceHasDefault,
    bool TargetHasDefault,
    RowState TargetRowState,
    bool IsExplicitApprovedOperation
);

You can split by drift kind first so Codex cannot merge policies.

A. No drift
DriftKind = None
=> Action = Nop
=> Reason = ColumnExistsInBothNoDrift
B. Default-only drift

I recommend conservative default handling in this phase.

DriftKind = DefaultOnly
=> Action = SuggestRemediation
=> Reason = SafeDefinitionDrift

Why not auto-apply? Because adding a default is safe, but removing or changing one can be intent-sensitive. If you want a more aggressive policy later, you can split DefaultOnly into finer classes. For now, keep it boxed.

C. Nullability-only drift
DriftKind = NullabilityOnly
SourceNullable = true
TargetNullable = false
=> Action = AlterColumnSafe
=> Reason = SafeDefinitionDrift

Relaxing nullability is safe.

DriftKind = NullabilityOnly
SourceNullable = false
TargetNullable = true
TargetRowState = ZeroRows
=> Action = AlterColumnSafe
=> Reason = SafeDefinitionDrift
DriftKind = NullabilityOnly
SourceNullable = false
TargetNullable = true
TargetRowState = HasRows
=> Action = ValidateThenAlter
=> Reason = DefinitionDriftNeedsValidation

Validation means “prove there are no NULL rows.” If validation fails, do not alter. Return remediation suggestions.

D. Type-only drift
DriftKind = TypeOnly
TypeChangeClass = SameType
=> Action = Nop
=> Reason = ColumnExistsInBothNoDrift
DriftKind = TypeOnly
TypeChangeClass = SafeWidening
=> Action = AlterColumnSafe
=> Reason = TypeChangeSafeWidening
DriftKind = TypeOnly
TypeChangeClass = NeedsDataValidation
=> Action = ValidateThenAlter
=> Reason = TypeChangeNeedsValidation
DriftKind = TypeOnly
TypeChangeClass = ExplicitTransformRequired
=> Action = SuggestRemediation
=> Reason = TypeChangeExplicitTransformRequired
DriftKind = TypeOnly
TypeChangeClass = Unsupported
=> Action = Block
=> Reason = TypeChangeUnsupported
E. Type + nullability drift

Be strict here.

DriftKind = TypeAndNullability
TypeChangeClass = SafeWidening
SourceNullable = true
=> Action = AlterColumnSafe
=> Reason = SafeDefinitionDrift
DriftKind = TypeAndNullability
TypeChangeClass = SafeWidening
SourceNullable = false
TargetRowState = ZeroRows
=> Action = AlterColumnSafe
=> Reason = SafeDefinitionDrift
DriftKind = TypeAndNullability
TypeChangeClass = SafeWidening
SourceNullable = false
TargetRowState = HasRows
=> Action = ValidateThenAlter
=> Reason = DefinitionDriftNeedsValidation
DriftKind = TypeAndNullability
TypeChangeClass = NeedsDataValidation
(any nullability)
=> Action = ValidateThenAlter
=> Reason = DefinitionDriftNeedsValidation
DriftKind = TypeAndNullability
TypeChangeClass = ExplicitTransformRequired
=> Action = SuggestRemediation
=> Reason = DefinitionDriftNeedsExplicitTransform
DriftKind = TypeAndNullability
TypeChangeClass = Unsupported
=> Action = Block
=> Reason = TypeChangeUnsupported
F. Multiple drift
DriftKind = Multiple
IsExplicitApprovedOperation = false
=> Action = SuggestRemediation
=> Reason = DefinitionDriftNeedsExplicitTransform
DriftKind = Multiple
IsExplicitApprovedOperation = true
=> still do not auto-execute unless every component drift individually classifies to SafeWidening or NeedsDataValidation and all validations pass.
Otherwise:
=> Action = SuggestRemediation
=> Reason = DefinitionDriftNeedsExplicitTransform

That keeps Codex from “helpfully” combining several risky changes into one alter.

7. Stage 4: type conversion classification matrix

This is the most important part for boxing Codex in.

Do not let it reason from prose like “seems safe.”

Force it to classify using an explicit matrix.

Key:

public sealed record TypeConversionKey(
    TypeFamily SourceFamily,
    TypeFamily TargetFamily,
    bool SourceUnicode,
    bool TargetUnicode,
    int? SourceLength,
    int? TargetLength,
    byte? SourcePrecision,
    byte? TargetPrecision,
    byte? SourceScale,
    byte? TargetScale
);

In practice, you will not enumerate every possible size combination as rows. Instead you do a tiny amount of deterministic preprocessing and then classify into one of a small number of structural relationships:

public enum StructuralTypeRelation
{
    Identical,
    SameFamilyWider,
    SameFamilyNarrower,
    SameFamilyDifferentSemantics,
    CrossFamily,
    Unknown
}

Then the actual policy table uses that.

Key:

public sealed record TypePolicyKey(
    TypeFamily SourceFamily,
    TypeFamily TargetFamily,
    StructuralTypeRelation Relation
);

Policy rows:

Boolean -> Boolean, Identical            => SameType
Integer -> Integer, Identical            => SameType
Decimal -> Decimal, Identical            => SameType
String  -> String,  Identical            => SameType
Date    -> Date,    Identical            => SameType
DateTime -> DateTime, Identical          => SameType
DateTimeOffset -> DateTimeOffset, Identical => SameType
Guid -> Guid, Identical                  => SameType
Binary -> Binary, Identical              => SameType
Json -> Json, Identical                  => SameType
Xml -> Xml, Identical                    => SameType

Safe widening whitelist:

Integer -> Integer, SameFamilyWider      => SafeWidening
Decimal -> Decimal, SameFamilyWider      => SafeWidening
String  -> String,  SameFamilyWider      => SafeWidening
Date    -> DateTime, CrossFamily         => SafeWidening
DateTime -> DateTime, SameFamilyWider    => SafeWidening

Needs validation whitelist:

Integer -> Integer, SameFamilyNarrower   => NeedsDataValidation
Decimal -> Decimal, SameFamilyNarrower   => NeedsDataValidation
String  -> String,  SameFamilyNarrower   => NeedsDataValidation
DateTime -> Date, CrossFamily            => NeedsDataValidation
DateTimeOffset -> DateTime, CrossFamily  => NeedsDataValidation

Explicit transform required:

Boolean -> Integer, CrossFamily          => ExplicitTransformRequired
Integer -> Boolean, CrossFamily          => ExplicitTransformRequired
Integer -> Decimal, CrossFamily          => ExplicitTransformRequired
Decimal -> Integer, CrossFamily          => ExplicitTransformRequired
String  -> Integer, CrossFamily          => ExplicitTransformRequired
Integer -> String, CrossFamily           => ExplicitTransformRequired
String  -> Decimal, CrossFamily          => ExplicitTransformRequired
Decimal -> String, CrossFamily           => ExplicitTransformRequired
Guid    -> String, CrossFamily           => ExplicitTransformRequired
String  -> Guid, CrossFamily             => ExplicitTransformRequired
Xml     -> String, CrossFamily           => ExplicitTransformRequired
Json    -> String, CrossFamily           => ExplicitTransformRequired
String  -> Xml, CrossFamily              => ExplicitTransformRequired
String  -> Json, CrossFamily             => ExplicitTransformRequired

Unsupported default:

anything not matched above => Unsupported

Now define exactly what “SameFamilyWider” means so Codex cannot reinterpret it.

8. Structural widening rules

These are deterministic preprocessors, not policy.

Integer family

Order:

tinyint < smallint < int < bigint

If target rank > source rank, then SameFamilyWider.
If target rank < source rank, then SameFamilyNarrower.
If equal, Identical.

Decimal family

Source (p1,s1) to target (p2,s2).

Treat as SameFamilyWider only if both are true:

p2 >= p1
s2 >= s1
(p2 - s2) >= (p1 - s1)

Treat as SameFamilyNarrower if target can represent strictly less range or less fractional precision.

String family

Only same encoding family qualifies.

So varchar -> varchar and nvarchar -> nvarchar may be wider/narrower.
But varchar <-> nvarchar is not widening. Treat as SameFamilyDifferentSemantics or CrossFamily depending on your normalization. I would classify it as ExplicitTransformRequired.

Wider rule:

target length is null(max) OR
source length and target length both known and target length >= source length

Narrower rule:

both known and target length < source length
Date/time family

Keep this conservative.

Allow these as SafeWidening:

date -> datetime
date -> datetime2
datetime -> datetime2

Allow these as NeedsDataValidation:

datetime2 -> datetime
datetime -> date
datetime2 -> date
datetimeoffset -> datetime2
datetimeoffset -> datetime

Everything else: ExplicitTransformRequired or Unsupported.

9. Rename handling

Do not infer rename automatically.

Use a separate interactive path.

Key:

public sealed record RenameDecisionKey(
    bool UserSelectedRename,
    bool CandidateExists,
    bool NameCollision,
    bool DefinitionsCompatible
);

Table:

UserSelectedRename = false
=> no rename action at all
UserSelectedRename = true
CandidateExists = false
=> Action = Block
=> Reason = RenameRequestedButNoCandidate
UserSelectedRename = true
CandidateExists = true
NameCollision = true
=> Action = Block
=> Reason = RenameRequestedButCollision
UserSelectedRename = true
CandidateExists = true
NameCollision = false
DefinitionsCompatible = false
=> Action = SuggestRemediation
=> Reason = RenameRequestedButDefinitionIncompatible
UserSelectedRename = true
CandidateExists = true
NameCollision = false
DefinitionsCompatible = true
=> Action = PromptRenameTargetColumn
=> Reason = ColumnMissingInTargetNeedsRenameDecision

I am using PromptRenameTargetColumn as the executor boundary here because actual rename may still involve platform-specific DDL.

10. Remediation suggestions

These must also be closed. Do not let Codex invent freeform remedy types.

public enum RemediationKind
{
    AddColumnAsNullableThenBackfillThenTighten,
    AddColumnWithDefaultValue,
    KeepTargetDefinition,
    ManualDataCleanupThenRetry,
    StageIntoNewColumnThenSwap,
    TruncateTableThenRetry,
    DropAndRecreateColumn,
    ExplicitTransformMigrationRequired
}

Example mapping:

Reason = NewNonNullableColumnOnPopulatedTableWithoutDefault
=> Suggestions:
   AddColumnAsNullableThenBackfillThenTighten
   AddColumnWithDefaultValue
   TruncateTableThenRetry
   ExplicitTransformMigrationRequired
Reason = TypeChangeNeedsValidation
=> Suggestions:
   ManualDataCleanupThenRetry
   KeepTargetDefinition
   StageIntoNewColumnThenSwap
   TruncateTableThenRetry
Reason = TypeChangeExplicitTransformRequired
=> Suggestions:
   StageIntoNewColumnThenSwap
   ExplicitTransformMigrationRequired
   TruncateTableThenRetry

This is how you stay “useful” without letting execution improvise.

11. Implementation shape Codex must follow

This is the project shape I would force.

SchemaSync.Policy/
    Enums.cs
    NormalizedType.cs
    Normalizers/
        TypeNormalizer.cs
        StructuralTypeRelationClassifier.cs

    Decisions/
        TableDecisionKey.cs
        TableDecisionTable.cs
        ColumnPresenceDecisionKey.cs
        ColumnPresenceDecisionTable.cs
        MatchedColumnDecisionKey.cs
        MatchedColumnDecisionTable.cs
        RenameDecisionKey.cs
        RenameDecisionTable.cs
        TypePolicyKey.cs
        TypePolicyTable.cs

    Planner/
        SchemaPlanner.cs
        DecisionResult.cs
        RemediationCatalog.cs

    Execution/
        SchemaExecutor.cs

    Validation/
        PolicyTableValidator.cs
12. Non-negotiable implementation rules for Codex

This is the core “box it in” section.

Give Codex exactly this kind of instruction.

Implement a table-driven schema synchronization planner and executor.

Hard rules:
1. All migration policy must live in explicit decision tables or explicit type policy tables.
2. Do not encode policy in scattered if/else logic outside the normalizers and structural relation classifiers.
3. Absence in source must never imply automatic deletion.
4. Extra target tables and columns must be preserved by default.
5. Rename must never be inferred automatically.
6. Type changes must be classified only through the explicit type policy table.
7. Any type conversion not represented by the policy table must classify as Unsupported.
8. If a scenario does not match exactly one policy row, return Block with the correct reason code.
9. The executor may only perform the emitted action; it may not reinterpret or add side effects.
10. SuggestRemediation may emit only RemediationKind values from the closed enum.
11. Do not add heuristics, fuzzy matching, similarity scoring, or fallback behavior.
12. Multiple simultaneous column drifts must remain conservative and must not be auto-merged into a risky one-shot alteration.
13. Keep planning, execution, and message rendering separate.
14. Use immutable/static readonly tables as the sole policy source.
15. Add a startup validator that ensures the tables contain no duplicate exact keys.
16. Add a startup validator that ensures every supported enum combination in each stage resolves to exactly one row or intentionally resolves to Block.
17. If a case is not explicitly supported, block rather than invent behavior.
13. Concrete Codex prompt

This is the prompt I would actually give it.

Build a strict table-driven schema synchronization engine in C#.

Goal:
Compare a source schema and a target schema and produce a migration plan that makes target satisfy source requirements while preserving existing target data by default.

Core policy:
- SOURCE defines required schema.
- TARGET keeps existing data unless the user explicitly approves destruction.
- Absence in SOURCE never implies automatic deletion.
- Automatic execution may only do safe, non-destructive, unambiguous changes.
- Potentially safe but data-dependent changes require validation against TARGET data.
- Destructive or semantic-changing operations may be suggested, but not auto-executed.
- If a case is missing from policy tables, BLOCK.
- If classification is ambiguous, BLOCK.

Required architecture:
1. Normalize raw schema facts into closed scenario records.
2. Classify table presence using an explicit TableDecisionTable.
3. Classify column presence using an explicit ColumnPresenceDecisionTable.
4. For matched columns, classify definition drift using an explicit MatchedColumnDecisionTable.
5. Classify type changes using an explicit TypePolicyTable and deterministic structural relation classifiers.
6. Emit only closed ActionKind, ReasonCode, and RemediationKind enums.
7. Executor may perform only the emitted action.
8. No migration policy may be implemented outside the decision tables and type policy table.

Required enums:
- ActionKind
- ReasonCode
- PresenceKind
- RowState
- DefinitionDriftKind
- TypeFamily
- TypeChangeClass
- StructuralTypeRelation
- RemediationKind

Required types:
- NormalizedType
- TableDecisionKey / TableDecision
- ColumnPresenceDecisionKey / ColumnPresenceDecision
- MatchedColumnDecisionKey / MatchedColumnDecision
- RenameDecisionKey / RenameDecision
- TypePolicyKey / TypePolicyDecision
- DecisionResult

Required behavior:
- Extra target tables => InformExtraTablePreserved
- Extra target columns => InformExtraColumnPreserved
- Source-only column with unmatched target columns present => PromptNewOrRename
- Source-only nullable column on populated target => CreateColumn
- Source-only non-null column with default on populated target => CreateColumn
- Source-only non-null column without default on populated target => SuggestRemediation
- int -> bigint => SafeWidening
- bigint -> int => NeedsDataValidation
- varchar(50) -> varchar(200) => SafeWidening
- varchar(200) -> varchar(50) => NeedsDataValidation
- bit -> int => ExplicitTransformRequired
- nvarchar <-> varchar => ExplicitTransformRequired
- unsupported or unknown conversions => Unsupported => Block

Required constraints:
- Use immutable/static readonly tables.
- Add validation tests or startup validation that every exact key resolves deterministically.
- Do not add fuzzy rename inference.
- Do not add auto-drop.
- Do not add hidden convenience behavior.
- If anything is unclear, choose the more conservative behavior and block.

Deliverables:
- The policy enums and record types.
- The explicit decision tables.
- The planner that uses only those tables.
- The executor stub that executes only emitted actions.
- The policy validator.
- A small set of examples showing classification outputs.
14. Very important extra constraint for Codex

Add this, because otherwise it may “helpfully refactor” policy into methods.

Do not replace literal policy rows with inferred helper logic except for deterministic structural preprocessing such as integer rank comparison, string length comparison, decimal precision/scale comparison, and date/time family mapping. Those preprocessors must not make action decisions. They may only classify relation shape for the policy tables.

That sentence matters a lot.

15. My recommendation on scope

For the first version, keep the supported type system deliberately small.

Support these well:

bit
tinyint smallint int bigint
decimal(p,s)
varchar(n) nvarchar(n) varchar(max) nvarchar(max)
date datetime datetime2 datetimeoffset
uniqueidentifier

Treat everything else as Unknown or Unsupported.