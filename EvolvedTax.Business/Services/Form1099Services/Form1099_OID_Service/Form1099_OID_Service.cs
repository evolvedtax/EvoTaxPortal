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
    public class Form1099_OID_Service : IForm1099_OID_Service
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly IInstituteService _instituteService;
        readonly IMailService _mailService;
        readonly ITrailAudit1099Service _trailAudit1099Service;
        readonly IMapper _mapper;
        public Form1099_OID_Service(EvolvedtaxContext evolvedtaxContext, IMapper mapper, IInstituteService instituteService, IMailService mailService = null, ITrailAudit1099Service trailAudit1099Service = null)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _mapper = mapper;
            _instituteService = instituteService;
            _mailService = mailService;
            _trailAudit1099Service = trailAudit1099Service;
        }

        public IQueryable<Form1099OIDResponse> GetForm1099OIDList()
        {
            var response = _evolvedtaxContext.Tbl1099_OID.Select(p => new Form1099OIDResponse
            {
                Id = p.Id,
                Corrected = p.Corrected,
                EntityId = p.EntityId,
                Address_Apt_Suite = p.Address_Apt_Suite,
                Address_Deliv_Street = p.Address_Deliv_Street,
                Address_Type = p.Address_Type,
                Batch_ID = p.Batch_ID,
                Box_12_State = p.Box_12_State,
                Box_13_StateID = p.Box_13_StateID,
                Box_10_Amount = p.Box_10_Amount,
                Box_11_Amount = p.Box_11_Amount,
                Box_1_Amount = p.Box_1_Amount,
                Box_2_Amount = p.Box_2_Amount,
                Box_3_Amount = p.Box_3_Amount,
                Box_4_Amount = p.Box_4_Amount,
                Box_5_Amount = p.Box_5_Amount,
                Box_6_Amount = p.Box_6_Amount,
                Box_8_Amount = p.Box_8_Amount,
                Box_9_Amount = p.Box_9_Amount,
                Box_14_Amount = p.Box_14_Amount,
                Box_7_All_Lines = p.Box_7_All_Lines,
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
                Name_Line2 = p.Name_Line2,
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
            var result = from ic in _evolvedtaxContext.Tbl1099_OID
                         where selectValues.Contains(ic.Id) && !string.IsNullOrEmpty(ic.Rcp_Email)
                         select new Tbl1099_OID
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
        public async Task<MessageResponseModel> Upload1099_OID_Data(IFormFile file, int InstId, int entityId, string UserId)
        {
            bool Status = false;
            var response = new List<Tbl1099_OID>();
            var OIDList = new List<Tbl1099_OID>();
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

                    var request = new Tbl1099_OID
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
                        Second_TIN_Notice = (excelRow.GetCell(columnMapping["2nd TIN Notice"])?.ToString() != null && (bool)excelRow.GetCell(columnMapping["2nd TIN Notice"])?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        FATCA_Checkbox = (excelRow.GetCell(columnMapping["FATCA Checkbox"])?.ToString() != null && (bool)excelRow.GetCell(columnMapping["FATCA Checkbox"])?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        Box_1_Amount = excelRow.GetCell(columnMapping["Box 1 Amount"])?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(columnMapping["Box 1 Amount"])?.ToString()) : 0,
                        Box_2_Amount = excelRow.GetCell(columnMapping["Box 2 Amount"])?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(columnMapping["Box 2 Amount"])?.ToString()) : 0,
                        Box_3_Amount = excelRow.GetCell(columnMapping["Box 3 Amount"])?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(columnMapping["Box 3 Amount"])?.ToString()) : 0,
                        Box_4_Amount = excelRow.GetCell(columnMapping["Box 4 Amount"])?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(columnMapping["Box 4 Amount"])?.ToString()) : 0,
                        Box_5_Amount = excelRow.GetCell(columnMapping["Box 5 Amount"])?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(columnMapping["Box 5 Amount"])?.ToString()) : 0,
                        Box_6_Amount = excelRow.GetCell(columnMapping["Box 6 Amount"])?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(columnMapping["Box 6 Amount"])?.ToString()) : 0,
                        Box_7_All_Lines = excelRow.GetCell(columnMapping["Box 7 All Lines"])?.ToString() ?? string.Empty,
                        Box_8_Amount = excelRow.GetCell(columnMapping["Box 8 Amount"])?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(columnMapping["Box 8 Amount"])?.ToString()) : 0,
                        Box_9_Amount = excelRow.GetCell(columnMapping["Box 9 Amount"])?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(columnMapping["Box 9 Amount"])?.ToString()) : 0,
                        Box_10_Amount = excelRow.GetCell(columnMapping["Box 10 Amount"])?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(columnMapping["Box 10 Amount"])?.ToString()) : 0,
                        Box_11_Amount = excelRow.GetCell(columnMapping["Box 11 Amount"])?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(columnMapping["Box 11 Amount"])?.ToString()) : 0,
                        Box_12_State = excelRow.GetCell(columnMapping["Box 12 State"])?.ToString() ?? string.Empty,
                        Box_13_StateID = excelRow.GetCell(columnMapping["Box 13 State ID"])?.ToString() ?? string.Empty,
                        Box_14_Amount = excelRow.GetCell(columnMapping["Box 14 Amount"])?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(columnMapping["Box 14 Amount"])?.ToString()) : 0,
                        Form_Category = excelRow.GetCell(columnMapping["Form Category"])?.ToString() ?? string.Empty,
                        Form_Source = excelRow.GetCell(columnMapping["Form Source"])?.ToString() ?? string.Empty,
                        Batch_ID = excelRow.GetCell(columnMapping["Batch ID"])?.ToString() ?? string.Empty,
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
                    if (await _evolvedtaxContext.Tbl1099_OID.AnyAsync(p => p.Rcp_TIN == request.Rcp_TIN && p.EntityId == request.EntityId && p.Created_Date != null &&
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
                    OIDList.Add(request);

                }
                await _evolvedtaxContext.Tbl1099_OID.AddRangeAsync(OIDList);
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
            var OIDresponse = _evolvedtaxContext.Tbl1099_OID.FirstOrDefault(p => p.Id == Id);
            var requestInstitue = _instituteService.GetPayeeData((int)OIDresponse.InstID);
            string templatefile = TemplatefilePath;
            string newFile1 = string.Empty;

            String ClientName = OIDresponse.First_Name + " " + OIDresponse.Name_Line2?.Replace(": ", "");

            if (!String.IsNullOrEmpty(Page))
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099OID, "_", OIDresponse.Id, "_Page_", Page);
            }
            else
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099OID, "_", OIDresponse.Id);
            }


            string FilenameNew = "/Form1099OID/" + newFile1 + ".pdf";
            string newFileName = newFile1 + ".pdf"; // Add ".pdf" extension to the file name

            string newFilePath = Path.Combine(SaveFolderPath, newFileName);

            PdfReader pdfReader = new PdfReader(templatefile);
            PdfReader.unethicalreading = true;
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFilePath, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;

            string Recepient_CountryCode = "";
            if (OIDresponse.Country != "United States")
            {
                var country = _evolvedtaxContext.MstrCountries.FirstOrDefault(c => c.Country == OIDresponse.Country);
                if (country != null)
                {
                    Recepient_CountryCode = country.CountryId;
                }
            }

            string RecipentCity = string.Join(", ",
               new[]
               {
                    OIDresponse.City,
                    OIDresponse.State,
                    string.IsNullOrWhiteSpace(OIDresponse.Province) ? null : OIDresponse.Province,
                     string.IsNullOrWhiteSpace(Recepient_CountryCode) ? null : Recepient_CountryCode,
                    OIDresponse.Zip,
                    string.IsNullOrWhiteSpace(OIDresponse.PostalCode) ? null : OIDresponse.PostalCode

               }.Where(s => !string.IsNullOrWhiteSpace(s))
           );

            String RecipentAddress = string.Concat(OIDresponse.Address_Deliv_Street, ", ", OIDresponse.Address_Apt_Suite);
            string currentYear = Convert.ToString(DateTime.Now.Year % 100);

            #region PDF Columns
            pdfFormFields.SetField("topmostSubform.CopyA.CopyAHeader.CalendarYear.f2_1", currentYear);   //23
            if (OIDresponse.Corrected == "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyA.CopyAHeader.c1_1", "0");   //Void
            }
            else
            {
                pdfFormFields.SetField("efield2_topmostSubform.CopyA.CopyAHeader.c1_1", "0");   //Corrected
            }
            pdfFormFields.SetField("topmostSubform.CopyA.LeftCol.f1_02", requestInstitue.PayeeData);   //PayData
            pdfFormFields.SetField("topmostSubform.CopyA.LeftCol.f1_03", requestInstitue.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("topmostSubform.CopyA.LeftCol.f1_04", OIDresponse.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("topmostSubform.CopyA.LeftCol.f1_05", OIDresponse.First_Name + " " + OIDresponse.Name_Line2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("topmostSubform.CopyA.LeftCol.f1_06", OIDresponse.Address_Deliv_Street + " " + OIDresponse.Address_Apt_Suite);   //RecipentAddress
            pdfFormFields.SetField("topmostSubform.CopyA.LeftCol.f1_07", OIDresponse.City + " " + OIDresponse.State + " " + OIDresponse.Country + " " + OIDresponse.Zip);   //RecipentCity  
            if (OIDresponse.FATCA_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyA.LeftCol.c1_3", "0");   //FATCA_Checkbox
            }
            pdfFormFields.SetField("topmostSubform.CopyA.LeftCol.f1_08", OIDresponse.Rcp_Account);   //request.Rcp_Account
            if (OIDresponse.Second_TIN_Notice == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyA.LeftCol.c1_4", "0");   //Second_TIN_Notice
            }
            pdfFormFields.SetField("topmostSubform.CopyA.RightCol.f1_09", OIDresponse.Box_1_Amount.ToString());   //Box_1_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RightCol.f1_10", OIDresponse.Box_2_Amount.ToString());   //Box_2_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RightCol.Box3_ReadOrder.f1_11", OIDresponse.Box_3_Amount.ToString());   //Box_3_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RightCol.f1_12", OIDresponse.Box_4_Amount.ToString());   //Box_4_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RightCol.Box5_ReadOrder.f1_13", OIDresponse.Box_5_Amount.ToString());   //Box_5_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RightCol.f1_14", OIDresponse.Box_6_Amount.ToString());   //Box_6_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RightCol.f1_15", OIDresponse.Box_7_All_Lines);   //Box_7_All_Lines
            pdfFormFields.SetField("topmostSubform.CopyA.RightCol.Box8_ReadOrder.f1_16", OIDresponse.Box_8_Amount.ToString());   //Box_8_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RightCol.f1_17", OIDresponse.Box_9_Amount.ToString());   //Box_9_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RightCol.Box10_ReadOrder.f1_18", OIDresponse.Box_10_Amount.ToString());   //Box_10_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RightCol.f1_19", OIDresponse.Box_11_Amount.ToString());   //Box_11_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RightCol.Box12.f1_20", OIDresponse.Box_12_State);   //Box_12_State
            pdfFormFields.SetField("topmostSubform.CopyA.RightCol.Box12.f1_21", OIDresponse.Box_12_State);   //Box_12_State
            pdfFormFields.SetField("topmostSubform.CopyA.RightCol.Box13.f1_22", OIDresponse.Box_13_StateID);   //Box_13_StateID
            pdfFormFields.SetField("topmostSubform.CopyA.RightCol.Box13.f1_23", OIDresponse.Box_13_StateID);   //Box_13_StateID
            pdfFormFields.SetField("topmostSubform.CopyA.RightCol.f1_24", OIDresponse.Box_14_Amount.ToString());   //Box_14_Amount
            pdfFormFields.SetField("topmostSubform.CopyA.RightCol.f1_25", OIDresponse.Box_14_Amount.ToString());   //Box_14_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.Copy1Header.CalendarYear.f2_01", currentYear);   //23
            if (OIDresponse.Corrected == "1")
            {
                pdfFormFields.SetField("topmostSubform.Copy1.Copy1Header.c2_1", "0");   //Void
            }
            else
            {
                pdfFormFields.SetField("efield31_topmostSubform.Copy1.Copy1Header.c2_1", "0");   //Corrected
            }
            pdfFormFields.SetField("topmostSubform.Copy1.LeftCol.f2_02", requestInstitue.PayeeData);   //PayData
            pdfFormFields.SetField("topmostSubform.Copy1.LeftCol.f2_03", requestInstitue.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("topmostSubform.Copy1.LeftCol.f2_04", OIDresponse.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("topmostSubform.Copy1.LeftCol.f2_05", OIDresponse.First_Name + " " + OIDresponse.Name_Line2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("topmostSubform.Copy1.LeftCol.f2_06", OIDresponse.Address_Deliv_Street + " " + OIDresponse.Address_Apt_Suite);   //RecipentAddress
            pdfFormFields.SetField("topmostSubform.Copy1.LeftCol.f2_07", OIDresponse.City + " " + OIDresponse.State + " " + OIDresponse.Country + " " + OIDresponse.Zip);   //RecipentCity
            if (OIDresponse.FATCA_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.Copy1.LeftCol.c2_3", "0");   //FATCA_Checkbox
            }
            pdfFormFields.SetField("topmostSubform.Copy1.LeftCol.f2_08", OIDresponse.Rcp_Account);   //request.Rcp_Account
            pdfFormFields.SetField("topmostSubform.Copy1.RightCol.f2_09", OIDresponse.Box_1_Amount.ToString());   //Box_1_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RightCol.f2_10", OIDresponse.Box_2_Amount.ToString());   //Box_2_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RightCol.Box3_ReadOrder.f2_11", OIDresponse.Box_3_Amount.ToString());   //Box_3_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RightCol.f2_12", OIDresponse.Box_4_Amount.ToString());   //Box_4_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RightCol.Box5_ReadOrder.f2_13", OIDresponse.Box_5_Amount.ToString());   //Box_5_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RightCol.f2_14", OIDresponse.Box_6_Amount.ToString());   //Box_6_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RightCol.f2_15", OIDresponse.Box_7_All_Lines);   //Box_7_All_Lines
            pdfFormFields.SetField("topmostSubform.Copy1.RightCol.Box8_ReadOrder.f2_16", OIDresponse.Box_8_Amount.ToString());   //Box_8_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RightCol.f2_17", OIDresponse.Box_9_Amount.ToString());   //Box_9_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RightCol.Box10_ReadOrder.f2_18", OIDresponse.Box_10_Amount.ToString());   //Box_10_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RightCol.f2_19", OIDresponse.Box_11_Amount.ToString());   //Box_11_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RightCol.Box12.f2_20", OIDresponse.Box_12_State);   //Box_12_State
            pdfFormFields.SetField("topmostSubform.Copy1.RightCol.Box12.f2_21", OIDresponse.Box_12_State);   //Box_12_State
            pdfFormFields.SetField("topmostSubform.Copy1.RightCol.Box13.f2_22", OIDresponse.Box_13_StateID);   //Box_13_StateID
            pdfFormFields.SetField("topmostSubform.Copy1.RightCol.Box13.f2_23", OIDresponse.Box_13_StateID);   //Box_13_StateID
            pdfFormFields.SetField("topmostSubform.Copy1.RightCol.f2_24", OIDresponse.Box_14_Amount.ToString());   //Box_14_Amount
            pdfFormFields.SetField("topmostSubform.Copy1.RightCol.f2_25", OIDresponse.Box_14_Amount.ToString());   //Box_14_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.CopyBHeader.CalendarYear.f2_01", currentYear);   //23
            pdfFormFields.SetField("topmostSubform.CopyB.CopyBHeader.c2_1", "Corrected");   //Corrected
            pdfFormFields.SetField("topmostSubform.CopyB.LeftCol.f2_02", requestInstitue.PayeeData);   //PayData
            pdfFormFields.SetField("topmostSubform.CopyB.LeftCol.f2_03", requestInstitue.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("topmostSubform.CopyB.LeftCol.f2_04", OIDresponse.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("topmostSubform.CopyB.LeftCol.f2_05", OIDresponse.First_Name + " " + OIDresponse.Name_Line2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("topmostSubform.CopyB.LeftCol.f2_06", OIDresponse.Address_Deliv_Street + " " + OIDresponse.Address_Apt_Suite);   //RecipentAddress
            pdfFormFields.SetField("topmostSubform.CopyB.LeftCol.f2_07", OIDresponse.City + " " + OIDresponse.State + " " + OIDresponse.Country + " " + OIDresponse.Zip);   //RecipentCity
            if (OIDresponse.FATCA_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyB.LeftCol.c2_3", "0");   //FATCA_Checkbox
            }
            pdfFormFields.SetField("topmostSubform.CopyB.LeftCol.f2_08", OIDresponse.Rcp_Account);   //request.Rcp_Account
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_09", OIDresponse.Box_1_Amount.ToString());   //Box_1_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_10", OIDresponse.Box_2_Amount.ToString());   //Box_2_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.Box3_ReadOrder.f2_11", OIDresponse.Box_3_Amount.ToString());   //Box_3_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_12", OIDresponse.Box_4_Amount.ToString());   //Box_4_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.Box5_ReadOrder.f2_13", OIDresponse.Box_5_Amount.ToString());   //Box_5_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_14", OIDresponse.Box_6_Amount.ToString());   //Box_6_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_15", OIDresponse.Box_7_All_Lines);   //Box_7_All_Lines
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.Box8_ReadOrder.f2_16", OIDresponse.Box_8_Amount.ToString());   //Box_8_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_17", OIDresponse.Box_9_Amount.ToString());   //Box_9_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.Box10_ReadOrder.f2_18", OIDresponse.Box_10_Amount.ToString());   //Box_10_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_19", OIDresponse.Box_11_Amount.ToString());   //Box_11_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.Box12.f2_20", OIDresponse.Box_12_State);   //Box_12_State
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.Box12.f2_21", OIDresponse.Box_12_State);   //Box_12_State
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.Box13.f2_22", OIDresponse.Box_13_StateID);   //Box_13_StateID
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.Box13.f2_23", OIDresponse.Box_13_StateID);   //Box_13_StateID
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_24", OIDresponse.Box_14_Amount.ToString());   //Box_14_Amount
            pdfFormFields.SetField("topmostSubform.CopyB.RightCol.f2_25", OIDresponse.Box_14_Amount.ToString());   //Box_14_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.Copy2Header.CalendarYear.f2_01", currentYear);   //23
            pdfFormFields.SetField("topmostSubform.Copy2.Copy2Header.c2_1", "Corrected");   //Corrected
            pdfFormFields.SetField("topmostSubform.Copy2.LeftCol.f2_02", requestInstitue.PayeeData);   //PayData
            pdfFormFields.SetField("topmostSubform.Copy2.LeftCol.f2_03", requestInstitue.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("topmostSubform.Copy2.LeftCol.f2_04", OIDresponse.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("topmostSubform.Copy2.LeftCol.f2_05", OIDresponse.First_Name + " " + OIDresponse.Name_Line2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("topmostSubform.Copy2.LeftCol.f2_06", OIDresponse.Address_Deliv_Street + " " + OIDresponse.Address_Apt_Suite);   //RecipentAddress
            pdfFormFields.SetField("topmostSubform.Copy2.LeftCol.f2_07", OIDresponse.City + " " + OIDresponse.State + " " + OIDresponse.Country + " " + OIDresponse.Zip);   //RecipentCity
            if (OIDresponse.FATCA_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.Copy2.LeftCol.c2_3", "0");   //FATCA_Checkbox
            }
            pdfFormFields.SetField("topmostSubform.Copy2.LeftCol.f2_08", OIDresponse.Rcp_Account);   //request.Rcp_Account
            pdfFormFields.SetField("topmostSubform.Copy2.RightCol.f2_09", OIDresponse.Box_1_Amount.ToString());   //Box_1_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RightCol.f2_10", OIDresponse.Box_2_Amount.ToString());   //Box_2_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RightCol.Box3_ReadOrder.f2_11", OIDresponse.Box_3_Amount.ToString());   //Box_3_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RightCol.f2_12", OIDresponse.Box_4_Amount.ToString());   //Box_4_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RightCol.Box5_ReadOrder.f2_13", OIDresponse.Box_5_Amount.ToString());   //Box_5_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RightCol.f2_14", OIDresponse.Box_6_Amount.ToString());   //Box_6_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RightCol.f2_15", OIDresponse.Box_7_All_Lines);   //Box_7_All_Lines
            pdfFormFields.SetField("topmostSubform.Copy2.RightCol.Box8.f2_16", OIDresponse.Box_8_Amount.ToString());   //Box_8_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RightCol.f2_17", OIDresponse.Box_9_Amount.ToString());   //Box_9_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RightCol.Box10_ReadOrder.f2_18", OIDresponse.Box_10_Amount.ToString());   //Box_10_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RightCol.f2_19", OIDresponse.Box_11_Amount.ToString());   //Box_11_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RightCol.Box12.f2_20", OIDresponse.Box_12_State);   //Box_12_State
            pdfFormFields.SetField("topmostSubform.Copy2.RightCol.Box12.f2_21", OIDresponse.Box_12_State);   //Box_12_State
            pdfFormFields.SetField("topmostSubform.Copy2.RightCol.Box13.f2_22", OIDresponse.Box_13_StateID);   //Box_13_StateID
            pdfFormFields.SetField("topmostSubform.Copy2.RightCol.Box13.f2_23", OIDresponse.Box_13_StateID);   //Box_13_StateID
            pdfFormFields.SetField("topmostSubform.Copy2.RightCol.f2_24", OIDresponse.Box_14_Amount.ToString());   //Box_14_Amount
            pdfFormFields.SetField("topmostSubform.Copy2.RightCol.f2_25", OIDresponse.Box_14_Amount.ToString());   //Box_14_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.CopyCHeader.CalendarYear.f2_01", currentYear);   //23
            if (OIDresponse.Corrected == "1")
            {
                pdfFormFields.SetField("topmostSubform.CopyC.CopyCHeader.c2_1", "0");   //Void
            }
            else
            {
                pdfFormFields.SetField("efield113_topmostSubform.CopyC.CopyCHeader.c2_1", "0");   //Corrected
            }
            pdfFormFields.SetField("topmostSubform.CopyC.LeftCol.f2_02", requestInstitue.PayeeData);   //PayData
            pdfFormFields.SetField("topmostSubform.CopyC.LeftCol.f2_03", requestInstitue.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("topmostSubform.CopyC.LeftCol.f2_04", OIDresponse.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("topmostSubform.CopyC.LeftCol.f2_05", OIDresponse.First_Name + " " + OIDresponse.Name_Line2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("topmostSubform.CopyC.LeftCol.f2_06", OIDresponse.Address_Deliv_Street + " " + OIDresponse.Address_Apt_Suite);   //RecipentAddress
            pdfFormFields.SetField("topmostSubform.CopyC.LeftCol.f2_07", OIDresponse.City + " " + OIDresponse.State + " " + OIDresponse.Country + " " + OIDresponse.Zip);   //RecipentCity
            if (OIDresponse.FATCA_Checkbox == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyC.LeftCol.c2_3", "0");   //FATCA_Checkbox
            }
            pdfFormFields.SetField("topmostSubform.CopyC.LeftCol.f2_08", OIDresponse.Rcp_Account);   //request.Rcp_Account
            if (OIDresponse.Second_TIN_Notice == "0")
            {
                pdfFormFields.SetField("topmostSubform.CopyC.LeftCol.c2_4", "0");   //Second_TIN_Notice
            }
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_09", OIDresponse.Box_1_Amount.ToString());   //Box_1_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_10", OIDresponse.Box_2_Amount.ToString());   //Box_2_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.Box3_ReadOrder.f2_11", OIDresponse.Box_3_Amount.ToString());   //Box_3_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_12", OIDresponse.Box_4_Amount.ToString());   //Box_4_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.Box5_ReadOrder.f2_13", OIDresponse.Box_5_Amount.ToString());   //Box_5_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_14", OIDresponse.Box_6_Amount.ToString());   //Box_6_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_15", OIDresponse.Box_7_All_Lines);   //Box_7_All_Lines
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.Bow8_ReadOrder.f2_16", OIDresponse.Box_8_Amount.ToString());   //Box_8_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_17", OIDresponse.Box_9_Amount.ToString());   //Box_9_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.Box10_ReadOrder.f2_18", OIDresponse.Box_10_Amount.ToString());   //Box_10_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_19", OIDresponse.Box_11_Amount.ToString());   //Box_11_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.Box12_ReadOrder.f2_20", OIDresponse.Box_12_State);   //Box_12_State
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.Box12_ReadOrder.f2_21", OIDresponse.Box_12_State);   //Box_12_State
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.Box13_ReadOrder.f2_22", OIDresponse.Box_13_StateID);   //Box_13_StateID
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.Box13_ReadOrder.f2_23", OIDresponse.Box_13_StateID);   //Box_13_StateID
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_24", OIDresponse.Box_14_Amount.ToString());   //Box_14_Amount
            pdfFormFields.SetField("topmostSubform.CopyC.RightCol.f2_25", OIDresponse.Box_14_Amount.ToString());   //Box_14_Amount
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
            var request = _evolvedtaxContext.Tbl1099_OID.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.First_Name + " " + request.Name_Line2?.Replace(": ", "");
            newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099OID, "_", Id);
            string FilenameNew = "/Form1099OID/" + newFile1 + ".pdf";
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
            var request = _evolvedtaxContext.Tbl1099_OID.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.First_Name + " " + request.Name_Line2?.Replace(": ", "");

            newFile1 = string.Concat(ClientName, "_", AppConstants.Form1099OID, "_", request.Id, "_Page_", selectedPage);
            string FilenameNew = "/Form1099OID/" + newFile1 + ".pdf";
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
            string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.Form1099OIDTemplateFileName);
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
                            compileFileName = "To be filed with recipient’s state income tax return, when required.pdf";
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
        public string GenerateAndZipPdfs(List<int> ids, string SaveFolderPath, List<string> selectedPages, string RootPath)
        {
            var pdfPaths = new List<string>();

            foreach (var id in ids)
            {

                string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.Form1099OIDTemplateFileName);
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

            var recordToDelete = _evolvedtaxContext.Tbl1099_OID.First(p => p.Id == id);
            if (recordToDelete != null)
            {
                _evolvedtaxContext.Tbl1099_OID.Remove(recordToDelete);
                await _evolvedtaxContext.SaveChangesAsync();
                return new MessageResponseModel { Status = true };
            }

            return new MessageResponseModel { Status = false };
        }

        public async Task<MessageResponseModel> KeepRecord(int id)
        {

            var recordToUpdate = _evolvedtaxContext.Tbl1099_OID.First(p => p.Id == id);
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
