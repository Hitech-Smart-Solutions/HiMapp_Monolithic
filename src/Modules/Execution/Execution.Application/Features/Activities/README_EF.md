# Note
This file is intentionally left blank in this iteration.

The current Execution.Application Activities feature uses an ActivityDto that does NOT match the database schema `Activity` table.
To correctly implement EF-backed CRUD, we must update:
- ActivityDto / ActivityRequest
- Commands/Queries/Handlers
- Controller payloads

Until that refactor happens, any EF repository is a temporary placeholder.

