using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EvoTax._1042.Controllers
{
    [Area("1042")]
    public class Form1042Controller : BaseController
    {
        public IActionResult SelectForm(int entityId)
        {
            HttpContext.Session.SetInt32("EntityId", entityId);
            return View();
        }
    }
}
