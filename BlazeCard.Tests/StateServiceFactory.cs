using BlazeCard.Services;
using Microsoft.Extensions.Options;

namespace BlazeCard.Tests;

internal static class StateServiceFactory
{
    public static IBlazeCardStateService Create()
    {
        var options = Options.Create(new BlazeCardOptions
        {
            SnippetEmitDebounceMs = 0,
            CodeParseDebounceMs = 0,
            PreviewDebounceMs = 0
        });

        IApplePassService apple = new ApplePassService();
        IGoogleWalletService google = new GoogleWalletService();
        return new BlazeCardStateService(apple, google, options);
    }
}
