using CHNU_Connect.BLL.DTOs.Notification;
using CHNU_Connect.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace CHNU_Connect.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        // Словник: userId → connectionId (online/offline)
        private static readonly ConcurrentDictionary<int, HashSet<string>> _onlineUsers
            = new ConcurrentDictionary<int, HashSet<string>>();

        private readonly INotificationService _notificationService;

        public ChatHub(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // ==================== CONNECT / DISCONNECT ====================

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId != null)
            {
                _onlineUsers.AddOrUpdate(
                    userId.Value,
                    new HashSet<string> { Context.ConnectionId },
                    (_, set) => { set.Add(Context.ConnectionId); return set; }
                );

                // Повідомити всіх що цей юзер онлайн
                await Clients.Others.SendAsync("UserOnline", userId.Value);

                // Відправити поточному юзеру список онлайн юзерів
                var onlineIds = _onlineUsers.Keys.ToList();
                await Clients.Caller.SendAsync("OnlineUsers", onlineIds);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (userId != null)
            {
                if (_onlineUsers.TryGetValue(userId.Value, out var connections))
                {
                    connections.Remove(Context.ConnectionId);
                    if (connections.Count == 0)
                    {
                        _onlineUsers.TryRemove(userId.Value, out _);
                        // Повідомити всіх що юзер офлайн
                        await Clients.Others.SendAsync("UserOffline", userId.Value);
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        // ==================== CHAT GROUPS ====================

        public async Task JoinChat(int chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-{chatId}");
        }

        public async Task LeaveChat(int chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat-{chatId}");
        }

        // ==================== NOTIFICATION PUSH ====================

        /// Відправити нотифікацію конкретному юзеру через SignalR
        public static async Task SendNotificationToUser(
            IHubContext<ChatHub> hubContext,
            int userId,
            NotificationDto notification)
        {
            await hubContext.Clients
                .Group($"user-{userId}")
                .SendAsync("ReceiveNotification", notification);
        }

        public async Task SubscribeToNotifications()
        {
            var userId = GetUserId();
            if (userId != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId.Value}");
            }
        }

        // ==================== HELPERS ====================

        public static bool IsUserOnline(int userId)
            => _onlineUsers.ContainsKey(userId);

        public static List<int> GetOnlineUserIds()
            => _onlineUsers.Keys.ToList();

        private int? GetUserId()
        {
            var claim = Context.User?.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}
