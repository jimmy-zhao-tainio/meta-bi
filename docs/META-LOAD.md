# META-LOAD

This document is an inventory of concerns that can surface when loading data in ETL systems.

It is intentionally explicit rather than compressed.
Categories overlap on purpose.
The same design choice can surface as a source concern, a target concern, a temporal concern, a data quality concern, and an operational concern at the same time.

## 1. Source acquisition concerns

These concern how source data becomes available to the loader in the first place.

- One-time backfill source vs recurring operational source
- Full extract vs incremental extract
- Snapshot extract vs live mutable source read
- Pull ingestion vs push delivery
- Batch vs micro-batch vs streaming intake
- Single source vs multi-source acquisition
- Homogeneous vs heterogeneous source technologies
- Table/query source vs file source vs API source vs message source vs object-store source
- Source authentication mechanism
- Source authorization scope
- Source endpoint discovery/configuration
- Stable endpoint names vs rotating endpoint names
- Stable schema vs schema drift
- Stable object names vs renamed tables/files/topics
- Trusted source metadata vs weak/incomplete metadata
- Availability window restrictions
- Maintenance blackout windows
- Source throttling / rate limits
- Source pagination semantics
- Source cursor/token expiration
- Chunk sizing / page sizing
- Partition discovery
- Partition arrival completeness detection
- Parallel source reads
- Ordered source reads requirement
- Cross-database / cross-server reads
- Cross-region / cross-network reads
- Source transaction consistency / snapshot isolation requirement
- Dirty-read tolerance or intolerance
- Read lock / contention sensitivity
- Extract query pushdown requirements
- Column projection pushdown requirements
- Predicate pushdown requirements
- Source-side join pushdown requirement
- Source-side sort/order requirement
- Source extract window overlap policy
- Late source arrival
- Missing source partitions/files
- Empty source partition semantics
- Duplicate source delivery
- Out-of-order source delivery
- Restatement of previously delivered source data
- Source delete visibility vs delete invisibility
- Soft-delete source vs hard-delete source
- Source retention window limits
- Source archival tier access
- Re-reading historical source snapshots
- File finalization detection
- Control-file / manifest-file dependency
- Compressed source format handling
- Encrypted source payload handling
- File naming convention stability
- Source path renames
- Source object overwrite-in-place vs append-new-object behavior
- Multi-file consistency for one logical extract
- Source extract identity and batch identity
- Multi-tenant source isolation
- Source system identity mapping across environments
- Clock skew between source and loader when selecting increments

---

## 2. Change detection concerns

These concern how the loader determines what changed and what that change means.

- Full compare vs delta compare
- Explicit CDC feed vs inferred change detection
- Source transaction-log CDC vs trigger-based CDC vs query-based CDC
- Watermark-based change detection
- Sequence-based change detection
- Log-sequence-number-based change detection
- Version-number-based change detection
- Modified-timestamp-based change detection
- Created-and-modified timestamp pair semantics
- Status-flag-based change detection
- Hash-based row change detection
- Column-subset hash vs full-row hash
- Business-key-based matching
- Surrogate-key-based matching
- Composite-key matching
- Stable row identity vs unstable row identity
- Natural key mutation
- Key reuse by source
- Null semantics in change detection
- Blank-vs-null change semantics
- Case/collation sensitivity in change detection
- Whitespace normalization before comparison
- Floating-point tolerance vs exactness
- Decimal-scale normalization before comparison
- Datetime precision mismatch
- Time-zone normalization before comparison
- Binary/blob comparison policy
- Detecting inserts
- Detecting updates
- Detecting deletes
- Detecting resurrected previously deleted rows
- Detecting no-op updates
- Detecting partial-row updates
- Detecting out-of-order source changes
- Detecting duplicate source changes
- Compressing multiple source changes within one batch
- Choosing first change vs last change within window
- Transactional grouping of multiple row changes
- Before-image availability vs after-image-only feeds
- Delete tombstone availability
- CDC gap detection
- CDC restart token reuse
- Watermark inclusive vs exclusive boundary semantics
- Watermark precision loss
- Watermark based on commit time vs source event time
- Late commits crossing watermark boundaries
- Backdated corrections to old business periods
- Future-dated source rows
- Schema change detection separate from data change detection
- DDL in CDC stream
- Key split / merge events
- Change reason availability
- Unknown or ambiguous change reason
- Conservative reread overlap because source markers are unreliable
- Cross-table consistency when detecting changes across related entities
- High-water mark per source object vs global high-water mark
- Multi-tenant watermark scoping
- Reconciliation when source emits repeated snapshots instead of explicit deltas

