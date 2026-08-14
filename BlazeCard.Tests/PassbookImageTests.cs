using BlazeCard.Models;
using FluentAssertions;
using Passbook.Generator;

namespace BlazeCard.Tests;

public class PassbookImageTests
{
    [Fact]
    public void SetPassbookImage_stores_slot_and_prefers_retina_for_preview()
    {
        var card = new CardModel();
        card.SetPassbookImage(PassbookImage.Logo, "data:logo1x");
        card.SetPassbookImage(PassbookImage.Logo2X, "data:logo2x");

        card.GetPassbookPreview(PassbookImage.Logo3X, PassbookImage.Logo2X, PassbookImage.Logo)
            .Should().Be("data:logo2x");
    }

    [Fact]
    public void Strip_slots_match_apple_allotted_space_not_a_stretch_flag()
    {
        // Apple Wallet cover-fills a fixed strip slot (crop, keep aspect).
        // pass.json / dotnet-passbook have no stretch/distort option.
        var strip = PassbookImageCatalog.AppleGroups.Single(g => g.Name == "Strip");
        strip.Slots.Select(s => (s.Image, s.Width, s.Height)).Should().Equal(
            (PassbookImage.Strip, 320, 123),
            (PassbookImage.Strip2X, 640, 246),
            (PassbookImage.Strip3X, 960, 369));
        strip.Hint.Should().Contain("no stretch");
    }

    [Fact]
    public void Clearing_passbook_image_removes_slot()
    {
        var state = StateServiceFactory.Create();
        state.SetPassbookImage(PassbookImage.Icon2X, "data:icon");
        state.Card.PassbookImages.Should().ContainKey(PassbookImage.Icon2X);

        state.SetPassbookImage(PassbookImage.Icon2X, null);
        state.Card.PassbookImages.Should().NotContainKey(PassbookImage.Icon2X);
    }
}
