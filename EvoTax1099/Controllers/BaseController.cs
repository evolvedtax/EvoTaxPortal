global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Rendering;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Authorization;
using EvolvedTax.Data.Models.DTOs;
using EvolvedTax1099.Common.Utils;
using EvolvedTax1099.Business.Services.SessionProfileUser;

namespace EvolvedTax1099.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public UserSessionProfileDTO SessionUser
        {
            get
            {
                using var serviceScope = ServiceActivator.GetScope();
                return serviceScope.ServiceProvider.GetRequiredService<UserSessionProfileService>().GetUserModel();
            }
        }
    }
}