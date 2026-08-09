using System.Text.RegularExpressions;
using BlazeCard.Models;
using BlazeCard.Services.Presets;
using Microsoft.Extensions.Options;

namespace BlazeCard.Services;

public class BlazeCardStateService : IBlazeCardStateService
{
    private readonly IApplePassService _apple;
    private readonly IGoogleWalletService _google;
    private readonly BlazeCardOptions _options;
    private readonly List<VisualComponent> _visualComponents = [];
    private CancellationTokenSource? _emitCts;
    private CancellationTokenSource? _parseCts;

    public BlazeCardStateService(
        IApplePassService apple,
        IGoogleWalletService google,
        IOptions<BlazeCardOptions> options)
    {
        _apple = apple;
        _google = google;
        _options = options.Value;
        Card = new CardModel { DefaultLanguage = _options.DefaultLanguage };
        EmitSnippets();
    }

    public CardModel Card { get; private set; }
    public AppMode CurrentMode { get; private set; } = AppMode.Form;
    public Skin Skin { get; private set; } = Skin.Hbf;
    public AppearanceMode Appearance { get; private set; } = AppearanceMode.Light;
    public PreviewFocus PreviewFocus { get; private set; } = PreviewFocus.Dual;

    public string AppleCodeSnippet { get; private set; } = string.Empty;
    public string GoogleCodeSnippet { get; private set; } = string.Empty;
    public BuildResult? LastBuildResult { get; private set; }

    public bool HasCodeSyncWarning { get; private set; }
    public string? CodeSyncWarningMessage { get; private set; }

    public IReadOnlyList<VisualComponent> VisualComponents => _visualComponents;

    public event Action? OnStateChanged;

    public void SetMode(AppMode mode)
    {
        if (CurrentMode == mode) return;
        CurrentMode = mode;
        if (mode == AppMode.Visual)
            ProjectVisualFromCard();
        Notify();
    }

    public void SetSkin(Skin skin)
    {
        Skin = skin;
        Notify();
    }

    public void SetAppearance(AppearanceMode appearance)
    {
        Appearance = appearance;
        Notify();
    }

    public void SetPreviewFocus(PreviewFocus focus)
    {
        PreviewFocus = focus;
        Notify();
    }

    public Task<BuildResult> BuildAsync()
    {
        var errors = Validate(Card);
        if (errors.Count > 0)
        {
            LastBuildResult = BuildResult.Failure(errors);
            Notify();
            return Task.FromResult(LastBuildResult);
        }

        EmitSnippets();
        HasCodeSyncWarning = false;
        CodeSyncWarningMessage = null;
        LastBuildResult = BuildResult.Ok(AppleCodeSnippet, GoogleCodeSnippet);
        Notify();
        return Task.FromResult(LastBuildResult);
    }

    public Task<string> RegenerateAppleCodeAsync()
    {
        AppleCodeSnippet = _apple.GenerateCodeSnippet(Card);
        HasCodeSyncWarning = false;
        CodeSyncWarningMessage = null;
        Notify();
        return Task.FromResult(AppleCodeSnippet);
    }

    public Task<string> RegenerateGoogleCodeAsync()
    {
        GoogleCodeSnippet = _google.GenerateCodeSnippet(Card);
        HasCodeSyncWarning = false;
        CodeSyncWarningMessage = null;
        Notify();
        return Task.FromResult(GoogleCodeSnippet);
    }

    public void UpdateCard(Action<CardModel> mutate)
    {
        mutate(Card);
        ScheduleEmit();
        Notify();
    }

    public void SetImage(ImageSlot slot, string? base64DataUri)
    {
        switch (slot)
        {
            case ImageSlot.Logo: Card.LogoImage = base64DataUri; break;
            case ImageSlot.Icon: Card.IconImage = base64DataUri; break;
            case ImageSlot.Strip: Card.StripImage = base64DataUri; break;
            case ImageSlot.Hero: Card.HeroImage = base64DataUri; break;
            case ImageSlot.Thumbnail: Card.ThumbnailImage = base64DataUri; break;
        }
        ScheduleEmit();
        Notify();
    }

    public void AddField(FieldGroup group)
    {
        GetFieldList(group).Add(new PassField { Key = $"field{Guid.NewGuid():N}"[..12] });
        ScheduleEmit();
        Notify();
    }

    public void RemoveField(FieldGroup group, Guid fieldId)
    {
        var list = GetFieldList(group);
        list.RemoveAll(f => f.Id == fieldId);
        ScheduleEmit();
        Notify();
    }

    public void UpdateField(FieldGroup group, Guid fieldId, Action<PassField> mutate)
    {
        var field = GetFieldList(group).FirstOrDefault(f => f.Id == fieldId);
        if (field is null) return;
        mutate(field);
        ScheduleEmit();
        Notify();
    }

    public void ReorderFields(FieldGroup group, List<Guid> orderedIds)
    {
        var list = GetFieldList(group);
        var map = list.ToDictionary(f => f.Id);
        list.Clear();
        foreach (var id in orderedIds)
        {
            if (map.TryGetValue(id, out var field))
                list.Add(field);
        }
        ScheduleEmit();
        Notify();
    }

