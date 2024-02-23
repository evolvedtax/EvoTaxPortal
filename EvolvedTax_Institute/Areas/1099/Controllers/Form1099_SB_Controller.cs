using EvolvedTax.Business.Services.AnnouncementService;
using EvolvedTax.Business.Services.Form1099Services;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.DTOs.Response.Form1099;
using EvolvedTax.Helpers;
using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Xml.Linq;

namespace EvolvedTax_Institute.Areas._1099.Controllers
{
    [Area("1099")]
    public class Form1099_SB_Controller : BaseController
    {
        private readonly IForm1099_SB_Service _Form1099_SB_Service;
        private readonly IInstituteService _instituteService;
        private readonly IWebHostEnvironment _webHostEnvironment;
   
        public Form1099_SB_Controller(IForm1099_SB_Service Form1099_SB_Service, IWebHostEnvironment webHostEnvironment, IInstituteService instituteService)
        {
            _Form1099_SB_Service = Form1099_SB_Service;
            _webHostEnvironment = webHostEnvironment;
            _instituteService = instituteService;
        }
        public IActionResult Index()
        {
            var EntityId = HttpContext.Session.GetInt32("EntityId") ?? 0;
            var InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            ViewBag.EntitiesList = _instituteService.GetEntitiesByInstId(SessionUser.InstituteId, Convert.ToInt32(AppConstants.FormSubscription_1099)).Select(p => new SelectListItem
            {
                Text = p.EntityName,
                Value = p.EntityId.ToString(),
                Selected = p.EntityId == EntityId
            });
            //testing
            XDocument xmlDocument = Generate1099Xml();
            return View(_Form1099_SB_Service.GetForm1099List().Where(p => p.EntityId == EntityId && p.InstID == InstId));
        }
        [Route("Form1099_SB_/uploadClients")]
        [HttpPost]
        public async Task<IActionResult> UploadClients(IFormFile file, int EntityId, string entityName)
        {
            if (file == null)
            {
                return Json(false);
            }
            var response = await _Form1099_SB_Service.Upload1099_Data(file, EntityId, SessionUser.InstituteId, SessionUser.UserId);
            return Json(response);
        }
        public IActionResult ChangeEntity(int entityId)
        {
            HttpContext.Session.SetInt32("EntityId", entityId);
            return Json(new { Data = "true" }); ;
        }

        public IActionResult DownloadCsv([FromBody] List<string> selectedRows)
        //public IActionResult DownloadCsv()
        {

            var EntityId = HttpContext.Session.GetInt32("EntityId") ?? 0;
            var InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            // Get data from the service
            var data = _Form1099_SB_Service.GetCSVForm1099List(EntityId, InstId, selectedRows).ToList();

            // Generate CSV content in the service
            string csvContent = _Form1099_SB_Service.GenerateCsvContent(data);
           

            // Create a CSV file name
            var fileName = $"Form1099_{DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture)}.csv";

            // Return the CSV file for download
            return File(Encoding.UTF8.GetBytes(csvContent), "text/csv", fileName);

          
        }

        #region PDF Creation Methods
        public IActionResult downlodPdf(int id)
        {
            string TemplatePathFile = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.SB_1099_TemplateFileName);
            string SavePathFolder = Path.Combine(_webHostEnvironment.WebRootPath, "1099SB");
            string pdfUrl = _Form1099_SB_Service.GeneratePdf(id, TemplatePathFile, SavePathFolder);
            return Json(pdfUrl);

        }


        [HttpPost]
        [Route("Form1099_SB_/DownloadAll")]
        public IActionResult DownloadAll([FromBody] DownloadRequestModel model)
        {

            List<int> ids = model.ids;
            List<string> selectedPage = model.selectedPage;
            string RootPath = _webHostEnvironment.WebRootPath;
            string SavePathFolder = Path.Combine(_webHostEnvironment.WebRootPath, "1099SB");
            var zipFilePath = _Form1099_SB_Service.GenerateAndZipPdfs(ids, SavePathFolder, selectedPage, RootPath);
            string contentType = "application/zip";

            var fileBytes = System.IO.File.ReadAllBytes(zipFilePath);
            return File(fileBytes, contentType, "GeneratedPDFs.zip");


        }


