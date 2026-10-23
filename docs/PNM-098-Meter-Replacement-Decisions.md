# PNM-098 Meter Replacement Decisions

**Status:** BI boundary confirmed; implementation decisions pending

**Scope:** Decision record for PNM-098 and its future logbook dependency. This document does not authorize or implement meter replacement or logbook functionality.

## Purpose

BI has confirmed that PNM-098 records the meter transition but does not update cumulative equipment hours. The future logbook functionality in P&M Phase 2 will handle cumulative updates. This document separates those confirmed boundaries from the implementation questions that remain pending. No pending decision below is approved by this document.

## BI-confirmed requirements

| Requirement | Status | Clarification |
|---|---|---|
| Equipment has meter epochs and a cumulative meter record. | Confirmed | The existing PM entities represent these records. |
| Epoch opening and final/reset readings retain their established field meanings. | Confirmed | Preserve the current field names and do not assume that similarly named fields are interchangeable. |
| Example epoch usage | Confirmed | Old meter: opening `0`, final `1000`, usage `1000`. New meter: opening `100`, later reading `2000`, usage `1900`. Total usage across these periods is `1000 + 1900 = 2900`. |
| Cumulative meter value is not updated during replacement. | Confirmed | PNM-098 must not update `EquipmentMeterCumulativeDetail.FinalReadingAmount`. |
| Cumulative updates happen when a logbook entry is recorded. | Confirmed | This belongs to future logbook processing, not PNM-098. |
| A logbook entry is required following meter replacement. | Confirmed | BI has not decided whether it will be generated automatically or entered by a user. |
| Logbook screen and functionality are in P&M Phase 2. | Confirmed | No logbook UI, endpoint, entity, or processing is part of the current PNM-098 scope. |
| Replacement transitions the old and new meter epochs. | Confirmed | PNM-098 handles meter replacement only. Previously confirmed status requirements are old epoch `StatusID = 4` and new epoch `StatusID = 3`; the new epoch's opening value must follow the approved replacement rules. |

### Replacement and logbook boundary

| Current PNM-098 responsibility | Future P&M Phase 2 logbook responsibility |
|---|---|
| Validate the approved replacement request, close the old epoch, and open the new epoch. | Provide the logbook screen and the required logbook entry behavior following replacement. |
| Do not change cumulative hours. | Update cumulative hours when an approved logbook entry is recorded. |
| Do not create or depend on an interim logbook implementation. | Decide whether replacement-related logbook data is system-generated or user-entered and how it contributes to usage. |

For the BI example, the new meter's opening reading is `100`; its later reading is `2000`, yielding example usage of `1900`. This is an example, not a general default for every replacement. Preserve the existing entity and DTO field names and their established meanings. Do not add a logbook dependency, endpoint, entity, command, handler, service, or UI to PNM-098.

## Repository facts

### Meter data and current read behavior

- `src/Modules/PM/PM.Domain/Entities/EquipmentMeterEpochDetails.cs` defines nullable `ResetReadingAmount`, `OpeningReadingAmount`, `MeterReadingID`, `EquipmentID`, and `StatusID` fields. It does not define an old-meter final-reading field.
- `src/Modules/PM/PM.Domain/Entities/EquipmentMeterCumulativeDetail.cs` defines nullable `FinalReadingAmount` and `ActualOpeningReadingAmount` fields.
- `src/Modules/PM/PM.Application/Features/Equipments/EquipmentDto.cs` contains the existing replacement request and response DTOs. The request has nullable `ResetReadingAmount` and `OldMeterFinalReading`; the response has nullable `CumulativeEquipmentHours`. These property names do not establish calculation semantics.
- The PNM-099 handler at `src/Modules/PM/PM.Application/Features/Equipments/Handlers/GetEquipmentMeterRegisterSummaryQueryHandler.cs` reads `FinalReadingAmount` as the stored cumulative value. Its `EpochUsage` field is currently derived as `ResetReadingAmount - OpeningReadingAmount`. It does not update cumulative data or implement replacement behavior. This per-epoch expression does not establish how replacement readings contribute to the cumulative sum.
- The existing register query and handler are `GetMeterEpochRegisterQuery` and `GetMeterEpochRegisterQueryHandler` in `src/Modules/PM/PM.Application/Features/Equipments/Queries/EquipmentQueries.cs` and `src/Modules/PM/PM.Application/Features/Equipments/Handlers/EquipmentHandlers.cs`.
- No code found defines whether `EquipmentMeterEpochDetails.OpeningReadingAmount` or `EquipmentMeterCumulativeDetail.ActualOpeningReadingAmount` is the authoritative opening/offset value, or how those fields relate.

