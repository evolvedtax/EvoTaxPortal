using EvolvedTax.Business.Services.AnnouncementService;
using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.Form1042Services;
using EvolvedTax.Business.Services.Form1099Services;
using EvolvedTax.Business.Services.Form3921Services;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Helpers;
using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax_Institute.Areas._1099.Controllers
{
    [Area("1042")]
    public class Form1042_S_Controller : BaseController
    {
        private readonly IForm1042_S_Service _form1042_Service;
        private readonly IInstituteService _instituteService;
        private readonly ICommonService _commonService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public Form1042_S_Controller(IForm1042_S_Service form1042_Service, IWebHostEnvironment webHostEnvironment, ICommonService commonService, IInstituteService instituteService)
        {
            _form1042_Service = form1042_Service;
            _webHostEnvironment = webHostEnvironment;
            _commonService = commonService;
            _instituteService = instituteService;
        }
        public IActionResult Index()
        {
            var EntityId = HttpContext.Session.GetInt32("EntityId") ?? 0;
            ViewBag.EntitiesList = _instituteService.GetEntitiesByInstId(SessionUser.InstituteId, Convert.ToInt32(AppConstants.FormSubscription_1042)).Select(p => new SelectListItem
            {
                Text = p.EntityName,
                Value = p.EntityId.ToString(),
                Selected = p.EntityId == EntityId
            });
            return View(_form1042_Service.GetForm1042SList().Where(p => p.EntityId == EntityId));
        }
        [Route("Form1042_S_/uploadClients")]
        [HttpPost]
        public async Task<IActionResult> UploadClients(IFormFile file, int EntityId, string entityName)
        {
            if (file == null)
            {
                return Json(false);
            }
            var response = await _form1042_Service.Upload1042_S_Data(file, SessionUser.InstituteId, EntityId, SessionUser.UserId);
            return Json(response);
        }
        public IActionResult ChangeEntity(int entityId)
        {
            //int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            HttpContext.Session.SetInt32("EntityId", entityId);
            var response = _form1042_Service.GetForm1042SList().Where(p => p.EntityId == entityId);
            //return RedirectToAction("Index");
            return Json(new { Data = "true" });
        }
        #region PDF Creation Methods
        [HttpGet]
        public IActionResult DownlodPdf(int Id)
        {
            string TemplatePathFile = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.Form1042STemplateFileName);
            string SavePathFolder = Path.Combine(_webHostEnvironment.WebRootPath, AppConstants.Form1042S);
            var entityId = HttpContext.Session.GetInt32("EntityId") ?? 0;
            string pdfUrl = _form1042_Service.GeneratePdf(Id, TemplatePathFile, SavePathFolder, entityId);
            return Json(pdfUrl);
        }

        [HttpPost]

        [Route("Form1042_S_/DownloadAll")]
        public IActionResult DownloadAll([FromBody] DownloadRequestModel model)
        {

            List<int> ids = model.ids;
            List<string> selectedPage = model.selectedPage;
            string RootPath = _webHostEnvironment.WebRootPath;
            string SavePathFolder = Path.Combine(_webHostEnvironment.WebRootPath, AppConstants.Form1042S);
            var entityId = HttpContext.Session.GetInt32("EntityId") ?? 0;
            var zipFilePath = _form1042_Service.GenerateAndZipPdfs(ids, SavePathFolder, selectedPage, RootPath, entityId);
            string contentType = "application/zip";

            var fileBytes = System.IO.File.ReadAllBytes(zipFilePath);
            return File(fileBytes, contentType, "GeneratedPDFs.zip");


        }

        [HttpPost]
        [Route("Form1042_S_/DownloadOneFile")]
        public IActionResult DownloadOneFile([FromBody] DownloadRequestModel model)
        {

            List<int> ids = model.ids;
            List<string> selectedPage = model.selectedPage;
            string RootPath = _webHostEnvironment.WebRootPath;
            string SavePathFolder = Path.Combine(_webHostEnvironment.WebRootPath, AppConstants.Form1042S);
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
            var zipFilePath = _form1042_Service.DownloadOneFile(ids, SavePathFolder, selectedPage, RootPath, entityId);
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
            var response = await _form1042_Service.KeepRecord(id);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRecord(int id)
        {
            if (id == 0)
            {
                return Json(false);
            }
            var response = await _form1042_Service.DeletePermeant(id);
            return Json(response);
        }
    }
}
