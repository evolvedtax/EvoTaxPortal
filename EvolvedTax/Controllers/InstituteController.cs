using System.Text;
using EvolvedTax.Business.MailService;
using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Enums;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Helpers;
using Newtonsoft.Json.Converters;

namespace EvolvedTax.Controllers
{
    [SessionTimeout]
    public class InstituteController : Controller
    {
        readonly private IInstituteService _instituteService;
        readonly private IWebHostEnvironment _webHostEnvironment;
        readonly private IMailService _emailService;
        readonly ICommonService _commonService;
        readonly EvolvedtaxContext _evolvedtaxContext;
        public InstituteController(IInstituteService instituteService, IMailService emailService, IWebHostEnvironment webHostEnvironment, ICommonService commonService, EvolvedtaxContext evolvedtaxContext)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _commonService = commonService;
            _instituteService = instituteService;
            _emailService = emailService;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Master()
        {
            return View(_instituteService.GetMaster());
        }
        public IActionResult Entities()
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
            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            return View(_instituteService.GetEntitiesByInstId(InstId));
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
        public IActionResult ClientsRecyleBin(int entityId)
        {
            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            return View(_instituteService.GetRecyleBinClientsByEntityId(InstId, entityId));
        }
        public IActionResult RestoreClient(int[] selectedValues)
        {
            return Json(_instituteService.RestoreClients(selectedValues));
        }
        public IActionResult Client(int EntityId)
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
            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var entities = _instituteService.GetEntitiesByInstId(InstId);
            ViewBag.EntitiesList = entities.Select(p => new SelectListItem
            {
                Text = p.EntityName,
                Value = p.EntityId.ToString(),
                Selected = (p.EntityId == EntityId)
            });
            ViewBag.EmailFrequency = entities?.FirstOrDefault(p => p.EntityId == EntityId)?.EmailFrequency;
            ViewBag.EntityId = EntityId;
            return View(_instituteService.GetClientByEntityId(InstId, EntityId));
        }

        public IActionResult ChangeEntity(int entityId)
        {
            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var response = _instituteService.GetClientByEntityId(InstId, entityId);
            var EmailFrequency = _evolvedtaxContext.InstituteEntities.FirstOrDefault(p => p.EntityId == entityId)?.EmailFrequency;
            return Json(new { Data = response, EmailFrequency = EmailFrequency });
        }

        public async Task<IActionResult> SendEmail(int[] selectedValues)
        {
            var scheme = HttpContext.Request.Scheme; // "http" or "https"
            var host = HttpContext.Request.Host.Value; // Hostname (e.g., example.com)
            var fullUrl = $"{scheme}://{host}";

            string URL = Path.Combine(fullUrl, "Account", "OTP");
            await _emailService.SendEmailAsync(_instituteService.GetClientInfoByClientId(selectedValues), "Action Required: Verify Your Registration with EvoTax Portal", "", URL);
            return Json(new { type = ResponseMessageConstants.SuccessStatus, message = ResponseMessageConstants.SuccessEmailSend });
        }
        [Route("institute/uploadEntities")]
        [HttpPost]
        public async Task<IActionResult> UploadEntities(IFormFile file)
        {
            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var instName = HttpContext.Session.GetString("InstituteName") ?? string.Empty;
            var response = await _instituteService.UploadEntityData(file, instId, instName);
            return Json(response);
        }

        [Route("institute/uploadClients")]
        [HttpPost]
        public async Task<IActionResult> UploadClients(IFormFile file, int EntityId)
        {
            if (file == null)
            {
                return Json(false);
            }
            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var response = await _instituteService.UploadClientData(file, instId, EntityId);
            return Json(response);
        }

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
        [Route("institute/UpdateEntity")]
        [HttpPost]
        public async Task<IActionResult> UpdateEntity(InstituteEntityRequest request)
        {
            if (request.EntityId == 0)
            {
                return Json(false);
            }
            var response = await _instituteService.UpdateEntity(request);
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
        [Route("institute/UpdateClient")]
        [HttpPost]
        public async Task<IActionResult> UpdateClient(InstituteClientRequest request)
        {
            if (request.ClientId == 0)
            {
                return Json(false);
            }
            var response = await _instituteService.UpdateClient(request);
            return Json(response);
        }
        [Route("institute/UpdateEmailFrequency")]
        [HttpPost]
        public async Task<IActionResult> UpdateEmailFrequency(int EntityId, int EmailFrequency)
        {
            if (EntityId == 0)
            {
                return Json(false);
            }
            var response = await _instituteService.UpdateEmailFrequncy(EntityId, EmailFrequency);
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
    }
}