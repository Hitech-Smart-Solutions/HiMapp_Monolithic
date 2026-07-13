# Admin Module

Owns project, contractor, labour registration, labour status derivation, and gatepass issue/renew workflows.

Cross-module access is exposed through `Admin.Contracts` only. The module publishes `Admin.LabourRegistered` for Safety induction intake and consumes `Safety.LabourClearanceChanged` to update the labour clearance banner/status.
