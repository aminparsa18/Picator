namespace Picator.Realtime.Common.Services;

public interface IMatchFoundReceiver
{
    void OnMatchFound(string gameCode, bool isDrawer);
    void OnQueueExpired();
}
