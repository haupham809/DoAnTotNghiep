using DOANTOTNGHIEP.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DOANTOTNGHIEP.Signalr
{
    [HubName("chat")]
    public class Message : Hub
    {
       
        public void Messages(string sender, string receiver ,string malop,string message)
        {
            DB db = new DB();
            var token =db.TaiKhoans.SingleOrDefault(x => x.TenDangNhap.Equals(receiver));
            var user = token.token;
            if(user == null)
            {
                user = "";
            }
                Clients.Client(user).message(sender, receiver,malop ,message);
            
           
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


    }
}