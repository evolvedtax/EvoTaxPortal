﻿
using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.AnnouncementService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using EvolvedTax.Web.Controllers;

namespace EvolvedTax_Institute.Controllers
{
    public class AnnouncementController : BaseController
    {
        readonly IWebHostEnvironment _webHostEnvironment;
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly IAnnouncementService _announcementService;
        private readonly IHubContext<AnnouncementHub> _hubContext;


        public AnnouncementController(IWebHostEnvironment webHostEnvironment, EvolvedtaxContext evolvedtaxContext,
        IAnnouncementService announcementService, IHubContext<AnnouncementHub> hubContext)
        {

            _announcementService = announcementService;
            _evolvedtaxContext = evolvedtaxContext;
            _webHostEnvironment = webHostEnvironment;
            _hubContext = hubContext;

        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult PostAnnouncementAsync(SettingRequest model)
        {
            _announcementService.SaveAnnouncement(model.AnnouncementRequest);
            // await _hubContext.Clients.All.SendAsync("ReceiveAnnouncement", model.Message);

            //return Task.FromResult<IActionResult>(RedirectToAction("Index"));
            return Json(new { Status = true });
        }
        [HttpPost]
        public IActionResult PostSubscription(SettingRequest model)
        {
            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
           
            foreach (var selectedFormName in model.FormAccessRequest.SelectedFormNames)
            {
                if (!string.IsNullOrEmpty(selectedFormName))
                {
                    model.FormAccessRequest.InstituteID = instId;
                    model.FormAccessRequest.Form_Name = selectedFormName;
                    _announcementService.SaveSubscription(model.FormAccessRequest);
                }
      
            }
    
        
            return Json(new { Status = true });
        }

        public IActionResult GetAnnouncements()
        {

            List<AnnouncementRequest> announcements = _announcementService.GetAnnouncements();
            return PartialView("_Announcements", announcements);
        }
        public IActionResult ViewAnnouncement(int id)
        {

            var announcement = _announcementService.GetAnnouncementByID(id);

            if (announcement == null)
            {
                return NotFound();
            }
            return View(announcement);
        }
    }
}
