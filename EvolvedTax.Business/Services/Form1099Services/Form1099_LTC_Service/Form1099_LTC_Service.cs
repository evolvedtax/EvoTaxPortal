﻿using EvolvedTax.Data.Models.Entities;
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
using NPOI.SS.Formula;
using EvolvedTax.Business.Services.InstituteService;
using Azure;

namespace EvolvedTax.Business.Services.Form1099Services
{
    public class Form1099_LTC_Service : IForm1099_LTC_Service
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly IMailService _mailService;
        readonly IInstituteService _instituteService;
        readonly ITrailAudit1099Service _trailAudit1099Service;


        public Form1099_LTC_Service(EvolvedtaxContext evolvedtaxContext, IMailService mailService, ITrailAudit1099Service trailAudit1099Service, IInstituteService instituteService)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _mailService = mailService;
            _trailAudit1099Service = trailAudit1099Service;
            _instituteService = instituteService;
        }
        public async Task<MessageResponseModel> Upload1099_Data(IFormFile file, int entityId, int InstId, string UserId)
        {
            bool Status = false;
            var response = new List<Tbl1099_LTC>();
            var List = new List<Tbl1099_LTC>();
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

                    string cell_value_15 = excelRow.GetCell(columnMapping["Box 3 Checkbox 1"])?.ToString();
                    string Box_3_Checkbox1 = string.IsNullOrEmpty(cell_value_15) ? "0" : (cell_value_15.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    string cell_value_16 = excelRow.GetCell(columnMapping["Box 3 Checkbox 2"])?.ToString();
                    string Box_3_Checkbox2 = string.IsNullOrEmpty(cell_value_16) ? "0" : (cell_value_16.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    string cell_value_23 = excelRow.GetCell(columnMapping["Box 4 Checkbox"])?.ToString();
                    string Box_4_Checkbox = string.IsNullOrEmpty(cell_value_23) ? "0" : (cell_value_23.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    string cell_value_24 = excelRow.GetCell(columnMapping["Box 5 Checkbox 1"])?.ToString();
                    string Box_5_Checkbox1 = string.IsNullOrEmpty(cell_value_24) ? "0" : (cell_value_24.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    string cell_value_25 = excelRow.GetCell(columnMapping["Box 5 Checkbox 2"])?.ToString();
                    string Box_5_Checkbox2 = string.IsNullOrEmpty(cell_value_25) ? "0" : (cell_value_25.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");


                    string cell_value_30 = excelRow.GetCell(columnMapping["Is Corrected Form of 1099"])?.ToString();
                    string Corrected = string.IsNullOrEmpty(cell_value_30) ? "0" : (cell_value_30.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    var entity = new Tbl1099_LTC
                    {
                        Rcp_TIN = excelRow.GetCell(columnMapping["Rcp TIN"])?.ToString(),
                        Last_Name_Company = excelRow.GetCell(columnMapping["Company"])?.ToString(),
                        First_Name = excelRow.GetCell(columnMapping["First Name"])?.ToString(),
                        Name_Line2 = excelRow.GetCell(columnMapping["Last Name"])?.ToString(),
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
                        Box_1_Amount = TryConvertToDecimal(excelRow.GetCell(columnMapping["Box 1 Amount"])),
                        Box_2_Amount = TryConvertToDecimal(excelRow.GetCell(columnMapping["Box 2 Amount"])),
                        Box_3_Checkbox1 = Box_3_Checkbox1,
                        Box_3_Checkbox2 = Box_3_Checkbox2,
                        Box_INS_SSN = excelRow.GetCell(columnMapping["Box INS SSN"])?.ToString(),
                        Box_INS_Name = excelRow.GetCell(columnMapping["Box INS Name"])?.ToString(),
                        Box_INS_Address = excelRow.GetCell(columnMapping["Box INS Address"])?.ToString(),
                        Box_INS_City = excelRow.GetCell(columnMapping["Box INS City"])?.ToString(),
                        Box_INS_State = excelRow.GetCell(columnMapping["Box INS State"])?.ToString(),
                        Box_INS_Zip = excelRow.GetCell(columnMapping["Box INS Zip"])?.ToString(),
                        Box_4_Checkbox = Box_4_Checkbox,
                        Box_5_Checkbox1 = Box_5_Checkbox1,
                        Box_5_Checkbox2 = Box_5_Checkbox2,
                        Box_5_Date = !string.IsNullOrWhiteSpace(excelRow.GetCell(columnMapping["Box 5 Date"])?.ToString()) ? (DateTime?)Convert.ToDateTime(excelRow.GetCell(columnMapping["Box 5 Date"])?.ToString()) : null,
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
                    if (await _evolvedtaxContext.Tbl1099_LTC.AnyAsync(p =>
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


                    await _evolvedtaxContext.Tbl1099_LTC.AddRangeAsync(List);
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
            var request = _evolvedtaxContext.Tbl1099_LTC.FirstOrDefault(p => p.Id == Id);
            var requestInstitue = _instituteService.GetPayeeData((int)request.InstID);
            string templatefile = TemplatefilePath;
            string newFile1 = string.Empty;

            String ClientName = request.First_Name + " " + request.Name_Line2?.Replace(": ", "");

            if (!String.IsNullOrEmpty(Page))
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.LTC1099Form, "_", request.Id, "_Page_", Page);
            }
            else
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.LTC1099Form, "_", request.Id);
            }


            string FilenameNew = "/1099LTC/" + newFile1 + ".pdf";
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

            pdfFormFields.SetField("Form1099-LTC.CopyA.FormHeader.CalendarYear.f1_1", currentYear);   //23
        

            if (request.Corrected.ToString() == "1")
            {
                pdfFormFields.SetField("Form1099-LTC.CopyA.FormHeader.c1_1", "0");   //PageAVoid
            }
            if (request.Corrected.ToString() == "0")
            {
                pdfFormFields.SetField("efield2_Form1099-LTC.CopyA.FormHeader.c1_1", "0");   //PageACorrected
            }



            pdfFormFields.SetField("Form1099-LTC.CopyA.LeftCol.f1_2", requestInstitue.PayeeData);   //PayData
            pdfFormFields.SetField("Form1099-LTC.CopyA.LeftCol.f1_3", requestInstitue.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("Form1099-LTC.CopyA.LeftCol.f1_4", request.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("Form1099-LTC.CopyA.LeftCol.f1_5", request.First_Name + " " + request.Name_Line2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("Form1099-LTC.CopyA.LeftCol.f1_6", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("Form1099-LTC.CopyA.LeftCol.f1_7", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("Form1099-LTC.CopyA.LeftCol.f1_8", request.Rcp_Account);   //request.Rcp_Account
            pdfFormFields.SetField("Form1099-LTC.CopyA.RightCol.f1_9", request.Box_1_Amount.HasValue ? request.Box_1_Amount.Value.ToString() : string.Empty);   //Box_1_Amount
            pdfFormFields.SetField("Form1099-LTC.CopyA.RightCol.f1_10", request.Box_2_Amount.HasValue ? request.Box_2_Amount.Value.ToString() : string.Empty);   //Box_2_Amount
            if (request.Box_3_Checkbox2 != "1")
            {
                pdfFormFields.SetField("Form1099-LTC.CopyA.RightCol.c1_2", "1");   //PageAFATCA

            }

            if (request.Box_3_Checkbox1 != "1")
            {
                pdfFormFields.SetField("efield13_Form1099-LTC.CopyA.RightCol.c1_2", "1");   //PageAFATCA

            }
            pdfFormFields.SetField("Form1099-LTC.CopyA.RightCol.f1_11", request.Box_INS_SSN);   //Box_INS_SSN
            pdfFormFields.SetField("Form1099-LTC.CopyA.RightCol.f1_12", request.Box_INS_Name);   //Box_INS_Name
            pdfFormFields.SetField("Form1099-LTC.CopyA.RightCol.f1_13", request.Box_INS_Address);   //Box_INS_Address
            pdfFormFields.SetField("Form1099-LTC.CopyA.RightCol.f1_14", request.Box_INS_City + " " + request.Box_INS_State + " " + request.Box_INS_Zip);   //Box_INS_City & " " & Box_INS_State & " " & Box_INS_Zip
            if (request.Box_4_Checkbox != "1")
            {
                pdfFormFields.SetField("Form1099-LTC.CopyA.RightCol.c1_3", "1");   //PageAFATCA

            }
            if (request.Box_5_Checkbox1 != "1")
            {
                pdfFormFields.SetField("Form1099-LTC.CopyA.RightCol.c1_4", "1");   //PageAFATCA

            }
            if (request.Box_5_Checkbox2 != "1")
            {
                pdfFormFields.SetField("Form1099-LTC.CopyA.RightCol.c1_5", "1");   //PageAFATCA

            }
            if (!string.IsNullOrEmpty(request.Box_5_Date?.ToString()))
            {
                DateTime Box_5_Date;
                if (DateTime.TryParse(request.Box_5_Date?.ToString(), out Box_5_Date))
                {
                    pdfFormFields.SetField("Form1099-LTC.CopyA.RightCol.f1_15", Box_5_Date.ToString("MM/dd/yyyy"));
                }

            }

            #endregion


            #region Page 2

            pdfFormFields.SetField("Form1099-LTC.CopyB.FormHeader.CalendarYear.f1_1", currentYear);   //23

            if (request.Corrected != "1")
            {
                pdfFormFields.SetField("Form1099-LTC.CopyB.FormHeader.c1_1", "1");   //PageAFATCA

            }

            pdfFormFields.SetField("Form1099-LTC.CopyB.LeftCol.f1_2", requestInstitue.PayeeData);   //PayData
            pdfFormFields.SetField("Form1099-LTC.CopyB.LeftCol.f1_3", requestInstitue.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("Form1099-LTC.CopyB.LeftCol.f1_4", request.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("Form1099-LTC.CopyB.LeftCol.f1_5", request.First_Name + " " + request.Name_Line2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("Form1099-LTC.CopyB.LeftCol.f1_6", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("Form1099-LTC.CopyB.LeftCol.f1_7", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("Form1099-LTC.CopyB.LeftCol.f1_8", request.Rcp_Account);   //request.Rcp_Account
            pdfFormFields.SetField("Form1099-LTC.CopyB.RightCol.f1_9", request.Box_1_Amount.HasValue ? request.Box_1_Amount.Value.ToString() : string.Empty);   //Box_1_Amount
            pdfFormFields.SetField("Form1099-LTC.CopyB.RightCol.f1_10", request.Box_2_Amount.HasValue ? request.Box_2_Amount.Value.ToString() : string.Empty);   //Box_2_Amount
            if (request.Box_3_Checkbox1 != "1")
            {
                pdfFormFields.SetField("Form1099-LTC.CopyB.RightCol.c1_2", "1");   //PageAFATCA

            }
            if (request.Box_3_Checkbox1 != "1")
            {
                pdfFormFields.SetField("efield34_Form1099-LTC.CopyB.RightCol.c1_2", "1");   //PageAFATCA

            }

            pdfFormFields.SetField("Form1099-LTC.CopyB.RightCol.f1_11", request.Box_INS_SSN);   //Box_INS_SSN
            pdfFormFields.SetField("Form1099-LTC.CopyB.RightCol.f1_12", request.Box_INS_Name);   //Box_INS_Name
            pdfFormFields.SetField("Form1099-LTC.CopyB.RightCol.f1_13", request.Box_INS_Address);   //Box_INS_Address
            pdfFormFields.SetField("Form1099-LTC.CopyB.RightCol.f1_14", request.Box_INS_City + " " + request.Box_INS_State + " " +request.Box_INS_Zip);   //Box_INS_City & " " & Box_INS_State & " " & Box_INS_Zip
            if (request.Box_4_Checkbox != "1")
            {
                pdfFormFields.SetField("Form1099-LTC.CopyB.RightCol.c1_3", "1");   //PageAFATCA

            }
            if (request.Box_5_Checkbox1 != "1")
            {
                pdfFormFields.SetField("Form1099-LTC.CopyB.RightCol.c1_4", "1");   //PageAFATCA

            }

            if (request.Box_5_Checkbox2 != "1")
            {
                pdfFormFields.SetField("Form1099-LTC.CopyB.RightCol.c1_5", "1");   //PageAFATCA

            }

            if (!string.IsNullOrEmpty(request.Box_5_Date?.ToString()))
            {
                DateTime Box_5_Date;
                if (DateTime.TryParse(request.Box_5_Date?.ToString(), out Box_5_Date))
                {
                    pdfFormFields.SetField("Form1099-LTC.CopyB.RightCol.f1_15", Box_5_Date.ToString("MM/dd/yyyy"));
                }

            }

            #endregion


            #region Page 3
            pdfFormFields.SetField("Form1099-LTC.CopyC.FormHeader.CalendarYear.f1_1", currentYear);   //23
            if (request.Corrected != "1")
            {
                pdfFormFields.SetField("Form1099-LTC.CopyC.FormHeader.c1_1", "1");   //PageAFATCA

            }

            pdfFormFields.SetField("Form1099-LTC.CopyC.LeftCol.f1_2", requestInstitue.PayeeData);   //PayData
            pdfFormFields.SetField("Form1099-LTC.CopyC.LeftCol.f1_3", requestInstitue.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("Form1099-LTC.CopyC.LeftCol.f1_4", request.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("Form1099-LTC.CopyC.LeftCol.f1_5", request.First_Name + " " + request.Name_Line2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("Form1099-LTC.CopyC.LeftCol.f1_6", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("Form1099-LTC.CopyC.LeftCol.f1_7", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("Form1099-LTC.CopyC.LeftCol.f1_8", request.Rcp_Account);   //request.Rcp_Account
            pdfFormFields.SetField("Form1099-LTC.CopyC.RightCol.f1_9", request.Box_1_Amount.HasValue ? request.Box_1_Amount.Value.ToString() : string.Empty);   //Box_1_Amount
            pdfFormFields.SetField("Form1099-LTC.CopyC.RightCol.f1_10", request.Box_2_Amount.HasValue ? request.Box_2_Amount.Value.ToString() : string.Empty);   //Box_2_Amount

            if (request.Box_3_Checkbox2 != "1")
            {
                pdfFormFields.SetField("Form1099-LTC.CopyC.RightCol.c1_2", "1");   //PageAFATCA

            }
            if (request.Box_3_Checkbox1 != "1")
            {
                pdfFormFields.SetField("efield55_Form1099-LTC.CopyC.RightCol.c1_2", "1");   //PageAFATCA

            }

            pdfFormFields.SetField("Form1099-LTC.CopyC.RightCol.f1_11", request.Box_INS_SSN);   //Box_INS_SSN
            pdfFormFields.SetField("Form1099-LTC.CopyC.RightCol.f1_12", request.Box_INS_Name);   //Box_INS_Name
            pdfFormFields.SetField("Form1099-LTC.CopyC.RightCol.f1_13", request.Box_INS_Address);   //Box_INS_Address
            pdfFormFields.SetField("Form1099-LTC.CopyC.RightCol.f1_14", request.Box_INS_City + " " + request.Box_INS_State + " " + request.Box_INS_Zip);   //Box_INS_City & " " & Box_INS_State & " " & Box_INS_Zip
            if (request.Box_4_Checkbox != "1")
            {
                pdfFormFields.SetField("Form1099-LTC.CopyC.RightCol.c1_3", "1");   //PageAFATCA

            }
            if (request.Box_5_Checkbox1 != "1")
            {
                pdfFormFields.SetField("Form1099-LTC.CopyC.RightCol.c1_4", "1");   //PageAFATCA

            }
            if (request.Box_5_Checkbox2 != "1")
            {
                pdfFormFields.SetField("Form1099-LTC.CopyC.RightCol.c1_5", "1");   //PageAFATCA

            }


            if (!string.IsNullOrEmpty(request.Box_5_Date?.ToString()))
            {
                DateTime Box_5_Date;
                if (DateTime.TryParse(request.Box_5_Date?.ToString(), out Box_5_Date))
                {
                    pdfFormFields.SetField("Form1099-LTC.CopyC.RightCol.f1_15", Box_5_Date.ToString("MM/dd/yyyy"));
                }

            }

            #endregion


            #region Page 4
            pdfFormFields.SetField("Form1099-LTC.CopyD.FormHeader.CalendarYear.f1_1", currentYear);   //23
            if (request.Corrected.ToString() == "1")
            {
                pdfFormFields.SetField("Form1099-LTC.CopyD.FormHeader.c1_1", "0");   //PageAVoid
            }
            if (request.Corrected.ToString() == "0")
            {
                pdfFormFields.SetField("efield66_Form1099-LTC.CopyD.FormHeader.c1_1", "0");   //PageACorrected
            }

            pdfFormFields.SetField("Form1099-LTC.CopyD.LeftCol.f1_2", requestInstitue.PayeeData);   //PayData
            pdfFormFields.SetField("Form1099-LTC.CopyD.LeftCol.f1_3", requestInstitue.Idnumber);   //requestInstitue.Idnumber
            pdfFormFields.SetField("Form1099-LTC.CopyD.LeftCol.f1_4", request.Rcp_TIN);   //Rcp_TIN
            pdfFormFields.SetField("Form1099-LTC.CopyD.LeftCol.f1_5", request.First_Name + " " + request.Name_Line2);   //request.First_Name + " " + request.Name_Line2
            pdfFormFields.SetField("Form1099-LTC.CopyD.LeftCol.f1_6", RecipentAddress);   //RecipentAddress
            pdfFormFields.SetField("Form1099-LTC.CopyD.LeftCol.f1_7", RecipentCity);   //RecipentCity
            pdfFormFields.SetField("Form1099-LTC.CopyD.LeftCol.f1_8", request.Rcp_Account);   //request.Rcp_Account
            pdfFormFields.SetField("Form1099-LTC.CopyD.RightCol.f1_9", request.Box_1_Amount.HasValue ? request.Box_1_Amount.Value.ToString() : string.Empty);   //Box_1_Amount
            pdfFormFields.SetField("Form1099-LTC.CopyD.RightCol.f1_10", request.Box_2_Amount.HasValue ? request.Box_2_Amount.Value.ToString() : string.Empty);   //Box_2_Amount
            if (request.Box_3_Checkbox2 != "1")
            {
                pdfFormFields.SetField("Form1099-LTC.CopyD.RightCol.c1_2", "1");   //PageAFATCA

            }
            if (request.Box_3_Checkbox1 != "1")
            {
                pdfFormFields.SetField("efield77_Form1099-LTC.CopyD.RightCol.c1_2", "1");   //PageAFATCA

            }

            pdfFormFields.SetField("Form1099-LTC.CopyD.RightCol.f1_11", request.Box_INS_SSN);   //Box_INS_SSN
            pdfFormFields.SetField("Form1099-LTC.CopyD.RightCol.f1_12", request.Box_INS_Name);   //Box_INS_Name
            pdfFormFields.SetField("Form1099-LTC.CopyD.RightCol.f1_13", request.Box_INS_Address);   //Box_INS_Address
            pdfFormFields.SetField("Form1099-LTC.CopyD.RightCol.f1_14", request.Box_INS_City +" " + request.Box_INS_State + " " + request.Box_INS_Zip);   //Box_INS_City & " " & Box_INS_State & " " & Box_INS_Zip

            if (request.Box_4_Checkbox != "1")
            {
                pdfFormFields.SetField("Form1099-LTC.CopyD.RightCol.c1_3", "1");   //PageAFATCA

            }
            if (request.Box_5_Checkbox1 != "1")
            {
                pdfFormFields.SetField("Form1099-LTC.CopyD.RightCol.c1_4", "1");   //PageAFATCA

            }
            if (request.Box_5_Checkbox2 != "1")
            {
                pdfFormFields.SetField("Form1099-LTC.CopyD.RightCol.c1_5", "1");   //PageAFATCA

            }
            if (!string.IsNullOrEmpty(request.Box_5_Date?.ToString()))
            {
                DateTime Box_5_Date;
                if (DateTime.TryParse(request.Box_5_Date?.ToString(), out Box_5_Date))
                {
                    pdfFormFields.SetField("Form1099-LTC.CopyD.RightCol.f1_15", Box_5_Date.ToString("MM/dd/yyyy"));
                }

            }


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

                string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.LTC_1099_TemplateFileName);
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
            var request = _evolvedtaxContext.Tbl1099_LTC.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.First_Name + " " + request.Name_Line2?.Replace(": ", "");
            newFile1 = string.Concat(ClientName, "_", AppConstants.LTC1099Form, "_", Id);
            string FilenameNew = "/1099LTC/" + newFile1 + ".pdf";
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
            var request = _evolvedtaxContext.Tbl1099_LTC.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.First_Name + " " + request.Name_Line2?.Replace(": ", "");

            newFile1 = string.Concat(ClientName, "_", AppConstants.LTC1099Form, "_", request.Id, "_Page_", selectedPage);
            string FilenameNew = "/1099LTC/" + newFile1 + ".pdf";
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
            string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.LTC_1099_TemplateFileName);
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
                            compileFileName = "For Policyholder .pdf";
                            break;
                        case "4":
                            compileFileName = "For Insured.pdf";
                            break;
                        case "6":
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

            var recordToDelete = _evolvedtaxContext.Tbl1099_LTC.First(p => p.Id == id);
            if (recordToDelete != null)
            {
                _evolvedtaxContext.Tbl1099_LTC.Remove(recordToDelete);
                await _evolvedtaxContext.SaveChangesAsync();
                return new MessageResponseModel { Status = true };
            }

            return new MessageResponseModel { Status = false };
        }

        public async Task<MessageResponseModel> KeepRecord(int id)
        {

            var recordToUpdate = _evolvedtaxContext.Tbl1099_LTC.First(p => p.Id == id);
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
            var result = from ic in _evolvedtaxContext.Tbl1099_LTC
                         where selectValues.Contains(ic.Id) && !string.IsNullOrEmpty(ic.Rcp_Email)
                         select new Tbl1099_LTC
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
        public IEnumerable<Tbl1099_LTC> GetForm1099List()
        {
            return _evolvedtaxContext.Tbl1099_LTC.AsEnumerable();
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
