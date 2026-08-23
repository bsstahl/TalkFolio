# Integration Strategy

TalkFolio is the catalog of talk concepts inside SpeakerOps. It should stay focused on the speaking portfolio and avoid taking ownership of deck-building, federation, or conference workflow state.

## LiquidVictor

TalkFolio references built decks by LiquidVictor `SlideDeck.Id`.

```yaml
SlideDeckIds:
  - 70b739b6-8dcb-43da-adc8-7392abf9a6ef
```

Rules:

- LiquidVictor remains the source of truth for deck structure and build-time presentation metadata.
- TalkFolio never writes fields back into LiquidVictor YAML.
- A Talk may have zero decks, one deck, or multiple format-specific decks.

## SlideFed

TalkFolio may store public URLs or federated identifiers produced by SlideFed for catalog display.

Rules:

- SlideFed owns ActivityPub/ActivityStreams resources and session interaction.
- TalkFolio treats SlideFed identifiers as external publication references.
- TalkFolio does not model session lifecycle, follows, annotations, or federation delivery.

## TalkCircuit

TalkCircuit consumes TalkFolio data when preparing submissions.

TalkCircuit needs from TalkFolio:

- Talk identity
- current LifecycleStatus
- Category and Tags
- PresentationFamily membership
- proposal copy
- references to built decks when available

Rules:

- TalkCircuit owns submission and booking state.
- TalkCircuit may snapshot proposal copy at submission time, because submitted wording can diverge from the current catalog text.
- TalkCircuit enforces the rule that two Talks from the same PresentationFamily are not submitted to the same conference, finding a talk's family members by querying Talks that share its `PresentationFamily.Id`.

## Related Content

TalkFolio stores lightweight references to companion material (blog posts, videos, articles, essays, notebooks, and similar) via the Talk's `RelatedContent` collection.

Rules:

- RelatedContent is associated with the Talk concept, not with a specific LiquidVictor deck.
- Some associated material lives outside the TalkFolio repo (for example, blog posts in CognitiveInheritance) and is still linked from the Talk by type, title, URL, and notes.
- The owning domain retains the content lifecycle; TalkFolio never models publication state, summaries, or dates for related content.

## Migration Sources

Initial import candidates:

- `C:\s\r\CognitiveInheritance\Pages\Talk-Catalog.md`
- `C:\s\r\bss-notes\Community\Presentations\README.md`
- `C:\s\r\bss-notes\Community\Presentations\**\*.md`

Expected migration mapping:

| Existing source content | TalkFolio target |
|---|---|
| README mindmap top-level branch | Category |
| README glyph/status annotations | Lifecycle or flags after review |
| README grouping / near-duplicate clusters | PresentationFamily |
| Per-talk abstract | Proposal copy |
| Elevator pitch / short version | Proposal copy |
| Target audience | Target audience |
| Other possible titles | Alternate titles |
| Rough notes / TODOs | Ideation notes |

## Decisions Made

- Should proposal copy be stored inline in Talk records or as separate markdown files? See [ADR-001](ADRs.md#adr-001-proposal-copy-is-stored-inline-as-a-typed-array).
- Should the product couple itself directly to a file-based store? See [ADR-011](ADRs.md#adr-011-storage-access-occurs-through-repository-abstractions).
- Should the maintained file-backed catalog live in this repo? See [ADR-012](ADRs.md#adr-012-file-backed-data-roots-are-configurable-and-generally-external-to-this-repo).
- Should Talk identities be GUIDs, slugs, or both? See [ADR-005](ADRs.md#adr-005-talk-ids-use-guids).
- Should Tags be controlled by a repo-local vocabulary file or free-form strings? See [ADR-006](ADRs.md#adr-006-tags-are-free-form-string-tokens-constrained-to-alphanumerics-and-dash).
- Should Category remain fixed, or be a controlled list that can expand? See [ADR-007](ADRs.md#adr-007-category-uses-a-controlled-extensible-list).
- Should TargetAudience be structured or a list of strings? See [ADR-008](ADRs.md#adr-008-targetaudience-uses-a-controlled-extensible-list-of-strings).
- How should extra talk metadata flags be modeled? See [ADR-009](ADRs.md#adr-009-extra-talk-metadata-flags-use-a-flexible-key-value-map).
- Which fields should remain unstructured? See [ADR-010](ADRs.md#adr-010-only-narrative-context-fields-remain-unstructured).
- How much of the existing `bss-notes` prose should remain freeform versus structured? See [ADR-010](ADRs.md#adr-010-only-narrative-context-fields-remain-unstructured).

## Open Questions

- What exact on-disk directory layout should a file-backed repository use for Talks and PresentationFamilies?
- What file-naming convention should a file-backed repository use for stable, readable records?
- How should the configurable data-root path be supplied at runtime (for example, CLI option, config file, environment variable, or a combination)?

## MVP Implementation Plan

This section captures the medium-term implementation plan for TalkFolio. Short-term execution notes can stay in the session plan, but the durable MVP direction belongs in the repo.

### Goal

Implement a file-based TalkFolio MVP that can hold the structured speaking catalog, validate it, import the initial note sources, and support basic catalog exploration.

### MVP scope

1. Define the on-disk record layout for Talk and PresentationFamily data.
2. Implement schema and cross-record validation.
3. Implement migration tooling from existing notes sources.
4. Provide basic catalog listing and filtering capabilities.

### Phases

#### Phase 1: Repository structure and sample records

- Choose the on-disk directory layout for Talk and PresentationFamily records.
- Define the file-naming convention for file-backed records.
- Define how the configurable data-root path is supplied.
- Keep maintained catalog data generally outside this repo, while adding dedicated test repositories or fixture datasets here for validation.
- Add sample records that exercise the full schema.

#### Phase 2: Parsing and validation

- Parse Talk and PresentationFamily files.
- Validate schema shape, required fields, and data types.
- Validate tag format, GUID fields, and unstructured-field boundaries.
- Validate cross-record links such as PresentationFamily membership and referenced records.

#### Phase 3: Migration tooling

- Parse the initial migration sources from `Talk-Catalog.md` and `bss-notes`.
- Map source content into structured TalkFolio fields.
- Surface ambiguous content for manual review instead of guessing.

#### Phase 4: Catalog read model

- Build a normalized read model representation of Talks and families.
- Implement list/filter capabilities for category, tag, lifecycle, audience, family, and flags.
- Expose stable catalog outputs for human and tool consumption.

#### Phase 5: Hardening and docs

- Add targeted tests for parsing, validation, migration, and filtering.
- Update repo docs to describe the file layout and validation workflows.
- Validate the sample data and migration flow end-to-end.

### Risks to watch

- Overfitting the migration tooling to messy source notes.
- Letting validation rules drift from the ADRs and schema docs.
- Pulling TalkCircuit or SlideFed workflow concerns into TalkFolio.
- Leaving the feature surface too broad before the file model and migration path are proven.

### Suggested first implementation slice

Start with Phase 1 plus the minimum Phase 2 validator needed to load one Talk and one PresentationFamily successfully.
