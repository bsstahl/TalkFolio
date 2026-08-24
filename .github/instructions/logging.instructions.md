---
description: "Logging requirements and conventions for TalkFolio implementation work"
applyTo: '**/*.cs'
---

## Logging requirements for TalkFolio

This repository treats logging as a required part of implementation quality. Any code change is not considered complete until its logging follows these rules.

The standard is straightforward:

* informational logs describe what the system is doing at boundaries and workflow transitions
* trace logs carry payload detail, verbose values, and object snapshots
* warnings and errors surface problems without dumping full payloads at default log levels
* sensitive values are never logged in raw form

## Required invariant

The following rule is mandatory for all TalkFolio implementation work:

* Method entry, exit, orchestration steps, cross-boundary calls, and major lifecycle events must be logged at `Information` level when they represent product-relevant activity.
* Payload detail, deserialized content, field values, and verbose diagnostic snapshots must be logged at `Trace` level.
* Recoverable issues, skipped records, and degraded-but-continued behavior must be logged at `Warning` level.
* Fatal failures, invalid configuration, and broken business invariants must be logged at `Error` level.
* Work is not considered finished unless the logging behavior matches this rule.

## What to log

### Informational logs

Log at `Information` when the code is doing work that matters to operators, reviewers, or other component boundaries. Typical examples include:

* repository or data-source startup and shutdown
* loading or scanning a catalog directory
* processing a batch of records
* querying a catalog or filter operation with a meaningful result count
* external integration handoff or retrieval
* completion of a major workflow step
* summarizing counts after a load or transform

Keep informational messages concise and outcome-oriented. They should answer: what boundary was crossed, what action happened, and what result was produced.

Examples:

* `Loading TalkFolio catalog from {DataRoot}`
* `Loaded talk catalog with {TalkCount} talk(s) and {FamilyCount} family(ies)`
* `Querying talks by category {Category}`
* `Completed catalog refresh in {ElapsedMs} ms`

### Trace logs

Log at `Trace` for the details that are useful for diagnosing behavior but would overwhelm normal logs. Typical examples include:

* the record identifier, title, type, and source location for a loaded item
* selected field values from a deserialized object
* filter arguments or result payload summaries for debugging
* file paths, source metadata, and record-level snapshots
* object graphs or partial payloads when they are needed for diagnosis

Trace logging is the place for payload detail. Do not put raw object dumps or verbose payloads into `Information` logs.

Examples:

* `Trace: Loaded talk {TalkId} with title {Title} from {SourcePath}`
* `Trace: Filter criteria category={Category}, tag={Tag}, lifecycle={Lifecycle}`
* `Trace: Enriched record payload for family {FamilyId} with variant {Variant}`

### Warning logs

Log at `Warning` when behavior is degraded, recoverable, or suspicious but not fatal. Typical examples include:

* a file was skipped because it was invalid or unreadable
* a record is missing an optional field and a default is being used
* duplicate or conflicting values were encountered but a safe fallback was applied
* a call succeeded with a degraded result, not a fully successful outcome

Warnings should identify the condition and the likely effect, but they should stay compact and actionable.

Examples:

* `Skipping malformed talk file {SourcePath} because the YAML could not be parsed`
* `No related content was found for talk {TalkId}; continuing with empty collection`
* `Using default category value because the source category was empty`

### Error logs

Log at `Error` when the operation fails or cannot safely continue. Typical examples include:

* repository root cannot be resolved or accessed
* YAML or configuration parsing fails at a required boundary
* required data is missing and the workflow cannot proceed
* an external dependency fails in a way that prevents the operation from succeeding

Error logs should include enough context to enable diagnosis, but they should not be a wall of raw payload. Use structured fields and short summaries.

Examples:

* `Failed to load TalkFolio catalog from {DataRoot}`
* `Unable to parse talk record {SourcePath}; validation failed for required field {FieldName}`
* `Catalog query failed for {Category}; repository returned an error`

## When to log

Log at the right point in the workflow:

* at method entry and exit when the method is a public or cross-boundary operation
* before and after file-system, repository, or integration calls
* before returning a result set from a query or catalog operation
* when a record is skipped, transformed, or normalized
* when a fallback path is taken
* when configuration or environment values materially affect runtime behavior

