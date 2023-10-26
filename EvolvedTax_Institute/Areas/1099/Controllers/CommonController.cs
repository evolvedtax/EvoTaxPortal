﻿using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.Form1099Services;
using EvolvedTax.Common.Constants;
using EvolvedTax.Web.Controllers;

namespace EvolvedTax_Institute.Areas._1099.Controllers
{
    [Area("1099")]
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
        private readonly IForm1099_K_Service _form1099_K_Service;
        private readonly IForm1099_OID_Service _form1099_OID_Service;
        private readonly IForm1099_Q_Service _form1099_Q_Service;
        private readonly IForm1099_S_Service _form1099_S_Service;
        private readonly IForm1099_LTC_Service _form1099_LTC_Service;
        private readonly IForm1099_PATR_Service _form1099_PATR_Service;
        private readonly IForm1099_R_Service _form1099_R_Service;
        private readonly IForm1099_SA_Service _form1099_SA_Service;
        private readonly IForm1099_SB_Service _form1099_SB_Service;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CommonController(IWebHostEnvironment webHostEnvironment, ICommonService commonService,
            IForm1099_MISC_Service form1099_MISC_Service, IForm1099_NEC_Service form1099_NEC_Service,
            IForm1099_INT_Service form1099_INT_Service, IForm1099_A_Service form1099_A_Service, IForm1099_B_Service form1099_B_Service,
            IForm1099_DIV_Service form1099_DIV_Service, IForm1099_LS_Service form1099_LS_Service, IForm1099_K_Service form1099_K_Service, 
            IForm1099_OID_Service form1099_OID_Service, IForm1099_Q_Service form1099_Q_Service, IForm1099_S_Service form1099_S_Service,
            IForm1099_C_Service form1099_C_Service, IForm1099_CAP_Service form1099_CAP_Service, IForm1099_G_Service form1099_G_Service, 
            IForm1099_LTC_Service form1099_LTC_Service, IForm1099_PATR_Service form1099_PATR_Service, IForm1099_R_Service form1099_R_Service, 
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
            _form1099_K_Service = form1099_K_Service;
            _form1099_OID_Service = form1099_OID_Service;
            _form1099_Q_Service = form1099_Q_Service;
            _form1099_S_Service = form1099_S_Service;
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
                case AppConstants.Form1099K:
                    fileName = AppConstants.Form1099KExcelTemplate;
                    break;
                case AppConstants.Form1099OID:
                    fileName = AppConstants.Form1099OIDExcelTemplate;
                    break;
                case AppConstants.Form1099Q:
                    fileName = AppConstants.Form1099QExcelTemplate;
                    break;
                case AppConstants.Form1099S:
                    fileName = AppConstants.Form1099SExcelTemplate;
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
            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var scheme = HttpContext.Request.Scheme; // "http" or "https"
            var host = string.Empty;
            if (_webHostEnvironment.IsDevelopment())
            {
                //host = HttpContext.Request.Host.Value;
                host = "localhost:7163";
            }
            else
            {
                host = URLConstants.ClientUrl; // Hostname (e.g., example.com)
            }
            //host = HttpContext.Request.Host.Value; // Comment this line if the project is other than institute
            var fullUrl = $"{scheme}://{host}";
            string URL = string.Concat(fullUrl, "/AuthRecVerify/Account", "/OTP");
            switch (form)
            {
                case AppConstants.Form1099MISC:
                    await _form1099_MISC_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099MISC, instId);
                    break;
                case AppConstants.Form1099NEC:
                    await _form1099_NEC_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099NEC, instId);
                    break;
                case AppConstants.Form1099INT:
                    await _form1099_INT_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099INT, instId);
                    break;
                case AppConstants.Form1099B:
                    await _form1099_B_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099B, instId);
                    break;
                case AppConstants.Form1099A:
                    await _form1099_A_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099A, instId);
                    break;
                case AppConstants.Form1099DIV:
                    await _form1099_DIV_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099DIV, instId);
                    break;
                case AppConstants.Form1099LS:
                    await _form1099_LS_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099LS, instId);
                    break;
                case AppConstants.Form1099C:
                    await _form1099_C_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099C, instId);
                    break;
                case AppConstants.Form1099CAP:
                    await _form1099_CAP_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099CAP, instId);
                    break;
                case AppConstants.Form1099G:
                    await _form1099_G_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099G, instId);
                    break;
                case AppConstants.Form1099LTC:
                    await _form1099_LTC_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099LTC, instId);
                    break;
                case AppConstants.Form1099PATR:
                    await _form1099_PATR_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099PATR, instId);
                    break;
                case AppConstants.Form1099R:
                    await _form1099_R_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099R, instId);
                    break;
                case AppConstants.Form1099SA:
                    await _form1099_SA_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099SA, instId);
                    break;
                case AppConstants.Form1099SB:
                    await _form1099_SB_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099SB,instId);
                    break;
                case AppConstants.Form1099K:
                    await _form1099_K_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099K, instId);
                    break;
                case AppConstants.Form1099OID:
                    await _form1099_OID_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099OID, instId);
                    break;
                case AppConstants.Form1099Q:
                    await _form1099_Q_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099Q, instId);
                    break;
                case AppConstants.Form1099S:
                    await _form1099_S_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1099S, instId);
                    break;
            }

            return Json(new { type = ResponseMessageConstants.SuccessStatus, message = ResponseMessageConstants.SuccessEmailSend });
        }
    }
}