---

## 3. Temporal semantics concerns

- Load time vs source arrival time vs event time vs business-effective time
- Processing date vs accounting date vs business date
- Valid-from / valid-to semantics
- System-effective period vs business-effective period
- Current-row marker semantics
- Open-ended interval semantics
- Inclusive vs exclusive end dates
- Day-grain vs timestamp-grain validity
- Local-time vs UTC storage semantics
- Time zone of event capture vs time zone of business meaning
- Calendar-day semantics vs exact-instant semantics
- Bitemporal handling
- SCD Type 0 handling
- SCD Type 1 handling
- SCD Type 2 handling
- SCD Type 3 handling
- SCD Type 4 handling
- SCD Type 6 or hybrid handling
- Slowly changing facts, not just dimensions
- Slowly changing relationships, not just attributes
- Slowly changing hierarchies
- Early-arriving facts
- Late-arriving dimensions
- Late-arriving facts
- Future-arriving dimension rows already effective in business time
- Restating historical facts
- Rebuilding history after late corrections
- Periodic snapshot loads
- Accumulating snapshot loads
- Transactional fact loads
- Sliding-window reloads
- Rolling recomputation windows
- Point-in-time correctness
- As-of joins
- Temporal alignment across multiple sources
- Different source clocks
- Watermark time vs business-effective time mismatch
- Historical survivorship rules
- Reopening previously closed history rows
- Overlap prevention between historical rows
- Gap prevention between historical rows
- Multiple changes with identical timestamps
- Tie-breaker rules for equal timestamps
- Closing current row when delete arrives
- Reinstating a row after prior logical delete
- Historical correction cutoff policy
- Closed-period adjustment policy
- Temporal bridge handling
- Historical parent-child relationship validity
- Period-boundary alignment to fiscal calendar
- Month-end / week-end / day-end cutoff semantics
- Event date vs posting date choice for fact dating
- Time-travel rebuild capability for prior as-of points

---

## 4. Target write semantics concerns

These concern what the loader is allowed and expected to do to the target.

- Insert-only loading
- Append-only loading
- Full-replace loading
- Truncate-and-reload loading
- Merge/upsert loading
- Update-only loading
- Delete-and-insert replacement
- Hard-delete in target
- Soft-delete in target
- Logical inactivation of rows absent from source
- Keep missing target rows indefinitely
- Close current history row then insert successor row
- Partition overwrite
- Partition append
- Partition exchange / swap / atomic replacement
- Table-level materialization vs view-level materialization
- Staging-table write then publish
- Direct write to final target
- Temporary-table-based materialization
- One transform populating several targets
- Several transforms feeding one target
- Fan-out from one source batch to several targets
- Pre-load cleanup requirements
- Post-load maintenance requirements
- Index disable/rebuild behavior
- Constraint disable/re-enable behavior
- Trigger firing vs trigger suppression
- Identity column handling
- Sequence handling
- Platform-generated surrogate key vs pipeline-generated surrogate key
- Stable technical key assignment across reruns
- Audit/platform column stamping
- Load batch ID stamping
- Load run timestamp stamping
- Derived technical columns
- Default values supplied by platform vs supplied by transform
- Merge match key definition
- Update changed columns only vs overwrite full row
- No-op update suppression
- Conflict resolution when the same target row is hit twice in one batch
- Target deduping policy
- Target uniqueness enforcement
- Write order requirements due to foreign keys or dependency
- Fact foreign-key stamping
- Deferred foreign-key stamping
- Post-load foreign-key repair
- Unknown-key stamping conventions
- Inferred-key stamping conventions
- Partition-management responsibility
- Target schema evolution implications
- Isolation level for target writes
- Transaction scope per row vs per chunk vs per table vs per pipeline
- Atomic publish vs partially visible intermediate writes
- Visibility of staging artifacts
- Retry behavior for partially written chunks
- Locking behavior on target
- Concurrency with readers during load
- Concurrency with other writers during load
- Retry-safe delete semantics
- Delete detection mapped to close-out vs physical delete
- Materialized aggregate refresh after base-table load
- Side-effect order when one load updates multiple targets

