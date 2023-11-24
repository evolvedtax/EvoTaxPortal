using System.Text;
using EvolvedTax.Business.MailService;
using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.InstituteService;
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
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;

namespace EvolvedTax_Admin.Controllers
{
    public class InstituteController : BaseController
    {
        readonly private IInstituteService _instituteService;
        readonly private IWebHostEnvironment _webHostEnvironment;
        readonly private IMailService _emailService;
        readonly ICommonService _commonService;
        readonly RoleManager<IdentityRole> _identityRoles;
        readonly UserManager<User> _userManager;
        readonly EvolvedtaxContext _evolvedtaxContext;
        public InstituteController(IInstituteService instituteService, IMailService emailService, IWebHostEnvironment webHostEnvironment, ICommonService commonService, EvolvedtaxContext evolvedtaxContext, RoleManager<IdentityRole> identityRoles, UserManager<User> userManager)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _commonService = commonService;
            _instituteService = instituteService;
            _emailService = emailService;
            _webHostEnvironment = webHostEnvironment;
            _identityRoles = identityRoles;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Master()
        {
            return View(_instituteService.GetMaster());
        }
        [HttpPost]
        public async Task<IActionResult> RequestInstituteName(string NewInstituteName, string Comments)
        {
            var scheme = HttpContext.Request.Scheme; // "http" or "https"
            var host = string.Empty;
            if (_webHostEnvironment.IsDevelopment())
            {
                host = HttpContext.Request.Host.Value;
            }
            else
            {
                host = URLConstants.ClientUrl; // Hostname (e.g., example.com)
            }
            var fullUrl = $"{scheme}://{host}";

            int instituteId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var instituteName = _instituteService.GetInstituteDataById(instituteId).InstitutionName ?? "";

            var requestChangeNameModel = new InstituteRequestNameChange { OldName = instituteName, InstituteId = SessionUser.InstituteId, NewName = NewInstituteName, IsApproved = RequestChangeNameStatusEnum.Pending, RequestedOn = DateTime.Now, RequesterUserId = SessionUser.UserId };
            await _evolvedtaxContext.InstituteRequestNameChange.AddAsync(requestChangeNameModel);

            var alrtModel = new Alert { AlertText = SessionUser.FirstName + " " + SessionUser.LastName + " has requested for change institute name.", CreatedDate = DateTime.Now, InstituteID = SessionUser.InstituteId, Title = "Request Change Name", IsRead = false };
            await _evolvedtaxContext.Alert.AddAsync(alrtModel);
            await _evolvedtaxContext.SaveChangesAsync();

            //var acceptLink = string.Concat(fullUrl, "/Institute/", "ChangeInstituteName?u=" + SessionUser.UserId, "&s=" + EncryptionHelper.Encrypt("Approved"));
            //var rejectLink = string.Concat(fullUrl, "/Institute/", "ChangeInstituteName?u=" + SessionUser.UserId, "&s=" + EncryptionHelper.Encrypt("Rejected"));

            var acceptLink = fullUrl;
            var rejectLink = fullUrl;

            var userFullName = SessionUser.FirstName + " " + SessionUser.LastName;
            await _emailService.SendEmailForChangeInstituteNameRequest(instituteName, NewInstituteName, userFullName, acceptLink, rejectLink, Comments);
            return Json(new { Status = true });
        }
        //[HttpGet]
        //public IActionResult ChangeInstituteName(string u, string s)
        //{
        //    var status = EncryptionHelper.Decrypt(s.Replace(' ', '+').Replace('-', '+').Replace('_', '/'));
        //    var requestChangeNameResult = _evolvedtaxContext.InstituteRequestNameChange.OrderBy(n => n).LastOrDefault(p => p.RequesterUserId == u);
        //    if (status == "Approved")
        //    {
        //        requestChangeNameResult.IsApproved = RequestChangeNameStatusEnum.Approved;
        //        var institute = _instituteService.GetInstituteDataById(requestChangeNameResult.InstituteId);
        //        institute.InstitutionName = requestChangeNameResult.NewName;
        //        _evolvedtaxContext.InstituteMasters.Update(institute);
        //    }
        //    else if (status == "Rejected")
        //    {
        //        requestChangeNameResult.IsApproved = RequestChangeNameStatusEnum.Approved;
        //    }
        //    requestChangeNameResult.ApprovedOn = DateTime.Now;
        //    requestChangeNameResult.ApprovedBy = SessionUser.UserId;

