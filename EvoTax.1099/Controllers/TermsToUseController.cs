using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax_1099.Controllers
{
    public class TermsToUseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
