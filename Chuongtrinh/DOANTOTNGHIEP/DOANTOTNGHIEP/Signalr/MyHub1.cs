﻿using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOANTOTNGHIEP.Signalr
{
    public class MyHub1 : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}