namespace Picator.Realtime.Common.Services;

public interface IMatchFoundReceiver
{
    void OnMatchFound(string gameCode);
    void OnQueueExpired();
}
