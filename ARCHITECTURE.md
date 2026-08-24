---
title: TalkFolio Architecture
description: Architectural direction for TalkFolio as a product in the SpeakerOps family, including domain boundaries, implementation principles, and future evolution.
---

## TalkFolio Architecture

This document describes the intended architecture for TalkFolio as a product in the SpeakerOps family of products. It is intentionally oriented toward the repo's current documentation-first state, while still describing the architecture the implementation should preserve as the codebase evolves.

## Architectural objective

TalkFolio is a product for managing talk concepts as a standalone domain within the broader SpeakerOps ecosystem. It is designed to operate independently as a functional product while integrating with other SpeakerOps systems where useful.

The system should organize and validate the canonical talk data needed for:

* discovery and catalog browsing
* proposal copy management
* family grouping and variant tracking
* related companion material references
* integration with downstream systems that consume talk metadata

The architecture should make the ownership boundaries explicit so that TalkFolio stores the relationship and metadata it owns without trying to become a content-management system.

## Product boundary and integration model

TalkFolio is a self-contained product with its own domain responsibilities, while also participating in the SpeakerOps family of products through lightweight integration points.

### What TalkFolio owns

TalkFolio owns the concept-level modeling for a talk:

* Talk identity and canonical title
* proposal copy and narrative pitch material
* audience fit and target messaging
* topic classification via category and tags
* PresentationFamily membership and variant role
* talk lifecycle state (for example Ideation, Active, Retired)
* references to built decks or companion material relevant to the talk concept

### What TalkFolio does not own

TalkFolio does not own the lifecycle of content that belongs elsewhere, including:

* LiquidVictor deck authoring and deck lifecycle
* SlideFed publication or public presentation lifecycle
* Blog content creation or status management
* Conference submission workflow state and acceptance operations

This boundary is a first-class architectural rule. TalkFolio may participate in the broader SpeakerOps ecosystem and exchange references with external systems, but it remains a product with its own domain responsibility and operational identity. External systems remain authoritative for their own states and lifecycle events. TalkFolio stores only the lightweight relationship or pointer it needs.

## Architectural principles

### 1. Bounded domain, not publication platform

TalkFolio should model talks as durable concepts independent of whether a deck exists, a conference accepted the talk, or a public resource has been published.

That means a Talk can exist before any deck exists and may continue to exist after a deck is retired or superseded.

### 2. Relationship metadata, not content ownership

RelatedContent, deck references, and public presentation references are relationship metadata. They are not full content-management systems.

For example:

* a blog post may live outside the repo and still be linked to a Talk
* a video or notebook may be owned by another domain but referenced from TalkFolio
* a deck may be authored in LiquidVictor while TalkFolio simply stores the relationship to the deck

This keeps the repo light, consistent, and aligned to the domain that owns the content.

### 3. Domain-first data shape

The schema should prefer the smallest useful structure that clearly communicates ownership and intent.

Examples:

* `PresentationFamily` is a nested object containing the family identity and the talk's role in that family
* `RelatedContent` is a collection of typed companion references with `Type`, `Title`, optional `Url`, and `Notes`
* `Flags` remain a flexible map for extension without schema churn
* controlled vocabularies are intentionally coarse and extensible

### 4. Documentation-first implementation

This repo currently emphasizes design and schema direction before implementation. The architecture is intended to be durable enough that code changes can be validated against the documented model and boundary decisions.

### 5. Observability follows the same boundary discipline

TalkFolio's logging should describe activity without exposing verbose payload detail at normal log levels.

* informational logs describe boundary activity, orchestration, and successful cross-layer work
* trace logs hold payload snapshots, field-level detail, and other verbose diagnostic data
* warnings and errors should identify failure conditions without dumping full payload bodies unless a deeper trace is explicitly required

This keeps the default signal high while preserving enough detail to debug when trace logging is enabled.

## Planned system structure

Although the repo does not yet contain the full application, the intended structure should remain aligned to a simple layered design.

### Domain layer

Responsible for the talk model and the core invariants.

Likely responsibilities:

* Talk and PresentationFamily concepts
* category, tags, audience, and lifecycle semantics
* validation rules and invariants
* value objects for controlled vocabularies and flags
* talk-level relationship metadata

This layer should remain stable and independent of UI or infrastructure concerns.

### Application layer

Responsible for workflows that operate on the Talk model.

