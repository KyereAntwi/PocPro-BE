using DevSync.Pocpro.Shops.Notifications.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DevSync.Pocpro.Shops.Notifications.Hubs;

public class NotificationHub : Hub<INotificationClient>
{
    
}