    public void ApplyPreset(string presetId)
    {
        if (string.Equals(presetId, "hbf-member", StringComparison.OrdinalIgnoreCase))
            HbfMemberPreset.Apply(Card);
        ScheduleEmit();
        Notify();
    }

    public void ProjectVisualFromCard()
    {
        _visualComponents.Clear();
        var order = 0;

        void AddFields(IEnumerable<PassField> fields, ToolboxItemType type, DropZoneId zone)
        {
            foreach (var f in fields)
            {
                _visualComponents.Add(new VisualComponent
                {
                    Type = type,
                    Zone = zone,
                    Field = f,
                    Order = order++
                });
            }
        }

        AddFields(Card.HeaderFields, ToolboxItemType.HeaderField, DropZoneId.Header);
        AddFields(Card.PrimaryFields, ToolboxItemType.PrimaryField, DropZoneId.Primary);
        AddFields(Card.SecondaryFields, ToolboxItemType.SecondaryField, DropZoneId.Secondary);
        AddFields(Card.AuxiliaryFields, ToolboxItemType.AuxiliaryField, DropZoneId.Auxiliary);
        AddFields(Card.BackFields, ToolboxItemType.BackField, DropZoneId.Back);

        if (!string.IsNullOrEmpty(Card.LogoImage))
            _visualComponents.Add(new VisualComponent { Type = ToolboxItemType.LogoImage, Zone = DropZoneId.Logo, Order = order++ });
        if (!string.IsNullOrEmpty(Card.StripImage))
            _visualComponents.Add(new VisualComponent { Type = ToolboxItemType.StripImage, Zone = DropZoneId.Strip, Order = order++ });
        if (Card.BarcodeFormat != BarcodeFormat.None)
            _visualComponents.Add(new VisualComponent { Type = ToolboxItemType.Barcode, Zone = DropZoneId.Barcode, Order = order++ });
    }

    public void AddVisualComponent(VisualComponent component)
    {
        switch (component.Type)
        {
            case ToolboxItemType.HeaderField:
                EnsureFieldCapacity(Card.HeaderFields, 1);
                var hf = component.Field ?? new PassField { Key = "header" };
                Card.HeaderFields.Add(hf);
                component.Field = hf;
                component.Zone = DropZoneId.Header;
                break;
            case ToolboxItemType.PrimaryField:
                EnsureFieldCapacity(Card.PrimaryFields, 2);
                var pf = component.Field ?? new PassField { Key = "primary" };
                Card.PrimaryFields.Add(pf);
                component.Field = pf;
                component.Zone = DropZoneId.Primary;
                break;
            case ToolboxItemType.SecondaryField:
                var sf = component.Field ?? new PassField { Key = "secondary" };
                Card.SecondaryFields.Add(sf);
                component.Field = sf;
                component.Zone = DropZoneId.Secondary;
                break;
            case ToolboxItemType.AuxiliaryField:
                var af = component.Field ?? new PassField { Key = "auxiliary" };
                Card.AuxiliaryFields.Add(af);
                component.Field = af;
                component.Zone = DropZoneId.Auxiliary;
                break;
            case ToolboxItemType.BackField:
                var bf = component.Field ?? new PassField { Key = "back" };
                Card.BackFields.Add(bf);
                component.Field = bf;
                component.Zone = DropZoneId.Back;
                break;
            case ToolboxItemType.Barcode:
                if (Card.BarcodeFormat == BarcodeFormat.None)
                    Card.BarcodeFormat = BarcodeFormat.QR;
                component.Zone = DropZoneId.Barcode;
                break;
            case ToolboxItemType.LogoImage:
                component.Zone = DropZoneId.Logo;
                break;
            case ToolboxItemType.StripImage:
                component.Zone = DropZoneId.Strip;
                break;
        }

        component.Order = _visualComponents.Count;
        _visualComponents.Add(component);
        ScheduleEmit();
        Notify();
    }

    public void RemoveVisualComponent(Guid id)
    {
        var component = _visualComponents.FirstOrDefault(c => c.Id == id);
        if (component is null) return;

        if (component.Field is not null)
        {
            Card.HeaderFields.RemoveAll(f => f.Id == component.Field.Id);
            Card.PrimaryFields.RemoveAll(f => f.Id == component.Field.Id);
            Card.SecondaryFields.RemoveAll(f => f.Id == component.Field.Id);
            Card.AuxiliaryFields.RemoveAll(f => f.Id == component.Field.Id);
            Card.BackFields.RemoveAll(f => f.Id == component.Field.Id);
        }

        if (component.Type == ToolboxItemType.Barcode)
            Card.BarcodeFormat = BarcodeFormat.None;

        _visualComponents.Remove(component);
        ScheduleEmit();
        Notify();
    }

    public void UpdateVisualComponent(Guid id, Action<VisualComponent> mutate)
    {
        var component = _visualComponents.FirstOrDefault(c => c.Id == id);
        if (component is null) return;
        mutate(component);
        ScheduleEmit();
        Notify();
    }

