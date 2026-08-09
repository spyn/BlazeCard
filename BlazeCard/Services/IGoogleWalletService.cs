using BlazeCard.Models;
using BlazeCard.Models.Google;

namespace BlazeCard.Services;

public interface IGoogleWalletService
{
    /// <summary>Full annotated HTTP envelope + genericObject JSON body.</summary>
    string GenerateCodeSnippet(CardModel model);

    /// <summary>Local DTO for preview hydration / serialization (not Google client types).</summary>
    GenericObjectDto BuildGenericObject(CardModel model);

    bool TryParseCodeSnippet(string code, CardModel target, out string? warning);
}
