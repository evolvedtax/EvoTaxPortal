using Azure;
using EvolvedTax.Business.Services.AnnouncementService;
using EvolvedTax.Business.Services.Form1099Services;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.DTOs.ViewModels;
using EvolvedTax.Data.Models.Entities._1099;
using EvolvedTax.Helpers;
using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace EvolvedTax_1099.Controllers
{
    public class Form1099_NEC_Controller : BaseController
    {
        private readonly IForm1099_NEC_Service _form1099_NEC_Service;
        private readonly IInstituteService _instituteService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public Form1099_NEC_Controller(IForm1099_NEC_Service form1099_NEC_Service, IWebHostEnvironment webHostEnvironment, IInstituteService instituteService)
        {
            _form1099_NEC_Service = form1099_NEC_Service;
            _webHostEnvironment = webHostEnvironment;
            _instituteService = instituteService;
        }
        public IActionResult Index()
        {
            var EntityId = HttpContext.Session.GetInt32("EntityId") ?? 0;
            ViewBag.EntitiesList = _instituteService.GetEntitiesByInstId(SessionUser.InstituteId).Select(p => new SelectListItem
            {
                Text = p.EntityName,
                Value = p.EntityId.ToString(),
                Selected = p.EntityId == EntityId
            });
            return View(_form1099_NEC_Service.GetForm1099NECList().Where(p => p.EntityId == EntityId));
        }
        [Route("Form1099_NEC_/uploadClients")]
        [HttpPost]
        public async Task<IActionResult> UploadClients(IFormFile file, int EntityId, string entityName)
        {
            if (file == null)
            {
                return Json(false);
            }
            var response = await _form1099_NEC_Service.Upload1099_NEC_Data(file, EntityId, SessionUser.InstituteId, SessionUser.UserId);
            return Json(response);
        }
        public IActionResult ChangeEntity(int entityId)
        {
            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var response = _form1099_NEC_Service.GetForm1099NECList().Where(p => p.EntityId == entityId);
            return Json(new { Data = response });
        }

        #region PDF Creation Methods
        public IActionResult downlodPdf(int id)
        {
            string TemplatePathFile = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.NEC_1099_TemplateFileName);
            string SavePathFolder = Path.Combine(_webHostEnvironment.WebRootPath, "1099NEC");
            string pdfUrl = _form1099_NEC_Service.GeneratePdf(id, TemplatePathFile, SavePathFolder);
            return Json(pdfUrl);

        }


        [HttpPost]
        [Route("DownloadAll")]
        public IActionResult DownloadAll([FromBody] DownloadAllRequestModel model)
        {

            List<int> ids = model.ids;
            List<string> selectedPage = model.selectedPage;
            string RootPath = _webHostEnvironment.WebRootPath;
            string SavePathFolder = Path.Combine(_webHostEnvironment.WebRootPath, "1099NEC");
            var zipFilePath = _form1099_NEC_Service.GenerateAndZipPdfs(ids, SavePathFolder, selectedPage, RootPath);
            string contentType = "application/zip";

            var fileBytes = System.IO.File.ReadAllBytes(zipFilePath);
            return File(fileBytes, contentType, "GeneratedPDFs.zip");


        }


        [HttpPost]
        [Route("DownloadOneFile")]
        public IActionResult DownloadOneFile([FromBody] DownloadAllRequestModel model)
        {

            List<int> ids = model.ids;
            List<string> selectedPage = model.selectedPage;
            string RootPath = _webHostEnvironment.WebRootPath;
            string SavePathFolder = Path.Combine(_webHostEnvironment.WebRootPath, "1099NEC");
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
            var zipFilePath = _form1099_NEC_Service.DownloadOneFile(ids, SavePathFolder, selectedPage, RootPath);
            string contentType = "application/zip";

            var fileBytes = System.IO.File.ReadAllBytes(zipFilePath);
            return File(fileBytes, contentType, "GeneratedPDFs.zip");


        }
        #endregion


        [Route("orm1099_NEC_/KeepRecord")]
        [HttpPost]
        public async Task<IActionResult> KeepRecord(int id)
        {
            if (id == 0)
            {
                return Json(false);
            }
            var response = await _form1099_NEC_Service.KeepRecord(id);
            return Json(response);
        }

        [Route("Form1099_NEC/DeleteRecord")]
        [HttpPost]
        public async Task<IActionResult> DeleteRecord(int id)
        {
            if (id == 0)
            {
                return Json(false);
            }
            var response = await _form1099_NEC_Service.DeletePermeant(id);
            return Json(response);
        }




    }
    public class DownloadAllRequestModel
    {
        public List<int> ids { get; set; }
        public List<string> selectedPage { get; set; }
    }
}
