using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace BackEnd.hub
{
    public class NotificationHub : Hub
    {
        public Task SendAll(string data)
        {
            return Clients.User("67a628f5316aa743d4699a3d").SendAsync("Send", new {
                Id = "67a628f5316aa743d4699a3d",
                Content = data,
                Type = "message"
            });
        }
     
    }
}