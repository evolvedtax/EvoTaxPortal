﻿using System.Text;
using EvolvedTax.Business.MailService;
using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Common.Constants;
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
        public InstituteController(IInstituteService instituteService, IMailService emailService, IWebHostEnvironment webHostEnvironment, ICommonService commonService)
        {
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
            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            return View(_instituteService.GetEntitiesByInstId(InstId));
        }
        public IActionResult Client(int EntityId)
        {
            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            ViewBag.EntitiesList = _instituteService.GetEntitiesByInstId(InstId).Select(p => new SelectListItem
            {
                Text = p.EntityName,
                Value = p.EntityId.ToString(),
                Selected = (p.EntityId == EntityId)
            });
            ViewBag.EntityId = EntityId;
            return View(_instituteService.GetClientByEntityId(InstId, EntityId));
        }

        public IActionResult ChangeEntity(int entityId)
        {
            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var response = _instituteService.GetClientByEntityId(InstId, entityId);
            return Json(response);
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

            filePath = Path.Combine(_webHostEnvironment.WebRootPath,"Templates", fileName);

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