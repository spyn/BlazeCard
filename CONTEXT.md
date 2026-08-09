# BlazeCard

A browser-based, preview-only tool for designing Apple Wallet passes and Google Wallet cards and exporting integration code (dotnet-passbook C# and Google Wallet JSON). No live distribution or real credentials.

## Language

**BlazeCard**:
The product — a generic developer/designer workspace for wallet-pass preview and code export.
_Avoid_: treating HBF as the product identity

**HBF Skin**:
An optional visual theme and first-customer presentation (teal palette, Quokka mascot) layered on BlazeCard; not a separate product or domain model.
_Avoid_: HBF Health app, HBF-only tool

**Wallet Pass**:
The digital credential being designed — maps to an Apple Wallet pass and/or a Google Wallet object from one shared configuration. Canonical domain noun; “card” is brand/UI metaphor only.
_Avoid_: Card (as domain term), plastic card (except tagline metaphor), physical membership card

**CardModel**:
Implementation name for the shared in-memory configuration of a **Wallet Pass** (not a domain concept).
_Avoid_: using “Card” in product language to mean the domain entity

**Live Preview**:
The on-screen HTML/CSS approximation of the Apple Wallet pass and Google Wallet card. This is the **visual preview**. It is **not** a `.pkpass` file. A dummy-signed `.pkpass` also does **not** yield Apple’s real on-device look (Wallet won’t install it) — at best you’d inspect zip contents.
_Avoid_: calling Live Preview a “generated pass file”; equating `.pkpass` generation with visual fidelity; installable pass

**dotnet-passbook**:
NuGet **PackageReference** in the app. Apple **Code Snippets** are shaped using real library types/enums where useful; **`Generate()` is never called**. No `.pkpass` bytes; no dummy cert.
_Avoid_: implying BlazeCard ships a `.pkpass` file; calling `Generate()` in the product; docs-only / freehand C# that drifts from the package

**Apple Code Snippet**:
Full pasteable C# sample mirroring **dotnet-passbook**: usings, `PassGenerator`, request, fields, cert path comments, and `Generate()` + write-file lines as **what the integrator’s backend will run** — BlazeCard itself never calls `Generate()`. Live editor of the shared model (best-effort parse); **Live Preview** follows the model; **Copy** to paste elsewhere.
_Avoid_: fragment-only as the sole export; treating BlazeCard’s UI as calling `Generate()`; ephemeral-only code; file download as primary export

**Code Snippet**:
Integration text in **Code Mode** — Apple full pasteable **dotnet-passbook** sample; Google full annotated HTTP envelope + `genericObject` body. Live views of the shared model (best-effort parse-on-edit) plus **Copy**. Google image fields use placeholder HTTPS URLs; Apple C# emits placeholder file-path image lines for filled slots. Google JSON from local templates/DTOs — not `Google.Apis.Walletobjects`.
_Avoid_: Download files as main export; embedding session base64; Google client libs; body-only as the sole Google export

**Code Mode**:
Panes showing **Apple Code Snippet** + Google JSON beside **Live Preview**. Form, Visual, and Code all read/write the same model; preview follows the model. Parse of edited C#/JSON is best-effort; Form remains the reliable structured editor when parse fails.
_Avoid_: file download as primary export; `.pkpass`; Code as a dead one-way export

**Build**:
Explicit action that validates configuration and forces a clean re-sync of both **Code Snippets** from the shared model (e.g. after parse failure). Does not create a `.pkpass` file. Normal Form/Visual edits already re-emit snippets continuously (debounced).
_Avoid_: “download pass”; publish; distribute; gating Live Preview; implying snippets only update on Build

**Session Restore** (was “Workspace Snapshot”):
Optional client-side recovery of editor configuration after refresh: auto `localStorage` without images; optional explicit JSON import/export. Not a named “workspace” product feature.
_Avoid_: “workspace” as a domain noun; accounts; cloud sync; auto-saving base64 images

**Image Upload**:
User selects image bytes into a session image slot (logo, icon, strip, hero, thumbnail) for **Live Preview**, and to decide which placeholder image lines appear in the **Apple Code Snippet**.
_Avoid_: requiring icon because of a pass bundle; calling upload a download

**Default Language**:
A single BCP-47 tag used when emitting Google localized string values in **Code Snippets** and preview mappings. Not a multi-language value map. Default: `en-AU`.
_Avoid_: localisation feature; multiple TranslatedString entries per field in v1

**Preview Focus**:
UI affordance to enlarge/inspect one platform’s **Live Preview**; dual preview remains the default.
_Avoid_: single-card-only workspace as the product default

**Pass Type**:
Apple Wallet style. **v1 = Generic only.**
_Avoid_: Coupon/Event/Store/Boarding in v1 UI

**Pass Field**:
Labelled key/value in an Apple-shaped group (Header, Primary, Secondary, Auxiliary, Back).
_Avoid_: Member Name / Card Number / Expiry as hard-wired domain properties

**Preset**:
Named starter config (e.g. HBF Member) that pre-fills fields — not a new model concept.
_Avoid_: persisted template library

**Visual Mode**:
Fixed drop-zone editor (SortableJS); projects from shared config.
_Avoid_: free-position canvas; Blazor.Diagrams

**Workspace Shell**:
App chrome: left sidebar modes + right **Live Preview**.
_Avoid_: “workspace” as a saved project entity

**Skin** / **Appearance Mode**:
Brand accent (HBF | Blaze) × light/dark. Default: HBF + light.
_Avoid_: four fully separate themes; Skin ≠ Preset

**Barcode Preview**:
Real QR in Live Preview when format is QR; other formats are placeholders.
_Avoid_: claiming all formats are scannable in preview

## Relationships

- **Requirements authority:** `docs/PRD.md` and `docs/SPEC.md` define behaviour. `docs/Design/` supplies **theme / visual language** (tokens, look); IA and feature details in Design mocks may be stale or wrong — do not treat them as requirements.
- **Critical loop:** Form, Visual, and **Code Mode** all edit one shared configuration; **Live Preview** always follows that model; **Apple Code Snippet** stays a faithful **dotnet-passbook** replica for **Copy** into another app. Same for Google JSON + Google preview.
- **BlazeCard** may present an **HBF Skin**; the skin does not change the underlying **Wallet Pass** model or code-export contracts.
- **Skin** is brand accent; **Appearance Mode** is light/dark; **Preset** is starter pass data — three independent axes.
- One shared configuration (`CardModel` in code) is the sole source of truth; Visual is a projection rebuilt when entering Visual Mode.
- **Live Preview** shows both platforms by default; **Preview Focus** is optional magnification of one side.
- **Live Preview** updates continuously from the model; **Code Snippets** re-emit continuously (debounced) when the model changes from Form/Visual — unless the focused code pane has unsynced edit text mid-parse. **Build** validates and forces a clean re-sync of both panes.
- Code-pane edits parse back into the model best-effort (debounced); on parse failure, keep last good model, show a warning, Form remains authoritative for structured fixes.
- v1 **Wallet Pass** configurations are **Pass Type** Generic on Apple and Generic Object on Google.
- Form Mode edits core metadata plus **Pass Field** groups; an **HBF Member** **Preset** may pre-fill those groups.
- **Visual Mode** mutates the same configuration via fixed zones, not free layout. DnD uses SortableJS + native drag.
- **Workspace Shell** = app chrome only — not a saveable workspace entity.
- Apple **Live Preview** is HTML/CSS only. **No `.pkpass` file.** **`dotnet-passbook`** is a **PackageReference**; snippets use real types; **`Generate()` never called**. No dummy cert / openssl.
- **Barcode Preview** is QR-faithful; other formats are layout stubs.
- Google output is HTML preview + JSON snippet only — no save URL / JWT.
- No authentication or account concept in v1.
- **Session Restore** may auto-save non-image config to `localStorage`.
- Google localized strings use **Default Language** (default `en-AU`) only.

## Example dialogue

> **Dev:** "Is BlazeCard an HBF product?"
> **Domain expert:** "No — BlazeCard is the generic previewer. **HBF Skin** is branding for the first customer/demo; the same tool exports code for any organisation."
>
> **Dev:** "Do we only show Apple until they switch?"
> **Domain expert:** "No — **Live Preview** is dual by default. **Preview Focus** lets someone zoom one platform without abandoning the other."
>
> **Dev:** "Does the preview wait for Build?"
> **Domain expert:** "No — preview is live. **Build** only validates and fills Code Mode with snippets."
>
> **Dev:** "Is the thing we're editing a Card or a Pass?"
> **Domain expert:** "A **Wallet Pass**. ‘Card’ is the product name and metaphor — don't put it in the glossary as the entity."
>
> **Dev:** "Can I pick Boarding Pass in the form?"
> **Domain expert:** "Not in v1 — **Pass Type** is Generic only until we can do honest dual-platform support."
>
> **Dev:** "Where do Member Name and Expiry live in the model?"
> **Domain expert:** "They don't — they're just **Pass Fields**. Load the **HBF Member** **Preset** if you want those labels pre-filled."
>
> **Dev:** "Can I drag fields anywhere on the Visual canvas?"
> **Domain expert:** "No — only into **fixed drop zones**. Real wallets don't allow free layout."
>
> **Dev:** "I tweaked the C# in Code Mode — will the form update?"
> **Domain expert:** "Yes when parse succeeds — one model. If parse fails, fix in Form or **Build** to re-emit clean snippets."
>
> **Dev:** "Are we generating a preview?"
> **Domain expert:** "Yes — **Live Preview** (HTML on screen). Not a `.pkpass` file. The **Apple Code Snippet** is the dotnet-passbook replica you copy."

> **Dev:** "Are Form/Code/Visual top tabs?"
> **Domain expert:** "No — modes live in the left sidebar of the **Workspace Shell**; preview stays right."
>
> **Dev:** "Why is the UI teal if the product is BlazeCard?"
> **Domain expert:** "Default **Skin** is HBF for demos. Switch to **Blaze Skin** for the neutral product look — Skin ≠ Preset."
>
> **Dev:** "I edited fields in Form, then opened Visual — is that a second copy?"
> **Domain expert:** "No. Visual is rebuilt from the shared configuration every time you enter it."
>
> **Dev:** "The .pkpass won't open on my iPhone — is that a bug?"
> **Domain expert:** "We don't ship a `.pkpass` file. You get HTML **Live Preview** and a C# **Code Snippet** to paste into your app."
>
> **Dev:** "Why won't PDF417 scan from the preview?"
> **Domain expert:** "Only QR is a real **Barcode Preview** glyph in v1; other formats are placeholders."
>
> **Dev:** "Where's Add to Google Wallet?"
> **Domain expert:** "There isn't one — this is a previewer. **Copy** the JSON from **Code Mode** into your own backend."
>
> **Dev:** "The Design mock has an account menu — should I build login?"
> **Domain expert:** "No. PRD/SPEC win. Design is theme; ignore stray chrome that isn't in requirements."
>
> **Dev:** "Should the Google JSON include my uploaded logo as base64?"
> **Domain expert:** "No — use a placeholder HTTPS URL in the **Code Snippet**. Uploads feed **Live Preview** only."
>
> **Dev:** "How do images appear in the Apple C# snippet?"
> **Domain expert:** "As `File.ReadAllBytes(\"path/to/…\")` placeholder lines for slots you filled — not the session bytes."
>
> **Dev:** "Is light mode a third Skin?"
> **Domain expert:** "No — **Appearance Mode** is light/dark; **Skin** only swaps brand accents."
>
> **Dev:** "What does Convert from Google do?"
> **Domain expert:** "Don't call it convert — **Regenerate Apple** re-emits C# from the shared model (e.g. after a bad parse)."
>
> **Dev:** "I changed the form — why does Code Mode look outdated?"
> **Domain expert:** "It shouldn't — snippets re-emit continuously from the model. Use **Build** only to validate or force a clean re-sync after a bad parse."
>
> **Dev:** "What theme do we show on first open?"
> **Domain expert:** "Light **Appearance Mode**, **HBF Skin** — clinical demo default; user can switch."
>
> **Dev:** "Why isn't Google.Apis.Walletobjects in the csproj?"
> **Domain expert:** "We don't call Google. **Code Snippets** are local templates — keep the dependency out."
>
> **Dev:** "I refreshed and lost my pass — bug?"
> **Domain expert:** "Fields should come back from **Session Restore** auto-save. Logos won't — re-upload or import a JSON backup."
>
> **Dev:** "Why is language en-AU in the JSON?"
> **Domain expert:** "That's **Default Language**. Change the field if you need another tag — still one language, not localisation."
>
> **Dev:** "What's a Workspace?"
> **Domain expert:** "There isn't one as a product concept. **Workspace Shell** just means the app chrome. Config recovery is **Session Restore**."
>
> **Dev:** "Why does the snippet call Generate() if BlazeCard doesn't?"
> **Domain expert:** "The snippet is the integrator's backend sample. BlazeCard only shows **Live Preview** + that text for **Copy**."

## Flagged ambiguities

- Design PRD framed BlazeCard as HBF-only; PRD/SPEC framed it as generic — **resolved: generic product + optional HBF Skin (option C).**
- Design “single card focus” vs PRD/SPEC dual preview — **resolved: dual default + optional Preview Focus (option C).**
- Design “Build refreshes preview” vs PRD/SPEC live preview — **resolved: Live Preview always from model; Build validates + re-emits Code Snippets (evolved from Q3 A).**
- "Card" vs "Pass" overloaded — **resolved: domain = Wallet Pass / Pass; Card = brand + `CardModel` code name (option A).**
- Pass Type scope — **resolved: Generic only in v1 (option A).**
- Design membership fields vs Apple field groups — **resolved: field groups + HBF Member Preset (option C).**
- Visual Mode free grid vs fixed zones — **resolved: fixed drop zones (option A).**
- Code Mode two-way sync — **resolved: Form/Visual/Code write one model; Live Preview follows; best-effort parse (Q29 C). Reverses old Q8 ephemeral.**
- Stale Code Mode — **superseded; Build/Regenerate re-emit; parse-failure warning.**
- Do Form edits auto-re-emit Code panes continuously, or only on Build? — **resolved: continuous debounced re-emit (Q30 A).**
- Runtime `PassGenerator.Generate()` — **resolved: never in app; templates only (Q31 A). Drop dummy cert/openssl product path.**
- CertificateService / DevCertSetup / Docker openssl for dummy.p12 — **resolved: remove from v1 (Q32 A confirmed). Dummy .pkpass ≠ visual preview; HTML Live Preview is the visual path. Future unzip-inspect = separate decision.**
- Is `dotnet-passbook` a PackageReference or docs-only? — **resolved: PackageReference; types used; Generate() never called (Q33 A).**
- Apple Code Snippet shape — **resolved: full pasteable sample including Generate() in the text (Q34 A); app never runs Generate().**
- Google JSON snippet shape — **resolved: full annotated HTTP envelope + body (Q35 A).**
- Shared understanding strong — **ADRs written:** `docs/adr/0001`–`0004`.
- **PRD v1.1 + SPEC v1.1 synced** to CONTEXT/ADRs (grill apply pass).
