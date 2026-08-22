# TalkFolio Schema Direction

This document captures the current schema direction for TalkFolio, based on the SpeakerOps decisions reached so far. It is intentionally a working proposal rather than a final implementation contract. Decisions that have been finalized are recorded in [ADRs.md](ADRs.md); only open questions remain in the "Open Questions / Pending Decisions" section below.

## Scope

TalkFolio owns talk concepts and proposal metadata, not deck construction or conference-submission execution.

The domain concerns are:

- talk identity and canonical description
- talk proposal copy
- audience fit
- category/tags for discovery and CFP relevance
- grouping into a PresentationFamily
- concept-level lifecycle
- references to built decks or public presentations

TalkFolio does not own:

- deck build status or slide structure
- publication mechanics in SlideFed
- CFP submission state, booking, or acceptance results
- execution of the no-duplicate-family rule

## Core Entity: Talk

A Talk is a speakable concept independent of any specific deck, conference, or delivery event.

### Proposed YAML shape

```yaml
Id: 6c8d4d27-9cc7-4c41-9bf8-19e55758e7cc
Slug: rag-deep-dive
Title: RAG Deep Dive
AlternateTitles:
  - Harnessing the Power of Retrieval-Augmented Generation
  - Demystifying RAG for Business Applications
Category: Language Models
Tags:
  - rag
  - retrieval
  - embeddings
  - knowledge-graph
PresentationFamily:
  Id: 8ccdf8b8-fd2c-4d41-9fe0-32fade0f41dc
  Variant: Canonical
LifecycleStatus: Active
ProposalCopyItems:
  - Type: Abstract
    Copy: |-
      Retrieval-augmented generation is the most important architecture pattern for
      real-world AI applications that need grounded, explainable answers.
  - Type: ElevatorPitch
    Copy: |-
      In this talk, Barry explains how RAG works in practice and why so many teams
      struggle with retrieval quality, chunking, and evaluation.
  - Type: ShortVersion
    Copy: |-
      A practical tour of the architecture, the failure modes, and the design tradeoffs.
TargetAudience:
  - Software engineers building AI-enabled applications
  - Architects designing enterprise knowledge systems
  - Technical leaders evaluating AI adoption patterns
SlideDeckIds:
  - 70b739b6-8dcb-43da-adc8-7392abf9a6ef
PublicPresentationReferences:
  - Source: SlideFed
    Url: https://example.com/presentation/rag-deep-dive
    PublicId: rag-deep-dive
RelatedContent:
  - Type: BlogPost
    Title: RAG in Practice: Grounding Answers with Retrieval
    Url: https://example.com/blog/rag-in-practice
    Notes: |-
      Companion article expanding on the retrieval patterns covered in the talk.
  - Type: BlogPost
    Title: Evaluating Retrieval Quality Without a Gold Dataset
    Url: https://example.com/blog/evaluating-retrieval-quality
    Notes: |-
      Follow-up post on the evaluation section of the talk.
  - Type: Video
    Title: RAG Deep Dive — Workshop Recording
    Url: https://example.com/videos/rag-deep-dive-workshop
    Notes: |-
      Recorded workshop covering the same material in a longer format.
IdeationNotes: |-
  This talk could also be reframed as a practical architecture talk or a more
  abstract systems talk.
CreatedAt: 2026-08-20T00:00:00Z
UpdatedAt: 2026-08-21T00:00:00Z
```

## Canonical fields

The current best-fit set of core fields is:

