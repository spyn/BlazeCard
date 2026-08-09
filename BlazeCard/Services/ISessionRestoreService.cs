using BlazeCard.Models;

namespace BlazeCard.Services;

public interface ISessionRestoreService
{
    Task SaveAsync(CardModel card, Skin skin, AppearanceMode appearance);
    Task<(CardModel? Card, Skin Skin, AppearanceMode Appearance)?> TryLoadAsync();
    string ExportJson(CardModel card, bool includeImages);
    bool TryImportJson(string json, out CardModel card, out string? error);
}
