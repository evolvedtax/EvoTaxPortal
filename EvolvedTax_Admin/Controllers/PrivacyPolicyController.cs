using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax_Admin.Controllers
{
    public class PrivacyPolicyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
