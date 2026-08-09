# BlazeCard — Product Requirements Document

> **Tagline:** Burning plastic cards for the better  
> **Purpose:** A live-preview single-page app for designing Apple Wallet passes and Google Wallet member cards, with pasteable integration code (dotnet-passbook C# and Google Wallet REST JSON).  
> **Glossary:** `CONTEXT.md`  
> **ADRs:** `docs/adr/`  
> **Authority:** This PRD + `docs/SPEC.md` define behaviour. `docs/Design/` is **theme / visual language only** (see ADR 0001). SPEC wins over PRD on implementation conflict; `CONTEXT.md` + ADRs win over both until those docs are updated.

---

## 1. Overview

BlazeCard is a **generic** developer/designer tool (optional **HBF Skin** for demos). It is **preview-only**: HTML/CSS **Live Preview** of Apple and Google faces, plus **Code Snippets** to copy into another application. It does **not** distribute passes, call Google Wallet live, or produce a `.pkpass` file in the UI.

### Key Goals

- Dual **Live Preview** (Apple + Google) that always follows one shared configuration.
- Three editors of that configuration: **Form**, **Code**, **Visual** — all two-way with the model.
- Pasteable **Apple Code Snippet** (full dotnet-passbook-shaped sample) and **Google Code Snippet** (full annotated HTTP envelope + body).
- Docker offline; no external network calls required for core use.
- Optional **Session Restore** (localStorage without images) so refresh does not wipe field work.

### Reference Projects

| Reference | Purpose |
|---|---|
| [`dotnet-passbook`](https://github.com/tomasmcguinness/dotnet-passbook) | API shape for Apple Code Snippets (PackageReference; `Generate()` never called in-app) |
| [Google Wallet Generic API](https://developers.google.com/wallet/generic) | Shape for Google JSON snippets (local DTOs; no Google client NuGet) |
| [`pkpass-html-viewer`](https://github.com/bkonetzny/pkpass-html-viewer) | Inspiration for HTML Apple pass preview |

---

## 2. Tech Stack

| Layer | Technology |
|---|---|
| Frontend | Blazor Server (`net9.0`) |
| UI Framework | MudBlazor |
| Apple snippet fidelity | `dotnet-passbook` NuGet (types/reference; **no** `Generate()` in app) |
| Google snippets | Local DTOs / templates (no `Google.Apis.Walletobjects`) |
| Drag-and-drop | SortableJS + native HTML5 drag |
| Code panes | BlazorMonaco |
| Containerisation | Docker (Linux) |
| Reverse proxy | Nginx optional (`prod` compose profile) |

---

## 3. Architecture

```
┌──────────────────────────────────────────────────────────────┐
│                     Workspace Shell                          │
│  ┌────────────┐  ┌─────────────────┐  ┌──────────────────┐ │
│  │  Sidebar   │  │  Editor (mode)  │  │  Live Preview    │ │
│  │  Form      │  │  Form / Code /  │  │  Apple │ Google  │ │
│  │  Code      │  │  Visual         │  │  (+ Preview Focus)│ │
│  │  Visual    │  └────────┬────────┘  └────────▲─────────┘ │
│  │  Skin/…    │           │                     │           │
│  └────────────┘           ▼                     │           │
│              Shared CardModel (sole source of truth)          │
│              Snippets re-emit ↔ model (best-effort parse)     │
└──────────────────────────────────────────────────────────────┘
```

State: scoped `IBlazeCardStateService` per Blazor circuit. Form, Visual, and Code all mutate `CardModel`. Live Preview binds to `CardModel`.

---

## 4. Application Modes

**Workspace Shell:** left sidebar for mode + contextual tools (Preset, Skin, Appearance Mode, Session Restore); right **Live Preview**. Theme tokens from Design system; default **HBF Skin** + **light** Appearance Mode; **Blaze Skin** available.

### 4.1 Form Mode

Structured editor for metadata, colours, images, **Pass Field** groups (Header / Primary / Secondary / Auxiliary / Back), barcode, and **Default Language** (default `en-AU`).

- **Pass Type:** Generic only in v1.
- **HBF Member Preset** pre-fills membership-style fields (not hard-wired domain properties).
- Edits update `CardModel` immediately → Live Preview + continuous snippet re-emit (debounced).
- **Build** validates and forces a clean snippet re-sync (e.g. after parse failure). Does not gate preview.

### 4.2 Code Mode

Two panes: Apple C# (full pasteable dotnet-passbook sample) and Google (full HTTP envelope + body).

- Primary action: **Copy** (clipboard). No file-download as primary export.
- Edits parse back into `CardModel` (best-effort, debounced); Live Preview follows model.
- On parse failure: keep last good model, warn; Form or **Build** / **Regenerate** for clean re-emit.
- **Regenerate Apple / Regenerate Google** re-emits from model (not “Convert”).
- Image lines: Apple = placeholder `File.ReadAllBytes` for filled slots; Google = placeholder HTTPS URLs (not session base64).

### 4.3 Visual Mode

Toolbox → **fixed drop zones** (Apple layout constraints). SortableJS + native drag. Visual is a **projection** rebuilt from `CardModel` when entering the mode; mutations write back to the model.

---

## 5. Shared Live Preview

- Dual Apple + Google HTML previews by default; optional **Preview Focus** (enlarge one + zoom).
- Apple: HTML/CSS approximation (flip for back fields OK). **No `.pkpass` download or generation.**
- Google: HTML/CSS approximation. **No** JWT / “save to Wallet” URL.
- **Barcode Preview:** real QR when format is QR; other formats are placeholders + message text.

---

## 6. Image Upload

- Slots: logo, icon, strip, hero, thumbnail — browse or drag-drop; PNG/JPEG/WebP; max 5 MB; in-memory data URIs for Live Preview only.
- Dimension warnings vs Apple/Google guidance (±10%).
- Used in Live Preview and to decide which placeholder image lines appear in snippets.

---

## 7. Certificates

**None in v1.** No dummy.p12, no openssl in Docker, no `CertificateService`. Real signing belongs in the integrator’s backend (shown as comments in the Apple snippet). See ADR 0002.

---

## 8. Card Data Model (summary)

Shared `CardModel`: PassType (Generic), Description, OrganizationName, LogoText, colours, field lists, image data URIs, barcode, **DefaultLanguage**. See SPEC for full C#.

---

## 9. Apple (dotnet-passbook)

- PackageReference for types/enums used when shaping snippets.
- **Never** call `PassGenerator.Generate()` in the app.
- Snippet service emits full pasteable C# (usings, request, fields, cert placeholders, `Generate()` + write-file for the *integrator*).

---

## 10. Google Wallet

- Local DTOs/templates only — **no** `Google.Apis.Walletobjects`.
- Map Generic Object fields from `CardModel`; emit full annotated HTTP request + body.
- Placeholder `{ISSUER_ID}` / `{ACCESS_TOKEN}` and image HTTPS placeholders.

---

## 11. Docker

- Build/publish Blazor app; expose 8080; health endpoint.
- **No** openssl / dummy cert generation.
- Optional nginx prod profile.

---

## 12. Session Restore

- Auto-save non-image configuration to `localStorage`.
- Optional explicit JSON import/export (may include images in the file format).
- Not a multi-user or server DB feature.

---

## 13. Non-Functional Requirements

| Requirement | Target |
|---|---|
| Preview update latency | < 200 ms after field change (debounce ~100 ms) |
| Snippet re-emit | Debounced with model updates (~100–300 ms); don’t clobber focused dirty parse |
| Image upload size | 5 MB per image |
| Browsers | Chrome 120+, Firefox 120+, Edge 120+, Safari 17+ |
| Mobile | Preview stacks below editor &lt; 768 px |
| Offline | No required external network for core preview/snippets |
| Persistence | Session Restore only; no server DB |

---

## 14. Out of Scope (v1)

- Real Apple certificates, `.pkpass` generation/download, Wallet install
- Live Google Wallet API / JWT save links / Google sign-in
- Pass types other than Generic
- Free-position Visual canvas; Blazor.Diagrams
- Auth / accounts / multi-user / server template library
- NFC / push updates / full localisation maps
- Analytics

---

*BlazeCard PRD v1.1 — aligned with CONTEXT.md and ADRs 0001–0004*
