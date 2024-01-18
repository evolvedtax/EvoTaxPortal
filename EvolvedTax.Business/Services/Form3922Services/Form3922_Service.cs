using AutoMapper;
using EvolvedTax.Business.MailService;
using EvolvedTax.Business.Services.Form1099Services;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs;
using EvolvedTax.Data.Models.DTOs.Response.Form1042;
using EvolvedTax.Data.Models.DTOs.Response.Form3922Response;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Data.Models.Entities._1042;
using EvolvedTax.Data.Models.Entities._1099;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MathNet.Numerics.Distributions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Org.BouncyCastle.Cms;
using System.IO.Compression;

namespace EvolvedTax.Business.Services.Form3922Services
{
    public class Form3922_Service : IForm3922_Service
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly IInstituteService _instituteService;
        readonly IMailService _mailService;
        readonly ITrailAudit1099Service _trailAudit1099Service;
        readonly IMapper _mapper;
        public Form3922_Service(EvolvedtaxContext evolvedtaxContext, IMapper mapper, IInstituteService instituteService, IMailService mailService = null, ITrailAudit1099Service trailAudit1099Service = null)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _mapper = mapper;
            _instituteService = instituteService;
            _mailService = mailService;
            _trailAudit1099Service = trailAudit1099Service;
        }

        public IQueryable<Form3922Response> GetFormList()
        {
            var response = _evolvedtaxContext.Tbl_3922.Select(p => new Form3922Response
            {
                Id = p.Id,
                RcpTIN = p.RcpTIN,
                LastNameCompany = p.LastNameCompany,
                FirstName = p.FirstName,
                NameLine2 = p.NameLine2,
                AddressType = p.AddressType,
                AddressDelivStreet = p.AddressDelivStreet,
                AddressAptSuite = p.AddressAptSuite,
                City = p.City,
                State = p.State,
                Zip = p.Zip,
                Country = p.Country,
                RcpAccount = p.RcpAccount,
                RcpEmail = p.RcpEmail,
                Box1Date = p.Box1Date,
                Box2Date = p.Box2Date,
                Box3Amount = p.Box3Amount,
                Box4Amount = p.Box4Amount,
                Box5Amount = p.Box5Amount,
                Box6Number = p.Box6Number,
                Box7Date = p.Box7Date,
                Box8Amount = p.Box8Amount,
                FormCategory = p.FormCategory,
                FormSource = p.FormSource,
                TaxState = p.TaxState,
                Uploaded_File = p.Uploaded_File,
                Status = p.Status,
                Created_By = p.Created_By,
                Created_Date = p.Created_Date,
                UserId = p.UserId,
                InstID = p.InstID,
                EntityId = p.EntityId,
                IsDuplicated = p.IsDuplicated,
                Province = p.Province,
                PostalCode = p.PostalCode
            });
            return response;
        }
        public async Task<bool> SendEmailToRecipients(int[] selectValues, string URL, string form, int instituteId = -1)
        {
            var result = from ic in _evolvedtaxContext.Tbl_3922
                         where selectValues.Contains(ic.Id) && !string.IsNullOrEmpty(ic.RcpEmail)
                         select new Tbl_3922
                         {
                             RcpEmail = ic.RcpEmail,
                             EntityId = ic.EntityId,
                         };
            foreach (var item in result.ToList())
            {
                await _mailService.SendElectronicAcceptanceEmail(item.RcpEmail, (int)item.EntityId, string.Empty, "Action Required", URL, form, instituteId);

                await _trailAudit1099Service.AddUpdateRecipientAuditDetails(new AuditTrail1099 { RecipientEmail = item.RcpEmail, FormName = form, Token = item.EntityId.ToString() ?? string.Empty });
            }
            return true;
        }
        public async Task<MessageResponseModel> Upload_Data(IFormFile file, int InstId, int entityId, string UserId)
        {
            bool Status = false;
            var response = new List<Tbl_3922>();
            var AList = new List<Tbl_3922>();
            using (var stream = file.OpenReadStream())
            {
                IWorkbook workbook = new XSSFWorkbook(stream);
                ISheet sheet = workbook.GetSheetAt(0); // Assuming the data is in the first sheet

                HashSet<string> uniqueEINNumber = new HashSet<string>();
                HashSet<string> uniqueEntityNames = new HashSet<string>();
                // Define a dictionary to map column names to their indexes
                Dictionary<string, int> columnMapping = new Dictionary<string, int>();

                // Assuming you have access to the header row (excelHeaderRow)
                IRow excelHeaderRow = sheet.GetRow(0);

                // Loop through the header row to populate the columnMapping dictionary
                for (int columnIndex = 0; columnIndex < excelHeaderRow.LastCellNum; columnIndex++)
                {
                    ICell cell = excelHeaderRow.GetCell(columnIndex);
                    if (cell != null)
                    {
                        // Assuming the header text is stored as a string
                        string columnHeader = cell.ToString() ?? string.Empty;
                        columnMapping[columnHeader] = columnIndex;
                    }
                }
                for (int row = 1; row <= sheet.LastRowNum; row++) // Starting from the second row
                {
                    IRow excelRow = sheet.GetRow(row);

                    var request = new Tbl_3922
                    {
                        //ProRataBasisChkbx = (excelRow.GetCell(columnMapping["Ammended"])?.ToString() != null && (bool)excelRow.GetCell(columnMapping["Ammended"])?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        //Ammended_No = excelRow.GetCell(columnMapping["Ammended #"])?.ToString() ?? string.Empty,
                        //UniqueFormID = Convert.ToInt32(excelRow.GetCell(columnMapping["UNIQUE FORM IDENTIFIER"])?.ToString() ?? "0"),
                        RcpTIN = excelRow.GetCell(columnMapping["RcpTIN"])?.ToString() ?? string.Empty,
                        LastNameCompany = excelRow.GetCell(columnMapping["LastNameCompany"])?.ToString() ?? string.Empty,
                        FirstName = excelRow.GetCell(columnMapping["FirstName"])?.ToString() ?? string.Empty,
                        NameLine2 = excelRow.GetCell(columnMapping["NameLine2"])?.ToString() ?? string.Empty,
                        AddressType = excelRow.GetCell(columnMapping["AddressType"])?.ToString() ?? string.Empty,
                        AddressDelivStreet = excelRow.GetCell(columnMapping["AddressDelivStreet"])?.ToString() ?? string.Empty,
                        AddressAptSuite = excelRow.GetCell(columnMapping["AddressAptSuite"])?.ToString() ?? string.Empty,
                        City = excelRow.GetCell(columnMapping["City"])?.ToString() ?? string.Empty,
                        State = excelRow.GetCell(columnMapping["State"])?.ToString() ?? string.Empty,
                        Zip = excelRow.GetCell(columnMapping["Zip"])?.ToString() ?? string.Empty,
                        Country = excelRow.GetCell(columnMapping["Country"])?.ToString() ?? string.Empty,
                        RcpAccount = excelRow.GetCell(columnMapping["RcpAccount"])?.ToString() ?? string.Empty,
                        RcpEmail = excelRow.GetCell(columnMapping["RcpEmail"])?.ToString() ?? string.Empty,
                        Box1Date = excelRow.GetCell(columnMapping["Box1Date"])?.ToString() == null ? null : Convert.ToDateTime(excelRow.GetCell(columnMapping["Box1Date"]).ToString()),
                        Box2Date = excelRow.GetCell(columnMapping["Box2Date"])?.ToString() == null ? null : Convert.ToDateTime(excelRow.GetCell(columnMapping["Box2Date"]).ToString()),
                        Box3Amount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Box3Amount"])?.ToString() ?? "0.0"),
                        Box4Amount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Box4Amount"])?.ToString() ?? "0.0"),
                        Box5Amount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Box5Amount"])?.ToString() ?? "0.0"),
                        Box6Number = Convert.ToInt32(excelRow.GetCell(columnMapping["Box6Number"])?.ToString() ?? "0"),
                        Box7Date = excelRow.GetCell(columnMapping["Box7Date"])?.ToString() == null ? null : Convert.ToDateTime(excelRow.GetCell(columnMapping["Box7Date"]).ToString()),
                        Box8Amount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Box8Amount"])?.ToString() ?? "0.0"),
                        FormSource = excelRow.GetCell(columnMapping["FormCategory"])?.ToString() ?? string.Empty,
                        FormCategory = excelRow.GetCell(columnMapping["FormSource"])?.ToString() ?? string.Empty,
                        TaxState = excelRow.GetCell(columnMapping["TaxState"])?.ToString() ?? string.Empty,


                        
                        Created_Date = DateTime.Now,
                        Created_By = UserId,
                        InstID = InstId,
                        EntityId = entityId,
                        UserId = UserId,
                    };
                    string clientEmailEINNumber = request.RcpTIN ?? string.Empty;
                    if (uniqueEINNumber.Contains(clientEmailEINNumber))
                    {
                        // This entity is a duplicate within the Excel sheet
                        Status = false;
                        return new MessageResponseModel { Status = Status, Message = new { Title = "Duplication Record In Excel", TagLine = "Record not uploaded due to duplication record in excel" }, Param = "Excel" };
                    }
                    else
                    {
                        // Add the values to the HashSet to track duplicates
                        uniqueEINNumber.Add(clientEmailEINNumber);
                    }
                    // Check for duplicate records based on entityName in the database
                    if (await _evolvedtaxContext.Tbl_3922.AnyAsync(p => p.RcpTIN == request.RcpTIN && p.EntityId == request.EntityId && p.Created_Date != null &&
                     p.Created_Date.Value.Year == DateTime.Now.Year))
                    {
                        response.Add(request);
                        Status = true;
                        request.IsDuplicated = true;
                    }
                    else
                    {

                        request.IsDuplicated = false;
                    }
                    AList.Add(request);

                }
                await _evolvedtaxContext.Tbl_3922.AddRangeAsync(AList);
                await _evolvedtaxContext.SaveChangesAsync();
            }
            return new MessageResponseModel { Status = Status, Message = response, Param = "Database" };
            //return new MessageResponseModel { Status = Status, Message = response, Param = "Entity" };
        }


        public string GeneratePdf(int Id, string TemplatefilePath, string SaveFolderPath, int entityId)
        {
            return CreatePdf(Id, TemplatefilePath, SaveFolderPath, entityId, false);

        }

        public string CreatePdf(int Id, string TemplatefilePath, string SaveFolderPath, int entityId, bool IsAll, string Page = "")
        {
            var response = _evolvedtaxContext.Tbl_3922.FirstOrDefault(p => p.Id == Id);
            //var requestInstitue = _instituteService.GetPayeeData((int)response.InstID);
            var entityData = _instituteService.GetEntityDataById(entityId);
            string templatefile = TemplatefilePath;
            string newFile1 = string.Empty;

            string ClientName = response.FirstName + " " + response.NameLine2?.Replace(": ", "");

            if (!string.IsNullOrEmpty(Page))
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.Form3922, "_", response.Id, "_Page_", Page);
            }
            else
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.Form3922, "_", response.Id);
            }


            string FilenameNew = "/Form3922/" + newFile1 + ".pdf";
            string newFileName = newFile1 + ".pdf"; // Add ".pdf" extension to the file name

            string newFilePath = Path.Combine(SaveFolderPath, newFileName);

            PdfReader pdfReader = new PdfReader(templatefile);
            PdfReader.unethicalreading = true;
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFilePath, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;

            //string Recepient_CountryCode = "";
            //if (response.Country != "United States")
            //{
            //    var country = _evolvedtaxContext.MstrCountries.FirstOrDefault(c => c.Country == response.Country);
            //    if (country != null)
            //    {
            //        Recepient_CountryCode = country.CountryId;
            //    }
            //}

            // string RecipentCity = string.Join(", ",
            //    new[]
            //    {
            //          response.City,
            //          response.State,
            //          string.IsNullOrWhiteSpace(response.Province) ? null : response.Province,
            //           string.IsNullOrWhiteSpace(Recepient_CountryCode) ? null : Recepient_CountryCode,
            //          response.Zip,
            //          string.IsNullOrWhiteSpace(response.PostalCode) ? null : response.PostalCode

            //    }.Where(s => !string.IsNullOrWhiteSpace(s))
            //);

            //String RecipentAddress = string.Concat(response.Address_Deliv_Street, ", ", response.Address_Apt_Suite);
            string currentYear = Convert.ToString(DateTime.Now.Year % 100);
            string EntityInfo = string.Concat(entityData.EntityName, " ", entityData.Address1, " ", entityData.Address2, " ", entityData.City, " ", entityData.State, entityData.Province, " ", entityData.Zip);
            #region PDF Columns
            if (response.IsCorrected== 1)
            {
                pdfFormFields.SetField("topmostSubform.CopyA.CopyAHeader.c1_01", "0"); //void

            }
            if (response.IsCorrected == 0)
            {
                pdfFormFields.SetField("efield1_topmostSubform.CopyA.CopyAHeader.c1_01", "0"); //corrected
            }
            pdfFormFields.SetField("topmostSubform.CopyA.LeftColumn.gf1_001", EntityInfo);
            pdfFormFields.SetField("topmostSubform.CopyA.LeftColumn.gf1_002", entityData.Ein);
            pdfFormFields.SetField("topmostSubform.CopyA.LeftColumn.gf1_003", response.RcpTIN);
            pdfFormFields.SetField("topmostSubform.CopyA.LeftColumn.gf1_004", response.FirstName + response.NameLine2);
            pdfFormFields.SetField("topmostSubform.CopyA.LeftColumn.gf1_005", response.AddressDelivStreet + " " + response.AddressAptSuite);
            pdfFormFields.SetField("topmostSubform.CopyA.LeftColumn.gf1_006", response.City + " " + response.State + " " + response.Zip + " " + response.Country);
            pdfFormFields.SetField("topmostSubform.CopyA.LeftColumn.gf1_007", response.RcpAccount);
            pdfFormFields.SetField("topmostSubform.CopyA.RightColumn.gf1_008", response.Box1Date?.ToString("MM/dd/yyyy"));
            pdfFormFields.SetField("topmostSubform.CopyA.RightColumn.gf1_009", response.Box2Date?.ToString("MM/dd/yyyy"));
            pdfFormFields.SetField("topmostSubform.CopyA.RightColumn.gf1_010", response.Box3Amount?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyA.RightColumn.gf1_011", response.Box4Amount?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyA.RightColumn.gf1_012", response.Box5Amount?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyA.RightColumn.gf1_013", response.Box6Number?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyA.RightColumn.gf1_014", response.Box7Date?.ToString("MM/dd/yyyy"));
            pdfFormFields.SetField("topmostSubform.CopyA.RightColumn.gf1_015", response.Box8Amount?.ToString());


            if (response.IsCorrected == 0)
            {
                pdfFormFields.SetField("topmostSubform.CopyB.CopyBHeader.c1_01", "0");

            }
            //if (response.IsCorrected == 0)
            //{
            //    pdfFormFields.SetField("topmostSubform.CopyB.CopyBHeader.c1_01", "1");
            //}
            //else
            //{
            //    pdfFormFields.SetField("topmostSubform.CopyB.CopyBHeader.c1_01", "0");
            //}
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.gf1_001", EntityInfo);
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.gf1_002", entityData.Ein);
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.gf1_003", response.RcpTIN);
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.gf1_004", response.FirstName + response.NameLine2);
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.gf1_005", response.AddressDelivStreet + " " + response.AddressAptSuite);
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.gf1_006", response.City + " " + response.State + " " + response.Zip + " " + response.Country);
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.gf1_007", response.RcpAccount);
            pdfFormFields.SetField("topmostSubform.CopyB.RightColumn.gf1_008", response.Box1Date?.ToString("MM/dd/yyyy"));
            pdfFormFields.SetField("topmostSubform.CopyB.RightColumn.gf1_009", response.Box2Date?.ToString("MM/dd/yyyy"));
            pdfFormFields.SetField("topmostSubform.CopyB.RightColumn.gf1_010", response.Box3Amount?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyB.RightColumn.gf1_011", response.Box4Amount?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyB.RightColumn.gf1_012", response.Box5Amount?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyB.RightColumn.gf1_013", response.Box6Number?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyB.RightColumn.gf1_014", response.Box7Date?.ToString("MM/dd/yyyy"));
            pdfFormFields.SetField("topmostSubform.CopyB.RightColumn.gf1_015", response.Box8Amount?.ToString());

            if (response.IsCorrected == 1)
            {
                pdfFormFields.SetField("topmostSubform.CopyC.CopyCHeader.c1_01", "0"); //void

            }
            if (response.IsCorrected == 0)
            {
                pdfFormFields.SetField("efield34_topmostSubform.CopyC.CopyCHeader.c1_01", "0"); //corrected
            }
            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.gf1_001", EntityInfo);
            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.gf1_002", entityData.Ein);
            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.gf1_003", response.RcpTIN);
            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.gf1_004", response.FirstName + response.NameLine2);
            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.gf1_005", response.AddressDelivStreet + " " + response.AddressAptSuite);
            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.gf1_006", response.City + " " + response.State + " " + response.Zip + " " + response.Country);
            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.gf1_007", response.RcpAccount);
            pdfFormFields.SetField("topmostSubform.CopyC.RightColumn.gf1_008", response.Box1Date?.ToString("MM/dd/yyyy"));
            pdfFormFields.SetField("topmostSubform.CopyC.RightColumn.gf1_009", response.Box2Date?.ToString("MM/dd/yyyy"));
            pdfFormFields.SetField("topmostSubform.CopyC.RightColumn.gf1_010", response.Box3Amount?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyC.RightColumn.gf1_011", response.Box4Amount?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyC.RightColumn.gf1_012", response.Box5Amount?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyC.RightColumn.gf1_013", response.Box6Number?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyC.RightColumn.gf1_014", response.Box7Date?.ToString("MM/dd/yyyy"));
            pdfFormFields.SetField("topmostSubform.CopyC.RightColumn.gf1_015", response.Box8Amount?.ToString());
         
            #endregion

            pdfStamper.FormFlattening = true;
            pdfStamper.Close();
            pdfReader.Close();


            if (IsAll)
            {
                return newFilePath;
            }
            return FilenameNew;

        }

        public string GeneratePdfForSpecificPage(int Id, string TemplatefilePath, string SaveFolderPath, List<string> selectedPages, int entityId)
        {
            string newFile1 = string.Empty;
            var request = _evolvedtaxContext.Tbl_3922.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.FirstName + " " + request.NameLine2?.Replace(": ", "");
            newFile1 = string.Concat(ClientName, "_", AppConstants.Form3922, "_", Id);
            string FilenameNew = "/Form3922/" + newFile1 + ".pdf";
            string newFileName = newFile1 + ".pdf";



            var newFilePath = CreatePdf(Id, TemplatefilePath, SaveFolderPath, entityId, true);

            // Create a copy of the generated PDF
            string tempFilePath = Path.Combine(SaveFolderPath, newFile1 + "_temp.pdf");
            File.Copy(newFilePath, tempFilePath);

            // Open the copied PDF
            PdfReader pdfReaderTemp = new PdfReader(tempFilePath);
            PdfReader.unethicalreading = true;

            // Create a new PDF document to save the modified pages
            string modifiedFilePath = Path.Combine(SaveFolderPath, newFile1 + "_modified.pdf");
            using (FileStream fs = new FileStream(modifiedFilePath, FileMode.Create))
            using (Document doc = new Document())
            using (PdfCopy copy = new PdfCopy(doc, fs))
            {
                doc.Open();
                List<int> pagesToInclude = selectedPages.Select(int.Parse).ToList();


                for (int page = 1; page <= pdfReaderTemp.NumberOfPages; page++)
                {
                    if (pagesToInclude.Contains(page))
                    {
                        PdfImportedPage importedPage = copy.GetImportedPage(pdfReaderTemp, page);
                        copy.AddPage(importedPage);
                    }
                }


                doc.Close();
            }

            // Close the copied PDF reader
            pdfReaderTemp.Close();

            // Delete the temporary copied PDF
            File.Delete(tempFilePath);

            // Delete the original PDF
            File.Delete(newFilePath);

            // Rename the modified PDF by removing "_modified" from the filename
            string fileNameWithoutModified = newFile1.Replace("_modified", "");
            string finalFilePath = Path.Combine(SaveFolderPath, fileNameWithoutModified + ".pdf");
            File.Move(modifiedFilePath, finalFilePath);

            return finalFilePath;
            //return null;
        }
        public string GeneratePdForSpecificType(int Id, string TemplatefilePath, string SaveFolderPath, string selectedPage, int entityId = 0)
        {
            string newFile1 = string.Empty;
            var request = _evolvedtaxContext.Tbl_3922.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.FirstName + " " + request.NameLine2?.Replace(": ", "");

            newFile1 = string.Concat(ClientName, "_", AppConstants.Form3922, "_", request.Id, "_Page_", selectedPage);
            string FilenameNew = "/Form3922/" + newFile1 + ".pdf";
            string newFileName = newFile1 + ".pdf";

            var newFilePath = CreatePdf(Id, TemplatefilePath, SaveFolderPath, entityId, true, selectedPage);

            // Create a copy of the generated PDF
            string tempFilePath = Path.Combine(SaveFolderPath, newFile1 + "_temp.pdf");
            File.Copy(newFilePath, tempFilePath);

            // Open the copied PDF
            PdfReader pdfReaderTemp = new PdfReader(tempFilePath);
            PdfReader.unethicalreading = true;

            // Create a new PDF document to save the modified pages
            string modifiedFilePath = Path.Combine(SaveFolderPath, newFile1 + "_modified.pdf");
            using (FileStream fs = new FileStream(modifiedFilePath, FileMode.Create))
            using (Document doc = new Document())
            using (PdfCopy copy = new PdfCopy(doc, fs))
            {
                doc.Open();

                PdfImportedPage importedPage = copy.GetImportedPage(pdfReaderTemp, Convert.ToInt32(selectedPage));
                copy.AddPage(importedPage);
                doc.Close();
            }

            // Close the copied PDF reader
            pdfReaderTemp.Close();

            // Delete the temporary copied PDF
            File.Delete(tempFilePath);

            // Delete the original PDF
            File.Delete(newFilePath);

            // Rename the modified PDF by removing "_modified" from the filename
            string fileNameWithoutModified = newFile1.Replace("_modified", "");
            string finalFilePath = Path.Combine(SaveFolderPath, fileNameWithoutModified + ".pdf");
            File.Move(modifiedFilePath, finalFilePath);

            return finalFilePath;
            //return null;
        }
        public string DownloadOneFile(List<int> ids, string SaveFolderPath, List<string> selectedPages, string RootPath, int entityId = 0)
        {
            var pdfPaths = new List<string>();
            var CompilepdfPaths = new List<string>();
            string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.Form3922TemplateFileName);
            bool containsAll = selectedPages.Contains("All");

            if (containsAll)
            {
                foreach (var id in ids)
                {
                    var pdfPath = CreatePdf(id, TemplatePathFile, SaveFolderPath, entityId, true);
                    pdfPaths.Add(pdfPath);
                }

                #region CompilePDFs

                string compileFileName = "All Form Single File.pdf";
                string outputFilePath = Path.Combine(SaveFolderPath, compileFileName);
                CompilepdfPaths.Add(outputFilePath);

                // Create a Document object
                Document document = new Document();

                // Create a PdfCopy object to write the output PDF
                PdfCopy pdfCopy = new PdfCopy(document, new FileStream(outputFilePath, FileMode.Create));

                // Open the document for writing
                document.Open();

                foreach (string pdfFilePath in pdfPaths)
                {
                    // Open each input PDF file
                    PdfReader pdfReader = new PdfReader(pdfFilePath);

                    // Iterate through the pages of the input PDF and add them to the output PDF
                    for (int pageNum = 1; pageNum <= pdfReader.NumberOfPages; pageNum++)
                    {
                        PdfImportedPage page = pdfCopy.GetImportedPage(pdfReader, pageNum);
                        pdfCopy.AddPage(page);
                    }

                    pdfReader.Close();
                }


                document.Close();
                pdfCopy.Close();
                pdfPaths.Clear();

                #endregion

            }
            else
            {
                foreach (var selectedPage in selectedPages)
                {


                    foreach (var id in ids)
                    {
                        var pdfPath = GeneratePdForSpecificType(id, TemplatePathFile, SaveFolderPath, selectedPage, entityId);
                        pdfPaths.Add(pdfPath);
                    }

                    #region CompilePDFs
                    string compileFileName;

                    switch (selectedPage)
                    {
                        case "1":
                            compileFileName = "Internal Revenue Service Center.pdf";
                            break;
                        case "2":
                            compileFileName = "For Employee.pdf";
                            break;
                        case "4":
                            compileFileName = "For Corporation.pdf";
                            break;
                        default:
                            compileFileName = "compiled_page.pdf";
                            break;
                    }

                    string outputFilePath = Path.Combine(SaveFolderPath, compileFileName);
                    CompilepdfPaths.Add(outputFilePath);

                    // Create a Document object
                    Document document = new Document();

                    // Create a PdfCopy object to write the output PDF
                    PdfCopy pdfCopy = new PdfCopy(document, new FileStream(outputFilePath, FileMode.Create));

                    // Open the document for writing
                    document.Open();

                    foreach (string pdfFilePath in pdfPaths)
                    {
                        // Open each input PDF file
                        PdfReader pdfReader = new PdfReader(pdfFilePath);

                        // Iterate through the pages of the input PDF and add them to the output PDF
                        for (int pageNum = 1; pageNum <= pdfReader.NumberOfPages; pageNum++)
                        {
                            PdfImportedPage page = pdfCopy.GetImportedPage(pdfReader, pageNum);
                            pdfCopy.AddPage(page);
                        }

                        pdfReader.Close();
                    }


                    document.Close();
                    pdfCopy.Close();
                    pdfPaths.Clear();

                    #endregion


                }
            }

            //Create Zip
            var zipFileName = $"GeneratedPDFs_{DateTime.Now:yyyyMMddHHmmss}.zip";
            var zipFilePath = Path.Combine(SaveFolderPath, zipFileName);

            using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                foreach (var pdfPath in CompilepdfPaths)
                {
                    var pdfFileName = Path.GetFileName(pdfPath);
                    zipArchive.CreateEntryFromFile(pdfPath, pdfFileName);
                }
            }

            return zipFilePath; // Return the ZIP file path.
            //return null;
        }
        public string GenerateAndZipPdfs(List<int> ids, string SaveFolderPath, List<string> selectedPages, string RootPath, int entityId)
        {
            var pdfPaths = new List<string>();

            foreach (var id in ids)
            {

                string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.Form3922TemplateFileName);
                bool containsAll = selectedPages.Contains("All");

                if (containsAll)
                {
                    var pdfPath = CreatePdf(id, TemplatePathFile, SaveFolderPath, entityId, true);
                    pdfPaths.Add(pdfPath);
                }
                else
                {
                    var pdfPath = GeneratePdfForSpecificPage(id, TemplatePathFile, SaveFolderPath, selectedPages, entityId);
                    pdfPaths.Add(pdfPath);

                }


            }

            var zipFileName = $"GeneratedPDFs_{DateTime.Now:yyyyMMddHHmmss}.zip";
            var zipFilePath = Path.Combine(SaveFolderPath, zipFileName);

            using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                foreach (var pdfPath in pdfPaths)
                {
                    var pdfFileName = Path.GetFileName(pdfPath);
                    zipArchive.CreateEntryFromFile(pdfPath, pdfFileName);
                }
            }

            return zipFilePath; // Return the ZIP file path.
            //return null;
        }

        public async Task<MessageResponseModel> DeletePermeant(int id)
        {

            var recordToDelete = _evolvedtaxContext.Tbl_3922.First(p => p.Id == id);
            if (recordToDelete != null)
            {
                _evolvedtaxContext.Tbl_3922.Remove(recordToDelete);
                await _evolvedtaxContext.SaveChangesAsync();
                return new MessageResponseModel { Status = true };
            }

            return new MessageResponseModel { Status = false };
        }

        public async Task<MessageResponseModel> KeepRecord(int id)
        {

            var recordToUpdate = _evolvedtaxContext.Tbl_3922.First(p => p.Id == id);
            if (recordToUpdate != null)
            {
                recordToUpdate.IsDuplicated = false;
                await _evolvedtaxContext.SaveChangesAsync();
                return new MessageResponseModel { Status = true, Message = "The record has been kept" };
            }

            return new MessageResponseModel { Status = false, Message = "Oops! something wrong" };
        }
    }
}
