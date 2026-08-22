# TalkFolio

TalkFolio is the SpeakerOps bounded context for managing talks as concepts, independent of whether any particular slide deck has been built yet.

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

## Status

Documentation-first repository. Domain model and schemas are not yet implemented.
