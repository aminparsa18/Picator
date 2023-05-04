namespace Picator.Service.Contracts;

public interface ISmsSender
{
    string SendAuthSmsAsync(string code, string phoneNumber);
}