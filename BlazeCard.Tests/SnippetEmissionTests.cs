using BlazeCard.Models;
using FluentAssertions;

namespace BlazeCard.Tests;

public class SnippetEmissionTests
{
    [Fact]
    public async Task New_card_model_snippets_contain_org_description_and_fields()
    {
        var state = StateServiceFactory.Create();

        state.UpdateCard(c =>
        {
            c.Description = "BlazeCard Pass";
            c.OrganizationName = "Blaze Inc.";
            c.BarcodeFormat = BarcodeFormat.None;
            c.PrimaryFields.Add(new PassField
            {
                Key = "memberName",
                Label = "Member Name",
                Value = "Alex Taylor"
            });
        });

        var result = await state.BuildAsync();

        result.Success.Should().BeTrue();
        state.AppleCodeSnippet.Should().Contain("BlazeCard Pass");
        state.AppleCodeSnippet.Should().Contain("Blaze Inc.");
        state.AppleCodeSnippet.Should().Contain("memberName");
        state.AppleCodeSnippet.Should().Contain("Alex Taylor");

        state.GoogleCodeSnippet.Should().Contain("Blaze Inc.");
        state.GoogleCodeSnippet.Should().Contain("Alex Taylor");
        state.GoogleCodeSnippet.Should().Contain("Member Name");
    }

    [Fact]
    public void UpdateCard_changes_model_and_re_emits_snippets()
    {
        var state = StateServiceFactory.Create();

        state.UpdateCard(c =>
        {
            c.Description = "Original";
            c.OrganizationName = "Org A";
            c.BarcodeFormat = BarcodeFormat.None;
        });

        state.AppleCodeSnippet.Should().Contain("Original");
        state.GoogleCodeSnippet.Should().Contain("Org A");

        state.UpdateCard(c =>
        {
            c.Description = "Updated Pass";
            c.OrganizationName = "Org B";
        });

        state.Card.Description.Should().Be("Updated Pass");
        state.Card.OrganizationName.Should().Be("Org B");
        state.AppleCodeSnippet.Should().Contain("Updated Pass");
        state.AppleCodeSnippet.Should().NotContain("Original");
        state.GoogleCodeSnippet.Should().Contain("Org B");
    }
}

