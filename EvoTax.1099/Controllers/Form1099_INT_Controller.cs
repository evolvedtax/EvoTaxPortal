using EvolvedTax.Business.Services.AnnouncementService;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Helpers;
using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax_1099.Controllers
{
    public class Form1099_INT_Controller : BaseController
    {
        readonly IAnnouncementService _announcementService;
        public Form1099_INT_Controller(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
