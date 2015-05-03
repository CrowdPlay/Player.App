using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace CrowdPlay.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ((ClaimsIdentity) User.Identity).FindFirst("FullName");

            var moods = new List<string> { "HAPPY", "SLEEPY" };

            ViewBag.Moods = moods;

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
    }
}