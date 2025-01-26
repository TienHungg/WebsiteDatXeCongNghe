using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteDatXeCongNghe
{
    public class ChatHub : Hub
    {
        public void CustomerSend(string message, string phoneOfSender, string phoneOfReceiver)
        {
            // Broadcast the message to all connected clients
            Clients.All.displayCustomer(message, phoneOfSender, phoneOfReceiver);
        }
        public void DriverSend(string message, string phoneOfSender, string phoneOfReceiver)
        {
            // Broadcast the message to all connected clients
            Clients.All.displayDriver(message, phoneOfSender, phoneOfReceiver);
        }
    }
}