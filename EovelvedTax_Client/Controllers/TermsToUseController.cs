using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax_Client.Controllers
{
    public class TermsToUseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
