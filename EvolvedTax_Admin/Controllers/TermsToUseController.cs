using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax_Admin.Controllers
{
    public class TermsToUseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