    public async Task ApplyAppleCodeEditAsync(string code)
    {
        await DebounceParseAsync(async () =>
        {
            var working = Card.Clone();
            if (_apple.TryParseCodeSnippet(code, working, out var warning))
            {
                Card = working;
                HasCodeSyncWarning = false;
                CodeSyncWarningMessage = null;
                GoogleCodeSnippet = _google.GenerateCodeSnippet(Card);
            }
            else
            {
                HasCodeSyncWarning = true;
                CodeSyncWarningMessage = warning ?? "Apple code parse failed.";
            }
            Notify();
            await Task.CompletedTask;
        });
    }

    public async Task ApplyGoogleCodeEditAsync(string code)
    {
        await DebounceParseAsync(async () =>
        {
            var working = Card.Clone();
            if (_google.TryParseCodeSnippet(code, working, out var warning))
            {
                Card = working;
                HasCodeSyncWarning = false;
                CodeSyncWarningMessage = null;
                AppleCodeSnippet = _apple.GenerateCodeSnippet(Card);
            }
            else
            {
                HasCodeSyncWarning = true;
                CodeSyncWarningMessage = warning ?? "Google code parse failed.";
            }
            Notify();
            await Task.CompletedTask;
        });
    }

    private async Task DebounceParseAsync(Func<Task> action)
    {
        _parseCts?.Cancel();
        _parseCts = new CancellationTokenSource();
        var token = _parseCts.Token;
        try
        {
            if (_options.CodeParseDebounceMs > 0)
                await Task.Delay(_options.CodeParseDebounceMs, token);
            if (!token.IsCancellationRequested)
                await action();
        }
        catch (TaskCanceledException)
        {
            // superseded by a newer edit
        }
    }

    private void ScheduleEmit()
    {
        if (_options.SnippetEmitDebounceMs <= 0)
        {
            EmitSnippets();
            return;
        }

        _emitCts?.Cancel();
        _emitCts = new CancellationTokenSource();
        var token = _emitCts.Token;
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(_options.SnippetEmitDebounceMs, token);
                if (token.IsCancellationRequested) return;
                EmitSnippets();
                Notify();
            }
            catch (TaskCanceledException)
            {
            }
        }, token);
    }

    private void EmitSnippets()
    {
        AppleCodeSnippet = _apple.GenerateCodeSnippet(Card);
        GoogleCodeSnippet = _google.GenerateCodeSnippet(Card);
    }

    private List<PassField> GetFieldList(FieldGroup group) => group switch
    {
        FieldGroup.Header => Card.HeaderFields,
        FieldGroup.Primary => Card.PrimaryFields,
        FieldGroup.Secondary => Card.SecondaryFields,
        FieldGroup.Auxiliary => Card.AuxiliaryFields,
        FieldGroup.Back => Card.BackFields,
        _ => Card.SecondaryFields
    };

    private static void EnsureFieldCapacity(List<PassField> list, int max)
    {
        while (list.Count >= max && list.Count > 0)
            list.RemoveAt(list.Count - 1);
    }


    internal static List<string> Validate(CardModel card)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(card.Description))
            errors.Add("Description is required.");
        else if (card.Description.Length > 255)
            errors.Add("Description is required.");

        if (string.IsNullOrWhiteSpace(card.OrganizationName))
            errors.Add("Organisation Name is required.");
        else if (card.OrganizationName.Length > 255)
            errors.Add("Organisation Name is required.");

        if (card.PassType != PassType.Generic)
            errors.Add("Only Generic pass type is supported in v1.");

        if (card.HeaderFields.Count > 1)
            errors.Add("Apple Wallet allows at most 1 header field.");

        if (card.PrimaryFields.Count > 2)
            errors.Add("Apple Wallet allows at most 2 primary fields.");

        foreach (var field in card.AllFields())
        {
            if (string.IsNullOrWhiteSpace(field.Key))
                errors.Add("Field key is required.");
            else if (field.Key.Contains(' ', StringComparison.Ordinal))
                errors.Add("Keys must not contain spaces.");
            else if (!Regex.IsMatch(field.Key, @"^[\x20-\x7E]+$") || field.Key.Any(c => c > 127))
            {
                // ASCII printable without relying on spaces (already checked)
            }
            if (!string.IsNullOrEmpty(field.Key) && field.Key.Any(c => c > 127))
                errors.Add("Keys must be ASCII.");
        }

        if (card.BarcodeFormat != BarcodeFormat.None && string.IsNullOrWhiteSpace(card.BarcodeMessage))
            errors.Add("Barcode message is required when a barcode format is selected.");

        ValidateColour(card.BackgroundColor, "Background colour", errors);
        ValidateColour(card.LabelColor, "Label colour", errors);
        ValidateColour(card.ForegroundColor, "Foreground colour", errors);

        if (string.IsNullOrWhiteSpace(card.DefaultLanguage))
            errors.Add("Default language is required.");

        return errors;
    }

    private static void ValidateColour(string colour, string name, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(colour) || !Regex.IsMatch(colour, @"^#[0-9A-Fa-f]{6}$"))
            errors.Add($"{name} must be a valid hex colour.");
    }

    private void Notify() => OnStateChanged?.Invoke();
}
