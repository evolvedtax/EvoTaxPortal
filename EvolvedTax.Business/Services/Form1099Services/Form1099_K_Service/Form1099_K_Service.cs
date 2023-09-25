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
    public class Form1099_K_Service : IForm1099_K_Service
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly IInstituteService _instituteService;
        readonly IMailService _mailService;
        readonly ITrailAudit1099Service _trailAudit1099Service;
        readonly IMapper _mapper;
        public Form1099_K_Service(EvolvedtaxContext evolvedtaxContext, IMapper mapper, IInstituteService instituteService, IMailService mailService = null, ITrailAudit1099Service trailAudit1099Service = null)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _mapper = mapper;
            _instituteService = instituteService;
            _mailService = mailService;
            _trailAudit1099Service = trailAudit1099Service;
        }

        public IQueryable<Form1099KResponse> GetForm1099KList()
        {
            var response = _evolvedtaxContext.Tbl1099_K.Select(p => new Form1099KResponse
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
                Other_3rd_Party_Checkbox = p.Other_3rd_Party_Checkbox,
                PSE_Name_Telephone_Number = p.PSE_Name_Telephone_Number,
                PSE_Checkbox = p.PSE_Checkbox,
                Box_1a_Amount = p.Box_1a_Amount,
                Box_1b_Amount = p.Box_1b_Amount,
                Box_2_MCC = p.Box_2_MCC,
                Box_3_Number = p.Box_3_Number,
                Box_4_Amount = p.Box_4_Amount,
                Box_5a_Amount = p.Box_5a_Amount,
                Box_5b_Amount = p.Box_5b_Amount,
                Box_5c_Amount = p.Box_5c_Amount,
                Box_5d_Amount = p.Box_5d_Amount,
                Box_5e_Amount = p.Box_5e_Amount,
                Box_5f_Amount = p.Box_5f_Amount,
                Box_5g_Amount = p.Box_5g_Amount,
                Box_5h_Amount = p.Box_5h_Amount,
                Box_5i_Amount = p.Box_5i_Amount,
                Box_5j_Amount = p.Box_5j_Amount,
                Box_5k_Amount = p.Box_5k_Amount,
                Box_5l_Amount = p.Box_5l_Amount,
                Box_6_State = p.Box_6_State,
                Box_7_IDNumber = p.Box_7_IDNumber,
                Box_8_Amount = p.Box_8_Amount,
                Name_Line2 = p.Name_Line2,
                Payment_Card_Chkbox = p.Payment_Card_Chkbox,
                Third_Party_Chkbox = p.Third_Party_Chkbox,
                Form_Category = p.Form_Category,
                Form_Source = p.Form_Source,
                InstID = p.InstID,
                Last_Name_Company = p.Last_Name_Company,
                Rcp_Account = p.Rcp_Account,
                Rcp_Email = p.Rcp_Email != null ? p.Rcp_Email.Trim() : string.Empty,
                Rcp_TIN = p.Rcp_TIN,
                Second_TIN_Notice = p.Second_TIN_Notice,
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
            var result = from ic in _evolvedtaxContext.Tbl1099_K
                         where selectValues.Contains(ic.Id) && !string.IsNullOrEmpty(ic.Rcp_Email)
                         select new Tbl1099_K
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
        public async Task<MessageResponseModel> Upload1099_K_Data(IFormFile file, int InstId, int entityId, string UserId)
        {
            bool Status = false;
            var response = new List<Tbl1099_K>();
            var KList = new List<Tbl1099_K>();
            using (var stream = file.OpenReadStream())
            {
                IWorkbook workbook = new XSSFWorkbook(stream);
                ISheet sheet = workbook.GetSheetAt(0); // Assuming the data is in the first sheet

                HashSet<string> uniqueEINNumber = new HashSet<string>();
                HashSet<string> uniqueEntityNames = new HashSet<string>();

                for (int row = 1; row <= sheet.LastRowNum; row++) // Starting from the second row
                {
                    IRow excelRow = sheet.GetRow(row);

                    var Kresponse = new Tbl1099_K
                    {
                        Rcp_TIN = excelRow.GetCell(0)?.ToString() ?? string.Empty,
                        Last_Name_Company = excelRow.GetCell(1)?.ToString() ?? string.Empty,
                        First_Name = excelRow.GetCell(2)?.ToString() ?? string.Empty,
                        Name_Line2 = excelRow.GetCell(3)?.ToString() ?? string.Empty,
                        Address_Type = excelRow.GetCell(4)?.ToString() ?? string.Empty,
                        Address_Deliv_Street = excelRow.GetCell(5)?.ToString() ?? string.Empty,
                        Address_Apt_Suite = excelRow.GetCell(6)?.ToString() ?? string.Empty,
                        City = excelRow.GetCell(7)?.ToString() ?? string.Empty,
                        State = excelRow.GetCell(8)?.ToString() ?? string.Empty,
                        Zip = excelRow.GetCell(9)?.ToString() ?? string.Empty,
                        Country = excelRow.GetCell(10)?.ToString() ?? string.Empty,
                        Rcp_Account = excelRow.GetCell(11)?.ToString() ?? string.Empty,
                        Rcp_Email = excelRow.GetCell(12)?.ToString() ?? string.Empty,
                        Second_TIN_Notice = (excelRow.GetCell(13)?.ToString() != null && (bool)excelRow.GetCell(13)?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        PSE_Checkbox = (excelRow.GetCell(14)?.ToString() != null && (bool)excelRow.GetCell(14)?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        Other_3rd_Party_Checkbox = (excelRow.GetCell(15)?.ToString() != null && (bool)excelRow.GetCell(15)?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        Payment_Card_Chkbox = (excelRow.GetCell(16)?.ToString() != null && (bool)excelRow.GetCell(16)?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        Third_Party_Chkbox = (excelRow.GetCell(17)?.ToString() != null && (bool)excelRow.GetCell(17)?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        PSE_Name_Telephone_Number = excelRow.GetCell(18)?.ToString() ?? string.Empty,
                        Box_1a_Amount = !string.IsNullOrEmpty(excelRow.GetCell(19)?.ToString()) ? Convert.ToDecimal(excelRow.GetCell(19)?.ToString()) : 0,
                        Box_1b_Amount = !string.IsNullOrEmpty(excelRow.GetCell(20)?.ToString()) ? Convert.ToDecimal(excelRow.GetCell(20)?.ToString()) : 0,
                        Box_2_MCC = !string.IsNullOrEmpty(excelRow.GetCell(21)?.ToString()) ? Convert.ToInt32(excelRow.GetCell(21)?.ToString()) : 0,
                        Box_3_Number = !string.IsNullOrEmpty(excelRow.GetCell(22)?.ToString()) ? Convert.ToDecimal(excelRow.GetCell(22)?.ToString()) : 0,
                        Box_4_Amount = !string.IsNullOrEmpty(excelRow.GetCell(23)?.ToString()) ? Convert.ToDecimal(excelRow.GetCell(23)?.ToString()) : 0,
                        Box_5a_Amount = !string.IsNullOrEmpty(excelRow.GetCell(24)?.ToString()) ? Convert.ToDecimal(excelRow.GetCell(24)?.ToString()) : 0,
                        Box_5b_Amount = !string.IsNullOrEmpty(excelRow.GetCell(25)?.ToString()) ? Convert.ToDecimal(excelRow.GetCell(25)?.ToString()) : 0,
                        Box_5c_Amount = !string.IsNullOrEmpty(excelRow.GetCell(26)?.ToString()) ? Convert.ToDecimal(excelRow.GetCell(26)?.ToString()) : 0,
                        Box_5d_Amount = !string.IsNullOrEmpty(excelRow.GetCell(27)?.ToString()) ? Convert.ToDecimal(excelRow.GetCell(27)?.ToString()) : 0,
                        Box_5e_Amount = !string.IsNullOrEmpty(excelRow.GetCell(28)?.ToString()) ? Convert.ToDecimal(excelRow.GetCell(28)?.ToString()) : 0,
                        Box_5f_Amount = !string.IsNullOrEmpty(excelRow.GetCell(29)?.ToString()) ? Convert.ToDecimal(excelRow.GetCell(29)?.ToString()) : 0,
                        Box_5g_Amount = !string.IsNullOrEmpty(excelRow.GetCell(30)?.ToString()) ? Convert.ToDecimal(excelRow.GetCell(30)?.ToString()) : 0,
                        Box_5h_Amount = !string.IsNullOrEmpty(excelRow.GetCell(31)?.ToString()) ? Convert.ToDecimal(excelRow.GetCell(31)?.ToString()) : 0,
                        Box_5i_Amount = !string.IsNullOrEmpty(excelRow.GetCell(32)?.ToString()) ? Convert.ToDecimal(excelRow.GetCell(32)?.ToString()) : 0,
                        Box_5j_Amount = !string.IsNullOrEmpty(excelRow.GetCell(33)?.ToString()) ? Convert.ToDecimal(excelRow.GetCell(33)?.ToString()) : 0,
                        Box_5k_Amount = !string.IsNullOrEmpty(excelRow.GetCell(34)?.ToString()) ? Convert.ToDecimal(excelRow.GetCell(34)?.ToString()) : 0,
                        Box_5l_Amount = !string.IsNullOrEmpty(excelRow.GetCell(35)?.ToString()) ? Convert.ToDecimal(excelRow.GetCell(35)?.ToString()) : 0,
                        Box_6_State = excelRow.GetCell(36)?.ToString() ?? string.Empty,
                        Box_7_IDNumber = excelRow.GetCell(37)?.ToString() ?? string.Empty,
                        Box_8_Amount = !string.IsNullOrEmpty(excelRow.GetCell(38)?.ToString()) ? Convert.ToDecimal(excelRow.GetCell(38)?.ToString()) : 0,
                        Form_Category = excelRow.GetCell(39)?.ToString() ?? string.Empty,
                        Form_Source = excelRow.GetCell(40)?.ToString() ?? string.Empty,
                        Tax_State = excelRow.GetCell(41)?.ToString() ?? string.Empty,
                        Corrected = (excelRow.GetCell(42)?.ToString() != null && (bool)excelRow.GetCell(42)?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        Created_By = UserId,
                        Created_Date = DateTime.Now,
                        InstID = InstId,
                        EntityId = entityId,
                        UserId = UserId,
                    };
                    string clientEmailEINNumber = Kresponse.Rcp_TIN ?? string.Empty;
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
                    if (await _evolvedtaxContext.Tbl1099_K.AnyAsync(p => p.Rcp_TIN == Kresponse.Rcp_TIN && p.EntityId == Kresponse.EntityId && p.Created_Date != null &&
                     p.Created_Date.Value.Year == DateTime.Now.Year))
                    {
                        response.Add(Kresponse);
                        Status = true;
                        Kresponse.IsDuplicated = true;
                    }
                    else
                    {

                        Kresponse.IsDuplicated = false;
                    }
                    KList.Add(Kresponse);

                }
                await _evolvedtaxContext.Tbl1099_K.AddRangeAsync(KList);
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
            var Kresponse = _evolvedtaxContext.Tbl1099_K.FirstOrDefault(p => p.Id == Id);
            var instResponse = _instituteService.GetInstituteDataById((int)Kresponse.InstID);
            string templatefile = TemplatefilePath;
            string newFile1 = string.Empty;

            string PayData = string.Concat(instResponse.InstitutionName, "\r\n", instResponse.Madd1, "\r\n", instResponse.Madd2, "\r\n", instResponse.Mcity, ", ", instResponse.Mstate, instResponse.Mprovince, ", ", instResponse.Mcountry, ", ", instResponse.Mzip, ", ", instResponse.Phone);
            string RecipentCity = string.Concat(Kresponse.City, ", ", Kresponse.State, ", ", Kresponse.Zip, ", ", Kresponse.Country);
            string RecipentAddress = string.Concat(Kresponse.Address_Deliv_Street, ", ", Kresponse.Address_Apt_Suite);


            String ClientName = Kresponse.First_Name + " " + Kresponse.Name_Line2?.Replace(": ", "");

            if (!String.IsNullOrEmpty(Page))
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099K, "_", Kresponse.Id, "_Page_", Page);
            }
            else
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099K, "_", Kresponse.Id);
            }


            string FilenameNew = "/Form1099K/" + newFile1 + ".pdf";
            string newFileName = newFile1 + ".pdf"; // Add ".pdf" extension to the file name
            string newFilePath = Path.Combine(SaveFolderPath, newFileName);

            PdfReader pdfReader = new PdfReader(templatefile);
            PdfReader.unethicalreading = true;
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFilePath, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;
            string currentYear = Convert.ToString(DateTime.Now.Year % 100);

            #region PDF Columns
            pdfFormFields.SetField("topmostSubform.CopyA.CopyAHeader.CalendarYear.f1_1", currentYear);   //23
            if (Kresponse.Corrected == "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyA.CopyAHeader.c1_1", "0");   //Void
            }
            else
            {
                pdfFormFields.SetField("efield2_topmostSubform.CopyA.CopyAHeader.c1_1", "0");   //Corrected
            }
            pdfFormFields.SetField("topmostSubform.CopyA.LeftCol.f1_2", PayData);   //PayData
            if (Kresponse.PSE_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyA.LeftCol.FILERCheckboxes_ReadOrder.c1_2", "0");   //PSE_Checkbox
            }
            if (Kresponse.Corrected == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyA.LeftCol.FILERCheckboxes_ReadOrder.c1_3", "0");   //Other_3rd_Party_Checkbox
            }
            if (Kresponse.Corrected == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyA.LeftCol.c1_4", "0");   //Payment_Card_Chkbox
            }
            if (Kresponse.Corrected == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyA.LeftCol.c1_5", "0");   //Third_Party_Chkbox
            }

            pdfFormFields.SetField("topmostSubform.CopyA.LeftCol.f1_3", Kresponse.First_Name + " " + Kresponse.Name_Line2);   //Kresponse.First_Name + " " + Kresponse.Name_Line2
            pdfFormFields.SetField("topmostSubform.CopyA.LeftCol.f1_4", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("topmostSubform.CopyA.LeftCol.f1_5", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("topmostSubform.CopyA.LeftCol.f1_6", Kresponse.PSE_Name_Telephone_Number);   //PSE_Name_Telephone_Number
            pdfFormFields.SetField("topmostSubform.CopyA.LeftCol.f1_7", Kresponse.Rcp_Account);   //Kresponse.Rcp_Account
            if (Kresponse.Second_TIN_Notice == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyA.LeftCol.c1_6", "0");   //Second_TIN_Notice
            }
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.f1_8", instResponse.Idnumber);   //instResponse.Idnumber
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.f1_9", Kresponse.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.f1_10", Kresponse.Box_1a_Amount.ToString());   //Box_1a_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.Box1b_ReadOrder.f1_11", Kresponse.Box_1b_Amount.ToString());   //Box_1b_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.f1_12", Kresponse.Box_2_MCC.ToString());   //Box_2_MCC
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.f1_13", Kresponse.Box_3_Number.ToString());   //Box_3_Number
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.f1_14", Kresponse.Box_4_Amount.ToString());   //Box_4_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.Box5a_ReadOrder.f1_15", Kresponse.Box_5a_Amount.ToString());   //Box_5a_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.f1_16", Kresponse.Box_5b_Amount.ToString());   //Box_5b_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.Box5c_ReadOrder.f1_17", Kresponse.Box_5c_Amount.ToString());   //Box_5c_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.f1_18", Kresponse.Box_5d_Amount.ToString());   //Box_5d_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.Box5e_ReadOrder.f1_19", Kresponse.Box_5e_Amount.ToString());   //Box_5e_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.f1_20", Kresponse.Box_5f_Amount.ToString());   //Box_5f_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.Box5g_ReadOrder.f1_21", Kresponse.Box_5g_Amount.ToString());   //Box_5g_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.f1_22", Kresponse.Box_5h_Amount.ToString());   //Box_5h_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.Box5i_ReadOrder.f1_23", Kresponse.Box_5i_Amount.ToString());   //Box_5i_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.f1_24", Kresponse.Box_5j_Amount.ToString());   //Box_5j_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.Box5k_ReadOrder.f1_25", Kresponse.Box_5k_Amount.ToString());   //Box_5k_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.f1_26", Kresponse.Box_5l_Amount.ToString());   //Box_5l_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.Box6_ReadOrder.f1_27", Kresponse.Box_6_State.ToString());   //Box_6_State
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.Box6_ReadOrder.f1_28", Kresponse.Box_6_State.ToString());   //Box_6_State
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.Box7_ReadOrder.f1_29", Kresponse.Box_7_IDNumber.ToString());   //Box_7_IDNumber
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.Box7_ReadOrder.f1_30", Kresponse.Box_7_IDNumber.ToString());   //Box_7_IDNumber
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.f1_31", Kresponse.Box_8_Amount.ToString());   //Box_8_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.f1_32", Kresponse.Box_8_Amount.ToString());   //Box_8_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.Copy1Header.CalendarYear.f2_1", currentYear);   //23
            if (Kresponse.Corrected == "1")
            {
                pdfFormFields.SetField("topmostSubform.Copy1.Copy1Header.c2_1", "0");   //Void
            }
            else
            {
                pdfFormFields.SetField("efield41_topmostSubform.Copy1.Copy1Header.c2_1", "0");   //Corrected
            }
            pdfFormFields.SetField("topmostSubform.Copy1.LeftCol.f2_2", PayData);   //PayData
            if (Kresponse.PSE_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.Copy1.LeftCol.FILERCheckbox_ReadOrder.c2_2", "0");   //PSE_Checkbox
            }
            if (Kresponse.Corrected == "0")
            {
                pdfFormFields.SetField("topmostSubform.Copy1.LeftCol.FILERCheckbox_ReadOrder.c2_3", "0");   //Other_3rd_Party_Checkbox
            }
            if (Kresponse.Corrected == "0")
            {
                pdfFormFields.SetField("topmostSubform.Copy1.LeftCol.c2_4", "0");   //Payment_Card_Chkbox
            }
            if (Kresponse.Corrected == "0")
            {
                pdfFormFields.SetField("topmostSubform.Copy1.LeftCol.c2_5", "0");   //Third_Party_Chkbox
            }
            pdfFormFields.SetField("topmostSubform.Copy1.LeftCol.f2_3", Kresponse.First_Name + " " + Kresponse.Name_Line2);   //Kresponse.First_Name + " " + Kresponse.Name_Line2
            pdfFormFields.SetField("topmostSubform.Copy1.LeftCol.f2_4", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("topmostSubform.Copy1.LeftCol.f2_5", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("topmostSubform.Copy1.LeftCol.f2_6", Kresponse.PSE_Name_Telephone_Number);   //PSE_Name_Telephone_Number
            pdfFormFields.SetField("topmostSubform.Copy1.LeftCol.f2_7", Kresponse.Rcp_Account);   //Kresponse.Rcp_Account
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.f2_8", instResponse.Idnumber);   //instResponse.Idnumber
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.f2_9", Kresponse.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.f2_10", Kresponse.Box_1a_Amount.ToString());   //Box_1a_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.Box1b_ReadOrder.f2_11", Kresponse.Box_1b_Amount.ToString());   //Box_1b_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.f2_12", Kresponse.Box_2_MCC.ToString());   //Box_2_MCC
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.f2_13", Kresponse.Box_3_Number.ToString());   //Box_3_Number
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.f2_14", Kresponse.Box_4_Amount.ToString());   //Box_4_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.Box5a_ReadOrder.f2_15", Kresponse.Box_5a_Amount.ToString());   //Box_5a_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.f2_16", Kresponse.Box_5b_Amount.ToString());   //Box_5b_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.Box5c_ReadOrder.f2_17", Kresponse.Box_5c_Amount.ToString());   //Box_5c_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.f2_18", Kresponse.Box_5d_Amount.ToString());   //Box_5d_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.Box5e_ReadOrder.f2_19", Kresponse.Box_5e_Amount.ToString());   //Box_5e_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.f2_20", Kresponse.Box_5f_Amount.ToString());   //Box_5f_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.Box5g_ReadOrder.f2_21", Kresponse.Box_5g_Amount.ToString());   //Box_5g_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.f2_22", Kresponse.Box_5h_Amount.ToString());   //Box_5h_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.Box5i_ReadOrder.f2_23", Kresponse.Box_5i_Amount.ToString());   //Box_5i_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.f2_24", Kresponse.Box_5j_Amount.ToString());   //Box_5j_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.Box5k_ReadOrder.f2_25", Kresponse.Box_5k_Amount.ToString());   //Box_5k_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.f2_26", Kresponse.Box_5l_Amount.ToString());   //Box_5l_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.Box6_ReadOrder.f2_27", Kresponse.Box_6_State.ToString());   //Box_6_State
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.Box6_ReadOrder.f2_28", Kresponse.Box_6_State.ToString());   //Box_6_State
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.Box7_ReadOrder.f2_29", Kresponse.Box_7_IDNumber.ToString());   //Box_7_IDNumber
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.Box7_ReadOrder.f2_30", Kresponse.Box_7_IDNumber.ToString());   //Box_7_IDNumber
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.f2_31", Kresponse.Box_8_Amount.ToString());   //Box_8_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RghtCol.f2_32", Kresponse.Box_8_Amount.ToString());   //Box_8_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.CopyBHeader.CalendarYear.f2_1", currentYear);   //23
            if (Kresponse.Corrected == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyB.CopyBHeader.c2_1", "0");   //Corrected 
            }
            pdfFormFields.SetField("topmostSubform.CopyB.LeftCol.f2_2", PayData);   //PayData
            if (Kresponse.PSE_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyB.LeftCol.FILERCheckbox_ReadOrder.c2_2", "0");   //PSE_Checkbox 
            }
            if (Kresponse.Other_3rd_Party_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyB.LeftCol.FILERCheckbox_ReadOrder.c2_3", "0");   //Other_3rd_Party_Checkbox
            }
            if (Kresponse.Payment_Card_Chkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyB.LeftCol.c2_4", "0");   //Payment_Card_Chkbox
            }
            if (Kresponse.Third_Party_Chkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyB.LeftCol.c2_5", "0");   //Third_Party_Chkbox
            }
            pdfFormFields.SetField("topmostSubform.CopyB.LeftCol.f2_3", Kresponse.First_Name + " " + Kresponse.Name_Line2);   //Kresponse.First_Name + " " + Kresponse.Name_Line2
            pdfFormFields.SetField("topmostSubform.CopyB.LeftCol.f2_4", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("topmostSubform.CopyB.LeftCol.f2_5", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("topmostSubform.CopyB.LeftCol.f2_6", Kresponse.PSE_Name_Telephone_Number);   //PSE_Name_Telephone_Number
            pdfFormFields.SetField("topmostSubform.CopyB.LeftCol.f2_7", Kresponse.Rcp_Account);   //Kresponse.Rcp_Account
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_8", "instResponse.Idnumber");   //instResponse.Idnumber
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_9", Kresponse.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_10", Kresponse.Box_1a_Amount.ToString());   //Box_1a_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.Box1b_ReadOrder.f2_11", Kresponse.Box_1b_Amount.ToString());   //Box_1b_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_12", Kresponse.Box_2_MCC.ToString());   //Box_2_MCC
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_13", Kresponse.Box_3_Number.ToString());   //Box_3_Number
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_14", Kresponse.Box_4_Amount.ToString());   //Box_4_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.Box5a_ReadOrder.f2_15", Kresponse.Box_5a_Amount.ToString());   //Box_5a_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_16", Kresponse.Box_5b_Amount.ToString());   //Box_5b_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.Box5c_ReadOrder.f2_17", Kresponse.Box_5c_Amount.ToString());   //Box_5c_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_18", Kresponse.Box_5d_Amount.ToString());   //Box_5d_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.Box5e_ReadOrder.f2_19", Kresponse.Box_5e_Amount.ToString());   //Box_5e_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_20", Kresponse.Box_5f_Amount.ToString());   //Box_5f_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.Box5g_ReadOrder.f2_21", Kresponse.Box_5g_Amount.ToString());   //Box_5g_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_22", Kresponse.Box_5h_Amount.ToString());   //Box_5h_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.Box5i_ReadOrder.f2_23", Kresponse.Box_5i_Amount.ToString());   //Box_5i_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_24", Kresponse.Box_5j_Amount.ToString());   //Box_5j_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.Box5k_ReadOrder.f2_25", Kresponse.Box_5k_Amount.ToString());   //Box_5k_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_26", Kresponse.Box_5l_Amount.ToString());   //Box_5l_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.Box6_ReadOrder.f2_27", Kresponse.Box_6_State.ToString());   //Box_6_State
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.Box6_ReadOrder.f2_28", Kresponse.Box_6_State.ToString());   //Box_6_State
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.Box7_ReadOrder.f2_29", Kresponse.Box_7_IDNumber.ToString());   //Box_7_IDNumber
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.Box7_ReadOrder.f2_30", Kresponse.Box_7_IDNumber.ToString());   //Box_7_IDNumber
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_31", Kresponse.Box_8_Amount.ToString());   //Box_8_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_32", Kresponse.Box_8_Amount.ToString());   //Box_8_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.Copy2Header.CalendarYear.f2_1", currentYear);   //23
            if (Kresponse.Corrected == "0")
            {
                pdfFormFields.SetField("topmostSubform.Copy2.Copy2Header.c2_1", "0");   //Corrected
            }
            pdfFormFields.SetField("topmostSubform.Copy2.LeftCol.f2_2", PayData);   //PayData
            if (Kresponse.PSE_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.Copy2.LeftCol.FILERCheckbox_ReadOrder.c2_2", "0");   //PSE_Checkbox
            }
            if (Kresponse.Other_3rd_Party_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.Copy2.LeftCol.FILERCheckbox_ReadOrder.c2_3", "0");   //Other_3rd_Party_Checkbox
            }
            if (Kresponse.Payment_Card_Chkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.Copy2.LeftCol.c2_4", "0");   //Payment_Card_Chkbox
            }
            if (Kresponse.Third_Party_Chkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.Copy2.LeftCol.c2_5", "0");   //Third_Party_Chkbox
            }
            pdfFormFields.SetField("topmostSubform.Copy2.LeftCol.f2_3", Kresponse.First_Name + " " + Kresponse.Name_Line2);   //Kresponse.First_Name + " " + Kresponse.Name_Line2
            pdfFormFields.SetField("topmostSubform.Copy2.LeftCol.f2_4", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("topmostSubform.Copy2.LeftCol.f2_5", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("topmostSubform.Copy2.LeftCol.f2_6", Kresponse.PSE_Name_Telephone_Number);   //PSE_Name_Telephone_Number
            pdfFormFields.SetField("topmostSubform.Copy2.LeftCol.f2_7", Kresponse.Rcp_Account);   //Kresponse.Rcp_Account
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.f2_8", "instResponse.Idnumber");   //instResponse.Idnumber
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.f2_9", Kresponse.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.f2_10", Kresponse.Box_1a_Amount.ToString());   //Box_1a_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.Box1b_ReadOrder.f2_11", Kresponse.Box_1b_Amount.ToString());   //Box_1b_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.f2_12", Kresponse.Box_2_MCC.ToString());   //Box_2_MCC
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.f2_13", Kresponse.Box_3_Number.ToString());   //Box_3_Number
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.f2_14", Kresponse.Box_4_Amount.ToString());   //Box_4_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.Box5a_ReadOrder.f2_15", Kresponse.Box_5a_Amount.ToString());   //Box_5a_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.f2_16", Kresponse.Box_5b_Amount.ToString());   //Box_5b_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.Box5c_ReadOrder.f2_17", Kresponse.Box_5c_Amount.ToString());   //Box_5c_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.f2_18", Kresponse.Box_5d_Amount.ToString());   //Box_5d_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.Box5e_ReadOrder.f2_19", Kresponse.Box_5e_Amount.ToString());   //Box_5e_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.f2_20", Kresponse.Box_5f_Amount.ToString());   //Box_5f_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.Box5g_ReadOrder.f2_21", Kresponse.Box_5g_Amount.ToString());   //Box_5g_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.f2_22", Kresponse.Box_5h_Amount.ToString());   //Box_5h_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.Box5i_ReadOrder.f2_23", Kresponse.Box_5i_Amount.ToString());   //Box_5i_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.f2_24", Kresponse.Box_5j_Amount.ToString());   //Box_5j_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.Box5k_ReadOrder.f2_25", Kresponse.Box_5k_Amount.ToString());   //Box_5k_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.f2_26", Kresponse.Box_5l_Amount.ToString());   //Box_5l_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.Box6_ReadOrder.f2_27", Kresponse.Box_6_State.ToString());   //Box_6_State
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.Box6_ReadOrder.f2_28", Kresponse.Box_6_State.ToString());   //Box_6_State
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.Box7_ReadOrder.f2_29", Kresponse.Box_7_IDNumber.ToString());   //Box_7_IDNumber
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.Box7_ReadOrder.f2_30", Kresponse.Box_7_IDNumber.ToString());   //Box_7_IDNumber
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.f2_31", Kresponse.Box_8_Amount.ToString());   //Box_8_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RghtCol.f2_32", Kresponse.Box_8_Amount.ToString());   //Box_8_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.CopyCHeader.CalendarYear.f2_1", currentYear);   //23
            if (Kresponse.Corrected == "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyC.CopyCHeader.c2_1", "0");   //Void
            }
            else
            {
                pdfFormFields.SetField("efield153_topmostSubform.CopyC.CopyCHeader.c2_1", "0");   //Corrected
            }
            pdfFormFields.SetField("topmostSubform.CopyC.LeftCol.f2_2", PayData);   //PayData
            if (Kresponse.PSE_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyC.LeftCol.FILERCheckbox_ReadOrder.c2_2", "0");   //PSE_Checkbox
            }
            if (Kresponse.Other_3rd_Party_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyC.LeftCol.FILERCheckbox_ReadOrder.c2_3", "0");   //Other_3rd_Party_Checkbox
            }
            if (Kresponse.Payment_Card_Chkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyC.LeftCol.c2_4", "0");   //Payment_Card_Chkbox
            }
            if (Kresponse.Third_Party_Chkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyC.LeftCol.c2_5", "0");   //Third_Party_Chkbox
            }
            pdfFormFields.SetField("topmostSubform.CopyC.LeftCol.f2_3", Kresponse.First_Name + " " + Kresponse.Name_Line2);   //Kresponse.First_Name + " " + Kresponse.Name_Line2
            pdfFormFields.SetField("topmostSubform.CopyC.LeftCol.f2_4", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("topmostSubform.CopyC.LeftCol.f2_5", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("topmostSubform.CopyC.LeftCol.f2_6", Kresponse.PSE_Name_Telephone_Number);   //PSE_Name_Telephone_Number
            pdfFormFields.SetField("topmostSubform.CopyC.LeftCol.f2_7", Kresponse.Rcp_Account);   //Kresponse.Rcp_Account
            if (Kresponse.Second_TIN_Notice == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyC.LeftCol.c2_6", "0");   //Second_TIN_Notice
            }
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_8", "instResponse.Idnumber");   //instResponse.Idnumber
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_9", Kresponse.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_10", Kresponse.Box_1a_Amount.ToString());   //Box_1a_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.Box1b_ReadOrder.f2_11", Kresponse.Box_1b_Amount.ToString());   //Box_1b_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_12", Kresponse.Box_2_MCC.ToString());   //Box_2_MCC
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_13", Kresponse.Box_3_Number.ToString());   //Box_3_Number
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_14", Kresponse.Box_4_Amount.ToString());   //Box_4_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.Box5a_ReadOrder.f2_15", Kresponse.Box_5a_Amount.ToString());   //Box_5a_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_16", Kresponse.Box_5b_Amount.ToString());   //Box_5b_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.Box5c_ReadOrder.f2_17", Kresponse.Box_5c_Amount.ToString());   //Box_5c_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_18", Kresponse.Box_5d_Amount.ToString());   //Box_5d_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.Box5e_ReadOrder.f2_19", Kresponse.Box_5e_Amount.ToString());   //Box_5e_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_20", Kresponse.Box_5f_Amount.ToString());   //Box_5f_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.Box5g_ReadOrder.f2_21", Kresponse.Box_5g_Amount.ToString());   //Box_5g_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_22", Kresponse.Box_5h_Amount.ToString());   //Box_5h_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.Box5i_ReadOrder.f2_23", Kresponse.Box_5i_Amount.ToString());   //Box_5i_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_24", Kresponse.Box_5j_Amount.ToString());   //Box_5j_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.Box5k_ReadOrder.f2_25", Kresponse.Box_5k_Amount.ToString());   //Box_5k_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_26", Kresponse.Box_5l_Amount.ToString());   //Box_5l_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.Box6_ReadOrder.f2_27", Kresponse.Box_6_State.ToString());   //Box_6_State
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.Box6_ReadOrder.f2_28", Kresponse.Box_6_State.ToString());   //Box_6_State
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.Box7_ReadOrder.f2_29", Kresponse.Box_7_IDNumber.ToString());   //Box_7_IDNumber
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.Box7_ReadOrder.f2_30", Kresponse.Box_7_IDNumber.ToString());   //Box_7_IDNumber
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_31", Kresponse.Box_8_Amount.ToString());   //Box_8_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_32", Kresponse.Box_8_Amount.ToString());   //Box_8_Amount
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
            var Kresponse = _evolvedtaxContext.Tbl1099_K.FirstOrDefault(p => p.Id == Id);
            String ClientName = Kresponse.First_Name + " " + Kresponse.Name_Line2?.Replace(": ", "");
            newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099K, "_", Id);
            string FilenameNew = "/Form1099K/" + newFile1 + ".pdf";
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
            var Kresponse = _evolvedtaxContext.Tbl1099_K.FirstOrDefault(p => p.Id == Id);
            String ClientName = Kresponse.First_Name + " " + Kresponse.Name_Line2?.Replace(": ", "");

            newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099K, "_", Kresponse.Id, "_Page_", selectedPage);
            string FilenameNew = "/Form1099K/" + newFile1 + ".pdf";
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
            string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.Form1099KTemplateFileName);
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
                            compileFileName = "For State Tax Department.pdf";
                            break;
                        case "4":
                            compileFileName = "For Payee.pdf";
                            break;
                        case "6":
                            compileFileName = "To be filed with recipient’s state income tax return, when required.pdf";
                            break;
                        case "7":
                            compileFileName = "For FILER.pdf";
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

                string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.Form1099KTemplateFileName);
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

            var recordToDelete = _evolvedtaxContext.Tbl1099_K.First(p => p.Id == id);
            if (recordToDelete != null)
            {
                _evolvedtaxContext.Tbl1099_K.Remove(recordToDelete);
                await _evolvedtaxContext.SaveChangesAsync();
                return new MessageResponseModel { Status = true };
            }

            return new MessageResponseModel { Status = false };
        }

        public async Task<MessageResponseModel> KeepRecord(int id)
        {

            var recordToUpdate = _evolvedtaxContext.Tbl1099_K.First(p => p.Id == id);
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
