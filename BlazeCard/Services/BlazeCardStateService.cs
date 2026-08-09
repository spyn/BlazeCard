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
    public Skin Skin { get; private set; } = Skin.Blaze;
    public AppearanceMode Appearance { get; private set; } = AppearanceMode.Light;
    public PreviewFocus PreviewFocus { get; private set; } = PreviewFocus.Apple;

    public string AppleCodeSnippet { get; private set; } = string.Empty;
    public string GoogleCodeSnippet { get; private set; } = string.Empty;
    public BuildResult? LastBuildResult { get; private set; }

    public bool HasCodeSyncWarning { get; private set; }
    public string? CodeSyncWarningMessage { get; private set; }

    public event Action? OnStateChanged;

    public void SetMode(AppMode mode)
    {
        if (CurrentMode == mode) return;
        CurrentMode = mode;
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

    public void SetPassbookImage(Passbook.Generator.PassbookImage image, string? dataUri)
    {
        Card.SetPassbookImage(image, dataUri);
        ScheduleEmit();
        Notify();
    }

    public void SetGoogleImage(GoogleImageSlot slot, string? dataUri)
    {
        switch (slot)
        {
            case GoogleImageSlot.Hero:
                Card.HeroImage = dataUri;
                break;
            case GoogleImageSlot.WideLogo:
                Card.WideLogoImage = dataUri;
                break;
            case GoogleImageSlot.ImageModule:
                Card.ImageModuleImage = dataUri;
                break;
        }

        ScheduleEmit();
        Notify();
    }

    public void AddField(FieldGroup group)
    {
        var list = GetFieldList(group);
        var max = group switch
        {
            FieldGroup.Header => 1,
            FieldGroup.Primary => 2,
            _ => int.MaxValue
        };
        if (list.Count >= max) return;

        list.Add(new PassField { Key = $"field{Guid.NewGuid():N}"[..12] });
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
        if (string.Equals(presetId, "sample-member", StringComparison.OrdinalIgnoreCase))
            SampleMemberPreset.Apply(Card);
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
