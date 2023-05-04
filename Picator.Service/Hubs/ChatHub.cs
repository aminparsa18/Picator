using Microsoft.AspNetCore.SignalR;
using Picator.Entities.Models;
using Picator.Repository;

namespace Picator.Service.Hubs;

public class ChatHub : Hub
{
    private readonly IUnitOfWork _unitOfWork;

    public ChatHub(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task SendMessage(string groupName, string msg, string type, string sender)
    {
        await Clients.OthersInGroup(groupName).SendAsync("MessageReceived", msg, type, sender);
        await _unitOfWork.GameMessage.AddFast(new GameMessage()
        {
            Content = msg,
            GameId = Guid.Parse(groupName),
            UserId = Guid.Parse(sender)
        });
    }
}