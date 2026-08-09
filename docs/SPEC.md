# BlazeCard — Technical Specification

> Companion to: `docs/PRD.md` (v1.1)  
> Glossary: `CONTEXT.md`  
> ADRs: `docs/adr/0001`–`0004`  
> Audience: implementing engineer  
> This document is the authoritative **implementation contract**.

---

## Authority

| Source | Role |
|---|---|
| `docs/PRD.md` + this SPEC | Behaviour / product requirements |
| This SPEC | Implementation detail; **wins over PRD** on conflict |
| `CONTEXT.md` + `docs/adr/` | Domain language and locked decisions; **win until PRD/SPEC are synced** (this revision syncs them) |
| `docs/Design/` | **Theme / visual language only** (tokens, look). Mock IA and chrome may be stale — do not implement Design features that contradict PRD/SPEC/CONTEXT (ADR 0001) |

---

## Table of Contents

1. [Solution Layout](#1-solution-layout)
2. [csproj & NuGet References](#2-csproj--nuget-references)
3. [Program.cs — DI Registration](#3-programcs--di-registration)
4. [Models](#4-models)
5. [Service Interfaces & Contracts](#5-service-interfaces--contracts)
6. [BlazeCardStateService — Behaviour](#6-blazecardstateservice--behaviour)
7. [ApplePassService](#7-applepassservice)
8. [GoogleWalletService](#8-googlewalletservice)
9. [Component Tree & Parameter Contracts](#9-component-tree--parameter-contracts)
10. [Index.razor — Workspace Shell](#10-indexrazor--workspace-shell)
11. [PreviewPanel Components](#11-previewpanel-components)
12. [FormMode Component](#12-formmode-component)
13. [CodeMode Component](#13-codemode-component)
14. [VisualMode Components](#14-visualmode-components)
15. [Shared Sub-Components](#15-shared-sub-components)
16. [JS Interop Contracts](#16-js-interop-contracts)
17. [CSS Architecture & Skins](#17-css-architecture--skins)
18. [Apple Pass HTML/CSS Render Spec](#18-apple-pass-htmlcss-render-spec)
19. [Google Wallet Card HTML/CSS Render Spec](#19-google-wallet-card-htmlcss-render-spec)
20. [Code Generation Templates](#20-code-generation-templates)
21. [Validation Rules](#21-validation-rules)
22. [Image Handling Pipeline](#22-image-handling-pipeline)
23. [Certificates — Removed](#23-certificates--removed)
24. [Configuration (appsettings.json)](#24-configuration-appsettingsjson)
25. [Dockerfile & Compose](#25-dockerfile--compose)
26. [Session Restore](#26-session-restore)
27. [Error Handling Conventions](#27-error-handling-conventions)
28. [Out of Scope (v1)](#28-out-of-scope-v1)

---

## 1. Solution Layout

```
BlazeCard.sln
│
├── BlazeCard/                          ← main Blazor Server project
│   ├── BlazeCard.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   │
│   ├── Models/
│   │   ├── CardModel.cs
│   │   ├── PassField.cs
│   │   ├── Enums.cs
│   │   ├── BuildResult.cs
│   │   ├── VisualComponent.cs
│   │   └── Google/                    ← local Google Wallet DTOs (no client NuGet)
│   │       ├── GenericObjectDto.cs
│   │       ├── LocalizedStringDto.cs
│   │       ├── ImageDto.cs
│   │       ├── TextModuleDataDto.cs
│   │       └── BarcodeDto.cs
│   │
│   ├── Services/
│   │   ├── IBlazeCardStateService.cs
│   │   ├── BlazeCardStateService.cs
│   │   ├── IApplePassService.cs
│   │   ├── ApplePassService.cs
│   │   ├── IGoogleWalletService.cs
│   │   ├── GoogleWalletService.cs
│   │   ├── ISessionRestoreService.cs
│   │   ├── SessionRestoreService.cs
│   │   ├── AppleCodeTemplate.cs
│   │   ├── GoogleCodeTemplate.cs
│   │   └── Presets/
│   │       └── HbfMemberPreset.cs
│   │
│   ├── Components/
│   │   ├── App.razor
│   │   ├── Routes.razor
│   │   ├── _Imports.razor
│   │   │
│   │   ├── Layout/
│   │   │   ├── MainLayout.razor
│   │   │   └── MainLayout.razor.css
│   │   │
│   │   ├── Pages/
│   │   │   └── Index.razor            ← Workspace Shell
│   │   │
│   │   ├── Shell/
│   │   │   ├── WorkspaceSidebar.razor
│   │   │   └── SkinAppearanceControls.razor
│   │   │
│   │   ├── Preview/
│   │   │   ├── PreviewPanel.razor
│   │   │   ├── PreviewPanel.razor.css
│   │   │   ├── AppleCardPreview.razor
│   │   │   ├── AppleCardPreview.razor.css
│   │   │   ├── GoogleCardPreview.razor
│   │   │   └── GoogleCardPreview.razor.css
│   │   │
│   │   ├── Modes/
│   │   │   ├── FormMode.razor
│   │   │   ├── FormMode.razor.css
│   │   │   ├── CodeMode.razor
│   │   │   ├── CodeMode.razor.css
│   │   │   ├── VisualMode.razor
│   │   │   └── VisualMode.razor.css
│   │   │
│   │   ├── Shared/
│   │   │   ├── ImageUploader.razor
│   │   │   ├── ImageUploader.razor.css
│   │   │   ├── ColourPicker.razor
│   │   │   ├── FieldEditor.razor
│   │   │   ├── FieldList.razor
│   │   │   ├── BarcodeConfig.razor
│   │   │   └── QrBarcodeGlyph.razor   ← real QR for Live Preview
│   │   │
│   │   └── Visual/
│   │       ├── Toolbox.razor
│   │       ├── Toolbox.razor.css
│   │       ├── ToolboxItem.razor
│   │       ├── CanvasCard.razor
│   │       ├── CanvasCard.razor.css
│   │       ├── DropZone.razor
│   │       └── ComponentProperties.razor
│   │
│   └── wwwroot/
│       ├── favicon.ico
│       ├── css/
│       │   ├── app.css
│       │   ├── apple-card.css
│       │   └── google-card.css
│       └── js/
│           ├── sortable-interop.js
│           ├── clipboard.js
│           ├── session-restore.js
│           └── qrcode-interop.js
│
├── Dockerfile
├── docker-compose.yml
└── docs/
    ├── PRD.md
    ├── SPEC.md                         ← this file
    ├── adr/
    └── Design/                         ← theme only
```

**Removed from v1 layout (do not create):**

- `Services/CertificateService.cs`
- `Resources/dummy.p12` / `dummy.key` / `dummy.crt`
- `DevCertSetup.sh` (or equivalent openssl bootstrap)

---

## 2. csproj & NuGet References

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <RootNamespace>BlazeCard</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <!-- UI -->
    <PackageReference Include="MudBlazor" Version="7.*" />

    <!-- Apple snippet fidelity: types/enums only; Generate() NEVER called in-app (ADR 0002) -->
    <PackageReference Include="dotnet-passbook" Version="3.*" />

    <!-- Monaco code editor -->
    <PackageReference Include="BlazorMonaco" Version="3.*" />

    <!-- JSON -->
    <PackageReference Include="System.Text.Json" Version="9.*" />
  </ItemGroup>
</Project>
```

**Forbidden PackageReferences (v1):**

- `Google.Apis.Walletobjects` / `Google.Apis.Walletobjects.v1`
- `Google.Apis.Auth`
- Any package whose sole purpose is Apple pass signing / cert loading

**Version pinning:** Minor floating (`7.*`) is intentional. Lock patches in `Directory.Packages.props` if reproducibility is required.

**QR library:** Prefer a small JS QR encoder in `wwwroot/js` (or a minimal NuGet used only for preview glyphs). Do not pull Google/Apple client SDKs for barcode rendering.

---

## 3. Program.cs — DI Registration

```csharp
using BlazeCard.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 4000;
    config.SnackbarConfiguration.HideTransitionDuration = 300;
    config.SnackbarConfiguration.ShowTransitionDuration = 300;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

// Scoped: one per Blazor circuit. CardModel is per-user session.
builder.Services.AddScoped<IBlazeCardStateService, BlazeCardStateService>();
builder.Services.AddScoped<IApplePassService, ApplePassService>();
builder.Services.AddScoped<IGoogleWalletService, GoogleWalletService>();
builder.Services.AddScoped<ISessionRestoreService, SessionRestoreService>();

builder.Services.Configure<BlazeCardOptions>(
    builder.Configuration.GetSection("BlazeCard"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error");

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", app = "BlazeCard" }));

app.Run();
```

**Do not register:** `CertificateService`, cert loaders, or any service that calls `PassGenerator.Generate()`.

### BlazeCardOptions

```csharp
namespace BlazeCard;

public class BlazeCardOptions
{
    public int PreviewDebounceMs { get; set; } = 100;
    public int SnippetEmitDebounceMs { get; set; } = 200;
    public int CodeParseDebounceMs { get; set; } = 500;
    public long MaxImageSizeBytes { get; set; } = 5_242_880; // 5 MB
    public string DefaultLanguage { get; set; } = "en-AU";
    public string SessionStorageKey { get; set; } = "blazecard.session.v1";
}
```

---

## 4. Models

### `Models/Enums.cs`

```csharp
namespace BlazeCard.Models;

public enum AppMode { Form, Code, Visual }

public enum PassType
{
    /// <summary>v1 supports Generic only. Other values are reserved / not offered in UI.</summary>
    Generic = 0
}

public enum BarcodeFormat
{
    None,
    QR,
    PDF417,
    Aztec,
    Code128
}

public enum TextAlignment { Left, Center, Right, Natural }

public enum ImageSlot
{
    Logo,
    Icon,
    Strip,
    Hero,
    Thumbnail
}

public enum FieldGroup
{
    Header, Primary, Secondary, Auxiliary, Back
}

public enum Skin
{
    Hbf,    // default — teal / Quokka demo brand
    Blaze   // product-neutral orange accent
}

public enum AppearanceMode
{
    Light,  // default
    Dark
}

public enum PreviewFocus
{
    Dual,   // default
    Apple,
    Google
}

public enum ToolboxItemType
{
    HeaderField,
    PrimaryField,
    SecondaryField,
    AuxiliaryField,
    BackField,
    LogoImage,
    StripImage,
    IconImage,
    HeroImage,
    ThumbnailImage,
    Barcode,
    BackgroundColor,
    LabelColor,
    ForegroundColor
}

public enum DropZoneId
{
    Header,
    Primary,
    Secondary,
    Auxiliary,
    Back,
    Strip,
    Logo,
    Barcode
}
```

**Pass Type UI:** Form/Visual expose Generic only. Do not render Coupon / Event / Store / Boarding selectors in v1.

### `Models/PassField.cs`

```csharp
namespace BlazeCard.Models;

public class PassField
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public TextAlignment TextAlignment { get; set; } = TextAlignment.Left;

    public bool HasError { get; set; }
    public string? ErrorMessage { get; set; }

    public PassField Clone() => new()
    {
        Key = Key,
        Label = Label,
        Value = Value,
        TextAlignment = TextAlignment
    };
}
```

There are **no** hard-wired Member Name / Card Number / Expiry properties. Membership-shaped labels come from the **HBF Member Preset** only.

### `Models/CardModel.cs`

```csharp
namespace BlazeCard.Models;

public class CardModel
{
    // ── Core ──────────────────────────────────────────────────
    public PassType PassType { get; set; } = PassType.Generic;
    public string Description { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public string LogoText { get; set; } = string.Empty;

    // ── Appearance (pass face colours — not Shell Skin) ───────
    public string BackgroundColor { get; set; } = "#0D7377"; // HBF-ish teal default
    public string LabelColor { get; set; } = "#FFFFFF";
    public string ForegroundColor { get; set; } = "#FFFFFF";

    // ── Fields ────────────────────────────────────────────────
    public List<PassField> HeaderFields { get; set; } = [];
    public List<PassField> PrimaryFields { get; set; } = [];
    public List<PassField> SecondaryFields { get; set; } = [];
    public List<PassField> AuxiliaryFields { get; set; } = [];
    public List<PassField> BackFields { get; set; } = [];

    // ── Images (session data URIs for Live Preview only) ──────
    public string? LogoImage { get; set; }
    public string? IconImage { get; set; }
    public string? StripImage { get; set; }
    public string? HeroImage { get; set; }
    public string? ThumbnailImage { get; set; }

    // ── Barcode ───────────────────────────────────────────────
    public BarcodeFormat BarcodeFormat { get; set; } = BarcodeFormat.QR;
    public string BarcodeMessage { get; set; } = string.Empty;
    public string BarcodeAltText { get; set; } = string.Empty;

    // ── Locale for Google localized strings ───────────────────
    public string DefaultLanguage { get; set; } = "en-AU";

    public CardModel Clone()
    {
        return new CardModel
        {
            PassType = PassType,
            Description = Description,
            OrganizationName = OrganizationName,
            LogoText = LogoText,
            BackgroundColor = BackgroundColor,
            LabelColor = LabelColor,
            ForegroundColor = ForegroundColor,
            HeaderFields = HeaderFields.Select(f => f.Clone()).ToList(),
            PrimaryFields = PrimaryFields.Select(f => f.Clone()).ToList(),
            SecondaryFields = SecondaryFields.Select(f => f.Clone()).ToList(),
            AuxiliaryFields = AuxiliaryFields.Select(f => f.Clone()).ToList(),
            BackFields = BackFields.Select(f => f.Clone()).ToList(),
            LogoImage = LogoImage,
            IconImage = IconImage,
            StripImage = StripImage,
            HeroImage = HeroImage,
            ThumbnailImage = ThumbnailImage,
            BarcodeFormat = BarcodeFormat,
            BarcodeMessage = BarcodeMessage,
            BarcodeAltText = BarcodeAltText,
            DefaultLanguage = DefaultLanguage
        };
    }
}

public static class CardModelExtensions
{
    public static IEnumerable<PassField> AllFields(this CardModel m) =>
        m.HeaderFields
         .Concat(m.PrimaryFields)
         .Concat(m.SecondaryFields)
         .Concat(m.AuxiliaryFields)
         .Concat(m.BackFields);
}
```

### `Models/BuildResult.cs`

```csharp
namespace BlazeCard.Models;

public class BuildResult
{
    public bool Success { get; init; }
    public string? AppleCodeSnippet { get; init; }
    public string? GoogleCodeSnippet { get; init; }
    public List<string> ValidationErrors { get; init; } = [];

    public static BuildResult Failure(IEnumerable<string> errors) =>
        new() { Success = false, ValidationErrors = errors.ToList() };

    public static BuildResult Ok(string appleCode, string googleCode) =>
        new() { Success = true, AppleCodeSnippet = appleCode, GoogleCodeSnippet = googleCode };
}
```

### `Models/VisualComponent.cs`

```csharp
namespace BlazeCard.Models;

public class VisualComponent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public ToolboxItemType Type { get; set; }
    public DropZoneId Zone { get; set; }
    public PassField? Field { get; set; }
    public int Order { get; set; }
}
```

### Google local DTOs (`Models/Google/`)

Minimal POCOs matching Generic Object JSON shape used by snippets/preview hydration. **Not** from `Google.Apis.Walletobjects`. Include at least:

- `GenericObjectDto` — `Id`, `ClassId`, `GenericType`, `HexBackgroundColor`, `CardTitle`, `Subheader`, `Header`, `Logo`, `HeroImage`, `TextModulesData`, `Barcode`
- `LocalizedStringDto` / `TranslatedStringDto` — `DefaultValue.Language` + `Value`
- `ImageDto` / `ImageUriDto` — `SourceUri.Uri` (placeholder HTTPS URLs in snippets)
- `TextModuleDataDto`, `BarcodeDto`

---

## 5. Service Interfaces & Contracts

### `IBlazeCardStateService`

```csharp
namespace BlazeCard.Services;

public interface IBlazeCardStateService
{
    CardModel Card { get; }
    AppMode CurrentMode { get; }
    Skin Skin { get; }
    AppearanceMode Appearance { get; }
    PreviewFocus PreviewFocus { get; }

    /// <summary>Latest emitted snippets (continuous re-emit + Build force-sync).</summary>
    string AppleCodeSnippet { get; }
    string GoogleCodeSnippet { get; }
    BuildResult? LastBuildResult { get; }

    /// <summary>True when Code pane text may not match last good emit (parse dirty / failed).</summary>
    bool HasCodeSyncWarning { get; }
    string? CodeSyncWarningMessage { get; }

    void SetMode(AppMode mode);
    void SetSkin(Skin skin);
    void SetAppearance(AppearanceMode appearance);
    void SetPreviewFocus(PreviewFocus focus);

    /// <summary>
    /// Validates and force re-syncs both Code Snippets from CardModel.
    /// Does NOT create a .pkpass. Does NOT gate Live Preview.
    /// </summary>
    Task<BuildResult> BuildAsync();

    /// <summary>Re-emit Apple snippet from model (not "Convert").</summary>
    Task<string> RegenerateAppleCodeAsync();

    /// <summary>Re-emit Google snippet from model (not "Convert").</summary>
    Task<string> RegenerateGoogleCodeAsync();

    void UpdateCard(Action<CardModel> mutate);
    void SetImage(ImageSlot slot, string? base64DataUri);

    void AddField(FieldGroup group);
    void RemoveField(FieldGroup group, Guid fieldId);
    void UpdateField(FieldGroup group, Guid fieldId, Action<PassField> mutate);
    void ReorderFields(FieldGroup group, List<Guid> orderedIds);

    /// <summary>Apply named starter config (e.g. HBF Member). Mutates CardModel fields only.</summary>
    void ApplyPreset(string presetId);

    // Visual projection (rebuilt on enter Visual Mode)
    IReadOnlyList<VisualComponent> VisualComponents { get; }
    void ProjectVisualFromCard();
    void AddVisualComponent(VisualComponent component);
    void RemoveVisualComponent(Guid id);
    void UpdateVisualComponent(Guid id, Action<VisualComponent> mutate);

    /// <summary>Best-effort parse of edited Apple C# into CardModel.</summary>
    Task ApplyAppleCodeEditAsync(string code);

    /// <summary>Best-effort parse of edited Google JSON body/envelope into CardModel.</summary>
    Task ApplyGoogleCodeEditAsync(string code);

    event Action OnStateChanged;
}
```

**Removed from interface (v1):**

- `GeneratePkpassAsync()`
- Any method returning `.pkpass` bytes
- “Convert from Apple/Google” naming — use **Regenerate**

### `IApplePassService`

```csharp
namespace BlazeCard.Services;

public interface IApplePassService
{
    /// <summary>
    /// Full pasteable C# sample (usings, PassGenerator, request, fields,
    /// cert path comments, Generate() + write-file lines for the integrator).
    /// App never executes Generate().
    /// </summary>
    string GenerateCodeSnippet(CardModel model);

    /// <summary>Best-effort parse of snippet text into a partial CardModel update.</summary>
    bool TryParseCodeSnippet(string code, CardModel target, out string? warning);
}
```

**Forbidden:** calling `new PassGenerator().Generate(...)` anywhere in the BlazeCard app.

### `IGoogleWalletService`

```csharp
namespace BlazeCard.Services;

public interface IGoogleWalletService
{
    /// <summary>Full annotated HTTP envelope + genericObject JSON body.</summary>
    string GenerateCodeSnippet(CardModel model);

    /// <summary>Local DTO for preview hydration / serialization (not Google client types).</summary>
    GenericObjectDto BuildGenericObject(CardModel model);

    bool TryParseCodeSnippet(string code, CardModel target, out string? warning);
}
```

### `ISessionRestoreService`

```csharp
namespace BlazeCard.Services;

public interface ISessionRestoreService
{
    Task SaveAsync(CardModel card, Skin skin, AppearanceMode appearance);
    Task<(CardModel? Card, Skin Skin, AppearanceMode Appearance)?> TryLoadAsync();
    string ExportJson(CardModel card, bool includeImages);
    bool TryImportJson(string json, out CardModel card, out string? error);
}
```

---

## 6. BlazeCardStateService — Behaviour

### Sole source of truth

- One in-memory `CardModel` per circuit.
- **Form**, **Visual**, and **Code** all read/write that model (ADR 0003).
- **Live Preview** always binds to `CardModel` (never to snippet text, never to a pass file).

### Continuous updates

| Trigger | Live Preview | Code Snippets |
|---|---|---|
| Form / Visual mutate model | Immediate (UI bind) | Debounced re-emit (`SnippetEmitDebounceMs`, ~200 ms) |
| Code pane edit | Follows model after successful parse | Keep editor text; do not clobber focused dirty pane |
| Parse failure | Last good model | Warning; Form remains authoritative |
| **Build** | Unchanged (already live) | Validate + **force** re-emit both panes |

Do **not** gate Live Preview on Build. Do **not** treat snippets as ephemeral one-way exports.

### Mode changes

```csharp
public void SetMode(AppMode mode)
{
    if (CurrentMode == mode) return;
    CurrentMode = mode;
    if (mode == AppMode.Visual)
        ProjectVisualFromCard(); // Visual is a projection rebuilt on enter
    Notify();
}
```

### Visual projection

- On enter Visual Mode: rebuild `VisualComponents` from `CardModel` field lists / image presence / barcode.
- Mutations on canvas write back to `CardModel` (and re-emit snippets).
- Fixed drop zones only — no free-position canvas; no Blazor.Diagrams.

### Build

```csharp
public async Task<BuildResult> BuildAsync()
{
    var errors = Validate(Card);
    if (errors.Count > 0)
    {
        LastBuildResult = BuildResult.Failure(errors);
        Notify();
        return LastBuildResult;
    }

    var apple = _apple.GenerateCodeSnippet(Card);
    var google = _google.GenerateCodeSnippet(Card);
    AppleCodeSnippet = apple;
    GoogleCodeSnippet = google;
    HasCodeSyncWarning = false;
    CodeSyncWarningMessage = null;
    LastBuildResult = BuildResult.Ok(apple, google);
    Notify();
    return LastBuildResult;
}
```

### Code parse

- Debounce `CodeParseDebounceMs` (~500 ms).
- On success: mutate `Card`, clear warning, Live Preview updates, optionally re-emit the *other* pane.
- On failure: keep last good `Card`, set `HasCodeSyncWarning`, snackbar/badge; user fixes in Form or clicks **Build** / **Regenerate**.

### Presets

`ApplyPreset("hbf-member")` fills Apple-shaped field groups with membership-style labels/values (e.g. Member Name, Membership Number, Expiry) — still plain `PassField`s. Does not introduce domain properties.

### Defaults on new session

- `PassType = Generic`
- `DefaultLanguage = en-AU`
- `Skin = Hbf`, `Appearance = Light`
- `PreviewFocus = Dual`
- Sensible empty field lists; barcode format QR with empty message until user fills

---

## 7. ApplePassService

Snippet-only service. Uses `dotnet-passbook` **types/enums in template mapping helpers** where useful (e.g. enum name strings). Does **not**:

- Instantate and run `PassGenerator.Generate()`
- Load certificates
- Produce `.pkpass` bytes
- Depend on `CertificateService`

Image lines in the snippet: for each filled slot, emit placeholder `File.ReadAllBytes("path/to/…")` assignments — **not** session base64.

See §20 for template outline.

---

## 8. GoogleWalletService

- Build `GenericObjectDto` from `CardModel` using local DTOs.
- Localized strings use `model.DefaultLanguage` (default `en-AU`) for `TranslatedString.Language`.
- Image fields in the **snippet** use placeholder HTTPS URLs (e.g. `https://example.com/images/logo.png`) when the corresponding session slot is filled — **never** data URIs / session base64.
- Wrap serialized body in full annotated HTTP envelope (`GoogleCodeTemplate`).
- No JWT, no “save URL”, no live API calls, no Google Auth.

Field mapping (guidance):

| CardModel | Google Generic Object |
|---|---|
| `OrganizationName` | `cardTitle` |
| First primary label / value | `subheader` / `header` |
| Secondary + Auxiliary + Back | `textModulesData` |
| `BackgroundColor` | `hexBackgroundColor` |
| Logo / Hero slots (filled) | `logo` / `heroImage` with placeholder HTTPS URIs in snippet |
| Barcode | `barcode` |

---

## 9. Component Tree & Parameter Contracts

All stateful components:

```csharp
@implements IDisposable
@inject IBlazeCardStateService State

@code {
    protected override void OnInitialized()
        => State.OnStateChanged += StateHasChanged;

    public void Dispose()
        => State.OnStateChanged -= StateHasChanged;
}
```

Subscribe once; never mutate `State.Card` properties without going through state helpers.

---

## 10. Index.razor — Workspace Shell

**Layout (not top-tabs-only):**

```
┌─────────────────────────────────────────────────────────────┐
│ App bar: BlazeCard + “Preview Only” chip                    │
├──────────┬────────────────────────────┬─────────────────────┤
│ Sidebar  │ Editor pane                │ Live Preview        │
│          │ Form | Code | Visual       │ Apple │ Google      │
│ • Form   │ (active mode content)      │ + Preview Focus     │
│ • Code   │                            │                     │
│ • Visual │                            │                     │
│ ───────  │                            │                     │
│ Preset   │                            │                     │
│ Skin     │                            │                     │
│ Light/   │                            │                     │
│ Dark     │                            │                     │
│ Session  │                            │                     │
│ Restore  │                            │                     │
└──────────┴────────────────────────────┴─────────────────────┘
```

- Left: `WorkspaceSidebar` drives `State.SetMode` + Skin / Appearance / Preset / Session Restore controls.
- Centre: active mode component (`FormMode` | `CodeMode` | `VisualMode`).
- Right: `PreviewPanel` (always visible on desktop).
- Mobile (`< 768px`): preview stacks below editor; sidebar may collapse to a drawer.

**Do not** use Design-mock account menus, auth chrome, or single-card-only workspace as product defaults.

Apply `data-skin` / `data-appearance` on a root element for CSS tokens (§17).

---

## 11. PreviewPanel Components

### `PreviewPanel.razor`

- Dual Apple + Google by default.
- **Preview Focus** control: Dual | Apple | Google (enlarge/zoom one side; other remains accessible — dual remains product default).
- **No** “Download .pkpass” button.
- Optional flip control on Apple preview for back fields.

### `AppleCardPreview.razor`

| Parameter | Type | Description |
|---|---|---|
| `Card` | `CardModel` | Required; one-way bind |

Pure HTML/CSS face. Parent re-renders on state change. Structure:

```html
<div class="apple-pass apple-pass--generic"
     style="background-color: {BackgroundColor}; color: {ForegroundColor}">
  <div class="apple-pass__header">…logo / logoText / header fields…</div>
  <!-- optional strip -->
  <div class="apple-pass__primary-fields">…</div>
  <div class="apple-pass__secondary-fields">…secondary + auxiliary…</div>
  <div class="apple-pass__barcode-area">…</div>
</div>
```

Back fields: separate flipped face or details panel — HTML only.

**Barcode Preview:** if `BarcodeFormat == QR` and message non-empty → render real QR via `QrBarcodeGlyph`; else placeholder glyph + message text + caption that only QR is scannable in preview.

Pass size: `320px × 202px` @1x, radius `12px` (see §18).

### `GoogleCardPreview.razor`

Same `Card` parameter. Structure:

```html
<div class="google-card" style="background-color: {BackgroundColor}">
  <div class="google-card__top-bar">…logo + org…</div>
  <!-- optional hero -->
  <div class="google-card__title-block">…subheader / header…</div>
  <!-- secondary rows -->
  <div class="google-card__barcode-strip">…</div>
</div>
```

Same QR vs placeholder barcode rules. **No** “Add to Google Wallet” / JWT link.

---

## 12. FormMode Component

No parameters; uses `IBlazeCardStateService`.

### Sections (`MudExpansionPanels`)

1. **Pass Details** — Pass Type (read-only Generic or disabled select with only Generic), Description, OrganizationName, LogoText, **Default Language** (default `en-AU`)
2. **Appearance** — Background / Label / Foreground `ColourPicker`s
3. **Images** — `ImageUploader` per slot (Logo, Icon, Strip, Hero, Thumbnail)
4. **Pass Fields** — `FieldList` for Header / Primary / Secondary / Auxiliary / Back
5. **Barcode** — `BarcodeConfig`
6. **Actions** — **Build** (validate + force snippet re-sync); **Apply HBF Member Preset**

### Binding

```razor
<MudTextField Label="Description"
              Value="@State.Card.Description"
              ValueChanged="@(v => State.UpdateCard(c => c.Description = v))"
              Immediate="true" />
```

Always `State.UpdateCard` / field helpers — never assign `State.Card.X` directly.

### Build button

Calls `await State.BuildAsync()`. Success → snackbar suggesting Code Mode if needed. Failure → snackbars per validation error. Does not download files; does not generate `.pkpass`.

---

## 13. CodeMode Component

Two Monaco panes (`BlazorMonaco` `StandaloneCodeEditor`):

| Pane | Language | Content |
|---|---|---|
| Apple | `csharp` | Full pasteable dotnet-passbook sample |
| Google | `json` or plaintext | Full HTTP envelope + body |

### Behaviour

- Primary export: **Copy** (clipboard). File download is optional/secondary — not the product primary.
- Display `State.AppleCodeSnippet` / `State.GoogleCodeSnippet`; continuous updates from model unless pane is dirty mid-edit.
- On edit: debounced `ApplyAppleCodeEditAsync` / `ApplyGoogleCodeEditAsync`.
- Parse failure → warning badge; last good model kept.
- Buttons: **Regenerate Apple**, **Regenerate Google** (labels must not say “Convert”).
- **Build** may also appear here for force re-sync after bad parse.

### Appearance of editors

Theme follows **Appearance Mode** (light/dark). Do not hard-require VS Dark only.

---

## 14. VisualMode Components

### `VisualMode.razor`

Toolbox (≈200px) + `CanvasCard` with fixed `DropZone`s.

On `OnParametersSet` / mode enter: ensure `State.ProjectVisualFromCard()` has run (state service does this in `SetMode`).

### `Toolbox.razor` / `ToolboxItem.razor`

Groups: Fields, Images, Colours, Barcodes. `draggable="true"`; payload = `ToolboxItemType`. SortableJS + native HTML5 drag.

### `DropZone.razor`

| Parameter | Type |
|---|---|
| `ZoneId` | `DropZoneId` |
| `MaxItems` | `int` |
| `Label` | `string` |
| `AcceptsTypes` | `ToolboxItemType[]` |

Max guidance: Header ≤ 1, Primary ≤ 2; Secondary/Auxiliary/Back more flexible but still zone-bound.

### `ComponentProperties.razor`

Drawer for selected component’s `PassField` / slot properties; writes via state helpers.

**Forbidden:** free-position absolute canvas, Blazor.Diagrams.

---

## 15. Shared Sub-Components

### `ImageUploader.razor`

| Parameter | Type |
|---|---|
| `Slot` | `ImageSlot` |
| `Label` | `string` |
| `RecommendedWidth` / `RecommendedHeight` | `int` |

- Accept PNG/JPEG/WebP; max 5 MB.
- Store data URI on `CardModel` for Live Preview only.
- Dimension warnings ±10% (non-blocking).
- Snippets use placeholders based on whether slot is filled — not the bytes.

### `ColourPicker.razor`

Hex `MudColorPicker`; parent wires `ValueChanged` → `State.UpdateCard`.

### `FieldList.razor` / `FieldEditor.razor`

Pass Field groups only. Key / Label / Value / TextAlignment. Add/Remove/Reorder via state.

### `BarcodeConfig.razor`

Format (including None), Message, AltText.

### `QrBarcodeGlyph.razor`

Renders scannable QR for preview when format is QR; otherwise parent shows placeholder.

---

## 16. JS Interop Contracts

### `wwwroot/js/clipboard.js`

```javascript
window.blazeCard = window.blazeCard || {};

window.blazeCard.copyToClipboard = async (text) => {
    try {
        await navigator.clipboard.writeText(text);
        return true;
    } catch {
        const el = document.createElement('textarea');
        el.value = text;
        document.body.appendChild(el);
        el.select();
        document.execCommand('copy');
        document.body.removeChild(el);
        return true;
    }
};

// Optional secondary export for .cs / .json text — NOT for .pkpass
window.blazeCard.downloadTextFile = (filename, mimeType, text) => {
    const blob = new Blob([text], { type: mimeType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.click();
    URL.revokeObjectURL(url);
};
```

**Removed:** `blazeCard.downloadFile` for `application/vnd.apple.pkpass`.

### `wwwroot/js/sortable-interop.js`

Keep SortableJS init / drop-zone / toolbox drag payload contracts from prior SPEC:

- `initSortable(containerId, dotnetRef)` → `OnSortEnd`
- `initDropZone(zoneId, dotnetRef)` → `OnToolboxDrop`
- `setDragData(itemType)`

### `wwwroot/js/session-restore.js`

```javascript
window.blazeCard.session = {
    save: (key, json) => localStorage.setItem(key, json),
    load: (key) => localStorage.getItem(key),
    clear: (key) => localStorage.removeItem(key)
};
```

### Image dimensions

```javascript
window.blazeCard.getImageDimensions = (dataUri) => new Promise((resolve) => {
    const img = new Image();
    img.onload = () => resolve({ width: img.naturalWidth, height: img.naturalHeight });
    img.src = dataUri;
});
```

### QR

`qrcode-interop.js` exposes `blazeCard.renderQr(elementId, message)` (or returns data URL) for Live Preview only.

---

## 17. CSS Architecture & Skins

### Strategy

- Component-scoped `.razor.css` for layout.
- Global `app.css` for tokens, Workspace Shell grid, utilities.
- Global `apple-card.css` / `google-card.css` for pass faces (not Blazor-scoped).
- MudBlazor theme bridges to CSS variables; update on Skin × Appearance Mode.

### Axes (independent)

| Axis | Values | Default |
|---|---|---|
| **Skin** | HBF \| Blaze | HBF |
| **Appearance Mode** | Light \| Dark | Light |
| **Preset** | e.g. HBF Member | none (empty / last session) |

Skin ≠ Preset ≠ Appearance Mode.

### Tokens (`wwwroot/css/app.css`)

```css
:root,
[data-skin="hbf"][data-appearance="light"] {
    --bc-brand-primary:   #0D7377;   /* HBF teal */
    --bc-brand-accent:    #14919B;
    --bc-brand-dark:      #0A4F52;
    --bc-surface:         #F7FAFA;
    --bc-surface-raised:  #FFFFFF;
    --bc-border:          #D0DEDE;
    --bc-text-primary:    #1A2B2B;
    --bc-text-secondary:  #5A6F6F;
    --bc-editor-bg:       #FFFFFF;
    --bc-preview-width:   380px;
    --bc-sidebar-width:   220px;
    --bc-editor-min-width: 360px;
}

[data-skin="hbf"][data-appearance="dark"] {
    --bc-brand-primary:   #14919B;
    --bc-brand-accent:    #0D7377;
    --bc-surface:         #121A1A;
    --bc-surface-raised:  #1A2424;
    --bc-border:          #2A3A3A;
    --bc-text-primary:    #F0F7F7;
    --bc-text-secondary:  #9BB0B0;
    --bc-editor-bg:       #0E1515;
}

[data-skin="blaze"][data-appearance="light"] {
    --bc-brand-primary:   #FF4500;   /* Blaze orange */
    --bc-brand-accent:    #FF6A33;
    --bc-brand-dark:      #1A1A1A;
    --bc-surface:         #FAFAFA;
    --bc-surface-raised:  #FFFFFF;
    --bc-border:          #E0E0E0;
    --bc-text-primary:    #1A1A1A;
    --bc-text-secondary:  #666666;
    --bc-editor-bg:       #FFFFFF;
}

[data-skin="blaze"][data-appearance="dark"] {
    --bc-brand-primary:   #FF4500;
    --bc-brand-accent:    #FF6A33;
    --bc-surface:         #1A1A1A;
    --bc-surface-raised:  #242424;
    --bc-border:          #3A3A3A;
    --bc-text-primary:    #F5F5F5;
    --bc-text-secondary:  #A0A0A0;
    --bc-editor-bg:       #1E1E1E;
}

.blaze-shell {
    display: grid;
    grid-template-columns: var(--bc-sidebar-width) 1fr var(--bc-preview-width);
    height: calc(100vh - 64px);
    overflow: hidden;
}

.blaze-shell__sidebar {
    overflow-y: auto;
    border-right: 1px solid var(--bc-border);
    background: var(--bc-surface-raised);
}

.blaze-shell__editor {
    overflow-y: auto;
    padding: 16px;
    background: var(--bc-surface);
}

.blaze-shell__preview {
    overflow-y: auto;
    padding: 16px;
    border-left: 1px solid var(--bc-border);
    background: var(--bc-surface);
}

@media (max-width: 768px) {
    .blaze-shell {
        grid-template-columns: 1fr;
        grid-template-rows: auto auto auto;
        height: auto;
    }
}
```

Design docs under `docs/Design/` may refine token values; behaviour/IA still come from this SPEC.

---

## 18. Apple Pass HTML/CSS Render Spec

Canonical 1x pass size: **320×202px**, corner radius **12px**.

```css
/* wwwroot/css/apple-card.css */

.apple-pass {
    width: 320px;
    min-height: 202px;
    border-radius: 12px;
    overflow: hidden;
    font-family: -apple-system, "SF Pro Display", "Helvetica Neue", sans-serif;
    box-shadow: 0 8px 32px rgba(0, 0, 0, 0.4);
    position: relative;
    display: flex;
    flex-direction: column;
}

.apple-pass__header {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    padding: 12px 16px 8px;
}

.apple-pass__logo-area {
    display: flex;
    align-items: center;
    gap: 8px;
    flex: 1;
}

.apple-pass__logo-img {
    max-height: 25px;
    max-width: 80px;
    object-fit: contain;
}

.apple-pass__logo-text {
    font-size: 14px;
    font-weight: 600;
    letter-spacing: -0.2px;
}

.apple-pass__header-field {
    text-align: right;
}

.apple-pass__strip {
    width: 100%;
    height: 123px;
    overflow: hidden;
}
.apple-pass__strip-img {
    width: 100%;
    height: 100%;
    object-fit: cover;
}

.apple-pass__primary-fields {
    display: flex;
    gap: 16px;
    padding: 10px 16px 6px;
}

.apple-pass__field-label {
    font-size: 9px;
    font-weight: 500;
    letter-spacing: 0.6px;
    text-transform: uppercase;
    opacity: 0.85;
}

.apple-pass__field-value {
    font-size: 13px;
    font-weight: 500;
    margin-top: 2px;
}

.apple-pass__field-value--primary {
    font-size: 22px;
    font-weight: 300;
}

.apple-pass__secondary-fields {
    display: flex;
    flex-wrap: wrap;
    gap: 8px 16px;
    padding: 6px 16px;
}

.apple-pass__secondary-field {
    flex: 1 1 80px;
    min-width: 60px;
}

.apple-pass__barcode-area {
    display: flex;
    justify-content: center;
    align-items: center;
    padding: 10px;
    background: rgba(255, 255, 255, 0.12);
    margin: 8px 16px 12px;
    border-radius: 8px;
}

.apple-pass__barcode-placeholder {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 4px;
}

.apple-pass__barcode-icon {
    font-size: 40px;
    opacity: 0.6;
}

.apple-pass__barcode-text {
    font-size: 10px;
    font-family: monospace;
    opacity: 0.7;
}
```

---

## 19. Google Wallet Card HTML/CSS Render Spec

```css
/* wwwroot/css/google-card.css */

.google-card {
    width: 320px;
    border-radius: 16px;
    overflow: hidden;
    font-family: "Google Sans", Roboto, sans-serif;
    box-shadow: 0 8px 32px rgba(0, 0, 0, 0.4);
    display: flex;
    flex-direction: column;
}

.google-card__top-bar {
    display: flex;
    align-items: center;
    padding: 12px 16px;
    gap: 10px;
    background: rgba(0, 0, 0, 0.15);
}

.google-card__logo {
    height: 24px;
    width: auto;
    object-fit: contain;
}

.google-card__org-name {
    font-size: 14px;
    font-weight: 500;
    color: #fff;
    opacity: 0.9;
}

.google-card__hero {
    width: 100%;
    height: 100px;
    overflow: hidden;
}

.google-card__hero-img {
    width: 100%;
    height: 100%;
    object-fit: cover;
}

.google-card__title-block {
    padding: 14px 16px 8px;
}

.google-card__subheader {
    font-size: 11px;
    font-weight: 500;
    letter-spacing: 0.8px;
    text-transform: uppercase;
    opacity: 0.75;
}

.google-card__header {
    font-size: 24px;
    font-weight: 400;
    margin-top: 2px;
}

.google-card__row {
    display: flex;
    justify-content: space-between;
    padding: 4px 16px;
    font-size: 13px;
}

.google-card__row-label {
    opacity: 0.7;
}

.google-card__barcode-strip {
    display: flex;
    flex-direction: column;
    align-items: center;
    background: rgba(255,255,255,0.15);
    padding: 12px;
    margin: 10px 16px 14px;
    border-radius: 10px;
    gap: 4px;
}

.google-card__barcode-placeholder {
    font-size: 36px;
    opacity: 0.5;
}

.google-card__barcode-value {
    font-size: 11px;
    font-family: monospace;
    opacity: 0.65;
}
```

---

## 20. Code Generation Templates

### Apple (`AppleCodeTemplate`)

Emit a **full pasteable sample**, including:

1. Header comment: Generated by BlazeCard; link to dotnet-passbook
2. Usings (`Passbook.Generator`, Fields, `System.Security.Cryptography.X509Certificates`, etc.)
3. `var passGenerator = new PassGenerator();`
4. `PassGeneratorRequest` with placeholders for Pass Type ID / Team ID; v1 `PassType = PKPassType.PKGenericPass`
5. Colours as `rgb(...)`
6. `AddHeaderField` / `AddPrimaryField` / … for each field
7. Barcode block when enabled
8. For each filled image slot: commented/`File.ReadAllBytes("path/to/logo@2x.png")`-style lines — not session bytes
9. Cert path comments for the **integrator**
10. **`byte[] pkpassBytes = passGenerator.Generate(request);`** and `File.WriteAllBytes(...)` — present in the **text** only; BlazeCard never executes these

Escape user strings for C# literals.

### Google (`GoogleCodeTemplate`)

Emit:

```
// Generated by BlazeCard — Google Wallet Generic Objects API
// prerequisites + issuer notes…

POST https://walletobjects.googleapis.com/walletobjects/v1/genericObject
Authorization: Bearer {ACCESS_TOKEN}
Content-Type: application/json

{prettyPrintedGenericObjectBody}

// How to obtain ACCESS_TOKEN — comments only; no Google.Apis.Auth dependency in this app
```

Body IDs use `{ISSUER_ID}` placeholders. Image URIs are HTTPS placeholders when slots are filled.

---

## 21. Validation Rules

Enforced in `BlazeCardStateService.Validate()` and mirrored as Form inline validation. Used by **Build**; Live Preview may still show incomplete data.

| Field | Rule | Error Message |
|---|---|---|
| `Description` | Required, max 255 | "Description is required." |
| `OrganizationName` | Required, max 255 | "Organisation Name is required." |
| `PassType` | Must be Generic in v1 | "Only Generic pass type is supported in v1." |
| `HeaderFields.Count` | ≤ 1 | "Apple Wallet allows at most 1 header field." |
| `PrimaryFields.Count` | ≤ 2 | "Apple Wallet allows at most 2 primary fields." |
| `PassField.Key` | Required; no spaces; ASCII | Missing key / "Keys must not contain spaces." |
| `BarcodeMessage` | Required if format ≠ None | "Barcode message is required when a barcode format is selected." |
| Colours | `#RRGGBB` | "… must be a valid hex colour." |
| `DefaultLanguage` | Non-empty BCP-47-ish tag | "Default language is required." |

**Image dimension warnings** (non-blocking):

| Slot | Expected @2x | Tolerance |
|---|---|---|
| Logo | 160×50 | ±10% |
| Icon | 58×58 | ±10% |
| Strip | 640×246 | ±10% |
| Hero | 1125×432 | ±10% |
| Thumbnail | 660×660 | ±10% |

---

## 22. Image Handling Pipeline

```
User selects file
        │
        ▼
IBrowserFile validation (image/*, ≤ 5 MB)
        │
        ▼
Data URI on CardModel slot → Live Preview
        │
        ▼
JS dimension check (warning only)
        │
        ▼
Snippet emit:
  Apple  → File.ReadAllBytes placeholder lines for filled slots
  Google → https://example.com/... placeholder URIs for filled slots
        │
        ▼
Session Restore auto-save: OMIT image data URIs
Explicit JSON export: may include images if user opts in
```

Uploads never become Google snippet data URIs. Uploads never trigger `.pkpass` packaging.

---

## 23. Certificates — Removed

**v1 does not include certificate bootstrapping.** Per ADR 0002 / CONTEXT:

| Removed | Notes |
|---|---|
| `CertificateService` | Do not implement |
| `dummy.p12` / embedded cert resources | Do not generate or ship |
| `DevCertSetup.sh` / local openssl scripts | Do not require |
| Docker `openssl` RUN steps | Do not include |
| `GeneratePkpassAsync` / `.pkpass` download | Do not include |
| Runtime `PassGenerator.Generate()` | Never call |

Real signing belongs in the integrator’s backend, documented as comments inside the Apple Code Snippet.

---

## 24. Configuration (appsettings.json)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "BlazeCard": {
    "PreviewDebounceMs": 100,
    "SnippetEmitDebounceMs": 200,
    "CodeParseDebounceMs": 500,
    "MaxImageSizeBytes": 5242880,
    "DefaultLanguage": "en-AU",
    "SessionStorageKey": "blazecard.session.v1"
  }
}
```

`appsettings.Development.json` may raise `BlazeCard` log level to Debug.

**Removed settings:** `DummyCertPath`, `DummyCertPassword`, and any cert-related env vars.

---

## 25. Dockerfile & Compose

### `Dockerfile`

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY BlazeCard.sln .
COPY BlazeCard/BlazeCard.csproj BlazeCard/
RUN dotnet restore

COPY . .
RUN dotnet publish BlazeCard/BlazeCard.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "BlazeCard.dll"]
```

**No** `apt-get install openssl`, **no** pkcs12 generation.

### `docker-compose.yml`

```yaml
services:
  blazecard:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "8080:8080"
    environment:
      ASPNETCORE_ENVIRONMENT: Development
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 5s
      retries: 3
      start_period: 10s

  nginx:
    image: nginx:alpine
    profiles: ["prod"]
    ports:
      - "80:80"
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf:ro
    depends_on:
      - blazecard
```

Core preview/snippets must work offline (no required external network).

---

## 26. Session Restore

- Auto-save non-image configuration (+ Skin / Appearance) to `localStorage` via JS interop; debounce writes.
- On circuit start: try load; re-hydrate `CardModel` without images (user re-uploads logos).
- Optional explicit **Export JSON** / **Import JSON** (export may include images when opted in).
- Not accounts, not cloud sync, not a server DB, not a named “Workspace” product entity.
- Workspace Shell = chrome only.

---

## 27. Error Handling Conventions

### Service layer

```csharp
public class BlazeCardException(string userMessage, Exception? inner = null)
    : Exception(userMessage, inner)
{
    public string UserMessage { get; } = userMessage;
}

public class CodeParseException(string userMessage, Exception? inner = null)
    : BlazeCardException(userMessage, inner);
```

**Removed:** `PassGenerationException`, `CertificateException` as product paths.

- Do not catch `OperationCanceledException` in services.
- Parse failures are soft (warning + last good model), not hard crashes.

### Component layer

try/catch around Build / import; MudBlazor snackbars for `BlazeCardException.UserMessage`; log unexpected errors.

### Global

`App.razor` `<ErrorBoundary>` with Retry — same pattern as prior SPEC.

---

## 28. Out of Scope (v1)

- Real Apple certificates, `.pkpass` generation/download, Wallet install
- Dummy cert / openssl / `CertificateService` / DevCertSetup
- Live Google Wallet API, JWT save links, Google sign-in, `Google.Apis.*` client packages
- Pass types other than Generic
- Free-position Visual canvas; Blazor.Diagrams
- Auth / accounts / multi-user / server template library
- NFC / push updates / multi-language value maps
- Analytics
- Treating Design mocks as behavioural requirements

---

## Non-Functional Targets

| Requirement | Target |
|---|---|
| Preview update latency | < 200 ms after field change (debounce ~100 ms) |
| Snippet re-emit | Debounced ~100–300 ms; don’t clobber focused dirty parse |
| Image upload size | 5 MB per image |
| Browsers | Chrome 120+, Firefox 120+, Edge 120+, Safari 17+ |
| Mobile | Preview stacks below editor &lt; 768 px |
| Offline | No required external network for core preview/snippets |
| Persistence | Session Restore only; no server DB |
| Auth | None |

---

*BlazeCard SPEC v1.1 — aligned with PRD v1.1, CONTEXT.md, and ADRs 0001–0004*