### Database and transaction facts

- `src/Modules/PM/PM.Infrastructure/PMDbContext.cs` maps both meter entities and adds indexes named `IX_EquipmentMeterEpochDetails_EquipmentID` and `IX_EquipmentMeterCumulativeDetail_EquipmentID`. The cumulative index is not configured as unique.
- The PM migration/model snapshot also represents the equipment indexes as non-unique. The schema therefore does not enforce one cumulative row per equipment.
- `src/Shared/SharedKernel/Abstractions/BaseEntity.cs` has identity, audit, and `UniqueID` properties, but no concurrency token, row version, or request idempotency key.
- `src/Modules/PM/PM.Application/IPMDbContext.cs` exposes EF Core's `DatabaseFacade`, which supports transactions. That gives an implementation access to atomic transactions; by itself, it does not enforce uniqueness or prevent two requests from applying the same transition.
- `src/Modules/PM/PM.Infrastructure/PMDbContextFactory.cs` configures Npgsql/PostgreSQL. The PM/Shared code search found no existing serializable transaction, advisory lock, `FOR UPDATE` pattern, or concurrency-token configuration. The explicit `BeginTransactionAsync` usage found in the broader application is in the Execution module, not an established PM replacement pattern.
- Existing PM controller actions return `409 Conflict` for selected `InvalidOperationException` validation failures, for example in `PlantMachineryController.cs`. This shows a conflict response convention, but not a current policy for duplicate or serialization failures.
- No request id or idempotency mechanism was found for meter replacement. `MeterReadingID` exists as an epoch field, but no uniqueness or idempotency meaning is established for it.

## Cumulative calculation boundary

The earlier options to update cumulative hours directly during replacement are superseded by BI's confirmation. **PNM-098 cumulative update formula: none; replacement must not modify the cumulative record.**

The BI example confirms the conceptual arithmetic for those example periods: each period's usage is its final reading minus its opening reading, and the example total is the sum of both period usages (`1000 + 1900 = 2900`). It does not, by itself, establish which existing database field supplies each value for every event or the detailed future logbook processing rules.

The existing PNM-099 handler currently derives its row `EpochUsage` as `ResetReadingAmount - OpeningReadingAmount`. It only reads the stored cumulative record. That code is not a replacement update and must not be used to add a cumulative write to PNM-098.

For future P&M Phase 2 logbook work, the business/technical team must decide how logbook entries identify the relevant epoch opening and final readings, how usage is accumulated across epochs, and how the resulting cumulative value is updated exactly once.

## Null and inconsistent-value decisions

BI has not specified null-handling for replacement validation or future logbook processing. Do not silently substitute zero for missing readings. The cumulative record is not updated by PNM-098, so a missing cumulative record or null `FinalReadingAmount` must not be handled by inventing or writing a replacement value. Whether either condition blocks the epoch transition remains a pending validation decision.

| Case | Possible behavior | Data-integrity consideration |
|---|---|---|
| Cumulative record is missing | Allow the epoch transition without a cumulative write; or reject until data is repaired | PNM-098 does not create or update the cumulative record. The business must decide whether its presence is a replacement precondition. |
| Cumulative `FinalReadingAmount` is null | Allow the epoch transition without changing the null; or reject pending repair | Do not replace null with zero. Cumulative update behavior belongs to future logbook processing. |
| Old meter final reading is missing | Reject replacement; or allow transition and require logbook/manual reconciliation | Without a confirmed final reading, the old period's usage may not be recoverable. The source field is still unresolved. |
| New meter opening reading is missing | Reject; or use another explicitly approved source | Do not assume zero. The BI example uses `100`, and does not establish the default for other replacements. |
| Opening/offset values are missing or differ | Reject pending reconciliation; or follow a later approved source-of-truth rule | The repository does not establish equivalence or precedence between `EquipmentMeterEpochDetails.OpeningReadingAmount` and `EquipmentMeterCumulativeDetail.ActualOpeningReadingAmount`. |

## Duplicate submissions and retries

