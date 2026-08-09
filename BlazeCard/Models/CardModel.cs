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

    public string? LogoImage { get; set; }
    public string? IconImage { get; set; }
    public string? StripImage { get; set; }
    public string? HeroImage { get; set; }
    public string? ThumbnailImage { get; set; }

    public BarcodeFormat BarcodeFormat { get; set; } = BarcodeFormat.QR;
    public string BarcodeMessage { get; set; } = string.Empty;
    public string BarcodeAltText { get; set; } = string.Empty;

    public string DefaultLanguage { get; set; } = "en-AU";

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
            LogoImage = LogoImage,
            IconImage = IconImage,
            StripImage = StripImage,
            HeroImage = HeroImage,
            ThumbnailImage = ThumbnailImage,
            BarcodeFormat = BarcodeFormat,
            BarcodeMessage = BarcodeMessage,
            BarcodeAltText = BarcodeAltText,
            DefaultLanguage = DefaultLanguage
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
