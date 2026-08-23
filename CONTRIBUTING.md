---
title: Contributing to TalkFolio
description: Contribution expectations for the TalkFolio SpeakerOps repository, including tooling, workflow, and review conventions.
---

## Contributing to TalkFolio

This repository is TalkFolio, a fully functional product in the SpeakerOps family of products for managing talk concepts. Contributions should keep the product focused on talk identity, proposal language, catalog metadata, and the relationships that help surface and discover a talk without owning external content lifecycles.

## Purpose and scope

TalkFolio owns:

* the Talk concept itself
* talk-level identity and lifecycle
* proposal copy and related narrative language
* PresentationFamily grouping for family variants
* category, tags, audience, and flags used for discovery
* references to deck or public-resource material when those are relevant to the talk concept

TalkFolio does not own:

* slide creation or deck authoring workflows
* publication lifecycle for content outside this repo
* submission-state management for conferences

When in doubt, keep the work aligned to the TalkFolio product domain and the repo's existing design docs, while preserving the product's independence within the broader SpeakerOps family.

## Required repository guidance

Before making changes, read the repo guidance that applies to the work:

* [README.md](./README.md)
* [CONTEXT.md](./CONTEXT.md)
* [docs/TalkSchema.md](./docs/TalkSchema.md)
* [docs/Integration-Strategy.md](./docs/Integration-Strategy.md)
* [docs/ADRs.md](./docs/ADRs.md)
* [.github/instructions/csharp.instructions.md](./.github/instructions/csharp.instructions.md)
* [.github/instructions/test-methodology.instructions.md](./.github/instructions/test-methodology.instructions.md)
* [.github/instructions/markdown.instructions.md](./.github/instructions/markdown.instructions.md)
* [.github/instructions/writing-style.instructions.md](./.github/instructions/writing-style.instructions.md)

## Tooling and environment

This repository is expected to target the latest .NET line with C# 14 and .NET 10 conventions, following the repo's existing C# guidance.

Use the standard .NET CLI from the repository root:

```bash
dotnet build
dotnet test
```

At this stage, the repo is still documentation-first; do not assume there is a full implementation or a fully populated CI pipeline. When implementation work begins, keep the project runnable from the repo root with no hidden setup steps.

## Test data conventions

When you add TalkFolio-only test data, keep it aligned to the repository's established Beavis & Butthead / Great Cornholio theme.

Use that theme for fixture talks, related content, sample speakers, and similar repository-local examples whenever it fits naturally. The current baseline example speaker is The Great Cornholio, with talks such as "Finding TP for Your People's Bungholes" and "Identifying Sources of Caffeine."

This convention exists to keep test fixtures recognizable, consistent, and clearly separate from real speaking-catalog content. It applies to repository-local tests and fixtures, not to the product's canonical schema or to external production data sources.

## Development workflow

### 1. Start from the design baseline

Use the canonical domain docs as the first source of truth before editing code or schema. Keep the Talk model, constraints, and integration boundaries aligned to the repo docs.

### 2. Keep the task narrow and explicit

Prefer small, well-scoped changes. Avoid broad “cleanup” or “improve architecture” tasks without a concrete requirement or failing case.

### 3. Use TDD when behavior changes

For any code change that modifies behavior:

1. Write or update the failing test first
2. Confirm the test fails for the right reason
3. Implement only the minimum fix
4. Refactor carefully while keeping the relevant tests green
5. Re-run the relevant validation before continuing

Follow the repository test instructions in [.github/instructions/test-methodology.instructions.md](./.github/instructions/test-methodology.instructions.md).

### 4. Keep docs and implementation in sync

This repo is deliberately documentation-heavy. When a change affects a domain decision, schema, or boundary, update the relevant docs alongside the work.

## Branching and review expectations

* Use a short-lived feature branch for work
* Keep branch scope focused on one change or one cohesive task
* Prefer small commits over large batch edits
* Do not push to remote until the work is ready to share
* Use pull requests for merge review and discussion
* Accept pull requests from individual contributors
* Fully automated PRs are not accepted (those submitted by an agent instead of an individual)
* AI assistance is allowed when preparing a pull request, but the human contributor remains responsible for the content, accuracy, and intent of the submission

When the repo reaches its implementation stage, human review should remain required for changes that affect architecture, schema, or business rules.

## Coding and documentation conventions

### C# conventions

Follow the repo's C# standards in [.github/instructions/csharp.instructions.md](./.github/instructions/csharp.instructions.md):

* use .NET 10 conventions
* prefer clear naming and predictable project structure
* enable analyzers and treat warnings as errors in implementation work
* avoid unnecessary abstractions

### Markdown conventions

Follow [.github/instructions/markdown.instructions.md](./.github/instructions/markdown.instructions.md) and [.github/instructions/writing-style.instructions.md](./.github/instructions/writing-style.instructions.md) for all documentation edits.

Keep repository docs consistent with the repo's domain language and avoid generic Copilot framework phrases, imported template wording, or unrelated agent/skill scaffolding language.

## Boundaries and “do not touch” guidance

The repo has specific domain boundaries. Keep work inside these boundaries unless the user explicitly asks for a broader change.

When a source or file is external to this repo, treat it as authoritative only at its own boundary and keep TalkFolio responsible for the lightweight relationship model, not for owning that external lifecycle.

## Pull requests and validation

Before opening or completing a pull request:

* ensure the change matches the repo's current design intent
* verify related docs are updated when required
* run the smallest relevant validation locally
* confirm the change is scoped and understandable
* include any assumptions or follow-up work explicitly in the PR description

## Questions and escalation

If a change is ambiguous, crosses an external system boundary, or could affect the TalkFolio domain boundary, ask for clarification before making a broad implementation change.

The default stance is: stay narrow, stay domain-aligned, and keep the repo focused on the SpeakerOps talk concept.
