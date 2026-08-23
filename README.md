# TalkFolio

TalkFolio is a product in the [SpeakerOps](../SpeakerOps/README.md) family for managing talks as concepts, independent of whether any particular slide deck has been built yet.

It owns the speaking portfolio: talk titles, proposal language, audience fit, topic classification, and the PresentationFamily grouping used to identify talks that are fundamentally the same idea or marketing variant.

## Code of Conduct

All contributors and users are expected to follow the [Strict Accountability Policy](CODE_OF_CONDUCT.md).

## Responsibilities

TalkFolio owns:

- talk concepts and their canonical identities
- abstracts, elevator pitches, short versions, and selection-committee language
- target audience descriptions
- alternate titles and marketing variants
- Category and Tags used for discovery and CFP fit
- PresentationFamily grouping
- concept-level lifecycle (`Ideation`, `Active`, `Retired`)
- references to built LiquidVictor decks that fulfill a talk concept
- optional references to published SlideFed resources that represent public/federated versions of a talk

TalkFolio does not own slide construction, Fediverse publication mechanics, or conference-submission state.

## SpeakerOps Integrations

| Context | Relationship |
|---|---|
| LiquidVictor | TalkFolio may reference LiquidVictor `SlideDeck.Id` values in `SlideDeckIds` once a deck exists. LiquidVictor does not reference TalkFolio. |
| SlideFed | TalkFolio may store public/federated URLs or identifiers exposed by SlideFed for catalog display. SlideFed does not depend on TalkFolio. |
| TalkCircuit | TalkCircuit submits TalkFolio talks to conferences and enforces the no-duplicate-family-per-conference rule using TalkFolio's PresentationFamily data. |

## Migration Sources

Initial TalkFolio data should be migrated from:

- `C:\s\r\CognitiveInheritance\Pages\Talk-Catalog.md`
- `C:\s\r\bss-notes\Community\Presentations\**`
- `C:\s\r\bss-notes\Community\Presentations\README.md`

The per-talk notes are especially valuable for abstracts, elevator pitches, audience notes, outlines, alternate titles, and rough ideation material.

## Additional Docs

- [TalkFolio Context](CONTEXT.md)
- [TalkFolio Integration Strategy](docs/Integration-Strategy.md)
- [TalkFolio Schema Direction](docs/TalkSchema.md)
- [Architecture Decision Records](docs/ADRs.md)

## Feature Candidates

| Feature | What it covers | MVP |
|---|---|---|
| Structured talk catalog | Store Talk records with GUID identity, title, lifecycle, category, tags, audience, flags, and references. | Yes |
| Proposal copy library | Keep typed proposal copy blocks on each talk for abstracts, pitches, and related CFP text. | Yes |
| Presentation family grouping | Group near-duplicate talk variants and classify each talk's family role with `PresentationFamily.Variant`. | Yes |
| Related content references | Link companion blog posts, videos, articles, essays, notebooks, and similar material to a talk. | Yes |
| Deck and publication references | Reference LiquidVictor decks and SlideFed/public presentation resources without owning their lifecycle. | Yes |
| Validation and linting | Validate talk files, schema rules, relationship integrity, and data quality. | Yes |
| Migration tooling | Import and normalize talk data from `bss-notes`, `Talk-Catalog.md`, and related sources. | Yes |
| Catalog browsing and filtering | List and filter talks by category, tag, lifecycle, audience, and family membership. | Yes |
| Controlled vocabularies | Maintain curated-but-extensible lists for Category and TargetAudience, plus tag formatting rules. | Later |
| TalkCircuit read model/export | Produce a stable machine-consumable output for downstream submission tooling. | Later |
| Authoring scaffolds | Generate starter Talk and PresentationFamily records from templates. | Later |
| Rich authoring UI | Form-based or browser-based editing experience. | Later |
| External sync automation | Automatic sync with external repos or content systems. | Later |
| Submission workflow | CFP state, acceptance tracking, bookings, and conference operations. | No |
| Publication workflow | SlideFed publication lifecycle or CognitiveInheritance content lifecycle management. | No |

## Recommended MVP

The recommended MVP should include:

- file-based Talk and PresentationFamily records that match the documented schema
- validation tooling for schema and cross-record relationships
- migration tooling to pull structured data from the existing notes sources
- basic catalog commands or reports for listing and filtering talks

The MVP should explicitly defer:

- controlled vocabulary source files for Category and TargetAudience
- TalkCircuit-friendly export/read model for proposal submission workflows
- rich editing UI
- authoring scaffolds unless they fall out cheaply from the file format work
- automated external sync
- submission-state workflows
- publication/content-management workflows outside TalkFolio's boundary

## Status

Documentation-first repository. Domain model and schemas are not yet implemented.
