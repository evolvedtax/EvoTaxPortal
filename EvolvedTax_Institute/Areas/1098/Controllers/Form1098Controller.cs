using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EvoTax._1098.Controllers
{
    [Area("1098")]
    public class Form1098Controller : BaseController
    {
        public IActionResult SelectForm(int entityId)
        {
            HttpContext.Session.SetInt32("EntityId", entityId);
            return View();
        }
    }
}