        //    _evolvedtaxContext.InstituteRequestNameChange.Update(requestChangeNameResult);
        //    _evolvedtaxContext.SaveChanges();

        //    return Json(new { Status = true });
        //}
        [HttpGet]
        public IActionResult ChangeInstituteName(int Id, RequestChangeNameStatusEnum requestChange)
        {
            var requestChangeNameResult = _evolvedtaxContext.InstituteRequestNameChange.OrderBy(n => n).LastOrDefault(p => p.Id == Id);
            if (requestChange == RequestChangeNameStatusEnum.Approved)
            {
                requestChangeNameResult.IsApproved = RequestChangeNameStatusEnum.Approved;
                // Get the institute entity from the context if it's already being tracked
                var institute = _evolvedtaxContext.InstituteMasters.Find(requestChangeNameResult.InstituteId);

                if (institute == null)
                {
                    // If the institute entity isn't tracked, fetch it and attach it to the context
                    institute = _instituteService.GetInstituteDataById(requestChangeNameResult.InstituteId);
                    _evolvedtaxContext.Attach(institute);
                }
                // Update the entity properties
                institute.InstitutionName = requestChangeNameResult.NewName;
            }
            else if (requestChange == RequestChangeNameStatusEnum.Rejected)
            {
                requestChangeNameResult.IsApproved = RequestChangeNameStatusEnum.Rejected;
            }
            requestChangeNameResult.ApprovedOn = DateTime.Now;
            requestChangeNameResult.ApprovedBy = SessionUser.UserId;

            _evolvedtaxContext.InstituteRequestNameChange.Update(requestChangeNameResult);
            _evolvedtaxContext.SaveChanges();

            return Json(new { Status = true });
        }

