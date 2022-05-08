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
            return View();
        }
        public ActionResult Searchlibrary()
        {
            var cauhoi = Request.Form["noidungcantin"].ToString();
            ViewData["s"] = getfiletkb(cauhoi);
            return PartialView();
        }
        [HttpPost]
        public ActionResult Uploaddocument(HttpPostedFileBase[] file)
        {
            DB db = new DB();
            var user = Session["user"] as DOANTOTNGHIEP.Models.TaiKhoan;
            if (user == null) return RedirectToAction("Login", "Login");
            if (Session["malop"] == null) return RedirectToAction("Index", "TrangChu");
            string malop = Session["malop"].ToString();
            var gv = db.LopHocs.SingleOrDefault(x => x.MaLop.ToString().Equals(malop) && x.NguoiTao.Equals(user.TenDangNhap));
            if (gv == null)
            {
                return Library();
            }
            return Library();
        }


    }

}