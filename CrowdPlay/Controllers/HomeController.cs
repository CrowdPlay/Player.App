using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CrowdPlay.Models;
using RestSharp;

namespace CrowdPlay.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ((ClaimsIdentity) User.Identity).FindFirst("FullName");

            var moods = GetAllMoods();

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Moods = moods;
                var twitterHandle = GetTwitterHandle();

                ViewBag.TwitterHandle = twitterHandle;
                ViewBag.Room = GetUser(twitterHandle).Room;
                ViewBag.Mood = "kjasnd";
            }
            
            return View();
        }

        public ActionResult UpdateMood()
        {
            var json = new StreamReader(Request.InputStream).ReadToEnd();

            var requestUser = new JavaScriptSerializer().Deserialize<RequestUser>(json);

            var request = new RestRequest("/user", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddBody(requestUser);

            new RestClient("http://191.238.115.5:8081").Execute(request);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private RequestUser GetUser(string twitterHandle)
        {
            var request = new RestRequest("/user/" + twitterHandle, Method.GET) { RequestFormat = DataFormat.Json };

            var response = new RestClient("http://191.238.115.5:8081").Execute(request).Content;

            return new JavaScriptSerializer().Deserialize<RequestUser>(response);
        }

        private string GetTwitterHandle()
        {
            var prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            return prinicpal.Claims.Where(claim => claim.Type == ("urn:twitter:screen_name")).First().Value;
        }

        private IEnumerable<string> GetAllMoods()
        {
            var request = new RestRequest("GetAllMood/", Method.GET) { RequestFormat = DataFormat.Json };

            var response = new RestClient("http://191.238.115.5:8081").Execute(request).Content;

            return new JavaScriptSerializer().Deserialize<IEnumerable<string>>(response);
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