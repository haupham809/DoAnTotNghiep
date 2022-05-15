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

namespace DOANTOTNGHIEP.Controllers
{
    public class ClassController : Controller
    {





        DB db = new DB();
        // GET: Class
        //hien thong báo 
        public ActionResult Index(string id)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            var lop = db.ThanhVienLops.SingleOrDefault(x => x.MaLop.ToString().Equals(id) && x.Mathanhvien.Equals(user.TenDangNhap));
            if (lop == null)
            {
                return RedirectToAction("Index", "TrangChu");
            }

            if (id != null)
            {
                Session["malop"] = id;

            }
            else
            {
                if (Session["malop"] == null)
                {
                    return RedirectToAction("Index", "TrangChu");
                }
                id = Session["malop"].ToString();
            }


            var thongbao = db.ThongBaos.Where(x => x.MaLop.ToString().Equals(id)).OrderByDescending(y => y.NgayDang).ToList();
            Session["lophoc"] = db.LopHocs.SingleOrDefault(x => x.MaLop.ToString().Equals(id));

            return View(thongbao);
        }
        [HttpPost]
        //tham gia lop hoc
        public ActionResult checkclass()
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            string nguoitao = user.TenDangNhap;
            string id = Request.Form["maclassid"];
            string ma = Models.crypt.Encrypt.Decryptclass(id).ToString();


