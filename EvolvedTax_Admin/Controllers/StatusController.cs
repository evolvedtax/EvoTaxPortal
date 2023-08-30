using EvolvedTax.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax_Admin.Controllers
{
    [UserSession]
    public class StatusController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}