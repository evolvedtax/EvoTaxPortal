using EvolvedTax.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax.Controllers
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