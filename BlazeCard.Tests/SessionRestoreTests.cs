using BlazeCard.Models;
using BlazeCard.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Passbook.Generator;

namespace BlazeCard.Tests;

public class SessionRestoreTests
{
    [Fact]
    public async Task Session_restore_round_trips_non_image_fields_and_excludes_images()
    {
        var storage = new InMemoryLocalStorage();
        var options = Options.Create(new BlazeCardOptions { SessionStorageKey = "test.session" });
        ISessionRestoreService restore = new SessionRestoreService(storage, options);

        var card = new CardModel
        {
            Description = "Restored Pass",
            OrganizationName = "Restored Org",
            LogoText = "Logo",
            BackgroundColor = "#112233",
            DefaultLanguage = "en-AU",
            BarcodeFormat = BarcodeFormat.QR,
            BarcodeMessage = "ABC",
            PrimaryFields =
            [
                new PassField { Key = "memberName", Label = "Member", Value = "Guybrush" }
            ]
        };
        card.SetPassbookImage(PassbookImage.Logo2X, "data:image/png;base64,SHOULD_NOT_PERSIST");
        card.HeroImage = "data:image/png;base64,HERO_SHOULD_NOT";
        card.WideLogoImage = "data:image/png;base64,WIDE_SHOULD_NOT";
        card.ImageModuleImage = "data:image/png;base64,MODULE_SHOULD_NOT";

        await restore.SaveAsync(card, Skin.Blaze, AppearanceMode.Dark);
        var loaded = await restore.TryLoadAsync();

        loaded.Should().NotBeNull();
        loaded!.Value.Card.Should().NotBeNull();
        loaded.Value.Card!.Description.Should().Be("Restored Pass");
        loaded.Value.Card.OrganizationName.Should().Be("Restored Org");
        loaded.Value.Card.LogoText.Should().Be("Logo");
        loaded.Value.Card.BackgroundColor.Should().Be("#112233");
        loaded.Value.Card.BarcodeMessage.Should().Be("ABC");
        loaded.Value.Card.PrimaryFields.Should().ContainSingle(f => f.Value == "Guybrush");
        loaded.Value.Card.PassbookImages.Should().BeEmpty();
        loaded.Value.Card.HeroImage.Should().BeNull();
        loaded.Value.Card.WideLogoImage.Should().BeNull();
        loaded.Value.Card.ImageModuleImage.Should().BeNull();
        loaded.Value.Skin.Should().Be(Skin.Blaze);
        loaded.Value.Appearance.Should().Be(AppearanceMode.Dark);

        var export = restore.ExportJson(card, includeImages: false);
        export.Should().NotContain("SHOULD_NOT_PERSIST");
        export.Should().NotContain("data:image");
    }

    private sealed class InMemoryLocalStorage : ILocalStorage
    {
        private readonly Dictionary<string, string> _items = new();

        public Task SetItemAsync(string key, string value)
        {
            _items[key] = value;
            return Task.CompletedTask;
        }

        public Task<string?> GetItemAsync(string key) =>
            Task.FromResult(_items.TryGetValue(key, out var v) ? v : null);

        public Task RemoveItemAsync(string key)
        {
            _items.Remove(key);
            return Task.CompletedTask;
        }
    }
}
