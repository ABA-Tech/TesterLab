using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace TesterLab.Hubs
{
    public class TestCaseHub : Hub
    {
        public Task JoinGroup(string groupName) =>
            Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        public Task LeaveGroup(string groupName) =>
            Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}
