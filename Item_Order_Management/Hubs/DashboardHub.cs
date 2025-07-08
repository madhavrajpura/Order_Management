using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Item_Order_Management.Hubs;

[Authorize(Roles = "Admin")]
public class DashboardHub : Hub
{
    public async Task JoinAdminGroup()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
    }

    public async Task NotifyDashboardUpdate()
    {
        await Clients.Group("Admins").SendAsync("ReceiveDashboardUpdate");
    }
}