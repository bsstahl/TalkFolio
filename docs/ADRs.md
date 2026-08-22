# TalkFolio Architecture Decision Records

This document consolidates the design decisions reached for TalkFolio. Each entry records the decision, its rationale, and the consequences. Open questions that are not yet decided remain in [TalkSchema.md](TalkSchema.md#open-questions--pending-decisions).

## ADR-001: Proposal copy is stored inline as a typed array

**Status:** Decided

**Decision:** Proposal copy lives inline on the Talk record as `ProposalCopyItems`, a list of typed items, each with:

- `Type`: the copy classification (for example `Abstract`, `ElevatorPitch`, `ShortVersion`, `CommitteeNotes`, `AudienceNotes`, `AlternateTitleCandidates`)
- `Copy`: the proposal text stored as a YAML `|-` literal block

**Rationale:** A typed array keeps proposal text extensible — new copy types can be introduced without changing the Talk schema — while keeping the copy inline on the record for a straightforward model.

**Consequences:** There are no top-level `Abstract`, `ElevatorPitch`, or `ShortVersion` fields; consumers read from `ProposalCopyItems` by `Type`.

## ADR-002: Companion material is modeled as RelatedContent on the Talk

**Status:** Decided

**Decision:** The Talk schema includes a `RelatedContent` list of typed companion items, each with:

- `Type` (for example `BlogPost`, `Video`, `Article`, `Essay`, `Notebook`)
- `Title`
- `Url` (optional)
- `Notes`

**Rules:**

- RelatedContent is associated with the Talk concept, not with a specific LiquidVictor SlideDeck.
- Multiple items of the same `Type` are allowed.
- A RelatedContent item has no separate `Id`; when present, `Url` is the canonical identifier.
- `Summary`, `Status`, and `PublishedAt` are intentionally excluded.

**Rationale:** TalkFolio needs a generic way to reference companion material for discovery, navigation, and finding content tied to a talk, without becoming a content-management system. The content lifecycle remains in the domain that owns the content (for example, CognitiveInheritance for blog posts), which may live outside the TalkFolio repo.

**Consequences:** RelatedContent is a lightweight relationship model only. Publication state, summaries, and dates are never stored in TalkFolio.

## ADR-003: The Talk owns the PresentationFamily relationship

**Status:** Decided

**Decision:** PresentationFamily remains a separate entity, but the relationship is owned by the Talk:

- Each Talk carries a nested `PresentationFamily` object with `Id` and `Variant` (for example `Canonical`, `ExecutiveOverview`, `Lightning`, `Workshop`). A talk with no family omits the object.
- The PresentationFamily entity is just `Id`, `Name`, and `Notes`; it does not list members. Membership is discovered by querying Talks by `PresentationFamily.Id`.
- `CanonicalTalkId` is removed. A family is not required to have a canonical talk; when one exists, "canonical" is a `Variant` value on the Talk, not a structural field on the family.

**Rationale:** Single-direction ownership matches the existing Talk → SlideDeckIds reference pattern and avoids duplicated references (Talk → family and family → talks) drifting out of sync. Variant identity belongs to the talk, and canonical status is a classification, not structure. Encapsulating `Id` and `Variant` in one object keeps family membership cohesive rather than flat fields on the Talk root.

**Consequences:** TalkCircuit finds a talk's family members by querying Talks that share its `PresentationFamily.Id`. There is no denormalized member list to maintain.

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

- `ProposalCopyItems[].Copy`
- `IdeationNotes`
- `PresentationFamily.Notes`
- `RelatedContent[].Notes`

**Rationale:** Structured fields improve consistency, filtering, and downstream tooling. The remaining prose fields exist specifically to preserve authored language, explanatory context, and editorial notes that do not fit cleanly into a rigid structure.

**Consequences:** New talk data should default to structured fields unless it is clearly narrative or explanatory prose.
