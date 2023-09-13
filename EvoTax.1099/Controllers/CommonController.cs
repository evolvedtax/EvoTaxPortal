using EvolvedTax.Business.MailService;
using EvolvedTax.Business.Services.AnnouncementService;
using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.Form1099Services;
using EvolvedTax.Business.Services.W9FormService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Helpers;
using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax_1099.Controllers
{
    public class CommonController : BaseController
    {
        private readonly ICommonService _commonService;
        private readonly IMailService _mailService;
        private readonly ITrailAudit1099Service _trailAudit1099Service;
        private readonly IForm1099_MISC_Service _form1099_MISC_Service;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CommonController(IWebHostEnvironment webHostEnvironment, ICommonService commonService, IMailService mailService, ITrailAudit1099Service trailAudit1099Service, IForm1099_MISC_Service form1099_MISC_Service)
        {
            _webHostEnvironment = webHostEnvironment;
            _commonService = commonService;
            _mailService = mailService;
            _trailAudit1099Service = trailAudit1099Service;
            _form1099_MISC_Service = form1099_MISC_Service;
        }
        [HttpGet]
        public IActionResult DownloadExcel(string fileType)
        {
            string fileName;
            string filePath;

            switch (fileType)
            {
                case AppConstants.Form1099MISC:
                    fileName = AppConstants.Form1099MISCExcelTemplate;
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
        [Route("common/SendEmailstoRecipients")]
        [HttpPost]
        public async Task<IActionResult> SendLinkToRecipients(int[] selectedValues)
        {
            var scheme = HttpContext.Request.Scheme; // "http" or "https"
            var host = string.Empty;
            if (_webHostEnvironment.IsDevelopment())
            {
                //host = HttpContext.Request.Host.Value;
                host = "localhost:7228";
            }
            else
            {
                host = URLConstants.RecipientUrl; // Hostname (e.g., example.com)
            }
            var fullUrl = $"{scheme}://{host}";
            string URL = string.Concat(fullUrl, "/Account", "/OTP");
            await _mailService.SendElectronicAcceptanceEmail(_form1099_MISC_Service.GetRecipientEmailsByIds(selectedValues), "","Action Required",URL);

            return Json(new { type = ResponseMessageConstants.SuccessStatus, message = ResponseMessageConstants.SuccessEmailSend });
        }
    }
}
