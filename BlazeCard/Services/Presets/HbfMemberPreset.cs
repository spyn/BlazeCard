using BlazeCard.Models;

namespace BlazeCard.Services.Presets;

public static class HbfMemberPreset
{
    public static void Apply(CardModel card)
    {
        card.Description = "HBF Membership Card";
        card.OrganizationName = "HBF Health";
        card.LogoText = "HBF";
        card.BackgroundColor = "#0D7377";
        card.LabelColor = "#FFFFFF";
        card.ForegroundColor = "#FFFFFF";
        card.DefaultLanguage = "en-AU";
        card.BarcodeFormat = BarcodeFormat.QR;
        card.BarcodeMessage = "HBF-SAMPLE-12345";
        card.BarcodeAltText = "HBF-SAMPLE-12345";

        card.HeaderFields =
        [
            new PassField { Key = "memberSince", Label = "Member Since", Value = "2020" }
        ];
        card.PrimaryFields =
        [
            new PassField { Key = "memberName", Label = "Member Name", Value = "Alex Taylor" }
        ];
        card.SecondaryFields =
        [
            new PassField { Key = "membershipNumber", Label = "Membership Number", Value = "12345678" }
        ];
        card.AuxiliaryFields =
        [
            new PassField { Key = "expiry", Label = "Expiry", Value = "31 Dec 2027" }
        ];
        card.BackFields =
        [
            new PassField { Key = "support", Label = "Support", Value = "hbf.com.au" }
        ];
    }
}
