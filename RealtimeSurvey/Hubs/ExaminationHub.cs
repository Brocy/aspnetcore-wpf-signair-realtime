using Microsoft.AspNetCore.SignalR;
using RealtimeSurvey.Hubs.Presence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealtimeSurvey.Hubs
{
    public class ExaminationHub : Hub
    {
        private readonly PresenceTracker presenceTracker;

        public ExaminationHub(PresenceTracker presenceTracker)
        {
            this.presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            var userid = "User" + Context.ConnectionId;

            var httpContext = Context.GetHttpContext();

            var param1 = httpContext.Request.Query["device"];
          

            var result = await presenceTracker.ConnectionOpened(userid);
            if (result.UserJoined)
            {
                await Clients.All.SendAsync("newMessage", "system", $"{userid} joined");
            }

            var currentUsers = await presenceTracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("newMessage", "system", $"Currently online:\n{string.Join("\n", currentUsers)}");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userid = "User" + Context.ConnectionId;

            var result = await presenceTracker.ConnectionClosed(userid);
            if (result.UserLeft)
            {
                await Clients.All.SendAsync("newMessage", "system", $"{userid} left");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("newMessage", "anonymous", message);
        }
    }
}
