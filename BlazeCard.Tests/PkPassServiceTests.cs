using BlazeCard.Models;
using BlazeCard.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Passbook.Generator;

namespace BlazeCard.Tests;

public class PkPassServiceTests
{
    [Fact]
    public void BuildRequestPreview_requires_icon()
    {
        var svc = CreateService();
        var card = new CardModel
        {
            Description = "Pass",
            OrganizationName = "Org"
        };

        var preview = svc.BuildRequestPreview(card);
        preview.HasIcon.Should().BeFalse();
        preview.MissingRequirements.Should().Contain(r => r.Contains("Icon", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Preview_package_includes_pass_json_manifest_images_and_signature_marker()
    {
        var svc = CreateService();
        var card = new CardModel
        {
            Description = "Pass",
            OrganizationName = "Org",
            LogoText = "BLAZE",
            BackgroundColor = "#0D7377",
            LabelColor = "#FFFFFF",
            ForegroundColor = "#FFFFFF"
        };
        card.SetPassbookImage(PassbookImage.Icon2X,
            "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==");
        card.PrimaryFields.Add(new PassField { Key = "name", Label = "Name", Value = "Guybrush" });

        var result = svc.Generate(card);
        result.Success.Should().BeTrue(result.Error);
        result.Bytes.Should().NotBeNull();
        result.Entries.Should().Contain(e => e.Name == "pass.json");
        result.Entries.Should().Contain(e => e.Name == "manifest.json");
        result.Entries.Should().Contain(e => e.Name == "signature");
        result.Entries.Should().Contain(e => e.Name == "icon@2x.png");
        result.Entries.First(e => e.Name == "pass.json").Text.Should().Contain("Guybrush");
        result.Entries.First(e => e.Name == "signature").Text.Should().Contain("preview");
    }

    private static PkPassService CreateService()
    {
        var options = Options.Create(new BlazeCardOptions
        {
            PkPass = new PkPassOptions
            {
                Enabled = true,
                UsePreviewCertificates = true,
                PassTypeIdentifier = "pass.com.example.blazecard",
                TeamIdentifier = "TEAMID"
            }
        });
        return new PkPassService(options, new StubEnv());
    }

    private sealed class StubEnv : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "BlazeCard";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = Path.GetTempPath();
        public string EnvironmentName { get; set; } = "Development";
        public string ContentRootPath { get; set; } = Path.GetTempPath();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
