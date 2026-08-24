# TalkFolio Architecture Decision Records

This document consolidates the design decisions reached for TalkFolio. Each entry records the decision, its rationale, and the consequences. Open questions that are not yet decided remain in [TalkSchema.md](TalkSchema.md#open-questions--pending-decisions).

## ADR-001: Proposal copy is stored inline as a typed array

**Status:** Decided

**Decision:** Proposal copy lives inline on the Talk record as `ProposalCopyItems`, a list of typed items, each with:

* `Type`: the copy classification (for example `Abstract`, `ElevatorPitch`, `ShortVersion`, `CommitteeNotes`, `AudienceNotes`, `AlternateTitleCandidates`)
* `Copy`: the proposal text stored as a YAML `|-` literal block

**Rationale:** A typed array keeps proposal text extensible — new copy types can be introduced without changing the Talk schema — while keeping the copy inline on the record for a straightforward model.

**Consequences:** There are no top-level `Abstract`, `ElevatorPitch`, or `ShortVersion` fields; consumers read from `ProposalCopyItems` by `Type`.

## ADR-002: Companion material is modeled as RelatedContent on the Talk

**Status:** Decided

**Decision:** The Talk schema includes a `RelatedContent` list of typed companion items, each with:

* `Type` (for example `BlogPost`, `Video`, `Article`, `Essay`, `Notebook`)
* `Title`
* `Url` (optional)
* `Notes`

**Rules:**

* RelatedContent is associated with the Talk concept, not with a specific LiquidVictor SlideDeck.
* Multiple items of the same `Type` are allowed.
* A RelatedContent item has no separate `Id`; when present, `Url` is the canonical identifier.
* `Summary`, `Status`, and `PublishedAt` are intentionally excluded.

**Rationale:** TalkFolio needs a generic way to reference companion material for discovery, navigation, and finding content tied to a talk, without becoming a content-management system. The content lifecycle remains in the domain that owns the content (for example, CognitiveInheritance for blog posts), which may live outside the TalkFolio repo.

**Consequences:** RelatedContent is a lightweight relationship model only. Publication state, summaries, and dates are never stored in TalkFolio.

## ADR-003: The Talk owns the PresentationFamily relationship

**Status:** Decided

**Decision:** The PresentationFamily relationship is owned by the Talk, and the model does not keep a separate family file or family record list:

* Each Talk carries a nested `PresentationFamily` object with `Name` and `Variant` (for example `Canonical`, `ExecutiveOverview`, `Lightning`, `Workshop`). A talk with no family omits the object.
* The family name acts as the stable identity for the grouping concept; a talk's variant is a per-talk classification within that family.
* `CanonicalTalkId` is removed. A family is not required to have a canonical talk; when one exists, "canonical" is a `Variant` value on the Talk, not a structural field on the family.

**Rationale:** Single-direction ownership matches the existing Talk → SlideDeckIds pattern and avoids a redundant family entity whose membership list would drift out of sync. The lightweight talk-level object keeps family membership cohesive without needing a second persisted file or membership table. The tradeoff is that family names must be treated as stable identifiers and renamed intentionally when the grouping changes.

**Consequences:** TalkCircuit finds a talk's family members by querying Talks that share the same `PresentationFamily.Name`. There is no denormalized family entity or membership list to maintain.

## ADR-004: Multi-line text uses `|-` literal blocks

**Status:** Decided

**Decision:** Multi-line text fields in TalkFolio YAML documents use `|-` (literal block, strip trailing newline) rather than `>-` (folded).

**Rationale:** `|-` preserves exact line breaks as authored, avoiding unexpected whitespace folding in notes and proposal copy.

**Consequences:** All examples in the schema docs (`ProposalCopyItems.Copy`, `RelatedContent.Notes`, `IdeationNotes`, `PresentationFamily.Notes`) use `|-`.

## ADR-005: Talk IDs use GUIDs

**Status:** Decided

**Decision:** All Talk-level identifiers use GUIDs for `Id` values. The human-readable title remains available as a separate field for display and discovery, and there is no requirement for a slug or URL-derived ID.

**Rationale:** The repository already treats the title as the human-facing value, and there is not always an external URL to derive a canonical identifier from. A GUID is a stable internal identity without adding a redundant slug or URL field.

**Consequences:** `Id` remains the canonical database identity. `Title` is the user-facing value; no slug field is required for the initial schema.

## ADR-006: Tags are free-form string tokens constrained to alphanumerics and dash

**Status:** Decided

**Decision:** `Tags` are free-form strings, but each tag value is constrained to alphanumerics and `-` only; whitespace is not allowed.

**Rationale:** This keeps tags flexible enough to evolve naturally while preserving a consistent, easy-to-query token format.

**Consequences:** Tag examples such as `graph-rag`, `sports-analytics`, and `software-engineering` are valid; values such as `graph rag` or `AI Systems` are not.

## ADR-007: Category uses a controlled, extensible list

**Status:** Decided

**Decision:** `Category` is a controlled list that can expand over time as the domain grows. The list is not fixed forever, but it is curated and intentionally not a deeply nested taxonomy.

**Rationale:** The initial categories are broad and stable, but TalkFolio needs room to add new categories over time without redesigning the model.

**Consequences:** Category values remain coarse and discovery-oriented; nuance stays in `Tags` rather than in nested category trees.

## ADR-008: TargetAudience uses a controlled, extensible list of strings

**Status:** Decided

**Decision:** `TargetAudience` remains a list of strings, but the list is drawn from a controlled vocabulary that can expand over time. It is not modeled as a nested object unless the repo later decides it needs richer segmentation.

**Rationale:** The current requirement is simple audience identification rather than advanced targeting metadata. Keeping this as a string list reduces schema complexity while still allowing a curated vocabulary to grow.

**Consequences:** Audience values are human-readable and consistent, while future segmentation can be introduced without rewriting the core model.

## ADR-009: Extra talk metadata flags use a flexible key-value map

**Status:** Decided

**Decision:** Additional talk-specific flags are stored in a flexible `Flags` map, keyed by name (for example `Locked`, `ForKids`, `HandsOn`) and valued as booleans or other lightweight scalars as needed.

**Rationale:** This keeps the model open-ended without forcing all possible flags into schema fields up front. It preserves the ability to add flags as the domain grows while keeping the flags clearly associated with the Talk rather than with a deck.

**Consequences:** The `Flags` field is the extension point for future talk-level metadata, while `Summary`, `Status`, and other lifecycle metadata remain outside this model unless a later decision adds them explicitly.

## ADR-010: Only narrative context fields remain unstructured

**Status:** Decided

**Decision:** TalkFolio keeps the model structured wherever reasonable. The only intentionally unstructured Talk fields are the narrative/context fields used for authored prose and commentary:

* `ProposalCopyItems[].Copy`
* `IdeationNotes`
* `PresentationFamily.Notes`
* `RelatedContent[].Notes`

**Rationale:** Structured fields improve consistency, filtering, and downstream tooling. The remaining prose fields exist specifically to preserve authored language, explanatory context, and editorial notes that do not fit cleanly into a rigid structure.

**Consequences:** New talk data should default to structured fields unless it is clearly narrative or explanatory prose.

## ADR-011: Storage access occurs through repository abstractions

**Status:** Decided

**Decision:** All product interactions with TalkFolio data occur through repository abstractions rather than directly through file paths, directory walks, or storage-specific code.

**Rules:**

* The initial persistence adapter may be file-based and YAML-backed.
* Application and domain logic depend on repository contracts, not on file-system details.
* The canonical identity of a Talk or PresentationFamily remains its `Id`, not its file name or path.
* Swapping the file-based adapter for a database-backed adapter should not require redesigning the domain model.

**Rationale:** TalkFolio wants a file-based MVP, but it should not couple the rest of the product to that storage choice. A repository boundary keeps the model portable and makes future storage changes, such as moving to a database, far easier.

**Consequences:** File naming, directory layout, and root-path resolution are infrastructure concerns. Validation and catalog logic should operate on loaded records, not on direct file-system assumptions.

## ADR-012: File-backed data roots are configurable and generally external to this repo

**Status:** Decided

**Decision:** When TalkFolio uses a file-backed store, the data root is configurable and will generally live outside this implementation repo so the data can be versioned and maintained separately from the product code.

**Rules:**

* Production-like or maintained talk catalogs should not be assumed to live inside this repo.
* The implementation must accept a configurable data root rather than hard-coding a repository-local path.
* This repo may include dedicated test repositories or fixture datasets for automated tests, local development, and validation scenarios.
* The test datasets exist to support product validation, not to define the long-term location of maintained catalog data.

**Rationale:** The product code and the talk catalog have different lifecycles. Keeping the maintained catalog generally external preserves independent versioning and reduces coupling between implementation work and content maintenance.

## ADR-013: Configuration uses standard .NET precedence with in-memory overrides at the top

**Status:** Decided

**Decision:** TalkFolio configuration follows the standard .NET configuration precedence: JSON files load first, environment variables override file-based values, and in-memory configuration provided by tests or host bootstrap code is applied last so it can override all other sources.

**Rules:**

* JSON configuration provides the baseline defaults for the product.
* Environment variables are the operational override layer for deployment-specific settings.
* In-memory configuration is reserved for test-time or bootstrap-time overrides where the caller intentionally wants a value to win.
* If command-line configuration is introduced later, it should sit above environment variables and file values.

**Rationale:** This is the conventional .NET configuration model and keeps the app behavior predictable across local development, deployed environments, and automated tests. It also preserves a clean separation between checked-in defaults, environment-specific deployment values, and test-specific overrides.

**Consequences:** The data root, repository selection, and related runtime settings should all be supplied through the configuration system rather than as hard-coded constants. Bootstrap work must define how the configurable root is supplied and how test repositories are organized, and the product cannot assume that a checked-in repo-local catalog is the default operating mode.

## ADR-014: Boundary activity logs are informational; payload detail is trace-only

**Status:** Decided

**Decision:** TalkFolio logs boundary and activity events at informational levels, while message payload detail is reserved for trace-level logs. This is a required completion rule for all TalkFolio work, not an optional preference.

**Rules:**

* Method entry, method exit, and cross-boundary activity should be logged at `Information` or higher when they represent product-relevant work.
* Payload content, record field values, and other verbose data snapshots should be logged at `Trace` so they do not appear in normal operation.
* Diagnostic warnings and failures may use `Warning` or `Error` as appropriate, but they should not duplicate verbose payload bodies at higher levels.
* The same convention should apply across TalkFolio components so logs remain readable and predictable.
* Work is not considered complete unless any new or changed logging follows this convention.

**Rationale:** Operational logs should describe what the system is doing without flooding normal output with full payload data. Trace-level payload logging preserves debugging detail when needed while keeping default log volume manageable.

**Consequences:** Implementations must separate activity logs from payload-detail logs. Reviewers should expect informational logs at subsystem boundaries and trace logs for payload snapshots or object-value dumps. Any completed change should be checked against this rule before it is considered done.

## ADR-015: Catalog validation is fail-fast with typed domain load errors

**Status:** Decided

**Decision:** TalkFolio catalog loading now fails fast for invalid talk data, and does not run a warning-and-skip mode.

**Rules:**

* Malformed YAML throws `MalformedTalkYamlException`.
* Duplicate talk IDs throw `DuplicateTalkIdException`.
* Duplicate `(Title, PresentationFamily.Variant)` pairs throw `DuplicateTalkTitleVariantException`.
* `PresentationFamily.Name` does not participate in the duplicate-talk uniqueness key.
* These exceptions are logged as load failures and then rethrown so upstream callers can handle each failure type distinctly.
* Catalog loads only succeed when all talk files satisfy the repository invariants.

**Rationale:** TalkFolio and LiquidVictor should follow the same fail-fast behavior for invalid catalog content. Silent skips can hide data problems and create partial, misleading read models.

**Consequences:** Catalog maintainers must fix invalid files before load can succeed. Upstream callers can choose specific handling by exception type without changing core repository behavior.
