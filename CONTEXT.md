# TalkFolio Context

Glossary-only domain language for TalkFolio. This file records canonical terms and invariants, not implementation details.

## Term: Talk
Definition: A speakable concept, independent of any specific slide deck, conference submission, or delivery event.
Owns: title, proposal copy, target audience, tags, category, PresentationFamily membership, concept lifecycle, talk-level flags, and references to built decks or companion material.
Distinguish from: LiquidVictor SlideDeck, TalkCircuit Submission, SlideFed PresentationSession.
Invariants:
- A Talk may exist before any deck exists.
- A Talk may reference zero or more LiquidVictor `SlideDeck.Id` values.
- A Talk may have multiple alternate titles or marketing variants.
- `Id` values are GUIDs.
- Talk-level flags are stored in a flexible `Flags` key-value map.

## Term: Category
Definition: A single top-level topic grouping used as the starting point for browsing and CFP selection.
Values: Agile, Algorithms, Language Models, Leadership & Community, Software Engineering.
Invariants:
- Exactly one Category per Talk.
- Category is intentionally coarse.
- Category is maintained as a controlled list that can expand over time.
- Cross-cutting topic nuance belongs in Tags, not nested category trees.

## Term: Tags
Definition: A many-to-many set of topical labels used for discovery, CFP fit, and cross-cutting classification.
Invariants:
- A Talk may have any number of Tags.
- Tags should capture overlap that a single Category cannot.
- Tags are free-form strings constrained to alphanumerics and `-` with no whitespace.
- Conference-specific tag mapping is not owned here; TalkCircuit maps TalkFolio Tags to a conference's allowed vocabulary.

## Term: TargetAudience
Definition: The intended audience for the Talk, expressed as a set of human-readable values from the TalkFolio vocabulary.
Invariants:
- A Talk may have zero or many TargetAudience values.
- TargetAudience values are strings drawn from a controlled list that can expand over time.
- This is intentionally simpler than a deeply structured audience model unless the repo later decides to add richer segmentation.

## Term: Flags
Definition: A flexible map of talk-level metadata flags.
Invariants:
- Each flag is a key-value pair.
- The map is intended for extension without changing the core talk schema.
- Examples include `Locked`, `ForKids`, and `HandsOn`.

## Term: PresentationFamily
Definition: A grouping of Talks or variants that are fundamentally the same presentation idea with different branding, format, or emphasis.
Invariants:
- PresentationFamily is not a taxonomy node.
- Talks in the same PresentationFamily should not be co-submitted to the same conference.
- TalkCircuit enforces the no-duplicate-family rule at submission time.
- The Talk owns the family relationship via a nested `PresentationFamily` object (`Id` + `Variant`); the family entity does not list its members.
- `Variant` identifies the talk's role in the family (for example `Canonical`, `ExecutiveOverview`, `Lightning`, `Workshop`).
- A family is not required to have a canonical talk; when one exists, it is expressed as a `Variant` value, not a structural field on the family.

## Term: LifecycleStatus
Definition: The concept-level state of a Talk.
Canonical states: Ideation, Active, Retired.
Invariants:
- Ideation means the talk idea exists but may not have proposal copy or a deck yet.
- Active means the talk is available for submission or delivery.
- Retired means the talk concept should no longer be offered.
- Deck construction status is not represented here; that belongs to LiquidVictor.

## Term: Proposal Copy
Definition: Text used to pitch a Talk to conferences or selection committees.
Examples: abstract, short version, elevator pitch, memo to selection committee, key takeaways, target audience.
Invariants:
- Proposal copy belongs to TalkFolio, even when it is later submitted through TalkCircuit.
- Conference-specific submitted versions may be captured by TalkCircuit as submission snapshots.
- ProposalCopyItems.Copy is intentionally unstructured prose; all other fields should be structured unless they are explicitly narrative/context fields.

## Term: SlideDeckIds
Definition: References from a Talk to one or more built LiquidVictor decks.
Invariants:
- Values are LiquidVictor `SlideDeck.Id` GUIDs.
- TalkFolio holds these references; LiquidVictor does not reference TalkFolio.
- A Talk in Ideation may have no SlideDeckIds.

## Term: PublicPresentationReference
Definition: Optional reference to a public or federated presentation resource exposed by SlideFed or another publication surface.
Invariants:
- This is a pointer for catalog/display purposes.
- SlideFed owns the federated resource and its ActivityPub semantics.

## Term: RelatedContent
Definition: Companion material associated with a Talk, such as blog posts, videos, articles, essays, or notebooks, referenced for discovery, navigation, and finding material tied to a talk.
Invariants:
- Related content is associated with the Talk concept, not with a specific LiquidVictor SlideDeck.
- One Talk may have zero or many RelatedContent entries.
- More than one RelatedContent item of the same type is allowed.
- Each item has a `Type`, a `Title`, an optional `Url`, and `Notes`.
- A RelatedContent item has no separate `Id`; when present, `Url` is the canonical identifier.
- `Notes` is the primary field for a brief explanation of the relationship to the talk, stored as a `|-` literal block.
- Related content may live in another domain such as CognitiveInheritance, outside the TalkFolio repo; TalkFolio stores only the lightweight reference and relationship.
- RelatedContent is a lightweight relationship model, not a content-management system or publication workflow; the content lifecycle remains with the domain that owns the content.
- `Summary`, `Status`, and `PublishedAt` are intentionally not part of this concept because those belong to the domain that owns the content lifecycle.
