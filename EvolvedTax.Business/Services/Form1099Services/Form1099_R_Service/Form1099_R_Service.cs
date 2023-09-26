using EvolvedTax.Data.Models.Entities;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using EvolvedTax.Data.Models.Entities._1099;
using System.IO.Compression;
using EvolvedTax.Business.MailService;
using EvolvedTax.Data.Models.DTOs.Response.Form1099;
using NPOI.SS.Formula.Functions;

namespace EvolvedTax.Business.Services.Form1099Services
{
    public class Form1099_R_Service : IForm1099_R_Service
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly IMailService _mailService;
        readonly ITrailAudit1099Service _trailAudit1099Service;


        public Form1099_R_Service(EvolvedtaxContext evolvedtaxContext, IMailService mailService, ITrailAudit1099Service trailAudit1099Service)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _mailService = mailService;
            _trailAudit1099Service = trailAudit1099Service;
        }
        public async Task<MessageResponseModel> Upload1099_Data(IFormFile file, int entityId, int InstId, string UserId)
        {
            bool Status = false;
            var response = new List<Tbl1099_R>();
            var List = new List<Tbl1099_R>();
            using (var stream = file.OpenReadStream())
            {
                IWorkbook workbook = new XSSFWorkbook(stream);
                ISheet sheet = workbook.GetSheetAt(0); // Assuming the data is in the first sheet

                HashSet<string> uniqueEINNumber = new HashSet<string>();
                HashSet<string> uniqueEntityNames = new HashSet<string>();

                for (int row = 1; row <= sheet.LastRowNum; row++) // Starting from the second row
                {
                    IRow excelRow = sheet.GetRow(row);


                    string cell_value_15 = excelRow.GetCell(15)?.ToString();
                    string Box_2b_Checkbox1 = string.IsNullOrEmpty(cell_value_15) ? "0" : (cell_value_15.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    string cell_value_16 = excelRow.GetCell(16)?.ToString();
                    string Box_2b_Checkbox2 = string.IsNullOrEmpty(cell_value_16) ? "0" : (cell_value_16.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    string cell_value_22 = excelRow.GetCell(22)?.ToString();
                    string Box_7_Checkbox = string.IsNullOrEmpty(cell_value_22) ? "0" : (cell_value_22.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    string cell_value_29 = excelRow.GetCell(29)?.ToString();
                    string Box_12_FATCA_Check = string.IsNullOrEmpty(cell_value_29) ? "0" : (cell_value_29.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    string cell_value_42 = excelRow.GetCell(42)?.ToString();
                    string Corrected = string.IsNullOrEmpty(cell_value_42) ? "0" : (cell_value_42.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    var entity = new Tbl1099_R
                    {
                        Rcp_TIN = excelRow.GetCell(0)?.ToString(),
                        Last_Name_Company = excelRow.GetCell(1)?.ToString(),
                        First_Name = excelRow.GetCell(2)?.ToString(),
                        Name_Line_2 = excelRow.GetCell(3)?.ToString(),
                        Address_Type = excelRow.GetCell(4)?.ToString(),
                        Address_Deliv_Street = excelRow.GetCell(5)?.ToString(),
                        Address_Apt_Suite = excelRow.GetCell(6)?.ToString(),
                        City = excelRow.GetCell(7)?.ToString(),
                        State = excelRow.GetCell(8)?.ToString(),
                        Zip = excelRow.GetCell(9)?.ToString(),
                        Country = excelRow.GetCell(10)?.ToString(),
                        Rcp_Account = excelRow.GetCell(11)?.ToString(),
                        Rcp_Email = excelRow.GetCell(12)?.ToString(),
                        Box_1_Amount = TryConvertToDecimal(excelRow.GetCell(13)),
                        Box_2a_Amount = TryConvertToDecimal(excelRow.GetCell(14)),
                        Box_2b_Checkbox1= Box_2b_Checkbox1,
                        Box_2b_Checkbox2= Box_2b_Checkbox2,
                        Box_3_Amount = TryConvertToDecimal(excelRow.GetCell(17)),
                        Box_4_Amount = TryConvertToDecimal(excelRow.GetCell(18)),
                        Box_5_Amount = TryConvertToDecimal(excelRow.GetCell(19)),
                        Box_6_Amount = TryConvertToDecimal(excelRow.GetCell(20)),
                        Box_7_Code = excelRow.GetCell(21)?.ToString(),
                        Box_7_Checkbox= Box_7_Checkbox,
                        Box_8_Amount = TryConvertToDecimal(excelRow.GetCell(23)),
                        Box_8_Number = TryConvertToInt(excelRow.GetCell(24)),
                        Box_9a_Number = TryConvertToInt(excelRow.GetCell(25)),
                        Box_9b_Amount = TryConvertToDecimal(excelRow.GetCell(26)),
                        Box_10_Amount = TryConvertToDecimal(excelRow.GetCell(27)),
                        Box_11_Roth_Year = TryConvertToInt(excelRow.GetCell(28)),
                        Box_12_FATCA_Check= Box_12_FATCA_Check,
                        Box_13_DATE =  !string.IsNullOrWhiteSpace(excelRow.GetCell(30)?.ToString()) ? (DateTime?)Convert.ToDateTime(excelRow.GetCell(30)?.ToString()) : null,
                        Box_14_Amount = TryConvertToDecimal(excelRow.GetCell(31)),
                        Box_15_State = excelRow.GetCell(32)?.ToString(),
                        Box_15_IDNumber = TryConvertToInt(excelRow.GetCell(33)),
                        Box_16_Amount = TryConvertToDecimal(excelRow.GetCell(34)),
                        Box_17_Amount = TryConvertToDecimal(excelRow.GetCell(35)),
                        Box_18_Name = excelRow.GetCell(36)?.ToString(),
                        Box_19_Amount = TryConvertToDecimal(excelRow.GetCell(37)),
                        Form_Category = excelRow.GetCell(38)?.ToString(),
                        Form_Source = excelRow.GetCell(39)?.ToString(),
                        Batch_ID = TryConvertToInt(excelRow.GetCell(40)),
                        Tax_State = excelRow.GetCell(41)?.ToString(),
                        InstID = InstId,
                        EntityId = entityId,
                        //UserId = UserId,
                        Created_Date = DateTime.Now.Date,
                        Created_By = UserId,
                        Corrected = Corrected

                    };

                    string clientEmailEINNumber = entity.Rcp_TIN ?? string.Empty;

                    if (uniqueEINNumber.Contains(clientEmailEINNumber))
                    {
                        // This entity is a duplicate within the Excel sheet
                        Status = false;
                        return new MessageResponseModel
                        {
                            Status = Status,
                            Message = new
                            {
                                Title = "Duplication Record In Excel",
                                TagLine = "Record not uploaded due to duplication record in excel"
                            },
                            Param = "Client"
                        };
                    }
                    else
                    {
                        // Add the values to the HashSet to track duplicates
                        uniqueEINNumber.Add(clientEmailEINNumber);
                    }

                    // Check for duplicate records based on Rcp_TIN in the database
                    if (await _evolvedtaxContext.Tbl1099_R.AnyAsync(p => 
                        p.Rcp_TIN == entity.Rcp_TIN && p.EntityId == entity.EntityId
                         && p.Created_Date != null &&
            p.Created_Date.Value.Year == DateTime.Now.Year))
                        {
                        response.Add(entity);
                        Status = true;
                        entity.IsDuplicated = true;
                    }
                    else
                    {
                        entity.IsDuplicated = false;


                    }
                    List.Add(entity);
                }


                try
                {
                    await _evolvedtaxContext.Tbl1099_R.AddRangeAsync(List);
                    await _evolvedtaxContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.ToString());
                }

                return new MessageResponseModel { Status = Status, Message = response, Param = "Entity" };
            }

        }

        public string GeneratePdf(int Id, string TemplatefilePath, string SaveFolderPath)
        {
            return CreatePdf(Id, TemplatefilePath, SaveFolderPath, false);

        }

        public string CreatePdf(int Id, string TemplatefilePath, string SaveFolderPath, bool IsAll, string Page = "")
        {
            var request = _evolvedtaxContext.Tbl1099_R.FirstOrDefault(p => p.Id == Id);
            var requestInstitue = _evolvedtaxContext.InstituteMasters.FirstOrDefault(p => p.InstId == request.InstID);
            string templatefile = TemplatefilePath;
            string newFile1 = string.Empty;

            String ClientName = request.First_Name + " " + request.Name_Line_2?.Replace(": ", "");

            if (!String.IsNullOrEmpty(Page))
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.R1099Form, "_", request.Id, "_Page_", Page);
            }
            else
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.R1099Form, "_", request.Id);
            }


            string FilenameNew = "/1099R/" + newFile1 + ".pdf";
            string newFileName = newFile1 + ".pdf"; // Add ".pdf" extension to the file name

            string newFilePath = Path.Combine(SaveFolderPath, newFileName);

            PdfReader pdfReader = new PdfReader(templatefile);
            PdfReader.unethicalreading = true;
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFilePath, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;

            string PayData = string.Concat(requestInstitue.InstitutionName, "\r\n", requestInstitue.Madd1, "\r\n", requestInstitue.Madd2, "\r\n", requestInstitue.Mcity, ", ", requestInstitue.Mstate, requestInstitue.Mprovince, ", ", requestInstitue.Mcountry, ", ", requestInstitue.Mzip, ", ", requestInstitue.Phone);
            string RecipentCity = string.Concat(request.City, ", ", request.State, ", ", request.Zip, ", ", request.Country);
            String RecipentAddress = string.Concat(request.Address_Deliv_Street, ", ", request.Address_Apt_Suite);
            int currenDate = DateTime.Now.Year;
            string currentYear = Convert.ToString(currenDate % 100);

            #region PDF Columns

            #region Page 1



            if (request.Corrected.ToString() == "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyA.c1_1", "0");   //PageAVoid
            }
            if (request.Corrected.ToString() == "0")
            {
                pdfFormFields.SetField("efield1_topmostSubform.CopyA.c1_1", "0");   //PageACorrected
            }

            pdfFormFields.SetField("topmostSubform.CopyA.LeftCol_ReadOrder.f1_01", PayData);   //PayData
            pdfFormFields.SetField("topmostSubform.CopyA.LeftCol_ReadOrder.PayersTIN.f1_02", requestInstitue.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("topmostSubform.CopyA.LeftCol_ReadOrder.f1_03", request.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("topmostSubform.CopyA.LeftCol_ReadOrder.f1_04", request.First_Name + " " + request.Name_Line_2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("topmostSubform.CopyA.LeftCol_ReadOrder.f1_05", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("topmostSubform.CopyA.LeftCol_ReadOrder.f1_06", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("topmostSubform.CopyA.LeftCol_ReadOrder.f1_07", request.Rcp_Account);   //request.Rcp_Account
            pdfFormFields.SetField("topmostSubform.CopyA.f1_08", request.Box_1_Amount.HasValue ? request.Box_1_Amount.Value.ToString() : string.Empty);   //Box_1_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.f1_09", request.Box_2a_Amount.HasValue ? request.Box_2a_Amount.Value.ToString() : string.Empty);   //Box_2a_Amount
            if (request.Box_2b_Checkbox1 != "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyA.c1_2", "1");   //PageAFATCA

            }
            if (request.Box_2b_Checkbox2 != "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyA.c1_3", "1");   //PageAFATCA

            }

            pdfFormFields.SetField("topmostSubform.CopyA.Box3_ReadOrder.f1_10", request.Box_3_Amount.HasValue ? request.Box_3_Amount.Value.ToString() : string.Empty);   //Box_3_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.f1_11", request.Box_4_Amount.HasValue ? request.Box_4_Amount.Value.ToString() : string.Empty);   //Box_4_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.Box5_ReadOrder.f1_12", request.Box_5_Amount.HasValue ? request.Box_5_Amount.Value.ToString() : string.Empty);   //Box_5_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.f1_13", request.Box_6_Amount.HasValue ? request.Box_6_Amount.Value.ToString() : string.Empty);   //Box_6_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.Box7_ReadOrder.f1_14", request.Box_7_Code);   //Box_7_Code
 
            if (request.Box_7_Checkbox != "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyA.Box7_ReadOrder.c1_4", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("topmostSubform.CopyA.f1_15", request.Box_8_Amount.HasValue ? request.Box_8_Amount.Value.ToString() : string.Empty);   //Box_8_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.f1_16", request.Box_8_Number.HasValue ? request.Box_8_Number.Value.ToString() : string.Empty);   //Box_8_Number
            pdfFormFields.SetField("topmostSubform.CopyA.Box9a_ReadOrder.f1_17", request.Box_9a_Number.HasValue ? request.Box_9a_Number.Value.ToString() : string.Empty);   //Box_9a_Number
            pdfFormFields.SetField("topmostSubform.CopyA.f1_18", request.Box_9b_Amount.HasValue ? request.Box_9b_Amount.Value.ToString() : string.Empty);   //Box_9b_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.Box10_ReadOrder.f1_19", request.Box_10_Amount.HasValue ? request.Box_10_Amount.Value.ToString() : string.Empty);   //Box_10_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.f1_20", request.Box_11_Roth_Year.HasValue ? request.Box_11_Roth_Year.Value.ToString() : string.Empty);   //Box_11_Roth_Year
    
            if (request.Box_12_FATCA_Check != "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyA.Box12-13_ReadOrder.c1_5", "1");   //PageAFATCA

            }
            if (!string.IsNullOrEmpty(request.Box_13_DATE?.ToString()))
            {
                DateTime Box_13_DATE;
                if (DateTime.TryParse(request.Box_13_DATE?.ToString(), out Box_13_DATE))
                {
                    pdfFormFields.SetField("topmostSubform.CopyA.Box12-13_ReadOrder.f1_21", Box_13_DATE.ToString("MM/dd/yyyy"));
                }

            }
            pdfFormFields.SetField("topmostSubform.CopyA.Box14_ReadOrder.f1_22", request.Box_14_Amount.HasValue ? request.Box_14_Amount.Value.ToString() : string.Empty);   //Box_14_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.Box14_ReadOrder.f1_23", request.Box_14_Amount.HasValue ? request.Box_14_Amount.Value.ToString() : string.Empty);   //Box_14_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.Box15_ReadOrder.f1_24", request.Box_15_State);   //Box_15_State
            pdfFormFields.SetField("topmostSubform.CopyA.Box15_ReadOrder.f1_25", request.Box_15_IDNumber.HasValue ? request.Box_15_IDNumber.Value.ToString() : string.Empty);   //Box_15_IDNumber
            pdfFormFields.SetField("topmostSubform.CopyA.f1_26", request.Box_16_Amount.HasValue ? request.Box_16_Amount.Value.ToString() : string.Empty);   //Box_16_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.f1_27", request.Box_16_Amount.HasValue ? request.Box_16_Amount.Value.ToString() : string.Empty);   //Box_16_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.Box17_ReadOrder.f1_28", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty);   //Box_17_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.Box17_ReadOrder.f1_29", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty);   //Box_17_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.Box18_ReadOrder.f1_30", request.Box_18_Name);   //Box_18_Name
            pdfFormFields.SetField("topmostSubform.CopyA.Box18_ReadOrder.f1_31", request.Box_18_Name);   //Box_18_Name
            pdfFormFields.SetField("topmostSubform.CopyA.f1_32", request.Box_19_Amount.HasValue ? request.Box_19_Amount.Value.ToString() : string.Empty);   //Box_19_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.f1_33", request.Box_19_Amount.HasValue ? request.Box_19_Amount.Value.ToString() : string.Empty);   //Box_19_Amount
            #endregion

            #region Page 2

            if (request.Corrected.ToString() == "1")
            {
                pdfFormFields.SetField("topmostSubform.Copy1.c2_1", "0");   //PageAVoid
            }
            if (request.Corrected.ToString() == "0")
            {
                pdfFormFields.SetField("efield40_topmostSubform.Copy1.c2_1", "0");   //PageACorrected
            }

            pdfFormFields.SetField("topmostSubform.Copy1.LeftCol_ReadOrder.f2_01", PayData);   //PayData
            pdfFormFields.SetField("topmostSubform.Copy1.LeftCol_ReadOrder.PayersTIN.f2_02", requestInstitue.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("topmostSubform.Copy1.LeftCol_ReadOrder.f2_03", request.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("topmostSubform.Copy1.LeftCol_ReadOrder.f2_04", request.First_Name + " " + request.Name_Line_2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("topmostSubform.Copy1.LeftCol_ReadOrder.f2_05", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("topmostSubform.Copy1.LeftCol_ReadOrder.f2_06", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("topmostSubform.Copy1.LeftCol_ReadOrder.f2_07", request.Rcp_Account);   //request.Rcp_Account
            pdfFormFields.SetField("topmostSubform.Copy1.f2_08", request.Box_1_Amount.HasValue ? request.Box_1_Amount.Value.ToString() : string.Empty);   //Box_1_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.f2_09", request.Box_2a_Amount.HasValue ? request.Box_2a_Amount.Value.ToString() : string.Empty);   //Box_2a_Amount
            if (request.Box_2b_Checkbox1 != "1")
            {
                pdfFormFields.SetField("topmostSubform.Copy1.c2_2", "1");   //PageAFATCA

            }
            if (request.Box_2b_Checkbox2 != "1")
            {
                pdfFormFields.SetField("topmostSubform.Copy1.c2_3", "1");   //PageAFATCA

            }

            pdfFormFields.SetField("topmostSubform.Copy1.Box3_ReadOrder.f2_10", request.Box_3_Amount.HasValue ? request.Box_3_Amount.Value.ToString() : string.Empty);   //Box_3_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.f2_11", request.Box_4_Amount.HasValue ? request.Box_4_Amount.Value.ToString() : string.Empty);   //Box_4_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.Box5_ReadOrder.f2_12", request.Box_5_Amount.HasValue ? request.Box_5_Amount.Value.ToString() : string.Empty);   //Box_5_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.f2_13", request.Box_6_Amount.HasValue ? request.Box_6_Amount.Value.ToString() : string.Empty);   //Box_6_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.Box7_ReadOrder.f2_14", request.Box_7_Code);   //Box_7_Code

            if (request.Box_7_Checkbox != "1")
            {
                pdfFormFields.SetField("topmostSubform.Copy1.Box7_ReadOrder.c2_4", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("topmostSubform.Copy1.f2_15", request.Box_8_Amount.HasValue ? request.Box_8_Amount.Value.ToString() : string.Empty);   //Box_8_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.f2_16", request.Box_8_Number.HasValue ? request.Box_8_Number.Value.ToString() : string.Empty);   //Box_8_Number
            pdfFormFields.SetField("topmostSubform.Copy1.Box9a_ReadOrder.f2_17", request.Box_9a_Number.HasValue ? request.Box_9a_Number.Value.ToString() : string.Empty);   //Box_9a_Number
            pdfFormFields.SetField("topmostSubform.Copy1.f2_18", request.Box_9b_Amount.HasValue ? request.Box_9b_Amount.Value.ToString() : string.Empty);   //Box_9b_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.Box10_ReadOrder.f2_19", request.Box_10_Amount.HasValue ? request.Box_10_Amount.Value.ToString() : string.Empty);   //Box_10_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.f2_20", request.Box_11_Roth_Year.HasValue ? request.Box_11_Roth_Year.Value.ToString() : string.Empty);   //Box_11_Roth_Year

            if (request.Box_12_FATCA_Check != "1")
            {
                pdfFormFields.SetField("topmostSubform.Copy1.Box12-13_ReadOrder.c2_5", "1");   //PageAFATCA

            }
            if (!string.IsNullOrEmpty(request.Box_13_DATE?.ToString()))
            {
                DateTime Box_13_DATE;
                if (DateTime.TryParse(request.Box_13_DATE?.ToString(), out Box_13_DATE))
                {
                    pdfFormFields.SetField("topmostSubform.Copy1.Box12-13_ReadOrder.f2_21", Box_13_DATE.ToString("MM/dd/yyyy"));
                }

            }
            pdfFormFields.SetField("topmostSubform.Copy1.Box14_ReadOrder.f2_22", request.Box_14_Amount.HasValue ? request.Box_14_Amount.Value.ToString() : string.Empty);   //Box_14_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.Box14_ReadOrder.f2_23", request.Box_14_Amount.HasValue ? request.Box_14_Amount.Value.ToString() : string.Empty);   //Box_14_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.Box15_ReadOrder.f2_24", request.Box_15_State);   //Box_15_State
            pdfFormFields.SetField("topmostSubform.Copy1.Box15_ReadOrder.f2_25", request.Box_15_IDNumber.HasValue ? request.Box_15_IDNumber.Value.ToString() : string.Empty);   //Box_15_IDNumber
            pdfFormFields.SetField("topmostSubform.Copy1.f2_26", request.Box_16_Amount.HasValue ? request.Box_16_Amount.Value.ToString() : string.Empty);   //Box_16_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.f2_27", request.Box_16_Amount.HasValue ? request.Box_16_Amount.Value.ToString() : string.Empty);   //Box_16_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.Box17_ReadOrder.f2_28", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty);   //Box_17_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.Box17_ReadOrder.f2_29", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty);   //Box_17_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.Box18_ReadOrder.f2_30", request.Box_18_Name);   //Box_18_Name
            pdfFormFields.SetField("topmostSubform.Copy1.Box18_ReadOrder.f2_31", request.Box_18_Name);   //Box_18_Name
            pdfFormFields.SetField("topmostSubform.Copy1.f2_32", request.Box_19_Amount.HasValue ? request.Box_19_Amount.Value.ToString() : string.Empty);   //Box_19_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.f2_33", request.Box_19_Amount.HasValue ? request.Box_19_Amount.Value.ToString() : string.Empty);

            #endregion

            #region Page 3

            if (request.Corrected != "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyB.c2_1", "1");   //PageAFATCA

            }

            pdfFormFields.SetField("topmostSubform.CopyB.LeftCol_ReadOrder.f2_01", PayData);   //PayData
            pdfFormFields.SetField("topmostSubform.CopyB.LeftCol_ReadOrder.PayersTIN.f2_02", requestInstitue.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("topmostSubform.CopyB.LeftCol_ReadOrder.f2_03", request.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("topmostSubform.CopyB.LeftCol_ReadOrder.f2_04", request.First_Name + " " + request.Name_Line_2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("topmostSubform.CopyB.LeftCol_ReadOrder.f2_05", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("topmostSubform.CopyB.LeftCol_ReadOrder.f2_06", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("topmostSubform.CopyB.LeftCol_ReadOrder.f2_07", request.Rcp_Account);   //request.Rcp_Account
            pdfFormFields.SetField("topmostSubform.CopyB.f2_08", request.Box_1_Amount.HasValue ? request.Box_1_Amount.Value.ToString() : string.Empty);   //Box_1_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.f2_09", request.Box_2a_Amount.HasValue ? request.Box_2a_Amount.Value.ToString() : string.Empty);   //Box_2a_Amount
            if (request.Box_2b_Checkbox1 != "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyB.c2_2", "1");   //PageAFATCA

            }
            if (request.Box_2b_Checkbox2 != "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyB.c2_3", "1");   //PageAFATCA

            }

            pdfFormFields.SetField("topmostSubform.CopyB.Box3_ReadOrder.f2_10", request.Box_3_Amount.HasValue ? request.Box_3_Amount.Value.ToString() : string.Empty);   //Box_3_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.f2_11", request.Box_4_Amount.HasValue ? request.Box_4_Amount.Value.ToString() : string.Empty);   //Box_4_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.Box5_ReadOrder.f2_12", request.Box_5_Amount.HasValue ? request.Box_5_Amount.Value.ToString() : string.Empty);   //Box_5_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.f2_13", request.Box_6_Amount.HasValue ? request.Box_6_Amount.Value.ToString() : string.Empty);   //Box_6_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.Box7_ReadOrder.f2_14", request.Box_7_Code);   //Box_7_Code

            if (request.Box_7_Checkbox != "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyB.Box7_ReadOrder.c2_4", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("topmostSubform.CopyB.f2_15", request.Box_8_Amount.HasValue ? request.Box_8_Amount.Value.ToString() : string.Empty);   //Box_8_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.f2_16", request.Box_8_Number.HasValue ? request.Box_8_Number.Value.ToString() : string.Empty);   //Box_8_Number
            pdfFormFields.SetField("topmostSubform.CopyB.Box9a_ReadOrder.f2_17", request.Box_9a_Number.HasValue ? request.Box_9a_Number.Value.ToString() : string.Empty);   //Box_9a_Number
            pdfFormFields.SetField("topmostSubform.CopyB.f2_18", request.Box_9b_Amount.HasValue ? request.Box_9b_Amount.Value.ToString() : string.Empty);   //Box_9b_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.Box10_ReadOrder.f2_19", request.Box_10_Amount.HasValue ? request.Box_10_Amount.Value.ToString() : string.Empty);   //Box_10_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.f2_20", request.Box_11_Roth_Year.HasValue ? request.Box_11_Roth_Year.Value.ToString() : string.Empty);   //Box_11_Roth_Year

            if (request.Box_12_FATCA_Check != "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyB.Box12-13_ReadOrder.c2_5", "1");   //PageAFATCA

            }
            if (!string.IsNullOrEmpty(request.Box_13_DATE?.ToString()))
            {
                DateTime Box_13_DATE;
                if (DateTime.TryParse(request.Box_13_DATE?.ToString(), out Box_13_DATE))
                {
                    pdfFormFields.SetField("topmostSubform.CopyB.Box12-13_ReadOrder.f2_21", Box_13_DATE.ToString("MM/dd/yyyy"));
                }

            }
            pdfFormFields.SetField("topmostSubform.CopyB.Box14_ReadOrder.f2_22", request.Box_14_Amount.HasValue ? request.Box_14_Amount.Value.ToString() : string.Empty);   //Box_14_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.Box14_ReadOrder.f2_23", request.Box_14_Amount.HasValue ? request.Box_14_Amount.Value.ToString() : string.Empty);   //Box_14_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.Box15_ReadOrder.f2_24", request.Box_15_State);   //Box_15_State
            pdfFormFields.SetField("topmostSubform.CopyB.Box15_ReadOrder.f2_25", request.Box_15_IDNumber.HasValue ? request.Box_15_IDNumber.Value.ToString() : string.Empty);   //Box_15_IDNumber
            pdfFormFields.SetField("topmostSubform.CopyB.f2_26", request.Box_16_Amount.HasValue ? request.Box_16_Amount.Value.ToString() : string.Empty);   //Box_16_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.f2_27", request.Box_16_Amount.HasValue ? request.Box_16_Amount.Value.ToString() : string.Empty);   //Box_16_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.Box17_ReadOrder.f2_28", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty);   //Box_17_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.Box17_ReadOrder.f2_29", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty);   //Box_17_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.Box18_ReadOrder.f2_30", request.Box_18_Name);   //Box_18_Name
            pdfFormFields.SetField("topmostSubform.CopyB.Box18_ReadOrder.f2_31", request.Box_18_Name);   //Box_18_Name
            pdfFormFields.SetField("topmostSubform.CopyB.f2_32", request.Box_19_Amount.HasValue ? request.Box_19_Amount.Value.ToString() : string.Empty);   //Box_19_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.f2_33", request.Box_19_Amount.HasValue ? request.Box_19_Amount.Value.ToString() : string.Empty);

            #endregion

            #region Page 4

            if (request.Corrected != "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyC.c2_1", "1");   //PageAFATCA

            }

            pdfFormFields.SetField("topmostSubform.CopyC.LeftCol_ReadOrder.f2_01", PayData);   //PayData
            pdfFormFields.SetField("topmostSubform.CopyC.LeftCol_ReadOrder.PayersTIN.f2_02", requestInstitue.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("topmostSubform.CopyC.LeftCol_ReadOrder.f2_03", request.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("topmostSubform.CopyC.LeftCol_ReadOrder.f2_04", request.First_Name + " " + request.Name_Line_2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("topmostSubform.CopyC.LeftCol_ReadOrder.f2_05", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("topmostSubform.CopyC.LeftCol_ReadOrder.f2_06", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("topmostSubform.CopyC.LeftCol_ReadOrder.f2_07", request.Rcp_Account);   //request.Rcp_Account
            pdfFormFields.SetField("topmostSubform.CopyC.f2_08", request.Box_1_Amount.HasValue ? request.Box_1_Amount.Value.ToString() : string.Empty);   //Box_1_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.f2_09", request.Box_2a_Amount.HasValue ? request.Box_2a_Amount.Value.ToString() : string.Empty);   //Box_2a_Amount
            if (request.Box_2b_Checkbox1 != "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyC.c2_2", "1");   //PageAFATCA

            }
            if (request.Box_2b_Checkbox2 != "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyC.c2_3", "1");   //PageAFATCA

            }

            pdfFormFields.SetField("topmostSubform.CopyC.Box3_ReadOrder.f2_10", request.Box_3_Amount.HasValue ? request.Box_3_Amount.Value.ToString() : string.Empty);   //Box_3_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.f2_11", request.Box_4_Amount.HasValue ? request.Box_4_Amount.Value.ToString() : string.Empty);   //Box_4_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.Box5_ReadOrder.f2_12", request.Box_5_Amount.HasValue ? request.Box_5_Amount.Value.ToString() : string.Empty);   //Box_5_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.f2_13", request.Box_6_Amount.HasValue ? request.Box_6_Amount.Value.ToString() : string.Empty);   //Box_6_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.Box7_ReadOrder.f2_14", request.Box_7_Code);   //Box_7_Code

            if (request.Box_7_Checkbox != "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyC.Box7_ReadOrder.c2_4", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("topmostSubform.CopyC.f2_15", request.Box_8_Amount.HasValue ? request.Box_8_Amount.Value.ToString() : string.Empty);   //Box_8_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.f2_16", request.Box_8_Number.HasValue ? request.Box_8_Number.Value.ToString() : string.Empty);   //Box_8_Number
            pdfFormFields.SetField("topmostSubform.CopyC.Box9a_ReadOrder.f2_17", request.Box_9a_Number.HasValue ? request.Box_9a_Number.Value.ToString() : string.Empty);   //Box_9a_Number
            pdfFormFields.SetField("topmostSubform.CopyC.f2_18", request.Box_9b_Amount.HasValue ? request.Box_9b_Amount.Value.ToString() : string.Empty);   //Box_9b_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.Box10_ReadOrder.f2_19", request.Box_10_Amount.HasValue ? request.Box_10_Amount.Value.ToString() : string.Empty);   //Box_10_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.f2_20", request.Box_11_Roth_Year.HasValue ? request.Box_11_Roth_Year.Value.ToString() : string.Empty);   //Box_11_Roth_Year

            if (request.Box_12_FATCA_Check != "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyC.Box12-13_ReadOrder.c2_5", "1");   //PageAFATCA

            }
            if (!string.IsNullOrEmpty(request.Box_13_DATE?.ToString()))
            {
                DateTime Box_13_DATE;
                if (DateTime.TryParse(request.Box_13_DATE?.ToString(), out Box_13_DATE))
                {
                    pdfFormFields.SetField("topmostSubform.CopyC.Box12-13_ReadOrder.f2_21", Box_13_DATE.ToString("MM/dd/yyyy"));
                }

            }
            pdfFormFields.SetField("topmostSubform.CopyC.Box14_ReadOrder.f2_22", request.Box_14_Amount.HasValue ? request.Box_14_Amount.Value.ToString() : string.Empty);   //Box_14_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.Box14_ReadOrder.f2_23", request.Box_14_Amount.HasValue ? request.Box_14_Amount.Value.ToString() : string.Empty);   //Box_14_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.Box15_ReadOrder.f2_24", request.Box_15_State);   //Box_15_State
            pdfFormFields.SetField("topmostSubform.CopyC.Box15_ReadOrder.f2_25", request.Box_15_IDNumber.HasValue ? request.Box_15_IDNumber.Value.ToString() : string.Empty);   //Box_15_IDNumber
            pdfFormFields.SetField("topmostSubform.CopyC.f2_26", request.Box_16_Amount.HasValue ? request.Box_16_Amount.Value.ToString() : string.Empty);   //Box_16_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.f2_27", request.Box_16_Amount.HasValue ? request.Box_16_Amount.Value.ToString() : string.Empty);   //Box_16_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.Box17_ReadOrder.f2_28", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty);   //Box_17_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.Box17_ReadOrder.f2_29", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty);   //Box_17_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.Box18_ReadOrder.f2_30", request.Box_18_Name);   //Box_18_Name
            pdfFormFields.SetField("topmostSubform.CopyC.Box18_ReadOrder.f2_31", request.Box_18_Name);   //Box_18_Name
            pdfFormFields.SetField("topmostSubform.CopyC.f2_32", request.Box_19_Amount.HasValue ? request.Box_19_Amount.Value.ToString() : string.Empty);   //Box_19_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.f2_33", request.Box_19_Amount.HasValue ? request.Box_19_Amount.Value.ToString() : string.Empty);

            #endregion

            #region Page 5
            if (request.Corrected != "1")
            {
                pdfFormFields.SetField("topmostSubform.Copy2.c2_1", "1");   //PageAFATCA

            }

            pdfFormFields.SetField("topmostSubform.Copy2.LeftCol_ReadOrder.f2_01", PayData);   //PayData
            pdfFormFields.SetField("topmostSubform.Copy2.LeftCol_ReadOrder.PayersTIN.f2_02", requestInstitue.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("topmostSubform.Copy2.LeftCol_ReadOrder.f2_03", request.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("topmostSubform.Copy2.LeftCol_ReadOrder.f2_04", request.First_Name + " " + request.Name_Line_2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("topmostSubform.Copy2.LeftCol_ReadOrder.f2_05", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("topmostSubform.Copy2.LeftCol_ReadOrder.f2_06", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("topmostSubform.Copy2.LeftCol_ReadOrder.f2_07", request.Rcp_Account);   //request.Rcp_Account
            pdfFormFields.SetField("topmostSubform.Copy2.f2_08", request.Box_1_Amount.HasValue ? request.Box_1_Amount.Value.ToString() : string.Empty);   //Box_1_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.f2_09", request.Box_2a_Amount.HasValue ? request.Box_2a_Amount.Value.ToString() : string.Empty);   //Box_2a_Amount
            if (request.Box_2b_Checkbox1 != "1")
            {
                pdfFormFields.SetField("topmostSubform.Copy2.c2_2", "1");   //PageAFATCA

            }
            if (request.Box_2b_Checkbox2 != "1")
            {
                pdfFormFields.SetField("topmostSubform.Copy2.c2_3", "1");   //PageAFATCA

            }

            pdfFormFields.SetField("topmostSubform.Copy2.Box3_ReadOrder.f2_10", request.Box_3_Amount.HasValue ? request.Box_3_Amount.Value.ToString() : string.Empty);   //Box_3_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.f2_11", request.Box_4_Amount.HasValue ? request.Box_4_Amount.Value.ToString() : string.Empty);   //Box_4_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.Box5_ReadOrder.f2_12", request.Box_5_Amount.HasValue ? request.Box_5_Amount.Value.ToString() : string.Empty);   //Box_5_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.f2_13", request.Box_6_Amount.HasValue ? request.Box_6_Amount.Value.ToString() : string.Empty);   //Box_6_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.Box7_ReadOrder.f2_14", request.Box_7_Code);   //Box_7_Code

            if (request.Box_7_Checkbox != "1")
            {
                pdfFormFields.SetField("topmostSubform.Copy2.Box7_ReadOrder.c2_4", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("topmostSubform.Copy2.f2_15", request.Box_8_Amount.HasValue ? request.Box_8_Amount.Value.ToString() : string.Empty);   //Box_8_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.f2_16", request.Box_8_Number.HasValue ? request.Box_8_Number.Value.ToString() : string.Empty);   //Box_8_Number
            pdfFormFields.SetField("topmostSubform.Copy2.Box9a_ReadOrder.f2_17", request.Box_9a_Number.HasValue ? request.Box_9a_Number.Value.ToString() : string.Empty);   //Box_9a_Number
            pdfFormFields.SetField("topmostSubform.Copy2.f2_18", request.Box_9b_Amount.HasValue ? request.Box_9b_Amount.Value.ToString() : string.Empty);   //Box_9b_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.Box10_ReadOrder.f2_19", request.Box_10_Amount.HasValue ? request.Box_10_Amount.Value.ToString() : string.Empty);   //Box_10_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.f2_20", request.Box_11_Roth_Year.HasValue ? request.Box_11_Roth_Year.Value.ToString() : string.Empty);   //Box_11_Roth_Year

            if (request.Box_12_FATCA_Check != "1")
            {
                pdfFormFields.SetField("topmostSubform.Copy2.Box12-13_ReadOrder.c2_5", "1");   //PageAFATCA

            }
            if (!string.IsNullOrEmpty(request.Box_13_DATE?.ToString()))
            {
                DateTime Box_13_DATE;
                if (DateTime.TryParse(request.Box_13_DATE?.ToString(), out Box_13_DATE))
                {
                    pdfFormFields.SetField("topmostSubform.Copy2.Box12-13_ReadOrder.f2_21", Box_13_DATE.ToString("MM/dd/yyyy"));
                }

            }
            pdfFormFields.SetField("topmostSubform.Copy2.Box14_ReadOrder.f2_22", request.Box_14_Amount.HasValue ? request.Box_14_Amount.Value.ToString() : string.Empty);   //Box_14_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.Box14_ReadOrder.f2_23", request.Box_14_Amount.HasValue ? request.Box_14_Amount.Value.ToString() : string.Empty);   //Box_14_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.Box15_ReadOrder.f2_24", request.Box_15_State);   //Box_15_State
            pdfFormFields.SetField("topmostSubform.Copy2.Box15_ReadOrder.f2_25", request.Box_15_IDNumber.HasValue ? request.Box_15_IDNumber.Value.ToString() : string.Empty);   //Box_15_IDNumber
            pdfFormFields.SetField("topmostSubform.Copy2.f2_26", request.Box_16_Amount.HasValue ? request.Box_16_Amount.Value.ToString() : string.Empty);   //Box_16_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.f2_27", request.Box_16_Amount.HasValue ? request.Box_16_Amount.Value.ToString() : string.Empty);   //Box_16_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.Box17_ReadOrder.f2_28", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty);   //Box_17_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.Box17_ReadOrder.f2_29", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty);   //Box_17_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.Box18_ReadOrder.f2_30", request.Box_18_Name);   //Box_18_Name
            pdfFormFields.SetField("topmostSubform.Copy2.Box18_ReadOrder.f2_31", request.Box_18_Name);   //Box_18_Name
            pdfFormFields.SetField("topmostSubform.Copy2.f2_32", request.Box_19_Amount.HasValue ? request.Box_19_Amount.Value.ToString() : string.Empty);   //Box_19_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.f2_33", request.Box_19_Amount.HasValue ? request.Box_19_Amount.Value.ToString() : string.Empty);
            #endregion

            #region Page 6
            if (request.Corrected.ToString() == "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyD.c2_1", "0");   //PageAVoid
            }
            if (request.Corrected.ToString() == "0")
            {
                pdfFormFields.SetField("efield193_topmostSubform.CopyD.c2_1", "0");   //PageACorrected
            }

            pdfFormFields.SetField("topmostSubform.CopyD.LeftCol_ReadOrder.f2_01", PayData);   //PayData
            pdfFormFields.SetField("topmostSubform.CopyD.LeftCol_ReadOrder.PayersTIN.f2_02", requestInstitue.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("topmostSubform.CopyD.LeftCol_ReadOrder.f2_03", request.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("topmostSubform.CopyD.LeftCol_ReadOrder.f2_04", request.First_Name + " " + request.Name_Line_2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("topmostSubform.CopyD.LeftCol_ReadOrder.f2_05", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("topmostSubform.CopyD.LeftCol_ReadOrder.f2_06", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("topmostSubform.CopyD.LeftCol_ReadOrder.f2_07", request.Rcp_Account);   //request.Rcp_Account
            pdfFormFields.SetField("topmostSubform.CopyD.f2_08", request.Box_1_Amount.HasValue ? request.Box_1_Amount.Value.ToString() : string.Empty);   //Box_1_Amount
            pdfFormFields.SetField("topmostSubform.CopyD.f2_09", request.Box_2a_Amount.HasValue ? request.Box_2a_Amount.Value.ToString() : string.Empty);   //Box_2a_Amount
            if (request.Box_2b_Checkbox1 != "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyD.c2_2", "1");   //PageAFATCA

            }
            if (request.Box_2b_Checkbox2 != "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyD.c2_3", "1");   //PageAFATCA

            }

            pdfFormFields.SetField("topmostSubform.CopyD.Box3_ReadOrder.f2_10", request.Box_3_Amount.HasValue ? request.Box_3_Amount.Value.ToString() : string.Empty);   //Box_3_Amount
            pdfFormFields.SetField("topmostSubform.CopyD.f2_11", request.Box_4_Amount.HasValue ? request.Box_4_Amount.Value.ToString() : string.Empty);   //Box_4_Amount
            pdfFormFields.SetField("topmostSubform.CopyD.Box5_ReadOrder.f2_12", request.Box_5_Amount.HasValue ? request.Box_5_Amount.Value.ToString() : string.Empty);   //Box_5_Amount
            pdfFormFields.SetField("topmostSubform.CopyD.f2_13", request.Box_6_Amount.HasValue ? request.Box_6_Amount.Value.ToString() : string.Empty);   //Box_6_Amount
            pdfFormFields.SetField("topmostSubform.CopyD.Box7_ReadOrder.f2_14", request.Box_7_Code);   //Box_7_Code

            if (request.Box_7_Checkbox != "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyD.Box7_ReadOrder.c2_4", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("topmostSubform.CopyD.Box7_ReadOrder.f2_15", request.Box_8_Amount.HasValue ? request.Box_8_Amount.Value.ToString() : string.Empty);   //Box_8_Amount
            pdfFormFields.SetField("topmostSubform.CopyD.f2_16", request.Box_8_Number.HasValue ? request.Box_8_Number.Value.ToString() : string.Empty);   //Box_8_Number
            pdfFormFields.SetField("topmostSubform.CopyD.Box9a_ReadOrder.f2_17", request.Box_9a_Number.HasValue ? request.Box_9a_Number.Value.ToString() : string.Empty);   //Box_9a_Number
            pdfFormFields.SetField("topmostSubform.CopyD.f2_18", request.Box_9b_Amount.HasValue ? request.Box_9b_Amount.Value.ToString() : string.Empty);   //Box_9b_Amount
            pdfFormFields.SetField("topmostSubform.CopyD.Box10_ReadOrder.f2_19", request.Box_10_Amount.HasValue ? request.Box_10_Amount.Value.ToString() : string.Empty);   //Box_10_Amount
            pdfFormFields.SetField("topmostSubform.CopyD.f2_20", request.Box_11_Roth_Year.HasValue ? request.Box_11_Roth_Year.Value.ToString() : string.Empty);   //Box_11_Roth_Year

            if (request.Box_12_FATCA_Check != "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyD.Box12-13_ReadOrder.c2_5", "1");   //PageAFATCA

            }
            if (!string.IsNullOrEmpty(request.Box_13_DATE?.ToString()))
            {
                DateTime Box_13_DATE;
                if (DateTime.TryParse(request.Box_13_DATE?.ToString(), out Box_13_DATE))
                {
                    pdfFormFields.SetField("topmostSubform.CopyD.Box12-13_ReadOrder.f2_21", Box_13_DATE.ToString("MM/dd/yyyy"));
                }

            }
            pdfFormFields.SetField("topmostSubform.CopyD.Box14_ReadOrder.f2_22", request.Box_14_Amount.HasValue ? request.Box_14_Amount.Value.ToString() : string.Empty);   //Box_14_Amount
            pdfFormFields.SetField("topmostSubform.CopyD.Box14_ReadOrder.f2_23", request.Box_14_Amount.HasValue ? request.Box_14_Amount.Value.ToString() : string.Empty);   //Box_14_Amount
            pdfFormFields.SetField("topmostSubform.CopyD.Box15_ReadOrder.f2_24", request.Box_15_State);   //Box_15_State
            pdfFormFields.SetField("topmostSubform.CopyD.Box15_ReadOrder.f2_25", request.Box_15_IDNumber.HasValue ? request.Box_15_IDNumber.Value.ToString() : string.Empty);   //Box_15_IDNumber
            pdfFormFields.SetField("topmostSubform.CopyD.f2_26", request.Box_16_Amount.HasValue ? request.Box_16_Amount.Value.ToString() : string.Empty);   //Box_16_Amount
            pdfFormFields.SetField("topmostSubform.CopyD.f2_27", request.Box_16_Amount.HasValue ? request.Box_16_Amount.Value.ToString() : string.Empty);   //Box_16_Amount
            pdfFormFields.SetField("topmostSubform.CopyD.Box17_ReadOrder.f2_28", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty);   //Box_17_Amount
            pdfFormFields.SetField("topmostSubform.CopyD.Box17_ReadOrder.f2_29", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty);   //Box_17_Amount
            pdfFormFields.SetField("topmostSubform.CopyD.Box18_ReadOrder.f2_30", request.Box_18_Name);   //Box_18_Name
            pdfFormFields.SetField("topmostSubform.CopyD.Box18_ReadOrder.f2_31", request.Box_18_Name);   //Box_18_Name
            pdfFormFields.SetField("topmostSubform.CopyD.f2_32", request.Box_19_Amount.HasValue ? request.Box_19_Amount.Value.ToString() : string.Empty);   //Box_19_Amount
            pdfFormFields.SetField("topmostSubform.CopyD.f2_33", request.Box_19_Amount.HasValue ? request.Box_19_Amount.Value.ToString() : string.Empty);
            #endregion

            /*
            #region Page 1

            pdfFormFields.SetField("topmostSubform.CopyA.CopyHeader.CalendarYear.f1_1", currentYear);   //23
            if (request.Corrected.ToString() == "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyA.CopyHeader.c1_1", "0");   //PageAVoid
            }
            if (request.Corrected.ToString() == "0")
            {
                pdfFormFields.SetField("efield2_topmostSubform.CopyA.CopyHeader.c1_1", "0");   //PageACorrected
            }
            pdfFormFields.SetField("topmostSubform.CopyA.LeftColumn.f1_2", PayData);   //PageAPayerNameAddress
            pdfFormFields.SetField("topmostSubform.CopyA.LeftColumn.f1_3", requestInstitue.Idnumber);
            pdfFormFields.SetField("topmostSubform.CopyA.LeftColumn.f1_4", request.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("topmostSubform.CopyA.LeftColumn.f1_5", request.First_Name + " " + request.Name_Line_2);
            pdfFormFields.SetField("topmostSubform.CopyA.LeftColumn.f1_6", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("topmostSubform.CopyA.LeftColumn.f1_7", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("topmostSubform.CopyA.LeftColumn.f1_8", request.Rcp_Account);   //request.Rcp_Account
            if (!string.IsNullOrEmpty(request.Box_1_Date?.ToString()))
            {
                DateTime Box_1_Date;
                if (DateTime.TryParse(request.Box_1_Date?.ToString(), out Box_1_Date))
                {
                    pdfFormFields.SetField("topmostSubform.CopyA.RightColumn.f1_9", Box_1_Date.ToString("MM/dd/yyyy"));
                }

            }
            pdfFormFields.SetField("topmostSubform.CopyA.RightColumn.f1_10", request.Box_2_Amount.HasValue ? request.Box_2_Amount.Value.ToString() : string.Empty);   //PageA1
            pdfFormFields.SetField("topmostSubform.CopyA.RightColumn.f1_11", request.Box_3_Number.HasValue ? request.Box_3_Number.Value.ToString() : string.Empty);   //PageA1
            pdfFormFields.SetField("topmostSubform.CopyA.RightColumn.f1_12", request.Box_4_Class);   //Box_6_Code
            pdfFormFields.SetField("topmostSubform.CopyA.RightColumn.f1_11", request.Box_4_Class);   //Box_6_Code
            pdfFormFields.SetField("topmostSubform.CopyA.RightColumn.f1_13", request.Box_5_All_Lines);



            #endregion

            #region Page 2

            pdfFormFields.SetField("topmostSubform.CopyB.CopyHeader.CalendarYear.f1_1", currentYear);   //23
            if (request.Corrected != "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyB.CopyHeader.c1_1", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.f1_2", PayData);   //PageAPayerNameAddress
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.f1_3", requestInstitue.Idnumber);
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.f1_4", request.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.f1_5", request.First_Name + " " + request.Name_Line_2);
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.f1_6", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.f1_7", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.f1_8", request.Rcp_Account);   //request.Rcp_Account
            if (!string.IsNullOrEmpty(request.Box_1_Date?.ToString()))
            {
                DateTime Box_1_Date;
                if (DateTime.TryParse(request.Box_1_Date?.ToString(), out Box_1_Date))
                {
                    pdfFormFields.SetField("topmostSubform.CopyB.RightColumn.f1_9", Box_1_Date.ToString("MM/dd/yyyy"));
                }

            }
            pdfFormFields.SetField("topmostSubform.CopyB.RightColumn.f1_10", request.Box_2_Amount.HasValue ? request.Box_2_Amount.Value.ToString() : string.Empty);   //PageA1
            pdfFormFields.SetField("topmostSubform.CopyB.RightColumn.f1_11", request.Box_3_Number.HasValue ? request.Box_3_Number.Value.ToString() : string.Empty);   //PageA1
            pdfFormFields.SetField("topmostSubform.CopyB.RightColumn.f1_12", request.Box_4_Class);   //Box_6_Code
            pdfFormFields.SetField("topmostSubform.CopyB.RightColumn.f1_11", request.Box_4_Class);   //Box_6_Code
            pdfFormFields.SetField("topmostSubform.CopyB.RightColumn.f1_13", request.Box_5_All_Lines);



            #endregion

            #region Page 3

            pdfFormFields.SetField("topmostSubform.CopyC.CopyHeader.CalendarYear.f1_1", currentYear);   //23
            if (request.Corrected.ToString() == "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyC.CopyHeader.c1_1", "0");   //PageAVoid
            }
            if (request.Corrected.ToString() == "0")
            {
                pdfFormFields.SetField("efield31_topmostSubform.CopyC.CopyHeader.c1_1", "0");   //PageACorrected
            }


            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.f1_2", PayData);   //PageAPayerNameAddress
            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.f1_3", requestInstitue.Idnumber);
            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.f1_4", request.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.f1_5", request.First_Name + " " + request.Name_Line_2);
            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.f1_6", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.f1_7", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.f1_8", request.Rcp_Account);   //request.Rcp_Account
            if (!string.IsNullOrEmpty(request.Box_1_Date?.ToString()))
            {
                DateTime Box_1_Date;
                if (DateTime.TryParse(request.Box_1_Date?.ToString(), out Box_1_Date))
                {
                    pdfFormFields.SetField("topmostSubform.CopyC.RightColumn.f1_9", Box_1_Date.ToString("MM/dd/yyyy"));
                }

            }
            pdfFormFields.SetField("topmostSubform.CopyC.RightColumn.f1_10", request.Box_2_Amount.HasValue ? request.Box_2_Amount.Value.ToString() : string.Empty);   //PageA1
            pdfFormFields.SetField("topmostSubform.CopyC.RightColumn.f1_11", request.Box_3_Number.HasValue ? request.Box_3_Number.Value.ToString() : string.Empty);   //PageA1
            pdfFormFields.SetField("topmostSubform.CopyC.RightColumn.f1_12", request.Box_4_Class);   //Box_6_Code
            pdfFormFields.SetField("topmostSubform.CopyC.RightColumn.f1_11", request.Box_4_Class);   //Box_6_Code
            pdfFormFields.SetField("topmostSubform.CopyC.RightColumn.f1_13", request.Box_5_All_Lines);



            #endregion
            */


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

        public string GenerateAndZipPdfs(List<int> ids, string SaveFolderPath, List<string> selectedPages, string RootPath)
        {
            var pdfPaths = new List<string>();

            foreach (var id in ids)
            {

                string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.R_1099_TemplateFileName);
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


        public string GeneratePdfForSpecificPage(int Id, string TemplatefilePath, string SaveFolderPath, List<string> selectedPages)
        {
            string newFile1 = string.Empty;
            var request = _evolvedtaxContext.Tbl1099_R.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.First_Name + " " + request.Name_Line_2?.Replace(": ", "");
            newFile1 = string.Concat(ClientName, "_", AppConstants.R1099Form, "_", Id);
            string FilenameNew = "/1099R/" + newFile1 + ".pdf";
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
            var request = _evolvedtaxContext.Tbl1099_R.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.First_Name + " " + request.Name_Line_2?.Replace(": ", "");    

            newFile1 = string.Concat(ClientName, "_", AppConstants.R1099Form, "_", request.Id, "_Page_", selectedPage);
            string FilenameNew = "/1099R/" + newFile1 + ".pdf";
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
            string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.R_1099_TemplateFileName);
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
                            compileFileName = "State, City, or Local Tax Department.pdf";
                            break;
                        case "4":
                            compileFileName = "Report this income on your federal tax return.pdf";
                            break;
                        case "6":
                            compileFileName = " For Recipient’s Records.pdf";
                            break;
                        case "8":
                            compileFileName = " File this copy with your state, city, or local income tax return, when required.pdf";
                            break;
                        case "10":
                            compileFileName = " For Payer.pdf";
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

        public async Task<MessageResponseModel> DeletePermeant(int id)
        {

            var recordToDelete = _evolvedtaxContext.Tbl1099_R.First(p => p.Id == id);
            if (recordToDelete != null)
            {
                _evolvedtaxContext.Tbl1099_R.Remove(recordToDelete);
                await _evolvedtaxContext.SaveChangesAsync();
                return new MessageResponseModel { Status = true };
            }

            return new MessageResponseModel { Status = false };
        }

        public async Task<MessageResponseModel> KeepRecord(int id)
        {

            var recordToUpdate = _evolvedtaxContext.Tbl1099_R.First(p => p.Id == id);
            if (recordToUpdate != null)
            {
                recordToUpdate.IsDuplicated = false;
                await _evolvedtaxContext.SaveChangesAsync();
                return new MessageResponseModel { Status = true, Message = "The record has been kept" };
            }

            return new MessageResponseModel { Status = false, Message = "Oops! something wrong" };
        }
        public async Task<bool> SendEmailToRecipients(int[] selectValues, string URL, string form)
        {
            var result = from ic in _evolvedtaxContext.Tbl1099_R
                         where selectValues.Contains(ic.Id) && !string.IsNullOrEmpty(ic.Rcp_Email)
                         select new Tbl1099_R
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
        public IEnumerable<Tbl1099_R> GetForm1099List()
        {
            return _evolvedtaxContext.Tbl1099_R.AsEnumerable();
        }


        private decimal? TryConvertToDecimal(ICell cell)
        {
            if (cell != null && !string.IsNullOrWhiteSpace(cell.ToString()))
            {
                if (decimal.TryParse(cell.ToString(), out decimal result))
                {
                    return result;
                }
            }
            return null;
        }
        private int? TryConvertToInt(ICell cell)
        {
            if (cell != null && !string.IsNullOrWhiteSpace(cell.ToString()))
            {
                if (int.TryParse(cell.ToString(), out int result))
                {
                    return result;
                }
            }
            return null;
        }

    }
}
