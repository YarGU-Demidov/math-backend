using Microsoft.AspNetCore.Mvc;

namespace MathSite.Api.Server.Controllers
{
    [ApiVersionNeutral]
    public class HomeController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}