---

## 5. Data quality and contract concerns

These often sit half in validation and half in runtime behavior.

- Required column presence
- Optional column presence with defaulting
- Unexpected extra columns
- Missing expected columns
- Source schema version compatibility
- Data-type compatibility
- Length fit
- Precision fit
- Scale fit
- Nullability conformance
- Domain/value-set conformance
- Format conformance
- Reference/integrity conformance
- Unique-key conformance
- Duplicate business-key detection
- Unexpected source duplicates
- Unexpected target duplicates
- Mandatory business-rule validation before load
- Mandatory business-rule validation after load
- Reject invalid rows vs fail entire batch
- Quarantine invalid rows
- Redirect invalid rows to error store
- Threshold-based tolerance
- Drift detection
- Semantic contract mismatch despite structural match
- Unknown code mapping
- Reference data not yet loaded
- Parent-before-child readiness
- Child-before-parent tolerance
- Lookup-miss detection
- Lookup-ambiguity detection
- Duplicate reference-row detection
- Missing conformed dimension member
- Invalid business-key format for lookup
- Reference-data readiness validation
- Foreign-key resolution completeness threshold
- String normalization rules
- Unicode normalization rules
- Trim semantics
- Blank-string vs null semantics
- Locale/culture-sensitive parsing
- Numeric rounding policy
- Numeric overflow policy
- Decimal truncation policy
- Invalid dates
- Sentinel dates
- Impossible timestamps
- Broken UTF or file-encoding issues
- File-header validation
- File-trailer validation
- Record-count expectations
- Control totals / checksums
- Cross-source reconciliation
- Row-level validation vs batch-level validation
- Warning-only vs error-level validation
- Duplicate file or duplicate batch detection
- Unexpected sparse-column population
- Required relationship between columns
- Value dependency rules across columns
- Canonicalization before validation vs after validation
- Data-quality score threshold before publish
- Error-reporting detail required for rejected rows

---

## 6. Idempotency, replay, and recovery concerns

A serious load platform has to be precise here.

- Safe rerun of the same batch
- Replay from arbitrary point
- Replay from watermark
- Replay from named run
- Replay from source-artifact identity
- Backfill of historical range
- Partial-batch failure recovery
- Step restartability
- Whole-pipeline restartability
- Exactly-once vs at-least-once vs at-most-once semantics
- Duplicate prevention on replay
- Watermark advancement timing
- Watermark rollback policy
- Commit per step vs commit per pipeline
- Recover after crash between target write and watermark update
- Recover after crash between log write and target write
- Recover after crash between file acknowledgment and target write
- Poison-batch handling
- Manual override / operator intervention points
- Reconciliation after manual data fixes
- Recompute target from scratch
- Rebuild limited time window
- Rebuild one business entity only
- Determinism of repeated execution
- Stable input ordering across reruns
- Batch identity vs run identity vs correlation identity
- Dependency-aware restart after upstream correction
- Reprocessing facts after delayed dimension arrival
- Replay behavior for repaired foreign keys
- Deterministic inferred-member creation on rerun
- Stable surrogate-key reuse on replay
- Idempotent late-key repair
- Checkpoint granularity
- Checkpoint persistence and durability
- Rehydrating prior source extract vs re-querying a mutable source
- Replaying with original code version vs current code version
- Replaying with original reference data vs current reference data
- Replay cutoff protection so operational runs do not overlap with backfills
- Duplicate-message suppression window
- Reconciliation between staged data and final published data after recovery
- Compensation for partial side effects outside the target database
- Operator-visible reason for replay decisions

