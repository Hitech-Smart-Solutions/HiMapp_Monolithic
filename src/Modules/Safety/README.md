# Safety Module

Owns induction sessions, PPE issue rules, BOCW tests, medical records, and clearance decisions.

The module reads labour/project display data through `Admin.Contracts` and publishes clearance/test events through the shared outbox.
