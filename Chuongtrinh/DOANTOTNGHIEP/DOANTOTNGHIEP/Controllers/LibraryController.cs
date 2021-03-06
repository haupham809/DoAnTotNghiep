using HtmlAgilityPack;
/*using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using DOANTOTNGHIEP.Models;
using System.IO;
using DOANTOTNGHIEP.Models.GetData;
using Spire.Pdf;
using System.Drawing;
using System.Drawing.Imaging;


namespace DOANTOTNGHIEP.Controllers
{
    public class LibraryController : Controller
    {

        public static List<DOANTOTNGHIEP.Models.Tailieu> getfiletkb(string noidung)
        {
            List<DOANTOTNGHIEP.Models.Tailieu> tailieu = new List<Models.Tailieu>();
            /*ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            var options = new ChromeOptions();
            options.AddArgument("no-sandbox");
            // Chạy ngầm không pop up trình duyệt ra ngoài 
            options.AddArgument("headless");
            options.AddArgument("--window-position=-32000,-32000");
            options.AddArgument("--incognito");
            IWebDriver webDriver = new ChromeDriver(HostingEnvironment.MapPath("~/Content/chromdriver"), options);
            webDriver.Url = "https://kupdf.net/";
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            if(noidung.Length != 0)
            {
            var nd = webDriver.FindElement(By.XPath("//*[@id='keyword']"));
            nd.SendKeys(noidung);
            var bt = webDriver.FindElement(By.XPath("/html/body/div[4]/div/form/div/button"));
            bt.Click();
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            }*/


            int trang = 1;

            HtmlWeb htmlWeb = new HtmlWeb()
            {
                AutoDetectEncoding = true,
                OverrideEncoding = Encoding.UTF8
            };
            while (true)
            {
                string search = "https://kupdf.net";
                if (noidung.Length > 0)
                {
                    search = search + "/search/" + noidung + "/" + trang;
                }

                trang++;



                /*HtmlDocument document = htmlWeb.Load(webDriver.Url);*/
                HtmlDocument document = htmlWeb.Load(search);


                if (document.DocumentNode.SelectNodes("//div/div[@class='card']") == null) break;
                var threadItems = document.DocumentNode.SelectNodes("//div/div[@class='card']").ToList();
                foreach (var s in threadItems)
                {
                    DOANTOTNGHIEP.Models.Tailieu tl = new Models.Tailieu();
                    var c = s.SelectSingleNode(".//div[@class='card-image']/a/img");
                    c.Attributes["class"].Remove();/*
                c.Attributes.Append("background-size");
                c.SetAttributeValue("background-size", "cover");*/
                    c.Attributes.Append("style");
                    c.SetAttributeValue("style", "max-width:250;width:auto;height:auto;max-height:100%");
                    tl.anh = c.OuterHtml;
                    var a = s.SelectSingleNode(".//div[@class='card-header']/h3[@class='card-title']/a");
                    tl.ten = a.InnerHtml;
                    var b = s.SelectSingleNode(".//div[@class='card-footer']/a");
                    b.Attributes.Append("target");
                    b.SetAttributeValue("target", "_blank");
                    tl.duongdan = b.OuterHtml;
                    tailieu.Add(tl);
                }
                if (trang == 10 || noidung.Length == 0)
                {
                    break;
                }



            }




            return tailieu;
        }
        // GET: Library
        public ActionResult Library()
        {
            DB db = new DB();
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            /*if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            string malop = Session["malop"].ToString();
            var gv = db.LopHocs.SingleOrDefault(x => x.MaLop.ToString().Equals(malop) && x.NguoiTao.Equals(user.TenDangNhap));
            if (gv == null)
            {
                return RedirectToAction("Index", "TrangChu");
            }*/
            return View();
        }
        public ActionResult Searchlibrary()
        {
            DB db = new DB();
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            var cauhoi = Request.Form["noidungcantin"].ToString().ToLower();
            string malop = Session["malop"].ToString();
            List<document> documents = new List<document>();
            if(cauhoi.Replace("  ","").Length == 0)
            {
                documents = db.documents.Where(x => x.MaLop.ToString().Equals(malop)).ToList();
            }
            List<Tailieu> tailieu=new List<Tailieu>();
            foreach(var filedoccument in documents)
            {
                Tailieu tl = new  Tailieu();
                tl.ten = filedoccument.Ten;
                tl.anh = filedoccument.Image;
                tl.duongdan = filedoccument.Vitriluu;
                tailieu.Add(tl);

            }
            List<document> docs = new List<document>();
            List<string> keyseach = new List<string>();
            if (cauhoi.Replace("  ", "").Length > 0)
            {
                for(var i =0;i< cauhoi.Replace("  ", " ").TrimEnd(' ').TrimStart(' ').Split(' ').ToList().Count; i++)
                {
                    var s = cauhoi.Replace("  ", " ").TrimEnd(' ').TrimStart(' ').Split(' ').ToArray()[i];
                    for (var j = i+1; j < cauhoi.Replace("  ", " ").TrimEnd(' ').TrimStart(' ').Split(' ').ToList().Count; j++)
                    {
                        var v = s + " " + cauhoi.Replace("  ", " ").TrimEnd(' ').TrimStart(' ').Split(' ').ToArray()[j];
                        keyseach.Add(v);
                        s = v;
                    }

                }
                foreach(var v in cauhoi.Replace("  ", " ").TrimEnd(' ').TrimStart(' ').Split(' ').ToList())
                {
                    keyseach.Add(v);
                }

            }
            foreach(var v in keyseach.OrderByDescending(x=>x.TrimEnd(' ').TrimStart(' ').Split(' ').Length))
            {
                documents = db.documents.Where(x => x.MaLop.ToString().Equals(malop) && x.Noidung.Replace("  ", " ").Contains(v)).ToList();
                docs.AddRange(documents);
            }
            foreach(var filedoccument in docs)
            {
                if(tailieu.Where(x=>x.duongdan.Equals(filedoccument.Vitriluu)  ).ToList().Count == 0)
                {
                    Tailieu tl = new Tailieu();
                    tl.ten = filedoccument.Ten;
                    tl.anh = filedoccument.Image;
                    tl.duongdan = filedoccument.Vitriluu;
                    tailieu.Add(tl);

                }
            }
            ViewData["s"] = tailieu;
            return PartialView();
        }
        [HttpPost]
        public ActionResult Uploaddocument(HttpPostedFileBase filedocumentupload)
        {
            DB db = new DB();
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            string malop = Session["malop"].ToString();
            var gv = db.LopHocs.SingleOrDefault(x => x.MaLop.ToString().Equals(malop) && x.NguoiTao.Equals(user.TenDangNhap));
            if (gv == null)
            {
                return RedirectToAction("Index", "TrangChu");
            }
            string returnImagePath = string.Empty;
            string fileName;
            string Extension;
            string imageName;
            string imageSavePath;
            string tieude = Request.Form["titledocument"];
            if (filedocumentupload.ContentLength > 0)
            {
                fileName = Path.GetFileNameWithoutExtension(filedocumentupload.FileName);
                Extension = Path.GetExtension(filedocumentupload.FileName);
                imageName = fileName + DateTime.Now.ToString("yyyyMMddHHmmss");
                imageSavePath = Server.MapPath("~/Content/document/"+ Models.crypt.Encrypt.encryptfoder(malop).Replace("+", "").Replace("=", "").Replace("-", "").Replace("_", "") + "/" + Models.crypt.Encrypt.encryptfoder(user.TenDangNhap).Replace("+", "").Replace("=", "").Replace("-", "").Replace("_", "") + "/") + imageName +Extension;
                filedocumentupload.SaveAs(imageSavePath);
                document documentpdf = new document();
                documentpdf.Vitriluu = "/Content/document/" + Models.crypt.Encrypt.encryptfoder( malop).Replace("+", "").Replace("=", "").Replace("-", "").Replace("_", "") + "/" + Models.crypt.Encrypt.encryptfoder(user.TenDangNhap).Replace("+", "").Replace("=", "").Replace("-", "").Replace("_", "") + "/" + imageName + Extension;
                documentpdf.Ten = tieude;
                documentpdf.Nguoisohuu = user.TenDangNhap;
                documentpdf.Ngaydang = DateTime.Now;
                documentpdf.LuotTaiXuong = 0;
                documentpdf.Luotxem = 0;
                documentpdf.Noidung = getdatapdf(documentpdf.Vitriluu);
                documentpdf.Image = getimagepdf(documentpdf.Vitriluu, malop, user.TenDangNhap);
                documentpdf.MaLop = Convert.ToInt64( malop);
                db.documents.Add(documentpdf);
                db.SaveChanges();
              

            }
           
            return RedirectToAction("Library", "Library");
        }
        public string getdatapdf(string filepdf)
        {
            PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(Server.MapPath(filepdf));
            StringBuilder buffer = new StringBuilder();
            IList<Image> images = new List<Image>();
            foreach (PdfPageBase page in doc.Pages)
            {
                buffer.Append(page.ExtractText().Replace("  ", " ").Replace("\r", "").Replace("\n", ""));
            }
            doc.Close();
            var noidung = buffer.ToString().Replace("\r", "").Replace("\n", "").Replace("Evaluation Warning : The document was created with Spire.PDF for .NET.", "").Replace("  ", " ");
            while (true)
            {
                noidung = noidung.Replace("  ", " ");
                if (noidung.IndexOf("  ") < 0) break;
            }
            return noidung;
        }

        public string getimagepdf(string  filepdf , string malop, string TenDangNhap )
        {

            PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(Server.MapPath(filepdf));
            Image bmp = doc.SaveAsImage(0);
            Image emf = doc.SaveAsImage(0, Spire.Pdf.Graphics.PdfImageType.Metafile);
            Image zoomImg = new Bitmap((int)(emf.Size.Width * 2), (int)(emf.Size.Height * 2));
            using (Graphics g = Graphics.FromImage(zoomImg))
            {
   
                g.ScaleTransform(2.0f, 2.0f);
                g.DrawImage(emf, new Rectangle(new Point(0, 0), emf.Size), new Rectangle(new Point(0, 0), emf.Size), GraphicsUnit.Pixel);
            }
            string extension = DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
            string path = Server.MapPath("~/Content/document/" + Models.crypt.Encrypt.encryptfoder(malop).Replace("+", "").Replace("=", "").Replace("-", "").Replace("_", "") + "/" + Models.crypt.Encrypt.encryptfoder(TenDangNhap).Replace("+", "").Replace("=", "").Replace("-", "").Replace("_", "") + "/") + extension;
            emf.Save(path, ImageFormat.Png);
            return "/Content/document/" + Models.crypt.Encrypt.encryptfoder(malop).Replace("+", "").Replace("=", "").Replace("-", "").Replace("_", "") + "/" + Models.crypt.Encrypt.encryptfoder(TenDangNhap).Replace("+", "").Replace("=", "").Replace("-", "").Replace("_", "") + "/"+extension;

        }



    }

}