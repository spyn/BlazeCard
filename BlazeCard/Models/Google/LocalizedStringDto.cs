namespace BlazeCard.Models.Google;

public class LocalizedStringDto
{
    public TranslatedStringDto DefaultValue { get; set; } = new();
}

public class TranslatedStringDto
{
    public string Language { get; set; } = "en-AU";
    public string Value { get; set; } = string.Empty;
}
