# PRD/SPEC own behaviour; Design is theme only

BlazeCard’s authoritative behaviour lives in `docs/PRD.md` and `docs/SPEC.md` (SPEC wins on conflict between those two). Files under `docs/Design/` supply visual language (tokens, palette, typography, atmosphere). Mock IA and feature chrome in Design may be stale or wrong — do not implement Design detail when it contradicts PRD/SPEC or `CONTEXT.md`. We chose this so HBF/teal mocks can guide look without silently expanding scope (accounts, single-card-only preview, etc.).
