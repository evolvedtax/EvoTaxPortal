using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax_Institute.Controllers
{
    public class TermsToUseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
