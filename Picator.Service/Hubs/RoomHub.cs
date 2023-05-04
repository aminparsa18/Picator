using Microsoft.AspNetCore.SignalR;

namespace Picator.Service.Hubs;

public class RoomHub : Hub
{
    public async Task AddToRoom(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

        await Clients.Group(roomName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {roomName}.");
    }

    public async Task RemoveFromRoom(string roomName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);

        await Clients.Group(roomName).SendAsync("Send", $"{Context.ConnectionId} has left the group {roomName}.");
    }
}