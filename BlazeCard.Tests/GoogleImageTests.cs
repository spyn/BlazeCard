using BlazeCard.Models;
using FluentAssertions;
using Passbook.Generator;

namespace BlazeCard.Tests;

public class GoogleImageTests
{
    [Fact]
    public void Google_snippet_emits_wideLogo_and_imageModulesData_placeholders()
    {
        var state = StateServiceFactory.Create();
        state.UpdateCard(c =>
        {
            c.Description = "Pass";
            c.OrganizationName = "Org";
            c.BarcodeFormat = BarcodeFormat.None;
            c.WideLogoImage = "data:image/png;base64,WIDE";
            c.ImageModuleImage = "data:image/png;base64,MODULE";
        });

        var google = state.GoogleCodeSnippet;
        google.Should().Contain("wideLogo");
        google.Should().Contain("https://example.com/images/wide-logo.png");
        google.Should().Contain("imageModulesData");
        google.Should().Contain("https://example.com/images/module.png");
        google.Should().NotContain("data:image");
        google.Should().NotContain("WIDE");
    }

    [Fact]
    public void Google_snippet_uses_camel_case_and_required_generic_object_keys()
    {
        var state = StateServiceFactory.Create();
        state.UpdateCard(c =>
        {
            c.Description = "Pass";
            c.OrganizationName = "Org";
            c.LogoText = "BLAZE";
            c.GoogleGenericType = "GENERIC_GYM_MEMBERSHIP";
            c.BarcodeFormat = BarcodeFormat.QR;
            c.BarcodeMessage = "ABC-1";
            c.SetPassbookImage(PassbookImage.Logo2X, "data:image/png;base64,AAAA");
            c.BackFields.Add(new PassField { Key = "web", Label = "Site", Value = "https://example.com" });
        });

        var google = state.GoogleCodeSnippet;
        google.Should().Contain("\"sourceUri\"");
        google.Should().NotContain("\"SourceUri\"");
        google.Should().Contain("\"type\": \"QR_CODE\"");
        google.Should().NotContain("\"Type\":");
        google.Should().Contain("\"state\": \"ACTIVE\"");
        google.Should().Contain("\"genericType\": \"GENERIC_GYM_MEMBERSHIP\"");
        google.Should().Contain("\"header\"");
        google.Should().Contain("genericClass");
        google.Should().Contain("linksModuleData");
        google.Should().Contain("https://example.com");
    }

    [Fact]
    public void SetGoogleImage_clears_and_stores_slots()
    {
        var state = StateServiceFactory.Create();
        state.SetGoogleImage(GoogleImageSlot.WideLogo, "data:wide");
        state.Card.WideLogoImage.Should().Be("data:wide");

        state.SetGoogleImage(GoogleImageSlot.WideLogo, null);
        state.Card.WideLogoImage.Should().BeNull();
    }
}
