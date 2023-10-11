using EvolvedTax.Business.Services.AnnouncementService;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Helpers;
using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax_Institute.Controllers
{
    public class SummaryController : BaseController
    {
        public SummaryController()
        {
        }
        public IActionResult Index()
        {
            var sd = SessionUser.TypeOfEntity;
            return View();
        }

    }
}
