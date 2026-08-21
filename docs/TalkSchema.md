# TalkFolio Schema Direction

This document captures the current schema direction for TalkFolio, based on the SpeakerOps decisions reached so far. It is intentionally a working proposal rather than a final implementation contract.

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
PresentationFamilyId: 8ccdf8b8-fd2c-4d41-9fe0-32fade0f41dc
LifecycleStatus: Active
Abstract: >-
  Retrieval-augmented generation is the most important architecture pattern for
  real-world AI applications that need grounded, explainable answers.
ElevatorPitch: >-
  In this talk, Barry explains how RAG works in practice and why so many teams
  struggle with retrieval quality, chunking, and evaluation.
ShortVersion: >-
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
IdeationNotes: >-
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
- PresentationFamilyId: grouping key for near-duplicate variants
- LifecycleStatus: concept-level state
- Abstract: long-form descriptive summary
- ElevatorPitch: succinct pitch language
- ShortVersion: compressed version for quick review or CFP summaries
- TargetAudience: list of audience descriptors
- SlideDeckIds: list of LiquidVictor `SlideDeck.Id` values
- PublicPresentationReferences: optional links to SlideFed or publication resources
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

### Proposed entity shape

```yaml
Id: 8ccdf8b8-fd2c-4d41-9fe0-32fade0f41dc
Name: RAG Deep Dive
CanonicalTalkId: 6c8d4d27-9cc7-4c41-9bf8-19e55758e7cc
TalkIds:
  - 6c8d4d27-9cc7-4c41-9bf8-19e55758e7cc
  - a183896a-795d-4d85-9d2d-da1b0c554f12
Notes: >-
  This family includes both the "deep dive" and variant branding used for different audiences.
```

### Invariants

- PresentationFamily is not a taxonomy node.
- It is a grouping concept, not a category hierarchy.
- Two talks in the same PresentationFamily should not be co-submitted to the same conference.
- TalkCircuit enforces this rule at submission time.

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

### Open decision

The remaining question is whether proposal copy should be stored inline on each Talk record or spread across referenceable markdown files.

Current default assumption:

- keep the primary proposal fields inline in the Talk record
- allow markdown-heavy detail to be captured as separate files only when the content becomes large or versioned

This keeps the model straightforward while still allowing richer content later.

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

### 3. Proposal copy layout

Should proposal text be inline on the Talk record or split into separate proposal documents referenced by the Talk?

Current preference: inline for the core fields, with markdown files only if a specific talk becomes very large or needs versioned copies.

### 4. Family semantics

Should PresentationFamily be a separate entity or a simple string field on Talks?

Current preference: separate entity for clarity and future metadata, but a simplified string field may be acceptable if implementation stays lightweight.

### 5. Category model

Should Category remain a fixed enum or become a controlled list that can evolve over time?

Current preference: fixed enum initially, with a clear path to expand later if the domain grows.

### 6. Audience model

Should TargetAudience be a list of strings or a structured object such as:

```yaml
TargetAudience:
  - role: engineer
    level: intermediate
    note: people building AI systems in production
```

Current preference: list of strings initially, with structure added only if the repo decides to support more advanced audience segmentation later.

### 7. Extra talk metadata flags

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
- PresentationFamily as a separate grouping entity
- concept lifecycle of Ideation | Active | Retired
- references to SlideDeckIds and optional public publication references
- proposal copy stored in a compact structured form on the Talk record

This gives a clean, minimal schema that matches the domain boundary without pulling in deck-building or submission-state concerns.
