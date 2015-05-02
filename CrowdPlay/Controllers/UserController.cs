using System;
using System.Web.Mvc;

namespace CrowdPlay.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            

            return View("Index");
        }
    }

    public class User
    {
        public string mood { get; set; }
        public int room { get; set; }
        public Guid id { get; set; }
    }
}