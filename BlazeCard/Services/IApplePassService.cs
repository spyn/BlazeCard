using BlazeCard.Models;

namespace BlazeCard.Services;

public interface IApplePassService
{
    /// <summary>
    /// Full pasteable C# sample. Final tab may also call Generate() when PkPass certs are configured.
    /// </summary>
    string GenerateCodeSnippet(CardModel model);

    /// <summary>Best-effort parse of snippet text into a partial CardModel update.</summary>
    bool TryParseCodeSnippet(string code, CardModel target, out string? warning);
}
