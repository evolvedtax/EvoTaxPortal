using EvolvedTax.Business.Services.AnnouncementService;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Helpers;
using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax_Admin.Controllers
{
    public class DashboardController : BaseController
    {
        readonly IAnnouncementService _announcementService;
        public DashboardController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }
        public IActionResult Index()
        {
            return View();
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
