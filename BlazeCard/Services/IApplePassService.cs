using BlazeCard.Models;

namespace BlazeCard.Services;

public interface IApplePassService
{
    /// <summary>
    /// Full pasteable C# sample. App never executes Generate().
    /// </summary>
    string GenerateCodeSnippet(CardModel model);

    /// <summary>Best-effort parse of snippet text into a partial CardModel update.</summary>
    bool TryParseCodeSnippet(string code, CardModel target, out string? warning);
}
