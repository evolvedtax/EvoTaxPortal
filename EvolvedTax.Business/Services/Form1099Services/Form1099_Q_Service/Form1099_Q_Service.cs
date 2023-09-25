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
    public class Form1099_Q_Service : IForm1099_Q_Service
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly IInstituteService _instituteService;
        readonly IMailService _mailService;
        readonly ITrailAudit1099Service _trailAudit1099Service;
        readonly IMapper _mapper;
        public Form1099_Q_Service(EvolvedtaxContext evolvedtaxContext, IMapper mapper, IInstituteService instituteService, IMailService mailService = null, ITrailAudit1099Service trailAudit1099Service = null)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _mapper = mapper;
            _instituteService = instituteService;
            _mailService = mailService;
            _trailAudit1099Service = trailAudit1099Service;
        }

        public IQueryable<Form1099QResponse> GetForm1099QList()
        {
            var response = _evolvedtaxContext.Tbl1099_Q.Select(p => new Form1099QResponse
            {
                Id = p.Id,
                Corrected = p.Corrected,
                EntityId = p.EntityId,
                Address_Apt_Suite = p.Address_Apt_Suite,
                Address_Deliv_Street = p.Address_Deliv_Street,
                Address_Type = p.Address_Type,
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
                Box_1_Amount = p.Box_1_Amount,
                Box_2_Amount = p.Box_2_Amount,
                Box_3_Amount = p.Box_3_Amount,
                Box_4_Checkbox = p.Box_4_Checkbox,
                Box_5_Checkbox1 = p.Box_5_Checkbox1,
                Box_5_Checkbox2 = p.Box_5_Checkbox2,
                Box_5_Checkbox3 = p.Box_5_Checkbox3,
                Box_6_Checkbox = p.Box_6_Checkbox,
                Box_X_Amount = p.Box_X_Amount,
                Box_X_Code = p.Box_X_Code,
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
            var result = from ic in _evolvedtaxContext.Tbl1099_Q
                         where selectValues.Contains(ic.Id) && !string.IsNullOrEmpty(ic.Rcp_Email)
                         select new Tbl1099_Q
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
        public async Task<MessageResponseModel> Upload1099_Q_Data(IFormFile file, int InstId, int entityId, string UserId)
        {
            bool Status = false;
            var response = new List<Tbl1099_Q>();
            var QList = new List<Tbl1099_Q>();
            using (var stream = file.OpenReadStream())
            {
                IWorkbook workbook = new XSSFWorkbook(stream);
                ISheet sheet = workbook.GetSheetAt(0); // Assuming the data is in the first sheet

                HashSet<string> uniqueEINNumber = new HashSet<string>();
                HashSet<string> uniqueEntityNames = new HashSet<string>();

                for (int row = 1; row <= sheet.LastRowNum; row++) // Starting from the second row
                {
                    IRow excelRow = sheet.GetRow(row);

                    var request = new Tbl1099_Q
                    {
                        Rcp_TIN = excelRow.GetCell(0)?.ToString() ?? string.Empty,
                        Last_Name_Company = excelRow.GetCell(1)?.ToString() ?? string.Empty,
                        First_Name = excelRow.GetCell(2)?.ToString() ?? string.Empty,
                        Name_Line_2 = excelRow.GetCell(3)?.ToString() ?? string.Empty,
                        Address_Type = excelRow.GetCell(4)?.ToString() ?? string.Empty,
                        Address_Deliv_Street = excelRow.GetCell(5)?.ToString() ?? string.Empty,
                        Address_Apt_Suite = excelRow.GetCell(6)?.ToString() ?? string.Empty,
                        City = excelRow.GetCell(7)?.ToString() ?? string.Empty,
                        State = excelRow.GetCell(8)?.ToString() ?? string.Empty,
                        Zip = excelRow.GetCell(9)?.ToString() ?? string.Empty,
                        Country = excelRow.GetCell(10)?.ToString() ?? string.Empty,
                        Rcp_Account = excelRow.GetCell(11)?.ToString() ?? string.Empty,
                        Rcp_Email = excelRow.GetCell(12)?.ToString() ?? string.Empty,
                        Box_1_Amount = excelRow.GetCell(13)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(13)?.ToString()) : 0,
                        Box_2_Amount = excelRow.GetCell(14)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(14)?.ToString()) : 0,
                        Box_3_Amount = excelRow.GetCell(14)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(15)?.ToString()) : 0,
                        Box_4_Checkbox = (excelRow.GetCell(16)?.ToString() != null && (bool)excelRow.GetCell(16)?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        Box_5_Checkbox1 = (excelRow.GetCell(17)?.ToString() != null && (bool)excelRow.GetCell(17)?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        Box_5_Checkbox2 = (excelRow.GetCell(18)?.ToString() != null && (bool)excelRow.GetCell(18)?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        Box_5_Checkbox3 = (excelRow.GetCell(19)?.ToString() != null && (bool)excelRow.GetCell(19)?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        Box_6_Checkbox = (excelRow.GetCell(20)?.ToString() != null && (bool)excelRow.GetCell(20)?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        Box_X_Amount = excelRow.GetCell(21)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(21)?.ToString()) : 0,
                        Box_X_Code = excelRow.GetCell(22)?.ToString() != null ? Convert.ToInt32(excelRow.GetCell(22)?.ToString()) : 0,
                        Form_Category = excelRow.GetCell(23)?.ToString() ?? string.Empty,
                        Form_Source = excelRow.GetCell(24)?.ToString() ?? string.Empty,
                        Tax_State = excelRow.GetCell(25)?.ToString() ?? string.Empty,
                        Corrected = (excelRow.GetCell(26)?.ToString() != null && (bool)excelRow.GetCell(26)?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
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
                    if (await _evolvedtaxContext.Tbl1099_Q.AnyAsync(p => p.Rcp_TIN == request.Rcp_TIN && p.EntityId == request.EntityId && p.Created_Date != null &&
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
                    QList.Add(request);

                }
                await _evolvedtaxContext.Tbl1099_Q.AddRangeAsync(QList);
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
            var Qresponse = _evolvedtaxContext.Tbl1099_Q.FirstOrDefault(p => p.Id == Id);
            var instResponse = _instituteService.GetInstituteDataById((int)Qresponse.InstID);
            string templatefile = TemplatefilePath;
            string newFile1 = string.Empty;

            String ClientName = Qresponse.First_Name + " " + Qresponse.Name_Line_2?.Replace(": ", "");

            if (!String.IsNullOrEmpty(Page))
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099Q, "_", Qresponse.Id, "_Page_", Page);
            }
            else
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099Q, "_", Qresponse.Id);
            }


            string FilenameNew = "/Form1099Q/" + newFile1 + ".pdf";
            string newFileName = newFile1 + ".pdf"; // Add ".pdf" extension to the file name

            string newFilePath = Path.Combine(SaveFolderPath, newFileName);

            PdfReader pdfReader = new PdfReader(templatefile);
            PdfReader.unethicalreading = true;
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFilePath, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;
            string currentYear = Convert.ToString(DateTime.Now.Year % 100);

            #region PDF Columns

            pdfFormFields.SetField("topmostSubform.CopyA.CopyAHeader.YEAR.f1_1", currentYear);   //23
            if (Qresponse.Corrected == "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyA.CopyAHeader.c1_1", "0");   //Void
            }
            else
            {
                pdfFormFields.SetField("efield2_topmostSubform.CopyA.CopyAHeader.c1_1", "0");   //Corrected
            }

            pdfFormFields.SetField("topmostSubform.CopyA.LeftColumn.f1_2", string.Concat(instResponse.InstitutionName, "\r\n", instResponse.Madd1, "\r\n", instResponse.Madd2, "\r\n", instResponse.Mcity, ", ", instResponse.Mstate, instResponse.Mprovince, ", ", instResponse.Mcountry, ", ", instResponse.Mzip, "\r\n", instResponse.Phone));   //PayData
            pdfFormFields.SetField("topmostSubform.CopyA.LeftColumn.f1_3", instResponse.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("topmostSubform.CopyA.LeftColumn.f1_4", Qresponse.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("topmostSubform.CopyA.LeftColumn.f1_5", Qresponse.First_Name + " " + Qresponse.Name_Line_2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("topmostSubform.CopyA.LeftColumn.f1_6", Qresponse.Address_Deliv_Street + " " + Qresponse.Address_Apt_Suite);   //RecipentAddress
            pdfFormFields.SetField("topmostSubform.CopyA.LeftColumn.f1_7", Qresponse.City + " " + Qresponse.State + " " + Qresponse.Country + " " + Qresponse.Zip);   //RecipentCity

            pdfFormFields.SetField("topmostSubform.CopyA.LeftColumn.f1_8", Qresponse.Rcp_Account);   //request.Rcp_Account

            pdfFormFields.SetField("topmostSubform.CopyA.RightColumn.f1_9", Qresponse.Box_1_Amount.ToString());   //Box_1_Amount

            pdfFormFields.SetField("topmostSubform.CopyA.RightColumn.f1_10", Qresponse.Box_2_Amount.ToString());   //Box_2_Amount

            pdfFormFields.SetField("topmostSubform.CopyA.RightColumn.f1_11", Qresponse.Box_3_Amount.ToString());   //Box_3_Amount
            if (Qresponse.Box_4_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyA.RightColumn.c1_2", "0");   //Box_4_Checkbox
            }
            if (Qresponse.Box_5_Checkbox1 == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyA.RightColumn.Box5.list_1.list_item_1.c1_3", "0");   //Box_5_Checkbox1
            }
            if (Qresponse.Box_5_Checkbox2 == "0")
            {
                pdfFormFields.SetField("efield15_topmostSubform.CopyA.RightColumn.Box5.list_1.list_item_1.c1_3", "0");   //Box_5_Checkbox2
            }
            if (Qresponse.Box_5_Checkbox3 == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyA.RightColumn.Box5.list_1.list_item_2.c1_3", "0");   //Box_5_Checkbox3
            }
            if (Qresponse.Box_6_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyA.RightColumn.c1_4", "0");   //Box_6_Checkbox
            }
            pdfFormFields.SetField("topmostSubform.CopyB.CopyBHeader.YEAR.f1_1", currentYear);   //23
            if (Qresponse.Corrected == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyB.CopyBHeader.c1_1", "0");   //Corrected
            }
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.f1_2", string.Concat(instResponse.InstitutionName, "\r\n", instResponse.Madd1, "\r\n", instResponse.Madd2, "\r\n", instResponse.Mcity, ", ", instResponse.Mstate, instResponse.Mprovince, ", ", instResponse.Mcountry, ", ", instResponse.Mzip, "\r\n", instResponse.Phone));   //PayData

            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.f1_3", instResponse.Idnumber);   //requestInstitue.Idnumber

            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.f1_4", Qresponse.Rcp_TIN);   //Rcp_TIN

            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.f1_5", Qresponse.First_Name + " " + Qresponse.Name_Line_2);   //request.First_Name + " " + request.Name_Line2

            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.f1_6", Qresponse.Address_Deliv_Street + " " + Qresponse.Address_Apt_Suite);   //RecipentAddress

            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.f1_7", Qresponse.City + " " + Qresponse.State + " " + Qresponse.Country + " " + Qresponse.Zip);   //RecipentCity

            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.f1_8", Qresponse.Rcp_Account);   //request.Rcp_Account

            pdfFormFields.SetField("topmostSubform.CopyB.RghtCol.f1_9", Qresponse.Box_1_Amount.ToString());   //Box_1_Amount

            pdfFormFields.SetField("topmostSubform.CopyB.RghtCol.f1_10", Qresponse.Box_2_Amount.ToString());   //Box_2_Amount

            pdfFormFields.SetField("topmostSubform.CopyB.RghtCol.f1_11", Qresponse.Box_3_Amount.ToString());   //Box_3_Amount
            if (Qresponse.Box_4_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyB.RghtCol.c1_2", "0");   //Box_4_Checkbox
            }
            if (Qresponse.Box_5_Checkbox1 == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyB.RghtCol.Box5.list_2.list_item_1.c1_3", "0");   //Box_5_Checkbox1
            }
            if (Qresponse.Box_5_Checkbox2 == "0")
            {
                pdfFormFields.SetField("efield32_topmostSubform.CopyB.RghtCol.Box5.list_2.list_item_1.c1_3", "0");   //Box_5_Checkbox2
            }
            if (Qresponse.Box_5_Checkbox3 == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyB.RghtCol.Box5.list_2.list_item_2.c1_3", "0");   //Box_5_Checkbox3
            }
            if (Qresponse.Box_6_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyB.RghtCol.c1_4", "0");   //Box_6_Checkbox
            }
            pdfFormFields.SetField("topmostSubform.CopyB.RghtCol.f1_12", Qresponse.Box_X_Amount + " & " + Qresponse.Box_X_Code);   //Box_X_Amount & " " & Box_X_Code

            pdfFormFields.SetField("topmostSubform.CopyC.CopyCHeader.YEAR.f1_1", currentYear);   //23

            pdfFormFields.SetField("topmostSubform.CopyC.CopyCHeader.c1_1", "Void");   //Void
            if (Qresponse.Corrected == "0")
            {
                pdfFormFields.SetField("efield38_topmostSubform.CopyC.CopyCHeader.c1_1", "0");   //Corrected
            }
            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.f1_2", string.Concat(instResponse.InstitutionName, "\r\n", instResponse.Madd1, "\r\n", instResponse.Madd2, "\r\n", instResponse.Mcity, ", ", instResponse.Mstate, instResponse.Mprovince, ", ", instResponse.Mcountry, ", ", instResponse.Mzip, "\r\n", instResponse.Phone));   //PayData

            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.f1_3", instResponse.Idnumber);   //requestInstitue.Idnumber

            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.f1_4", Qresponse.Rcp_TIN);   //Rcp_TIN

            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.f1_5", Qresponse.First_Name + " " + Qresponse.Name_Line_2);   //request.First_Name + " " + request.Name_Line2

            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.f1_6", Qresponse.Address_Deliv_Street + " " + Qresponse.Address_Apt_Suite);   //RecipentAddress

            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.f1_7", Qresponse.City + " " + Qresponse.State + " " + Qresponse.Country + " " + Qresponse.Zip);   //RecipentCity

            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.f1_8", Qresponse.Rcp_Account);   //request.Rcp_Account

            pdfFormFields.SetField("topmostSubform.CopyC.RightColumn.f1_9", Qresponse.Box_1_Amount.ToString());   //Box_1_Amount

            pdfFormFields.SetField("topmostSubform.CopyC.RightColumn.f1_10", Qresponse.Box_2_Amount.ToString());   //Box_2_Amount

            pdfFormFields.SetField("topmostSubform.CopyC.RightColumn.f1_11", Qresponse.Box_3_Amount.ToString());   //Box_3_Amount
            if (Qresponse.Box_4_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyC.RightColumn.c1_2", "0");   //Box_4_Checkbox
            }
            if (Qresponse.Box_5_Checkbox1 == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyC.RightColumn.Box5.list_3.list_item_1.c1_3", "0");   //Box_5_Checkbox1
            }
            if (Qresponse.Box_5_Checkbox2 == "0")
            {
                pdfFormFields.SetField("efield51_topmostSubform.CopyC.RightColumn.Box5.list_3.list_item_1.c1_3", "0");   //Box_5_Checkbox2
            }
            if (Qresponse.Box_5_Checkbox3 == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyC.RightColumn.Box5.list_3.list_item_2.c1_3", "0");   //Box_5_Checkbox3
            }
            if (Qresponse.Box_6_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyC.RightColumn.c1_4", "0");   //Box_6_Checkbox
            }
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
            var request = _evolvedtaxContext.Tbl1099_Q.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.First_Name + " " + request.Name_Line_2?.Replace(": ", "");
            newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099Q, "_", Id);
            string FilenameNew = "/Form1099Q/" + newFile1 + ".pdf";
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
            var request = _evolvedtaxContext.Tbl1099_Q.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.First_Name + " " + request.Name_Line_2?.Replace(": ", "");

            newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099Q, "_", request.Id, "_Page_", selectedPage);
            string FilenameNew = "/Form1099Q/" + newFile1 + ".pdf";
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
            string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.Form1099QTemplateFileName);
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
                        case "1":
                            compileFileName = "Internal Revenue Service Center.pdf";
                            break;
                        case "2":
                            compileFileName = "For Recipient.pdf";
                            break;
                        case "4":
                            compileFileName = "For Payer.pdf";
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

                string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.Form1099QTemplateFileName);
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

            var recordToDelete = _evolvedtaxContext.Tbl1099_Q.First(p => p.Id == id);
            if (recordToDelete != null)
            {
                _evolvedtaxContext.Tbl1099_Q.Remove(recordToDelete);
                await _evolvedtaxContext.SaveChangesAsync();
                return new MessageResponseModel { Status = true };
            }

            return new MessageResponseModel { Status = false };
        }

        public async Task<MessageResponseModel> KeepRecord(int id)
        {

            var recordToUpdate = _evolvedtaxContext.Tbl1099_Q.First(p => p.Id == id);
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
