using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HOCONLINE.Signalr
{
    [HubName("thongbao")]
    public class Thongbao : Hub
    {
        public void Thongbaos(string malop,string nguoidang)
        {

            Clients.All.thongbao( malop,nguoidang);
        }

    }
}