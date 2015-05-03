using System.Collections.Generic;
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
}