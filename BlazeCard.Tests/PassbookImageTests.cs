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
    public void Clearing_passbook_image_removes_slot()
    {
        var state = StateServiceFactory.Create();
        state.SetPassbookImage(PassbookImage.Icon2X, "data:icon");
        state.Card.PassbookImages.Should().ContainKey(PassbookImage.Icon2X);

        state.SetPassbookImage(PassbookImage.Icon2X, null);
        state.Card.PassbookImages.Should().NotContainKey(PassbookImage.Icon2X);
    }
}
