using Microsoft.JSInterop;

namespace BlazeCard.Services;

public class JsLocalStorage(IJSRuntime js) : ILocalStorage
{
    public async Task SetItemAsync(string key, string value) =>
        await js.InvokeVoidAsync("blazeCard.session.save", key, value);

    public async Task<string?> GetItemAsync(string key) =>
        await js.InvokeAsync<string?>("blazeCard.session.load", key);

    public async Task RemoveItemAsync(string key) =>
        await js.InvokeVoidAsync("blazeCard.session.clear", key);
}
