using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax_Institute.Controllers
{
    public class W8Controller : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Entities", "Institute", new { area = "" });
        }
    }
}