        [HttpPost]
        [Route("Form1099_SB_/DownloadOneFile")]
        public IActionResult DownloadOneFile([FromBody] DownloadRequestModel model)
        {

            List<int> ids = model.ids;
            List<string> selectedPage = model.selectedPage;
            string RootPath = _webHostEnvironment.WebRootPath;
            string SavePathFolder = Path.Combine(_webHostEnvironment.WebRootPath, "1099SB");
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
            var zipFilePath = _Form1099_SB_Service.DownloadOneFile(ids, SavePathFolder, selectedPage, RootPath);
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
            var response = await _Form1099_SB_Service.KeepRecord(id);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRecord(int id)
        {
            if (id == 0)
            {
                return Json(false);
            }
            var response = await _Form1099_SB_Service.DeletePermeant(id);
            return Json(response);
        }


        public XDocument Generate1099Xml()
        {
            var EntityId = HttpContext.Session.GetInt32("EntityId") ?? 0;
            var InstId = HttpContext.Session.GetInt32("InstId") ?? 0;

            var form1099Data = _Form1099_SB_Service.GetForm1099List().Where(p => p.EntityId == EntityId && p.InstID == InstId);

            // Create IRS1099File element
            XElement irs1099File = new XElement("IRS1099File",
                new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance")
            );

            // Add FileHeader element
            XElement fileHeader = new XElement("FileHeader",
                new XElement("RecordType", "F"),
                new XElement("SubmissionType", "Original"),
                new XElement("Timestamp", DateTime.Now.ToString("yyyyMMddHHmmss"))
            );
            irs1099File.Add(fileHeader);

            // Add Form1099 elements
            XElement form1099s = new XElement("Form1099");
            foreach (var data in form1099Data)
            {
                // Create Recipient element
                XElement recipient = new XElement("Recipient");

                // Dynamically add elements for each column in the form1099Data
                foreach (var property in data.GetType().GetProperties())
                {
                    string columnName = property.Name;
                    object columnValue = property.GetValue(data);

                    // Add XML element for the column
                    recipient.Add(new XElement(columnName, columnValue));
                }

                // Add Recipient element to Form1099
                form1099s.Add(recipient);
            }
            irs1099File.Add(form1099s);

            // Add FileFooter element
            XElement fileFooter = new XElement("FileFooter",
                new XElement("RecordType", "F"),
                new XElement("TotalRecords", form1099Data.Count())
            );
            irs1099File.Add(fileFooter);

            // Create XDocument with XML declaration
            XDocument xmlDocument = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"), // Add XML declaration here
                irs1099File
            );

            return xmlDocument;




            /*
            var form1099Data = _Form1099_SB_Service.GetForm1099List().Where(p => p.EntityId == EntityId && p.InstID == InstId);

            XDocument xmlDocument = new XDocument(
                new XElement("IRS1099File",
                    new XElement("FileHeader",
                        new XElement("RecordType", "F"),
                        new XElement("SubmissionType", "Original"),
                        // Add other header information here
                        new XElement("Timestamp", DateTime.Now.ToString("yyyyMMddHHmmss"))
                    ),
                    new XElement("Form1099",
                        form1099Data.Select(data => new XElement("Recipient",
                            new XElement("Name", string.Concat(data.First_Name," ",data.Last_Name_Company)),
                            // Add other recipient information here
                            new XElement("Income",
                                new XElement("Type", data.Address_Type),
                                new XElement("Amount", data.Box_1_Amount?.ToString("F2"))
                            // Add other income information here
                            ),
                            // Add other form elements for each recipient here
                            new XElement("Country", data.Country),
                            new XElement("AddressLine1", data.Address_Deliv_Street),
                            // ... add more elements as needed
                            new XElement("AdditionalIncome",
                                new XElement("Type", data.Rcp_Account),
                                new XElement("Amount", data.Box_2_Amount?.ToString("F2"))
                            // Add other additional income information here
                            )
                        ))
                    ),
                    new XElement("FileFooter",
                        new XElement("RecordType", "F"),
                        new XElement("TotalRecords", form1099Data.Count())
                    // Add other footer information here
                    )
                )
            );

            return xmlDocument;

            */
        }

    }
}
