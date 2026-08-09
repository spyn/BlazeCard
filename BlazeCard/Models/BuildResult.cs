namespace BlazeCard.Models;

public class BuildResult
{
    public bool Success { get; init; }
    public string? AppleCodeSnippet { get; init; }
    public string? GoogleCodeSnippet { get; init; }
    public List<string> ValidationErrors { get; init; } = [];

    public static BuildResult Failure(IEnumerable<string> errors) =>
        new() { Success = false, ValidationErrors = errors.ToList() };

    public static BuildResult Ok(string appleCode, string googleCode) =>
        new() { Success = true, AppleCodeSnippet = appleCode, GoogleCodeSnippet = googleCode };
}
