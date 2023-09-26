using AutoMapper;
using EvolvedTax.Business.MailService;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs;
using EvolvedTax.Data.Models.DTOs.Response.Form1099;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Data.Models.Entities._1099;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO.Compression;

namespace EvolvedTax.Business.Services.Form1099Services
{
    public class Form1099_A_Service : IForm1099_A_Service
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly IInstituteService _instituteService;
        readonly IMailService _mailService;
        readonly ITrailAudit1099Service _trailAudit1099Service;
        readonly IMapper _mapper;
        public Form1099_A_Service(EvolvedtaxContext evolvedtaxContext, IMapper mapper, IInstituteService instituteService, IMailService mailService = null, ITrailAudit1099Service trailAudit1099Service = null)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _mapper = mapper;
            _instituteService = instituteService;
            _mailService = mailService;
            _trailAudit1099Service = trailAudit1099Service;
        }

        public IQueryable<Form1099AResponse> GetForm1099AList()
        {
            var response = _evolvedtaxContext.Tbl1099_A.Select(p => new Form1099AResponse
            {
                Id = p.Id,
                Corrected = p.Corrected,
                EntityId = p.EntityId,
                Address_Apt_Suite = p.Address_Apt_Suite,
                Address_Deliv_Street = p.Address_Deliv_Street,
                Address_Type = p.Address_Type,
                Box_1_Date = p.Box_1_Date,
                Box_2_Amount = p.Box_2_Amount,
                Box_4_Amount = p.Box_4_Amount,
                Box_5_Checkbox = p.Box_5_Checkbox,
                Box_6_All_Lines = p.Box_6_All_Lines,
                City = p.City,
                Country = p.Country,
                Created_By = p.Created_By,
                Created_Date = p.Created_Date,
                First_Name = p.First_Name,
                Form_Category = p.Form_Category,
                Form_Source = p.Form_Source,
                InstID = p.InstID,
                Last_Name_Company = p.Last_Name_Company,
                Name_Line_2 = p.Name_Line_2,
                Rcp_Account = p.Rcp_Account,
                Rcp_Email = p.Rcp_Email != null ? p.Rcp_Email.Trim() : string.Empty,
                Rcp_TIN = p.Rcp_TIN,
                State = p.State,
                Status = p.Status,
                Tax_State = p.Tax_State,
                Uploaded_File = p.Uploaded_File,
                UserId = p.UserId,
                Zip = p.Zip,
                IsDuplicated = p.IsDuplicated
            });
            return response;
        }
        public async Task<bool> SendEmailToRecipients(int[] selectValues, string URL, string form)
        {
            var result = from ic in _evolvedtaxContext.Tbl1099_A
                         where selectValues.Contains(ic.Id) && !string.IsNullOrEmpty(ic.Rcp_Email)
                         select new Tbl1099_A
                         {
                             Rcp_Email = ic.Rcp_Email,
                             EntityId = ic.EntityId,
                         };
            foreach (var item in result.ToList())
            {
                await _mailService.SendElectronicAcceptanceEmail(item.Rcp_Email, (int)item.EntityId, string.Empty, "Action Required", URL, form);

                await _trailAudit1099Service.AddUpdateRecipientAuditDetails(new AuditTrail1099 { RecipientEmail = item.Rcp_Email, FormName = form, Token = item.EntityId.ToString() ?? string.Empty });
            }
            return true;
        }
        public async Task<MessageResponseModel> Upload1099_A_Data(IFormFile file, int InstId, int entityId, string UserId)
        {
            bool Status = false;
            var response = new List<Tbl1099_A>();
            var AList = new List<Tbl1099_A>();
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

                    var request = new Tbl1099_A
                    {
                        Rcp_TIN = excelRow.GetCell(columnMapping["Rcp TIN"])?.ToString() ?? string.Empty,
                        Last_Name_Company = excelRow.GetCell(columnMapping["Company"])?.ToString() ?? string.Empty,
                        First_Name = excelRow.GetCell(columnMapping["First Name"])?.ToString() ?? string.Empty,
                        Name_Line_2 = excelRow.GetCell(columnMapping["Last Name"])?.ToString() ?? string.Empty,
                        Address_Type = excelRow.GetCell(columnMapping["Address Type"])?.ToString() ?? string.Empty,
                        Address_Deliv_Street = excelRow.GetCell(columnMapping["Address Line 1"])?.ToString() ?? string.Empty,
                        Address_Apt_Suite = excelRow.GetCell(columnMapping["Address Line 2"])?.ToString() ?? string.Empty,
                        City = excelRow.GetCell(columnMapping["City"])?.ToString() ?? string.Empty,
                        State = excelRow.GetCell(columnMapping["State"])?.ToString() ?? string.Empty,
                        Zip = excelRow.GetCell(columnMapping["Zip"])?.ToString() ?? string.Empty,
                        PostalCode = excelRow.GetCell(columnMapping["Postal Code"])?.ToString() ?? string.Empty,
                        Province = excelRow.GetCell(columnMapping["Province"])?.ToString() ?? string.Empty,
                        Country = excelRow.GetCell(columnMapping["Country"])?.ToString() ?? string.Empty,
                        Rcp_Account = excelRow.GetCell(columnMapping["Rcp Account"])?.ToString() ?? string.Empty,
                        Rcp_Email = excelRow.GetCell(columnMapping["Rcp Email"])?.ToString() ?? string.Empty,
                        Box_1_Date = excelRow.GetCell(columnMapping["Box 1 Date"])?.ToString() == null ? null : Convert.ToDateTime(excelRow.GetCell(columnMapping["Box 1 Date"]).ToString()),
                        Box_2_Amount = excelRow.GetCell(columnMapping["Box 2 Amount"])?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(columnMapping["Box 2 Amount"])?.ToString()) : 0,
                        Box_4_Amount = excelRow.GetCell(columnMapping["Box 4 Amount"])?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(columnMapping["Box 4 Amount"])?.ToString()) : 0,
                        Box_5_Checkbox = (excelRow.GetCell(columnMapping["Box 5 Checkbox"])?.ToString() != null && (bool)excelRow.GetCell(columnMapping["Box 5 Checkbox"])?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        Box_6_All_Lines = excelRow.GetCell(columnMapping["Box 6 All Lines"])?.ToString() ?? string.Empty,
                        Form_Category = excelRow.GetCell(columnMapping["Form Category"])?.ToString() ?? string.Empty,
                        Form_Source = excelRow.GetCell(columnMapping["Form Source"])?.ToString() ?? string.Empty,
                        Tax_State = excelRow.GetCell(columnMapping["Tax State"])?.ToString() ?? string.Empty,
                        Corrected = (excelRow.GetCell(columnMapping["Is Corrected"])?.ToString() != null && (bool)excelRow.GetCell(columnMapping["Is Corrected"])?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        Created_Date = DateTime.Now,
                        Created_By = UserId,
                        InstID = InstId,
                        EntityId = entityId,
                        UserId = UserId,
                    };
                    string clientEmailEINNumber = request.Rcp_TIN ?? string.Empty;
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
                    if (await _evolvedtaxContext.Tbl1099_A.AnyAsync(p => p.Rcp_TIN == request.Rcp_TIN && p.EntityId == request.EntityId && p.Created_Date != null &&
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
                await _evolvedtaxContext.Tbl1099_A.AddRangeAsync(AList);
                await _evolvedtaxContext.SaveChangesAsync();
            }
            return new MessageResponseModel { Status = Status, Message = response, Param = "Database" };
            //return new MessageResponseModel { Status = Status, Message = response, Param = "Entity" };
        }


        public string GeneratePdf(int Id, string TemplatefilePath, string SaveFolderPath)
        {
            return CreatePdf(Id, TemplatefilePath, SaveFolderPath, false);

        }

        public string CreatePdf(int Id, string TemplatefilePath, string SaveFolderPath, bool IsAll, string Page = "")
        {
            var Aresponse = _evolvedtaxContext.Tbl1099_A.FirstOrDefault(p => p.Id == Id);
            var instResponse = _instituteService.GetInstituteDataById((int)Aresponse.InstID);
            string templatefile = TemplatefilePath;
            string newFile1 = string.Empty;

            String ClientName = Aresponse.First_Name + " " + Aresponse.Name_Line_2?.Replace(": ", "");

            if (!String.IsNullOrEmpty(Page))
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099A, "_", Aresponse.Id, "_Page_", Page);
            }
            else
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099A, "_", Aresponse.Id);
            }


            string FilenameNew = "/Form1099A/" + newFile1 + ".pdf";
            string newFileName = newFile1 + ".pdf"; // Add ".pdf" extension to the file name

            string newFilePath = Path.Combine(SaveFolderPath, newFileName);

            PdfReader pdfReader = new PdfReader(templatefile);
            PdfReader.unethicalreading = true;
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFilePath, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;
            string currentYear = Convert.ToString(DateTime.Now.Year % 100);

            #region PDF Columns
            if (Aresponse.Corrected == "1")
            {
                pdfFormFields.SetField("AVoid", "0");   //Void
            }
            else
            {
                pdfFormFields.SetField("ACorrected", "0");   //Corrected
            }
            pdfFormFields.SetField("ALenderName", string.Concat(instResponse.InstitutionName, "\r\n", instResponse.Madd1, "\r\n", instResponse.Madd2, "\r\n", instResponse.Mcity, ", ", instResponse.Mstate, instResponse.Mprovince, ", ", instResponse.Mcountry, ", ", instResponse.Mzip, "\r\n", instResponse.Phone));   //PayData
            pdfFormFields.SetField("AYear", currentYear);   //23
            pdfFormFields.SetField("ALenderTIN", instResponse.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("ABorrowerTIN", Aresponse.Rcp_TIN);   //request.Rcp_TIN
            pdfFormFields.SetField("AAcquisitionDate", Aresponse.Box_1_Date?.ToString("MM/dd/yyyy"));   //Box_1_Date
            pdfFormFields.SetField("ABalanceOutstanding", Aresponse.Box_2_Amount?.ToString());   //Box_2_Amount
            pdfFormFields.SetField("ABorrowerName", Aresponse.First_Name + " " + Aresponse.Name_Line_2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("AReserved", "Null");   //Null
            pdfFormFields.SetField("AFairValue", Aresponse.Box_4_Amount?.ToString());   //Box_4_Amount
            pdfFormFields.SetField("ABorrowAddress", Aresponse.Address_Deliv_Street + " " + Aresponse.Address_Apt_Suite);   //RecipentAddress
            if (Aresponse.Box_5_Checkbox == "0")
            {
                pdfFormFields.SetField("ARepaymentDebt", "0");   //Box_5_Checkbox
            }
            pdfFormFields.SetField("ABorrowerCityZip", Aresponse.City + " " + Aresponse.State + " " + Aresponse.Country + " " + Aresponse.Zip);   //RecipentCity
            pdfFormFields.SetField("APropertyDescription", Aresponse.Box_6_All_Lines);   //Box_6_All_Lines
            pdfFormFields.SetField("AAccountNumber", Aresponse.Rcp_Account);   //request.Rcp_Account


            if (Aresponse.Corrected == "1")
            {
                pdfFormFields.SetField("BVoid", "0");   //Void
            }
            else
            {
                pdfFormFields.SetField("BCorrected", "0");   //Corrected
            }
            pdfFormFields.SetField("BLenderName", string.Concat(instResponse.InstitutionName, "\r\n", instResponse.Madd1, "\r\n", instResponse.Madd2, "\r\n", instResponse.Mcity, ", ", instResponse.Mstate, instResponse.Mprovince, ", ", instResponse.Mcountry, ", ", instResponse.Mzip, "\r\n", instResponse.Phone));   //PayData
            pdfFormFields.SetField("BYear", currentYear);   //23
            pdfFormFields.SetField("BLenderTIN", instResponse.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("BBorrowerTIN", Aresponse.Rcp_TIN);   //request.Rcp_TIN
            pdfFormFields.SetField("BAcquisitionDate", Aresponse.Box_1_Date?.ToString("MM/dd/yyyy"));   //Box_1_Date
            pdfFormFields.SetField("BBalanceOutstanding", Aresponse.Box_2_Amount?.ToString());   //Box_2_Amount
            pdfFormFields.SetField("BBorrowerName", Aresponse.First_Name + " " + Aresponse.Name_Line_2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("BReserved", "Null");   //Null
            pdfFormFields.SetField("BFairValue", Aresponse.Box_4_Amount?.ToString());   //Box_4_Amount
            pdfFormFields.SetField("BBorrowAddress", Aresponse.Address_Deliv_Street + " " + Aresponse.Address_Apt_Suite);   //RecipentAddress
            if (Aresponse.Box_5_Checkbox == "0")
            {
                pdfFormFields.SetField("BRepaymentDebt", "0");   //Box_5_Checkbox
            }
            pdfFormFields.SetField("BBorrowerCityZip", Aresponse.City + " " + Aresponse.State + " " + Aresponse.Country + " " + Aresponse.Zip);   //RecipentCity
            pdfFormFields.SetField("BPropertyDescription", Aresponse.Box_6_All_Lines);   //Box_6_All_Lines
            pdfFormFields.SetField("BAccountNumber", Aresponse.Rcp_Account);   //request.Rcp_Account

            if (Aresponse.Corrected == "1")
            {
                pdfFormFields.SetField("CVoid", "0");   //Void
            }
            else
            {
                pdfFormFields.SetField("CCorrected", "0");   //Corrected
            }
            pdfFormFields.SetField("CLenderName", string.Concat(instResponse.InstitutionName, "\r\n", instResponse.Madd1, "\r\n", instResponse.Madd2, "\r\n", instResponse.Mcity, ", ", instResponse.Mstate, instResponse.Mprovince, ", ", instResponse.Mcountry, ", ", instResponse.Mzip, "\r\n", instResponse.Phone));   //PayData
            pdfFormFields.SetField("CYear", currentYear);   //23
            pdfFormFields.SetField("CLenderTIN", instResponse.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("CBorrowerTIN", Aresponse.Rcp_TIN);   //request.Rcp_TIN
            pdfFormFields.SetField("CAcquisitionDate", Aresponse.Box_1_Date?.ToString("MM/dd/yyyy"));   //Box_1_Date
            pdfFormFields.SetField("CBalanceOutstanding", Aresponse.Box_2_Amount?.ToString());   //Box_2_Amount
            pdfFormFields.SetField("CBorrowerName", Aresponse.First_Name + " " + Aresponse.Name_Line_2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("CReserved", "Null");   //Null
            pdfFormFields.SetField("CFairValue", Aresponse.Box_4_Amount?.ToString());   //Box_4_Amount
            pdfFormFields.SetField("CBorrowAddress", Aresponse.Address_Deliv_Street + " " + Aresponse.Address_Apt_Suite);   //RecipentAddress
            if (Aresponse.Box_5_Checkbox == "0")
            {
                pdfFormFields.SetField("CRepaymentDebt", "0");   //Box_5_Checkbox
            }
            pdfFormFields.SetField("CBorrowerCityZip", Aresponse.City + " " + Aresponse.State + " " + Aresponse.Country + " " + Aresponse.Zip);   //RecipentCity
            pdfFormFields.SetField("CPropertyDescription", Aresponse.Box_6_All_Lines);   //Box_6_All_Lines
            pdfFormFields.SetField("CAccountNumber", Aresponse.Rcp_Account);   //request.Rcp_Account

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

        public string GeneratePdfForSpecificPage(int Id, string TemplatefilePath, string SaveFolderPath, List<string> selectedPages)
        {
            string newFile1 = string.Empty;
            var request = _evolvedtaxContext.Tbl1099_A.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.First_Name + " " + request.Name_Line_2?.Replace(": ", "");
            newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099A, "_", Id);
            string FilenameNew = "/Form1099A/" + newFile1 + ".pdf";
            string newFileName = newFile1 + ".pdf";



            var newFilePath = CreatePdf(Id, TemplatefilePath, SaveFolderPath, true);

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





        }
        public string GeneratePdForSpecificType(int Id, string TemplatefilePath, string SaveFolderPath, string selectedPage)
        {
            string newFile1 = string.Empty;
            var request = _evolvedtaxContext.Tbl1099_A.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.First_Name + " " + request.Name_Line_2?.Replace(": ", "");

            newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099A, "_", request.Id, "_Page_", selectedPage);
            string FilenameNew = "/Form1099A/" + newFile1 + ".pdf";
            string newFileName = newFile1 + ".pdf";

            var newFilePath = CreatePdf(Id, TemplatefilePath, SaveFolderPath, true, selectedPage);

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
        }
        public string DownloadOneFile(List<int> ids, string SaveFolderPath, List<string> selectedPages, string RootPath)
        {
            var pdfPaths = new List<string>();
            var CompilepdfPaths = new List<string>();
            string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.Form1099ATemplateFileName);
            bool containsAll = selectedPages.Contains("All");

            if (containsAll)
            {
                foreach (var id in ids)
                {
                    var pdfPath = CreatePdf(id, TemplatePathFile, SaveFolderPath, true);
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
                        var pdfPath = GeneratePdForSpecificType(id, TemplatePathFile, SaveFolderPath, selectedPage);
                        pdfPaths.Add(pdfPath);
                    }

                    #region CompilePDFs
                    string compileFileName;

                    switch (selectedPage)
                    {
                        case "2":
                            compileFileName = "Internal Revenue Service Center.pdf";
                            break;
                        case "3":
                            compileFileName = "For Borrower.pdf";
                            break;
                        case "5":
                            compileFileName = "For Lender.pdf";
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
        }
        public string GenerateAndZipPdfs(List<int> ids, string SaveFolderPath, List<string> selectedPages, string RootPath)
        {
            var pdfPaths = new List<string>();

            foreach (var id in ids)
            {

                string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.Form1099ATemplateFileName);
                bool containsAll = selectedPages.Contains("All");

                if (containsAll)
                {
                    var pdfPath = CreatePdf(id, TemplatePathFile, SaveFolderPath, true);
                    pdfPaths.Add(pdfPath);
                }
                else
                {
                    var pdfPath = GeneratePdfForSpecificPage(id, TemplatePathFile, SaveFolderPath, selectedPages);
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
        }

        public async Task<MessageResponseModel> DeletePermeant(int id)
        {

            var recordToDelete = _evolvedtaxContext.Tbl1099_A.First(p => p.Id == id);
            if (recordToDelete != null)
            {
                _evolvedtaxContext.Tbl1099_A.Remove(recordToDelete);
                await _evolvedtaxContext.SaveChangesAsync();
                return new MessageResponseModel { Status = true };
            }

            return new MessageResponseModel { Status = false };
        }

        public async Task<MessageResponseModel> KeepRecord(int id)
        {

            var recordToUpdate = _evolvedtaxContext.Tbl1099_A.First(p => p.Id == id);
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
