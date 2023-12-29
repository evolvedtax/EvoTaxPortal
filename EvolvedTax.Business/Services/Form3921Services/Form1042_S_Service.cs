using AutoMapper;
using EvolvedTax.Business.MailService;
using EvolvedTax.Business.Services.Form1099Services;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs;
using EvolvedTax.Data.Models.DTOs.Response.Form1042;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Data.Models.Entities._1042;
using EvolvedTax.Data.Models.Entities._1099;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MathNet.Numerics.Distributions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Org.BouncyCastle.Cms;
using System.IO.Compression;

namespace EvolvedTax.Business.Services.Form3921Services
{
    public class Form1042_S_Service : IForm1042_S_Service
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly IInstituteService _instituteService;
        readonly IMailService _mailService;
        readonly ITrailAudit1099Service _trailAudit1099Service;
        readonly IMapper _mapper;
        public Form1042_S_Service(EvolvedtaxContext evolvedtaxContext, IMapper mapper, IInstituteService instituteService, IMailService mailService = null, ITrailAudit1099Service trailAudit1099Service = null)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _mapper = mapper;
            _instituteService = instituteService;
            _mailService = mailService;
            _trailAudit1099Service = trailAudit1099Service;
        }

        public IQueryable<Form1042SResponse> GetForm1042SList()
        {
            var response = _evolvedtaxContext.Tbl_1042s.Select(p => new Form3921
            {
                RcpLOBCode = p.RcpLOBCode,
                RcpTaxCountryName = p.RcpTaxCountryName,
                RcpCityForeign3 = p.RcpCityForeign3,
                RcpNameLine2 = p.RcpNameLine2,
                RcpFirstAndMI = p.RcpFirstAndMI,
                BatchID = p.BatchID,
                FormCategory = p.FormCategory,
                FormSource = p.FormSource,
                Ammended_No = p.Ammended_No,
                Box11Amount = p.Box11Amount,
                Box14aPrimaryName = p.Box14aPrimaryName,
                Box14bPrimaryEIN = p.Box14bPrimaryEIN,
                Box14bPrimaryType = p.Box14bPrimaryType,
                Box15aIFTEEIN = p.Box15aIFTEEIN,
                Box15bIFTECh3 = p.Box15bIFTECh3,
                Box15cIFTECh4 = p.Box15cIFTECh4,
                Box15dIFTENAME1 = p.Box15dIFTENAME1,
                Box15dIFTENAME2 = p.Box15dIFTENAME2,
                Box15dIFTENAME3 = p.Box15dIFTENAME3,
                Box15eIFTEGIIN = p.Box15eIFTEGIIN,
                Box15fCountryName = p.Box15fCountryName,
                Box15gForeignTIN = p.Box15gForeignTIN,
                Box15hIFTEAddr1 = p.Box15hIFTEAddr1,
                Box15hIFTEAddr2 = p.Box15hIFTEAddr2,
                Box15iIFTECity = p.Box15iIFTECity,
                Box15iStateProv = p.Box15iStateProv,
                Box15iZipPostal = p.Box15iZipPostal,
                Box16aPayersName = p.Box16aPayersName,
                Box16bPayersTIN = p.Box16bPayersTIN,
                Box16cPayersGIIN = p.Box16cPayersGIIN,
                Box16dPayersCh3 = p.Box16dPayersCh3,
                Box16ePayersCh4 = p.Box16ePayersCh4,
                Box17aAmount = p.Box17aAmount,
                Box17bStateTaxNo = p.Box17bStateTaxNo,
                Box17cState = p.Box17cState,
                Box1Code = p.Box1Code,
                Box2Amount = p.Box2Amount,
                Box3aExemptCode = p.Box3aExemptCode,
                Box3bTaxRate = p.Box3bTaxRate,
                Box3Chap3Chk = p.Box3Chap3Chk,
                Box4aExemptCode = p.Box4aExemptCode,
                Box4bTaxRate = p.Box4bTaxRate,
                Box4Chap4Chk = p.Box4Chap4Chk,
                Box5Amount = p.Box5Amount,
                Box6Amount = p.Box6Amount,
                Box7aAmount = p.Box7aAmount,
                Box7bCheck = p.Box7bCheck,
                Box7cCheck = p.Box7cCheck,
                Box8Amount = p.Box8Amount,
                Box9Amount = p.Box9Amount,
                Created_By = p.Created_By,
                Created_Date = p.Created_Date,
                EntityId = p.EntityId,
                Id = p.Id,
                InstID = p.InstID,
                IsDuplicated = p.IsDuplicated,
                PostalCode = p.PostalCode,
                ProRataBasisChkbx = p.ProRataBasisChkbx,
                Province = p.Province,
                RcpAcctNo = p.RcpAcctNo,
                RcpAddressLine1 = p.RcpAddressLine1,
                RcpAddressLine2 = p.RcpAddressLine2,
                RcpAddressType = p.RcpAddressType,
                RcpCh3Status = p.RcpCh3Status,
                RcpCh4Status = p.RcpCh4Status,
                RcpCountryName = p.RcpCountryName,
                RcpDOB = p.RcpDOB,
                RcpEmail = p.RcpEmail,
                RcpForeignTIN = p.RcpForeignTIN,
                RcpGIIN = p.RcpGIIN,
                RcpLastNameCompany = p.RcpLastNameCompany,
                RcpStateUSCanada = p.RcpStateUSCanada,
                RcpTINType = p.RcpTINType,
                RcpZipUSCanada = p.RcpZipUSCanada,
                RecipientTIN = p.RecipientTIN,
                Status = p.Status,
                UniqueFormID = p.UniqueFormID,
                Uploaded_File = p.Uploaded_File,
                UserId = p.UserId,
                WHACh3Status = p.WHACh3Status,
                WHACh4Status = p.WHACh4Status
            });
            return response;
        }
        public async Task<bool> SendEmailToRecipients(int[] selectValues, string URL, string form, int instituteId = -1)
        {
            var result = from ic in _evolvedtaxContext.Tbl_1042s
                         where selectValues.Contains(ic.Id) && !string.IsNullOrEmpty(ic.RcpEmail)
                         select new Tbl_1042s
                         {
                             RcpEmail = ic.RcpEmail,
                             EntityId = ic.EntityId,
                         };
            foreach (var item in result.ToList())
            {
                await _mailService.SendElectronicAcceptanceEmail(item.RcpEmail, (int)item.EntityId, string.Empty, "Action Required", URL, form, instituteId);

                await _trailAudit1099Service.AddUpdateRecipientAuditDetails(new AuditTrail1099 { RecipientEmail = item.RcpEmail, FormName = form, Token = item.EntityId.ToString() ?? string.Empty });
            }
            return true;
        }
        public async Task<MessageResponseModel> Upload1042_S_Data(IFormFile file, int InstId, int entityId, string UserId)
        {
            bool Status = false;
            var response = new List<Tbl_1042s>();
            var AList = new List<Tbl_1042s>();
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

                    var request = new Tbl_1042s
                    {
                        ProRataBasisChkbx = excelRow.GetCell(columnMapping["Ammended"])?.ToString() != null && (bool)excelRow.GetCell(columnMapping["Ammended"])?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0",
                        Ammended_No = excelRow.GetCell(columnMapping["Ammended #"])?.ToString() ?? string.Empty,
                        UniqueFormID = Convert.ToInt32(excelRow.GetCell(columnMapping["UNIQUE FORM IDENTIFIER"])?.ToString() ?? "0"),
                        WHACh3Status = excelRow.GetCell(columnMapping["WHA Ch3 Exemption Code"])?.ToString() ?? string.Empty,
                        WHACh4Status = excelRow.GetCell(columnMapping["WHA Ch4 Exemption Code"])?.ToString() ?? string.Empty,
                        RecipientTIN = excelRow.GetCell(columnMapping["Rcp TIN"])?.ToString() ?? string.Empty,
                        RcpTINType = Convert.ToInt32(excelRow.GetCell(columnMapping["Rcp TIN Type"])?.ToString() ?? "0"),
                        RcpAcctNo = excelRow.GetCell(columnMapping["Rcp Acct No"])?.ToString() ?? string.Empty,
                        RcpFirstAndMI = excelRow.GetCell(columnMapping["Rcp First Name"])?.ToString() ?? string.Empty,
                        RcpLastNameCompany = excelRow.GetCell(columnMapping["Rcp Company"])?.ToString() ?? string.Empty,
                        RcpNameLine2 = excelRow.GetCell(columnMapping["Rcp Last Name"])?.ToString() ?? string.Empty,
                        RcpAddressType = excelRow.GetCell(columnMapping["Address Type"])?.ToString() ?? string.Empty,
                        RcpAddressLine1 = excelRow.GetCell(columnMapping["Rcp Address Line 1"])?.ToString() ?? string.Empty,
                        RcpAddressLine2 = excelRow.GetCell(columnMapping["Rcp Address Line 2"])?.ToString() ?? string.Empty,
                        RcpCityForeign3 = excelRow.GetCell(columnMapping["Rcp City"])?.ToString() ?? string.Empty,
                        RcpStateUSCanada = excelRow.GetCell(columnMapping["Rcp State"])?.ToString() ?? string.Empty,
                        RcpZipUSCanada = excelRow.GetCell(columnMapping["Rcp Zip"])?.ToString() ?? string.Empty,
                        RcpCountryName = excelRow.GetCell(columnMapping["Rcp Country"])?.ToString() ?? string.Empty,
                        RcpCh3Status = Convert.ToInt32(excelRow.GetCell(columnMapping["Rcp Ch3 Status"])?.ToString() ?? "0"),
                        RcpCh4Status = Convert.ToInt32(excelRow.GetCell(columnMapping["Rcp Ch4 Status"])?.ToString() ?? "0"),
                        RcpDOB = excelRow.GetCell(columnMapping["Rcp DOB"])?.ToString() == null ? null : Convert.ToDateTime(excelRow.GetCell(columnMapping["Rcp DOB"]).ToString()),
                        RcpForeignTIN = excelRow.GetCell(columnMapping["Rcp Foreign TIN"])?.ToString() ?? string.Empty,
                        RcpGIIN = excelRow.GetCell(columnMapping["Rcp GIIN"])?.ToString() ?? string.Empty,
                        RcpLOBCode = excelRow.GetCell(columnMapping["Rcp LOB code"])?.ToString() ?? string.Empty,
                        RcpEmail = excelRow.GetCell(columnMapping["Rcp Email"])?.ToString() ?? string.Empty,
                        Box1Code = Convert.ToInt32(excelRow.GetCell(columnMapping["Income code"])?.ToString() ?? "0"),
                        Box2Amount = Convert.ToInt32(excelRow.GetCell(columnMapping["Gross income"])?.ToString() ?? "0"),
                        Box3Chap3Chk = excelRow.GetCell(columnMapping["Chap3"])?.ToString() ?? string.Empty,
                        Box3aExemptCode = excelRow.GetCell(columnMapping["Chp3ExCode"])?.ToString() ?? string.Empty,
                        Box3bTaxRate = Convert.ToDecimal(excelRow.GetCell(columnMapping["Chp3TaxRate"])?.ToString() ?? "0.0"),
                        Box4Chap4Chk = excelRow.GetCell(columnMapping["Chap4"])?.ToString() ?? string.Empty,
                        Box4aExemptCode = excelRow.GetCell(columnMapping["Chp4ExCode"])?.ToString() ?? string.Empty,
                        Box4bTaxRate = Convert.ToDecimal(excelRow.GetCell(columnMapping["Chp4TaxRate"])?.ToString() ?? "0.0"),
                        Box5Amount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Withholding allowance"])?.ToString() ?? "0.0"),
                        Box6Amount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Net income"])?.ToString() ?? "0.0"),
                        Box7aAmount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Federal tax withheld"])?.ToString() ?? "0.0"),
                        Box7bCheck = excelRow.GetCell(columnMapping["Escrow procedures were applied"])?.ToString() != null && (bool)excelRow.GetCell(columnMapping["Escrow procedures were applied"])?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0",
                        Box7cCheck = excelRow.GetCell(columnMapping["withholding occurred in subsequent year"])?.ToString() != null && (bool)excelRow.GetCell(columnMapping["withholding occurred in subsequent year"])?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0",
                        Box8Amount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Tax withheld by other agents"])?.ToString() ?? "0.0"),
                        Box9Amount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Overwithheld tax repaid"])?.ToString() ?? "0.0"),
                        Box11Amount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Tax paid by withholding agent"])?.ToString() ?? "0.0"),
                        RcpTaxCountryName = excelRow.GetCell(columnMapping["Rcp Tax Country Name"])?.ToString() ?? string.Empty,
                        Box14aPrimaryName = excelRow.GetCell(columnMapping["Primary WHA Name (if applicable)"])?.ToString() ?? string.Empty,
                        Box14bPrimaryEIN = excelRow.GetCell(columnMapping["Primary WHA EIN"])?.ToString() ?? string.Empty,
                        Box14bPrimaryType = excelRow.GetCell(columnMapping["Pro Rata Reporting"])?.ToString() ?? string.Empty,
                        Box15aIFTEEIN = excelRow.GetCell(columnMapping["IFTE EIN, if any"])?.ToString() ?? string.Empty,
                        Box15bIFTECh3 = excelRow.GetCell(columnMapping["IFTE Ch. 3 status code"])?.ToString() ?? string.Empty,
                        Box15cIFTECh4 = excelRow.GetCell(columnMapping["IFTE Ch. 4 status code"])?.ToString() ?? string.Empty,
                        Box15dIFTENAME1 = excelRow.GetCell(columnMapping["IFTE Name"])?.ToString() ?? string.Empty,
                        Box15eIFTEGIIN = excelRow.GetCell(columnMapping["IFTE GIIN"])?.ToString() ?? string.Empty,
                        Box15fCountryName = excelRow.GetCell(columnMapping["IFTE Country"])?.ToString() ?? string.Empty,
                        Box15gForeignTIN = excelRow.GetCell(columnMapping["IFTE FTIN"])?.ToString() ?? string.Empty,
                        Box15hIFTEAddr1 = excelRow.GetCell(columnMapping["IFTE Address Line1"])?.ToString() ?? string.Empty,
                        Box15hIFTEAddr2 = excelRow.GetCell(columnMapping["IFTE Address Line2"])?.ToString() ?? string.Empty,
                        Box15dIFTENAME2 = excelRow.GetCell(columnMapping["Box 15d I/FTE Name 2"])?.ToString() ?? string.Empty,
                        Box15dIFTENAME3 = excelRow.GetCell(columnMapping["Box 15d I/FTE Name 3"])?.ToString() ?? string.Empty,
                        Box15iIFTECity = excelRow.GetCell(columnMapping["IFTE City or town"])?.ToString() ?? string.Empty,
                        Box15iStateProv = excelRow.GetCell(columnMapping["State or Province"])?.ToString() ?? string.Empty,
                        Box15iZipPostal = excelRow.GetCell(columnMapping["Zip or Postal Code"])?.ToString() ?? string.Empty,
                        Box16aPayersName = excelRow.GetCell(columnMapping["Payer’s name"])?.ToString() ?? string.Empty,
                        Box16bPayersTIN = excelRow.GetCell(columnMapping["Payer's TIN"])?.ToString() ?? string.Empty,
                        Box16cPayersGIIN = excelRow.GetCell(columnMapping["Payer's GIIN"])?.ToString() ?? string.Empty,
                        Box16dPayersCh3 = excelRow.GetCell(columnMapping["Payer's Ch3 Status Code"])?.ToString() ?? string.Empty,
                        Box16ePayersCh4 = excelRow.GetCell(columnMapping["Payer's Ch4 Status Code"])?.ToString() ?? string.Empty,
                        Box17aAmount = Convert.ToDecimal(excelRow.GetCell(columnMapping["State income tax withheld"])?.ToString() ?? "0.0"),
                        Box17bStateTaxNo = excelRow.GetCell(columnMapping["Payer’s state tax no"])?.ToString() ?? string.Empty,
                        Box17cState = excelRow.GetCell(columnMapping["Payer's Name of state"])?.ToString() ?? string.Empty,
                        FormSource = excelRow.GetCell(columnMapping["Form Source"])?.ToString() ?? string.Empty,
                        FormCategory = excelRow.GetCell(columnMapping["Form Category"])?.ToString() ?? string.Empty,
                        BatchID = excelRow.GetCell(columnMapping["Batch ID"])?.ToString() ?? string.Empty,
                        Created_Date = DateTime.Now,
                        Created_By = UserId,
                        InstID = InstId,
                        EntityId = entityId,
                        UserId = UserId,
                    };
                    string clientEmailEINNumber = request.RecipientTIN ?? string.Empty;
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
                    if (await _evolvedtaxContext.Tbl_1042s.AnyAsync(p => p.RecipientTIN == request.RecipientTIN && p.EntityId == request.EntityId && p.Created_Date != null &&
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
                await _evolvedtaxContext.Tbl_1042s.AddRangeAsync(AList);
                await _evolvedtaxContext.SaveChangesAsync();
            }
            return new MessageResponseModel { Status = Status, Message = response, Param = "Database" };
            //return new MessageResponseModel { Status = Status, Message = response, Param = "Entity" };
        }


        public string GeneratePdf(int Id, string TemplatefilePath, string SaveFolderPath, int entityId)
        {
            return CreatePdf(Id, TemplatefilePath, SaveFolderPath, entityId, false);

        }

        public string CreatePdf(int Id, string TemplatefilePath, string SaveFolderPath, int entityId, bool IsAll, string Page = "")
        {
            var response = _evolvedtaxContext.Tbl_1042s.FirstOrDefault(p => p.Id == Id);
            //var requestInstitue = _instituteService.GetPayeeData((int)response.InstID);
            var entityData = _instituteService.GetEntityDataById(entityId);
            string templatefile = TemplatefilePath;
            string newFile1 = string.Empty;

            string ClientName = response.RcpFirstAndMI + " " + response.RcpNameLine2?.Replace(": ", "");

            if (!string.IsNullOrEmpty(Page))
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.Form1042S, "_", response.Id, "_Page_", Page);
            }
            else
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.Form1042S, "_", response.Id);
            }


            string FilenameNew = "/Form1042S/" + newFile1 + ".pdf";
            string newFileName = newFile1 + ".pdf"; // Add ".pdf" extension to the file name

            string newFilePath = Path.Combine(SaveFolderPath, newFileName);

            PdfReader pdfReader = new PdfReader(templatefile);
            PdfReader.unethicalreading = true;
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFilePath, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;

            //string Recepient_CountryCode = "";
            //if (response.Country != "United States")
            //{
            //    var country = _evolvedtaxContext.MstrCountries.FirstOrDefault(c => c.Country == response.Country);
            //    if (country != null)
            //    {
            //        Recepient_CountryCode = country.CountryId;
            //    }
            //}

            // string RecipentCity = string.Join(", ",
            //    new[]
            //    {
            //          response.City,
            //          response.State,
            //          string.IsNullOrWhiteSpace(response.Province) ? null : response.Province,
            //           string.IsNullOrWhiteSpace(Recepient_CountryCode) ? null : Recepient_CountryCode,
            //          response.Zip,
            //          string.IsNullOrWhiteSpace(response.PostalCode) ? null : response.PostalCode

            //    }.Where(s => !string.IsNullOrWhiteSpace(s))
            //);

            //String RecipentAddress = string.Concat(response.Address_Deliv_Street, ", ", response.Address_Apt_Suite);
            string currentYear = Convert.ToString(DateTime.Now.Year % 100);

            #region PDF Columns
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].PgHeader[0].f1_01[0]", response.UniqueFormID?.ToString());
            if (response.ProRataBasisChkbx == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyA[0].PgHeader[0].c1_1[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].PgHeader[0].f1_02[0]", response.Ammended_No);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_03[0]", response.Box1Code?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_04[0]", response.Box2Amount?.ToString());
            if (string.IsNullOrEmpty(response.Box4Chap4Chk))
            {
                pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].Lines3_b[0].f1_05[0]", "3");
            }
            else if (string.IsNullOrEmpty(response.Box3Chap3Chk))
            {
                pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].Lines3_b[0].f1_05[0]", "4");
            }
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].Lines3_b[0].f1_06[0]", response.Box3aExemptCode);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].Lines3_b[0].f1_07[0]", ((int)Math.Floor(response?.Box3bTaxRate ?? 0)).ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].Lines3_b[0].f1_08[0]", response?.Box3bTaxRate?.ToString("F2").Split('.')[1]);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_09[0]", response.Box4aExemptCode);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_10[0]", ((int)Math.Floor(response?.Box4bTaxRate ?? 0)).ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_11[0]", response?.Box4bTaxRate?.ToString("F2").Split('.')[1]);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_12[0]", response.Box5Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_13[0]", response.Box6Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_14[0]", response.Box7aAmount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].c1_2[0]", response.Box7bCheck);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].c1_3[0]", response.Box7cCheck);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_15[0]", response.Box8Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_16[0]", response.Box9Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_17[0]", (response?.Box7aAmount + response?.Box8Amount + response?.Box9Amount)?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_18[0]", response?.Box11Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_19[0]", entityData.Ein);
            //pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_20[0]", entityData.Entity Ch3 Status Code);
            //pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_21[0]", entityData.Entity Ch4 Status Code);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_22[0]", entityData.EntityName);
            //pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_23[0]", entityData.Entity GIIN);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_24[0]", entityData.Country);
            //pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_25[0]", entityData.Entity FTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_26[0]", entityData.Address1 + " " + entityData.Address2);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_27[0]", entityData.City);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_28[0]", response.RcpFirstAndMI);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_29[0]", response.RcpCountryName);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_30[0]", response.RcpAddressLine1 + " " + response.RcpAddressLine2);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftCol[0].f1_31[0]", response.RcpCityForeign3 + " " + response.RcpStateUSCanada + " " + response.RcpZipUSCanada);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_32[0]", response.RecipientTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_33[0]", response.RcpCh3Status?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_34[0]", response.RcpCh4Status?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_35[0]", response.RcpGIIN);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_36[0]", response.RcpForeignTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_37[0]", response.RcpLOBCode);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_38[0]", response.RcpAcctNo);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_39[0]", response.RcpDOB?.ToString("yyyyMMdd"));
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_40[0]", response.Box14aPrimaryName);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_41[0]", response.Box14bPrimaryEIN);
            if (response.ProRataBasisChkbx == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].c1_4[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_42[0]", response.Box15aIFTEEIN);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_43[0]", response.Box15bIFTECh3);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_44[0]", response.Box15cIFTECh4);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_45[0]", response.Box15dIFTENAME1);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_46[0]", response.Box15eIFTEGIIN);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_47[0]", response.Box15fCountryName);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_48[0]", response.Box15gForeignTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_49[0]", response.Box15hIFTEAddr1 + " " + response.Box15hIFTEAddr2);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_50[0]", response.Box15iIFTECity + " " + response.Box15iStateProv + " " + response.Box15iZipPostal);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_51[0]", response.Box16aPayersName);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_52[0]", response.Box16bPayersTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_53[0]", response.Box16cPayersGIIN);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_54[0]", response.Box16dPayersCh3);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_55[0]", response.Box16ePayersCh4);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_56[0]", response.Box17aAmount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_57[0]", response.Box17bStateTaxNo);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightCol[0].f1_58[0]", response.Box17cState);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].PgHeader[0].f1_01[0]", response.UniqueFormID?.ToString());
            if (response.ProRataBasisChkbx == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyB[0].PgHeader[0].c1_1[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].PgHeader[0].f1_02[0]", response.Ammended_No);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_03[0]", response.Box1Code?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_04[0]", response.Box2Amount?.ToString());
            if (string.IsNullOrEmpty(response.Box4Chap4Chk))
            {
                pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].Lines3_b[0].f1_05[0]", "3");
            }
            else if (string.IsNullOrEmpty(response.Box3Chap3Chk))
            {
                pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].Lines3_b[0].f1_05[0]", "4");
            }
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].Lines3_b[0].f1_06[0]", response.Box3aExemptCode);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].Lines3_b[0].f1_07[0]", ((int)Math.Floor(response?.Box3bTaxRate ?? 0)).ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].Lines3_b[0].f1_08[0]", response?.Box3bTaxRate?.ToString("F2").Split('.')[1]);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_09[0]", response.Box4aExemptCode);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_10[0]", response.Box4bTaxRate?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_11[0]", response?.Box4bTaxRate?.ToString("F2").Split('.')[1]);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_12[0]", response.Box5Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_13[0]", response.Box6Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_14[0]", response.Box7aAmount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].c1_2[0]", response.Box7bCheck);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].c1_3[0]", response.Box7cCheck);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_15[0]", response.Box8Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_16[0]", response.Box9Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_17[0]", (response.Box7aAmount + response.Box8Amount + response.Box9Amount)?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_18[0]", response.Box11Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_19[0]", entityData.Ein);
            //pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_20[0]", entityData.Entity Ch3 Status Code);
            //pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_21[0]", entityData.Entity Ch4 Status Code);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_22[0]", entityData.EntityName);
            //pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_23[0]", entityData.Entity GIIN);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_24[0]", entityData.Country);
            //pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_25[0]", entityData.Entity FTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_26[0]", entityData.Address1 + " " + entityData.Address2);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_27[0]", entityData.City);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_28[0]", response.RcpFirstAndMI);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_29[0]", response.RcpCountryName);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_30[0]", response.RcpAddressLine1 + " " + response.RcpAddressLine2);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftCol[0].f1_31[0]", response.RcpCityForeign3 + " " + response.RcpStateUSCanada + " " + response.RcpZipUSCanada);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_32[0]", response.RecipientTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_33[0]", response.RcpCh3Status?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_34[0]", response.RcpCh4Status?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_35[0]", response.RcpGIIN);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_36[0]", response.RcpForeignTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_37[0]", response.RcpLOBCode);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_38[0]", response.RcpAcctNo);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_39[0]", response.RcpDOB?.ToString("yyyyMMdd"));
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_40[0]", response.Box14aPrimaryName);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_41[0]", response.Box14bPrimaryEIN);
            if (response.ProRataBasisChkbx == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].c1_4[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_42[0]", response.Box15aIFTEEIN);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_43[0]", response.Box15bIFTECh3);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_44[0]", response.Box15cIFTECh4);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_45[0]", response.Box15dIFTENAME1);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_46[0]", response.Box15eIFTEGIIN);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_47[0]", response.Box15fCountryName);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_48[0]", response.Box15gForeignTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_49[0]", response.Box15hIFTEAddr1 + " " + response.Box15hIFTEAddr2);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_50[0]", response.Box15iIFTECity + " " + response.Box15iStateProv + " " + response.Box15iZipPostal);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_51[0]", response.Box16aPayersName);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_52[0]", response.Box16bPayersTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_53[0]", response.Box16cPayersGIIN);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_54[0]", response.Box16dPayersCh3);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_55[0]", response.Box16ePayersCh4);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_56[0]", response.Box17aAmount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_57[0]", response.Box17bStateTaxNo);
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightCol[0].f1_58[0]", response.Box17cState);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].PgHeader[0].f1_01[0]", response.UniqueFormID?.ToString());
            if (response.ProRataBasisChkbx == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyC[0].PgHeader[0].c1_1[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].PgHeader[0].f1_02[0]", response.Ammended_No);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_03[0]", response.Box1Code?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_04[0]", response.Box2Amount?.ToString());
            if (string.IsNullOrEmpty(response.Box4Chap4Chk))
            {
                pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].Lines3_b[0].f1_05[0]", "3");
            }
            else if (string.IsNullOrEmpty(response.Box3Chap3Chk))
            {
                pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].Lines3_b[0].f1_05[0]", "4");
            }
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].Lines3_b[0].f1_06[0]", response.Box3aExemptCode);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].Lines3_b[0].f1_07[0]", ((int)Math.Floor(response?.Box3bTaxRate ?? 0)).ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].Lines3_b[0].f1_08[0]", response?.Box3bTaxRate?.ToString("F2").Split('.')[1]);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_09[0]", response.Box4aExemptCode);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_10[0]", response.Box4bTaxRate?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_11[0]", response?.Box4bTaxRate?.ToString("F2").Split('.')[1]);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_12[0]", response.Box5Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_13[0]", response.Box6Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_14[0]", response.Box7aAmount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].c1_2[0]", response.Box7bCheck);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].c1_3[0]", response.Box7cCheck);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_15[0]", response.Box8Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_16[0]", response.Box9Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_17[0]", (response.Box7aAmount + response.Box8Amount + response.Box9Amount)?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_18[0]", response.Box11Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_19[0]", entityData.Ein);
            //pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_20[0], entityData.Entity Ch3 Status Code);
            //pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_21[0], entityData.Entity Ch4 Status Code);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_22[0]", entityData.EntityName);
            //pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_23[0], entityData.Entity GIIN);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_24[0]", entityData.Country);
            //pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_25[0], entityData.Entity FTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_26[0]", entityData.Address1 + " " + entityData.Address2);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_27[0]", entityData.City);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_28[0]", response.RcpFirstAndMI);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_29[0]", response.RcpCountryName);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_30[0]", response.RcpAddressLine1 + " " + response.RcpAddressLine2);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftCol[0].f1_31[0]", response.RcpCityForeign3 + " " + response.RcpStateUSCanada + " " + response.RcpZipUSCanada);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_32[0]", response.RecipientTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_33[0]", response.RcpCh3Status?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_34[0]", response.RcpCh4Status?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_35[0]", response.RcpGIIN);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_36[0]", response.RcpForeignTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_37[0]", response.RcpLOBCode);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_38[0]", response.RcpAcctNo);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_39[0]", response.RcpDOB?.ToString("yyyyMMdd"));
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_40[0]", response.Box14aPrimaryName);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_41[0]", response.Box14bPrimaryEIN);
            if (response.ProRataBasisChkbx == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].c1_4[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_42[0]", response.Box15aIFTEEIN);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_43[0]", response.Box15bIFTECh3);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_44[0]", response.Box15cIFTECh4);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_45[0]", response.Box15dIFTENAME1);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_46[0]", response.Box15eIFTEGIIN);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_47[0]", response.Box15fCountryName);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_48[0]", response.Box15gForeignTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_49[0]", response.Box15hIFTEAddr1 + " " + response.Box15hIFTEAddr2);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_50[0]", response.Box15iIFTECity + " " + response.Box15iStateProv + " " + response.Box15iZipPostal);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_51[0]", response.Box16aPayersName);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_52[0]", response.Box16bPayersTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_53[0]", response.Box16cPayersGIIN);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_54[0]", response.Box16dPayersCh3);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_55[0]", response.Box16ePayersCh4);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_56[0]", response.Box17aAmount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_57[0]", response.Box17bStateTaxNo);
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightCol[0].f1_58[0]", response.Box17cState);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].PgHeader[0].f1_01[0]", response.UniqueFormID?.ToString());
            if (response.ProRataBasisChkbx == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyD[0].PgHeader[0].c1_1[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].PgHeader[0].f1_02[0]", response.Ammended_No);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_03[0]", response.Box1Code?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_04[0]", response.Box2Amount?.ToString());
            if (string.IsNullOrEmpty(response.Box4Chap4Chk))
            {
                pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].Lines3_b[0].f1_05[0]", "3");
            }
            else if (string.IsNullOrEmpty(response.Box3Chap3Chk))
            {
                pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].Lines3_b[0].f1_05[0]", "4");
            }
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].Lines3_b[0].f1_06[0]", response.Box3aExemptCode);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].Lines3_b[0].f1_07[0]", ((int)Math.Floor(response?.Box3bTaxRate ?? 0)).ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].Lines3_b[0].f1_08[0]", response?.Box3bTaxRate?.ToString("F2").Split('.')[1]);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_09[0]", response.Box4aExemptCode);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_10[0]", response.Box4bTaxRate?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_11[0]", response?.Box4bTaxRate?.ToString("F2").Split('.')[1]);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_12[0]", response.Box5Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_13[0]", response.Box6Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_14[0]", response.Box7aAmount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].c1_2[0]", response.Box7bCheck);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].c1_3[0]", response.Box7cCheck);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_15[0]", response.Box8Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_16[0]", response.Box9Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_17[0]", (response.Box7aAmount + response.Box8Amount + response.Box9Amount)?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_18[0]", response.Box11Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_19[0]", entityData.Ein);
            //pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_20[0]", entityData.Entity Ch3 Status Code);
            //pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_21[0]", entityData.Entity Ch4 Status Code);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_22[0]", entityData.EntityName);
            //pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_23[0]", entityData.Entity GIIN);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_24[0]", entityData.Country);
            //pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_25[0]", entityData.Entity FTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_26[0]", entityData.Address1 + " " + entityData.Address2);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_27[0]", entityData.City);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_28[0]", response.RcpFirstAndMI);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_29[0]", response.RcpCountryName);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_30[0]", response.RcpAddressLine1 + " " + response.RcpAddressLine2);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].LeftCol[0].f1_31[0]", response.RcpCityForeign3 + " " + response.RcpStateUSCanada + " " + response.RcpZipUSCanada);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_32[0]", response.RecipientTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_33[0]", response.RcpCh3Status?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_34[0]", response.RcpCh4Status?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_35[0]", response.RcpGIIN);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_36[0]", response.RcpForeignTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_37[0]", response.RcpLOBCode);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_38[0]", response.RcpAcctNo);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_39[0]", response.RcpDOB?.ToString("yyyyMMdd"));
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_40[0]", response.Box14aPrimaryName);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_41[0]", response.Box14bPrimaryEIN);
            if (response.ProRataBasisChkbx == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].c1_4[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_42[0]", response.Box15aIFTEEIN);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_43[0]", response.Box15bIFTECh3);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_44[0]", response.Box15cIFTECh4);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_45[0]", response.Box15dIFTENAME1);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_46[0]", response.Box15eIFTEGIIN);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_47[0]", response.Box15fCountryName);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_48[0]", response.Box15gForeignTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_49[0]", response.Box15hIFTEAddr1 + " " + response.Box15hIFTEAddr2);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_50[0]", response.Box15iIFTECity + " " + response.Box15iStateProv + " " + response.Box15iZipPostal);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_51[0]", response.Box16aPayersName);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_52[0]", response.Box16bPayersTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_53[0]", response.Box16cPayersGIIN);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_54[0]", response.Box16dPayersCh3);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_55[0]", response.Box16ePayersCh4);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_56[0]", response.Box17aAmount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_57[0]", response.Box17bStateTaxNo);
            pdfFormFields.SetField("topmostSubform[0].CopyD[0].RightCol[0].f1_58[0]", response.Box17cState);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].PgHeader[0].f1_01[0]", response.UniqueFormID?.ToString());
            if (response.ProRataBasisChkbx == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyE[0].PgHeader[0].c1_1[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].PgHeader[0].f1_02[0]", response.Ammended_No);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_03[0]", response.Box1Code?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_04[0]", response.Box2Amount?.ToString());
            if (string.IsNullOrEmpty(response.Box4Chap4Chk))
            {
                pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].Lines3_b[0].f1_05[0]", "3");
            }
            else if (string.IsNullOrEmpty(response.Box3Chap3Chk))
            {
                pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].Lines3_b[0].f1_05[0]", "4");
            }
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].Lines3_b[0].f1_06[0]", response.Box3aExemptCode);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].Lines3_b[0].f1_07[0]", ((int)Math.Floor(response?.Box3bTaxRate ?? 0)).ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].Lines3_b[0].f1_08[0]", response?.Box3bTaxRate?.ToString("F2").Split('.')[1]);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_09[0]", response.Box4aExemptCode);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_10[0]", response.Box4bTaxRate?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_11[0]", response?.Box4bTaxRate?.ToString("F2").Split('.')[1]);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_12[0]", response.Box5Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_13[0]", response.Box6Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_14[0]", response.Box7aAmount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].c1_2[0]", response.Box7bCheck);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].c1_3[0]", response.Box7cCheck);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_15[0]", response.Box8Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_16[0]", response.Box9Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_17[0]", (response.Box7aAmount + response.Box8Amount + response.Box9Amount)?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_18[0]", response.Box11Amount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_19[0]", entityData.Ein);
            //pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_20[0]", entityData.Entity Ch3 Status Code);
            //pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_21[0]", entityData.Entity Ch4 Status Code);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_22[0]", entityData.EntityName);
            //pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_23[0]", entityData.Entity GIIN);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_24[0]", entityData.Country);
            //pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_25[0]", entityData.Entity FTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_26[0]", entityData.Address1 + " " + entityData.Address2);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_27[0]", entityData.City);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_28[0]", response.RcpFirstAndMI);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_29[0]", response.RcpCountryName);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_30[0]", response.RcpAddressLine1 + " " + response.RcpAddressLine2);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].LeftCol[0].f1_31[0]", response.RcpCityForeign3 + " " + response.RcpStateUSCanada + " " + response.RcpZipUSCanada);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_32[0]", response.RecipientTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_33[0]", response.RcpCh3Status?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_34[0]", response.RcpCh4Status?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_35[0]", response.RcpGIIN);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_36[0]", response.RcpForeignTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_37[0]", response.RcpLOBCode);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_38[0]", response.RcpAcctNo);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_39[0]", response.RcpDOB?.ToString("yyyyMMddy"));
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_40[0]", response.Box14aPrimaryName);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_41[0]", response.Box14bPrimaryEIN);
            if (response.ProRataBasisChkbx == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].c1_4[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_42[0]", response.Box15aIFTEEIN);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_43[0]", response.Box15bIFTECh3);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_44[0]", response.Box15cIFTECh4);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_45[0]", response.Box15dIFTENAME1);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_46[0]", response.Box15eIFTEGIIN);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_47[0]", response.Box15fCountryName);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_48[0]", response.Box15gForeignTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_49[0]", response.Box15hIFTEAddr1 + " " + response.Box15hIFTEAddr2);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_50[0]", response.Box15iIFTECity + " " + response.Box15iStateProv + " " + response.Box15iZipPostal);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_51[0]", response.Box16aPayersName);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_52[0]", response.Box16bPayersTIN);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_53[0]", response.Box16cPayersGIIN);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_54[0]", response.Box16dPayersCh3);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_55[0]", response.Box16ePayersCh4);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_56[0]", response.Box17aAmount?.ToString());
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_57[0]", response.Box17bStateTaxNo);
            pdfFormFields.SetField("topmostSubform[0].CopyE[0].RightCol[0].f1_58[0]", response.Box17cState);
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
            //string newFile1 = string.Empty;
            //var request = _evolvedtaxContext.Tbl_1042s.FirstOrDefault(p => p.Id == Id);
            //String ClientName = request.First_Name + " " + request.Name_Line_2?.Replace(": ", "");
            //newFile1 = string.Concat(ClientName, "_", AppConstants.Form1042S, "_", Id);
            //string FilenameNew = "/Form1042S/" + newFile1 + ".pdf";
            //string newFileName = newFile1 + ".pdf";



            //var newFilePath = CreatePdf(Id, TemplatefilePath, SaveFolderPath, true);

            //// Create a copy of the generated PDF
            //string tempFilePath = Path.Combine(SaveFolderPath, newFile1 + "_temp.pdf");
            //File.Copy(newFilePath, tempFilePath);

            //// Open the copied PDF
            //PdfReader pdfReaderTemp = new PdfReader(tempFilePath);
            //PdfReader.unethicalreading = true;

            //// Create a new PDF document to save the modified pages
            //string modifiedFilePath = Path.Combine(SaveFolderPath, newFile1 + "_modified.pdf");
            //using (FileStream fs = new FileStream(modifiedFilePath, FileMode.Create))
            //using (Document doc = new Document())
            //using (PdfCopy copy = new PdfCopy(doc, fs))
            //{
            //    doc.Open();
            //    List<int> pagesToInclude = selectedPages.Select(int.Parse).ToList();


            //    for (int page = 1; page <= pdfReaderTemp.NumberOfPages; page++)
            //    {
            //        if (pagesToInclude.Contains(page))
            //        {
            //            PdfImportedPage importedPage = copy.GetImportedPage(pdfReaderTemp, page);
            //            copy.AddPage(importedPage);
            //        }
            //    }


            //    doc.Close();
            //}

            //// Close the copied PDF reader
            //pdfReaderTemp.Close();

            //// Delete the temporary copied PDF
            //File.Delete(tempFilePath);

            //// Delete the original PDF
            //File.Delete(newFilePath);

            //// Rename the modified PDF by removing "_modified" from the filename
            //string fileNameWithoutModified = newFile1.Replace("_modified", "");
            //string finalFilePath = Path.Combine(SaveFolderPath, fileNameWithoutModified + ".pdf");
            //File.Move(modifiedFilePath, finalFilePath);

            //return finalFilePath;
            return null;
        }
        public string GeneratePdForSpecificType(int Id, string TemplatefilePath, string SaveFolderPath, string selectedPage)
        {
            //string newFile1 = string.Empty;
            //var request = _evolvedtaxContext.Tbl_1042s.FirstOrDefault(p => p.Id == Id);
            //String ClientName = request.First_Name + " " + request.Name_Line_2?.Replace(": ", "");

            //newFile1 = string.Concat(ClientName, "_", AppConstants.Form1042S, "_", request.Id, "_Page_", selectedPage);
            //string FilenameNew = "/Form1042S/" + newFile1 + ".pdf";
            //string newFileName = newFile1 + ".pdf";

            //var newFilePath = CreatePdf(Id, TemplatefilePath, SaveFolderPath, true, selectedPage);

            //// Create a copy of the generated PDF
            //string tempFilePath = Path.Combine(SaveFolderPath, newFile1 + "_temp.pdf");
            //File.Copy(newFilePath, tempFilePath);

            //// Open the copied PDF
            //PdfReader pdfReaderTemp = new PdfReader(tempFilePath);
            //PdfReader.unethicalreading = true;

            //// Create a new PDF document to save the modified pages
            //string modifiedFilePath = Path.Combine(SaveFolderPath, newFile1 + "_modified.pdf");
            //using (FileStream fs = new FileStream(modifiedFilePath, FileMode.Create))
            //using (Document doc = new Document())
            //using (PdfCopy copy = new PdfCopy(doc, fs))
            //{
            //    doc.Open();

            //    PdfImportedPage importedPage = copy.GetImportedPage(pdfReaderTemp, Convert.ToInt32(selectedPage));
            //    copy.AddPage(importedPage);
            //    doc.Close();
            //}

            //// Close the copied PDF reader
            //pdfReaderTemp.Close();

            //// Delete the temporary copied PDF
            //File.Delete(tempFilePath);

            //// Delete the original PDF
            //File.Delete(newFilePath);

            //// Rename the modified PDF by removing "_modified" from the filename
            //string fileNameWithoutModified = newFile1.Replace("_modified", "");
            //string finalFilePath = Path.Combine(SaveFolderPath, fileNameWithoutModified + ".pdf");
            //File.Move(modifiedFilePath, finalFilePath);

            //return finalFilePath;
            return null;
        }
        public string DownloadOneFile(List<int> ids, string SaveFolderPath, List<string> selectedPages, string RootPath)
        {
            //var pdfPaths = new List<string>();
            //var CompilepdfPaths = new List<string>();
            //string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.Form1042STemplateFileName);
            //bool containsAll = selectedPages.Contains("All");

            //if (containsAll)
            //{
            //    foreach (var id in ids)
            //    {
            //        var pdfPath = CreatePdf(id, TemplatePathFile, SaveFolderPath, true);
            //        pdfPaths.Add(pdfPath);
            //    }

            //    #region CompilePDFs

            //    string compileFileName = "All Form Single File.pdf";
            //    string outputFilePath = Path.Combine(SaveFolderPath, compileFileName);
            //    CompilepdfPaths.Add(outputFilePath);

            //    // Create a Document object
            //    Document document = new Document();

            //    // Create a PdfCopy object to write the output PDF
            //    PdfCopy pdfCopy = new PdfCopy(document, new FileStream(outputFilePath, FileMode.Create));

            //    // Open the document for writing
            //    document.Open();

            //    foreach (string pdfFilePath in pdfPaths)
            //    {
            //        // Open each input PDF file
            //        PdfReader pdfReader = new PdfReader(pdfFilePath);

            //        // Iterate through the pages of the input PDF and add them to the output PDF
            //        for (int pageNum = 1; pageNum <= pdfReader.NumberOfPages; pageNum++)
            //        {
            //            PdfImportedPage page = pdfCopy.GetImportedPage(pdfReader, pageNum);
            //            pdfCopy.AddPage(page);
            //        }

            //        pdfReader.Close();
            //    }


            //    document.Close();
            //    pdfCopy.Close();
            //    pdfPaths.Clear();

            //    #endregion

            //}
            //else
            //{
            //    foreach (var selectedPage in selectedPages)
            //    {


            //        foreach (var id in ids)
            //        {
            //            var pdfPath = GeneratePdForSpecificType(id, TemplatePathFile, SaveFolderPath, selectedPage);
            //            pdfPaths.Add(pdfPath);
            //        }

            //        #region CompilePDFs
            //        string compileFileName;

            //        switch (selectedPage)
            //        {
            //            case "2":
            //                compileFileName = "Internal Revenue Service Center.pdf";
            //                break;
            //            case "3":
            //                compileFileName = "For Borrower.pdf";
            //                break;
            //            case "5":
            //                compileFileName = "For Lender.pdf";
            //                break;
            //            default:
            //                compileFileName = "compiled_page.pdf";
            //                break;
            //        }

            //        string outputFilePath = Path.Combine(SaveFolderPath, compileFileName);
            //        CompilepdfPaths.Add(outputFilePath);

            //        // Create a Document object
            //        Document document = new Document();

            //        // Create a PdfCopy object to write the output PDF
            //        PdfCopy pdfCopy = new PdfCopy(document, new FileStream(outputFilePath, FileMode.Create));

            //        // Open the document for writing
            //        document.Open();

            //        foreach (string pdfFilePath in pdfPaths)
            //        {
            //            // Open each input PDF file
            //            PdfReader pdfReader = new PdfReader(pdfFilePath);

            //            // Iterate through the pages of the input PDF and add them to the output PDF
            //            for (int pageNum = 1; pageNum <= pdfReader.NumberOfPages; pageNum++)
            //            {
            //                PdfImportedPage page = pdfCopy.GetImportedPage(pdfReader, pageNum);
            //                pdfCopy.AddPage(page);
            //            }

            //            pdfReader.Close();
            //        }


            //        document.Close();
            //        pdfCopy.Close();
            //        pdfPaths.Clear();

            //        #endregion


            //    }
            //}

            ////Create Zip
            //var zipFileName = $"GeneratedPDFs_{DateTime.Now:yyyyMMddHHmmss}.zip";
            //var zipFilePath = Path.Combine(SaveFolderPath, zipFileName);

            //using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            //{
            //    foreach (var pdfPath in CompilepdfPaths)
            //    {
            //        var pdfFileName = Path.GetFileName(pdfPath);
            //        zipArchive.CreateEntryFromFile(pdfPath, pdfFileName);
            //    }
            //}

            //return zipFilePath; // Return the ZIP file path.
            return null;
        }
        public string GenerateAndZipPdfs(List<int> ids, string SaveFolderPath, List<string> selectedPages, string RootPath)
        {
            //var pdfPaths = new List<string>();

            //foreach (var id in ids)
            //{

            //    string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.Form1042STemplateFileName);
            //    bool containsAll = selectedPages.Contains("All");

            //    if (containsAll)
            //    {
            //        var pdfPath = CreatePdf(id, TemplatePathFile, SaveFolderPath, true);
            //        pdfPaths.Add(pdfPath);
            //    }
            //    else
            //    {
            //        var pdfPath = GeneratePdfForSpecificPage(id, TemplatePathFile, SaveFolderPath, selectedPages);
            //        pdfPaths.Add(pdfPath);

            //    }


            //}

            //var zipFileName = $"GeneratedPDFs_{DateTime.Now:yyyyMMddHHmmss}.zip";
            //var zipFilePath = Path.Combine(SaveFolderPath, zipFileName);

            //using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            //{
            //    foreach (var pdfPath in pdfPaths)
            //    {
            //        var pdfFileName = Path.GetFileName(pdfPath);
            //        zipArchive.CreateEntryFromFile(pdfPath, pdfFileName);
            //    }
            //}

            //return zipFilePath; // Return the ZIP file path.
            return null;
        }

        public async Task<MessageResponseModel> DeletePermeant(int id)
        {

            var recordToDelete = _evolvedtaxContext.Tbl_1042s.First(p => p.Id == id);
            if (recordToDelete != null)
            {
                _evolvedtaxContext.Tbl_1042s.Remove(recordToDelete);
                await _evolvedtaxContext.SaveChangesAsync();
                return new MessageResponseModel { Status = true };
            }

            return new MessageResponseModel { Status = false };
        }

        public async Task<MessageResponseModel> KeepRecord(int id)
        {

            var recordToUpdate = _evolvedtaxContext.Tbl_1042s.First(p => p.Id == id);
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
