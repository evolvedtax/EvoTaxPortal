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
        private readonly IForm1099_MISC_Service _form1099_MISC_Service;
        private readonly IForm1099_NEC_Service _form1099_NEC_Service;
        private readonly IForm1099_INT_Service _form1099_INT_Service;
        private readonly IForm1099_A_Service _form1099_A_Service;
        private readonly IForm1099_B_Service _form1099_B_Service;
        private readonly IForm1099_C_Service _form1099_C_Service;
        private readonly IForm1099_G_Service _form1099_G_Service;
        private readonly IForm1099_CAP_Service _form1099_CAP_Service;
        private readonly IForm1099_DIV_Service _form1099_DIV_Service;
        private readonly IForm1099_LS_Service _form1099_LS_Service;
        private readonly IForm1099_LTC_Service _form1099_LTC_Service;
        private readonly IForm1099_PATR_Service _form1099_PATR_Service;
        private readonly IForm1099_R_Service _form1099_R_Service;
        private readonly IForm1099_SA_Service _form1099_SA_Service;
        private readonly IForm1099_SB_Service _form1099_SB_Service;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CommonController(IWebHostEnvironment webHostEnvironment, ICommonService commonService,
            IForm1099_MISC_Service form1099_MISC_Service, IForm1099_NEC_Service form1099_NEC_Service,
            IForm1099_INT_Service form1099_INT_Service, IForm1099_A_Service form1099_A_Service,
            IForm1099_B_Service form1099_B_Service, IForm1099_C_Service form1099_C_Service, IForm1099_CAP_Service form1099_CAP_Service,
            IForm1099_G_Service form1099_G_Service, IForm1099_DIV_Service form1099_DIV_Service, 
            IForm1099_LS_Service form1099_LS_Service, IForm1099_LTC_Service form1099_LTC_Service,
            IForm1099_PATR_Service form1099_PATR_Service, IForm1099_R_Service form1099_R_Service,
            IForm1099_SA_Service form1099_SA_Service, IForm1099_SB_Service form1099_SB_Service)
        {
            _webHostEnvironment = webHostEnvironment;
            _commonService = commonService;
            _form1099_MISC_Service = form1099_MISC_Service;
            _form1099_NEC_Service = form1099_NEC_Service;
            _form1099_INT_Service = form1099_INT_Service;
            _form1099_A_Service = form1099_A_Service;
            _form1099_B_Service = form1099_B_Service;
            _form1099_DIV_Service = form1099_DIV_Service;
            _form1099_LS_Service = form1099_LS_Service;
            _form1099_C_Service = form1099_C_Service;
            _form1099_CAP_Service = form1099_CAP_Service;
            _form1099_G_Service = form1099_G_Service;
            _form1099_G_Service = form1099_G_Service;
            _form1099_LTC_Service = form1099_LTC_Service;
            _form1099_PATR_Service = form1099_PATR_Service;
            _form1099_R_Service = form1099_R_Service;
            _form1099_SA_Service = form1099_SA_Service;
            _form1099_SB_Service = form1099_SB_Service;
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
                case AppConstants.Form1099NEC:
                    fileName = AppConstants.Form1099NECExcelTemplate;
                    break;
                case AppConstants.Form1099INT:
                    fileName = AppConstants.Form1099INTExcelTemplate;
                    break;
                case AppConstants.Form1099A:
                    fileName = AppConstants.Form1099AExcelTemplate;
                    break;
                case AppConstants.Form1099B:
                    fileName = AppConstants.Form1099_B_ExcelTemplate;
                    break;
                case AppConstants.Form1099DIV:
                    fileName = AppConstants.Form1099DIVExcelTemplate;
                    break;
                case AppConstants.Form1099LS:
                    fileName = AppConstants.Form1099LSExcelTemplate;
                    break;
                case AppConstants.Form1099C:
                    fileName = AppConstants.Form1099_C_ExcelTemplate;
                    break;
                case AppConstants.Form1099CAP:
                    fileName = AppConstants.Form1099_CAP_ExcelTemplate;
                    break;
                case AppConstants.Form1099G:
                    fileName = AppConstants.Form1099_G_ExcelTemplate;
                    break;
                case AppConstants.Form1099LTC:
                    fileName = AppConstants.Form1099_LTC_ExcelTemplate;
                    break;
                case AppConstants.Form1099PATR:
                    fileName = AppConstants.Form1099_PATR_ExcelTemplate;
                    break;
                case AppConstants.Form1099R:
                    fileName = AppConstants.Form1099_R_ExcelTemplate;
                    break;
                case AppConstants.Form1099SA:
                    fileName = AppConstants.Form1099_SA_ExcelTemplate;
                    break;
                case AppConstants.Form1099SB:
                    fileName = AppConstants.Form1099_SB_ExcelTemplate;
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
        public async Task<IActionResult> SendLinkToRecipients(int[] selectedValues, string form)
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
            switch (form)
            {
                case AppConstants.Form1099MISC:
                    await _form1099_MISC_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099MISC);
                    break;
                case AppConstants.Form1099NEC:
                    await _form1099_NEC_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099NEC);
                    break;
                case AppConstants.Form1099INT:
                    await _form1099_INT_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099INT);
                    break;
                case AppConstants.Form1099B:
                    await _form1099_B_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099B);
                    break;
                case AppConstants.Form1099A:
                    await _form1099_A_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099A);
                    break;
                case AppConstants.Form1099DIV:
                    await _form1099_DIV_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099DIV);
                    break;
                case AppConstants.Form1099LS:
                    await _form1099_LS_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099LS);
                    break;
                case AppConstants.Form1099C:
                    await _form1099_C_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099C);
                    break;
                case AppConstants.Form1099CAP:
                    await _form1099_CAP_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099CAP);
                    break;
                case AppConstants.Form1099G:
                    await _form1099_G_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099G);
                    break;
                case AppConstants.Form1099LTC:
                    await _form1099_LTC_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099LTC);
                    break;
                case AppConstants.Form1099PATR:
                    await _form1099_PATR_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099PATR);
                    break;
                case AppConstants.Form1099R:
                    await _form1099_R_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099R);
                    break;
                case AppConstants.Form1099SA:
                    await _form1099_SA_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099SA);
                    break;
                case AppConstants.Form1099SB:
                    await _form1099_SB_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099SB);
                    break;
            }

            return Json(new { type = ResponseMessageConstants.SuccessStatus, message = ResponseMessageConstants.SuccessEmailSend });
        }
    }
}
