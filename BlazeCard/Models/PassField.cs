namespace BlazeCard.Models;

public class PassField
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public TextAlignment TextAlignment { get; set; } = TextAlignment.Left;

    public bool HasError { get; set; }
    public string? ErrorMessage { get; set; }

    public PassField Clone() => new()
    {
        Key = Key,
        Label = Label,
        Value = Value,
        TextAlignment = TextAlignment
    };
}