---

## 7. Orchestration and dependency concerns

This is the pipeline layer proper.

- Dependency ordering between loads
- Source-to-target dependency graph
- Explicit dependencies vs inferred dependencies
- Data dependency vs platform dependency
- Pre-step / main-step / post-step structure
- Conditional branching
- Optional steps
- Parallelizable steps
- Serialization requirements
- Resource class / workload class
- Priority / queueing
- Concurrency limits
- Mutual exclusion / single-writer protection
- Cross-pipeline locking
- Load windows / schedule windows
- Business-calendar-based execution windows
- Retry policy
- Timeout policy
- Cancellation policy
- Circuit-breaker / stop-on-error policy
- Continue-on-error allowances
- Multi-environment execution
- Promotion of the same pipeline across environments
- External scheduler integration
- Manual vs scheduled execution
- Trigger by source arrival
- Trigger by clock
- Trigger by dependency completion
- Event-driven orchestration
- Human approval gates for risky loads
- Batch grouping
- Partition-wise orchestration
- Tenant-wise orchestration
- Fan-out orchestration
- Fan-in orchestration
- Coordinating upstream extract completion with downstream load start
- Coordinating multi-table consistency points
- Per-dataset vs global backpressure handling
- Pause/resume of long-running pipelines
- Orchestration-state durability
- Dependency versioning when pipeline definitions change
- Late dependency arrival policy
- Skipped-step semantics
- Partial-success semantics across a multi-target run

---

## 8. Logging, observability, and audit concerns

This deserves its own major category.

- Run-start logging
- Run-end logging
- Step-start logging
- Step-end logging
- Status-transition logging
- Row counts read
- Row counts inserted
- Row counts updated
- Row counts deleted
- Row counts unchanged
- Row counts rejected
- Row counts quarantined
- Watermark used
- Watermark advanced to
- Window boundaries used
- Source-artifact identities consumed
- Audit/platform columns stamped
- Source query fingerprint
- Transform fingerprint
- Binding/configuration fingerprint
- Target contract or schema version
- Execution duration
- Throughput metrics
- Queue wait time
- Retry counts
- Error classification
- Captured failing SQL or command text
- Captured parameter values
- Captured warnings
- Data-quality exceptions
- Referential-resolution exceptions
- Load lineage / provenance
- Who triggered the run
- Which environment executed it
- Which code/model version executed it
- Correlation IDs across systems
- Operational dashboard metrics
- SLA-breach detection
- Anomaly-detection hooks
- Business reconciliation logging
- Regulatory/compliance audit trail
- Row-level audit vs batch-level audit
- Sample rejected-row capture vs full rejected-row capture
- Retention / purge of logs
- Tamper evidence / immutability expectations
- PII redaction in logs
- Explainability of automated load decisions
- Evidence that a replay used original vs current configuration

---

## 9. Performance and scale concerns

These are often what force pattern variation.

