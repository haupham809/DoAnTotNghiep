using DOANTOTNGHIEP.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DOANTOTNGHIEP.Models;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using Spire.Doc.Fields.OMath;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Tables;
using Spire.Xls;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using DOANTOTNGHIEP.Controllers;

namespace DOANTOTNGHIEP.Signalr
{
    [HubName("chat")]
    public class Chat : Hub
    {
        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

        public void Messages(string sender, string receiver, string malop, string message)
        {
            DB db = new DB();
            var token = db.TaiKhoans.SingleOrDefault(x => x.TenDangNhap.Equals(receiver));
            var user = token.token;
            if (user == null)
            {
                user = "";
            }
            Clients.Client(user).message(sender, receiver, malop, message);


        }
        public void classnames(string sender, string receiver, string malop)
        {

            DB db = new DB();
            var token = db.TaiKhoans.SingleOrDefault(x => x.TenDangNhap.Equals(receiver));
            var user = token.token;
            if (user == null)
            {
                user = "";
            }
            Clients.Client(user).classname(Models.GetData.GetClass.getnameclass(malop));


        }
        public void Thongbaos(string malop, string nguoidang)
        {

            Clients.All.thongbao(malop, nguoidang);
        }
        public void Baitaps(string malop)
        {

            Clients.All.baitap(malop);
        }




      

        public override Task OnConnected()
        {
            string name = Context.User.Identity.Name;
            _connections.Add(name, Context.ConnectionId);
           
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string name = Context.User.Identity.Name;

            _connections.Remove(name, Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            string name = Context.User.Identity.Name;

            if (!_connections.GetConnections(name).Contains(Context.ConnectionId))
            {
               
                _connections.Add(name, Context.ConnectionId); 
              
            }
            return base.OnReconnected();
        }
    }
}