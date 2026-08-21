# TalkFolio Context

Glossary-only domain language for TalkFolio. This file records canonical terms and invariants, not implementation details.

## Term: Talk
Definition: A speakable concept, independent of any specific slide deck, conference submission, or delivery event.
Owns: title, abstract, pitch language, target audience, tags, category, PresentationFamily membership, concept lifecycle, and references to built decks.
Distinguish from: LiquidVictor SlideDeck, TalkCircuit Submission, SlideFed PresentationSession.
Invariants:
- A Talk may exist before any deck exists.
- A Talk may reference zero or more LiquidVictor `SlideDeck.Id` values.
- A Talk may have multiple alternate titles or marketing variants.

## Term: Category
Definition: A single top-level topic grouping used as the starting point for browsing and CFP selection.
Values: Agile, Algorithms, Language Models, Leadership & Community, Software Engineering.
Invariants:
- Exactly one Category per Talk.
- Category is intentionally coarse.
- Cross-cutting topic nuance belongs in Tags, not nested category trees.

## Term: Tags
Definition: A many-to-many set of topical labels used for discovery, CFP fit, and cross-cutting classification.
Invariants:
- A Talk may have any number of Tags.
- Tags should capture overlap that a single Category cannot.
- Conference-specific tag mapping is not owned here; TalkCircuit maps TalkFolio Tags to a conference's allowed vocabulary.

## Term: PresentationFamily
Definition: A grouping of Talks or variants that are fundamentally the same presentation idea with different branding, format, or emphasis.
Invariants:
- PresentationFamily is not a taxonomy node.
- Talks in the same PresentationFamily should not be co-submitted to the same conference.
- TalkCircuit enforces the no-duplicate-family rule at submission time.

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
