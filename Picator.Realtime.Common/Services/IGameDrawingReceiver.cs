namespace Picator.Realtime.Common.Services;

public interface IGameDrawingReceiver
{
    void OnPlayerJoined();
    void OnPlayerLeft();
    void OnGameWordReceived(string? word);
    void OnPointAdded(float x, float y);
    void OnColorChanged(uint color);
    void OnThicknessChanged(float thickness);
    void OnLineCompleted();
}