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
        // GET: Login
        public ActionResult Login()
        {
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
            return RedirectToAction("Login", "Login");
        }
    }

}