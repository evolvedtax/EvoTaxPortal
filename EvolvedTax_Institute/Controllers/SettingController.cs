﻿using AutoMapper;
using EvolvedTax.Business.MailService;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Business.Services.UserService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Common.ExtensionMethods;
using EvolvedTax.Data.Enums;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.DTOs.Response;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Helpers;
using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace EvolvedTax_Institute.Controllers
{
    public class SettingController : BaseController
    {
        #region Fields
        #endregion

        #region Ctor
        readonly IUserService _userService;
        readonly IMailService _mailService;
        readonly IInstituteService _instituteService;
        readonly IWebHostEnvironment _webHostEnvironment;
        readonly UserManager<User> _userManager;
        readonly RoleManager<IdentityRole> _roleManager;
        readonly IMapper _mapper;
        private readonly EvolvedtaxContext _evolvedtaxContext;

        public SettingController(IUserService userService, IMailService mailService, EvolvedtaxContext evolvedtaxContext, IMapper mapper, IWebHostEnvironment webHostEnvironment, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IInstituteService instituteService)
        {
            _userService = userService;
            _mailService = mailService;
            _evolvedtaxContext = evolvedtaxContext;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _roleManager = roleManager;
            _instituteService = instituteService;
        }
        #endregion

        #region Methods
        public ActionResult Index()
        {
            var model = new SettingRequest();
            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
            //------------ UPDATE PROFILE -------------//
            var response = _instituteService.GetInstituteDataById(instId);
            var items = _evolvedtaxContext.MstrCountries.ToList();
            ViewBag.CountriesList = items.OrderBy(item => item.Favorite != "0" ? int.Parse(item.Favorite) : int.MaxValue)
                                  .ThenBy(item => item.Country).Select(p => new SelectListItem
                                  {
                                      Text = p.Country,
                                      Value = p.Country,
                                  });

            ViewBag.StatesList = _evolvedtaxContext.MasterStates.Select(p => new SelectListItem
            {
                Text = p.StateId,
                Value = p.StateId.ToString()
            });


            var timezones = _evolvedtaxContext.MasterTimezones
                 .OrderBy(p => p.Id)
        .Select(p => new
        {
            Text = string.Format("({0}) {1}", p.GmtOffset, p.TimeZone),
            Value = string.Format("({0}) {1}", p.GmtOffset, p.TimeZone)
        })
        .ToList();

            ViewBag.TimezonList = new SelectList(timezones, "Text", "Value");

            List<string> dateFormats = new List<string>
    {
                             "MM/DD/YYYY",
                            "MM-DD-YYYY",
                            "MM DD YYYY",
                            "MM.DD.YYYY",
                            "DD/MM/YYYY",
                            "DD-MM-YYYY",
                            "DD MM YYYY",
                            "DD.MM.YYYY",
                            "YYYY-MM-DD",
                            "YYYY/MM/DD",
                            "YYYY MM DD",
                            "YYYY.MM.DD",
                            "YYYY-DD-MM",
                            "YYYY/DD/MM",
                            "YYYY DD MM",
                            "YYYY.DD.MM"
    };

            ViewBag.DateFormats = new SelectList(dateFormats);
            model.InstituteMasterRequest = _mapper.Map<InstituteMasterRequest>(response ?? new InstituteMasterResponse());
            //------------ EMAIL REMINDER -------------//

            var emailSettingModel = _evolvedtaxContext.EmailSetting.Select(p => new EmailSettingRequest
            {
                EmailDoamin = p.EmailDoamin,
                Password = p.Password,
                SMTPPort = p.SMTPPort,
                SMTPServer = p.SMTPServer,
                POPServer = p.POPServer,
                POPPort = p.POPPort,
            }).First();
            model.EmailSettingRequest = emailSettingModel;
            //----------- REQUEST NAME CHANGE ----------//
            var requestChangeName = _evolvedtaxContext.InstituteRequestNameChange.OrderBy(n => n.RequestedOn).LastOrDefault(p => p.RequesterUserId == SessionUser.UserId);
            model.InstituteRequestNameChange.IsApproved = requestChangeName != null ? requestChangeName.IsApproved : RequestChangeNameStatusEnum.Approved;
            var users = _userManager.Users;
            var requestNameChangeResponse = _evolvedtaxContext.InstituteRequestNameChange.Where(p=>p.IsApproved == RequestChangeNameStatusEnum.Pending).Select(p => new InstituteRequestNameChangeResponse
            {
                Id = p.Id,
                InstituteId = p.InstituteId,
                IsApproved = p.IsApproved,
                NewName = p.NewName,
                OldName = p.OldName,
                ApprovedOn = p.ApprovedOn,
                RequestedOn = p.RequestedOn,
                RequesterUserId = p.RequesterUserId,
                UserName = users.First(x=>x.Id == p.RequesterUserId).UserName ?? ""
            }).AsQueryable();
            model.InstituteRequestNameChangeResponses = requestNameChangeResponse;
            //----------- RENDERING IFRAME -------------//
            //ViewData["UserManagement"] = @"<iframe src=""https://portal.evolvedforms.com/EvoSystem/UserManagement.aspx?InstID="" frameborder=""0"" width=""100%"" height=""800""></iframe>";
            ViewData["UserManagement"] = $@"<iframe src=""https://portal.evolvedforms.com/EvoSystem/UserManagement.aspx?InstID={instId}"" frameborder=""0"" width=""100%"" height=""800""></iframe>";

            ViewData["TransactionHistory"] = $@"<iframe src=""https://portal.evolvedforms.com/EvoSystem/TransactionHistory.aspx?InstID={instId}"" frameborder=""0"" width=""100%"" height=""800""></iframe>";

            //----------- subscription -------------//
            var FormNameItems = _evolvedtaxContext.FormName.ToList();
            var instituteId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var selectedFormNames = _evolvedtaxContext.FormAccess
                              .Where(f => f.InstituteID == instituteId)
                              .Select(f => f.Form_Name)
                              .ToList();

            ViewBag.CheckBoxFormNameList = FormNameItems;
            ViewBag.SelectedFormNames = selectedFormNames;

            return View(model);
        }
        [HttpPost]
        public IActionResult EmailSetting(SettingRequest request)
        {
            var model = _evolvedtaxContext.EmailSetting.First();
            model.EmailDoamin = request.EmailSettingRequest.EmailDoamin;
            model.Password = request.EmailSettingRequest.Password;
            model.SMTPPort = request.EmailSettingRequest.SMTPPort;
            model.SMTPServer = request.EmailSettingRequest.SMTPServer;
            model.POPServer = request.EmailSettingRequest.POPServer;
            model.POPPort = request.EmailSettingRequest.POPPort;
            _evolvedtaxContext.EmailSetting.Update(model);
            _evolvedtaxContext.SaveChanges();
            return Json(new { Status = true });
        }
        #endregion


    }
}
