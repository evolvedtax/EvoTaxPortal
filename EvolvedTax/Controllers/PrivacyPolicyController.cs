using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax.Controllers
{
    public class PrivacyPolicyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
