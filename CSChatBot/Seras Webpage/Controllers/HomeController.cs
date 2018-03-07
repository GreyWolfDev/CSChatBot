using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Seras_Webpage.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //get some tasty cookies
            var request = (HttpWebRequest)WebRequest.Create("https://oauth.telegram.org/auth?bot_id=547043436&request_access=write");
            request.Method = "GET";
            
            var response = (HttpWebResponse)request.GetResponse();
            var k = response.Headers["Set-Cookie"];
            Response.Cookies.Add(new HttpCookie(k));
            Response.Cookies.Add(new HttpCookie("stel_bot324283739_origin", "http%3A%2F%2Fseras.parawuff.com"));
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public string Widget()
        {
            var page = new WebClient().DownloadString("https://oauth.telegram.org/embed/serasvbot?origin=seras.parawuff.com&size=large&request_access=write");
            var doc = new HtmlDocument();
            byte[] array = Encoding.UTF8.GetBytes(page);
            var stream = new MemoryStream(array);
            doc.Load(stream);
            var dn = doc.DocumentNode.ChildNodes["html"];
            var body = dn.ChildNodes["body"];
            var scriptNode = body.ChildNodes["script"];
            scriptNode.Attributes["src"].Value = scriptNode.Attributes["src"].Value.Replace("//telegram.org/js/widget-frame.js", "/Scripts/widget-frame.js");

            return body.InnerHtml;
        }
    }
}