using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax.Controllers
{
    public class TermsToUseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
