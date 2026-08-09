using BlazeCard.Models;
using FluentAssertions;

namespace BlazeCard.Tests;

public class VisualProjectionTests
{
    [Fact]
    public void Entering_visual_mode_rebuilds_zones_from_model()
    {
        var state = StateServiceFactory.Create();
        state.UpdateCard(c =>
        {
            c.Description = "Pass";
            c.OrganizationName = "Org";
            c.BarcodeFormat = BarcodeFormat.QR;
            c.BarcodeMessage = "X";
            c.HeaderFields.Add(new PassField { Key = "h", Label = "H", Value = "1" });
            c.PrimaryFields.Add(new PassField { Key = "p", Label = "P", Value = "2" });
        });

        state.SetMode(AppMode.Visual);

        state.VisualComponents.Should().Contain(c => c.Zone == DropZoneId.Header && c.Field!.Key == "h");
        state.VisualComponents.Should().Contain(c => c.Zone == DropZoneId.Primary && c.Field!.Key == "p");
        state.VisualComponents.Should().Contain(c => c.Type == ToolboxItemType.Barcode && c.Zone == DropZoneId.Barcode);
    }

    [Fact]
    public void Dropping_visual_component_updates_card_model()
    {
        var state = StateServiceFactory.Create();
        state.UpdateCard(c =>
        {
            c.Description = "Pass";
            c.OrganizationName = "Org";
            c.BarcodeFormat = BarcodeFormat.None;
        });
        state.SetMode(AppMode.Visual);

        state.AddVisualComponent(new VisualComponent
        {
            Type = ToolboxItemType.PrimaryField,
            Field = new PassField { Key = "dropped", Label = "Dropped", Value = "Value" }
        });

        state.Card.PrimaryFields.Should().ContainSingle(f => f.Key == "dropped" && f.Value == "Value");
        state.VisualComponents.Should().Contain(c => c.Field!.Key == "dropped");
    }
}
