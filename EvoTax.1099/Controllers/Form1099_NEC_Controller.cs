using Azure;
using EvolvedTax.Business.Services.AnnouncementService;
using EvolvedTax.Business.Services.Form1099Services;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.DTOs.ViewModels;
using EvolvedTax.Data.Models.Entities._1099;
using EvolvedTax.Helpers;
using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax_1099.Controllers
{
    public class Form1099_NEC_Controller : BaseController
    {
        private readonly IForm1099_NEC_Service _form1099_NEC_Service;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public Form1099_NEC_Controller(IForm1099_NEC_Service form1099_NEC_Service, IWebHostEnvironment webHostEnvironment)
        {
            _form1099_NEC_Service = form1099_NEC_Service;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var instId = SessionUser.InstituteId; // Get the InstituteId from your session
            var data = _form1099_NEC_Service.GetRecodByInstId(instId).ToList();
            return View(data);
        }
        [Route("Form1099_NEC_/uploadClients")]
        [HttpPost]
        public async Task<IActionResult> UploadClients(IFormFile file, int EntityId, string entityName)
        {
            if (file == null)
            {
                return Json(false);
            }
            var response = await _form1099_NEC_Service.Upload1099_NEC_Data(file, SessionUser.InstituteId, SessionUser.UserId);
            return Json(response);
        }


        public IActionResult downlodPdf(int id)
        {
            string TemplatePathFile = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.NEC_1099_TemplateFileName);
            string SavePathFolder = Path.Combine(_webHostEnvironment.WebRootPath, "1099NEC");
            string pdfUrl = _form1099_NEC_Service.GeneratePdf(id, TemplatePathFile, SavePathFolder);
            return Json(pdfUrl);

        }




    }
}