        #region Entities
        public IActionResult changeSubscription(int SubscriptionId)
        {
            HttpContext.Session.SetInt32("SubscriptionId", SubscriptionId);
            return Json(new { Data = "true" }); ;
        }
        public IActionResult Entities(int? instituteId)
        {
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
            var FormNameItems = _evolvedtaxContext.FormName.ToList();

            var SubscriptionId = HttpContext.Session.GetInt32("SubscriptionId") ?? -1;
            if (SubscriptionId == -1)
            {
                HttpContext.Session.SetInt32("SubscriptionId", -1);
            }
            ViewBag.FormNameListMain = FormNameItems.Select(p => new SelectListItem
            {
                Text = p.Form_Name,
                Value = p.Id.ToString(),
                Selected = p.Id == SubscriptionId
            });
            if (instituteId != null)
            {
                ViewBag.InstituteId = instituteId;
                HttpContext.Session.SetInt32("SelectedInstitute", instituteId ?? 0);
                if (User.IsInRole("Admin") || User.IsInRole("Co-Admin"))
                {
                    model.InstituteEntitiesResponse = _instituteService.GetEntitiesByInstId(instituteId ?? 0, SubscriptionId);
                }
                else
                {
                    model.InstituteEntitiesResponse = _instituteService.GetEntitiesByInstId(instituteId ?? 0, SubscriptionId);
                    //model.InstituteEntitiesResponse = _instituteService.GetEntitiesByInstIdRole(instituteId ?? 0);
                }
                return View(model);
            }
            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            HttpContext.Session.SetInt32("SelectedInstitute", InstId);
         
            model.InstituteEntitiesResponse = _instituteService.GetEntitiesByInstId(InstId, SubscriptionId);
            return View(model);
        }
        public IActionResult EntitiesRecyleBin()
        {
            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            return View(_instituteService.GetRecyleBinEntitiesByInstId(InstId));
        }
        public IActionResult RestoreEntity(int[] selectedValues)
        {
            return Json(_instituteService.RestoreEntities(selectedValues));
        }
        public IActionResult ChangeEntity(int entityId)
        {
            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var response = _instituteService.GetClientByEntityId(InstId, entityId);
            var EmailFrequency = _evolvedtaxContext.InstituteEntities.FirstOrDefault(p => p.EntityId == entityId)?.EmailFrequency;
            return Json(new { Data = response, EmailFrequency = EmailFrequency });
        }
        [Route("institute/uploadEntities")]
        [HttpPost]
        public async Task<IActionResult> UploadEntities(IFormFile file, short InstituteId)
        {
            string InstituteName = string.Empty;

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
            var response = await _instituteService.UploadEntityData(file, InstituteId, InstituteName);
            return Json(response);
        }
        [Route("institute/AddEntity")]
        [HttpPost]
        public async Task<IActionResult> AddEntity(InstituteEntityViewModel request)
        {

            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
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
            var response = await _instituteService.AddEntity(request.InstituteEntityRequest);
            return Json(response);
        }
        [Route("institute/UpdateEntity")]
        [HttpPost]
        public async Task<IActionResult> UpdateEntity(InstituteEntityViewModel request)
        {
            if (request.InstituteEntityRequest.EntityId == 0)
            {
                return Json(false);
            }
            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
            request.InstituteEntityRequest.InstituteId = (short)instId;
            var response = await _instituteService.UpdateEntity(request.InstituteEntityRequest);
            return Json(response);
        }
        [Route("institute/DeleteEntity")]
        [HttpPost]
        public async Task<IActionResult> DeleteEntity(int id)
        {
            if (id == 0)
            {
                return Json(false);
            }
            var response = await _instituteService.DeleteEntity(id, RecordStatusEnum.Trash);
            return Json(response);
        }
        [Route("institute/EmptyRecycleBinEntity")]
        [HttpPost]
        public async Task<IActionResult> EmptyRecycleBinEntity(int[] selectedValues)
        {
            if (selectedValues.Length == 0)
            {
                return Json(false);
            }
            var response = await _instituteService.TrashEmptyEntity(selectedValues, RecordStatusEnum.EmptyTrash);
            return Json(response);
        }
        [Route("institute/LockUnlockEntity")]
        [HttpPost]
        public async Task<IActionResult> LockUnlockEntity(int[] selectedValues, bool isLocked)
        {
            var response = await _instituteService.LockUnlockEntity(selectedValues, isLocked);
            return Json(response);
        }
        [Route("institute/IsEntityNameExist")]
        [HttpGet]
        public IActionResult IsEntityNameExist(InstituteEntityViewModel model)
        {
            var institueId = HttpContext.Session.GetInt32("SelectedInstitute") ?? 0;
            var response = _instituteService.IsEntityNameExist(model.InstituteEntityRequest.EntityName, model.InstituteEntityRequest.EntityId, institueId);
            return Json(response);
        }
        [Route("institute/IsEINExist")]
        [HttpGet]
        public IActionResult IsEINExist(InstituteEntityViewModel model)
        {
            var institueId = HttpContext.Session.GetInt32("SelectedInstitute") ?? 0;
            var response = _instituteService.IsEINExist(model.InstituteEntityRequest.Ein, model.InstituteEntityRequest.EntityId, institueId);
            return Json(response);
        }
        #endregion

