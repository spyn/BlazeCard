using System.Text.Json;
using System.Text.Json.Serialization;
using BlazeCard.Models;
using Microsoft.Extensions.Options;

namespace BlazeCard.Services;

public class SessionRestoreService : ISessionRestoreService
{
    private readonly ILocalStorage _storage;
    private readonly BlazeCardOptions _options;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public SessionRestoreService(ILocalStorage storage, IOptions<BlazeCardOptions> options)
    {
        _storage = storage;
        _options = options.Value;
    }

    public async Task SaveAsync(CardModel card, Skin skin, AppearanceMode appearance)
    {
        var payload = new SessionPayload
        {
            Card = StripImages(card.Clone()),
            Skin = skin,
            Appearance = appearance
        };
        var json = JsonSerializer.Serialize(payload, JsonOptions);
        await _storage.SetItemAsync(_options.SessionStorageKey, json);
    }

    public async Task<(CardModel? Card, Skin Skin, AppearanceMode Appearance)?> TryLoadAsync()
    {
        var json = await _storage.GetItemAsync(_options.SessionStorageKey);
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            var payload = JsonSerializer.Deserialize<SessionPayload>(json, JsonOptions);
            if (payload?.Card is null)
                return null;

            ClearImages(payload.Card);
            return (payload.Card, payload.Skin, payload.Appearance);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public string ExportJson(CardModel card, bool includeImages)
    {
        var clone = card.Clone();
        if (!includeImages)
            ClearImages(clone);

        var payload = new SessionPayload
        {
            Card = clone,
            Skin = Skin.Blaze,
            Appearance = AppearanceMode.Light
        };
        return JsonSerializer.Serialize(payload, JsonOptions);
    }

    public bool TryImportJson(string json, out CardModel card, out string? error)
    {
        card = new CardModel();
        error = null;
        try
        {
            var payload = JsonSerializer.Deserialize<SessionPayload>(json, JsonOptions);
            if (payload?.Card is null)
            {
                error = "Invalid session JSON.";
                return false;
            }
            card = payload.Card;
            return true;
        }
        catch (JsonException ex)
        {
            error = ex.Message;
            return false;
        }
    }

    private static CardModel StripImages(CardModel card)
    {
        ClearImages(card);
        return card;
    }

    private static void ClearImages(CardModel card)
    {
        card.PassbookImages.Clear();
        card.HeroImage = null;
        card.WideLogoImage = null;
        card.ImageModuleImage = null;
    }

    private sealed class SessionPayload
    {
        public CardModel? Card { get; set; }
        public Skin Skin { get; set; }
        public AppearanceMode Appearance { get; set; }
    }
}
