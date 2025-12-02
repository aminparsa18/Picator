namespace Picator.Game.Services;

public interface IAlert
{
    void Show(string message, MessageType type);
}

public enum MessageType
{
    None,
    Success,
    Warning,
    Error,
    Info
}