        #region Clients
        public IActionResult ClientsRecyleBin(int entityId)
        {
            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            return View(_instituteService.GetRecyleBinClientsByEntityId(InstId, entityId));
        }
        public IActionResult RestoreClient(int[] selectedValues)
        {
            return Json(_instituteService.RestoreClients(selectedValues));
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

        public IActionResult Client(int EntityId, int? InstituteId)
        {
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
            string UserId = HttpContext.Session.GetString("UserId");
            string userRole = _evolvedtaxContext.EntitiesUsers.FirstOrDefault(p => p.UserId == UserId && p.EntityId == EntityId)?.Role.Trim();
            if (userRole == null)
            {
                userRole = "Admin";// SessionUser.UserRole;
            }
            ViewBag.UserRole = userRole;


            var roleHierarchyData = _evolvedtaxContext.RoleHierarchy.ToList();
            var roleHierarchy = roleHierarchyData.ToDictionary(
                role => role.RoleName,
                role => role.AllowedRoles.Split(',').ToList()
            );

            // Filter and construct SelectListItem based on user's role
            ViewBag.Roles = _identityRoles.Roles
                .Where(p => roleHierarchy[userRole].Contains(p.Name))
                .Select(p => new SelectListItem
                {
                    Text = "Invite as " + p.Name,
                    Value = p.Name,
                    Selected = p.Name == "Viewer"
                });

            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var entities = _instituteService.GetEntitiesByInstId(InstituteId ?? 0);
            var IntMaster = _instituteService.GetInstituteDataById(InstituteId ?? 0);

            ViewBag.IsEnableEmailFrequency = IntMaster.IsEmailFrequency ?? false;
            ViewBag.EntitiesList = entities.Select(p => new SelectListItem
            {
                Text = p.EntityName,
                Value = p.EntityId.ToString(),
                Selected = (p.EntityId == EntityId)
            });
            ViewBag.EmailFrequency = entities?.FirstOrDefault(p => p.EntityId == EntityId)?.EmailFrequency;
            ViewBag.EntityId = EntityId;
            var users = _userManager.Users.ToList();
            //var usersData = _evolvedtaxContext.EntitiesUsers
            //            .Where(p => p.EntityId == EntityId)
            //            .ToList(); 

            var usersData = _evolvedtaxContext.EntitiesUsers
                            .Where(p => p.EntityId == EntityId)
                            .Join(
                                _evolvedtaxContext.Users.Where(u => u.EmailConfirmed),
                                entitiesUser => entitiesUser.UserId,
                                user => user.Id,
                                (entitiesUser, user) => new
                                {
                                    EntitiesUser = entitiesUser,
                                    User = user
                                }
                            )
                            .ToList();


            var availableRoles = roleHierarchy[userRole];
            ViewBag.availableRoles = availableRoles;

            //var sharedUsersResponse = usersData
            //    .Select(p => new SharedUsersResponse
            //    {
            //        UserName = users.FirstOrDefault(x => x.Id == p.UserId.ToLower())?.FirstName + " " + users.FirstOrDefault(x => x.Id == p.UserId.ToLower())?.LastName,
            //        Role = p.Role.Trim(),
            //        Id = p.Id.ToString(),
            //    })
            //    .ToList();

            var sharedUsersResponse = usersData
              .Select(p => new SharedUsersResponse
              {
                  UserName = $"{p.User.FirstName} {p.User.LastName}",
                  Role = p.EntitiesUser.Role.Trim(), // Access the Role property from EntitiesUser
                  Id = p.EntitiesUser.Id.ToString(),
              })
              .ToList();

            var model = new InstituteClientViewModel { InstituteClientsResponse = _instituteService.GetClientByEntityId(InstituteId ?? 0, EntityId), SharedUsersResponse = sharedUsersResponse.AsQueryable() };
            return View(model);
        }
        public async Task<IActionResult> SendEmail(int[] selectedValues, int EntityId)
        {
            var scheme = HttpContext.Request.Scheme; // "http" or "https"
            var host = string.Empty;
            if (_webHostEnvironment.IsDevelopment())
            {
                host = HttpContext.Request.Host.Value;
            }
            else
            {
                host = URLConstants.ClientUrl; // Hostname (e.g., example.com)
            }
            var fullUrl = $"{scheme}://{host}";

            string URL = string.Concat(fullUrl, "/Account", "/OTP");

            var user = await _userManager.GetUserAsync(User);

            string userName = string.Concat(user.FirstName, " ", user.LastName);
            string ActionText = $"An email has been sent to {{Email}} for the client associated with the {{EntityName}}, sent by {user.FirstName} {user.LastName}";


            await _emailService.SendEmailAsync(_instituteService.GetClientInfoByClientId(selectedValues).Where(p => p.ClientStatus != AppConstants.ClientStatusFormSubmitted).ToList(), "Action Required: Verify Your Account with EvoForms", "", URL, ActionText, userName, SessionUser.InstituteId);
            return Json(new { type = ResponseMessageConstants.SuccessStatus, message = ResponseMessageConstants.SuccessEmailSend });
        }
        [Route("institute/uploadClients")]
        [HttpPost]
        public async Task<IActionResult> UploadClients(IFormFile file, int EntityId, string entityName)
        {
            if (file == null)
            {
                return Json(false);
            }
            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var response = await _instituteService.UploadClientData(file, instId, EntityId, entityName.Trim());
            return Json(response);
        }
        [Route("institute/AddClient")]
        [HttpPost]
        public async Task<IActionResult> AddClient(InstituteClientViewModel request)
        {
            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
            request.InstituteClientsRequest.InstituteId = (short)instId;
            request.InstituteClientsRequest.LastUpdatedBy = (short)instId;
            var response = await _instituteService.AddClient(request.InstituteClientsRequest);
            return Json(response);
        }
        [Route("institute/UpdateClient")]
        [HttpPost]
        public async Task<IActionResult> UpdateClient(InstituteClientViewModel request)
        {
            if (request.InstituteClientsRequest.ClientId == 0)
            {
                return Json(false);
            }
            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
            request.InstituteClientsRequest.LastUpdatedBy = (short)instId;
            var response = await _instituteService.UpdateClient(request.InstituteClientsRequest);
            return Json(response);
        }
        [Route("institute/UpdateEmailFrequency")]
        [HttpPost]
        public async Task<IActionResult> UpdateEmailFrequency(int EntityIdFreq, int EmailFrequency)
        {
            if (EntityIdFreq == 0)
            {
                return Json(false);
            }
            var response = await _instituteService.UpdateEmailFrequncy(EntityIdFreq, EmailFrequency);
            return Json(response);
        }
        [Route("institute/DeleteClient")]
        [HttpPost]
        public async Task<IActionResult> DeleteClient(int id)
        {
            if (id == 0)
            {
                return Json(false);
            }
            var response = await _instituteService.DeleteClient(id, RecordStatusEnum.Trash);
            return Json(response);
        }

        [Route("institute/DeleteClienRecord")]
        [HttpPost]
        public async Task<IActionResult> DeleteClienRecord(int id)
        {
            if (id == 0)
            {
                return Json(false);
            }
            var response = await _instituteService.DeleteClientPermeant(id);
            return Json(response);
        }

        [Route("institute/KeepClienRecord")]
        [HttpPost]
        public async Task<IActionResult> KeepClienRecord(int id)
        {
            if (id == 0)
            {
                return Json(false);
            }
            var response = await _instituteService.KeepClienRecord(id);
            return Json(response);
        }
        [Route("institute/EmptyRecycleBinClient")]
        [HttpPost]
        public async Task<IActionResult> EmptyRecycleBinClient(int[] selectedValues)
        {
            if (selectedValues.Length == 0)
            {
                return Json(false);
            }
            var response = await _instituteService.TrashEmptyClient(selectedValues, RecordStatusEnum.EmptyTrash);
            return Json(response);
        }
        [Route("institute/LockUnlockClient")]
        [HttpPost]
        public async Task<IActionResult> LockUnlockClient(int[] selectedValues, bool isLocked)
        {
            var response = await _instituteService.LockUnlockClient(selectedValues, isLocked);
            return Json(response);
        }
        #endregion
        [HttpGet]
        public IActionResult DownloadExcel(string fileType)
        {
            string fileName;
            string filePath;

            switch (fileType)
            {
                case AppConstants.Entity:
                    fileName = AppConstants.InstituteEntityTemplate;
                    break;
                case AppConstants.Client:
                    fileName = AppConstants.InstituteClientTemplate;
                    break;
                default:
                    return NotFound();
            }

            filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Templates", fileName);

            if (System.IO.File.Exists(filePath))
            {
                var memoryStream = _commonService.DownloadFile(filePath);
                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            else
            {
                return NotFound();
            }

        }


        [HttpPost]
        public async Task<IActionResult> LogButtonClicked(string buttonText, string entityId, string InstituteId)
        {
            var CreatedBy = HttpContext.Session.GetString("UserId");
            string Category = "";
            var user = await _userManager.GetUserAsync(User);
            //var InstituteResponse = _instituteService.GetClientByEntityId(Convert.ToInt32(InstituteId), Convert.ToInt32(entityId));
            var EntityName = _evolvedtaxContext.InstituteEntities.FirstOrDefault(p => p.EntityId == Convert.ToInt32(entityId))?.EntityName.Trim();
            string userName = string.Concat(user.FirstName, " ", user.LastName);
            string newButtonText = $"{user.FirstName} {user.LastName} click {buttonText} in ({EntityName})";
            if (buttonText.Contains("Import"))
            {
                Category = "Upload";
            }
            var response = await _instituteService.LogClientButtonClicked(userName, newButtonText, Convert.ToInt32(entityId), Category);
            return Json(response);


        }


    }
}