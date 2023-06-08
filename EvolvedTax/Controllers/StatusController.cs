using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax.Controllers
{
    public class StatusController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}