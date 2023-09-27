using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EvoTax._1099.Controllers
{
    [Area("1099")]
    public class Form1099Controller : BaseController
    {
        public IActionResult SelectForm(int entityId)
        {
            HttpContext.Session.SetInt32("EntityId", entityId);
            return View();
        }
    }
}
