using System.Text;
using EvolvedTax.Business.MailService;
using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Business.Services.UserService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Enums;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.DTOs.Response;
using EvolvedTax.Data.Models.DTOs.ViewModels;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Helpers;
using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EvolvedTax_Institute.Areas._3921.Controllers
{
    [Area("3921")]
    public class InstituteController : BaseController
    {
        readonly private IInstituteService _instituteService;
        readonly private IWebHostEnvironment _webHostEnvironment;
        readonly private IMailService _emailService;
        readonly ICommonService _commonService;
        readonly RoleManager<IdentityRole> _identityRoles;
        readonly UserManager<User> _userManager;
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly IMailService _mailService;
        readonly IUserService _userService;
        public InstituteController(IInstituteService instituteService, IMailService emailService, IWebHostEnvironment webHostEnvironment,
            ICommonService commonService, EvolvedtaxContext evolvedtaxContext, RoleManager<IdentityRole> identityRoles,
            UserManager<User> userManager, IUserService userService, IMailService mailService)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _commonService = commonService;
            _instituteService = instituteService;
            _emailService = emailService;
            _webHostEnvironment = webHostEnvironment;
            _identityRoles = identityRoles;
            _userManager = userManager;
            _userService = userService;
            _mailService = mailService;
        }

        #region Entities
        public IActionResult W9Entities(int? instituteId)
        {
            HttpContext.Session.SetInt32("EntityId", 0);
            var model = new InstituteEntityViewModel();
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
                Value = p.StateId
            });
            if (instituteId != null)
            {
                ViewBag.InstituteId = instituteId;
                HttpContext.Session.SetInt32("SelectedInstitute", instituteId ?? 0);
                if (User.IsInRole("Admin") || User.IsInRole("Co-Admin"))
                {
                    model.InstituteEntitiesResponse = _instituteService.GetEntitiesByInstId(instituteId ?? 0);
                }
                else
                {
                    model.InstituteEntitiesResponse = _instituteService.GetEntitiesByInstId(instituteId ?? 0);
                    //model.InstituteEntitiesResponse = _instituteService.GetEntitiesByInstIdRole(instituteId ?? 0);
                }
                return View(model);
            }
            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            HttpContext.Session.SetInt32("SelectedInstitute", InstId);
            model.InstituteEntitiesResponse = _instituteService.GetEntitiesByInstId(InstId,Convert.ToInt32( AppConstants.FormSubscription_3921));
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteMultipleEntity(int[] selectedValues)
        {
            var response = await _instituteService.DeleteMultipleEntity(selectedValues, RecordStatusEnum.Trash, Convert.ToInt32(AppConstants.FormSubscription_3921));
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> LockUnlockEntity(int[] selectedValues, bool isLocked)
        {
            var response = await _instituteService.LockUnlockEntity(selectedValues, isLocked);
            return Json(response);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteEntity(int id)
        {
            
            var response = await _instituteService.DeleteEntity(id, RecordStatusEnum.Trash, Convert.ToInt32(AppConstants.FormSubscription_3921));
            return Json(response);
        }

        [HttpGet]
        public IActionResult GetEntityRole(int entityId)
        {
            try
            {
                string userId = HttpContext.Session.GetString("UserId");
                string userRole = _evolvedtaxContext.EntitiesUsers.FirstOrDefault(p => p.UserId == userId && p.EntityId == entityId)?.Role.Trim();

                return Json(new { role = userRole });
            }
            catch (Exception ex)
            {
                // Handle error
                return StatusCode(500, "An error occurred while fetching entity role.");
            }
        }


 
        [HttpPost]
        public async Task<IActionResult> UploadEntities(IFormFile file, short InstituteId, int[] subscriptionId)
        {
            string InstituteName = string.Empty;
            int[] subscriptionIds = subscriptionId.Distinct().ToArray();

            if (InstituteId == 0)
            {
                var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
                InstituteId = (short)instId;
                InstituteName = HttpContext.Session.GetString("InstituteName") ?? string.Empty;
            }
            else
            {
                InstituteName = _instituteService.GetInstituteDataById(InstituteId).InstitutionName ?? string.Empty;
            }
            var response = await _instituteService.UploadEntityData(file, InstituteId, InstituteName, subscriptionIds);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddEntity(InstituteEntityViewModel request, int[] subscriptionId)
        {
            if (subscriptionId == null || subscriptionId.Length == 0)
            {
                subscriptionId = new int[] { Convert.ToInt32(AppConstants.FormSubscription_3921) };
            }

            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
            int[] subscriptionIds = subscriptionId.Distinct().ToArray();
            if (request.InstituteEntityRequest.InstituteId == 0)
            {
                request.InstituteEntityRequest.InstituteId = (short)instId;
                request.InstituteEntityRequest.InstituteName = HttpContext.Session.GetString("InstituteName");
            }
            else
            {
                request.InstituteEntityRequest.InstituteName = _instituteService.GetInstituteDataById(request.InstituteEntityRequest.InstituteId).InstitutionName;
            }
            request.InstituteEntityRequest.LastUpdatedBy = (short)instId;
            var response = await _instituteService.AddEntity(request.InstituteEntityRequest, subscriptionIds);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEntity(InstituteEntityViewModel request, int[] subscriptionId)
        {


            if (subscriptionId == null || subscriptionId.Length == 0)
            {
                subscriptionId = new int[] { Convert.ToInt32(AppConstants.FormSubscription_3921) };
            }

            if (request.InstituteEntityRequest.EntityId == 0)
            {
                return Json(false);
            }
            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
            request.InstituteEntityRequest.InstituteId = (short)instId;
            var response = await _instituteService.UpdateEntity(request.InstituteEntityRequest, subscriptionId);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> InviteUserForEntities(string role, string emailAddresses, string EntityNamesHidden)
        {
            List<string> emails = JsonConvert.DeserializeObject<List<string>>(emailAddresses);
            string[] entityNamePairs = EntityNamesHidden.Split(',');

            foreach (var entityNamePair in entityNamePairs)
            {
                string[] parts = entityNamePair.Split('$');
                if (parts.Length == 2)
                {
                    string entityName = parts[0].Trim();
                    string entityIdStr = parts[1].Trim();
                    if (int.TryParse(entityIdStr, out int entityId))
                    {
                        int InstituteId = HttpContext.Session.GetInt32("InstId") ?? 0;
                        var instituteName = HttpContext.Session.GetString("InstituteName");
                        string AssignedBy = HttpContext.Session.GetString("UserId");

                        foreach (var email in emails)
                        {
                            var responseForm = await _userService.SaveInvitedUserForShare(role, entityId, email, InstituteId, AssignedBy);
                            if (responseForm)
                            {
                                var URL = Url.Action("SignUpForInvite", "Account", new { i = "id", e = "email", s = "share" }, Request.Scheme) ?? "";
                                if (URL.Contains("/1099/"))
                                {
                                    URL = URL.Replace("/1099/", "/");
                                }
                                var user = await _userManager.GetUserAsync(User);
                                var invitee = await _userManager.GetUserAsync(User);
                                await _mailService.SendShareInvitaionEmailSignUp(email, URL, InstituteId.ToString(), "Action Required: You have been invited to signup with EvoForms", string.Concat(user.FirstName, " ", user.LastName), instituteName, entityName, role, InstituteId);
                            }
                            else
                            {
                                var scheme = HttpContext.Request.Scheme; // "http" or "https"
                                var host = HttpContext.Request.Host.Value; // Hostname (e.g., example.com)
                                var fullUrl = $"{scheme}://{host}";
                                var URL = string.Concat(fullUrl, "/Account/", "Login");
                                var user = await _userManager.GetUserAsync(User);
                                var invitee = await _userManager.GetUserAsync(User);
                                await _mailService.SendShareInvitaionEmail(email, URL, string.Concat(invitee.FirstName, " ", invitee.LastName), "Action Required: You have been invited to signup with EvoForms", string.Concat(user.FirstName, " ", user.LastName), instituteName, entityName, role, InstituteId);
                            }
                        }
                    }
                }
            }

            return Json(new { Status = true, Message = "Invited link has been sent." });
        }
        #endregion
    }
}