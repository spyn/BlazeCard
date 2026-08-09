namespace BlazeCard.Models;

public class VisualComponent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public ToolboxItemType Type { get; set; }
    public DropZoneId Zone { get; set; }
    public PassField? Field { get; set; }
    public int Order { get; set; }
}
