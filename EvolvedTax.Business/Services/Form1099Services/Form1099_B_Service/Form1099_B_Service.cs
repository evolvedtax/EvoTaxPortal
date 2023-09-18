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
using Microsoft.IdentityModel.Tokens;

namespace EvolvedTax.Business.Services.Form1099Services
{
    public class Form1099_B_Service : IForm1099_B_Service
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly IMailService _mailService;
        readonly ITrailAudit1099Service _trailAudit1099Service;


        public Form1099_B_Service(EvolvedtaxContext evolvedtaxContext, IMailService mailService, ITrailAudit1099Service trailAudit1099Service)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _mailService = mailService;
            _trailAudit1099Service = trailAudit1099Service;
        }
        public async Task<MessageResponseModel> Upload1099_Data(IFormFile file, int entityId, int InstId, string UserId)
        {
            bool Status = false;
            var response = new List<Tbl1099_B>();
            var List = new List<Tbl1099_B>();
            using (var stream = file.OpenReadStream())
            {
                IWorkbook workbook = new XSSFWorkbook(stream);
                ISheet sheet = workbook.GetSheetAt(0); // Assuming the data is in the first sheet

                HashSet<string> uniqueEINNumber = new HashSet<string>();
                HashSet<string> uniqueEntityNames = new HashSet<string>();

                for (int row = 1; row <= sheet.LastRowNum; row++) // Starting from the second row
                {
                    IRow excelRow = sheet.GetRow(row);

                    string cell_value_13 = excelRow.GetCell(13)?.ToString();
                    string Second_TIN_Notice = string.IsNullOrEmpty(cell_value_13) ? "0" : (cell_value_13.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    string cell_value_14 = excelRow.GetCell(14)?.ToString();
                    string FATCA_Checkbox = string.IsNullOrEmpty(cell_value_14) ? "0" : (cell_value_14.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    string cell_value_24 = excelRow.GetCell(24)?.ToString();
                    string Box_2_Checkbox_1 = string.IsNullOrEmpty(cell_value_24) ? "0" : (cell_value_24.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    string cell_value_25 = excelRow.GetCell(25)?.ToString();
                    string Box_2_Checkbox_2 = string.IsNullOrEmpty(cell_value_25) ? "0" : (cell_value_25.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    string cell_value_26 = excelRow.GetCell(26)?.ToString();
                    string Box_2_Checkbox_3 = string.IsNullOrEmpty(cell_value_26) ? "0" : (cell_value_26.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");


                    string cell_value_27 = excelRow.GetCell(27)?.ToString();
                    string Box_3_Checkbox_1 = string.IsNullOrEmpty(cell_value_27) ? "0" : (cell_value_27.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");


                    string cell_value_28 = excelRow.GetCell(28)?.ToString();
                    string Box_3_Checkbox_2 = string.IsNullOrEmpty(cell_value_28) ? "0" : (cell_value_28.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");


                    string cell_value_30 = excelRow.GetCell(30)?.ToString();
                    string Box_5_Checkbox = string.IsNullOrEmpty(cell_value_30) ? "0" : (cell_value_30.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    string cell_value_31 = excelRow.GetCell(31)?.ToString();
                    string Box_6_Checkbox_1 = string.IsNullOrEmpty(cell_value_31) ? "0" : (cell_value_31.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    string cell_value_32 = excelRow.GetCell(32)?.ToString();
                    string Box_6_Checkbox_2 = string.IsNullOrEmpty(cell_value_32) ? "0" : (cell_value_32.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    string cell_value_33 = excelRow.GetCell(33)?.ToString();
                    string Box_7_Checkbox = string.IsNullOrEmpty(cell_value_33) ? "0" : (cell_value_33.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    string cell_value_38 = excelRow.GetCell(38)?.ToString();
                    string Box_12_Checkbox = string.IsNullOrEmpty(cell_value_38) ? "0" : (cell_value_38.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    string cell_value_46 = excelRow.GetCell(46)?.ToString();
                    string Corrected = string.IsNullOrEmpty(cell_value_46) ? "0" : (cell_value_46.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                   
                    
                    var entity = new Tbl1099_B
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
                        Second_TIN_Notice = Second_TIN_Notice,
                        FATCA_Checkbox = FATCA_Checkbox,
                        CUSIP_No = excelRow.GetCell(15)?.ToString(),
                        Eight949_Code = excelRow.GetCell(16)?.ToString(),
                        Box_1a_Description = excelRow.GetCell(17)?.ToString(),
                        Box_1b_Date = !string.IsNullOrWhiteSpace(excelRow.GetCell(18)?.ToString()) ? (DateTime?)Convert.ToDateTime(excelRow.GetCell(18)?.ToString()) : null,
                        Box_1c_Date = !string.IsNullOrWhiteSpace(excelRow.GetCell(19)?.ToString()) ? (DateTime?)Convert.ToDateTime(excelRow.GetCell(19)?.ToString()) : null,
                        Box_1d_Amount = TryConvertToDecimal(excelRow.GetCell(20)),
                        Box_1e_Amount = TryConvertToDecimal(excelRow.GetCell(21)),
                        Box_1f_Amount = TryConvertToDecimal(excelRow.GetCell(22)),
                        Box_1g_Amount = TryConvertToDecimal(excelRow.GetCell(23)),
                        Box_2_Checkbox_1= Box_2_Checkbox_1,
                        Box_2_Checkbox_2= Box_2_Checkbox_2,
                        Box_2_Checkbox_3= Box_2_Checkbox_3,
                        Box_3_Checkbox_1= Box_3_Checkbox_1,
                        Box_3_Checkbox_2= Box_3_Checkbox_2,
                        Box_4_Amount = TryConvertToDecimal(excelRow.GetCell(29)),
                        Box_5_Checkbox= Box_5_Checkbox,
                        Box_6_Checkbox_1 = Box_6_Checkbox_1,
                        Box_6_Checkbox_2 = Box_6_Checkbox_2,
                        Box_7_Checkbox = Box_7_Checkbox,
                        Box_8_Amount = TryConvertToDecimal(excelRow.GetCell(34)),
                        Box_9_Amount = TryConvertToDecimal(excelRow.GetCell(35)),
                        Box_10_Amount = TryConvertToDecimal(excelRow.GetCell(36)),
                        Box_11_Amount = TryConvertToDecimal(excelRow.GetCell(37)),
                        Box_12_Checkbox = Box_12_Checkbox,
                        Box_13_Amount = TryConvertToDecimal(excelRow.GetCell(39)),
                        Box_14_State = excelRow.GetCell(40)?.ToString(),
                        Box_15_ID_Number = excelRow.GetCell(41)?.ToString(),
                        Box_16_Amount = TryConvertToDecimal(excelRow.GetCell(42)),
                        Form_Category = excelRow.GetCell(43)?.ToString(),
                        Form_Source = excelRow.GetCell(44)?.ToString(),
                        Tax_State = excelRow.GetCell(45)?.ToString(),
                        Corrected = Corrected,
                        InstID = InstId,
                        EntityId = entityId,
                        //UserId = UserId,
                        Created_Date = DateTime.Now.Date,
                        Created_By = UserId,
                      

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
                    if (await _evolvedtaxContext.Tbl1099_B.AnyAsync(p => 
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
                    await _evolvedtaxContext.Tbl1099_B.AddRangeAsync(List);
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
            var request = _evolvedtaxContext.Tbl1099_B.FirstOrDefault(p => p.Id == Id);
            var requestInstitue = _evolvedtaxContext.InstituteMasters.FirstOrDefault(p => p.InstId == request.InstID);
            string templatefile = TemplatefilePath;
            string newFile1 = string.Empty;

            String ClientName = request.First_Name + " " + request.Name_Line_2?.Replace(": ", "");

            if (!String.IsNullOrEmpty(Page))
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.B1099Form, "_", request.Id, "_Page_", Page);
            }
            else
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.B1099Form, "_", request.Id);
            }


            string FilenameNew = "/1099B/" + newFile1 + ".pdf";
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

            if (request.Corrected?.ToString() == "1")
            {
                pdfFormFields.SetField("AVoid", "0");   //PageAVoid
            }
            if (request.Corrected?.ToString() == "0")
            {
                pdfFormFields.SetField("ACorrected", "0");   //PageACorrected
            }
            pdfFormFields.SetField("APayerNameAddress", PayData);   //PageAPayerNameAddress
            pdfFormFields.SetField("A8949Code", request.Eight949_Code);   //Eight949_Code
            pdfFormFields.SetField("APropertyDescription", request.Box_1a_Description);   //Box_1a_Description
            if (!string.IsNullOrEmpty(request.Box_1b_Date?.ToString()))
            {
                DateTime box1bDate;
                if (DateTime.TryParse(request.Box_1b_Date?.ToString(), out box1bDate))
                {
                    pdfFormFields.SetField("AAcquiredDate", box1bDate.ToString("MM/dd/yyyy"));
                }
               
            }
            if (!string.IsNullOrEmpty(request.Box_1c_Date?.ToString()))
            {
                DateTime Box_1c_Date;
                if (DateTime.TryParse(request.Box_1c_Date?.ToString(), out Box_1c_Date))
                {
                    pdfFormFields.SetField("ADateSold", Box_1c_Date.ToString("MM/dd/yyyy"));
                }

            }
            pdfFormFields.SetField("APayerTIN", requestInstitue.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("ARecepientTIN", request.Rcp_TIN);   //request.Rcp_TIN
            pdfFormFields.SetField("ACashProceeds", request.Box_1d_Amount.HasValue ? request.Box_1d_Amount.Value.ToString() : string.Empty);   
            pdfFormFields.SetField("ACostBasis", request.Box_1e_Amount.HasValue ? request.Box_1e_Amount.Value.ToString() : string.Empty);   
            pdfFormFields.SetField("AAccruedDiscount", request.Box_1f_Amount.HasValue ? request.Box_1f_Amount.Value.ToString() : string.Empty);   
            pdfFormFields.SetField("AWashSaleLoss", request.Box_1g_Amount.HasValue ? request.Box_1g_Amount.Value.ToString() : string.Empty);
            pdfFormFields.SetField("ARecepientName", request.First_Name + " " + request.Name_Line_2);   //request.First_Name + " " + request.Name_Line2
            if (request.Box_2_Checkbox_1 != "1")
            {
                pdfFormFields.SetField("AShortTermGainLoss", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_2_Checkbox_2 != "1")
            {
                pdfFormFields.SetField("ALongTermGainLoss", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_3_Checkbox_1 != "1")
            {
                pdfFormFields.SetField("ACollectibles", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_2_Checkbox_3 != "1")
            {
                pdfFormFields.SetField("AOrdinary", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_3_Checkbox_2 != "1")
            {
                pdfFormFields.SetField("AQOF", "1");   //Box_2_Checkbox_1

            }
            pdfFormFields.SetField("ARecepientAddress", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("AFederalIncomeTax", request.Box_4_Amount.HasValue ? request.Box_4_Amount.Value.ToString() : string.Empty);   //PageA2
            if (request.Box_5_Checkbox != "1")
            {
                pdfFormFields.SetField("ANonCoveredSecurity", "1");   //Box_2_Checkbox_1

            }
            pdfFormFields.SetField("ARecepientCity", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("AAccountNumber", request.Rcp_Account);   //request.Rcp_Account
            pdfFormFields.SetField("ACUSIPNumber", request.CUSIP_No);
            pdfFormFields.SetField("AStateName1", request.Box_14_State);   //Box_14_State
            pdfFormFields.SetField("AStateName2", request.Box_14_State);   //Box_14_State
            if (request.Box_6_Checkbox_1 != "1")
            {
                pdfFormFields.SetField("AGrossProceed", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_6_Checkbox_2 != "1")
            {
                pdfFormFields.SetField("ANetProceed", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_7_Checkbox != "1")
            {
                pdfFormFields.SetField("ALossNotAllowed", "1");   //Box_2_Checkbox_1

            }
            pdfFormFields.SetField("A2ndTIN", request.Second_TIN_Notice);   //Second_TIN_Notice
            if (request.FATCA_Checkbox != "1")
            {
                pdfFormFields.SetField("AFATCAFilingrequirements", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("ASIN1", request.Box_15_ID_Number);   //Box_15_ID_Number
            pdfFormFields.SetField("AStateTaxwithheld1", request.Box_16_Amount.HasValue ? request.Box_16_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("ASIN2", request.Box_15_ID_Number);   //Box_15_ID_Number
            pdfFormFields.SetField("AStateTaxwithheld2", request.Box_16_Amount.HasValue ? request.Box_16_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("ARealizedPnL", request.Box_8_Amount.HasValue ? request.Box_8_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("AUnRealizedPnL22", request.Box_9_Amount.HasValue ? request.Box_9_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("AUnRealizedPnL", request.Box_10_Amount.HasValue ? request.Box_10_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("AAggregatePnL", request.Box_11_Amount.HasValue ? request.Box_11_Amount.Value.ToString() : string.Empty);   //PageA3
            if (request.Box_12_Checkbox != "1")
            {
                pdfFormFields.SetField("AReportedIRS", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("ABartering", request.Box_13_Amount.HasValue ? request.Box_13_Amount.Value.ToString() : string.Empty);   //PageA3

            #endregion

            #region Page 2
            if (request.Corrected?.ToString() == "1")
            {
                pdfFormFields.SetField("1Void", "0");   //PageAVoid
            }
            if (request.Corrected?.ToString() == "0")
            {
                pdfFormFields.SetField("1Corrected", "0");   //PageACorrected
            }
            pdfFormFields.SetField("1PayerNameAddress", PayData);   //PageAPayerNameAddress
            pdfFormFields.SetField("18949Code", request.Eight949_Code);   //Eight949_Code
            pdfFormFields.SetField("1PropertyDescription", request.Box_1a_Description);   //Box_1a_Description
            if (!string.IsNullOrEmpty(request.Box_1b_Date?.ToString()))
            {
                DateTime box1bDate;
                if (DateTime.TryParse(request.Box_1b_Date?.ToString(), out box1bDate))
                {
                    pdfFormFields.SetField("1AcquiredDate", box1bDate.ToString("MM/dd/yyyy"));
                }

            }
            if (!string.IsNullOrEmpty(request.Box_1c_Date?.ToString()))
            {
                DateTime Box_1c_Date;
                if (DateTime.TryParse(request.Box_1c_Date?.ToString(), out Box_1c_Date))
                {
                    pdfFormFields.SetField("1DateSold", Box_1c_Date.ToString("MM/dd/yyyy"));
                }

            }
            pdfFormFields.SetField("1PayerTIN", requestInstitue.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("1RecepientTIN", request.Rcp_TIN);   //request.Rcp_TIN
            pdfFormFields.SetField("1CashProceeds", request.Box_1d_Amount.HasValue ? request.Box_1d_Amount.Value.ToString() : string.Empty);
            pdfFormFields.SetField("1CostBasis", request.Box_1e_Amount.HasValue ? request.Box_1e_Amount.Value.ToString() : string.Empty);
            pdfFormFields.SetField("1AccruedDiscount", request.Box_1f_Amount.HasValue ? request.Box_1f_Amount.Value.ToString() : string.Empty);
            pdfFormFields.SetField("1WashSaleLoss", request.Box_1g_Amount.HasValue ? request.Box_1g_Amount.Value.ToString() : string.Empty);
            pdfFormFields.SetField("1RecepientName", request.First_Name + " " + request.Name_Line_2);   //request.First_Name + " " + request.Name_Line2
            if (request.Box_2_Checkbox_1 != "1")
            {
                pdfFormFields.SetField("1ShortTermGainLoss", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_2_Checkbox_2 != "1")
            {
                pdfFormFields.SetField("1LongTermGainLoss", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_3_Checkbox_1 != "1")
            {
                pdfFormFields.SetField("1Collectibles", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_2_Checkbox_3 != "1")
            {
                pdfFormFields.SetField("1Ordinary", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_3_Checkbox_2 != "1")
            {
                pdfFormFields.SetField("1QOF", "1");   //Box_2_Checkbox_1

            }
            pdfFormFields.SetField("1RecepientAddress", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("1FederalIncomeTax", request.Box_4_Amount.HasValue ? request.Box_4_Amount.Value.ToString() : string.Empty);   //PageA2
            if (request.Box_5_Checkbox != "1")
            {
                pdfFormFields.SetField("1NonCoveredSecurity", "1");   //Box_2_Checkbox_1

            }
            pdfFormFields.SetField("1RecepientCity", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("1AccountNumber", request.Rcp_Account);   //request.Rcp_Account
            pdfFormFields.SetField("1CUSIPNumber", request.CUSIP_No);
            pdfFormFields.SetField("1StateName1", request.Box_14_State);   //Box_14_State
            pdfFormFields.SetField("1StateName2", request.Box_14_State);   //Box_14_State
            if (request.Box_6_Checkbox_1 != "1")
            {
                pdfFormFields.SetField("1GrossProceed", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_6_Checkbox_2 != "1")
            {
                pdfFormFields.SetField("1NetProceed", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_7_Checkbox != "1")
            {
                pdfFormFields.SetField("1LossNotAllowed", "1");   //Box_2_Checkbox_1

            }
            if (request.FATCA_Checkbox != "1")
            {
                pdfFormFields.SetField("1FATCAFilingrequirements", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("1SIN1", request.Box_15_ID_Number);   //Box_15_ID_Number
            pdfFormFields.SetField("1StateTaxwithheld1", request.Box_16_Amount.HasValue ? request.Box_16_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("1SIN2", request.Box_15_ID_Number);   //Box_15_ID_Number
            pdfFormFields.SetField("1StateTaxwithheld2", request.Box_16_Amount.HasValue ? request.Box_16_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("1RealizedPnL", request.Box_8_Amount.HasValue ? request.Box_8_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("1UnRealizedPnL22", request.Box_9_Amount.HasValue ? request.Box_9_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("1UnRealizedPnL", request.Box_10_Amount.HasValue ? request.Box_10_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("1AggregatePnL", request.Box_11_Amount.HasValue ? request.Box_11_Amount.Value.ToString() : string.Empty);   //PageA3
            if (request.Box_12_Checkbox != "1")
            {
                pdfFormFields.SetField("1ReportedIRS", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("1Bartering", request.Box_13_Amount.HasValue ? request.Box_13_Amount.Value.ToString() : string.Empty);   //PageA3

            #endregion

            #region Page 3

            if (request.Corrected != "1")
            {
                pdfFormFields.SetField("BCorrected", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("BPayerNameAddress", PayData);   //PageAPayerNameAddress
            pdfFormFields.SetField("B8949Code", request.Eight949_Code);   //Eight949_Code
            pdfFormFields.SetField("BPropertyDescription", request.Box_1a_Description);   //Box_1a_Description
            if (!string.IsNullOrEmpty(request.Box_1b_Date?.ToString()))
            {
                DateTime box1bDate;
                if (DateTime.TryParse(request.Box_1b_Date?.ToString(), out box1bDate))
                {
                    pdfFormFields.SetField("BAcquiredDate", box1bDate.ToString("MM/dd/yyyy"));
                }

            }
            if (!string.IsNullOrEmpty(request.Box_1c_Date?.ToString()))
            {
                DateTime Box_1c_Date;
                if (DateTime.TryParse(request.Box_1c_Date?.ToString(), out Box_1c_Date))
                {
                    pdfFormFields.SetField("BDateSold", Box_1c_Date.ToString("MM/dd/yyyy"));
                }

            }
            pdfFormFields.SetField("BPayerTIN", requestInstitue.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("BRecepientTIN", request.Rcp_TIN);   //request.Rcp_TIN
            pdfFormFields.SetField("BCashProceeds", request.Box_1d_Amount.HasValue ? request.Box_1d_Amount.Value.ToString() : string.Empty);
            pdfFormFields.SetField("BCostBasis", request.Box_1e_Amount.HasValue ? request.Box_1e_Amount.Value.ToString() : string.Empty);
            pdfFormFields.SetField("BAccruedDiscount", request.Box_1f_Amount.HasValue ? request.Box_1f_Amount.Value.ToString() : string.Empty);
            pdfFormFields.SetField("BWashSaleLoss", request.Box_1g_Amount.HasValue ? request.Box_1g_Amount.Value.ToString() : string.Empty);
            pdfFormFields.SetField("BRecepientName", request.First_Name + " " + request.Name_Line_2);   //request.First_Name + " " + request.Name_Line2
            if (request.Box_2_Checkbox_1 != "1")
            {
                pdfFormFields.SetField("BShortTermGainLoss", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_2_Checkbox_2 != "1")
            {
                pdfFormFields.SetField("BLongTermGainLoss", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_3_Checkbox_1 != "1")
            {
                pdfFormFields.SetField("BCollectibles", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_2_Checkbox_3 != "1")
            {
                pdfFormFields.SetField("BOrdinary", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_3_Checkbox_2 != "1")
            {
                pdfFormFields.SetField("BQOF", "1");   //Box_2_Checkbox_1

            }
            pdfFormFields.SetField("BRecepientAddress", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("BFederalIncomeTax", request.Box_4_Amount.HasValue ? request.Box_4_Amount.Value.ToString() : string.Empty);   //PageA2
            if (request.Box_5_Checkbox != "1")
            {
                pdfFormFields.SetField("BNonCoveredSecurity", "1");   //Box_2_Checkbox_1

            }
            pdfFormFields.SetField("BRecepientCity", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("BAccountNumber", request.Rcp_Account);   //request.Rcp_Account
            pdfFormFields.SetField("BCUSIPNumber", request.CUSIP_No);
            pdfFormFields.SetField("BStateName1", request.Box_14_State);   //Box_14_State
            pdfFormFields.SetField("BStateName2", request.Box_14_State);   //Box_14_State
            if (request.Box_6_Checkbox_1 != "1")
            {
                pdfFormFields.SetField("BGrossProceed", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_6_Checkbox_2 != "1")
            {
                pdfFormFields.SetField("BNetProceed", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_7_Checkbox != "1")
            {
                pdfFormFields.SetField("BLossNotAllowed", "1");   //Box_2_Checkbox_1

            }
            if (request.FATCA_Checkbox != "1")
            {
                pdfFormFields.SetField("BFATCAFilingrequirements", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("BSIN1", request.Box_15_ID_Number);   //Box_15_ID_Number
            pdfFormFields.SetField("BStateTaxwithheld1", request.Box_16_Amount.HasValue ? request.Box_16_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("BSIN2", request.Box_15_ID_Number);   //Box_15_ID_Number
            pdfFormFields.SetField("BStateTaxwithheld2", request.Box_16_Amount.HasValue ? request.Box_16_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("BRealizedPnL", request.Box_8_Amount.HasValue ? request.Box_8_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("BUnRealizedPnL22", request.Box_9_Amount.HasValue ? request.Box_9_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("BUnRealizedPnL", request.Box_10_Amount.HasValue ? request.Box_10_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("BAggregatePnL", request.Box_11_Amount.HasValue ? request.Box_11_Amount.Value.ToString() : string.Empty);   //PageA3
            if (request.Box_12_Checkbox != "1")
            {
                pdfFormFields.SetField("BReportedIRS", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("BBartering", request.Box_13_Amount.HasValue ? request.Box_13_Amount.Value.ToString() : string.Empty);   //PageA3
            #endregion


            #region Page 4

            if (request.Corrected != "1")
            {
                pdfFormFields.SetField("2Corrected", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("2PayerNameAddress", PayData);   //PageAPayerNameAddress
            pdfFormFields.SetField("28949Code", request.Eight949_Code);   //Eight949_Code
            pdfFormFields.SetField("2PropertyDescription", request.Box_1a_Description);   //Box_1a_Description
            if (!string.IsNullOrEmpty(request.Box_1b_Date?.ToString()))
            {
                DateTime box1bDate;
                if (DateTime.TryParse(request.Box_1b_Date?.ToString(), out box1bDate))
                {
                    pdfFormFields.SetField("2AcquiredDate", box1bDate.ToString("MM/dd/yyyy"));
                }

            }
            if (!string.IsNullOrEmpty(request.Box_1c_Date?.ToString()))
            {
                DateTime Box_1c_Date;
                if (DateTime.TryParse(request.Box_1c_Date?.ToString(), out Box_1c_Date))
                {
                    pdfFormFields.SetField("2DateSold", Box_1c_Date.ToString("MM/dd/yyyy"));
                }

            }
            pdfFormFields.SetField("2PayerTIN", requestInstitue.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("2RecepientTIN", request.Rcp_TIN);   //request.Rcp_TIN
            pdfFormFields.SetField("2CashProceeds", request.Box_1d_Amount.HasValue ? request.Box_1d_Amount.Value.ToString() : string.Empty);
            pdfFormFields.SetField("2CostBasis", request.Box_1e_Amount.HasValue ? request.Box_1e_Amount.Value.ToString() : string.Empty);
            pdfFormFields.SetField("2AccruedDiscount", request.Box_1f_Amount.HasValue ? request.Box_1f_Amount.Value.ToString() : string.Empty);
            pdfFormFields.SetField("2WashSaleLoss", request.Box_1g_Amount.HasValue ? request.Box_1g_Amount.Value.ToString() : string.Empty);
            pdfFormFields.SetField("2RecepientName", request.First_Name + " " + request.Name_Line_2);   //request.First_Name + " " + request.Name_Line2
            if (request.Box_2_Checkbox_1 != "1")
            {
                pdfFormFields.SetField("2ShortTermGainLoss", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_2_Checkbox_2 != "1")
            {
                pdfFormFields.SetField("2LongTermGainLoss", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_3_Checkbox_1 != "1")
            {
                pdfFormFields.SetField("2Collectibles", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_2_Checkbox_3 != "1")
            {
                pdfFormFields.SetField("2Ordinary", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_3_Checkbox_2 != "1")
            {
                pdfFormFields.SetField("2QOF", "1");   //Box_2_Checkbox_1

            }
            pdfFormFields.SetField("2RecepientAddress", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("2FederalIncomeTax", request.Box_4_Amount.HasValue ? request.Box_4_Amount.Value.ToString() : string.Empty);   //PageA2
            if (request.Box_5_Checkbox != "1")
            {
                pdfFormFields.SetField("2NonCoveredSecurity", "1");   //Box_2_Checkbox_1

            }
            pdfFormFields.SetField("2RecepientCity", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("2AccountNumber", request.Rcp_Account);   //request.Rcp_Account
            pdfFormFields.SetField("2CUSIPNumber", request.CUSIP_No);
            pdfFormFields.SetField("2StateName1", request.Box_14_State);   //Box_14_State
            pdfFormFields.SetField("2StateName2", request.Box_14_State);   //Box_14_State
            if (request.Box_6_Checkbox_1 != "1")
            {
                pdfFormFields.SetField("2GrossProceed", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_6_Checkbox_2 != "1")
            {
                pdfFormFields.SetField("2NetProceed", "1");   //Box_2_Checkbox_1

            }
            if (request.Box_7_Checkbox != "1")
            {
                pdfFormFields.SetField("2LossNotAllowed", "1");   //Box_2_Checkbox_1

            }
            if (request.FATCA_Checkbox != "1")
            {
                pdfFormFields.SetField("2FATCAFilingrequirements", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("2SIN1", request.Box_15_ID_Number);   //Box_15_ID_Number
            pdfFormFields.SetField("2StateTaxwithheld1", request.Box_16_Amount.HasValue ? request.Box_16_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("2SIN2", request.Box_15_ID_Number);   //Box_15_ID_Number
            pdfFormFields.SetField("2StateTaxwithheld2", request.Box_16_Amount.HasValue ? request.Box_16_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("2RealizedPnL", request.Box_8_Amount.HasValue ? request.Box_8_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("2UnRealizedPnL22", request.Box_9_Amount.HasValue ? request.Box_9_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("2UnRealizedPnL", request.Box_10_Amount.HasValue ? request.Box_10_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("2AggregatePnL", request.Box_11_Amount.HasValue ? request.Box_11_Amount.Value.ToString() : string.Empty);   //PageA3
            if (request.Box_12_Checkbox != "1")
            {
                pdfFormFields.SetField("2ReportedIRS", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("2Bartering", request.Box_13_Amount.HasValue ? request.Box_13_Amount.Value.ToString() : string.Empty);   //PageA3
            #endregion




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

                string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.B_1099_TemplateFileName);
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
            var request = _evolvedtaxContext.Tbl1099_B.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.First_Name + " " + request.Name_Line_2?.Replace(": ", "");
            newFile1 = string.Concat(ClientName, "_", AppConstants.B1099Form, "_", Id);
            string FilenameNew = "/1099B/" + newFile1 + ".pdf";
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
            var request = _evolvedtaxContext.Tbl1099_B.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.First_Name + " " + request.Name_Line_2?.Replace(": ", "");    

            newFile1 = string.Concat(ClientName, "_", AppConstants.B1099Form, "_", request.Id, "_Page_", selectedPage);
            string FilenameNew = "/1099B/" + newFile1 + ".pdf";
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
            string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.B_1099_TemplateFileName);
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
                            compileFileName = "For Recipient.pdf";
                            break;
                        case "6":
                            compileFileName = "To be filed with recipient’s state income tax return.pdf";
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

            var recordToDelete = _evolvedtaxContext.Tbl1099_B.First(p => p.Id == id);
            if (recordToDelete != null)
            {
                _evolvedtaxContext.Tbl1099_B.Remove(recordToDelete);
                await _evolvedtaxContext.SaveChangesAsync();
                return new MessageResponseModel { Status = true };
            }

            return new MessageResponseModel { Status = false };
        }

        public async Task<MessageResponseModel> KeepRecord(int id)
        {

            var recordToUpdate = _evolvedtaxContext.Tbl1099_B.First(p => p.Id == id);
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
            var result = from ic in _evolvedtaxContext.Tbl1099_B
                         where selectValues.Contains(ic.Id) && !string.IsNullOrEmpty(ic.Rcp_Email)
                         select new Tbl1099_B
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
        public IEnumerable<Tbl1099_B> GetForm1099BList()
        {
            return _evolvedtaxContext.Tbl1099_B.AsEnumerable();
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

    }
}
