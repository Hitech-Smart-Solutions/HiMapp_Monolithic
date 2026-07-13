# Shared Services

Shared services are intentionally technical or cross-cutting:

- `SharedKernel`: base entity, domain event, result, clock, current user abstractions.
- `Notifications`: transactional outbox model, notification rules, inbox models, SignalR hub, dispatcher abstraction.
- `Workflow`: workflow instance and approval history abstractions.
- `Files`: file asset registration abstraction for photos, signatures, thumbprints, and documents.
- `Integrations.D365`: staging models and sync abstraction for D365 master data.

Business rules that belong to a module should stay inside that module.