Likely responsibilities:

* parsing and validating talk data
* migration from source notes or external inventories
* catalog listing and filtering operations
* normalized exports or read models for downstream systems
* integrity checks for cross-record relationships

This layer coordinates domain logic without owning external content lifecycles.

### Infrastructure layer

Responsible for concrete storage and external integration details.

Likely responsibilities:

* file-based persistence or repository adapters
* YAML or JSON schema validation
* migration adapters from notes or imports
* integration clients for other SpeakerOps systems

This layer implements the operational mechanics behind the domain contract.

All interactions with persisted TalkFolio data should occur through repository abstractions at the application boundary. The initial implementation may use a file-based YAML store, but the rest of the product should depend on repository contracts rather than file paths, directory layouts, or storage-specific mechanics.

For file-backed storage, the data root should be configurable and will generally live outside this implementation repo so the catalog can be versioned and maintained independently of the product code. This repo may still include dedicated test repositories or fixture datasets for automated tests, validation scenarios, and local development.

Record identity belongs to the data itself, not to the storage path. The canonical identity remains the record `Id`; file names and directory layout are infrastructure concerns that may change without changing the domain contract.

Configuration precedence follows the standard .NET pattern: JSON configuration files are loaded first, environment variables override file values, and in-memory configuration supplied by tests or host-specific bootstrap logic is applied last. This makes operational overrides easy to apply without changing code, while tests can intentionally force settings to ensure deterministic behavior.

The data-root for a file-backed catalog should therefore be supplied through the normal configuration system rather than being hard-coded. In practice, default values live in JSON config files, environment variables provide deployment-specific overrides, and test code can provide in-memory values when an individual test needs to override everything else.

### Integration boundaries

The main integration surfaces are intentionally narrow:

* LiquidVictor for deck references
* SlideFed for public or federated presentation references
* TalkCircuit for downstream conference submission logic
* external notes repositories or migration sources

TalkFolio should not start owning workflows that belong to these systems.

## Stable vs experimental areas

### Stable areas

These should remain durable and relatively low-change as the repo matures:

* Talk identity and core talk metadata
* PresentationFamily semantics
* related companion material model
* lifecycle semantics for talk concepts
* boundaries between TalkFolio and external systems

### Evolving areas

These can expand over time as the product grows:

* controlled vocabulary lists
* migration tooling
* catalog browsing and filtering features
* export/read models for downstream systems
* authoring templates and utilities
* richer query and reporting capabilities

## Data principles

### Identity

* `Id` values should be GUIDs for talk-level records
* human-facing naming remains in `Title` and other display-friendly fields

### Semantics

* `Category` is coarse-grained and controlled
* `Tags` are flexible and cross-cutting
* `TargetAudience` remains controlled but intentionally simple
* `Flags` provide extension points without schema churn

### Relationships

* Related content is associated to the Talk concept, not to a specific slide deck
* multiple companion items of the same type are allowed
* each companion item carries a lightweight reference and explanatory notes
* the owning domain remains responsible for the actual content lifecycle

## Non-goals

The repo should deliberately avoid becoming a system for:

* conference submission state management
* publication workflow orchestration
* deck-building and slide authoring
* content-management features with status, publication dates, or lifecycle states that belong elsewhere
* generic Copilot or instruction-authoring workflow infrastructure

These boundaries are part of the product design and should remain explicit in implementation work.

## MVP direction

The initial implementation should focus on:

* file-based talk records with schema validation
* PresentationFamily grouping semantics
* RelatedContent and proposal copy support
* migration tooling for existing notes and catalog sources
* simple catalog listing/filtering operations

The current repo explicitly defers larger, more workflow-heavy features until the domain model is stable and validated.

## Contribution alignment

Implementation work should stay aligned to:

* the Talk concept model
* the domain boundaries described in this repo
* the existing schema and ADR decisions
* the test methodology and C# conventions already defined by the repository

If a proposed change would broaden the repo into generic Copilot-instruction scaffolding or unrelated content-management operations, it should be rejected or redirected back to the relevant owning domain.

## Summary

TalkFolio should remain a small, durable, model-centric product in the SpeakerOps family: a repository for talking concepts, not an umbrella for the whole content pipeline. The architecture should make ownership explicit, keep external lifecycle systems external, and preserve the minimal metadata needed for discovery, navigation, and talk-level reference.
