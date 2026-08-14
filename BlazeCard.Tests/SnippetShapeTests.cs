using BlazeCard.Models;
using FluentAssertions;
using Passbook.Generator;

namespace BlazeCard.Tests;

public class SnippetShapeTests
{
    [Fact]
    public void Google_snippet_uses_https_placeholder_images_not_data_uris()
    {
        var state = StateServiceFactory.Create();
        state.UpdateCard(c =>
        {
            c.Description = "Pass";
            c.OrganizationName = "Org";
            c.BarcodeFormat = BarcodeFormat.None;
            c.SetPassbookImage(PassbookImage.Logo2X, "data:image/png;base64,iVBORw0KGgo=");
            c.HeroImage = "data:image/png;base64,AAAA";
        });

        state.GoogleCodeSnippet.Should().Contain("https://example.com/images/logo.png");
        state.GoogleCodeSnippet.Should().Contain("https://example.com/images/hero.png");
        state.GoogleCodeSnippet.Should().NotContain("data:image");
        state.GoogleCodeSnippet.Should().NotContain("iVBORw0KGgo");
    }

    [Fact]
    public void Apple_snippet_includes_full_dotnet_passbook_sample_shape()
    {
        var state = StateServiceFactory.Create();
        state.UpdateCard(c =>
        {
            c.Description = "Pass";
            c.OrganizationName = "Org";
            c.BarcodeFormat = BarcodeFormat.None;
            c.SetPassbookImage(PassbookImage.Logo2X, "data:image/png;base64,AAAA");
            c.SetPassbookImage(PassbookImage.Icon2X, "data:image/png;base64,BBBB");
        });

        var apple = state.AppleCodeSnippet;
        apple.Should().Contain("PassGenerator");
        apple.Should().Contain("new PassGenerator()");
        apple.Should().Contain("PassGeneratorRequest");
        apple.Should().Contain("passGenerator.Generate(");
        apple.Should().Contain("File.WriteAllBytes");
        apple.Should().Contain("X509Certificate");
        apple.Should().Contain("File.ReadAllBytes");
        apple.Should().Contain("PassbookImage.Logo2X");
        apple.Should().Contain("logo@2x.png");
        apple.Should().Contain("PassbookImage.Icon2X");
        apple.Should().Contain("using Passbook.Generator");
        apple.Should().Contain("SerialNumber = Guid.NewGuid()");
        apple.Should().Contain("PassStyle.Generic");
    }

    [Fact]
    public void Apple_snippet_notes_icon_requirement_and_generic_ignores_strip()
    {
        var state = StateServiceFactory.Create();
        state.UpdateCard(c =>
        {
            c.Description = "Pass";
            c.OrganizationName = "Org";
            c.BarcodeFormat = BarcodeFormat.None;
            c.SetPassbookImage(PassbookImage.Strip2X, "data:image/png;base64,AAAA");
        });

        var apple = state.AppleCodeSnippet;
        apple.Should().Contain("Apple requires icon.png");
        apple.Should().Contain("PassStyle.Generic ignores strip.png");
        apple.Should().Contain("no stretch");
        apple.Should().Contain("PassbookImage.Strip2X");
        apple.Should().NotContain("SuppressStripShine = true");
    }

    [Fact]
    public void Apple_snippet_emits_suppress_strip_shine_and_field_alignment()
    {
        var state = StateServiceFactory.Create();
        state.UpdateCard(c =>
        {
            c.Description = "Pass";
            c.OrganizationName = "Org";
            c.BarcodeFormat = BarcodeFormat.None;
            c.SuppressStripShine = true;
            c.PrimaryFields.Add(new PassField
            {
                Key = "name",
                Label = "Name",
                Value = "Ada",
                TextAlignment = TextAlignment.Right
            });
        });

        var apple = state.AppleCodeSnippet;
        apple.Should().Contain("SuppressStripShine = true");
        apple.Should().Contain("FieldTextAlignment.PKTextAlignmentRight");
        apple.Should().Contain("AddPrimaryField");
    }
}
