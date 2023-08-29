using EvolvedTax.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax_Institute.Controllers
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