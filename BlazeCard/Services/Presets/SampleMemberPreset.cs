using BlazeCard.Models;

namespace BlazeCard.Services.Presets;

public static class SampleMemberPreset
{
    public static void Apply(CardModel card)
    {
        card.Description = "BlazeCard Pass";
        card.OrganizationName = "Blaze Inc.";
        card.LogoText = "BLAZE";
        card.BackgroundColor = "#0D7377";
        card.LabelColor = "#FFFFFF";
        card.ForegroundColor = "#FFFFFF";
        card.DefaultLanguage = "en-AU";
        card.GoogleGenericType = "GENERIC_LOYALTY_CARD";
        card.BarcodeFormat = BarcodeFormat.QR;
        card.BarcodeMessage = "BLAZE-SAMPLE-12345";
        card.BarcodeAltText = "BLAZE-SAMPLE-12345";

        card.HeaderFields = [];
        card.PrimaryFields =
        [
            new PassField { Key = "memberName", Label = "Membership", Value = "JOHN DOE" }
        ];
        card.SecondaryFields =
        [
            new PassField { Key = "memberSince", Label = "Member Since", Value = "2023" },
            new PassField { Key = "tier", Label = "Tier", Value = "Platinum" }
        ];
        card.AuxiliaryFields = [];
        card.BackFields =
        [
            new PassField { Key = "support", Label = "Support", Value = "blazecard.example" }
        ];
    }
}
