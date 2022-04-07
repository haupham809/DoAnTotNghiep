using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HOCONLINE.Signalr
{
    [HubName("baitap")]
    public class Baitap : Hub
    {
        public void Baitaps(string malop)
        {

            Clients.All.baitap(malop);
        }
    }
}