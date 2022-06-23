using BundleTransformer.Core.Constants;
using CaptchaMvc.HtmlHelpers;
using DOANTOTNGHIEP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOANTOTNGHIEP.Controllers
{
    public class LoginController : Controller
    {
        public void checkcookieuser()
        {
            DB db = new DB();
            var user = Request.Cookies["user"];
            if (user != null && user["TenDangNhap"].ToString().Length > 0 && user["Matkhau"].ToString().Length > 0)
            {
                var tendangnhap = Models.crypt.Encrypt.Decryptuser(user["TenDangNhap"].ToString());
                var matkhau = user["Matkhau"].ToString();
                var TK = db.TaiKhoans.SingleOrDefault(x => x.TenDangNhap.Equals(tendangnhap) && x.MatKhau.Equals(matkhau));
                if (TK != null)
                {
                    Session["user"] = TK;
                }

            }
        }
        // GET: Login
        public ActionResult Login()
        {
            checkcookieuser();
            if (Session["user"] != null)
            {
                return RedirectToAction("Index", "TrangChu");
            }
            TaiKhoan taiKhoan = new TaiKhoan();

            return View(taiKhoan);
        }
        [HttpPost]

        public ActionResult checkaccount(TaiKhoan taiKhoan)
        {
            checkcookieuser();
            if (Session["user"] != null)
            {
                return RedirectToAction("Index", "TrangChu");
            }

            /*if (!this.IsCaptchaValid(""))
            {
                ModelState.AddModelError("", "Invalid Captcha");
            }

            else if (ModelState.IsValid)
            {*/
            DB db = new DB();
            string mk = Models.crypt.Encrypt.encryptuser(taiKhoan.MatKhau);
            var TK = db.TaiKhoans.SingleOrDefault(x => x.TenDangNhap.Equals(taiKhoan.TenDangNhap) && x.MatKhau.Equals(mk));
          
            if (TK != null)
            {
                Session["user"] = TK;
                HttpCookie user =new HttpCookie("user");
                user["TenDangNhap"] = Models.crypt.Encrypt.encryptuser(TK.TenDangNhap);
                user["Matkhau"] = TK.MatKhau;
                user.Expires = DateTime.Now.AddDays(365 * 10);
                Response.Cookies.Add(user);
                return RedirectToAction("Index", "TrangChu");
            }
            else if (db.TaiKhoans.SingleOrDefault(x => x.TenDangNhap.Equals(taiKhoan.TenDangNhap)) != null)
            {
                ModelState.AddModelError("", "Mật khẩu không đúng ");
            }
            else if (db.TaiKhoans.SingleOrDefault(x => x.TenDangNhap.Equals(taiKhoan.TenDangNhap)) == null)
            {
                ModelState.AddModelError("", "tên đăng nhập không đúng ");
            }

            /*}*/
            return View("Login");
        }

        public ActionResult logout()
        {
            Session.RemoveAll();
            HttpCookie user = new HttpCookie("user");
            user.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(user);
            return RedirectToAction("Login", "Login");
        }
    }

}