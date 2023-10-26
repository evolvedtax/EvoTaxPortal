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
using EvolvedTax.Business.Services.InstituteService;

namespace EvolvedTax.Business.Services.Form1099Services
{
    public class Form1099_INT_Service : IForm1099_INT_Service
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly IMailService _mailService;
        readonly IInstituteService _instituteService;
        readonly ITrailAudit1099Service _trailAudit1099Service;


        public Form1099_INT_Service(EvolvedtaxContext evolvedtaxContext, IMailService mailService, ITrailAudit1099Service trailAudit1099Service, IInstituteService instituteService)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _mailService = mailService;
            _trailAudit1099Service = trailAudit1099Service;
            _instituteService = instituteService;
        }
        public async Task<MessageResponseModel> Upload1099_Data(IFormFile file, int entityId, int InstId, string UserId)
        {
            bool Status = false;
            var response = new List<Tbl1099_INT>();
            var List = new List<Tbl1099_INT>();
            using (var stream = file.OpenReadStream())
            {
                IWorkbook workbook = new XSSFWorkbook(stream);
                ISheet sheet = workbook.GetSheetAt(0); // Assuming the data is in the first sheet

                HashSet<string> uniqueEINNumber = new HashSet<string>();
                HashSet<string> uniqueEntityNames = new HashSet<string>();

                var columnMapping = new Dictionary<string, int>();
                var headerRow = sheet.GetRow(0); // Assuming the header row is the first row

                if (headerRow != null)
                {
                    for (int columnIndex = 0; columnIndex < headerRow.LastCellNum; columnIndex++)
                    {
                        string columnName = headerRow.GetCell(columnIndex)?.ToString();

                        if (!string.IsNullOrWhiteSpace(columnName))
                        {
                            columnMapping[columnName] = columnIndex;
                        }
                    }
                }

                for (int row = 1; row <= sheet.LastRowNum; row++) // Starting from the second row
                {
                    IRow excelRow = sheet.GetRow(row);

                    string cell_value_13 = excelRow.GetCell(columnMapping[" 2nd TIN Notice"])?.ToString();
                    string Second_TIN_Notice = string.IsNullOrEmpty(cell_value_13) ? "0" : (cell_value_13.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    string cell_value_14 = excelRow.GetCell(columnMapping["FATCA Checkbox"])?.ToString();
                    string FATCA_Checkbox = string.IsNullOrEmpty(cell_value_14) ? "0" : (cell_value_14.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");


                    string cell_value_35 = excelRow.GetCell(columnMapping["Is Corrected Form of 1099"]  )?.ToString();
                    string Corrected = string.IsNullOrEmpty(cell_value_35) ? "0" : (cell_value_35.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    var entity = new Tbl1099_INT
                    {
                        Rcp_TIN = excelRow.GetCell(columnMapping["Rcp TIN"])?.ToString(),
                        Last_Name_Company = excelRow.GetCell(columnMapping["Company"])?.ToString(),
                        First_Name = excelRow.GetCell(columnMapping["First Name"])?.ToString(),
                        Name_Line_2 = excelRow.GetCell(columnMapping["Last Name"])?.ToString(),
                        Address_Type = excelRow.GetCell(columnMapping["Address Type"])?.ToString(),
                        Country = excelRow.GetCell(columnMapping["Country"])?.ToString(),
                        Address_Deliv_Street = excelRow.GetCell(columnMapping["Address Line 1"])?.ToString(),
                        Address_Apt_Suite = excelRow.GetCell(columnMapping["Address Line 2"])?.ToString(),
                        City = excelRow.GetCell(columnMapping["City"])?.ToString(),
                        State = excelRow.GetCell(columnMapping["State"])?.ToString(),
                        Province = excelRow.GetCell(columnMapping["Province"])?.ToString(),
                        Zip = excelRow.GetCell(columnMapping["Zip"])?.ToString(),
                        PostalCode = excelRow.GetCell(columnMapping["Postal Code"])?.ToString(),
                        Rcp_Account = excelRow.GetCell(columnMapping["Rcp Account"])?.ToString(),
                        Rcp_Email = excelRow.GetCell(columnMapping["Rcp Email"])?.ToString(),
                        Second_TIN_Notice = Second_TIN_Notice,
                        FATCA_Checkbox = FATCA_Checkbox,
                        Box_1_Amount = TryConvertToDecimal(excelRow.GetCell(columnMapping["Box 1 Amount"])),
                        Box_2_Amount = TryConvertToDecimal(excelRow.GetCell(columnMapping["Box 2 Amount"])),
                        Box_3_Amount = TryConvertToDecimal(excelRow.GetCell(columnMapping["Box 3 Amount"])),
                        Box_4_Amount = TryConvertToDecimal(excelRow.GetCell(columnMapping["Box 4 Amount"])),
                        Box_5_Amount = TryConvertToDecimal(excelRow.GetCell(columnMapping["Box 5 Amount"])),
                        Box_6_Amount = TryConvertToDecimal(excelRow.GetCell(columnMapping["Box 6 Amount"])),
                        Box_7_Foreign = excelRow.GetCell(columnMapping["Box 7 Foreign"])?.ToString(),
                        Box_8_Amount = TryConvertToDecimal(excelRow.GetCell(columnMapping["Box 8 Amount"])),
                        Box_9_Amount = TryConvertToDecimal(excelRow.GetCell(columnMapping["Box 9 Amount"])),
                        Box_10_Amount = TryConvertToDecimal(excelRow.GetCell(columnMapping["Box 10 Amount"])),
                        Box_11_Amount = TryConvertToDecimal(excelRow.GetCell(columnMapping["Box 11 Amount"])),
                        Box_12_Amount = TryConvertToDecimal(excelRow.GetCell(columnMapping["Box 12 Amount"])),
                        Box_13_Amount = TryConvertToDecimal(excelRow.GetCell(columnMapping["Box 13 Amount"])),
                        Box_14_CUSIPNo = excelRow.GetCell(columnMapping["Box 14 CUSIP No"])?.ToString(),
                        Box_15_State = excelRow.GetCell(columnMapping["Box 15 State"])?.ToString(),
                        Box_16_IDNumber = excelRow.GetCell(columnMapping["Box 16 ID Number"])?.ToString(),
                        Box_17_Amount = TryConvertToDecimal(excelRow.GetCell(columnMapping["Box 17 Amount"])),
                        Form_Category = excelRow.GetCell(columnMapping["Form Category"])?.ToString(),
                        Form_Source = excelRow.GetCell(columnMapping["Form Source"])?.ToString(),
                        Tax_State = excelRow.GetCell(columnMapping["Tax State"])?.ToString(),
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
                    if (await _evolvedtaxContext.Tbl1099_INT.AnyAsync(p => 
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

                
                    await _evolvedtaxContext.Tbl1099_INT.AddRangeAsync(List);
                    await _evolvedtaxContext.SaveChangesAsync();

                return new MessageResponseModel { Status = Status, Message = response, Param = "Entity" };
            }

        }

        public string GeneratePdf(int Id, string TemplatefilePath, string SaveFolderPath)
        {
            return CreatePdf(Id, TemplatefilePath, SaveFolderPath, false);

        }

        public string CreatePdf(int Id, string TemplatefilePath, string SaveFolderPath, bool IsAll, string Page = "")
        {
            var request = _evolvedtaxContext.Tbl1099_INT.FirstOrDefault(p => p.Id == Id);
            var requestInstitue = _instituteService.GetPayeeData((int)request.InstID);
            string templatefile = TemplatefilePath;
            string newFile1 = string.Empty;

            String ClientName = request.First_Name + " " + request.Name_Line_2?.Replace(": ", "");

            if (!String.IsNullOrEmpty(Page))
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.INT1099Form, "_", request.Id, "_Page_", Page);
            }
            else
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.INT1099Form, "_", request.Id);
            }


            string FilenameNew = "/1099INT/" + newFile1 + ".pdf";
            string newFileName = newFile1 + ".pdf"; // Add ".pdf" extension to the file name

            string newFilePath = Path.Combine(SaveFolderPath, newFileName);

            PdfReader pdfReader = new PdfReader(templatefile);
            PdfReader.unethicalreading = true;
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFilePath, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;

            string Recepient_CountryCode = "";
            if (request.Country != "United States")
            {
                var country = _evolvedtaxContext.MstrCountries.FirstOrDefault(c => c.Country == request.Country);
                if (country != null)
                {
                    Recepient_CountryCode = country.CountryId;
                }
            }

            string RecipentCity = string.Join(", ",
               new[]
               {
                    request.City,
                    request.State,
                    string.IsNullOrWhiteSpace(request.Province) ? null : request.Province,
                     string.IsNullOrWhiteSpace(Recepient_CountryCode) ? null : Recepient_CountryCode,
                    request.Zip,
                    string.IsNullOrWhiteSpace(request.PostalCode) ? null : request.PostalCode

               }.Where(s => !string.IsNullOrWhiteSpace(s))
           );

            String RecipentAddress = string.Concat(request.Address_Deliv_Street, ", ", request.Address_Apt_Suite);
            int currenDate = DateTime.Now.Year;
            string currentYear = Convert.ToString(currenDate % 100);

            #region PDF Columns

            #region Page 1

            if (request.Corrected.ToString() == "1")
            {
                pdfFormFields.SetField("AVoid", "0");   //PageAVoid
            }
            if (request.Corrected.ToString() == "0")
            {
                pdfFormFields.SetField("ACorrected", "0");   //PageACorrected
            }
            pdfFormFields.SetField("APayerNameAddress", requestInstitue.PayeeData);   //PageAPayerNameAddress
            //pdfFormFields.SetField("APayerRTN", "APayerRTN");   //PageAPayerRTN
            pdfFormFields.SetField("A1", request.Box_1_Amount.HasValue ? request.Box_1_Amount.Value.ToString() : string.Empty);   //PageA1
            pdfFormFields.SetField("ACalendarYear", currentYear);   //PageACalendarYear
            pdfFormFields.SetField("APayerTIN", requestInstitue.Idnumber);   //PageAPayerTIN
            pdfFormFields.SetField("ARecipientTIN", request.Rcp_TIN);   //PageARecipientTIN
            pdfFormFields.SetField("A2", request.Box_2_Amount.HasValue ? request.Box_2_Amount.Value.ToString() : string.Empty);   //PageA2
            pdfFormFields.SetField("A3", request.Box_3_Amount.HasValue ? request.Box_3_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("ARecipientName", request.First_Name + " " + request.Name_Line_2);   //PageARecipientName
            pdfFormFields.SetField("A4", request.Box_4_Amount.HasValue ? request.Box_4_Amount.Value.ToString() : string.Empty);  //PageA4
            pdfFormFields.SetField("A5", request.Box_5_Amount.HasValue ? request.Box_5_Amount.Value.ToString() : string.Empty);  //PageA5
            pdfFormFields.SetField("ARecipientAddress", RecipentAddress);   //PageARecipientAddress
            pdfFormFields.SetField("A6", request.Box_6_Amount.HasValue ? request.Box_6_Amount.Value.ToString() : string.Empty);  //PageA6
            pdfFormFields.SetField("A7", request.Box_7_Foreign);   //A7
            pdfFormFields.SetField("ARecipientCity", RecipentCity);   //PageARecipientCity
            pdfFormFields.SetField("A8", request.Box_8_Amount.HasValue ? request.Box_8_Amount.Value.ToString() : string.Empty); //PageA8
            pdfFormFields.SetField("A9", request.Box_9_Amount.HasValue ? request.Box_9_Amount.Value.ToString() : string.Empty); //PageA9
            pdfFormFields.SetField("A10", request.Box_10_Amount.HasValue ? request.Box_10_Amount.Value.ToString() : string.Empty); //PageA10
            pdfFormFields.SetField("A11", request.Box_11_Amount.HasValue ? request.Box_11_Amount.Value.ToString() : string.Empty); //PageA11
            if (request.FATCA_Checkbox != "1")
            {
                pdfFormFields.SetField("AFATCA", "1");   //PageAFATCA

            }

            pdfFormFields.SetField("A12", request.Box_12_Amount.HasValue ? request.Box_12_Amount.Value.ToString() : string.Empty); //PageA12
            pdfFormFields.SetField("A13", request.Box_13_Amount.HasValue ? request.Box_13_Amount.Value.ToString() : string.Empty); //PageA13
            pdfFormFields.SetField("AAccountNumber", request.Rcp_Account);   //PageAAccountNumber
            if (request.Second_TIN_Notice != "1")
            {
                pdfFormFields.SetField("A2ndTIN", "1");   //PageA2ndTIN

            }
            pdfFormFields.SetField("A15a", request.State);   //PageA15a
            pdfFormFields.SetField("A16a", request.Box_16_IDNumber);   //A16a
            pdfFormFields.SetField("A17a", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty); //A17a
            pdfFormFields.SetField("A14", request.Box_14_CUSIPNo);   //PageA14

            pdfFormFields.SetField("A15b", request.State);   //PageA15b
            pdfFormFields.SetField("A16b", request.Box_16_IDNumber);   //PageA16b
            pdfFormFields.SetField("A17b", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty);   //PageA17b 
            #endregion

            #region Page 2

            if (request.Corrected.ToString() == "1")
            {
                pdfFormFields.SetField("1Void", "0");   //PageAVoid
            }
            if (request.Corrected.ToString() == "0")
            {
                pdfFormFields.SetField("1Corrected", "0");   //PageACorrected
            }
            pdfFormFields.SetField("1PayerNameAddress", requestInstitue.PayeeData);   //PageAPayerNameAddress
            //pdfFormFields.SetField("1PayerRTN", "APayerRTN");   //PageAPayerRTN
            pdfFormFields.SetField("A1", request.Box_1_Amount.HasValue ? request.Box_1_Amount.Value.ToString() : string.Empty);   //PageA1
            pdfFormFields.SetField("1CalendarYear", currentYear);   //PageACalendarYear
            pdfFormFields.SetField("1PayerTIN", requestInstitue.Idnumber);   //PageAPayerTIN
            pdfFormFields.SetField("1RecipientTIN", request.Rcp_TIN);   //PageARecipientTIN
            pdfFormFields.SetField("1RecipientName", request.First_Name + " " + request.Name_Line_2);   //PageARecipientName
            pdfFormFields.SetField("1RecipientAddress", RecipentAddress);   //PageARecipientAddress
            pdfFormFields.SetField("1RecipientCity", RecipentCity);   //PageARecipientCity
            if (request.FATCA_Checkbox != "1")
            {
                pdfFormFields.SetField("1FATCA", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("1AccountNumber", request.Rcp_Account);   //PageAAccountNumber
            pdfFormFields.SetField("11", request.Box_1_Amount.HasValue ? request.Box_1_Amount.Value.ToString() : string.Empty);   //PageA2
            pdfFormFields.SetField("12", request.Box_2_Amount.HasValue ? request.Box_2_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("13", request.Box_3_Amount.HasValue ? request.Box_3_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("14", request.Box_4_Amount.HasValue ? request.Box_4_Amount.Value.ToString() : string.Empty);  //PageA4
            pdfFormFields.SetField("15", request.Box_5_Amount.HasValue ? request.Box_5_Amount.Value.ToString() : string.Empty);  //PageA5
            pdfFormFields.SetField("16", request.Box_6_Amount.HasValue ? request.Box_6_Amount.Value.ToString() : string.Empty);  //PageA6
            pdfFormFields.SetField("17", request.Box_7_Foreign);   //A7
            pdfFormFields.SetField("18", request.Box_8_Amount.HasValue ? request.Box_8_Amount.Value.ToString() : string.Empty); //PageA8
            pdfFormFields.SetField("19", request.Box_9_Amount.HasValue ? request.Box_9_Amount.Value.ToString() : string.Empty); //PageA9
            pdfFormFields.SetField("110", request.Box_10_Amount.HasValue ? request.Box_10_Amount.Value.ToString() : string.Empty); //PageA10
            pdfFormFields.SetField("111", request.Box_11_Amount.HasValue ? request.Box_11_Amount.Value.ToString() : string.Empty); //PageA11
            pdfFormFields.SetField("112", request.Box_12_Amount.HasValue ? request.Box_12_Amount.Value.ToString() : string.Empty); //PageA12
            pdfFormFields.SetField("113", request.Box_13_Amount.HasValue ? request.Box_13_Amount.Value.ToString() : string.Empty); //PageA13
   
          
            
            pdfFormFields.SetField("115a", request.State);   //PageA15a
            pdfFormFields.SetField("116a", request.Box_16_IDNumber);   //A16a
            pdfFormFields.SetField("117a", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty); //A17a
            pdfFormFields.SetField("114", request.Box_14_CUSIPNo);   //PageA14

            pdfFormFields.SetField("115b", request.State);   //PageA15b
            pdfFormFields.SetField("116b", request.Box_16_IDNumber);   //PageA16b
            pdfFormFields.SetField("117b", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty);   //PageA17b 
            #endregion

            #region Page 3


            if (request.Corrected.ToString() == "0")
            {
                pdfFormFields.SetField("BCorrected", "0");   //PageACorrected
            }

            pdfFormFields.SetField("BPayerNameAddress", requestInstitue.PayeeData);   //PageAPayerNameAddress
           //pdfFormFields.SetField("BPayerRTN", "APayerRTN");   //PageAPayerRTN
            pdfFormFields.SetField("BCalendarYear", currentYear);   //PageACalendarYear
            pdfFormFields.SetField("BRecipientTIN", request.Rcp_TIN);   //PageARecipientTIN
            pdfFormFields.SetField("BRecipientName", request.First_Name + " " + request.Name_Line_2);   //PageARecipientName
            pdfFormFields.SetField("BRecipientAddress", RecipentAddress);   //PageARecipientAddress
            pdfFormFields.SetField("BRecipientCity", RecipentCity);   //PageARecipientCity
            if (request.FATCA_Checkbox != "1")
            {
                pdfFormFields.SetField("BFATCA", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("BAccountNumber", request.Rcp_Account);   //PageAAccountNumber

            pdfFormFields.SetField("B1", request.Box_1_Amount.HasValue ? request.Box_1_Amount.Value.ToString() : string.Empty);   //PageA2
            pdfFormFields.SetField("B2", request.Box_2_Amount.HasValue ? request.Box_2_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("B3", request.Box_3_Amount.HasValue ? request.Box_3_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("B4", request.Box_4_Amount.HasValue ? request.Box_4_Amount.Value.ToString() : string.Empty);  //PageA4
            pdfFormFields.SetField("B5", request.Box_5_Amount.HasValue ? request.Box_5_Amount.Value.ToString() : string.Empty);  //PageA5
            pdfFormFields.SetField("B6", request.Box_6_Amount.HasValue ? request.Box_6_Amount.Value.ToString() : string.Empty);  //PageA6
            pdfFormFields.SetField("B7", request.Box_7_Foreign);   //A7
            pdfFormFields.SetField("B8", request.Box_8_Amount.HasValue ? request.Box_8_Amount.Value.ToString() : string.Empty); //PageA8
            pdfFormFields.SetField("B9", request.Box_9_Amount.HasValue ? request.Box_9_Amount.Value.ToString() : string.Empty); //PageA9
            pdfFormFields.SetField("B10", request.Box_10_Amount.HasValue ? request.Box_10_Amount.Value.ToString() : string.Empty); //PageA10
            pdfFormFields.SetField("B11", request.Box_11_Amount.HasValue ? request.Box_11_Amount.Value.ToString() : string.Empty); //PageA11
            pdfFormFields.SetField("B12", request.Box_12_Amount.HasValue ? request.Box_12_Amount.Value.ToString() : string.Empty); //PageA12
            pdfFormFields.SetField("B13", request.Box_13_Amount.HasValue ? request.Box_13_Amount.Value.ToString() : string.Empty); //PageA13
            pdfFormFields.SetField("B15a", request.State);   //PageA15a
            pdfFormFields.SetField("B16a", request.Box_16_IDNumber);   //A16a
            pdfFormFields.SetField("B17a", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty); //A17a
            pdfFormFields.SetField("B14", request.Box_14_CUSIPNo);   //PageA14

            pdfFormFields.SetField("B15b", request.State);   //PageA15b
            pdfFormFields.SetField("B16b", request.Box_16_IDNumber);   //PageA16b
            pdfFormFields.SetField("B17b", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty);   //PageA17b 
            #endregion


            #region Page 4

            if (request.Corrected.ToString() == "0")
            {
                pdfFormFields.SetField("2Corrected", "0");   //PageACorrected
            }
            pdfFormFields.SetField("2PayerNameAddress", requestInstitue.PayeeData);   //PageAPayerNameAddress
            //pdfFormFields.SetField("2PayerRTN", "APayerRTN");   //PageAPayerRTN
            pdfFormFields.SetField("2CalendarYear", currentYear);   //PageACalendarYear
            pdfFormFields.SetField("2PayerTIN", requestInstitue.Idnumber);   //Page2PayerTIN
            pdfFormFields.SetField("2RecipientTIN", request.Rcp_TIN);   //Page2PayerTIN
            pdfFormFields.SetField("2RecipientName", request.First_Name + " " + request.Name_Line_2);   //PageARecipientName
            pdfFormFields.SetField("2RecipientAddress", RecipentAddress);   //PageARecipientAddress
            pdfFormFields.SetField("2RecipientCity", RecipentCity);   //PageARecipientCity
            if (request.FATCA_Checkbox != "1")
            {
                pdfFormFields.SetField("2FATCA", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("2AccountNumber", request.Rcp_Account);   //PageAAccountNumber

            pdfFormFields.SetField("21", request.Box_1_Amount.HasValue ? request.Box_1_Amount.Value.ToString() : string.Empty);   //PageA2
            pdfFormFields.SetField("22", request.Box_2_Amount.HasValue ? request.Box_2_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("23", request.Box_3_Amount.HasValue ? request.Box_3_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("24", request.Box_4_Amount.HasValue ? request.Box_4_Amount.Value.ToString() : string.Empty);  //PageA4
            pdfFormFields.SetField("25", request.Box_5_Amount.HasValue ? request.Box_5_Amount.Value.ToString() : string.Empty);  //PageA5
            pdfFormFields.SetField("26", request.Box_6_Amount.HasValue ? request.Box_6_Amount.Value.ToString() : string.Empty);  //PageA6
            pdfFormFields.SetField("27", request.Box_7_Foreign);   //A7
            pdfFormFields.SetField("28", request.Box_8_Amount.HasValue ? request.Box_8_Amount.Value.ToString() : string.Empty); //PageA8
            pdfFormFields.SetField("29", request.Box_9_Amount.HasValue ? request.Box_9_Amount.Value.ToString() : string.Empty); //PageA9
            pdfFormFields.SetField("210", request.Box_10_Amount.HasValue ? request.Box_10_Amount.Value.ToString() : string.Empty); //PageA10
            pdfFormFields.SetField("211", request.Box_11_Amount.HasValue ? request.Box_11_Amount.Value.ToString() : string.Empty); //PageA11
            pdfFormFields.SetField("212", request.Box_12_Amount.HasValue ? request.Box_12_Amount.Value.ToString() : string.Empty); //PageA12
            pdfFormFields.SetField("213", request.Box_13_Amount.HasValue ? request.Box_13_Amount.Value.ToString() : string.Empty); //PageA13
            pdfFormFields.SetField("215a", request.State);   //PageA15a
            pdfFormFields.SetField("216a", request.Box_16_IDNumber);   //A16a
            pdfFormFields.SetField("217a", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty); //A17a
            pdfFormFields.SetField("214", request.Box_14_CUSIPNo);   //PageA14

            pdfFormFields.SetField("215b", request.State);   //PageA15b
            pdfFormFields.SetField("216b", request.Box_16_IDNumber);   //PageA16b
            pdfFormFields.SetField("217b", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty);   //PageA17b 
            #endregion

            #region Page 5

            if (request.Corrected.ToString() == "1")
            {
                pdfFormFields.SetField("CVoid", "0");   //PageAVoid
            }
      
            if (request.Corrected.ToString() == "0")
            {
                pdfFormFields.SetField("CCorrected", "0");   //PageACorrected
            }
            pdfFormFields.SetField("CPayerNameAddress", requestInstitue.PayeeData);   //PageAPayerNameAddress
            //pdfFormFields.SetField("CPayerRTN", "APayerRTN");   //PageAPayerRTN
            pdfFormFields.SetField("CCalendarYear", currentYear);   //PageACalendarYear
            pdfFormFields.SetField("CPayerTIN", requestInstitue.Idnumber);   //Page2PayerTIN
            pdfFormFields.SetField("CRecipientTIN", request.Rcp_TIN);   //Page2PayerTIN
            pdfFormFields.SetField("CRecipientName", request.First_Name + " " + request.Name_Line_2);   //PageARecipientName
            pdfFormFields.SetField("CRecipientAddress", RecipentAddress);   //PageARecipientAddress
            pdfFormFields.SetField("CRecipientCity", RecipentCity);   //PageARecipientCity
            if (request.FATCA_Checkbox != "1")
            {
                pdfFormFields.SetField("CFATCA", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("CAccountNumber", request.Rcp_Account);   //PageAAccountNumber
            if (request.Second_TIN_Notice != "1")
            {
                pdfFormFields.SetField("C2ndTIN", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("C1", request.Box_1_Amount.HasValue ? request.Box_1_Amount.Value.ToString() : string.Empty);   //PageA2
            pdfFormFields.SetField("C2", request.Box_2_Amount.HasValue ? request.Box_2_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("C3", request.Box_3_Amount.HasValue ? request.Box_3_Amount.Value.ToString() : string.Empty);   //PageA3
            pdfFormFields.SetField("C4", request.Box_4_Amount.HasValue ? request.Box_4_Amount.Value.ToString() : string.Empty);  //PageA4
            pdfFormFields.SetField("C5", request.Box_5_Amount.HasValue ? request.Box_5_Amount.Value.ToString() : string.Empty);  //PageA5
            pdfFormFields.SetField("C6", request.Box_6_Amount.HasValue ? request.Box_6_Amount.Value.ToString() : string.Empty);  //PageA6
            pdfFormFields.SetField("C7", request.Box_7_Foreign);   //A7
            pdfFormFields.SetField("C8", request.Box_8_Amount.HasValue ? request.Box_8_Amount.Value.ToString() : string.Empty); //PageA8
            pdfFormFields.SetField("C9", request.Box_9_Amount.HasValue ? request.Box_9_Amount.Value.ToString() : string.Empty); //PageA9
            pdfFormFields.SetField("C10", request.Box_10_Amount.HasValue ? request.Box_10_Amount.Value.ToString() : string.Empty); //PageA10
            pdfFormFields.SetField("C11", request.Box_11_Amount.HasValue ? request.Box_11_Amount.Value.ToString() : string.Empty); //PageA11
            pdfFormFields.SetField("C12", request.Box_12_Amount.HasValue ? request.Box_12_Amount.Value.ToString() : string.Empty); //PageA12
            pdfFormFields.SetField("C13", request.Box_13_Amount.HasValue ? request.Box_13_Amount.Value.ToString() : string.Empty); //PageA13
            pdfFormFields.SetField("C15a", request.State);   //PageA15a
            pdfFormFields.SetField("C16a", request.Box_16_IDNumber);   //A16a
            pdfFormFields.SetField("C17a", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty); //A17a
            pdfFormFields.SetField("C14", request.Box_14_CUSIPNo);   //PageA14

            pdfFormFields.SetField("C15b", request.State);   //PageA15b
            pdfFormFields.SetField("C16b", request.Box_16_IDNumber);   //PageA16b
            pdfFormFields.SetField("C17b", request.Box_17_Amount.HasValue ? request.Box_17_Amount.Value.ToString() : string.Empty);   //PageA17b 
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

                string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.INT_1099_TemplateFileName);
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
            var request = _evolvedtaxContext.Tbl1099_INT.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.First_Name + " " + request.Name_Line_2?.Replace(": ", "");
            newFile1 = string.Concat(ClientName, "_", AppConstants.INT1099Form, "_", Id);
            string FilenameNew = "/1099INT/" + newFile1 + ".pdf";
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
            var request = _evolvedtaxContext.Tbl1099_INT.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.First_Name + " " + request.Name_Line_2?.Replace(": ", "");    

            newFile1 = string.Concat(ClientName, "_", AppConstants.INT1099Form, "_", request.Id, "_Page_", selectedPage);
            string FilenameNew = "/1099INT/" + newFile1 + ".pdf";
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
            string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.INT_1099_TemplateFileName);
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
                        case "8":
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

        public async Task<MessageResponseModel> DeletePermeant(int id)
        {

            var recordToDelete = _evolvedtaxContext.Tbl1099_INT.First(p => p.Id == id);
            if (recordToDelete != null)
            {
                _evolvedtaxContext.Tbl1099_INT.Remove(recordToDelete);
                await _evolvedtaxContext.SaveChangesAsync();
                return new MessageResponseModel { Status = true };
            }

            return new MessageResponseModel { Status = false };
        }

        public async Task<MessageResponseModel> KeepRecord(int id)
        {

            var recordToUpdate = _evolvedtaxContext.Tbl1099_INT.First(p => p.Id == id);
            if (recordToUpdate != null)
            {
                recordToUpdate.IsDuplicated = false;
                await _evolvedtaxContext.SaveChangesAsync();
                return new MessageResponseModel { Status = true, Message = "The record has been kept" };
            }

            return new MessageResponseModel { Status = false, Message = "Oops! something wrong" };
        }
        public async Task<bool> SendEmailToRecipients(int[] selectValues, string URL, string form, int instituteId = -1)
        {
            var result = from ic in _evolvedtaxContext.Tbl1099_INT
                         where selectValues.Contains(ic.Id) && !string.IsNullOrEmpty(ic.Rcp_Email)
                         select new Tbl1099_INT
                         {
                             Rcp_Email = ic.Rcp_Email,
                             EntityId = ic.EntityId,
                         };
            foreach (var item in result.ToList())
            {
                await _mailService.SendElectronicAcceptanceEmail(item.Rcp_Email, (int)item.EntityId, string.Empty, "Action Required", URL, form, instituteId);



                await _trailAudit1099Service.AddUpdateRecipientAuditDetails(new AuditTrail1099 { RecipientEmail = item.Rcp_Email, FormName = form, Token = item.EntityId.ToString() ?? string.Empty });
            }
            return true;
        }
        public IEnumerable<Tbl1099_INT> GetForm1099INTList()
        {
            return _evolvedtaxContext.Tbl1099_INT.AsEnumerable();
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
