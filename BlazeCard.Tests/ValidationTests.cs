using BlazeCard.Models;
using FluentAssertions;

namespace BlazeCard.Tests;

public class ValidationTests
{
    [Fact]
    public async Task Build_fails_when_description_or_organisation_missing()
    {
        var state = StateServiceFactory.Create();
        state.UpdateCard(c =>
        {
            c.Description = "";
            c.OrganizationName = "";
            c.BarcodeFormat = BarcodeFormat.None;
        });

        var result = await state.BuildAsync();

        result.Success.Should().BeFalse();
        result.ValidationErrors.Should().Contain("Description is required.");
        result.ValidationErrors.Should().Contain("Organisation Name is required.");
    }

    [Fact]
    public async Task Build_fails_when_header_or_primary_field_limits_exceeded()
    {
        var state = StateServiceFactory.Create();
        state.UpdateCard(c =>
        {
            c.Description = "Pass";
            c.OrganizationName = "Org";
            c.BarcodeFormat = BarcodeFormat.None;
            c.HeaderFields.Add(new PassField { Key = "h1", Label = "H1", Value = "1" });
            c.HeaderFields.Add(new PassField { Key = "h2", Label = "H2", Value = "2" });
            c.PrimaryFields.Add(new PassField { Key = "p1", Label = "P1", Value = "1" });
            c.PrimaryFields.Add(new PassField { Key = "p2", Label = "P2", Value = "2" });
            c.PrimaryFields.Add(new PassField { Key = "p3", Label = "P3", Value = "3" });
        });

        var result = await state.BuildAsync();

        result.Success.Should().BeFalse();
        result.ValidationErrors.Should().Contain("Apple Wallet allows at most 1 header field.");
        result.ValidationErrors.Should().Contain("Apple Wallet allows at most 2 primary fields.");
    }

    [Fact]
    public async Task Build_fails_when_barcode_format_set_without_message()
    {
        var state = StateServiceFactory.Create();
        state.UpdateCard(c =>
        {
            c.Description = "Pass";
            c.OrganizationName = "Org";
            c.BarcodeFormat = BarcodeFormat.QR;
            c.BarcodeMessage = "";
        });

        var result = await state.BuildAsync();

        result.Success.Should().BeFalse();
        result.ValidationErrors.Should().Contain("Barcode message is required when a barcode format is selected.");
    }
}
