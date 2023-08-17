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
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Converters;

namespace EvolvedTax.Controllers
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
        #region Entities
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
            model.InstituteEntitiesResponse = _instituteService.GetEntitiesByInstId(InstId);
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
             string userRole =_evolvedtaxContext.EntitiesUsers.FirstOrDefault(p=>p.UserId== UserId && p.EntityId== EntityId)?.Role.Trim();
            //HttpContext.Session.SetString("UserRole", userRole);
            ViewBag.UserRole = userRole;

            // Define a dictionary to represent the role hierarchy
            var roleHierarchy = new Dictionary<string, List<string>>
            {
                { "Viewer", new List<string>() },
                { "Editor", new List<string> { "Viewer" } },
                { "Co-Admin", new List<string> { "Viewer", "Editor" } },
                { "Admin", new List<string> { "Viewer", "Editor", "Co-Admin" } }
            };

            // Filter and construct SelectListItem based on user's role
            ViewBag.Roles = _identityRoles.Roles
                //.Where(p => p.Name != "Admin" && p.Name != "SuperAdmin" && p.Name != "Co-Admin")
                .Where(p => roleHierarchy[userRole].Contains(p.Name))
                .Select(p => new SelectListItem
                {
                    Text = "Invite as " + p.Name,
                    Value = p.Name,
                    Selected = p.Name == "Viewer"
                });
            //ViewBag.Roles = _identityRoles.Roles.Where(p => p.Name != "Admin" && p.Name != "SuperAdmin" && p.Name != "Co-Admin")
            //    .Select(p => new SelectListItem
            //    {
            //        Text = "Invite as " + p.Name,
            //        Value = p.Name,
            //        Selected = p.Name == "Viewer"
            //    });
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
            var usersData = _evolvedtaxContext.EntitiesUsers
                        .Where(p => p.EntityId == EntityId)
                        .ToList(); // Fetch data from the database


            var availableRoles = roleHierarchy[userRole];
            ViewBag.availableRoles = availableRoles;
            //var availableRoles = roleHierarchy.ContainsKey(userRole) ? roleHierarchy[userRole] : new List<string>();

            var sharedUsersResponse = usersData
                .Select(p => new SharedUsersResponse
                {
                    UserName = users.FirstOrDefault(x => x.Id == p.UserId.ToLower())?.FirstName + " " + users.FirstOrDefault(x => x.Id == p.UserId.ToLower())?.LastName,
                    Role = p.Role.Trim(),
                    Id=p.Id.ToString(),
                    
                })
       //.Where(p => availableRoles.Contains(p.Role)) // Filter out roles not in availableRoles
                .ToList();
            var model = new InstituteClientViewModel { InstituteClientsResponse = _instituteService.GetClientByEntityId(InstituteId ?? 0, EntityId), SharedUsersResponse = sharedUsersResponse.AsQueryable() };
            return View(model);
        }
        public async Task<IActionResult> SendEmail(int[] selectedValues)
        {
            var scheme = HttpContext.Request.Scheme; // "http" or "https"
            var host = HttpContext.Request.Host.Value; // Hostname (e.g., example.com)
            var fullUrl = $"{scheme}://{host}";

            string URL = string.Concat(fullUrl, "/Account", "/OTP");
            await _emailService.SendEmailAsync(_instituteService.GetClientInfoByClientId(selectedValues).Where(p => p.ClientStatus != AppConstants.ClientStatusFormSubmitted).ToList(), "Action Required: Verify Your Registration with EvoTax Portal", "", URL);
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

  
    }
}