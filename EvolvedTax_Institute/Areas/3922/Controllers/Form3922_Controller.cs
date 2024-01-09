using EvolvedTax.Business.Services.AnnouncementService;
using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.Form1042Services;
using EvolvedTax.Business.Services.Form1099Services;
using EvolvedTax.Business.Services.Form3922Services;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Helpers;
using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax_Institute.Areas._3922.Controllers
{
    [Area("3922")]
    public class Form3922_Controller : BaseController
    {
        private readonly IForm3922_Service _form3922_Service;
        private readonly IInstituteService _instituteService;
        private readonly ICommonService _commonService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public Form3922_Controller(IForm3922_Service form3922_Service, IWebHostEnvironment webHostEnvironment, ICommonService commonService, IInstituteService instituteService)
        {
            _form3922_Service = form3922_Service;
            _webHostEnvironment = webHostEnvironment;
            _commonService = commonService;
            _instituteService = instituteService;
        }
        public IActionResult Index(int entityId)
        {
            var EntityId = entityId;

            //var EntityId = HttpContext.Session.GetInt32("EntityId") ?? 0;
            ViewBag.EntitiesList = _instituteService.GetEntitiesByInstId(SessionUser.InstituteId, Convert.ToInt32(AppConstants.FormSubscription_3922)).Select(p => new SelectListItem
            {
                Text = p.EntityName,
                Value = p.EntityId.ToString(),
                Selected = p.EntityId == EntityId
            });
            return View(_form3922_Service.GetFormList().Where(p => p.EntityId == EntityId));
        }
        [Route("Form3922_/uploadClients")]
        [HttpPost]
        public async Task<IActionResult> UploadClients(IFormFile file, int EntityId, string entityName)
        {
            if (file == null)
            {
                return Json(false);
            }
            var response = await _form3922_Service.Upload_Data(file, SessionUser.InstituteId, EntityId, SessionUser.UserId);
            return Json(response);
        }
        public IActionResult ChangeEntity(int entityId)
        {
            //int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            HttpContext.Session.SetInt32("EntityId", entityId);
            var response = _form3922_Service.GetFormList().Where(p => p.EntityId == entityId);
            //return RedirectToAction("Index");
            return Json(new { Data = "true" });
        }
        #region PDF Creation Methods
        [HttpGet]
        public IActionResult DownlodPdf(int Id)
        {
            string TemplatePathFile = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.Form3922TemplateFileName);
            string SavePathFolder = Path.Combine(_webHostEnvironment.WebRootPath, AppConstants.Form3922);
            var entityId = HttpContext.Session.GetInt32("EntityId") ?? 0;
            string pdfUrl = _form3922_Service.GeneratePdf(Id, TemplatePathFile, SavePathFolder, entityId);
            return Json(pdfUrl);
        }

        [HttpPost]

        [Route("Form3922_/DownloadAll")]
        public IActionResult DownloadAll([FromBody] DownloadRequestModel model)
        {

            List<int> ids = model.ids;
            List<string> selectedPage = model.selectedPage;
            string RootPath = _webHostEnvironment.WebRootPath;
            string SavePathFolder = Path.Combine(_webHostEnvironment.WebRootPath, AppConstants.Form3922);
            var entityId = HttpContext.Session.GetInt32("EntityId") ?? 0;
            var zipFilePath = _form3922_Service.GenerateAndZipPdfs(ids, SavePathFolder, selectedPage, RootPath, entityId);
            string contentType = "application/zip";

            var fileBytes = System.IO.File.ReadAllBytes(zipFilePath);
            return File(fileBytes, contentType, "GeneratedPDFs.zip");


        }

        [HttpPost]
        [Route("Form3922_/DownloadOneFile")]
        public IActionResult DownloadOneFile([FromBody] DownloadRequestModel model)
        {

            List<int> ids = model.ids;
            List<string> selectedPage = model.selectedPage;
            string RootPath = _webHostEnvironment.WebRootPath;
            string SavePathFolder = Path.Combine(_webHostEnvironment.WebRootPath, AppConstants.Form3922);
            //   bool containsAll = selectedPage.Contains("All");

            //    if (containsAll)
            //    {
            //    selectedPage.Clear();
            //    selectedPage.Add("2");
            //    selectedPage.Add("3");
            //    selectedPage.Add("4");
            //    selectedPage.Add("6");
            //    selectedPage.Add("7");
            //}
            var entityId = HttpContext.Session.GetInt32("EntityId") ?? 0;
            var zipFilePath = _form3922_Service.DownloadOneFile(ids, SavePathFolder, selectedPage, RootPath, entityId);
            string contentType = "application/zip";

            var fileBytes = System.IO.File.ReadAllBytes(zipFilePath);
            return File(fileBytes, contentType, "GeneratedPDFs.zip");


        }
        #endregion


        [HttpPost]
        public async Task<IActionResult> KeepRecord(int id)
        {
            if (id == 0)
            {
                return Json(false);
            }
            var response = await _form3922_Service.KeepRecord(id);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRecord(int id)
        {
            if (id == 0)
            {
                return Json(false);
            }
            var response = await _form3922_Service.DeletePermeant(id);
            return Json(response);
        }
    }
}
