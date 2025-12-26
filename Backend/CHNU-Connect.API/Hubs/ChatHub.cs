using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CHNU_Connect.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task JoinChat(int chatId)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"chat-{chatId}"
            );
        }

        public async Task LeaveChat(int chatId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                $"chat-{chatId}"
            );
        }
    }
}