- Per the confirmed business requirements, current epochs use `StatusID = 3` and closed epochs use `StatusID = 4`. A sequential replay referencing an epoch that has already been closed can be rejected by checking that the requested epoch is still current. Exact replacement validation and duplicate handling remain pending.
- The existing DTO includes `CurrentMeterEpochID`, which could serve as a stale-state precondition. It is not an idempotency key: after a timeout, the client cannot tell whether the first request committed, and a replay cannot return the original response from this identifier alone.
- No request identifier or idempotency store was found in the inspected meter code. The current fields do not uniquely identify a replacement request, and `MeterReadingID` has no established uniqueness guarantee.
- Two replacement requests can both observe the same current epoch before either commits unless an approved concurrency mechanism serializes or rejects one. A transaction alone does not prove that only one transition is applied.
- Future logbook submissions also need a duplicate/retry policy so the same usage cannot be included in cumulative hours twice. No logbook request identifier or processing mechanism exists in the current code; this must be designed in P&M Phase 2.
- PNM-098 must not create or update cumulative records. If future logbook processing creates a missing cumulative row, concurrent creation could produce duplicates because the current `EquipmentID` index is non-unique. Whether logbook processing may create rows is not established.

## Concurrency options for future cumulative updates

### PostgreSQL `SERIALIZABLE`

PostgreSQL serializable transactions may abort one of two concurrent transactions when their combined reads and writes cannot be serialized. The application must treat the serialization failure (PostgreSQL SQLSTATE `40001`) as an aborted operation; it must not continue or report success for that transaction. An explicit retry policy could retry the entire operation, while a no-retry policy could return a conflict and let the client decide whether to resubmit.

No PM code currently establishes this isolation level or handles serialization failures. Returning HTTP 409 is compatible with existing PM controller patterns for some validation conflicts, but the API does not currently define client retry semantics for replacement or future logbook updates. The technical team must approve whether to return 409, whether/how clients retry, and how to ensure a retry cannot apply a committed logbook update twice.

Serializable transactions can protect a transaction's read/write outcome; they do not create a unique database constraint. If future logbook processing is permitted to insert a missing cumulative row, uniqueness for concurrent inserts still needs explicit treatment. This task must not claim the existing non-unique index enforces one row per equipment.

### Other options and trade-offs

- **Schema uniqueness/concurrency token:** Strong database enforcement, but requires a schema/migration change and is outside the current no-schema-change constraint.
- **Database row or advisory lock:** Provider-specific and would need an established locking target, especially when the cumulative row is missing. No current PM pattern was found; do not add one without design approval.
- **Application check plus ordinary transaction:** Small implementation, but not sufficient to guarantee against simultaneous requests or duplicate cumulative rows.

## Decision checklist

BI-confirmed requirements above are **Confirmed**. Every unresolved decision below is **Pending**. Recommendations are technical safeguards, not business approval.

| # | Decision requiring clarification | Available options | Recommendation (if technically justified) | Approval status |
|---:|---|---|---|---|
| 1 | Which existing field represents the old meter's final reading? | Map the conceptual final reading to an established epoch field; use the existing request's `OldMeterFinalReading`; identify another authoritative source | Do not infer from field names. Confirm the source and its relationship to `ResetReadingAmount` before implementation. | Pending |
| 2 | What should a future replacement-related logbook record capture? | Old meter final reading; new meter reading; both readings; other approved event data | Capture enough approved data to calculate the replaced epoch's usage exactly once, but the business must choose the record contents. | Pending |
| 3 | How will future logbook processing calculate usage across epochs? | Sum each epoch's final minus opening reading as illustrated; another documented rule | The BI example supports the arithmetic for those two example periods, but field mapping and processing semantics still need confirmation. | Pending |
| 4 | How will P&M Phase 2 prevent duplicate logbook submissions or retries from counting usage twice? | Durable idempotency key; reject already-processed entry; another approved mechanism | Use durable duplicate detection for exactly-once effects; no such key/store exists in current meter code. Design it in Phase 2. | Pending |
| 5 | What concurrency strategy will protect future cumulative updates? | PostgreSQL `SERIALIZABLE` with handled `40001`; schema uniqueness/concurrency token; database lock; application check only | Evaluate serializable transactions with explicit conflict handling if schema changes remain disallowed. Do not treat an application check or transaction alone as sufficient. | Pending |
| 6 | What replacement validation and duplicate-request rules must PNM-098 enforce? | Validate current epoch/status, required readings and date; reject stale/closed epoch; define behavior for repeated submissions and API response | Require the requested epoch to be current and reject stale transitions, subject to an approved concurrency strategy. Define null handling and whether retries return conflict or the prior result. | Pending |
| 7 | What is the normal new meter opening reading? | Use the submitted opening reading; require zero; another business-defined rule | The BI example confirms `100` for that example only. Clarify whether it supersedes any earlier zero-opening requirement or is an example-specific value. | Pending |

## Approval gate

PNM-098 implementation remains on hold until the business owner resolves decisions 1 through 3 and 6 through 7, and the technical team approves decisions 4 through 5. The P&M Phase 2 logbook work remains a future dependency. No pending decision is approved by this document.

