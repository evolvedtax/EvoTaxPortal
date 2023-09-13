using EvolvedTax.Business.Services.AnnouncementService;
using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.Form1099Services;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Business.Services.W9FormService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Helpers;
using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax_1099.Controllers
{
    public class Form1099_MISC_Controller : BaseController
    {
        private readonly IForm1099_MISC_Service _form1099_MISC_Service;
        private readonly IInstituteService _instituteService;
        private readonly ICommonService _commonService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public Form1099_MISC_Controller(IForm1099_MISC_Service form1099_MISC_Service, IWebHostEnvironment webHostEnvironment, ICommonService commonService, IInstituteService instituteService)
        {
            _form1099_MISC_Service = form1099_MISC_Service;
            _webHostEnvironment = webHostEnvironment;
            _commonService = commonService;
            _instituteService = instituteService;
        }
        public IActionResult Index()
        {
            var EntityId = HttpContext.Session.GetInt32("EntityId") ?? 0;
            ViewBag.EntitiesList = _instituteService.GetEntitiesByInstId(SessionUser.InstituteId).Select(p => new SelectListItem{
                Text = p.EntityName,
                Value = p.EntityId.ToString(),
                Selected = p.EntityId == EntityId
            });
            return View(_form1099_MISC_Service.GetForm1099MISCList().Where(p => p.EntityId == EntityId));
        }
        [Route("Form1099_MISC_/uploadClients")]
        [HttpPost]
        public async Task<IActionResult> UploadClients(IFormFile file, int EntityId, string entityName)
        {
            if (file == null)
            {
                return Json(false);
            }
            var response = await _form1099_MISC_Service.Upload1099_MISC_Data(file, SessionUser.InstituteId, EntityId, SessionUser.UserId);
            return Json(response);
        }
        public IActionResult ChangeEntity(int entityId)
        {
            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var response = _form1099_MISC_Service.GetForm1099MISCList().Where(p => p.EntityId == entityId);
            return Json(new { Data = response });
        }
        [Route("Form1099_MISC_/downlodPdf")]
        [HttpGet]
        public IActionResult DownlodPdf(int Id)
        {
            var response = _form1099_MISC_Service.GeneratePdf(Id, _webHostEnvironment.WebRootPath);
            return Json(response);
        }
    }
}
