global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Rendering;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Authorization;
using EvolvedTax.Business.Services.SessionProfileUser;
using EvolvedTax.Data.Models.DTOs;
using EvolvedTax.Common.Utils;
using EvolvedTax.Helpers;

namespace EvolvedTax.Web.Controllers
{
    [Authorize]
    [SessionTimeout]
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