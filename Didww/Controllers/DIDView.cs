using Microsoft.AspNetCore.Mvc;

namespace Didww.Controllers
{
    public class DIDView : Controller
    {
        public IActionResult Index()
        {
            //var token = HttpContext.Session.GetString("Token");
            //if (token == null)
            //{
            //    return Unauthorized(new { Error = "UnAuthorized! Please Login First" });
            //}
            //else
            //{
            //    ViewData["Token"] = token;
            //}
            return View();
        }
        public IActionResult Remove()
        {
            return View();
        }
        public IActionResult Logs()
        {
            return View();
        }
        public IActionResult WebReport()
        {
            return View();
        }

    }
}