- Very large tables
- Very wide rows
- Small frequent batches
- Large infrequent batches
- Sparse changes vs dense changes
- Hot partitions
- Skewed keys
- Expensive source joins
- Expensive target merges
- Partition-elimination needs
- Bulk load vs row-wise load
- Batch sizing / chunk sizing
- Parallel degree
- Memory pressure
- Temporary-storage pressure
- Sort pressure
- Spill-to-disk behavior
- Network-bandwidth limits
- Cross-region latency
- Source-system impact minimization
- Target lock escalation
- Logging-volume minimization
- Incremental recomputation vs full recomputation
- Sliding-window size tradeoff
- Hash-computation cost
- Lookup-cache size and eviction behavior
- Index-maintenance cost
- Statistics-maintenance timing
- Staging-table reuse
- Reuse of prior extracts
- Compression/decompression overhead
- File split / coalesce strategy
- Checkpoint frequency
- Sort-merge vs hash-merge tradeoffs
- Partition-count tradeoffs
- Too many tiny files / micro-partitions
- Write amplification due to upsert strategy
- Read amplification due to replay overlap
- Large-cardinality dimension-lookup performance
- Historical lookup performance under SCD Type 2
- Fan-out target-write pressure
- Cross-database network-hop cost
- Shared warehouse workload contention
- Source pagination cost at deep offsets
- Serialization bottleneck for ordered processing
- Resource-cleanup pressure after failed large runs

---

## 10. Platform and environment concerns

These are about making the system executable and operable on real platforms.

- Development vs test vs production differences
- Logical system name vs physical connection mapping
- Secrets management
- Credential rotation
- Feature flags / behavior toggles
- Environment-specific source availability
- Environment-specific target availability
- Environment-specific schema differences
- Environment-specific data-volume differences
- Dry-run / plan-only mode
- Validation-only mode
- Deployment gating before execution
- Runtime-version compatibility
- Connector or driver version compatibility
- Plugin or extension compatibility
- Resource governance
- Host-process isolation
- Sandboxed execution vs unrestricted execution
- Network-reachability constraints
- File-system availability
- Object-storage integration
- External tool/runtime dependencies
- Transaction-capability differences by platform
- Merge-support differences by platform
- Delete-support differences by platform
- Temporary-table semantics differences by platform
- Cross-database query-capability differences
- SQL-dialect differences
- Case-sensitivity differences across platforms
- Collation differences across environments
- Clock/time-zone differences between hosts
- Locale differences between environments
- Filesystem rename-atomicity differences
- Object-store listing-consistency differences
- Maximum statement-size / parameter-count differences
- Bulk-loader capability differences
- Scheduler-capability differences
- Limits on concurrent sessions or transactions

---

## 11. Security and governance concerns

These matter quickly once the platform is real.

- Source-credential scoping
- Target-credential scoping
- Least privilege for reads
- Least privilege for writes
- Separation of deploy rights vs run rights
- Separation of operator rights vs developer rights
- PII handling
- Sensitive-column masking
- Encrypted staging storage
- Encryption in transit
- Encryption at rest
- Secret exposure prevention in logs
- Secret exposure prevention in error messages
- Row-level security implications
- Column-level security implications
- Data-residency constraints
- Cross-border transfer restrictions
- Approval requirements for destructive operations
- Approval requirements for backfills or replays
- Auditability of operator actions
- Change-management traceability
- Retention-policy enforcement
- Purge-policy enforcement
- Governance over custom load extensions
- Signed or approved pattern packages
- Allowed vs forbidden runtime actions per environment
- Break-glass access procedures
- Segregation of duties for schema changes vs data changes
- Governance over reference-data overrides
- Privacy-preserving observability
- Legal-hold interactions with deletion/reload behavior
- Policy compliance for intermediate files and error stores

---

## 12. Business modeling concerns that surface in loading

This is where generic ETL often stops being enough.

- Business-key instability
- Reused business keys
- Source-system identity collisions
- Same business entity represented in several sources
- Survivorship rules across sources
- Source precedence for mastered attributes
- Master data vs transactional data treatment
- Grain enforcement for fact tables
- One-row-per-entity vs one-row-per-event ambiguity
- Degenerate-dimension handling
- Junk-dimension handling
- Mini-dimension handling
- Many-to-many relationship handling
- Bridge-table materialization
- Parent-child hierarchy handling
- Ragged-hierarchy handling
- Slowly changing relationships
- Reclassification of historical facts
- Corrections to closed periods
- Restatement policy for published metrics
- Fiscal-calendar alignment
- Business-holiday-calendar alignment
- Time zone of business event vs platform processing
- Multi-currency handling
- Exchange-rate timing
- Unit-of-measure conversion timing
- Reference-data versioning
- Conformed dimensions across domains
- Same code set with different meaning by source
- Entity split over time
- Entity merge over time
- Semantic delete vs operational delete
- Legal retention vs business retention
- Snapshot fact vs transactional fact vs accumulating fact treatment
- Additive vs semi-additive vs non-additive measure handling
- Balance-forward vs point-in-time measure semantics
- Multiple active business states that must coexist
- Business-defined unknown / not-applicable / error member semantics
- Historical hierarchy reparenting effects on downstream facts
- Multi-source conformance before load vs after load

