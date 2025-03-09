using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace InfrastructureLayer.SignalR
{

        public class TaskHub : Hub
        {
            public async Task SendTaskNotification(string message)
            {
                await Clients.All.SendAsync("ReceiveTaskNotification", message);
            }
        }
    }

