global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Rendering;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Authorization;

namespace EvolvedTax.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
    }
}