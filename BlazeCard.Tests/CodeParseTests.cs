using BlazeCard.Models;
using FluentAssertions;

namespace BlazeCard.Tests;

public class CodeParseTests
{
    [Fact]
    public async Task Valid_enough_csharp_edit_updates_model()
    {
        var state = StateServiceFactory.Create();
        state.UpdateCard(c =>
        {
            c.Description = "Old";
            c.OrganizationName = "Old Org";
            c.BarcodeFormat = BarcodeFormat.None;
            c.PrimaryFields.Add(new PassField { Key = "memberName", Label = "Member", Value = "Old Name" });
        });

        var edited = state.AppleCodeSnippet
            .Replace("\"Old\"", "\"New Description\"", StringComparison.Ordinal)
            .Replace("\"Old Org\"", "\"New Org\"", StringComparison.Ordinal)
            .Replace("\"Old Name\"", "\"Sam Rivera\"", StringComparison.Ordinal);

        await state.ApplyAppleCodeEditAsync(edited);

        state.HasCodeSyncWarning.Should().BeFalse();
        state.Card.Description.Should().Be("New Description");
        state.Card.OrganizationName.Should().Be("New Org");
        state.Card.PrimaryFields.Should().ContainSingle(f => f.Value == "Sam Rivera");
    }

    [Fact]
    public async Task Garbage_csharp_parse_keeps_last_good_model()
    {
        var state = StateServiceFactory.Create();
        state.UpdateCard(c =>
        {
            c.Description = "Keep Me";
            c.OrganizationName = "Keep Org";
            c.BarcodeFormat = BarcodeFormat.None;
        });

        await state.ApplyAppleCodeEditAsync("this is not valid csharp {{{");

        state.HasCodeSyncWarning.Should().BeTrue();
        state.Card.Description.Should().Be("Keep Me");
        state.Card.OrganizationName.Should().Be("Keep Org");
    }

    [Fact]
    public async Task Google_snippet_parse_reads_genericType_header_and_barcode()
    {
        var state = StateServiceFactory.Create();
        state.UpdateCard(c =>
        {
            c.Description = "Pass";
            c.OrganizationName = "Org";
            c.GoogleGenericType = "GENERIC_GYM_MEMBERSHIP";
            c.BarcodeFormat = BarcodeFormat.QR;
            c.BarcodeMessage = "ABC-1";
            c.PrimaryFields.Add(new PassField { Key = "primary", Label = "Member", Value = "Ada" });
        });

        await state.ApplyGoogleCodeEditAsync(state.GoogleCodeSnippet);

        state.HasCodeSyncWarning.Should().BeFalse();
        state.Card.GoogleGenericType.Should().Be("GENERIC_GYM_MEMBERSHIP");
        state.Card.BarcodeMessage.Should().Be("ABC-1");
        state.Card.PrimaryFields.Should().ContainSingle(f => f.Value == "Ada" && f.Label == "Member");
    }
}
