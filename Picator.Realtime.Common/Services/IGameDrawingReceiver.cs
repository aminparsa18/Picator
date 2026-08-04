namespace Picator.Realtime.Common.Services;

public interface IGameDrawingReceiver
{
    void OnPlayerJoined();
    void OnPlayerLeft();
    void OnGameWordReceived(string? word);
    void OnPointAdded(float x, float y);
    void OnColorChanged(uint color);
    void OnThicknessChanged(float thickness);
    void OnUndoRequested();
    void OnCanvasCleared();
    void OnLineCompleted();
    void OnRoundEnded(bool wasCorrect, string word, int pointsAwarded, bool gameCompleted, int drawerScore, int guesserScore);
}