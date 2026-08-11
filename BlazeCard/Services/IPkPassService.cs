using BlazeCard.Models;

namespace BlazeCard.Services;

public interface IPkPassService
{
    bool IsConfigured { get; }
    string? ConfigurationHint { get; }
    PkPassGenerateResult Generate(CardModel model);
    PassGeneratorRequestPreview BuildRequestPreview(CardModel model);
}

/// <summary>Lightweight view of what would be sent to PassGenerator (for tests / UI).</summary>
public sealed class PassGeneratorRequestPreview
{
    public string PassTypeIdentifier { get; init; } = "";
    public string TeamIdentifier { get; init; } = "";
    public string Description { get; init; } = "";
    public string OrganizationName { get; init; } = "";
    public int ImageCount { get; init; }
    public bool HasIcon { get; init; }
    public IReadOnlyList<string> MissingRequirements { get; init; } = [];
}

public sealed class PkPassPackageEntry
{
    public string Name { get; init; } = "";
    public int SizeBytes { get; init; }
    public bool IsImage { get; init; }
    public string? DataUri { get; init; }
    public string? Text { get; init; }
}

public sealed class PkPassGenerateResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public byte[]? Bytes { get; init; }
    public IReadOnlyList<PkPassPackageEntry> Entries { get; init; } = [];
    public string FileName { get; init; } = "blazecard.pkpass";

    public static PkPassGenerateResult Fail(string error) => new() { Success = false, Error = error };
}
