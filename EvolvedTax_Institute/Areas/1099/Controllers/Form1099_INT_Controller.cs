using EvolvedTax.Business.Services.AnnouncementService;
using EvolvedTax.Business.Services.Form1099Services;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Helpers;
using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax_Institute.Areas._1099.Controllers
{
    [Area("1099")]
    public class Form1099_INT_Controller : BaseController
    {
        private readonly IForm1099_INT_Service _form1099_INT_Service;
        private readonly IInstituteService _instituteService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public Form1099_INT_Controller(IForm1099_INT_Service Form1099_INT_Service, IWebHostEnvironment webHostEnvironment, IInstituteService instituteService)
        {
            _form1099_INT_Service = Form1099_INT_Service;
            _webHostEnvironment = webHostEnvironment;
            _instituteService = instituteService;
        }
        public IActionResult Index()
        {
            var EntityId = HttpContext.Session.GetInt32("EntityId") ?? 0;
            var InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            ViewBag.EntitiesList = _instituteService.GetEntitiesByInstId(SessionUser.InstituteId).Select(p => new SelectListItem
            {
                Text = p.EntityName,
                Value = p.EntityId.ToString(),
                Selected = p.EntityId == EntityId
            });
            return View(_form1099_INT_Service.GetForm1099INTList().Where(p => p.EntityId == EntityId && p.InstID== InstId));
        }
        [Route("Form1099_INT_/uploadClients")]
        [HttpPost]
        public async Task<IActionResult> UploadClients(IFormFile file, int EntityId, string entityName)
        {
            if (file == null)
            {
                return Json(false);
            }
            var response = await _form1099_INT_Service.Upload1099_Data(file, EntityId, SessionUser.InstituteId, SessionUser.UserId);
            return Json(response);
        }
        public IActionResult ChangeEntity(int entityId)
        {
            HttpContext.Session.SetInt32("EntityId", entityId);
            return Json(new { Data = "true" }); ;
        }

        #region PDF Creation Methods
        public IActionResult downlodPdf(int id)
        {
            string TemplatePathFile = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.INT_1099_TemplateFileName);
            string SavePathFolder = Path.Combine(_webHostEnvironment.WebRootPath, "1099INT");
            string pdfUrl = _form1099_INT_Service.GeneratePdf(id, TemplatePathFile, SavePathFolder);
            return Json(pdfUrl);

        }


        [HttpPost]
        [Route("Form1099_INT_/DownloadAll")]
        public IActionResult DownloadAll([FromBody] DownloadRequestModel model)
        {

            List<int> ids = model.ids;
            List<string> selectedPage = model.selectedPage;
            string RootPath = _webHostEnvironment.WebRootPath;
            string SavePathFolder = Path.Combine(_webHostEnvironment.WebRootPath, "1099INT");
            var zipFilePath = _form1099_INT_Service.GenerateAndZipPdfs(ids, SavePathFolder, selectedPage, RootPath);
            string contentType = "application/zip";

            var fileBytes = System.IO.File.ReadAllBytes(zipFilePath);
            return File(fileBytes, contentType, "GeneratedPDFs.zip");


        }


        [HttpPost]
        [Route("Form1099_INT_/DownloadOneFile")]
        public IActionResult DownloadOneFile([FromBody] DownloadRequestModel model)
        {

            List<int> ids = model.ids;
            List<string> selectedPage = model.selectedPage;
            string RootPath = _webHostEnvironment.WebRootPath;
            string SavePathFolder = Path.Combine(_webHostEnvironment.WebRootPath, "1099INT");
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
            var zipFilePath = _form1099_INT_Service.DownloadOneFile(ids, SavePathFolder, selectedPage, RootPath);
            string contentType = "application/zip";

            var fileBytes = System.IO.File.ReadAllBytes(zipFilePath);
            return File(fileBytes, contentType, "GeneratedPDFs.zip");


        }
        #endregion


        [Route("Form1099_INT_/KeepRecord")]
        [HttpPost]
        public async Task<IActionResult> KeepRecord(int id)
        {
            if (id == 0)
            {
                return Json(false);
            }
            var response = await _form1099_INT_Service.KeepRecord(id);
            return Json(response);
        }

        [Route("Form1099_INT_/DeleteRecord")]
        [HttpPost]
        public async Task<IActionResult> DeleteRecord(int id)
        {
            if (id == 0)
            {
                return Json(false);
            }
            var response = await _form1099_INT_Service.DeletePermeant(id);
            return Json(response);
        }

    }
}
