using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EvoTax._5498.Controllers
{
    [Area("5498")]
    public class Form5498Controller : BaseController
    {
        public IActionResult SelectForm(int entityId)
        {
            HttpContext.Session.SetInt32("EntityId", entityId);
            return View();
        }
    }
}
