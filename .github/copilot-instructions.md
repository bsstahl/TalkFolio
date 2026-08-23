# Agent Instructions for this Repository

All instructions per below are to be followed whenever taking action in this repository unless the user has given explicit instructions to the contrary AND you have verified their intent with a validation of the form "These instructions run contrary to my standard instructions found in {filepath} which state {rule}".

* All code will be written in the latest version of C#, targeting .NET 10.

* All .NET code analyzers will be enabled, set to treat warnings as errors, and analyzer instructions followed fully.

* All code will be written using the [TDD Methodology](../.github/instructions/test-methodology.instructions.md).

* Repository documentation and public-facing text should stay aligned with the repo's SpeakerOps domain and not drift into generic Copilot authoring scaffolding.

* Do not do work on the `main` (default) branch. It is protected. Use a feature-branch for all changes and then PR them to `main` for review and merge. If asked to make changes on the `main` branch, confirm the request with a validation of the form "I have been asked to make changes on the main branch. Please confirm that this is correct and that you understand the risks of doing so."