            if (ModelState.IsValid)
            {
                DB db = new DB();
                var TK = db.LopHocs.SingleOrDefault(x => x.MaLop.ToString().Equals(ma));
                if (TK != null)
                {
                    if (db.ThanhVienLops.SingleOrDefault(x => x.MaLop.ToString().Equals(ma) && x.Mathanhvien.Equals(nguoitao)) != null)
                    {
                        ModelState.AddModelError("ErrorJoinClass", "Lop đã tham gia ");
                        return View("JoinClass");
                    }
                    ThanhVienLop tvl = new ThanhVienLop();
                    tvl.MaLop = Convert.ToInt32(ma);
                    tvl.Mathanhvien = nguoitao;
                    tvl.NgayThamGia = DateTime.Now;
                    db.ThanhVienLops.Add(tvl);
                    db.SaveChanges();
                    var baitap = db.BaiTaps.Where(x => x.MaLop.ToString().Equals(ma)).ToList();
                    foreach (var bai in baitap)
                    {
                        if (bai.LoaiBaiTap.Equals("TracNghiem"))
                        {
                            BaiTapTN btn = new BaiTapTN();
                            btn.MaBaiTap = bai.MaBaiTap;
                            btn.NguoiNop = nguoitao;
                            db.BaiTapTNs.Add(btn);
                            db.SaveChanges();
                        }
                        else if (bai.LoaiBaiTap.Equals("TuLuan"))
                        {
                            BaiTapTL btl = new BaiTapTL();
                            btl.MaBaiTap = bai.MaBaiTap;
                            btl.NguoiNop = nguoitao;
                            db.BaiTapTLs.Add(btl);
                            db.SaveChanges();
                        }
                    }
                    return RedirectToAction("Index", "TrangChu");
                }
                else
                {
                    ModelState.AddModelError("ErrorJoinClass", "Lop khong ton  tai ");
                }


            }
            return View("JoinClass");
        }
        public void CreateFolder(string strPath)
        {
            try
            {
                if (Directory.Exists(strPath) == false)
                {
                    Directory.CreateDirectory(strPath);
                }
            }
            catch { }
        }
        [HttpPost]
        //tao lop hoc
        public ActionResult createclass()
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            string nguoitao = user.TenDangNhap;
            string name = Request.Form["classname"];
            string infor = Request.Form["inforclass"];


            DB db = new DB();

            if (name != null && infor != null)
            {


                LopHoc lp = new LopHoc();
                lp.TenLop = name;
                lp.ThongTinLopHoc = infor;
                lp.NguoiTao = nguoitao;
                lp.NgayTao = DateTime.Now;
                lp.Hinhanh = "/Content/image/imageclass/img_backtoschool.jpg";
                db.LopHocs.Add(lp);
                db.SaveChanges();
                CreateFolder(Server.MapPath("~/Content/document/" + lp.MaLop));
                ThanhVienLop tvl = new ThanhVienLop();
                tvl.MaLop = lp.MaLop;
                tvl.Mathanhvien = nguoitao;
                tvl.NgayThamGia = DateTime.Now;
                db.ThanhVienLops.Add(tvl);
                db.SaveChanges();
                CreateFolder(Server.MapPath("~/Content/document/" + lp.MaLop+"/"+tvl.Mathanhvien));
                return RedirectToAction("Index", "TrangChu");
            }

            return RedirectToAction("Index", "TrangChu");
        }
        [HttpGet]
        public ActionResult Editclass()
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            string nguoitao = user.TenDangNhap;
            if (Session["lophoc"] == null) return RedirectToAction("Index", "TrangChu");
            string malop = Session["malop"].ToString();
            var lop = db.LopHocs.SingleOrDefault(x => x.MaLop.ToString().Equals(malop) && x.NguoiTao.Equals(user.TenDangNhap));
            if (lop == null)
            {
                return RedirectToAction("Index", "TrangChu");
            }

            var lophoc = Session["lophoc"] as DOANTOTNGHIEP.Models.LopHoc;
            return View(lophoc);
        }

        [HttpPost]
        public ActionResult Checkeditclass(LopHoc s, HttpPostedFileBase file)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            string nguoitao = user.TenDangNhap;
            var lophoc = Session["lophoc"] as DOANTOTNGHIEP.Models.LopHoc;
            var lop = db.LopHocs.SingleOrDefault(x => x.MaLop.ToString().Equals(lophoc.MaLop.ToString()) && x.NguoiTao.Equals(nguoitao));
            if (lop == null) return Index(lophoc.MaLop.ToString());
            lop.TenLop = s.TenLop;
            lop.ThongTinLopHoc = s.ThongTinLopHoc;
            if (file != null)
            {

                string path = Server.MapPath("~/Content/image/imageclass/" + lop.MaLop + file.FileName);

                lop.Hinhanh = "/Content/image/imageclass/" + lop.MaLop + file.FileName;

                file.SaveAs(path);
            }
            db.SaveChanges();

            return RedirectToAction("Index", new { id = lophoc.MaLop.ToString() });

        }


        //diem
        public ActionResult Diem(string id)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");

            Session["malop"] = id;

            var lop = db.ThanhVienLops.SingleOrDefault(x => x.MaLop.ToString().Equals(id) && x.Mathanhvien.Equals(user.TenDangNhap));

            if (lop == null)
            {
                return RedirectToAction("Index", "TrangChu");
            }
            var malop = Session["malop"].ToString();
            string nguoitao = user.TenDangNhap;
            var diem = DOANTOTNGHIEP.Models.GetData.Getdiem.danhsachdiem(malop, nguoitao);
            ViewBag.excel = exceldsdiem(diem);
            ViewBag.pdf = pdfdsdiem(diem);
            return View(diem);

        }

        public ActionResult Infordiem(string id)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            string malop = Session["malop"].ToString();
            var lop = db.LopHocs.SingleOrDefault(x => x.MaLop.ToString().Equals(malop) && x.NguoiTao.Equals(user.TenDangNhap));
            var kttn = db.BaiTapTNs.Where(c => c.NguoiNop.Equals(id) && c.NguoiNop.Equals(user.TenDangNhap) && c.BaiTap.MaLop.ToString().Equals(malop)).ToList();
            var kttl = db.BaiTapTLs.Where(c => c.NguoiNop.Equals(id) && c.NguoiNop.Equals(user.TenDangNhap) && c.BaiTap.MaLop.ToString().Equals(malop)).ToList();

            if (lop == null && kttn == null && kttl == null)
            {
                return RedirectToAction("Diem", "Class", new { id = malop });
            }
            var tn = db.BaiTapTNs.Where(c => c.NguoiNop.ToString().Equals(id) && c.BaiTap.MaLop.ToString().Equals(malop)).ToList();
            ViewData["baitracnghiem"] = tn;
            var tl = db.BaiTapTLs.Where(c => c.NguoiNop.ToString().Equals(id) && c.BaiTap.MaLop.ToString().Equals(malop)).ToList();
            ViewData["baituluan"] = tl;
            return View();
        }
        // thong tin bai tap da nop
        public ActionResult Bainop()
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            string nguoitao = user.TenDangNhap;
            string ml = Session["malop"].ToString();
            var bt = DOANTOTNGHIEP.Models.GetData.GetClass.getbaitapdanop(ml, nguoitao);
            return View("ShowBaiTap", bt);
        }
        //bai tap chua nop
        public ActionResult Baichuanop()
        {



            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            string nguoitao = user.TenDangNhap;
            string ml = Session["malop"].ToString();
            var bt = DOANTOTNGHIEP.Models.GetData.GetClass.getbaitapchuanop(ml, nguoitao);
            return View("ShowBaiTap", bt);
        }
        // bai tap nop muon

        public ActionResult Bainopmuon()
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            string nguoitao = user.TenDangNhap;
            string ml = Session["malop"].ToString();
            var bt = DOANTOTNGHIEP.Models.GetData.GetClass.getbaitapnopmuon(ml, nguoitao);
            return View("ShowBaiTap", bt);
        }

        //bagn thong tin tao lop
        public ActionResult AddClass()
        {

            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            return View();
        }
        //bang thong tin tham gia lop
        public ActionResult JoinClass()
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            return View();
        }
        // bang thong tin tat ca bai tap
        public ActionResult BaiTap(string id)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null && id == null) return RedirectToAction("Index", "TrangChu");
            if (id != null)
            {
                Session["malop"] = id;
            }
            var lop = db.ThanhVienLops.SingleOrDefault(x => x.MaLop.ToString().Equals(id) && x.Mathanhvien.Equals(user.TenDangNhap));
            if (lop == null)
            {
                return RedirectToAction("Index", "TrangChu");
            }
            string nguoitao = user.TenDangNhap;
            var lophoc = Session["lophoc"] as DOANTOTNGHIEP.Models.LopHoc;
            string ml = Session["malop"].ToString();
            var baitap = db.BaiTaps.Where(x => x.MaLop.ToString().Equals(ml)).OrderByDescending(x => x.ThoiGianDang).ToList();
            if (lophoc.NguoiTao.Equals(nguoitao))
            {
                return View("BaiTapTeacher", baitap);
            }

            return View(baitap);
        }
        public ActionResult BaiTapTeacher(List<BaiTap> baitap)
        {

            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            return View(baitap);
        }
        public ActionResult deletebaitap(string id)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            string malop = Session["malop"].ToString();
            var gv = db.LopHocs.SingleOrDefault(x => x.MaLop.ToString().Equals(malop) && x.NguoiTao.Equals(user.TenDangNhap));
            if (gv == null)
            {
                return RedirectToAction("BaiTap", new { id = malop });
            }
            var bt = db.BaiTaps.SingleOrDefault(c => c.MaBaiTap.ToString().Equals(id));
            if (bt != null)
            {
                if (bt.LoaiBaiTap.Equals("TracNghiem"))
                {

                    var tn = db.BaiTapTNs.Where(c => c.MaBaiTap.ToString().Equals(id)).ToList();
                    foreach (var bai in tn)
                    {
                        string mabai = bai.MaBaiNop.ToString();
                        var ttbai = db.TTBaiTapTNs.Where(x => x.MaBaiNop.ToString().Equals(mabai)).ToList();
                        db.TTBaiTapTNs.RemoveRange(ttbai);
                        db.SaveChanges();
                        db.BaiTapTNs.Remove(bai);
                        db.SaveChanges();
                    }
                    var thongbaobaitap = db.ThongBaos.SingleOrDefault(x => x.MaBaiTap.ToString().Equals(id));
                    db.ThongBaos.Remove(thongbaobaitap);
                    var cauhoi = db.CauHois.Where(x => x.MaBaiTap.ToString().Equals(id)).ToList();
                    foreach (var i in cauhoi)
                    {
                        db.DapAns.RemoveRange(i.DapAns.ToList());
                        db.SaveChanges();
                        db.CauHois.Remove(i);
                        db.SaveChanges();
                    }
                    db.BaiTaps.Remove(bt);
                    db.SaveChanges();

                }
                else if (bt.LoaiBaiTap.Equals("TuLuan"))
                {

                    var tn = db.BaiTapTLs.Where(c => c.MaBaiTap.ToString().Equals(id)).ToList();
                    foreach (var bai in tn)
                    {
                        string mabai = bai.MaBaiNop.ToString();
                        var ttbai = db.TTBaiTapTLs.Where(x => x.MaBaiNop.ToString().Equals(mabai)).ToList();
                        db.TTBaiTapTLs.RemoveRange(ttbai);
                        db.SaveChanges();
                        db.BaiTapTLs.Remove(bai);
                        db.SaveChanges();
                    }
                    var file = bt.FileBTTLs.ToList();
                    db.FileBTTLs.RemoveRange(file);
                    db.SaveChanges();
                    var thongbaobaitap = db.ThongBaos.SingleOrDefault(x => x.MaBaiTap.ToString().Equals(id));
                    db.ThongBaos.Remove(thongbaobaitap);
                    db.BaiTaps.Remove(bt);
                    db.SaveChanges();
                }
            }

            return BaiTap(malop);
        }
        public ActionResult Editbaitap(string id)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            string malop = Session["malop"].ToString();
            var gv = db.LopHocs.SingleOrDefault(x => x.MaLop.ToString().Equals(malop) && x.NguoiTao.Equals(user.TenDangNhap));
            if (gv == null)
            {
                return RedirectToAction("BaiTap", new { id = malop }); ;
            }
            string chude = Request.Form["Chude"];
            string noidung = Request.Unvalidated.Form["noidungbt"];
            string thoihan = Request.Form["thoigiankethuc"];

            var baitap = db.BaiTaps.SingleOrDefault(x => x.MaBaiTap.ToString().Equals(id));
            if (!baitap.ChuDe.Equals(chude) || !baitap.Thongtin.Equals(noidung) || !baitap.ThoiGianKetThuc.Equals(Convert.ToDateTime(thoihan)))
            {
                baitap.ChuDe = chude;
                baitap.Thongtin = noidung;
                baitap.ThoiGianKetThuc = Convert.ToDateTime(thoihan);
                baitap.ThoiGianDang = DateTime.Now;
                db.SaveChanges();
                var tb = db.ThongBaos.SingleOrDefault(x => x.MaBaiTap.ToString().Equals(id));
                tb.Thongtin = "Bài tập mới:" + chude;
                tb.NgayDang = DateTime.Now;
                db.SaveChanges();

            }

            return BaiTap(malop);
        }
        public ActionResult Editbaitaptl(string id, HttpPostedFileBase[] file)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            string malop = Session["malop"].ToString();
            var gv = db.LopHocs.SingleOrDefault(x => x.MaLop.ToString().Equals(malop) && x.NguoiTao.Equals(user.TenDangNhap));
            if (gv == null)
            {
                return RedirectToAction("BaiTap", new { id = malop });
            }
            string chude = Request.Form["Chude"];
            string noidung = Request.Unvalidated.Form["noidungbt"];
            string thoihan = Request.Form["thoigiankethuc"];
            var baitap = db.BaiTaps.SingleOrDefault(x => x.MaBaiTap.ToString().Equals(id));
            if (!baitap.ChuDe.Equals(chude) || !baitap.Thongtin.Equals(noidung) || !baitap.ThoiGianKetThuc.Equals(Convert.ToDateTime(thoihan)))
            {
                baitap.ChuDe = chude;
                baitap.Thongtin = noidung;
                baitap.ThoiGianKetThuc = Convert.ToDateTime(thoihan);
                baitap.ThoiGianDang = DateTime.Now;
                db.SaveChanges();
                var tb = db.ThongBaos.SingleOrDefault(x => x.MaBaiTap.ToString().Equals(id));
                tb.Thongtin = "Bài tập mới:" + chude;
                tb.NgayDang = DateTime.Now;
                db.SaveChanges();

            }
            foreach (var fil in file)
            {
                if (fil != null)
                {
                    var xoafiletb = db.FileBTTLs.Where(x => x.MaBt.ToString().Equals(id)).ToList();
                    if (xoafiletb != null)
                    {
                        db.FileBTTLs.RemoveRange(xoafiletb);
                        db.SaveChanges();
                        break;
                    }
                    break;


                }

            }
            foreach (var fil in file)
            {
                if (fil == null)
                {
                    break;
                }
                FileBTTL fbttl = new FileBTTL();
                fbttl.MaBt = baitap.MaBaiTap;
                fbttl.TenFile = fil.FileName;

                db.FileBTTLs.Add(fbttl);
                db.SaveChanges();
                try
                {
                    string path = Server.MapPath("~/Content/FileBTTL/" + fbttl.Mafile.ToString() + fil.FileName);
                    var ftb1 = db.FileBTTLs.SingleOrDefault(x => x.Mafile.ToString().Equals(fbttl.Mafile.ToString()));
                    ftb1.NoiLuu = "/Content/FileBTTL/" + fbttl.Mafile.ToString() + fil.FileName;
                    db.SaveChanges();
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    fil.SaveAs(path);

                }
                catch { }


            }

            var bt = db.BaiTaps.SingleOrDefault(x => x.MaBaiTap.ToString().Equals(id));
            return View("showcauhoituluan", bt);
        }


        public ActionResult Editbaitaptn(string id, HttpPostedFileBase[] file)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            string malop = Session["malop"].ToString();
            var gv = db.LopHocs.SingleOrDefault(x => x.MaLop.ToString().Equals(malop) && x.NguoiTao.Equals(user.TenDangNhap));
            if (gv == null)
            {
                return RedirectToAction("BaiTap", new { id = malop });
            }
            string chude = Request.Form["Chude"];
            string noidung = Request.Unvalidated.Form["noidungbt"];
            string thoihan = Request.Form["thoigiankethuc"];
            var baitap = db.BaiTaps.SingleOrDefault(x => x.MaBaiTap.ToString().Equals(id));
            if (!baitap.ChuDe.Equals(chude) || !baitap.Thongtin.Equals(noidung) || !baitap.ThoiGianKetThuc.Equals(Convert.ToDateTime(thoihan)))
            {
                baitap.ChuDe = chude;
                baitap.Thongtin = noidung;
                baitap.ThoiGianKetThuc = Convert.ToDateTime(thoihan);
                baitap.ThoiGianDang = DateTime.Now;
                db.SaveChanges();
                var tb = db.ThongBaos.SingleOrDefault(x => x.MaBaiTap.ToString().Equals(id));
                tb.Thongtin = "Bài tập mới:" + chude;
                tb.NgayDang = DateTime.Now;
                db.SaveChanges();

            }
            var cauhoi = db.CauHois.Where(x => x.MaBaiTap.ToString().Equals(id)).ToList();
            return RedirectToAction("ShowcauhoiTracnghiem", new { id = id });
        }


        public ActionResult ShowBaiTap(BaiTap baitap)
        {

            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            return View(baitap);
        }
        //chi tiec bai tap 
        public ActionResult ShowInforBaiTap(string id, string ma)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            string nguoitao = user.TenDangNhap;
            if (ma != null)
            {
                Session["malop"] = ma;
                Session["lophoc"] = db.LopHocs.SingleOrDefault(x => x.MaLop.ToString().Equals(ma));
            }

            var bt = db.BaiTaps.SingleOrDefault(c => c.MaBaiTap.ToString().Equals(id));
            if (bt != null)
            {


                if (bt.NguoiTao.Equals(nguoitao))
                {
                    Session["mabaitap"] = id;

                    return RedirectToAction("ShowInforBaiTapForTeacher");
                }
                else if (!bt.NguoiTao.Equals(nguoitao))
                {
                    if (bt.LoaiBaiTap.Equals("TracNghiem"))
                    {

                        var tn = db.BaiTapTNs.SingleOrDefault(c => c.MaBaiTap.ToString().Equals(id) && c.NguoiNop.Equals(nguoitao));
                        if (tn == null)
                        {
                            BaiTapTN btn = new BaiTapTN();
                            btn.MaBaiTap = long.Parse(id);
                            btn.NguoiNop = nguoitao;
                            db.BaiTapTNs.Add(btn);
                            db.SaveChanges();
                            tn = db.BaiTapTNs.SingleOrDefault(c => c.MaBaiTap.ToString().Equals(id) && c.NguoiNop.Equals(nguoitao));

                        }
                        return View("ShowInforBaiTapTn", tn);
                    }
                    else if (bt.LoaiBaiTap.Equals("TuLuan"))
                    {

                        var tn = db.BaiTapTLs.SingleOrDefault(c => c.MaBaiTap.ToString().Equals(id) && c.NguoiNop.Equals(nguoitao));

                        if (tn == null)
                        {
                            BaiTapTL btn = new BaiTapTL();
                            btn.MaBaiTap = long.Parse(id);
                            btn.NguoiNop = nguoitao;
                            db.BaiTapTLs.Add(btn);
                            db.SaveChanges();
                            tn = db.BaiTapTLs.SingleOrDefault(c => c.MaBaiTap.ToString().Equals(id) && c.NguoiNop.Equals(nguoitao));
                        }
                        Session["mabaitaptl"] = tn.MaBaiNop;
                        return View("ShowInforBaiTapTL", tn);
                    }
                }
            }
            return RedirectToAction("BaiTap", new { id = ma });
        }
        public ActionResult ShowInforBaiTapForTeacher()
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            var id = Session["mabaitap"].ToString();
            string malop = Session["malop"].ToString();
            var gv = db.LopHocs.SingleOrDefault(x => x.MaLop.ToString().Equals(malop) && x.NguoiTao.Equals(user.TenDangNhap));
            if (gv == null)
            {
                return RedirectToAction("BaiTap", new { id = malop });
            }
            var bt = db.BaiTaps.SingleOrDefault(c => c.MaBaiTap.ToString().Equals(id));
            if (bt.LoaiBaiTap.Equals("TracNghiem"))
            {

                var tn = db.BaiTapTNs.Where(c => c.MaBaiTap.ToString().Equals(id)).OrderBy(x => x.TaiKhoan.Ten).ToList();
                ViewData["baitracnghiem"] = tn;
                ViewBag.excel = exceldsdiembttn(tn);
                ViewBag.pdf = pdfdsdiembttn(tn);
                return View("ShowInforBaiTapForTeacher");
            }
            else if (bt.LoaiBaiTap.Equals("TuLuan"))
            {

                var tn = db.BaiTapTLs.Where(c => c.MaBaiTap.ToString().Equals(id)).OrderBy(x => x.TaiKhoan.Ten).ToList();
                ViewData["baituluan"] = tn;
                ViewBag.excel = exceldsdiembttl(tn);
                ViewBag.pdf = pdfdsdiembttl(tn);
                return View("ShowInforBaiTapForTeacher");
            }
            return RedirectToAction("BaiTap", new { id = malop });
        }
        public ActionResult ShowInforTL(string ten, string ma)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            string malop = Session["malop"].ToString();
            if (ten == null || ma == null)
            {
                return BaiTap(malop);
            }
            var bt = db.BaiTapTLs.SingleOrDefault(x => x.MaBaiNop.ToString().Equals(ma) && x.NguoiNop.Equals(ten));
            if (bt == null) return BaiTap(malop);
            Session["mabainop"] = bt.MaBaiNop;
            Session["nguoinop"] = bt.NguoiNop;
            Session["mabaitaptl"] = ma;
            return View("ShowInforBaiTapTL", bt);
        }
        public ActionResult ShowInforTN(string ten, string ma)
        {

            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            string malop = Session["malop"].ToString();
            if (ten == null || ma == null)
            {
                return BaiTap(malop);
            }
            var bt = db.BaiTapTNs.SingleOrDefault(x => x.MaBaiNop.ToString().Equals(ma) && x.NguoiNop.Equals(ten));
            if (bt == null) return BaiTap(malop);
            Session["mabainop"] = bt.MaBaiNop;
            Session["nguoinop"] = bt.NguoiNop;
            Session["mabaitaptl"] = ma;
            return View("ShowInforBaiTapTn", bt);
        }
        public ActionResult chambaitl(BaiTapTL baitap)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            string ma = Session["mabainop"].ToString();
            string ten = Session["nguoinop"].ToString();
            var bt = db.BaiTapTLs.SingleOrDefault(x => x.MaBaiNop.ToString().Equals(ma));
            bt.Diem = baitap.Diem;
            db.SaveChanges();
            var baitap1 = db.BaiTapTLs.SingleOrDefault(x => x.MaBaiNop.ToString().Equals(ma) && x.NguoiNop.Equals(ten));
            return View("ShowInforTL", baitap1);
        }



        public ActionResult ThanhVien(string id)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (id != null)
            {
                Session["malop"] = id;
            }
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");

            var lop = db.ThanhVienLops.SingleOrDefault(x => x.MaLop.ToString().Equals(id) && x.Mathanhvien.Equals(user.TenDangNhap));
            if (lop == null)
            {
                return RedirectToAction("Index", "TrangChu");
            }
            var thanhvien = db.ThanhVienLops.Where(x => x.MaLop.ToString().Equals(id)).OrderBy(x => x.TaiKhoan.Ten).ToList();

            return View(thanhvien);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult DangTB(HttpPostedFileBase[] file)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            string nguoitao = user.TenDangNhap;
            string noidung = Request.Unvalidated.Form["noidung"].ToString();
            ThongBao tb = new ThongBao();
            tb.Thongtin = noidung;
            tb.NgayDang = DateTime.Now;
            tb.NguoiDang = nguoitao;
            tb.MaLop = long.Parse((string)Session["malop"]);
            string malop = Session["malop"].ToString();
            db.ThongBaos.Add(tb);
            db.SaveChanges();

            foreach (var fil in file)
            {
                if (fil == null)
                {
                    break;
                }
                FileTB ftb = new FileTB();
                ftb.maTB = tb.MaBaiDang;
                ftb.TenFile = fil.FileName;
                db.FileTBs.Add(ftb);
                db.SaveChanges();
                try
                {
                    string path = Server.MapPath("~/Content/FileTB/" + ftb.Mafile.ToString() + fil.FileName);
                    var ftb1 = db.FileTBs.SingleOrDefault(x => x.Mafile.ToString().Equals(ftb.Mafile.ToString()));
                    ftb1.NoiLuu = "/Content/FileTB/" + ftb.Mafile.ToString() + fil.FileName;
                    db.SaveChanges();

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    fil.SaveAs(path);

                }
                catch { }

            }
            var thanhvien = db.ThanhVienLops.Where(x => x.MaLop.ToString().Equals(tb.MaLop.ToString()) && !x.Mathanhvien.Equals(nguoitao)).ToList();
            foreach (var tv in thanhvien)
            {
                EmailController.SendEmail(tv.TaiKhoan.Email, "Thông báo mới trong Lớp :" + (Session["lophoc"] as LopHoc).TenLop + "  của giáo viên " + (Session["lophoc"] as LopHoc).TaiKhoan.Ho + " " + (Session["lophoc"] as LopHoc).TaiKhoan.Ten, tb.Thongtin);
            }

            var thongbao = db.ThongBaos.Where(x => x.MaLop.ToString().Equals(malop)).OrderByDescending(y => y.NgayDang).ToList();
            Session["lophoc"] = db.LopHocs.SingleOrDefault(y => y.MaLop.ToString().Equals(malop));
            return RedirectToAction("Index", new { id = malop });
        }
        [HttpPost]
        public ActionResult DangBaiTapTL(HttpPostedFileBase[] file)
        {

            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            string malop = Session["malop"].ToString();
            var gv = db.LopHocs.SingleOrDefault(x => x.MaLop.ToString().Equals(malop) && x.NguoiTao.Equals(user.TenDangNhap));
            if (gv == null)
            {
                return RedirectToAction("BaiTap", new { id = malop });
            }
            string nguoitao = user.TenDangNhap;
            string noidung = Request.Unvalidated.Form["noidungbt"];
            string chude = Request.Form["Chude"];
            string thoigianketthuc = Request.Form["thoigiankethuc"].ToString();
            DateTime dt = Convert.ToDateTime(thoigianketthuc);
            BaiTap bt = new BaiTap();
            bt.Thongtin = noidung;
            bt.ThoiGianDang = DateTime.Now;
            bt.ThoiGianKetThuc = dt;
            bt.NguoiTao = nguoitao;
            bt.LoaiBaiTap = "TuLuan";
            bt.ChuDe = chude;
            bt.MaLop = long.Parse((string)Session["malop"]);
            db.BaiTaps.Add(bt);
            db.SaveChanges();
            ThongBao tb = new ThongBao();
            tb.NguoiDang = nguoitao;
            tb.NgayDang = DateTime.Now;
            tb.MaBaiTap = bt.MaBaiTap;
            tb.MaLop = long.Parse((string)Session["malop"]);
            tb.Thongtin = "Bài tập mới:" + chude;
            db.ThongBaos.Add(tb);
            db.SaveChanges();

            foreach (var fil in file)
            {
                if (fil == null)
                {
                    break;
                }
                FileBTTL fbttl = new FileBTTL();
                fbttl.MaBt = bt.MaBaiTap;
                fbttl.TenFile = fil.FileName;

                db.FileBTTLs.Add(fbttl);
                db.SaveChanges();
                try
                {
                    string path = Server.MapPath("~/Content/FileBTTL/" + fbttl.Mafile.ToString() + fil.FileName);
                    var ftb1 = db.FileBTTLs.SingleOrDefault(x => x.Mafile.ToString().Equals(fbttl.Mafile.ToString()));
                    ftb1.NoiLuu = "/Content/FileBTTL/" + fbttl.Mafile.ToString() + fil.FileName;
                    db.SaveChanges();
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    fil.SaveAs(path);

                }
                catch { }


            }
            string id = Session["malop"].ToString();
            var lophoc = db.LopHocs.SingleOrDefault(x => x.MaLop.ToString().Equals(id));
            var thanhvien = db.ThanhVienLops.Where(x => x.MaLop.ToString().Equals(id)).ToList();
            foreach (var tv in thanhvien)
            {
                if (!tv.Mathanhvien.Equals(lophoc.NguoiTao))
                {
                    BaiTapTL btl = new BaiTapTL();
                    btl.MaBaiTap = bt.MaBaiTap;
                    btl.NguoiNop = tv.Mathanhvien;

                    db.BaiTapTLs.Add(btl);
                    db.SaveChanges();

                    EmailController.SendEmail(tv.TaiKhoan.Email, "Bài tâp mới trong Lớp :" + (Session["lophoc"] as LopHoc).TenLop + " của giáo viên " + (Session["lophoc"] as LopHoc).TaiKhoan.Ho + " " + (Session["lophoc"] as LopHoc).TaiKhoan.Ten, "\n.Chủ đề :" + bt.ChuDe + "\n. Nội dung :" + bt.Thongtin);
                }

            }

            return RedirectToAction("BaiTap", new { id = malop });
        }
       
        [HttpPost]
        public ActionResult DangBaiTapTN(HttpPostedFileBase file)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            var malop = Session["malop"].ToString();
            var gv = db.LopHocs.SingleOrDefault(x => x.MaLop.ToString().Equals(malop) && x.NguoiTao.Equals(user.TenDangNhap));
            if (gv == null)
            {
                return RedirectToAction("BaiTap", new { id = malop });
            }
            string nguoitao = user.TenDangNhap;
            string noidung = Request.Unvalidated.Form["noidungbt"];
            string chude = Request.Form["Chude"];
            string thoigianketthuc = Request.Form["thoigiankethuc"].ToString();
            DateTime dt = Convert.ToDateTime(thoigianketthuc);
            BaiTap bt = new BaiTap();
            bt.Thongtin = noidung;
            bt.ThoiGianDang = DateTime.Now;
            bt.ThoiGianKetThuc = dt;
            bt.NguoiTao = nguoitao;
            bt.LoaiBaiTap = "TracNghiem";
            bt.ChuDe = chude;
            bt.MaLop = long.Parse((string)Session["malop"]);
            Session["detracnghiem"] = bt;
            object path = Server.MapPath("~/Content/FileBTTN/" + file.FileName);
            if (System.IO.File.Exists(path.ToString()))
            {
                System.IO.File.Delete(path.ToString());
            }

            file.SaveAs(path.ToString());

            if (docfiletracnghiem(path.ToString()).Count == 0)
            {
                if (System.IO.File.Exists(path.ToString()))
                {
                    System.IO.File.Delete(path.ToString());
                }
                return BaiTap(malop);
            }
            var cauhoi = docfiletracnghiem(path.ToString());
            Session["cauhoitracnghiem"] = cauhoi;
            return View("SaveCauhoitracnghiem", cauhoi);
        }
        public List<CauHoi> docfiletracnghiem(string file)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            string nguoitao = user.TenDangNhap;
            List<CauHoi> cauhoi = new List<CauHoi>();


            Document document = new Document(file);

            int sas = 1;
            Section section = document.Sections[0];
            if (section.Tables.Count > 0)
            {
                Table table = section.Tables[0] as Table;
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    CauHoi cau = new CauHoi();
                    for (int j = 0; j < table.Rows[i].Cells.Count; j++)
                    {

                        foreach (Paragraph paragraph in table.Rows[i].Cells[j].Paragraphs)
                        {
                            string noidung = "";
                            DapAn da = new DapAn();
                            //Get Each Document Object of Paragraph Items

                            foreach (DocumentObject docObject in paragraph.ChildObjects)
                            {

                                //If Type of Document Object is Picture, Extract.  
                                if (docObject.DocumentObjectType == DocumentObjectType.Picture)
                                {
                                    String anh = null;
                                    string s = nguoitao + "-" + sas + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
                                    DocPicture pic = docObject as DocPicture;
                                    String imgName = Server.MapPath("~/Content/image/ImageCauhoiTracnghiem/" + s);
                                    anh = "/Content/image/ImageCauhoiTracnghiem/" + s;
                                    //Save Image  
                                    pic.Image.Save(imgName, System.Drawing.Imaging.ImageFormat.Png);
                                    noidung = noidung + "   <img src='" + anh + "'>  ";
                                    sas++;
                                }
                                else if (docObject.DocumentObjectType == DocumentObjectType.TextRange)
                                {

                                    TextRange nd = docObject as TextRange;

                                    noidung = noidung + nd.Text;




                                }
                                else if (docObject.DocumentObjectType == DocumentObjectType.OfficeMath)
                                {

                                    noidung = noidung + (docObject as OfficeMath).ToMathMLCode().Replace("mml:", "");

                                }




                            }
                            if (j == 0)
                            {
                                cau.NoiDung = noidung;

                            }
                            else if (j != 0)
                            {
                                if (!noidung.ToString().Equals(""))
                                {
                                    da.NoiDung = noidung.ToString();


                                    if (noidung.Substring(0, noidung.ToString().IndexOf("*") + 1).Replace(" ", "").Equals("*"))
                                    {
                                        da.NoiDung = noidung.ToString().Substring(noidung.ToString().IndexOf("*") + 1, noidung.ToString().Length - noidung.ToString().IndexOf("*") - 1);
                                        da.LoaiDapAn = true;
                                    }
                                    else
                                    {

                                        da.LoaiDapAn = false;
                                    }


                                    cau.DapAns.Add(da);
                                }

                            }

                        }

                    }
                    cauhoi.Add(cau);

                }
            }

            return cauhoi;
        }
        public ActionResult luucauhoitracnghiem()
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            var malop = Session["malop"].ToString();
            string nguoitao = user.TenDangNhap;
            var bt = Session["detracnghiem"] as DOANTOTNGHIEP.Models.BaiTap;
            db.BaiTaps.Add(bt);
            db.SaveChanges();
            ThongBao tb = new ThongBao();
            tb.NguoiDang = nguoitao;
            tb.NgayDang = DateTime.Now;
            tb.MaBaiTap = bt.MaBaiTap;
            tb.MaLop = long.Parse((string)Session["malop"]);
            tb.Thongtin = "Bài tập mới:" + bt.ChuDe;
            db.ThongBaos.Add(tb);
            db.SaveChanges();

            var cauhoi = Session["cauhoitracnghiem"] as List<DOANTOTNGHIEP.Models.CauHoi>;
            foreach (var i in cauhoi)
            {
                CauHoi ch = new CauHoi();
                ch.MaBaiTap = bt.MaBaiTap;

                ch.NgayThem = DateTime.Now;
                ch.NoiDung = i.NoiDung;
                db.CauHois.Add(ch);
                db.SaveChanges();
                foreach (var j in i.DapAns.ToList())
                {
                    DapAn dapAn = new DapAn();
                    dapAn.MaCauHoi = ch.MaCauHoi;
                    dapAn.NoiDung = j.NoiDung;

                    dapAn.LoaiDapAn = j.LoaiDapAn;
                    db.DapAns.Add(dapAn);
                    db.SaveChanges();

                }
            }
            string id = Session["malop"].ToString();
            var lophoc = db.LopHocs.SingleOrDefault(x => x.MaLop.ToString().Equals(id));
            var thanhvien = db.ThanhVienLops.Where(x => x.MaLop.ToString().Equals(id)).ToList();

            foreach (var tv in thanhvien)
            {
                if (!tv.Mathanhvien.Equals(lophoc.NguoiTao))
                {
                    BaiTapTN btn = new BaiTapTN();
                    btn.MaBaiTap = bt.MaBaiTap;
                    btn.NguoiNop = tv.Mathanhvien;

                    db.BaiTapTNs.Add(btn);
                    db.SaveChanges();
                    EmailController.SendEmail(tv.TaiKhoan.Email, "Bài tâptrong Lớp :" + (Session["lophoc"] as LopHoc).TenLop + " của giáo viên " + (Session["lophoc"] as LopHoc).TaiKhoan.Ho + " " + (Session["lophoc"] as LopHoc).TaiKhoan.Ten, "\n.Chủ đề :" + bt.ChuDe + "\n. Nội dung :" + bt.Thongtin);
                }

            }
            Session.Remove("detracnghiem");
            return RedirectToAction("BaiTap", new { id = malop });
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditThongBao(string id, HttpPostedFileBase[] file)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            string malop = Session["malop"].ToString();
            string noidung = Request.Unvalidated.Form["suanoidungthongbao"];

            ThongBao tb = db.ThongBaos.SingleOrDefault(x => x.MaBaiDang.ToString().Equals(id) && x.NguoiDang.Equals(user.TenDangNhap));
            if (tb == null)
            {
                return RedirectToAction("Index", new { id = malop });
            }
            tb.Thongtin = noidung;
            tb.NgayDang = DateTime.Now;

            db.SaveChanges();
            foreach (var fil in file)
            {
                if (fil != null)
                {
                    var xoafiletb = db.FileTBs.Where(x => x.maTB.ToString().Equals(id)).ToList();
                    if (xoafiletb != null)
                    {
                        db.FileTBs.RemoveRange(xoafiletb);
                        db.SaveChanges();
                        break;
                    }
                    break;


                }

            }
            foreach (var fil in file)
            {
                if (fil == null)
                {
                    break;
                }

                FileTB ftb = new FileTB();
                ftb.maTB = tb.MaBaiDang;
                ftb.TenFile = fil.FileName;
                db.FileTBs.Add(ftb);
                db.SaveChanges();
                try
                {
                    string path = Server.MapPath("~/Content/FileTB/" + ftb.Mafile.ToString() + fil.FileName);
                    var ftb1 = db.FileTBs.SingleOrDefault(x => x.Mafile.ToString().Equals(ftb.Mafile.ToString()));
                    ftb1.NoiLuu = "/Content/FileTB/" + ftb.Mafile.ToString() + fil.FileName;
                    db.SaveChanges();

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    fil.SaveAs(path);

                }
                catch { }

            }
            var thongbao = db.ThongBaos.Where(x => x.MaLop.ToString().Equals(malop)).OrderByDescending(y => y.NgayDang).ToList();
            ViewData["lophoc"] = db.LopHocs.SingleOrDefault(x => x.MaLop.ToString().Equals(malop));
            return RedirectToAction("Index", new { id = malop });
        }
        public ActionResult DeleteThongBao(string id)
        {

            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            string malop = Session["malop"].ToString();
            string noidung = Request.Form["suanoidungthongbao"];
            ThongBao tb = db.ThongBaos.SingleOrDefault(x => x.MaBaiDang.ToString().Equals(id) && x.NguoiDang.Equals(user.TenDangNhap));
            if (tb == null)
            {
                return RedirectToAction("Index", new { id = malop });
            }
            var ftb = db.FileTBs.Where(x => x.maTB.ToString().Equals(id)).ToList();
            foreach (var f in ftb)
            {
                db.FileTBs.Remove(f);
                db.SaveChanges();
            }

            db.ThongBaos.Remove(tb);
            db.SaveChanges();

            var thongbao = db.ThongBaos.Where(x => x.MaLop.ToString().Equals(malop)).OrderByDescending(y => y.NgayDang).ToList();
            ViewData["lophoc"] = db.LopHocs.SingleOrDefault(x => x.MaLop.ToString().Equals(malop));
            return RedirectToAction("Index", new { id = malop });
        }
        [HttpPost]
        public ActionResult editnopbaitap(HttpPostedFileBase[] file)
        {

            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            var malop = Session["malop"].ToString();
            string nguoitao = user.TenDangNhap;
            var mabaitaptl = Session["mabaitaptl"].ToString();
            var bai = db.TTBaiTapTLs.Where(x => x.MaBaiNop.ToString().Equals(mabaitaptl)).ToList();
            db.TTBaiTapTLs.RemoveRange(bai);
            db.SaveChanges();
            foreach (var fil in file)
            {
                if (fil == null)
                {
                    break;
                }

                TTBaiTapTL bttl = new TTBaiTapTL();
                bttl.MaBaiNop = long.Parse(mabaitaptl);
                bttl.NgayNop = DateTime.Now;
                bttl.NguoiNop = nguoitao;
                db.TTBaiTapTLs.Add(bttl);
                var bt = db.BaiTapTLs.SingleOrDefault(x => x.MaBaiNop.ToString().Equals(mabaitaptl));
                bt.NgayNop = DateTime.Now;
                bt.Trangthai = true;
                db.SaveChanges();
                try
                {
                    string path = Server.MapPath("~/Content/BTTL/" + bttl.Ma.ToString() + fil.FileName);
                    var ftb1 = db.TTBaiTapTLs.SingleOrDefault(x => x.Ma.ToString().Equals(bttl.Ma.ToString()));
                    ftb1.Tenfile = fil.FileName;
                    ftb1.NoiLuu = "/Content/BTTL/" + bttl.Ma.ToString() + fil.FileName;
                    db.SaveChanges();
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    fil.SaveAs(path);

                }
                catch { }


            }


            return RedirectToAction("BaiTap", new { id = malop });
        }
        [HttpPost]
        public ActionResult nopbaitapTL(HttpPostedFileBase[] file)
        {

            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            var malop = Session["malop"].ToString();
            string nguoitao = user.TenDangNhap;

            foreach (var fil in file)
            {
                if (fil == null)
                {
                    break;
                }
                var mabaitaptl = Session["mabaitaptl"].ToString();
                TTBaiTapTL bttl = new TTBaiTapTL();
                bttl.MaBaiNop = long.Parse(mabaitaptl);
                bttl.NgayNop = DateTime.Now;
                bttl.NguoiNop = nguoitao;
                db.TTBaiTapTLs.Add(bttl);
                var bt = db.BaiTapTLs.SingleOrDefault(x => x.MaBaiNop.ToString().Equals(mabaitaptl));
                bt.NgayNop = DateTime.Now;
                bt.Trangthai = true;
                db.SaveChanges();
                try
                {
                    string path = Server.MapPath("~/Content/BTTL/" + bttl.Ma.ToString() + fil.FileName);
                    var ftb1 = db.TTBaiTapTLs.SingleOrDefault(x => x.Ma.ToString().Equals(bttl.Ma.ToString()));
                    ftb1.Tenfile = fil.FileName;
                    ftb1.NoiLuu = "/Content/BTTL/" + bttl.Ma.ToString() + fil.FileName;
                    db.SaveChanges();
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    fil.SaveAs(path);

                }
                catch { }


            }


            return RedirectToAction("BaiTap", new { id = malop });
        }
        public ActionResult LamBaiTN(string id)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            Session["mabai"] = id;
            var cauhoi = db.CauHois.Where(x => x.MaBaiTap.ToString().Equals(id)).ToList();
            return View(cauhoi);
        }
        public void Luucauhoikhilam(string macauhoi, string madapan)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) RedirectToAction("Login", "Login");
            if (Session["malop"] == null) RedirectToAction("Index", "TrangChu");
            string nguoitao = user.TenDangNhap;
            string mabaitap = Session["mabai"].ToString();
            var bai = db.BaiTapTNs.SingleOrDefault(x => x.MaBaiTap.ToString().Equals(mabaitap) && x.NguoiNop.Equals(nguoitao));
            var s = db.TTBaiTapTNs.SingleOrDefault(x => x.MaBaiNop.ToString().Equals(bai.MaBaiNop.ToString()) && x.NguoiNop.Equals(nguoitao) && x.MaCauHoi.ToString().Equals(macauhoi));
            if (s == null)
            {
                TTBaiTapTN tn = new TTBaiTapTN();
                tn.MaBaiNop = bai.MaBaiNop;
                tn.MaCauHoi = long.Parse(macauhoi);
                tn.MaDapAnluaChon = long.Parse(madapan);
                tn.NguoiNop = nguoitao;
                db.TTBaiTapTNs.Add(tn);
                db.SaveChanges();
            }
            else
            {
                s.MaCauHoi = long.Parse(macauhoi);
                s.MaDapAnluaChon = long.Parse(madapan);
                db.SaveChanges();

            }





        }
        //luu bai thi sau khi lam
        public ActionResult LuuBaiTN()
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            string nguoitao = user.TenDangNhap;
            string mabaitap = Session["mabai"].ToString();
            var bai = db.BaiTapTNs.SingleOrDefault(x => x.MaBaiTap.ToString().Equals(mabaitap) && x.NguoiNop.Equals(nguoitao));
            var s = db.TTBaiTapTNs.Where(x => x.MaBaiNop.ToString().Equals(bai.MaBaiNop.ToString()) && x.NguoiNop.Equals(nguoitao)).ToList();
            var cauhoi = db.CauHois.Where(x => x.MaBaiTap.ToString().Equals(mabaitap)).ToList();
            int slcaudung = 0;
            foreach (var i in s)
            {
                if (i.DapAn.LoaiDapAn.Value)
                {
                    slcaudung++;
                }
            }
            bai.Diem = ((100 / cauhoi.Count) * slcaudung);
            if (slcaudung == cauhoi.Count)
            {
                bai.Diem = 100;
            }
            bai.Trangthai = true;
            bai.NgayNop = DateTime.Now;
            db.SaveChanges();
            return ShowInforBaiTap(mabaitap, null);
        }
        public ActionResult ShowBaiThiTN(string id)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            var malop = Session["malop"].ToString();
            var bai = db.BaiTapTNs.SingleOrDefault(x => x.MaBaiNop.ToString().Equals(id) && (x.NguoiNop.Equals(user.TenDangNhap) || x.BaiTap.NguoiTao.Equals(user.TenDangNhap)));
            if (bai == null)
            {
                return RedirectToAction("BaiTap", new { id = malop });
            }
            ViewData["ttbaitt"] = db.TTBaiTapTNs.Where(x => x.MaBaiNop.ToString().Equals(id)).ToList();
            var cauhoi = db.CauHois.Where(x => x.MaBaiTap.ToString().Equals(bai.MaBaiTap.ToString())).ToList();
            return View(cauhoi);
        }
        public ActionResult ShowcauhoiTracnghiem(string id)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            var malop = Session["malop"].ToString();
            var bt = db.BaiTaps.SingleOrDefault(x => x.MaBaiTap.ToString().Equals(id) && x.NguoiTao.Equals(user.TenDangNhap));
            if (bt == null)
            {
                return RedirectToAction("BaiTap", new { id = malop });
            }
            var cauhoi = db.CauHois.Where(x => x.MaBaiTap.ToString().Equals(id)).ToList();
            return View(cauhoi);
        }
        public ActionResult showcauhoituluan(string id)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            var malop = Session["malop"].ToString();
            var bt = db.BaiTaps.SingleOrDefault(x => x.MaBaiTap.ToString().Equals(id) && x.NguoiTao.Equals(user.TenDangNhap));
            if (bt == null)
            {
                return BaiTap(malop);
            }
            return View(bt);
        }

        public ActionResult Mess(string id)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            Session["malop"] = id;
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            var lop = db.ThanhVienLops.SingleOrDefault(x => x.MaLop.ToString().Equals(id) && x.Mathanhvien.Equals(user.TenDangNhap));
            if (lop == null)
            {
                return RedirectToAction("Index", "TrangChu");
            }
            var thanhvienlop = db.ThanhVienLops.Where(x => x.MaLop.ToString().Equals(id)).OrderBy(x => x.TaiKhoan.Ten).ToList();

            var thanhvien1 = db.ThanhVienLops.Where(x => x.MaLop.ToString().Equals(id)).OrderBy(x => x.TaiKhoan.Ten).ToList();

            var thanhvien2 = db.ThanhVienLops.Where(x => x.MaLop.ToString().Equals(id)).OrderBy(x => x.TaiKhoan.Ten).ToList();

            List<Mess> tin11 = new List<Mess>();
            foreach (var i in thanhvienlop)
            {
                var mess1 = db.Messes.Where(x => ((x.NguoiNhan.Equals(user.TenDangNhap) && x.NguoiGui.Equals(i.Mathanhvien))) && x.malop.ToString().Equals(id)).OrderByDescending(x => x.thoigiangui.Value).ToList();
                if (mess1.Count > 0)
                {
                    tin11.Add(mess1[0]);
                }

            }
            foreach (var i in thanhvienlop)
            {
                var mess1 = db.Messes.Where(x => ((x.NguoiNhan.Equals(i.Mathanhvien) && x.NguoiGui.Equals(user.TenDangNhap))) && x.malop.ToString().Equals(id)).OrderByDescending(x => x.thoigiangui.Value).ToList();
                if (mess1.Count > 0)
                {
                    tin11.Add(mess1[0]);
                }

            }
            for (int i = 0; i < tin11.Count; i++)
                for (int j = 0; j < tin11.Count - 1; j++)
                    if (tin11[j].thoigiangui.Value > tin11[j + 1].thoigiangui.Value)
                    {
                        var a = tin11[j];
                        tin11[j] = tin11[j + 1];
                        tin11[j + 1] = a;

                    }

            List<ThanhVienLop> tv11 = new List<ThanhVienLop>();
            foreach (var i in tin11)
            {
                var s = i.NguoiGui;
                var t = i.thoigiangui;
                var tv1 = db.ThanhVienLops.SingleOrDefault(x => x.Mathanhvien.Equals(i.NguoiGui) && x.MaLop.ToString().Equals(id));
                tv11.Add(tv1);
                var tv2 = db.ThanhVienLops.SingleOrDefault(x => x.Mathanhvien.Equals(i.NguoiNhan) && x.MaLop.ToString().Equals(id));
                tv11.Add(tv2);
            }
            foreach (var j in tv11)
            {
                foreach (var i in thanhvien2)
                {

                    if (i.Mathanhvien.Equals(j.Mathanhvien))
                    {
                        thanhvienlop.Remove(j);
                        thanhvienlop.Insert(0, j);

                        break;
                    }
                }

            }







            return View(thanhvienlop);
        }
        //hien tin nhan trong csdl
        [HttpPost]
        public ActionResult InforMess(string id)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            string nguoitao = user.TenDangNhap;
            string malop = Session["malop"].ToString();
            var taikhoan = db.TaiKhoans.SingleOrDefault(x => x.TenDangNhap.Equals(id));
            Session["nguoinhan"] = taikhoan;
            ViewData["tinnhan"] = db.Messes.Where(x => x.malop.ToString().Equals(malop) && ((x.NguoiGui.Equals(nguoitao) && x.NguoiNhan.Equals(id)) || (x.NguoiGui.Equals(id) && x.NguoiNhan.Equals(nguoitao)))).OrderBy(y => y.thoigiangui).ToList();

            return PartialView(taikhoan);
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase uploadedFiles)
        {
            string returnImagePath = string.Empty;
            string fileName;
            string Extension;
            string imageName;
            string imageSavePath;

            if (uploadedFiles.ContentLength > 0)
            {
                fileName = Path.GetFileNameWithoutExtension(uploadedFiles.FileName);
                Extension = Path.GetExtension(uploadedFiles.FileName);
                imageName = fileName + DateTime.Now.ToString("yyyyMMddHHmmss");
                imageSavePath = Server.MapPath("~/Content/image/imagetinnhan/") + imageName +
Extension;

                uploadedFiles.SaveAs(imageSavePath);
                returnImagePath = "/Content/image/imagetinnhan/" + imageName +
Extension;
            }

            return Json(Convert.ToString(returnImagePath), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        public void SaveInforMess(string nguoinhan, string tinnhan)
        {
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            string nguoitao = user.TenDangNhap;
            string malop = Session["malop"].ToString();
            if (!tinnhan.Equals(""))
            {
                DOANTOTNGHIEP.Models.Mess mess = new Mess();
                mess.NguoiGui = nguoitao;
                mess.NguoiNhan = nguoinhan;
                mess.malop = long.Parse(malop);
                mess.TinNhan = tinnhan;
                mess.thoigiangui = DateTime.Now;
                db.Messes.Add(mess);
                db.SaveChanges();
            }


        }
        [HttpPost]
        public void addtoken(string token, string user)
        {
            var tk = db.TaiKhoans.SingleOrDefault(x => x.TenDangNhap.Equals(user));
            tk.token = token;
            db.SaveChanges();

        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult editcauhoitracnghiem(string id)
        {

            var cauhoi = Request.Unvalidated.Form["cauhoi-" + id].ToString();
            var dbcauhoi = db.CauHois.SingleOrDefault(x => x.MaCauHoi.ToString().Equals(id));
            dbcauhoi.NoiDung = cauhoi;
            var dapan1 = Request.Unvalidated.Form["dapan-" + id].ToString();
            foreach (var dapan in dbcauhoi.DapAns.ToList())
            {
                if (dapan.MaDapAn.ToString().Equals(dapan1))
                {
                    dapan.LoaiDapAn = true;
                }
                else dapan.LoaiDapAn = false;

                var da = Request.Unvalidated.Form["dapan-" + dapan.MaDapAn].ToString();
                dapan.NoiDung = da;
                db.SaveChanges();
            }
            db.SaveChanges();
            var dscauhoi = db.CauHois.Where(x => x.MaBaiTap.ToString().Equals(dbcauhoi.BaiTap.MaBaiTap.ToString())).ToList();
            return View("ShowcauhoiTracnghiem", dscauhoi);
        }
        [HttpPost]
        public JsonResult inviteclass(string email)
        {
            var ema = email;
            string malop = Session["malop"].ToString();
            var tk = db.TaiKhoans.SingleOrDefault(x => x.Email.Equals(email));

            if (tk != null)
            {
                var thanhvienlop = db.ThanhVienLops.SingleOrDefault(x => x.Mathanhvien.Equals(tk.TenDangNhap) && x.MaLop.ToString().Equals(malop));
                if (thanhvienlop == null)
                {

                    var lm = db.Loimois.SingleOrDefault(x => x.MaLop.ToString().Equals(malop) && x.TenDangNhap.Equals(tk.TenDangNhap));
                    if (lm == null)
                    {
                        Loimoi loimoi = new Loimoi();
                        loimoi.MaLop = long.Parse((string)Session["malop"]);
                        loimoi.TenDangNhap = tk.TenDangNhap;
                        db.Loimois.Add(loimoi);
                        db.SaveChanges();
                        return Json("<label class='result' >Đã  gửi lời mời .</label>");
                    }
                    else return Json("<label style='color: red' class='result' >Đã gửi trước đó.</label>");



                }
                else
                {
                    return Json("<label style='color: red' class='result' >tài khoản đã tham gia lớp học trước đó.</label>");

                }
            }
            else
            {
                return Json("<label style='color: red' class='result'>Tài khoản không tồn tại</label>");
            }


            return Json("Đã gửi");

        }


        public string exceldsdiembttl(List<BaiTapTL> dsdiem)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet = workbook.Worksheets[0];

            sheet.Range["A1"].Text = "Họ tên";
            sheet.Range["A1"].Style.Font.FontName = "Times New Roman";
            sheet.Range["A1"].Style.Font.Color = Color.White;
            sheet.Range["A1"].Style.Font.Size = 16;
            sheet.Range["A1"].Style.Color = Color.DeepSkyBlue;
            sheet.Range["A1"].ColumnWidth = 25;

            sheet.Range["B1"].Text = "Email";
            sheet.Range["B1"].Style.Font.FontName = "Times New Roman";
            sheet.Range["B1"].Style.Font.Color = Color.White;
            sheet.Range["B1"].Style.Font.Size = 16;
            sheet.Range["B1"].Style.Color = Color.DeepSkyBlue;
            sheet.Range["B1"].ColumnWidth = 25;

            sheet.Range["C1"].Text = "Ngày nộp";
            sheet.Range["C1"].Style.Font.FontName = "Times New Roman";
            sheet.Range["C1"].Style.Font.Color = Color.White;
            sheet.Range["C1"].Style.Font.Size = 16;
            sheet.Range["C1"].Style.Color = Color.DeepSkyBlue;
            sheet.Range["C1"].ColumnWidth = 25;

            sheet.Range["D1"].Text = "Ngày hết hạn";
            sheet.Range["D1"].Style.Font.FontName = "Times New Roman";
            sheet.Range["D1"].Style.Font.Color = Color.White;
            sheet.Range["D1"].Style.Font.Size = 16;
            sheet.Range["D1"].Style.Color = Color.DeepSkyBlue;
            sheet.Range["D1"].ColumnWidth = 25;

            sheet.Range["E1"].Text = "Trạng thái";
            sheet.Range["E1"].Style.Font.FontName = "Times New Roman";
            sheet.Range["E1"].Style.Font.Color = Color.White;
            sheet.Range["E1"].Style.Font.Size = 16;
            sheet.Range["E1"].Style.Color = Color.DeepSkyBlue;
            sheet.Range["E1"].ColumnWidth = 25;

            sheet.Range["F1"].Text = "Điểm";
            sheet.Range["F1"].Style.Font.FontName = "Times New Roman";
            sheet.Range["F1"].Style.Font.Color = Color.White;
            sheet.Range["F1"].Style.Font.Size = 16;
            sheet.Range["F1"].Style.Color = Color.DeepSkyBlue;
            sheet.Range["F1"].ColumnWidth = 25;


            for (int i = 2; i <= dsdiem.Count + 1; i++)
            {
                sheet.Range["A" + i].Text = dsdiem[i - 2].TaiKhoan.Ho + " " + dsdiem[i - 2].TaiKhoan.Ten;
                sheet.Range["A" + i].Style.Font.FontName = "Times New Roman";
                sheet.Range["A" + i].Style.Font.Size = 13;
                sheet.Range["A" + i].Style.Font.Color = Color.Black;

                sheet.Range["B" + i].Text = dsdiem[i - 2].TaiKhoan.Email;
                sheet.Range["B" + i].Style.Font.FontName = "Times New Roman";
                sheet.Range["B" + i].Style.Font.Size = 13;
                sheet.Range["B" + i].Style.Font.Color = Color.Black;


                string ngaynop = "";
                string ngayketthuc = "";
                if (dsdiem[i - 2].NgayNop != null)
                {
                    ngaynop = dsdiem[i - 2].NgayNop.Value.ToString(string.Format("HH:mm:ss dd/MM/yyyy"));
                }
                if (dsdiem[i - 2].BaiTap.ThoiGianKetThuc != null)
                {
                    ngayketthuc = dsdiem[i - 2].BaiTap.ThoiGianKetThuc.Value.ToString(string.Format("HH:mm:ss dd/MM/yyyy"));
                }
                sheet.Range["C" + i].Text = ngaynop;
                sheet.Range["C" + i].Style.Font.FontName = "Times New Roman";
                sheet.Range["C" + i].Style.Font.Size = 13;
                sheet.Range["C" + i].Style.Font.Color = Color.Black;

                sheet.Range["D" + i].Text = ngayketthuc;
                sheet.Range["D" + i].Style.Font.FontName = "Times New Roman";
                sheet.Range["D" + i].Style.Font.Size = 13;
                sheet.Range["D" + i].Style.Font.Color = Color.Black;

                string trangthai = "";
                if (dsdiem[i - 2].NgayNop == null)
                {
                    trangthai = "Chưa Nộp";
                }
                else if (dsdiem[i - 2].NgayNop != null)
                {
                    if (dsdiem[i - 2].BaiTap.ThoiGianKetThuc >= dsdiem[i - 2].NgayNop)
                    {
                        trangthai = "Đã nộp";
                    }
                    else if (dsdiem[i - 2].BaiTap.ThoiGianKetThuc < dsdiem[i - 2].NgayNop)
                    {
                        trangthai = "Nộp muộn";
                    }
                }

                sheet.Range["E" + i].Text = trangthai;
                sheet.Range["E" + i].Style.Font.FontName = "Times New Roman";
                sheet.Range["E" + i].Style.Font.Size = 13;
                if (trangthai.Equals("Đã nộp"))
                {
                    sheet.Range["E" + i].Style.Font.Color = Color.Black;
                }
                else
                {
                    sheet.Range["E" + i].Style.Font.Color = Color.Red;
                }


                sheet.Range["F" + i].Text = dsdiem[i - 2].Diem.ToString();
                sheet.Range["F" + i].Style.Font.FontName = "Times New Roman";
                sheet.Range["F" + i].Style.Font.Size = 13;
                sheet.Range["F" + i].Style.Font.Color = Color.Black;




            }
            string path = Server.MapPath("~/Content/file/" + dsdiem[0].BaiTap.MaLop.ToString() + "_" + dsdiem[0].BaiTap.MaBaiTap.ToString() + ".xlsx");
            string path1 = "/Content/file/" + dsdiem[0].BaiTap.MaLop.ToString() + "_" + dsdiem[0].BaiTap.MaBaiTap.ToString() + ".xlsx";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            workbook.SaveToFile(path, ExcelVersion.Version2010);
            /*return path;*/
            return path1;
        }
        public string exceldsdiembttn(List<BaiTapTN> dsdiem)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet = workbook.Worksheets[0];

            sheet.Range["A1"].Text = "Họ tên";
            sheet.Range["A1"].Style.Font.FontName = "Times New Roman";
            sheet.Range["A1"].Style.Font.Color = Color.White;
            sheet.Range["A1"].Style.Font.Size = 16;
            sheet.Range["A1"].Style.Color = Color.DeepSkyBlue;
            sheet.Range["A1"].ColumnWidth = 25;

            sheet.Range["B1"].Text = "Email";
            sheet.Range["B1"].Style.Font.FontName = "Times New Roman";
            sheet.Range["B1"].Style.Font.Color = Color.White;
            sheet.Range["B1"].Style.Font.Size = 16;
            sheet.Range["B1"].Style.Color = Color.DeepSkyBlue;
            sheet.Range["B1"].ColumnWidth = 25;

            sheet.Range["C1"].Text = "Ngày nộp";
            sheet.Range["C1"].Style.Font.FontName = "Times New Roman";
            sheet.Range["C1"].Style.Font.Color = Color.White;
            sheet.Range["C1"].Style.Font.Size = 16;
            sheet.Range["C1"].Style.Color = Color.DeepSkyBlue;
            sheet.Range["C1"].ColumnWidth = 25;

            sheet.Range["D1"].Text = "Ngày hết hạn";
            sheet.Range["D1"].Style.Font.FontName = "Times New Roman";
            sheet.Range["D1"].Style.Font.Color = Color.White;
            sheet.Range["D1"].Style.Font.Size = 16;
            sheet.Range["D1"].Style.Color = Color.DeepSkyBlue;
            sheet.Range["D1"].ColumnWidth = 25;

            sheet.Range["E1"].Text = "Trạng thái";
            sheet.Range["E1"].Style.Font.FontName = "Times New Roman";
            sheet.Range["E1"].Style.Font.Color = Color.White;
            sheet.Range["E1"].Style.Font.Size = 16;
            sheet.Range["E1"].Style.Color = Color.DeepSkyBlue;
            sheet.Range["E1"].ColumnWidth = 25;

            sheet.Range["F1"].Text = "Điểm";
            sheet.Range["F1"].Style.Font.FontName = "Times New Roman";
            sheet.Range["F1"].Style.Font.Color = Color.White;
            sheet.Range["F1"].Style.Font.Size = 16;
            sheet.Range["F1"].Style.Color = Color.DeepSkyBlue;
            sheet.Range["F1"].ColumnWidth = 25;


            for (int i = 2; i <= dsdiem.Count + 1; i++)
            {
                sheet.Range["A" + i].Text = dsdiem[i - 2].TaiKhoan.Ho + " " + dsdiem[i - 2].TaiKhoan.Ten;
                sheet.Range["A" + i].Style.Font.FontName = "Times New Roman";
                sheet.Range["A" + i].Style.Font.Size = 13;
                sheet.Range["A" + i].Style.Font.Color = Color.Black;

                sheet.Range["B" + i].Text = dsdiem[i - 2].TaiKhoan.Email;
                sheet.Range["B" + i].Style.Font.FontName = "Times New Roman";
                sheet.Range["B" + i].Style.Font.Size = 13;
                sheet.Range["B" + i].Style.Font.Color = Color.Black;


                string ngaynop = "";
                string ngayketthuc = "";
                if (dsdiem[i - 2].NgayNop != null)
                {
                    ngaynop = dsdiem[i - 2].NgayNop.Value.ToString(string.Format("HH:mm:ss dd/MM/yyyy"));
                }
                if (dsdiem[i - 2].BaiTap.ThoiGianKetThuc != null)
                {
                    ngayketthuc = dsdiem[i - 2].BaiTap.ThoiGianKetThuc.Value.ToString(string.Format("HH:mm:ss dd/MM/yyyy"));
                }
                sheet.Range["C" + i].Text = ngaynop;
                sheet.Range["C" + i].Style.Font.FontName = "Times New Roman";
                sheet.Range["C" + i].Style.Font.Size = 13;
                sheet.Range["C" + i].Style.Font.Color = Color.Black;

                sheet.Range["D" + i].Text = ngayketthuc;
                sheet.Range["D" + i].Style.Font.FontName = "Times New Roman";
                sheet.Range["D" + i].Style.Font.Size = 13;
                sheet.Range["D" + i].Style.Font.Color = Color.Black;

                string trangthai = "";
                if (dsdiem[i - 2].NgayNop == null)
                {
                    trangthai = "Chưa Nộp";
                }
                else if (dsdiem[i - 2].NgayNop != null)
                {
                    if (dsdiem[i - 2].BaiTap.ThoiGianKetThuc >= dsdiem[i - 2].NgayNop)
                    {
                        trangthai = "Đã nộp";
                    }
                    else if (dsdiem[i - 2].BaiTap.ThoiGianKetThuc < dsdiem[i - 2].NgayNop)
                    {
                        trangthai = "Nộp muộn";
                    }
                }

                sheet.Range["E" + i].Text = trangthai;
                sheet.Range["E" + i].Style.Font.FontName = "Times New Roman";
                sheet.Range["E" + i].Style.Font.Size = 13;
                if (trangthai.Equals("Đã nộp"))
                {
                    sheet.Range["E" + i].Style.Font.Color = Color.Black;
                }
                else
                {
                    sheet.Range["E" + i].Style.Font.Color = Color.Red;
                }


                sheet.Range["F" + i].Text = dsdiem[i - 2].Diem.ToString();
                sheet.Range["F" + i].Style.Font.FontName = "Times New Roman";
                sheet.Range["F" + i].Style.Font.Size = 13;
                sheet.Range["F" + i].Style.Font.Color = Color.Black;




            }
            string path = Server.MapPath("~/Content/file/" + dsdiem[0].BaiTap.MaLop.ToString() + "_" + dsdiem[0].BaiTap.MaBaiTap.ToString() + ".xlsx");

            string path1 = "/Content/file/" + dsdiem[0].BaiTap.MaLop.ToString() + "_" + dsdiem[0].BaiTap.MaBaiTap.ToString() + ".xlsx";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            workbook.SaveToFile(path, ExcelVersion.Version2010);
            /* return path;*/
            return path1;
        }
        public string exceldsdiem(List<Diem> dsdiem)
        {
            var lophoc = Session["lophoc"] as LopHoc;
            Workbook workbook = new Workbook();
            Worksheet sheet = workbook.Worksheets[0];

            sheet.Range["A1"].Text = "Họ tên";
            sheet.Range["A1"].Style.Font.FontName = "Times New Roman";
            sheet.Range["A1"].Style.Font.Color = Color.White;
            sheet.Range["A1"].Style.Font.Size = 16;
            sheet.Range["A1"].Style.Color = Color.DeepSkyBlue;
            sheet.Range["A1"].ColumnWidth = 25;

            sheet.Range["B1"].Text = "Email";
            sheet.Range["B1"].Style.Font.FontName = "Times New Roman";
            sheet.Range["B1"].Style.Font.Color = Color.White;
            sheet.Range["B1"].Style.Font.Size = 16;
            sheet.Range["B1"].Style.Color = Color.DeepSkyBlue;
            sheet.Range["B1"].ColumnWidth = 25;

            sheet.Range["C1"].Text = "Số lượng bài đã nộp";
            sheet.Range["C1"].Style.Font.FontName = "Times New Roman";
            sheet.Range["C1"].Style.Font.Color = Color.White;
            sheet.Range["C1"].Style.Font.Size = 16;
            sheet.Range["C1"].Style.Color = Color.DeepSkyBlue;
            sheet.Range["C1"].ColumnWidth = 25;

            sheet.Range["D1"].Text = "Số lượng bài trễ";
            sheet.Range["D1"].Style.Font.FontName = "Times New Roman";
            sheet.Range["D1"].Style.Font.Color = Color.White;
            sheet.Range["D1"].Style.Font.Size = 16;
            sheet.Range["D1"].Style.Color = Color.DeepSkyBlue;
            sheet.Range["D1"].ColumnWidth = 25;

            sheet.Range["E1"].Text = "Số lượng bài chưa nộp";
            sheet.Range["E1"].Style.Font.FontName = "Times New Roman";
            sheet.Range["E1"].Style.Font.Color = Color.White;
            sheet.Range["E1"].Style.Font.Size = 16;
            sheet.Range["E1"].Style.Color = Color.DeepSkyBlue;
            sheet.Range["E1"].ColumnWidth = 25;

            sheet.Range["F1"].Text = "Số lượng bài đã chấm";
            sheet.Range["F1"].Style.Font.FontName = "Times New Roman";
            sheet.Range["F1"].Style.Font.Color = Color.White;
            sheet.Range["F1"].Style.Font.Size = 16;
            sheet.Range["F1"].Style.Color = Color.DeepSkyBlue;
            sheet.Range["F1"].ColumnWidth = 25;

            sheet.Range["G1"].Text = "Điểm";
            sheet.Range["G1"].Style.Font.FontName = "Times New Roman";
            sheet.Range["G1"].Style.Font.Color = Color.White;
            sheet.Range["G1"].Style.Font.Size = 16;
            sheet.Range["G1"].Style.Color = Color.DeepSkyBlue;
            sheet.Range["G1"].ColumnWidth = 25;

            for (int i = 2; i <= dsdiem.Count + 1; i++)
            {
                sheet.Range["A" + i].Text = dsdiem[i - 2].Hoten;
                sheet.Range["A" + i].Style.Font.FontName = "Times New Roman";
                sheet.Range["A" + i].Style.Font.Size = 13;
                sheet.Range["A" + i].Style.Font.Color = Color.Black;

                sheet.Range["B" + i].Text = dsdiem[i - 2].email;
                sheet.Range["B" + i].Style.Font.FontName = "Times New Roman";
                sheet.Range["B" + i].Style.Font.Size = 13;
                sheet.Range["B" + i].Style.Font.Color = Color.Black;

                sheet.Range["C" + i].Text = dsdiem[i - 2].soluongbaitap.ToString();
                sheet.Range["C" + i].Style.Font.FontName = "Times New Roman";
                sheet.Range["C" + i].Style.Font.Size = 13;
                sheet.Range["C" + i].Style.Font.Color = Color.Black;

                sheet.Range["D" + i].Text = dsdiem[i - 2].soluongtre.ToString();
                sheet.Range["D" + i].Style.Font.FontName = "Times New Roman";
                sheet.Range["D" + i].Style.Font.Size = 13;
                sheet.Range["D" + i].Style.Font.Color = Color.Black;

                sheet.Range["E" + i].Text = dsdiem[i - 2].soluongchuanop.ToString();
                sheet.Range["E" + i].Style.Font.FontName = "Times New Roman";
                sheet.Range["E" + i].Style.Font.Size = 13;
                sheet.Range["E" + i].Style.Font.Color = Color.Black;

                sheet.Range["F" + i].Text = dsdiem[i - 2].soluongbaicodiem.ToString();
                sheet.Range["F" + i].Style.Font.FontName = "Times New Roman";
                sheet.Range["F" + i].Style.Font.Size = 13;
                sheet.Range["F" + i].Style.Font.Color = Color.Black;

                sheet.Range["G" + i].Text = dsdiem[i - 2].diem.ToString();
                sheet.Range["G" + i].Style.Font.FontName = "Times New Roman";
                sheet.Range["G" + i].Style.Font.Size = 13;
                sheet.Range["G" + i].Style.Font.Color = Color.Black;

            }
            string path = Server.MapPath("~/Content/file/" + lophoc.MaLop.ToString() + ".xlsx");
            string path1 = "/Content/file/" + lophoc.MaLop.ToString() + ".xlsx";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            workbook.SaveToFile(path, ExcelVersion.Version2010);
            /*return path;*/
            return path1;
        }
        public string pdfdsdiembttl(List<BaiTapTL> dsdiem)
        {
            var lophoc = Session["lophoc"] as LopHoc;
            PdfDocument doc = new PdfDocument();
            PdfSection sec = doc.Sections.Add();
            sec.PageSettings.Width = PdfPageSize.A4.Width;
            PdfPageBase page = sec.Pages.Add();
            float y = 10;
            PdfBrush brush1 = PdfBrushes.Black;


            PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("Arial Unicode MS", 24f, FontStyle.Bold), true);
            PdfStringFormat format1 = new PdfStringFormat(PdfTextAlignment.Center);

            page.Canvas.DrawString("BẢNG ĐIỂM BÀI TẬP LỚP " + lophoc.TenLop.ToUpper(), font1, brush1, page.Canvas.ClientSize.Width / 2, y, format1);
            page.Canvas.DrawString("CHỦ ĐỀ :" + dsdiem[0].BaiTap.ChuDe.ToUpper(), font1, brush1, page.Canvas.ClientSize.Width / 2, y + 20, format1);

            y = y + font1.MeasureString("Country List", format1).Height;

            y = y + 5;


            PdfTable table = new PdfTable();
            table.Style.CellPadding = 1;

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Họ tên");
            dataTable.Columns.Add("Email");
            dataTable.Columns.Add("Ngày nộp");
            dataTable.Columns.Add("Ngày hết hạn");
            dataTable.Columns.Add("Trạng thái");
            dataTable.Columns.Add("Điểm");
            for (int i = 2; i <= dsdiem.Count + 1; i++)
            {
                string trangthai = "";
                if (dsdiem[i - 2].NgayNop == null)
                {
                    trangthai = "Chưa Nộp";
                }
                else if (dsdiem[i - 2].NgayNop != null)
                {
                    if (dsdiem[i - 2].BaiTap.ThoiGianKetThuc >= dsdiem[i - 2].NgayNop)
                    {
                        trangthai = "Đã nộp";
                    }
                    else if (dsdiem[i - 2].BaiTap.ThoiGianKetThuc < dsdiem[i - 2].NgayNop)
                    {
                        trangthai = "Nộp muộn";
                    }
                }
                string ngaynop = "";
                string ngayketthuc = "";
                if (dsdiem[i - 2].NgayNop != null)
                {
                    ngaynop = dsdiem[i - 2].NgayNop.Value.ToString(string.Format("HH:mm:ss dd/MM/yyyy"));
                }
                if (dsdiem[i - 2].BaiTap.ThoiGianKetThuc != null)
                {
                    ngayketthuc = dsdiem[i - 2].BaiTap.ThoiGianKetThuc.Value.ToString(string.Format("HH:mm:ss dd/MM/yyyy"));
                }
                dataTable.Rows.Add(new string[] { dsdiem[i - 2].TaiKhoan.Ho + " " + dsdiem[i - 2].TaiKhoan.Ten, dsdiem[i - 2].TaiKhoan.Email
                    , ngaynop, ngayketthuc, trangthai, dsdiem[i - 2].Diem.ToString() });



            }
            table.DataSource = dataTable;
            table.Columns[0].Width = 15;
            table.Columns[1].Width = 15;
            table.Columns[5].Width = 3;
            table.Style.ShowHeader = true;
            table.Style.HeaderStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            table.Style.HeaderStyle.Font = new PdfTrueTypeFont(new Font("Arial Unicode MS", 10f), true);
            table.Style.DefaultStyle.Font = new PdfTrueTypeFont(new Font("Arial Unicode MS", 8f), true);
            foreach (PdfColumn col in table.Columns)
            {


                col.StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);

            }

            table.Style.HeaderStyle.BackgroundBrush = PdfBrushes.Gray;


            table.Draw(page, new RectangleF(0, 90, 500, 70));
            string path = Server.MapPath("~/Content/file/" + dsdiem[0].BaiTap.MaLop.ToString() + "_" + dsdiem[0].BaiTap.MaBaiTap.ToString() + ".pdf");

            string path1 = "/Content/file/" + dsdiem[0].BaiTap.MaLop.ToString() + "_" + dsdiem[0].BaiTap.MaBaiTap.ToString() + ".pdf";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            doc.SaveToFile(path);
            return path1;

        }
        public string pdfdsdiembttn(List<BaiTapTN> dsdiem)
        {
            var lophoc = Session["lophoc"] as LopHoc;
            PdfDocument doc = new PdfDocument();
            PdfSection sec = doc.Sections.Add();
            sec.PageSettings.Width = PdfPageSize.A4.Width;
            PdfPageBase page = sec.Pages.Add();
            float y = 10;
            PdfBrush brush1 = PdfBrushes.Black;


            PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("Arial Unicode MS", 24f, FontStyle.Bold), true);
            PdfStringFormat format1 = new PdfStringFormat(PdfTextAlignment.Center);

            page.Canvas.DrawString("BẢNG ĐIỂM BÀI TẬP LỚP " + lophoc.TenLop.ToUpper(), font1, brush1, page.Canvas.ClientSize.Width / 2, y, format1);
            page.Canvas.DrawString("CHỦ ĐỀ :" + dsdiem[0].BaiTap.ChuDe.ToUpper(), font1, brush1, page.Canvas.ClientSize.Width / 2, y + 20, format1);

            y = y + font1.MeasureString("Country List", format1).Height;

            y = y + 5;


            PdfTable table = new PdfTable();
            table.Style.CellPadding = 1;

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Họ tên");
            dataTable.Columns.Add("Email");
            dataTable.Columns.Add("Ngày nộp");
            dataTable.Columns.Add("Ngày hết hạn");
            dataTable.Columns.Add("Trạng thái");
            dataTable.Columns.Add("Điểm");
            for (int i = 2; i <= dsdiem.Count + 1; i++)
            {
                string trangthai = "";
                if (dsdiem[i - 2].NgayNop == null)
                {
                    trangthai = "Chưa Nộp";
                }
                else if (dsdiem[i - 2].NgayNop != null)
                {
                    if (dsdiem[i - 2].BaiTap.ThoiGianKetThuc >= dsdiem[i - 2].NgayNop)
                    {
                        trangthai = "Đã nộp";
                    }
                    else if (dsdiem[i - 2].BaiTap.ThoiGianKetThuc < dsdiem[i - 2].NgayNop)
                    {
                        trangthai = "Nộp muộn";
                    }
                }
                string ngaynop = "";
                string ngayketthuc = "";
                if (dsdiem[i - 2].NgayNop != null)
                {
                    ngaynop = dsdiem[i - 2].NgayNop.Value.ToString(string.Format("HH:mm:ss dd/MM/yyyy"));
                }
                if (dsdiem[i - 2].BaiTap.ThoiGianKetThuc != null)
                {
                    ngayketthuc = dsdiem[i - 2].BaiTap.ThoiGianKetThuc.Value.ToString(string.Format("HH:mm:ss dd/MM/yyyy"));
                }
                dataTable.Rows.Add(new string[] { dsdiem[i - 2].TaiKhoan.Ho + " " + dsdiem[i - 2].TaiKhoan.Ten, dsdiem[i - 2].TaiKhoan.Email
                    , ngaynop, ngayketthuc, trangthai, dsdiem[i - 2].Diem.ToString() });



            }
            table.DataSource = dataTable;
            table.Columns[0].Width = 15;
            table.Columns[1].Width = 15;
            table.Columns[5].Width = 3;
            table.Style.ShowHeader = true;
            table.Style.HeaderStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            table.Style.HeaderStyle.Font = new PdfTrueTypeFont(new Font("Arial Unicode MS", 10f), true);
            table.Style.DefaultStyle.Font = new PdfTrueTypeFont(new Font("Arial Unicode MS", 8f), true);
            foreach (PdfColumn col in table.Columns)
            {


                col.StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);

            }

            table.Style.HeaderStyle.BackgroundBrush = PdfBrushes.Gray;


            table.Draw(page, new RectangleF(0, 90, 500, 800));

            string path = Server.MapPath("~/Content/file/" + dsdiem[0].BaiTap.MaLop.ToString() + "_" + dsdiem[0].BaiTap.MaBaiTap.ToString() + ".pdf");
            string path1 = "/Content/file/" + dsdiem[0].BaiTap.MaLop.ToString() + "_" + dsdiem[0].BaiTap.MaBaiTap.ToString() + ".pdf";

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            doc.SaveToFile(path);
            return path1;

        }
        public string pdfdsdiem(List<Diem> dsdiem)
        {
            var lophoc = Session["lophoc"] as LopHoc;
            PdfDocument doc = new PdfDocument();
            PdfSection sec = doc.Sections.Add();
            sec.PageSettings.Width = PdfPageSize.A4.Width;
            PdfPageBase page = sec.Pages.Add();
            float y = 10;
            PdfBrush brush1 = PdfBrushes.Black;


            PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("Arial Unicode MS", 24f, FontStyle.Bold), true);
            PdfStringFormat format1 = new PdfStringFormat(PdfTextAlignment.Center);

            page.Canvas.DrawString("BẢNG ĐIỂM LỚP " + lophoc.TenLop.ToUpper(), font1, brush1, page.Canvas.ClientSize.Width / 2, y, format1);

            y = y + font1.MeasureString("Country List", format1).Height;

            y = y + 5;


            PdfTable table = new PdfTable();
            table.Style.CellPadding = 1;

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Họ tên");
            dataTable.Columns.Add("Email");
            dataTable.Columns.Add("Số lượng bài đã nộp");
            dataTable.Columns.Add("Số lượng bài trễ");
            dataTable.Columns.Add("Số lượng bài chưa nộp");
            dataTable.Columns.Add("Số lượng bài đã chấm");
            dataTable.Columns.Add("Điểm");
            for (int i = 2; i <= dsdiem.Count + 1; i++)
            {
                dataTable.Rows.Add(new string[] { dsdiem[i - 2].Hoten, dsdiem[i - 2].email, dsdiem[i - 2].soluongbaitap.ToString()
                    , dsdiem[i - 2].soluongtre.ToString(), dsdiem[i - 2].soluongchuanop.ToString(), dsdiem[i - 2].soluongbaicodiem.ToString(),
            dsdiem[i - 2].diem.ToString()});

            }
            table.DataSource = dataTable;
            table.Columns[0].Width = 15;
            table.Columns[1].Width = 20;
            table.Columns[6].Width = 8;
            table.Style.ShowHeader = true;
            table.Style.HeaderStyle.BackgroundBrush = PdfBrushes.Gray;

            table.Style.HeaderStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            table.Style.HeaderStyle.Font = new PdfTrueTypeFont(new Font("Arial Unicode MS", 10f), true);
            table.Style.DefaultStyle.Font = new PdfTrueTypeFont(new Font("Arial Unicode MS", 8f), true);
            foreach (PdfColumn col in table.Columns)
            {


                col.StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);

            }

            table.Style.HeaderStyle.BackgroundBrush = PdfBrushes.Gray;

            table.Draw(page, new RectangleF(0, 90, 500, 800));

            string path = Server.MapPath("~/Content/file/" + lophoc.MaLop.ToString() + ".pdf");
            string path1 = "/Content/file/" + lophoc.MaLop.ToString() + ".pdf";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            doc.SaveToFile(path);
            return path1;

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult addcauhoitracnghiem(string id)
        {

            var noidungcauhoi = Request.Unvalidated.Form["addcauhoi"].ToString().Replace("<p>", "").Replace("</p>", "");
            CauHoi cauhoi = new CauHoi();
            cauhoi.NoiDung = noidungcauhoi;
            cauhoi.NgayThem = DateTime.Now;
            cauhoi.MaBaiTap = long.Parse(id);
            db.CauHois.Add(cauhoi);
            db.SaveChanges();
            var dapan1 = Request.Unvalidated.Form["da"].ToString();
            string[] slda = { "A", "B", "C", "D" };
            for (int i = 0; i < 4; i++)
            {
                DapAn dapan = new DapAn();
                var da = Request.Unvalidated.Form["" + slda[i]].ToString().Replace("<p>", "").Replace("</p>", "");
                dapan.MaCauHoi = cauhoi.MaCauHoi;
                dapan.NoiDung = da;
                if (dapan1.Equals(slda[i]))
                {
                    dapan.LoaiDapAn = true;
                }
                else dapan.LoaiDapAn = false;
                db.DapAns.Add(dapan);
                db.SaveChanges();
            }



            return RedirectToAction("ShowcauhoiTracnghiem", new { id = id });
        }
        [HttpPost]
        public void deletecauhoitracnghiem(string macauhoi)
        {

            var cauhoi = db.CauHois.SingleOrDefault(x => x.MaCauHoi.ToString().Equals(macauhoi));
            var bainop = db.BaiTapTNs.Where(x => x.MaBaiTap.ToString().Equals(cauhoi.MaBaiTap.ToString())).ToList();
            foreach (var i in bainop)
            {
                var bailam = db.TTBaiTapTNs.Where(x => x.MaBaiNop.ToString().Equals(i.MaBaiNop.ToString())).ToList();
                db.TTBaiTapTNs.RemoveRange(bailam);
                db.SaveChanges();
                i.NgayNop = null;
                i.Diem = null;
                i.Trangthai = null;
                db.SaveChanges();
            }
            var dapan = db.DapAns.Where(x => x.MaCauHoi.ToString().Equals(macauhoi)).ToList();
            db.DapAns.RemoveRange(dapan);
            db.CauHois.Remove(cauhoi);
            db.SaveChanges();

        }

    }

}