- Id: GUID
- Slug: stable URL-safe identifier
- Title: canonical title
- AlternateTitles: list of marketing or branding variants
- Category: coarse top-level selection bucket
- Tags: topic labels for overlap and CFP matching
- PresentationFamily: family membership object with `Id` (grouping key for near-duplicate variants) and `Variant` (this talk's role within the family, for example `Canonical`, `ExecutiveOverview`, `Lightning`, `Workshop`)
- LifecycleStatus: concept-level state
- ProposalCopyItems: typed array of inline proposal copy blocks, each with `Type` and `Copy` (`|-` literal block)
- TargetAudience: list of audience descriptors
- SlideDeckIds: list of LiquidVictor `SlideDeck.Id` values
- PublicPresentationReferences: optional links to SlideFed or publication resources
- RelatedContent: typed list of companion content references (`Type`, `Title`, optional `Url`, `Notes`)
- IdeationNotes: freeform notes for ideas not yet fully refined
- CreatedAt / UpdatedAt: operational metadata

## Category

Category is intentionally coarse and should not be a deep taxonomy.

Current candidate values:

- Agile
- Algorithms
- Language Models
- Leadership & Community
- Software Engineering

### Design intent

- user should be able to filter by a broad topic at a glance
- most CFPs only need broad topical grouping
- nuance belongs in Tags, not nested taxonomy branches

## Tags

Tags are the main mechanism for overlap, cross-cutting classification, and CFP matching. They are expected to be the richest search surface in the domain.

### Proposed rules

- Tags are user-defined or repo-managed strings, not a deeply nested classification tree.
- A Talk may have many Tags.
- Tags capture overlap that a single Category cannot express.
- Conference submission systems may map TalkFolio Tags to a conference's fixed vocabulary.

### Examples

- rag
- embeddings
- retrieval
- graph-rag
- optimization
- genetic-algorithms
- sports-analytics
- architecture
- software-engineering

## PresentationFamily

A PresentationFamily groups talks that are materially the same core presentation but differ in branding, title, emphasis, or audience framing.

### Relationship ownership

The Talk owns the relationship through a nested `PresentationFamily` object:

- `PresentationFamily.Id`: the family the talk belongs to
- `PresentationFamily.Variant`: the talk's variant type within that family (for example `Canonical`, `ExecutiveOverview`, `Lightning`, `Workshop`)

Grouping the two fields into one object keeps family membership cohesive rather than spreading flat fields across the Talk root. A talk with no family simply omits the object.

The PresentationFamily entity does not list its members. Membership is discovered by querying Talks by `PresentationFamily.Id`, which keeps the relationship single-directional (matching the Talk → SlideDeckIds pattern) and avoids two copies of the same data drifting apart.

### Canonical talks

There is no `CanonicalTalkId` on the family. A family is not required to have a canonical talk at all. When one exists, "canonical" is expressed as a `PresentationFamily.Variant` value on the Talk, not as a structural field on the family.

### Proposed entity shape

```yaml
Id: 8ccdf8b8-fd2c-4d41-9fe0-32fade0f41dc
Name: RAG Deep Dive
Notes: |-
  This family includes both the "deep dive" and variant branding used for different audiences.
```

### Invariants

- PresentationFamily is not a taxonomy node.
- It is a grouping concept, not a category hierarchy.
- Two talks in the same PresentationFamily should not be co-submitted to the same conference.
- TalkCircuit enforces this rule at submission time.
- TalkCircuit can find a talk's family members by querying Talks that share its `PresentationFamily.Id`.

## LifecycleStatus

The current working model is concept-level lifecycle rather than build-status lifecycle.

### Proposed states

- Ideation
- Active
- Retired

### Design intent

- Ideation: the idea exists, but may not yet have polished copy or a deck.
- Active: the talk is ready for proposal work or delivery.
- Retired: the talk should no longer be offered.

### Explicit non-goal

Deck build state does not belong here.

A Talk can be Active while a deck is still being assembled or while it has not been built yet. The deck's own status is handled by LiquidVictor, not TalkFolio.

## Proposal Copy

Proposal copy supports the speaking portfolio and CFP process. The current proposal language falls into a few categories:

- Abstract
- ElevatorPitch
- ShortVersion
- CommitteeNotes
- AudienceNotes
- AlternateTitleCandidates

### Decision

Proposal copy is stored inline on the Talk record as `ProposalCopyItems`, a typed array where each item has:

- `Type`: the copy classification (for example `Abstract`, `ElevatorPitch`, `ShortVersion`, `CommitteeNotes`)
- `Copy`: the proposal text stored as a YAML `|-` literal block

This keeps proposal text extensible without changing the Talk schema whenever a new copy type is introduced.

## Related Content and Companion Material

Supporting material may be associated with a talk concept even when some of it lives in another domain, such as the CognitiveInheritance blog. To keep the model generic, TalkFolio should store lightweight references to that material rather than trying to own its lifecycle or publication metadata.

### Proposed shape

```yaml
RelatedContent:
  - Type: BlogPost
    Title: LLMs Under the Hood: What the Model Is Actually Doing
    Url: https://example.com/blog/llms-under-the-hood
    Notes: |-
      Companion article for the LLMs Under the Hood talk and workshop.
  - Type: BlogPost
    Title: Why LLMs Hallucinate and How to Reason About That Risk
    Url: https://example.com/blog/why-llms-hallucinate
    Notes: |-
      Related article on failure modes and trustworthiness.
  - Type: Video
    Title: LLMs Under the Hood — Deep Dive Recording
    Url: https://example.com/videos/llms-under-the-hood
    Notes: |-
      Recorded version of the longer explanation.
```

### Rules

- Related content is associated with the Talk concept, not with a specific built deck.
- A Talk may have zero, one, or many related content items.
- More than one item of the same `Type` is allowed.
- Each item has a `Type`, a `Title`, an optional `Url`, and `Notes`.
- There is no separate `Id` field on a related content item. When present, `Url` is the canonical identifier for the item.
- `Notes` is the primary place for context, framing, or reason for association, stored as a `|-` literal block.
- `Summary`, `Status`, and `PublishedAt` are intentionally not included here because those belong to the domain that actually owns the content lifecycle.
- If some associated material lives in CognitiveInheritance or another domain outside the TalkFolio repo, the TalkFolio entry can still reference it by type, title, URL, and notes without forcing that content to be modeled as a TalkFolio-owned resource.

### Usage guidance

- RelatedContent is intended for discovery, navigation, and finding companion material tied to a talk — for example, surfacing the blog posts, videos, articles, essays, or notebooks that expand on a talk's subject.
- It is a lightweight relationship model, not a content-management system or publication workflow. TalkFolio stores only the relationship and reference; the content's lifecycle (drafting, publication, updates, retirement) remains in the domain that owns that content, such as CognitiveInheritance for blog posts.

### Why this belongs here

This gives TalkFolio a generic, flexible way to model companion material without conflating it with deck-building or publication semantics. The content may live elsewhere, but the relationship to the Talk still belongs in TalkFolio.

## Reference Model

TalkFolio should point at external artifacts rather than own their lifecycle.

### LiquidVictor deck references

```yaml
SlideDeckIds:
  - 70b739b6-8dcb-43da-adc8-7392abf9a6ef
```

Rules:

- values are LiquidVictor `SlideDeck.Id` GUIDs
- LiquidVictor remains the source of truth for deck structure
- TalkFolio does not write back into the deck schema

### SlideFed publication references

```yaml
PublicPresentationReferences:
  - Source: SlideFed
    Url: https://example.com/presentation/...
    PublicId: some-fediverse-resource-id
```

Rules:

- SlideFed owns the public resource and its publication lifecycle
- TalkFolio only references it for display or cataloging

## Open Questions / Pending Decisions

These are the design questions that still need a decision before implementation:

### 1. ID strategy

Should Talk identity be:

- GUID only
- slug + GUID
- both slug and GUID

Current preference: both, with GUID as the canonical database identity and slug as a human-readable lookup value.

### 2. Tag source of truth

Should Tags be:

- fully freeform strings
- a repo-local controlled vocabulary
- a mix of controlled vocabulary + freeform additions

Current preference: repo-local controlled vocabulary, with a lightweight alias/mapping layer for specific conference tag sets.

### 3. Category model

Should Category remain a fixed enum or become a controlled list that can evolve over time?

Current preference: fixed enum initially, with a clear path to expand later if the domain grows.

### 4. Audience model

Should TargetAudience be a list of strings or a structured object such as:

```yaml
TargetAudience:
  - role: engineer
    level: intermediate
    note: people building AI systems in production
```

Current preference: list of strings initially, with structure added only if the repo decides to support more advanced audience segmentation later.

### 5. Extra talk metadata flags

The earlier design discussions raised flags such as:

- Locked
- ForKids
- HandsOn

These are not yet clearly placed. The current instinct is that they are talk-level metadata rather than deck-structure metadata, but this should be validated before implementation.

## Current Recommendation

The working baseline for the first TalkFolio implementation is:

- Talk entity with GUID + slug + canonical fields
- fixed Category enum
- Tags as a list of strings or repo vocabulary entries
- PresentationFamily as a separate grouping entity, with the Talk owning membership via a nested `PresentationFamily` object (`Id` + `Variant`)
- concept lifecycle of Ideation | Active | Retired
- references to SlideDeckIds and optional public publication references
- proposal copy stored inline as `ProposalCopyItems` (typed items with `|-` literal-block copy)
- companion material referenced via `RelatedContent` (typed, talk-level, lightweight references)

This gives a clean, minimal schema that matches the domain boundary without pulling in deck-building or submission-state concerns.
