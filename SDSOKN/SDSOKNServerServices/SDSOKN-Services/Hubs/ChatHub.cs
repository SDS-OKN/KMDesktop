using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SDSOKN_Services.Hubs
{
    public class ChatHub : Hub 
    {
        public async Task SendMessage(string user, string message)
        {
            if (user == "browser")
            {
                await Clients.All.SendAsync("ReceiveMessage", user, message);                
            }
            else
            {
                await Clients.All.SendAsync("ReceiveMessage", user, message);
            }
        }
    }
}
