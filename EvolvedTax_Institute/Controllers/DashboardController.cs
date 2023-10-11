using EvolvedTax.Business.Services.AnnouncementService;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Helpers;
using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax_Institute.Controllers
{
    public class DashboardController : BaseController
    {
        readonly IAnnouncementService _announcementService;
        readonly IInstituteService _instituteService;
        public DashboardController(IAnnouncementService announcementService, IInstituteService instituteService)
        {
            _announcementService = announcementService;
            _instituteService = instituteService;
        }
        public IActionResult Index()
        {
            DashboardRequest model = _instituteService.DashboardDataByInstituteId(SessionUser.InstituteId);
            return View(model);
        }
        public IActionResult GetAnnouncements()
        {

            List<AnnouncementRequest> announcements = _announcementService.GetAnnouncements();
            return PartialView("/Views/Announcement/_Announcements.cshtml", announcements);
        }
        public IActionResult GetAlerts()
        {
            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
            List<AlertRequest> alerts = _announcementService.GetAlerts(instId);
            return PartialView("/Views/Announcement/_Alerts.cshtml", alerts);
        }
    }
}
