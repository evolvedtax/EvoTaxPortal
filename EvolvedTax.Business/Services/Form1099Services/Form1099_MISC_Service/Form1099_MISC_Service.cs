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
    public class Form1099_MISC_Service : IForm1099_MISC_Service
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly IInstituteService _instituteService;
        readonly IMailService _mailService;
        readonly ITrailAudit1099Service _trailAudit1099Service;
        readonly IMapper _mapper;
        public Form1099_MISC_Service(EvolvedtaxContext evolvedtaxContext, IMapper mapper, IInstituteService instituteService, IMailService mailService = null, ITrailAudit1099Service trailAudit1099Service = null)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _mapper = mapper;
            _instituteService = instituteService;
            _mailService = mailService;
            _trailAudit1099Service = trailAudit1099Service;
        }

        public IQueryable<Form1099MISCResponse> GetForm1099MISCList()
        {
            var response = _evolvedtaxContext.Tbl1099_MISC.Select(p => new Form1099MISCResponse
            {
                Id = p.Id,
                Corrected = p.Corrected,
                EntityId = p.EntityId,
                Address_Apt_Suite = p.Address_Apt_Suite,
                Address_Deliv_Street = p.Address_Deliv_Street,
                Address_Type = p.Address_Type,
                Batch_ID = p.Batch_ID,
                Box_10_Amount = p.Box_10_Amount,
                Box_11_Amount = p.Box_11_Amount,
                Box_12_Amount = p.Box_12_Amount,
                Box_14_Amount = p.Box_14_Amount,
                Box_15_Amount = p.Box_15_Amount,
                Box_16_Amount = p.Box_16_Amount,
                Box_17_ID_Number = p.Box_17_ID_Number,
                Box_17_State = p.Box_17_State,
                Box_18_Amount = p.Box_18_Amount,
                Box_1_Amount = p.Box_1_Amount,
                Box_2_Amount = p.Box_2_Amount,
                Box_3_Amount = p.Box_3_Amount,
                Box_4_Amount = p.Box_4_Amount,
                Box_5_Amount = p.Box_5_Amount,
                Box_6_Amount = p.Box_6_Amount,
                Box_7_Checkbox = p.Box_7_Checkbox,
                Box_8_Amount = p.Box_8_Amount,
                Box_9_Amount = p.Box_9_Amount,
                City = p.City,
                Country = p.Country,
                Created_By = p.Created_By,
                Created_Date = p.Created_Date,
                FATCA_Checkbox = p.FATCA_Checkbox,
                First_Name = p.First_Name,
                Form_Category = p.Form_Category,
                Form_Source = p.Form_Source,
                InstID = p.InstID,
                Last_Name_Company = p.Last_Name_Company,
                Name_Line_2 = p.Name_Line_2,
                Opt_Rcp_Text_Line_1 = p.Opt_Rcp_Text_Line_1,
                Opt_Rcp_Text_Line_2 = p.Opt_Rcp_Text_Line_2,
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
                IsDuplicated=p.IsDuplicated
            });
            return response;
        }
        public async Task<bool> SendEmailToRecipients(int[] selectValues, string URL, string form)
        {
            var result = from ic in _evolvedtaxContext.Tbl1099_MISC
                         where selectValues.Contains(ic.Id) && !string.IsNullOrEmpty(ic.Rcp_Email)
                         select new Tbl1099_MISC
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
        public async Task<MessageResponseModel> Upload1099_MISC_Data(IFormFile file, int InstId, int entityId, string UserId)
        {
            bool Status = false;
            var response = new List<Tbl1099_MISC>();
            var mISCList = new List<Tbl1099_MISC>();
            using (var stream = file.OpenReadStream())
            {
                IWorkbook workbook = new XSSFWorkbook(stream);
                ISheet sheet = workbook.GetSheetAt(0); // Assuming the data is in the first sheet

                HashSet<string> uniqueEINNumber = new HashSet<string>();
                HashSet<string> uniqueEntityNames = new HashSet<string>();

                for (int row = 1; row <= sheet.LastRowNum; row++) // Starting from the second row
                {
                    IRow excelRow = sheet.GetRow(row);

                    var request = new Tbl1099_MISC
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
                        Second_TIN_Notice = excelRow.GetCell(13)?.ToString() ?? string.Empty,
                        FATCA_Checkbox = (excelRow.GetCell(14)?.ToString() != null && (bool)excelRow.GetCell(14)?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        Box_1_Amount = excelRow.GetCell(15)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(15)?.ToString()) : 0,
                        Box_2_Amount = excelRow.GetCell(16)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(16)?.ToString()) : 0,
                        Box_3_Amount = excelRow.GetCell(17)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(17)?.ToString()) : 0,
                        Box_4_Amount = excelRow.GetCell(18)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(18)?.ToString()) : 0,
                        Box_5_Amount = excelRow.GetCell(19)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(19)?.ToString()) : 0,
                        Box_6_Amount = excelRow.GetCell(20)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(20)?.ToString()) : 0,
                        Box_7_Checkbox = (excelRow.GetCell(21)?.ToString() != null && (bool)excelRow.GetCell(21)?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        Box_8_Amount = excelRow.GetCell(22)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(22)?.ToString()) : 0,
                        Box_9_Amount = excelRow.GetCell(23)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(23)?.ToString()) : 0,
                        Box_10_Amount = excelRow.GetCell(24)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(24)?.ToString()) : 0,
                        Box_11_Amount = excelRow.GetCell(25)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(25)?.ToString()) : 0,
                        Box_12_Amount = excelRow.GetCell(26)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(26)?.ToString()) : 0,
                        Box_14_Amount = excelRow.GetCell(27)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(27)?.ToString()) : 0,
                        Box_15_Amount = excelRow.GetCell(28)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(28)?.ToString()) : 0,
                        Box_16_Amount = excelRow.GetCell(29)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(29)?.ToString()) : 0,
                        Box_17_State = excelRow.GetCell(30)?.ToString() ?? string.Empty,
                        Box_17_ID_Number = excelRow.GetCell(31)?.ToString() ?? string.Empty,
                        Box_18_Amount = excelRow.GetCell(32)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(32)?.ToString()) : 0,
                        Opt_Rcp_Text_Line_1 = excelRow.GetCell(33)?.ToString() ?? string.Empty,
                        Opt_Rcp_Text_Line_2 = excelRow.GetCell(34)?.ToString() ?? string.Empty,
                        Form_Category = excelRow.GetCell(35)?.ToString() ?? string.Empty,
                        Form_Source = excelRow.GetCell(36)?.ToString() ?? string.Empty,
                        Batch_ID = excelRow.GetCell(37)?.ToString() ?? string.Empty,
                        Tax_State = excelRow.GetCell(38)?.ToString() ?? string.Empty,
                        Corrected = (excelRow.GetCell(39)?.ToString() != null && (bool)excelRow.GetCell(39)?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
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
                    if (await _evolvedtaxContext.Tbl1099_MISC.AnyAsync(p => p.Rcp_TIN == request.Rcp_TIN && p.EntityId==request.EntityId))
                    {
                        response.Add(request);
                        Status = true;
                        request.IsDuplicated = true;
                    }
                    else
                    {
                       
                        request.IsDuplicated = false;
                    }
                    mISCList.Add(request);

                }
                await _evolvedtaxContext.Tbl1099_MISC.AddRangeAsync(mISCList);
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
            var mISCresponse = _evolvedtaxContext.Tbl1099_MISC.FirstOrDefault(p => p.Id == Id);
            var instResponse = _instituteService.GetInstituteDataById((int)mISCresponse.InstID);
            string templatefile = TemplatefilePath;
            string newFile1 = string.Empty;

            String ClientName = mISCresponse.First_Name + " " + mISCresponse.Name_Line_2?.Replace(": ", ""); 

            if (!String.IsNullOrEmpty(Page))
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099MISC, "_", mISCresponse.Id, "_Page_", Page);
            }
            else
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099MISC, "_", mISCresponse.Id);
            }


            string FilenameNew = "/Form1099MISC/" + newFile1 + ".pdf";
            string newFileName = newFile1 + ".pdf"; // Add ".pdf" extension to the file name

            string newFilePath = Path.Combine(SaveFolderPath, newFileName);

            PdfReader pdfReader = new PdfReader(templatefile);
            PdfReader.unethicalreading = true;
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFilePath, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;



            #region PDF Columns
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].CopyAHeader[0].CalendarYear[0].f1_1[0]", "23");   //CalYear
            if (mISCresponse.Corrected == "1")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyA[0].CopyAHeader[0].c1_1[0]", "0");   //VOID
            }
            else
            {
                pdfFormFields.SetField("topmostSubform[0].CopyA[0].CopyAHeader[0].c1_1[1]", "0");   //CORRECTED
            }
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_2[0]", string.Concat(instResponse.InstitutionName, "\r\n", instResponse.Madd1, "\r\n", instResponse.Madd2, "\r\n", instResponse.Mcity, ", ", instResponse.Mstate, instResponse.Mprovince, ", ", instResponse.Mcountry, ", ", instResponse.Mzip, "\r\n", instResponse.Phone));   //PAYER’S name, street address, city or town, state or province, country, ZIP or foreign postal code, and telephone no
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_3[0]", instResponse.Ftin);   //Payer TIN
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_4[0]", mISCresponse.Rcp_TIN);   //Recepients TIN
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_5[0]", mISCresponse.First_Name + " " + mISCresponse.Name_Line_2);   //RECIPIENT’S name
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_6[0]", mISCresponse.Address_Deliv_Street + " " + mISCresponse.Address_Apt_Suite);   //Street address (including apt. no.)
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_7[0]", mISCresponse.City + " " + mISCresponse.State + " " + mISCresponse.Country + " " + mISCresponse.Zip);   //City or town, state or province, country, and ZIP or foreign postal code
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_8[0]", mISCresponse.Rcp_Account);   //Account number (see instructions)
            if (mISCresponse.Second_TIN_Notice == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].c1_2[0]", "0");   //2nd TIN not.
            }
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_9[0]", mISCresponse.Box_1_Amount.ToString());   //1 Rents
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_10[0]", mISCresponse.Box_2_Amount.ToString());   //2 Royalties
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_11[0]", mISCresponse.Box_3_Amount.ToString());   //3 Other income
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_12[0]", mISCresponse.Box_4_Amount.ToString());   //4 Federal income tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_13[0]", mISCresponse.Box_5_Amount.ToString());   //5 Fishing boat proceeds
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_14[0]", mISCresponse.Box_6_Amount.ToString());   //6 Medical and health care payments
            if (mISCresponse.Box_7_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].c1_4[0]", "0");   //7 CheckBox Payer made direct sales totaling $5,000 or more of consumer products to recipient for resale
            }
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_15[0]", mISCresponse.Box_8_Amount.ToString());   //8 Substitute payments in lieu of dividends or interest
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_16[0]", mISCresponse.Box_9_Amount.ToString());   //9 Crop insurance proceeds
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_17[0]", mISCresponse.Box_10_Amount.ToString());   //10 Gross proceeds paid to an attorney
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_18[0]", mISCresponse.Box_11_Amount.ToString());   //11 Fish purchased for resale
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_19[0]", mISCresponse.Box_12_Amount.ToString());   //12 Section 409A deferrals
            if (mISCresponse.FATCA_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].TagCorrectingSubform[0].c1_3[0]", "0");   //13 FATCA filing requirement
            }
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].Box14_ReadOrder[0].f1_20[0]", mISCresponse.Box_14_Amount.ToString()); //14 Excess golden parachute payments
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_21[0]", mISCresponse.Box_15_Amount.ToString());   //15 Nonqualified deferred compensation
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].Box16_ReadOrder[0].f1_22[0]", mISCresponse.Box_16_Amount.ToString());   //16 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].Box16_ReadOrder[0].f1_23[0]", mISCresponse.Box_16_Amount.ToString());   //16 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].Box17_ReadOrder[0].f1_24[0]", mISCresponse.Box_17_ID_Number);   //17 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].Box17_ReadOrder[0].f1_25[0]", mISCresponse.Box_17_State);   //17 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].f1_26[0]", mISCresponse.Box_18_Amount.ToString());   //18 State income
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].f1_27[0]", mISCresponse.Box_18_Amount.ToString());   //18 State income
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].Copy1Header[0].CalendarYear[0].f2_1[0]", "23");   //CalYear
            if (mISCresponse.Corrected == "1")
            {
                pdfFormFields.SetField("topmostSubform[0].Copy1[0].Copy1Header[0].c2_1[0]", "0");   //VOID
            }
            else
            {
                pdfFormFields.SetField("topmostSubform[0].Copy1[0].Copy1Header[0].c2_1[1]", "0");   //CORRECTED
            }
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].LeftColumn[0].f2_2[0]", string.Concat(instResponse.InstitutionName, ", ", instResponse.Madd1, ", ", instResponse.Madd2, ", ", instResponse.Mcity, ", ", instResponse.Mstate, instResponse.Mprovince, ", ", instResponse.Mcountry, ", ", instResponse.Mzip, ", ", instResponse.Phone));   //PAYER’S name, street address, city or town, state or province, country, ZIP or foreign postal code, and telephone no
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].LeftColumn[0].f2_3[0]", instResponse.Ftin);   //Payer TIN
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].LeftColumn[0].f2_4[0]", mISCresponse.Rcp_TIN);   //Recepients TIN
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].LeftColumn[0].f2_5[0]", mISCresponse.First_Name + " " + mISCresponse.Name_Line_2);   //RECIPIENT’S name
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].LeftColumn[0].f2_6[0]", mISCresponse.Address_Deliv_Street + " " + mISCresponse.Address_Apt_Suite);   //Street address (including apt. no.)
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].LeftColumn[0].f2_7[0]", mISCresponse.City + " " + mISCresponse.State + " " + mISCresponse.Country + " " + mISCresponse.Zip);   //City or town, state or province, country, and ZIP or foreign postal code
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].LeftColumn[0].f2_8[0]", mISCresponse.Rcp_Account);   //Account number (see instructions)
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_9[0]", mISCresponse.Box_1_Amount.ToString());   //1 Rents
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_10[0]", mISCresponse.Box_2_Amount.ToString());   //2 Royalties
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_11[0]", mISCresponse.Box_3_Amount.ToString());   //3 Other income
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_12[0]", mISCresponse.Box_4_Amount.ToString());   //4 Federal income tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_13[0]", mISCresponse.Box_5_Amount.ToString());   //5 Fishing boat proceeds
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_14[0]", mISCresponse.Box_6_Amount.ToString());   //6 Medical and health care payments
            if (mISCresponse.Box_7_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].c2_4[0]", mISCresponse.Box_7_Checkbox);   //7 CheckBox Payer made direct sales totaling $5,000 or more of consumer products to recipient for resale
            }
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_15[0]", mISCresponse.Box_8_Amount.ToString());   //8 Substitute payments in lieu of dividends or interest
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_16[0]", mISCresponse.Box_9_Amount.ToString());   //9 Crop insurance proceeds
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_17[0]", mISCresponse.Box_10_Amount.ToString());   //10 Gross proceeds paid to an attorney
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_18[0]", mISCresponse.Box_11_Amount.ToString());   //11 Fish purchased for resale
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_19[0]", mISCresponse.Box_12_Amount.ToString());   //12 Section 409A deferrals
            if (mISCresponse.FATCA_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].TagCorrectingSubform[0].c2_3[0]", "0");   //13 FATCA filing requirement
            }
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].Box14_ReadOrder[0].f2_20[0]", mISCresponse.Box_14_Amount.ToString());   //12 Section 409A deferrals);   //14 Excess golden parachute payments
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_21[0]", mISCresponse.Box_15_Amount.ToString());   //15 Nonqualified deferred compensation
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].Box16_ReadOrder[0].f2_22[0]", mISCresponse.Box_16_Amount.ToString());   //16 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].Box16_ReadOrder[0].f2_23[0]", mISCresponse.Box_16_Amount.ToString());   //16 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].Box17_ReadOrder[0].f2_24[0]", mISCresponse.Box_17_ID_Number);   //17 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].Box17_ReadOrder[0].f2_25[0]", mISCresponse.Box_17_State);   //17 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].f2_26[0]", "StateIncome1");   //18 State income
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].f2_27[0]", "StateIncome2");   //18 State income
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].CopyBHeader[0].CalendarYear[0].f2_1[0]", "23");   //CalYear
            if (mISCresponse.Corrected == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyB[0].CopyBHeader[0].c2_1[0]", "0");   //CORRECTED
            }
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftColumn[0].f2_2[0]", string.Concat(instResponse.InstitutionName, ", ", instResponse.Madd1, ", ", instResponse.Madd2, ", ", instResponse.Mcity, ", ", instResponse.Mstate, instResponse.Mprovince, ", ", instResponse.Mcountry, ", ", instResponse.Mzip, ", ", instResponse.Phone));   //PAYER’S name, street address, city or town, state or province, country, ZIP or foreign postal code, and telephone no
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftColumn[0].f2_3[0]", instResponse.Ftin);   //Payer TIN
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftColumn[0].f2_4[0]", mISCresponse.Rcp_TIN);   //Recepients TIN
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftColumn[0].f2_5[0]", mISCresponse.First_Name + " " + mISCresponse.Name_Line_2);   //RECIPIENT’S name
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftColumn[0].f2_6[0]", mISCresponse.Address_Deliv_Street + " " + mISCresponse.Address_Apt_Suite);   //Street address (including apt. no.)
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftColumn[0].f2_7[0]", mISCresponse.City + " " + mISCresponse.State + " " + mISCresponse.Country + " " + mISCresponse.Zip);   //City or town, state or province, country, and ZIP or foreign postal code
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftColumn[0].f2_8[0]", mISCresponse.Rcp_Account);   //Account number (see instructions)
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_9[0]", mISCresponse.Box_1_Amount.ToString());   //1 Rents
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_10[0]", mISCresponse.Box_2_Amount.ToString());   //2 Royalties
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_11[0]", mISCresponse.Box_3_Amount.ToString());   //3 Other income
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_12[0]", mISCresponse.Box_4_Amount.ToString());   //4 Federal income tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_13[0]", mISCresponse.Box_5_Amount.ToString());   //5 Fishing boat proceeds
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_14[0]", mISCresponse.Box_6_Amount.ToString());   //6 Medical and health care payments
            if (mISCresponse.Box_7_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].c2_4[0]", mISCresponse.Box_7_Checkbox);   //7 CheckBox Payer made direct sales totaling $5,000 or more of consumer products to recipient for resale
            }
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_15[0]", mISCresponse.Box_8_Amount.ToString());   //8 Substitute payments in lieu of dividends or interest
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_16[0]", mISCresponse.Box_9_Amount.ToString());   //9 Crop insurance proceeds
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_17[0]", mISCresponse.Box_10_Amount.ToString());   //10 Gross proceeds paid to an attorney
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_18[0]", mISCresponse.Box_11_Amount.ToString());   //11 Fish purchased for resale
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_19[0]", mISCresponse.Box_12_Amount.ToString());   //12 Section 409A deferrals
            if (mISCresponse.FATCA_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].TagCorrectingSubform[0].c2_3[0]", "0");   //13 FATCA filing requirement
            }
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].Box14_ReadOrder[0].f2_20[0]", mISCresponse.Box_14_Amount.ToString());   //12 Section 409A deferrals);   //14 Excess golden parachute payments
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_21[0]", mISCresponse.Box_15_Amount.ToString());   //15 Nonqualified deferred compensation
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].Box16_ReadOrder[0].f2_22[0]", mISCresponse.Box_16_Amount.ToString());   //16 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].Box16_ReadOrder[0].f2_23[0]", mISCresponse.Box_16_Amount.ToString());   //16 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].Box17_ReadOrder[0].f2_24[0]", mISCresponse.Box_17_ID_Number);   //17 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].Box17_ReadOrder[0].f2_25[0]", mISCresponse.Box_17_State);   //17 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].f2_26[0]", "StateIncome1");   //18 State income
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].f2_27[0]", "StateIncome2");   //18 State income
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].Copy2Header[0].CalendarYear[0].f2_1[0]", "23");   //CalYear
            if (mISCresponse.Corrected == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].Copy2[0].Copy2Header[0].c2_1[0]", "0");   //CORRECTED
            }
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].LeftColumn[0].f2_2[0]", string.Concat(instResponse.InstitutionName, ", ", instResponse.Madd1, ", ", instResponse.Madd2, ", ", instResponse.Mcity, ", ", instResponse.Mstate, instResponse.Mprovince, ", ", instResponse.Mcountry, ", ", instResponse.Mzip, ", ", instResponse.Phone));   //PAYER’S name, street address, city or town, state or province, country, ZIP or foreign postal code, and telephone no
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].LeftColumn[0].f2_3[0]", instResponse.Ftin);   //Payer TIN
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].LeftColumn[0].f2_4[0]", mISCresponse.Rcp_TIN);   //Recepients TIN
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].LeftColumn[0].f2_5[0]", mISCresponse.First_Name + " " + mISCresponse.Name_Line_2);   //RECIPIENT’S name
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].LeftColumn[0].f2_6[0]", mISCresponse.Address_Deliv_Street + " " + mISCresponse.Address_Apt_Suite);   //Street address (including apt. no.)
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].LeftColumn[0].f2_7[0]", mISCresponse.City + " " + mISCresponse.State + " " + mISCresponse.Country + " " + mISCresponse.Zip);   //City or town, state or province, country, and ZIP or foreign postal code
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].LeftColumn[0].f2_8[0]", mISCresponse.Rcp_Account);   //Account number (see instructions)
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_9[0]", mISCresponse.Box_1_Amount.ToString());   //1 Rents
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_10[0]", mISCresponse.Box_2_Amount.ToString());   //2 Royalties
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_11[0]", mISCresponse.Box_3_Amount.ToString());   //3 Other income
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_12[0]", mISCresponse.Box_4_Amount.ToString());   //4 Federal income tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_13[0]", mISCresponse.Box_5_Amount.ToString());   //5 Fishing boat proceeds
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_14[0]", mISCresponse.Box_6_Amount.ToString());   //6 Medical and health care payments
            if (mISCresponse.Box_7_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].c2_4[0]", mISCresponse.Box_7_Checkbox);   //7 CheckBox Payer made direct sales totaling $5,000 or more of consumer products to recipient for resale
            }
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_15[0]", mISCresponse.Box_8_Amount.ToString());   //8 Substitute payments in lieu of dividends or interest
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_16[0]", mISCresponse.Box_9_Amount.ToString());   //9 Crop insurance proceeds
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_17[0]", mISCresponse.Box_10_Amount.ToString());   //10 Gross proceeds paid to an attorney
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_18[0]", mISCresponse.Box_11_Amount.ToString());   //11 Fish purchased for resale
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_19[0]", mISCresponse.Box_12_Amount.ToString());   //12 Section 409A deferrals
            if (mISCresponse.FATCA_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].TagCorrectingSubform[0].c2_3[0]", "0");   //13 FATCA filing requirement
            }
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].Box14_ReadOrder[0].f2_20[0]", mISCresponse.Box_14_Amount.ToString());   //12 Section 409A deferrals);   //14 Excess golden parachute payments
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_21[0]", mISCresponse.Box_15_Amount.ToString());   //15 Nonqualified deferred compensation
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].Box16_ReadOrder[0].f2_22[0]", mISCresponse.Box_16_Amount.ToString());   //16 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].Box16_ReadOrder[0].f2_23[0]", mISCresponse.Box_16_Amount.ToString());   //16 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].Box17_ReadOrder[0].f2_24[0]", mISCresponse.Box_17_ID_Number);   //17 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].Box17_ReadOrder[0].f2_25[0]", mISCresponse.Box_17_State);   //17 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].f2_26[0]", "StateIncome1");   //18 State income
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].f2_27[0]", "StateIncome2");   //18 State income
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].CopyCHeader[0].CalendarYear[0].f2_1[0]", "23");   //CalYear
            if (mISCresponse.Corrected == "1")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyC[0].CopyCHeader[0].c2_1[0]", "0");   //VOID
            }
            else
            {
                pdfFormFields.SetField("topmostSubform[0].CopyC[0].CopyCHeader[0].c2_1[1]", "0");   //CORRECTED
            }
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_2[0]", string.Concat(instResponse.InstitutionName, ", ", instResponse.Madd1, ", ", instResponse.Madd2, ", ", instResponse.Mcity, ", ", instResponse.Mstate, instResponse.Mprovince, ", ", instResponse.Mcountry, ", ", instResponse.Mzip, ", ", instResponse.Phone));   //PAYER’S name, street address, city or town, state or province, country, ZIP or foreign postal code, and telephone no
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_3[0]", instResponse.Ftin);   //Payer TIN
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_4[0]", mISCresponse.Rcp_TIN);   //Recepients TIN
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_5[0]", mISCresponse.First_Name + " " + mISCresponse.Name_Line_2);   //RECIPIENT’S name
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_6[0]", mISCresponse.Address_Deliv_Street + " " + mISCresponse.Address_Apt_Suite);   //Street address (including apt. no.)
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_7[0]", mISCresponse.City + " " + mISCresponse.State + " " + mISCresponse.Country + " " + mISCresponse.Zip);   //City or town, state or province, country, and ZIP or foreign postal code
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_8[0]", mISCresponse.Rcp_Account);   //Account number (see instructions)
            if (mISCresponse.Second_TIN_Notice == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyC[0].c2_2[0]", mISCresponse.Second_TIN_Notice);   //2nd TIN not.
            }
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_9[0]", mISCresponse.Box_1_Amount.ToString());   //1 Rents
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_10[0]", mISCresponse.Box_2_Amount.ToString());   //2 Royalties
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_11[0]", mISCresponse.Box_3_Amount.ToString());   //3 Other income
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_12[0]", mISCresponse.Box_4_Amount.ToString());   //4 Federal income tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_13[0]", mISCresponse.Box_5_Amount.ToString());   //5 Fishing boat proceeds
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_14[0]", mISCresponse.Box_6_Amount.ToString());   //6 Medical and health care payments
            if (mISCresponse.Box_7_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].c2_4[0]", mISCresponse.Box_7_Checkbox);   //7 CheckBox Payer made direct sales totaling $5,000 or more of consumer products to recipient for resale
            }
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_15[0]", mISCresponse.Box_8_Amount.ToString());   //8 Substitute payments in lieu of dividends or interest
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_16[0]", mISCresponse.Box_9_Amount.ToString());   //9 Crop insurance proceeds
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_17[0]", mISCresponse.Box_10_Amount.ToString());   //10 Gross proceeds paid to an attorney
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_18[0]", mISCresponse.Box_11_Amount.ToString());   //11 Fish purchased for resale
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_19[0]", mISCresponse.Box_12_Amount.ToString());   //12 Section 409A deferrals
            if (mISCresponse.FATCA_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].TagCorrectingSubform[0].c2_3[0]", "0");   //13 FATCA filing requirement
            }
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].Box14_ReadOrder[0].f2_20[0]", mISCresponse.Box_14_Amount.ToString());   //12 Section 409A deferrals);   //14 Excess golden parachute payments
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_21[0]", mISCresponse.Box_15_Amount.ToString());   //15 Nonqualified deferred compensation
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].Box16_ReadOrder[0].f2_22[0]", mISCresponse.Box_16_Amount.ToString());   //16 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].Box16_ReadOrder[0].f2_23[0]", mISCresponse.Box_16_Amount.ToString());   //16 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].Box17_ReadOrder[0].f2_24[0]", mISCresponse.Box_17_ID_Number);   //17 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].Box17_ReadOrder[0].f2_25[0]", mISCresponse.Box_17_State);   //17 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].f2_26[0]", "StateIncome1");   //18 State income
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].f2_27[0]", "StateIncome2");   //18 State income
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
            var request = _evolvedtaxContext.Tbl1099_MISC.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.First_Name + " " + request.Name_Line_2?.Replace(": ", "");   
            newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099MISC, "_", Id);
            string FilenameNew = "/Form1099MISC/" + newFile1 + ".pdf";
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
            var request = _evolvedtaxContext.Tbl1099_MISC.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.First_Name + " " + request.Name_Line_2?.Replace(": ", "");

            newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099MISC, "_", request.Id, "_Page_", selectedPage);
            string FilenameNew = "/Form1099MISC/" + newFile1 + ".pdf";
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
            string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.Form1099MISCTemplateFileName);
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
                            compileFileName = "For Internal Revenue Service Center.pdf";
                            break;
                        case "3":
                            compileFileName = "For State Tax Department.pdf";
                            break;
                        case "4":
                            compileFileName = "For Recipient.pdf";
                            break;
                        case "6":
                            compileFileName = "To be filed with recipient’s state income tax return.pdf";
                            break;
                        case "7":
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

                string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.Form1099MISCTemplateFileName);
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

            var recordToDelete = _evolvedtaxContext.Tbl1099_MISC.First(p => p.Id == id);
            if (recordToDelete != null)
            {
                _evolvedtaxContext.Tbl1099_MISC.Remove(recordToDelete);
                await _evolvedtaxContext.SaveChangesAsync();
                return new MessageResponseModel { Status = true };
            }

            return new MessageResponseModel { Status = false };
        }

        public async Task<MessageResponseModel> KeepRecord(int id)
        {

            var recordToUpdate = _evolvedtaxContext.Tbl1099_MISC.First(p => p.Id == id);
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
