using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HOCONLINE.Controllers
{
    public class FeedbackController : Controller
    {
        // GET: Feedback
        public ActionResult Sendfeedback()
        {
            return View();

        }
        public ActionResult Send()
        {
            string thongtin = Request.Form["thongtin"].ToString();
            var user = Session["user"] as HOCONLINE.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");

            EmailController.SendEmail("haupham404@gmail.com","Góp ý cải thiện hệ thống từ email : " + user.Email,thongtin);
            return RedirectToAction("Index", "TrangChu");

        }
    }
}