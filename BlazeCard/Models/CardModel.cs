using System.Text.Json.Serialization;
using Passbook.Generator;

namespace BlazeCard.Models;

public class CardModel
{
    public PassType PassType { get; set; } = PassType.Generic;
    public string Description { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public string LogoText { get; set; } = string.Empty;

    public string BackgroundColor { get; set; } = "#0D7377";
    public string LabelColor { get; set; } = "#FFFFFF";
    public string ForegroundColor { get; set; } = "#FFFFFF";

    public List<PassField> HeaderFields { get; set; } = [];
    public List<PassField> PrimaryFields { get; set; } = [];
    public List<PassField> SecondaryFields { get; set; } = [];
    public List<PassField> AuxiliaryFields { get; set; } = [];
    public List<PassField> BackFields { get; set; } = [];

    /// <summary>
    /// Apple Passbook images keyed by <see cref="PassbookImage"/> (icon/logo/strip/… at 1x/2x/3x).
    /// Values are in-session data URIs for Live Preview; snippets emit file-path placeholders.
    /// </summary>
    public Dictionary<PassbookImage, string> PassbookImages { get; set; } = new();

    /// <summary>Google Wallet hero banner (not a PassbookImage).</summary>
    public string? HeroImage { get; set; }

    /// <summary>Google Wallet wideLogo — replaces circular logo when set.</summary>
    public string? WideLogoImage { get; set; }

    /// <summary>Google Wallet imageModulesData.mainImage (one module).</summary>
    public string? ImageModuleImage { get; set; }

    public BarcodeFormat BarcodeFormat { get; set; } = BarcodeFormat.QR;
    public string BarcodeMessage { get; set; } = string.Empty;
    public string BarcodeAltText { get; set; } = string.Empty;

    public string DefaultLanguage { get; set; } = "en-AU";

    /// <summary>Google Wallet <c>genericType</c> enum on GenericObject.</summary>
    public string GoogleGenericType { get; set; } = "GENERIC_TYPE_UNSPECIFIED";

    public string? GetPassbookPreview(params PassbookImage[] preference)
    {
        foreach (var key in preference)
        {
            if (PassbookImages.TryGetValue(key, out var uri) && !string.IsNullOrEmpty(uri))
                return uri;
        }
        return null;
    }

    public void SetPassbookImage(PassbookImage image, string? dataUri)
    {
        if (string.IsNullOrEmpty(dataUri))
            PassbookImages.Remove(image);
        else
            PassbookImages[image] = dataUri;
    }

    public CardModel Clone()
    {
        return new CardModel
        {
            PassType = PassType,
            Description = Description,
            OrganizationName = OrganizationName,
            LogoText = LogoText,
            BackgroundColor = BackgroundColor,
            LabelColor = LabelColor,
            ForegroundColor = ForegroundColor,
            HeaderFields = HeaderFields.Select(f => f.Clone()).ToList(),
            PrimaryFields = PrimaryFields.Select(f => f.Clone()).ToList(),
            SecondaryFields = SecondaryFields.Select(f => f.Clone()).ToList(),
            AuxiliaryFields = AuxiliaryFields.Select(f => f.Clone()).ToList(),
            BackFields = BackFields.Select(f => f.Clone()).ToList(),
            PassbookImages = new Dictionary<PassbookImage, string>(PassbookImages),
            HeroImage = HeroImage,
            WideLogoImage = WideLogoImage,
            ImageModuleImage = ImageModuleImage,
            BarcodeFormat = BarcodeFormat,
            BarcodeMessage = BarcodeMessage,
            BarcodeAltText = BarcodeAltText,
            DefaultLanguage = DefaultLanguage,
            GoogleGenericType = GoogleGenericType
        };
    }
}

public static class CardModelExtensions
{
    public static IEnumerable<PassField> AllFields(this CardModel m) =>
        m.HeaderFields
         .Concat(m.PrimaryFields)
         .Concat(m.SecondaryFields)
         .Concat(m.AuxiliaryFields)
         .Concat(m.BackFields);
}

/// <summary>UI catalog for PassbookImage slots (sizes are Apple point guidance × scale).</summary>
public static class PassbookImageCatalog
{
    public sealed record Slot(PassbookImage Image, string ScaleLabel, int Width, int Height);

    public sealed record Group(string Name, string Hint, Slot[] Slots);

    public static IReadOnlyList<Group> AppleGroups { get; } =
    [
        new("Icon", "Required on real passes — notifications / lock screen",
        [
            new(PassbookImage.Icon, "1x", 29, 29),
            new(PassbookImage.Icon2X, "2x", 58, 58),
            new(PassbookImage.Icon3X, "3x", 87, 87)
        ]),
        new("Logo", "Top-left of the pass face",
        [
            new(PassbookImage.Logo, "1x", 160, 50),
            new(PassbookImage.Logo2X, "2x", 320, 100),
            new(PassbookImage.Logo3X, "3x", 480, 150)
        ]),
        new("Strip", "Wide banner under the header. Wallet cover-fills a 320×123 pt slot (crop, keep aspect) — no stretch option in pass.json or dotnet-passbook. Size PNGs to 1x/2x/3x. Generic Wallet ignores strip on device; Coupon/Store Card show it.",
        [
            new(PassbookImage.Strip, "1x", 320, 123),
            new(PassbookImage.Strip2X, "2x", 640, 246),
            new(PassbookImage.Strip3X, "3x", 960, 369)
        ]),
        new("Thumbnail", "Optional square art (some pass layouts)",
        [
            new(PassbookImage.Thumbnail, "1x", 90, 90),
            new(PassbookImage.Thumbnail2X, "2x", 180, 180),
            new(PassbookImage.Thumbnail3X, "3x", 270, 270)
        ]),
        new("Background", "Full-bleed background art (event-style layouts)",
        [
            new(PassbookImage.Background, "1x", 180, 220),
            new(PassbookImage.Background2X, "2x", 360, 440),
            new(PassbookImage.Background3X, "3x", 540, 660)
        ]),
        new("Footer", "Above the barcode on some layouts",
        [
            new(PassbookImage.Footer, "1x", 286, 15),
            new(PassbookImage.Footer2X, "2x", 572, 30),
            new(PassbookImage.Footer3X, "3x", 858, 45)
        ])
    ];
}
