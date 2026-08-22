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
