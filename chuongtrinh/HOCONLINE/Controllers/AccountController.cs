using HOCONLINE.Models;
using HOCONLINE.Models.GetData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HOCONLINE.Controllers
{
    public class AccountController : Controller
    {
        DB db = new DB();
        // GET: Account
        public ActionResult Editaccount()
        {

            var user = Session["user"] as HOCONLINE.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            string nguoitao = user.TenDangNhap;
            var tk =GetAccount.get(nguoitao);


            return View(tk);
        }
        public ActionResult Register()
        {

           

            return View();
        }
        public ActionResult ConfirmEmail()
        {



            return View();
        }
        public ActionResult checkmaxacnhan()
        {
            var maxacnhan = Request.Form["ma"];
            string ma = Session["ma"].ToString();
            if (maxacnhan.Equals(ma))
            {
                var tk= Session["registeruser"] as HOCONLINE.Models.TaiKhoan;
                if (tk != null)
                {
                    string matkhau=Models.crypt.Encrypt.encryptuser(tk.MatKhau);
                    tk.MatKhau = matkhau;
                    db.TaiKhoans.Add(tk);
                    db.SaveChanges();
                    Session.Remove("registeruser");
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    return View("NewPass");
                }


            }
            else ModelState.AddModelError("", "Mã xác nhận không đúng");

            return View("ConfirmEmail");
        }
        public ActionResult Forgotpass()
        {



            return View();
        }
        public ActionResult checkuser()
        {
            var user = Request.Form["username"];
            var email = Request.Form["email"];
            var TK = db.TaiKhoans.SingleOrDefault(x => x.TenDangNhap.Equals(user) && x.Email.Equals(email));
            if (TK != null)
            {
                TaiKhoan tk = new TaiKhoan();
                tk.TenDangNhap = user;
                tk.Email = email;
                Session["forgotuser"] = tk;
                Random r = new Random();
                Session["ma"] = r.Next(100000, 999999);

                EmailController.SendEmail(tk.Email, "ma xac nhan", Session["ma"].ToString()); 
                return View("ConfirmEmail");
                
            }
            else ModelState.AddModelError("", "Tên đăng nhập và mật khẩu không đúng");

            return View("Forgotpass");
        }
        public ActionResult NewPass()
        {



            return View();
        }
      
        public ActionResult checkpassxacnhan()
        {
            var pass = Request.Form["pass"];
            var pass1 = Request.Form["confirmpass"];
            var tk = Session["forgotuser"] as HOCONLINE.Models.TaiKhoan;
            if (pass1.Equals(pass)&& tk!=null)
            {

               var taikhoan= db.TaiKhoans.SingleOrDefault(x => x.TenDangNhap.Equals(tk.TenDangNhap) && x.Email.Equals(tk.Email));

                taikhoan.MatKhau = Models.crypt.Encrypt.encryptuser(pass);
                db.SaveChanges();
                return RedirectToAction("Login", "Login");

            }
            else ModelState.AddModelError("", "Mật khẩu xác nhận không đúng");

            return View("NewPass");
        }
        
        [HttpPost]
        public ActionResult checkaccountRegister(TaiKhoan taiKhoan)
        {

            
            
                var pass = Request.Form["confirmpass"];
                if (pass.Equals(taiKhoan.MatKhau))
                {
                    DB db = new DB();
                    var TK = db.TaiKhoans.SingleOrDefault(x => x.TenDangNhap.Equals(taiKhoan.TenDangNhap));
                    var TK2 = db.TaiKhoans.Where(x => x.Email.Equals(taiKhoan.Email)).ToList();
                    if (TK == null && TK2.Count == 0 )
                    {
                        taiKhoan.HinhAnh = "/Content/image/imageaccount/d.jpg";
                        Session["registeruser"] = taiKhoan;
                        Random r = new Random();
                        Session["ma"] = r.Next(100000, 999999);
                      //  EmailController.SendEmail(taiKhoan.Email, "ma xac nhan", Session["ma"].ToString()); 
                        return View("ConfirmEmail");

                    }
                    else if (TK != null)
                    {
                        ModelState.AddModelError("", "Tên đăng nhập đã tồn tại ");
                    }
                    else if (TK2.Count != 0)
                    {
                        ModelState.AddModelError("", "Email đã tồn tại");
                    }

                }
                else ModelState.AddModelError("", "Mật khẩu xác nhận không đúng ");


            
            return View("Register");
        }

        [HttpPost]
        public ActionResult Checkaccount(TaiKhoan taikhoan,HttpPostedFileBase file)
        {
            
           
            var user = Session["user"] as HOCONLINE.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");

            string nguoitao = user.TenDangNhap;
            if (   taikhoan.Ho != null&& taikhoan.Ten!=null)
            {
               
                    var tk = db.TaiKhoans.SingleOrDefault(x => x.TenDangNhap.Equals(nguoitao));
                
                taikhoan.TenDangNhap = tk.TenDangNhap;
                taikhoan.Email = tk.Email;
                tk.Ho = taikhoan.Ho;
                tk.Ten = taikhoan.Ten;
                if(file != null)
                {
                    
                    string path = Server.MapPath("~/Content/image/imageaccount/" + nguoitao + file.FileName);

                    tk.HinhAnh = "/Content/image/imageaccount/" + nguoitao + file.FileName;
                    
                   

                    file.SaveAs(path);
                } 
                taikhoan.HinhAnh = tk.HinhAnh;
                Session["user"] = tk;
                db.SaveChanges();
                ModelState.AddModelError("Erroreditaccount", "chinh sua thanh cong");
                return View("Editaccount",taikhoan);
            }
            else
            {
                ModelState.AddModelError("Erroreditaccount", "thong tin con trong vui long nhap lai");
            }
            return View("Editaccount", taikhoan);



        }
        public ActionResult EditPass()
        {
            return View();

        }
        [HttpPost]
        public ActionResult CheckPass()
        {
            var pass1 = Request.Form["pass1"];
            var pass2 = Request.Form["pass2"];
            var pass3 = Request.Form["pass3"];
            var user = Session["user"] as HOCONLINE.Models.TaiKhoan;
            string nguoitao = user.TenDangNhap;
            string pass =Models.crypt.Encrypt.Decryptuser(user.MatKhau);
            if (pass1.Equals(pass))
            {
                if (pass2 == pass3)
                {
                    var tk = db.TaiKhoans.SingleOrDefault(x => x.TenDangNhap.Equals(nguoitao));
                    tk.MatKhau = Models.crypt.Encrypt.encryptuser( pass2);
                    Session["pass"] = Models.crypt.Encrypt.encryptuser(pass2);
                    db.SaveChanges();
                    ModelState.AddModelError("Erroreditpass", "doi mat khau thanh cong");
                    return View("EditPass");
                }
                else
                {
                    ModelState.AddModelError("Erroreditpass", "xác nhận mật khẩu không khớp");
                }
            }
            else
            {
                ModelState.AddModelError("Erroreditpass", "mat khau khong chinh xac");
            }   
            return View("EditPass");



        }
    }
}