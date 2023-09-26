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
    public class Form1099_LS_Service : IForm1099_LS_Service
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly IInstituteService _instituteService;
        readonly IMailService _mailService;
        readonly ITrailAudit1099Service _trailAudit1099Service;
        readonly IMapper _mapper;
        public Form1099_LS_Service(EvolvedtaxContext evolvedtaxContext, IMapper mapper, IInstituteService instituteService, IMailService mailService = null, ITrailAudit1099Service trailAudit1099Service = null)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _mapper = mapper;
            _instituteService = instituteService;
            _mailService = mailService;
            _trailAudit1099Service = trailAudit1099Service;
        }

        public IQueryable<Form1099LSResponse> GetForm1099LSList()
        {
            var response = _evolvedtaxContext.Tbl1099_LS.Select(p => new Form1099LSResponse
            {
                Id = p.Id,
                Corrected = p.Corrected,
                EntityId = p.EntityId,
                Address_Apt_Suite = p.Address_Apt_Suite,
                Address_Deliv_Street = p.Address_Deliv_Street,
                Address_Type = p.Address_Type,
                Name_Line2 = p.Name_Line2,
                Box_1_Amount = p.Box_1_Amount,
                Box_2_Date = p.Box_2_Date,
                Extra_address_line1 = p.Extra_address_line1,
                Extra_address_line2 = p.Extra_address_line2,
                Extra_address_line3 = p.Extra_address_line3,
                Extra_address_line4 = p.Extra_address_line4,
                Extra_contact_name = p.Extra_contact_name,
                Extra_contact_phone = p.Extra_contact_phone,
                Issuer_Name = p.Issuer_Name,
                City = p.City,
                Country = p.Country,
                Created_By = p.Created_By,
                Created_Date = p.Created_Date,
                First_Name = p.First_Name,
                Form_Category = p.Form_Category,
                Form_Source = p.Form_Source,
                InstID = p.InstID,
                Last_Name_Company = p.Last_Name_Company,
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
            var result = from ic in _evolvedtaxContext.Tbl1099_LS
                         where selectValues.Contains(ic.Id) && !string.IsNullOrEmpty(ic.Rcp_Email)
                         select new Tbl1099_LS
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
        public async Task<MessageResponseModel> Upload1099_LS_Data(IFormFile file, int InstId, int entityId, string UserId)
        {
            bool Status = false;
            var response = new List<Tbl1099_LS>();
            var LSList = new List<Tbl1099_LS>();
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
                    var request = new Tbl1099_LS
                    {
                                         

                        Rcp_TIN = excelRow.GetCell(columnMapping["Rcp TIN"])?.ToString() ?? string.Empty,
                        Last_Name_Company = excelRow.GetCell(columnMapping["Company"])?.ToString() ?? string.Empty,
                        First_Name = excelRow.GetCell(columnMapping["First Name"])?.ToString() ?? string.Empty,
                        Name_Line2 = excelRow.GetCell(columnMapping["Last Name"])?.ToString() ?? string.Empty,
                        Address_Type = excelRow.GetCell(columnMapping["Address Type"])?.ToString() ?? string.Empty,
                        Address_Deliv_Street = excelRow.GetCell(columnMapping["Address Line 1"])?.ToString() ?? string.Empty,
                        Address_Apt_Suite = excelRow.GetCell(columnMapping["Address Line 2"])?.ToString() ?? string.Empty,
                        City = excelRow.GetCell(columnMapping["City"])?.ToString() ?? string.Empty,
                        State = excelRow.GetCell(columnMapping["State"])?.ToString() ?? string.Empty,
                        Zip = excelRow.GetCell(columnMapping["Zip"])?.ToString() ?? string.Empty,
                        Country = excelRow.GetCell(columnMapping["Country"])?.ToString() ?? string.Empty,
                        PostalCode = excelRow.GetCell(columnMapping["Postal Code"])?.ToString() ?? string.Empty,
                        Province = excelRow.GetCell(columnMapping["Province"])?.ToString() ?? string.Empty,
                        Rcp_Account = excelRow.GetCell(columnMapping["Rcp Account"])?.ToString() ?? string.Empty,
                        Rcp_Email = excelRow.GetCell(columnMapping["Rcp Email"])?.ToString() ?? string.Empty,
                        Box_1_Amount = excelRow.GetCell(columnMapping["Box 1 Amount"])?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(columnMapping["Box 1 Amount"])?.ToString()) : 0,
                        Box_2_Date = excelRow.GetCell(columnMapping["Box 2 Date"])?.ToString() == null ? null : Convert.ToDateTime(excelRow.GetCell(columnMapping["Box 2 Date"]).ToString()),
                        Issuer_Name = excelRow.GetCell(columnMapping["Issuer Name"])?.ToString() ?? string.Empty,
                        Extra_address_line1 = excelRow.GetCell(columnMapping["Extra address line 1"])?.ToString() ?? string.Empty,
                        Extra_address_line2 = excelRow.GetCell(columnMapping["Extra address line 2"])?.ToString() ?? string.Empty,
                        Extra_address_line3 = excelRow.GetCell(columnMapping["Extra address line 3"])?.ToString() ?? string.Empty,
                        Extra_address_line4 = excelRow.GetCell(columnMapping["Extra address line 4"])?.ToString() ?? string.Empty,
                        Extra_contact_name = excelRow.GetCell(columnMapping["Extra contact name"])?.ToString() ?? string.Empty,
                        Extra_contact_phone = excelRow.GetCell(columnMapping["Extra contact phone"])?.ToString() ?? string.Empty,
                        Form_Category = excelRow.GetCell(columnMapping["Form Category"])?.ToString() ?? string.Empty,
                        Form_Source = excelRow.GetCell(columnMapping["Form Source"])?.ToString() ?? string.Empty,
                        Tax_State = excelRow.GetCell(columnMapping["Tax State"])?.ToString() ?? string.Empty,
                        Corrected = (excelRow.GetCell(columnMapping["Is Corrected"])?.ToString() != null && (bool)excelRow.GetCell(columnMapping["Is Corrected"])?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        Created_By = UserId,
                        Created_Date = DateTime.Now,
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
                    if (await _evolvedtaxContext.Tbl1099_LS.AnyAsync(p => p.Rcp_TIN == request.Rcp_TIN && p.EntityId == request.EntityId && p.Created_Date != null &&
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
                    LSList.Add(request);

                }
                await _evolvedtaxContext.Tbl1099_LS.AddRangeAsync(LSList);
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
            var LSresponse = _evolvedtaxContext.Tbl1099_LS.FirstOrDefault(p => p.Id == Id);
            var instResponse = _instituteService.GetInstituteDataById((int)LSresponse.InstID);
            string templatefile = TemplatefilePath;
            string newFile1 = string.Empty;

            String ClientName = LSresponse.First_Name + " " + LSresponse.Name_Line2?.Replace(": ", "");

            if (!String.IsNullOrEmpty(Page))
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099LS, "_", LSresponse.Id, "_Page_", Page);
            }
            else
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099LS, "_", LSresponse.Id);
            }


            string FilenameNew = "/Form1099LS/" + newFile1 + ".pdf";
            string newFileName = newFile1 + ".pdf"; // Add ".pdf" extension to the file name

            string newFilePath = Path.Combine(SaveFolderPath, newFileName);

            PdfReader pdfReader = new PdfReader(templatefile);
            PdfReader.unethicalreading = true;
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFilePath, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;
            string currentYear = Convert.ToString(DateTime.Now.Year % 100);

            #region PDF Columns
            pdfFormFields.SetField("Form1099-C.CopyA.FormHeader.CalendarYear.f1_1", currentYear);   //23
            if (LSresponse.Corrected == "1")
            {
                pdfFormFields.SetField("Form1099-C.CopyA.FormHeader.c1_1", "0");   //Void
            }
            else
            {
                pdfFormFields.SetField("efield2_Form1099-C.CopyA.FormHeader.c1_1", "0");   //Corrected
            }
            pdfFormFields.SetField("Form1099-C.CopyA.LeftCol.f1_2", string.Concat(instResponse.InstitutionName, "\r\n", instResponse.Madd1, "\r\n", instResponse.Madd2, "\r\n", instResponse.Mcity, ", ", instResponse.Mstate, instResponse.Mprovince, ", ", instResponse.Mcountry, ", ", instResponse.Mzip, "\r\n", instResponse.Phone));   //PayData
            pdfFormFields.SetField("Form1099-C.CopyA.LeftCol.f1_3", instResponse.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("Form1099-C.CopyA.LeftCol.f1_4", LSresponse.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("Form1099-C.CopyA.LeftCol.f1_5", LSresponse.First_Name + " " + LSresponse.Name_Line2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("Form1099-C.CopyA.LeftCol.f1_6", LSresponse.Address_Deliv_Street + " " + LSresponse.Address_Apt_Suite);   //RecipentAddress
            pdfFormFields.SetField("Form1099-C.CopyA.LeftCol.f1_7", LSresponse.City + " " + LSresponse.State + " " + LSresponse.Country + " " + LSresponse.Zip);   //RecipentCity
            pdfFormFields.SetField("Form1099-C.CopyA.LeftCol.f1_8", LSresponse.Rcp_Account);   //request.Rcp_Account
            pdfFormFields.SetField("Form1099-C.CopyA.RightCol.f1_9", LSresponse.Box_1_Amount.ToString());   //Box_1_Amount
            pdfFormFields.SetField("Form1099-C.CopyA.RightCol.f1_10", LSresponse.Box_2_Date?.ToString("MM/dd/yyyy"));   //Box_2_Date
            pdfFormFields.SetField("Form1099-C.CopyA.RightCol.f1_11", LSresponse.Issuer_Name);   //Issuer_Name
            pdfFormFields.SetField("Form1099-C.CopyA.RightCol.f1_12", LSresponse.Extra_address_line1 + " " + LSresponse.Extra_address_line2 + " " + LSresponse.Extra_address_line3 + " " + LSresponse.Extra_address_line4 + " " + LSresponse.Extra_contact_name + " " + LSresponse.Extra_contact_phone);   //Extra_address_line1 + " " + Extra_address_line2 + " " + Extra_address_line3 + " " + Extra_address_line4 + " " + Extra_contact_name + " " + Extra_contact_phone

            pdfFormFields.SetField("Form1099-C.CopyB.FormHeader.CalendarYear.f2_1", currentYear);   //23
            if (LSresponse.Corrected == "0")
            {
                pdfFormFields.SetField("Form1099-C.CopyB.FormHeader.c2_1", "0");   //Corrected
            }
            pdfFormFields.SetField("Form1099-C.CopyB.LeftCol.f2_2", string.Concat(instResponse.InstitutionName, "\r\n", instResponse.Madd1, "\r\n", instResponse.Madd2, "\r\n", instResponse.Mcity, ", ", instResponse.Mstate, instResponse.Mprovince, ", ", instResponse.Mcountry, ", ", instResponse.Mzip, "\r\n", instResponse.Phone));   //PayData
            pdfFormFields.SetField("Form1099-C.CopyB.LeftCol.f2_3", instResponse.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("Form1099-C.CopyB.LeftCol.f2_4", LSresponse.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("Form1099-C.CopyB.LeftCol.f2_5", LSresponse.First_Name + " " + LSresponse.Name_Line2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("Form1099-C.CopyB.LeftCol.f2_6", LSresponse.Address_Deliv_Street + " " + LSresponse.Address_Apt_Suite);   //RecipentAddress
            pdfFormFields.SetField("Form1099-C.CopyB.LeftCol.f2_7", LSresponse.City + " " + LSresponse.State + " " + LSresponse.Country + " " + LSresponse.Zip);   //RecipentCity
            pdfFormFields.SetField("Form1099-C.CopyB.LeftCol.f2_8", LSresponse.Rcp_Account);   //request.Rcp_Account
            pdfFormFields.SetField("Form1099-C.CopyB.RightCol.f2_9", LSresponse.Box_1_Amount.ToString());   //Box_1_Amount
            pdfFormFields.SetField("Form1099-C.CopyB.RightCol.f2_10", "Box_2_Date");   //Box_2_Date
            pdfFormFields.SetField("Form1099-C.CopyB.RightCol.f2_11", LSresponse.Issuer_Name);   //Issuer_Name
            pdfFormFields.SetField("Form1099-C.CopyB.RightCol.f2_12", LSresponse.Extra_address_line1 + " " + LSresponse.Extra_address_line2 + " " + LSresponse.Extra_address_line3 + " " + LSresponse.Extra_address_line4 + " " + LSresponse.Extra_contact_name + " " + LSresponse.Extra_contact_phone);   //Extra_address_line1 + " " + Extra_address_line2 + " " + Extra_address_line3 + " " + Extra_address_line4 + " " + Extra_contact_name + " " + Extra_contact_phone
            pdfFormFields.SetField("Form1099-C.CopyC.FormHeader.CalendarYear.f2_1", currentYear);   //23
            if (LSresponse.Corrected == "0")
            {
                pdfFormFields.SetField("Form1099-C.CopyC.FormHeader.c2_1", "0");   //Corrected
            }
            pdfFormFields.SetField("Form1099-C.CopyC.FormHeader.LeftCol.f2_2", string.Concat(instResponse.InstitutionName, "\r\n", instResponse.Madd1, "\r\n", instResponse.Madd2, "\r\n", instResponse.Mcity, ", ", instResponse.Mstate, instResponse.Mprovince, ", ", instResponse.Mcountry, ", ", instResponse.Mzip, "\r\n", instResponse.Phone));   //PayData
            pdfFormFields.SetField("Form1099-C.CopyC.FormHeader.LeftCol.f2_3", instResponse.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("Form1099-C.CopyC.FormHeader.LeftCol.f2_4", LSresponse.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("Form1099-C.CopyC.FormHeader.LeftCol.f2_5", LSresponse.First_Name + " " + LSresponse.Name_Line2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("Form1099-C.CopyC.FormHeader.LeftCol.f2_6", LSresponse.Address_Deliv_Street + " " + LSresponse.Address_Apt_Suite);   //RecipentAddress
            pdfFormFields.SetField("Form1099-C.CopyC.FormHeader.LeftCol.f2_7", LSresponse.City + " " + LSresponse.State + " " + LSresponse.Country + " " + LSresponse.Zip);   //RecipentCity
            pdfFormFields.SetField("Form1099-C.CopyC.FormHeader.LeftCol.f2_8", LSresponse.Rcp_Account);   //request.Rcp_Account
            pdfFormFields.SetField("Form1099-C.CopyC.FormHeader.RightCol.f2_9", LSresponse.Box_1_Amount.ToString());   //Box_1_Amount
            pdfFormFields.SetField("Form1099-C.CopyC.FormHeader.RightCol.f2_10", "Box_2_Date");   //Box_2_Date
            pdfFormFields.SetField("Form1099-C.CopyC.FormHeader.RightCol.f2_11", LSresponse.Issuer_Name);   //Issuer_Name
            pdfFormFields.SetField("Form1099-C.CopyC.FormHeader.RightCol.f2_12", LSresponse.Extra_address_line1 + " " + LSresponse.Extra_address_line2 + " " + LSresponse.Extra_address_line3 + " " + LSresponse.Extra_address_line4 + " " + LSresponse.Extra_contact_name + " " + LSresponse.Extra_contact_phone);   //Extra_address_line1 + " " + Extra_address_line2 + " " + Extra_address_line3 + " " + Extra_address_line4 + " " + Extra_contact_name + " " + Extra_contact_phone
            pdfFormFields.SetField("Form1099-C.CopyD.FormHeader.CalendarYear.f2_1", currentYear);   //23
            if (LSresponse.Corrected == "1")
            {
                pdfFormFields.SetField("Form1099-C.CopyD.FormHeader.c2_1", "0");   //Void
            }
            else
            {
                pdfFormFields.SetField("efield42_Form1099-C.CopyD.FormHeader.c2_1", "0");   //Corrected
            }
            pdfFormFields.SetField("Form1099-C.CopyD.FormHeader.LeftCol.f2_2", string.Concat(instResponse.InstitutionName, "\r\n", instResponse.Madd1, "\r\n", instResponse.Madd2, "\r\n", instResponse.Mcity, ", ", instResponse.Mstate, instResponse.Mprovince, ", ", instResponse.Mcountry, ", ", instResponse.Mzip, "\r\n", instResponse.Phone));   //PayData
            pdfFormFields.SetField("Form1099-C.CopyD.FormHeader.LeftCol.f2_3", instResponse.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("Form1099-C.CopyD.FormHeader.LeftCol.f2_4", LSresponse.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("Form1099-C.CopyD.FormHeader.LeftCol.f2_5", LSresponse.First_Name + " " + LSresponse.Name_Line2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("Form1099-C.CopyD.FormHeader.LeftCol.f2_6", LSresponse.Address_Deliv_Street + " " + LSresponse.Address_Apt_Suite);   //RecipentAddress
            pdfFormFields.SetField("Form1099-C.CopyD.FormHeader.LeftCol.f2_7", LSresponse.City + " " + LSresponse.State + " " + LSresponse.Country + " " + LSresponse.Zip);   //RecipentCity
            pdfFormFields.SetField("Form1099-C.CopyD.FormHeader.LeftCol.f2_8", LSresponse.Rcp_Account);   //request.Rcp_Account
            pdfFormFields.SetField("Form1099-C.CopyD.FormHeader.RightCol.f2_9", LSresponse.Box_1_Amount.ToString());   //Box_1_Amount
            pdfFormFields.SetField("Form1099-C.CopyD.FormHeader.RightCol.f2_10", "Box_2_Date");   //Box_2_Date
            pdfFormFields.SetField("Form1099-C.CopyD.FormHeader.RightCol.f2_11", LSresponse.Issuer_Name);   //Issuer_Name
            pdfFormFields.SetField("Form1099-C.CopyD.FormHeader.RightCol.f2_12", LSresponse.Extra_address_line1 + " " + LSresponse.Extra_address_line2 + " " + LSresponse.Extra_address_line3 + " " + LSresponse.Extra_address_line4 + " " + LSresponse.Extra_contact_name + " " + LSresponse.Extra_contact_phone);   //Extra_address_line1 + " " + Extra_address_line2 + " " + Extra_address_line3 + " " + Extra_address_line4 + " " + Extra_contact_name + " " + Extra_contact_phone
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
            var request = _evolvedtaxContext.Tbl1099_LS.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.First_Name + " " + request.Name_Line2?.Replace(": ", "");
            newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099LS, "_", Id);
            string FilenameNew = "/Form1099LS/" + newFile1 + ".pdf";
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
            var request = _evolvedtaxContext.Tbl1099_LS.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.First_Name + " " + request.Name_Line2?.Replace(": ", "");

            newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099LS, "_", request.Id, "_Page_", selectedPage);
            string FilenameNew = "/Form1099LS/" + newFile1 + ".pdf";
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
            string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.Form1099LSTemplateFileName);
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
                            compileFileName = "For Payment Recipient.pdf";
                            break;
                        case "5":
                            compileFileName = "For Issuer.pdf";
                            break;
                        case "7":
                            compileFileName = "For Acquirer.pdf";
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

                string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.Form1099LSTemplateFileName);
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

            var recordToDelete = _evolvedtaxContext.Tbl1099_LS.First(p => p.Id == id);
            if (recordToDelete != null)
            {
                _evolvedtaxContext.Tbl1099_LS.Remove(recordToDelete);
                await _evolvedtaxContext.SaveChangesAsync();
                return new MessageResponseModel { Status = true };
            }

            return new MessageResponseModel { Status = false };
        }

        public async Task<MessageResponseModel> KeepRecord(int id)
        {

            var recordToUpdate = _evolvedtaxContext.Tbl1099_LS.First(p => p.Id == id);
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