---

## 13. Reference resolution / keying concerns

This is a first-class concern family, not a minor detail under dimensions.

- Dimension key lookup
- Surrogate-key lookup from business key
- Composite business-key lookup
- Source-system-qualified business-key lookup
- Cross-source conformed-dimension lookup
- Effective-dated dimension lookup
- Point-in-time dimension lookup
- Current-row dimension lookup
- Fact-to-dimension key resolution
- Fact-to-multiple-dimension key resolution
- Role-playing dimension lookup
- Parent-child self-dimension lookup
- Recursive hierarchy key resolution
- Historical hierarchy lookup as of event date
- Lookup against SCD Type 1 dimension
- Lookup against SCD Type 2 dimension
- Fact event date vs dimension effective date alignment
- Tie-break rules when multiple historical rows match
- Surrogate-key assignment policy
- Platform-generated vs pipeline-generated surrogate keys
- Stable surrogate-key reuse across reruns
- Placeholder foreign keys
- Late-binding of foreign keys
- Deferred foreign-key stamping
- Post-load foreign-key repair
- Unknown-member fallback
- Not-applicable-member fallback
- Error-member fallback
- Inferred-member creation
- Deterministic inferred-member creation
- Early-arriving fact with missing dimension row
- Late-arriving dimension repair
- Re-keying facts after inferred-member replacement
- Re-keying facts after dimension correction
- Lookup-miss policy
- Lookup-ambiguity policy
- Duplicate business key in dimension
- Multiple active dimension rows for the same business key
- Lookup precedence across candidate dimensions or sources
- Survivorship before lookup
- Cached lookup vs live lookup
- Bulk lookup vs row-wise lookup
- Lookup performance under large cardinality
- Null business-key lookup semantics
- Blank business-key lookup semantics
- Trim/case/collation normalization before lookup
- Type conversion during lookup
- Required vs optional foreign-key resolution
- Missing reference data gating fact load
- Allow fact load before dimension load vs block load
- Retry policy for unresolved lookups
- Quarantine policy for unresolved lookups
- Auto-create reference row vs reject row
- Reconciliation of unresolved foreign keys
- Referential repair batch after delayed reference arrival
- Audit trail for original business key vs resolved surrogate key
- Multiple candidate match rules with deterministic precedence
- Match-rule versioning when lookup logic changes
- Foreign-key resolution completeness threshold before publish
- Cross-environment stability of key assignment

---

## 14. Custom behavior and extensibility concerns

These matter once a load platform has plug-ins, scripts, or custom runtime hooks.

- Custom pre-read hooks
- Custom post-read hooks
- Custom pre-write hooks
- Custom post-write hooks
- Custom validation extensions
- Custom business-rule injection
- Custom key-generation extensions
- Custom conflict-resolution extensions
- Custom error-routing extensions
- Deterministic behavior of custom code across reruns
- Sandboxing of custom code
- Allowed side effects of custom code
- Prohibited side effects of custom code
- Versioning of custom extensions
- Compatibility of extensions with platform/runtime upgrades
- Observability of extension behavior
- Failure isolation for extension crashes
- Approval/governance workflow for custom logic
- Promotion of extensions across environments
- Dependency packaging for extensions
- Reproducibility of extension execution environment
- Security review for custom code paths
- Ownership boundaries between platform behavior and custom behavior
- Fallback behavior when an extension is missing or disabled
