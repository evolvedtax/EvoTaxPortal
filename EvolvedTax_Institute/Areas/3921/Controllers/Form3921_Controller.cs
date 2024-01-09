using EvolvedTax.Business.Services.AnnouncementService;
using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.Form3921Services;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Helpers;
using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax_Institute.Areas._3921.Controllers
{
    [Area("3921")]
    public class Form3921_Controller : BaseController
    {
        private readonly IForm3921Service _form3921Service;
        private readonly IInstituteService _instituteService;
        private readonly ICommonService _commonService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public Form3921_Controller(IForm3921Service form3921_Service, IWebHostEnvironment webHostEnvironment, ICommonService commonService, IInstituteService instituteService)
        {
            _form3921Service = form3921_Service;
            _webHostEnvironment = webHostEnvironment;
            _commonService = commonService;
            _instituteService = instituteService;
        }
        public IActionResult Index(int entityId)
        {
            var EntityId = entityId;
            //var EntityId = HttpContext.Session.GetInt32("EntityId") ?? 0;
            HttpContext.Session.SetInt32("EntityId", EntityId);
            ViewBag.EntitiesList = _instituteService.GetEntitiesByInstId(SessionUser.InstituteId, Convert.ToInt32(AppConstants.FormSubscription_3921)).Select(p => new SelectListItem
            {
                Text = p.EntityName,
                Value = p.EntityId.ToString(),
                Selected = p.EntityId == EntityId
            });
            return View(_form3921Service.GetForm3921List().Where(p => p.EntityId == EntityId));
        }
        [Route("Form3921_/uploadClients")]
        [HttpPost]
        public async Task<IActionResult> UploadClients(IFormFile file, int EntityId, string entityName)
        {
            if (file == null)
            {
                return Json(false);
            }
            var response = await _form3921Service.Upload3921Data(file, SessionUser.InstituteId, EntityId, SessionUser.UserId);
            return Json(response);
        }
        public IActionResult ChangeEntity(int entityId)
        {
            //int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            HttpContext.Session.SetInt32("EntityId", entityId);
            var response = _form3921Service.GetForm3921List().Where(p => p.EntityId == entityId);
            //return RedirectToAction("Index");
            return Json(new { Data = "true" });
        }
        #region PDF Creation Methods
        [HttpGet]
        public IActionResult DownlodPdf(int Id)
        {
            string TemplatePathFile = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.Form3921TemplateFileName);
            string SavePathFolder = Path.Combine(_webHostEnvironment.WebRootPath, AppConstants.Form3921);
            var entityId = HttpContext.Session.GetInt32("EntityId") ?? 0;
            string pdfUrl = _form3921Service.GeneratePdf(Id, TemplatePathFile, SavePathFolder, entityId);
            return Json(pdfUrl);
        }

        [HttpPost]

        [Route("Form3921_/DownloadAll")]
        public IActionResult DownloadAll([FromBody] DownloadRequestModel model)
        {

            List<int> ids = model.ids;
            List<string> selectedPage = model.selectedPage;
            string RootPath = _webHostEnvironment.WebRootPath;
            string SavePathFolder = Path.Combine(_webHostEnvironment.WebRootPath, AppConstants.Form3921);
            var entityId = HttpContext.Session.GetInt32("EntityId") ?? 0;
            var zipFilePath = _form3921Service.GenerateAndZipPdfs(ids, SavePathFolder, selectedPage, RootPath, entityId);
            string contentType = "application/zip";

            var fileBytes = System.IO.File.ReadAllBytes(zipFilePath);
            return File(fileBytes, contentType, "GeneratedPDFs.zip");


        }

        [HttpPost]
        [Route("Form3921_/DownloadOneFile")]
        public IActionResult DownloadOneFile([FromBody] DownloadRequestModel model)
        {

            List<int> ids = model.ids;
            List<string> selectedPage = model.selectedPage;
            string RootPath = _webHostEnvironment.WebRootPath;
            string SavePathFolder = Path.Combine(_webHostEnvironment.WebRootPath, AppConstants.Form3921);
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
            var zipFilePath = _form3921Service.DownloadOneFile(ids, SavePathFolder, selectedPage, RootPath, entityId);
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
            var response = await _form3921Service.KeepRecord(id);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRecord(int id)
        {
            if (id == 0)
            {
                return Json(false);
            }
            var response = await _form3921Service.DeletePermeant(id);
            return Json(response);
        }
    }
}
