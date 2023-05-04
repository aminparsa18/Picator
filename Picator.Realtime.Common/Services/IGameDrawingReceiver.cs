namespace Picator.Realtime.Common.Services;

public interface IGameDrawingReceiver
{
    void OnPlayerJoined();
    void OnGameWordReceived(string? word);
    void OnPointAdded(float x, float y);
    void OnLineCompleted();
}