Do not log every low-level local calculation or trivial helper call unless it is part of a meaningful operational event. Prefer boundary-level logging over noisy method-by-method chatter.

There is a difference between a normal workflow detail and a meaningful operational event. Only the latter belongs in `Information` logs.

## How to log

### Prefer structured logging

Use `ILogger<T>` and message templates with structured placeholders. Do not rely on string concatenation in log messages.

Good:

```csharp
logger.LogInformation("Loading TalkFolio catalog from {DataRoot}", dataRoot);
logger.LogTrace("Loaded talk {TalkId} titled {Title}", talk.Id, talk.Title);
logger.LogWarning("Skipping malformed talk file {SourcePath}", sourcePath);
```

Avoid:

```csharp
logger.LogInformation("Loading TalkFolio catalog from " + dataRoot);
logger.LogTrace("Loaded talk " + talk.Id + " titled " + talk.Title);
```

### Keep message content concise

* Favor short, specific, action-oriented statements
* Keep the message readable in default logs
* Put verbosity into structured fields, not into long sentences
* Do not log full object graphs at `Information` or `Warning`

### Log only what is safe and relevant

Never log:

* secrets or API keys
* credentials or connection strings
* tokens, session identifiers, or password material
* personally identifying information unless it is explicitly needed and approved by the project requirement
* raw large payloads from external systems in default log levels

When a value might be sensitive, redact it or log only a non-sensitive identifier such as a record ID or a normalized type.

## Repository-specific expectations

For TalkFolio, logging should be aligned to the domain and the system boundaries described in the repo.

### Repository and storage boundaries

At repository boundaries, log:

* the data root or source path being used
* number of records loaded or skipped
* the start and end of a load operation
* any recoverable parse problems

Use `Information` for the overall operation and `Trace` for per-record metadata.

### Application/service boundaries

At application or service boundaries, log:

* the operation being performed
* the criteria used for a query or selection
* the result count or outcome
* anything that materially changes execution flow

Use `Information` for operation-level activity and `Trace` for query criteria, payload contents, and subtotal details.

### Integration boundaries

When calling other SpeakerOps components or external systems, log:

* the boundary crossed
* the operation name or request type
* the outcome
* expected fallback behavior when the call is degraded or partially successful

If a third-party or downstream service returns data, log a concise summary at `Information` and the detailed payload at `Trace` only when necessary.

## Required patterns by level

### `Information`

Use for:

* lifecycle transitions
* major operational steps
* cross-boundary orchestration
* outcomes with useful counts or summaries

Keep it to the signal you want in normal production logs.

### `Trace`

Use for:

* record-specific metadata
* payload snapshots
* file paths and identifiers
* filter criteria and per-item detail

This is where the detailed debugging material lives.

### `Warning`

Use for:

* recoverable issues
* skips or defaults
* degraded but continuing behavior
* suspicious input that the code handled safely

### `Error`

Use for:

* failed operations
* bad configuration
* invalid required inputs
* irrecoverable failures affecting the request or workflow

## Completion checklist for every change

Before a task is considered complete, confirm all of the following:

* the code logs major workflow boundaries at `Information`
* payload detail is reserved for `Trace`
* warnings and errors are explicit and actionable
* no sensitive data is logged
* message templates use structured logging, not string concatenation
* the logging level matches the business meaning of the event
* the logs are scoped to the actual boundary or workflow, not every helper method

If a change introduces new code but does not add or refine log statements when needed, it is incomplete.

## Examples from this repo

The repository convention should look like this:

```csharp
logger.LogInformation("Loading TalkFolio catalog from {DataRoot}", dataRoot);
logger.LogTrace("Loaded talk {TalkId} titled {Title} from {SourcePath}", talk.Id, talk.Title, sourcePath);
logger.LogWarning("Skipping malformed talk file {SourcePath}", sourcePath);
logger.LogError("Failed to load catalog from {DataRoot}", dataRoot);
```

This pattern keeps normal logs readable while preserving the detailed evidence needed when trace logging is enabled.
