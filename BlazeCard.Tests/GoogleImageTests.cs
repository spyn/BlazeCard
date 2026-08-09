using BlazeCard.Models;
using FluentAssertions;

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
    public void SetGoogleImage_clears_and_stores_slots()
    {
        var state = StateServiceFactory.Create();
        state.SetGoogleImage(GoogleImageSlot.WideLogo, "data:wide");
        state.Card.WideLogoImage.Should().Be("data:wide");

        state.SetGoogleImage(GoogleImageSlot.WideLogo, null);
        state.Card.WideLogoImage.Should().BeNull();
    }
}
