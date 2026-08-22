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

## Open Questions

- Should Talk identities be GUIDs, slugs, or both?
- Should Tags be controlled by a repo-local vocabulary file?
- How much of the existing `bss-notes` prose should remain freeform versus structured?
