namespace BlazeCard.Services;

/// <summary>Boundary for browser localStorage (JS interop). Tests use an in-memory fake.</summary>
public interface ILocalStorage
{
    Task SetItemAsync(string key, string value);
    Task<string?> GetItemAsync(string key);
    Task RemoveItemAsync(string key);
}
