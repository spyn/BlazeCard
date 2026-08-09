namespace BlazeCard.Services;

public class BlazeCardException(string userMessage, Exception? inner = null)
    : Exception(userMessage, inner)
{
    public string UserMessage { get; } = userMessage;
}

public class CodeParseException(string userMessage, Exception? inner = null)
    : BlazeCardException(userMessage, inner);
