using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DOANTOTNGHIEP.Models;
using DOANTOTNGHIEP.Models.GetData;


namespace DOANTOTNGHIEP.Controllers
{
    public class TrangChuController : Controller
    {
        // GET: TrangChu
        DB db = new DB();
        public ActionResult Index()
        {

            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");

            var lophoc = GetClass.GetLopHoc(user.TenDangNhap);
            if (Session["malop"] != null)
            {
                Session.Remove("malop");
            }
            ViewData["loimoithamgia"] = db.Loimois.Where(x => x.TenDangNhap.Equals(user.TenDangNhap)).ToList();
            return View(lophoc);

        }
        public ActionResult tuchoi(string id)
        {

            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            var loimoi = db.Loimois.SingleOrDefault(x => x.MaLop.ToString().Equals(id) && x.TenDangNhap.Equals(user.TenDangNhap));
            if (loimoi != null)
            {
                db.Loimois.Remove(loimoi);
                db.SaveChanges();

            }
            return RedirectToAction("Index", "TrangChu");
        }

        public ActionResult thamgia(string id)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            var loimoi = db.Loimois.SingleOrDefault(x => x.MaLop.ToString().Equals(id) && x.TenDangNhap.Equals(user.TenDangNhap));
            if (loimoi != null)
            {


                ThanhVienLop tvl = new ThanhVienLop();
                tvl.MaLop = Convert.ToInt32(id);
                tvl.Mathanhvien = user.TenDangNhap;
                tvl.NgayThamGia = DateTime.Now;
                db.ThanhVienLops.Add(tvl);
                db.SaveChanges();
                var baitap = db.BaiTaps.Where(x => x.MaLop.ToString().Equals(id)).ToList();
                foreach (var bai in baitap)
                {
                    if (bai.LoaiBaiTap.Equals("TracNghiem"))
                    {
                        BaiTapTN btn = new BaiTapTN();
                        btn.MaBaiTap = bai.MaBaiTap;
                        btn.NguoiNop = user.TenDangNhap;
                        db.BaiTapTNs.Add(btn);
                        db.SaveChanges();
                    }
                    else if (bai.LoaiBaiTap.Equals("TuLuan"))
                    {
                        BaiTapTL btl = new BaiTapTL();
                        btl.MaBaiTap = bai.MaBaiTap;
                        btl.NguoiNop = user.TenDangNhap;
                        db.BaiTapTLs.Add(btl);
                        db.SaveChanges();
                    }
                }
                db.Loimois.Remove(loimoi);
                db.SaveChanges();
                return RedirectToAction("Index", "TrangChu");


            }
            return RedirectToAction("Index", "TrangChu");

        }


        public ActionResult menu()
        {


            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            var lophoc = GetClass.GetLopHoc(user.TenDangNhap);

            return PartialView("menu", lophoc);
        }




    }

}