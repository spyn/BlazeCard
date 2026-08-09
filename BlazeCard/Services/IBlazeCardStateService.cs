using BlazeCard.Models;

namespace BlazeCard.Services;

public interface IBlazeCardStateService
{
    CardModel Card { get; }
    AppMode CurrentMode { get; }
    Skin Skin { get; }
    AppearanceMode Appearance { get; }
    PreviewFocus PreviewFocus { get; }

    string AppleCodeSnippet { get; }
    string GoogleCodeSnippet { get; }
    BuildResult? LastBuildResult { get; }

    bool HasCodeSyncWarning { get; }
    string? CodeSyncWarningMessage { get; }

    void SetMode(AppMode mode);
    void SetSkin(Skin skin);
    void SetAppearance(AppearanceMode appearance);
    void SetPreviewFocus(PreviewFocus focus);

    Task<BuildResult> BuildAsync();
    Task<string> RegenerateAppleCodeAsync();
    Task<string> RegenerateGoogleCodeAsync();

    void UpdateCard(Action<CardModel> mutate);
    void SetImage(ImageSlot slot, string? base64DataUri);

    void AddField(FieldGroup group);
    void RemoveField(FieldGroup group, Guid fieldId);
    void UpdateField(FieldGroup group, Guid fieldId, Action<PassField> mutate);
    void ReorderFields(FieldGroup group, List<Guid> orderedIds);

    void ApplyPreset(string presetId);

    IReadOnlyList<VisualComponent> VisualComponents { get; }
    void ProjectVisualFromCard();
    void AddVisualComponent(VisualComponent component);
    void RemoveVisualComponent(Guid id);
    void UpdateVisualComponent(Guid id, Action<VisualComponent> mutate);

    Task ApplyAppleCodeEditAsync(string code);
    Task ApplyGoogleCodeEditAsync(string code);

    event Action OnStateChanged;
}
