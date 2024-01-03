using AutoMapper;
using EvolvedTax.Business.MailService;
using EvolvedTax.Business.Services.Form1099Services;
using EvolvedTax.Business.Services.Form5498ervices;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs;
using EvolvedTax.Data.Models.DTOs.Response.Form1042;
using EvolvedTax.Data.Models.DTOs.Response.Form1098;
using EvolvedTax.Data.Models.DTOs.Response.Form5498;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Data.Models.Entities._1042;
using EvolvedTax.Data.Models.Entities._1098;
using EvolvedTax.Data.Models.Entities._1099;
using EvolvedTax.Data.Models.Entities._5498;
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

namespace EvolvedTax.Business.Services.Form1098Services
{
    public class Form1098_Service : IForm1098_Service
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly IInstituteService _instituteService;
        readonly IMailService _mailService;
        readonly ITrailAudit1099Service _trailAudit1099Service;
        readonly IMapper _mapper;
        public Form1098_Service(EvolvedtaxContext evolvedtaxContext, IMapper mapper, IInstituteService instituteService, IMailService mailService = null, ITrailAudit1099Service trailAudit1099Service = null)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _mapper = mapper;
            _instituteService = instituteService;
            _mailService = mailService;
            _trailAudit1099Service = trailAudit1099Service;
        }

        public IQueryable<Form1098Response> GetForm1098List()
        {
            var response = _evolvedtaxContext.Tbl_1098.Select(p => new Form1098Response
            {
                Id = p.Id,
                RcpTIN = p.RcpTIN,
                LastNameCompany = p.LastNameCompany,
                FirstName = p.FirstName,
                NameLine2 = p.NameLine2,
                AddressType = p.AddressType,
                AddressDelivStreet = p.AddressDelivStreet,
                AddressAptSuite = p.AddressAptSuite,
                City = p.City,
                State = p.State,
                Zip = p.Zip,
                Country = p.Country,
                RcpAccount = p.RcpAccount,
                RcpEmail = p.RcpEmail,
                Box1Amount = p.Box1Amount,
                Box2Amount = p.Box2Amount,
                Box3DATETIME = p.Box3DATETIME,
                Box4Amount = p.Box4Amount,
                Box5Amount = p.Box5Amount,
                Box6Amount = p.Box6Amount,
                Box7Checkbox = p.Box7Checkbox,
                Box8AllLines = p.Box8AllLines,
                Box9Number = p.Box9Number,
                Box10AllLines = p.Box10AllLines,
                Box11DATETIME = p.Box11DATETIME,
                FormCategory = p.FormCategory,
                FormSource = p.FormSource,
                TaxState = p.TaxState,
                Uploaded_File = p.Uploaded_File,
                Status = p.Status,
                Created_By = p.Created_By,
                Created_Date = p.Created_Date,
                UserId = p.UserId,
                InstID = p.InstID,
                EntityId = p.EntityId,
                IsDuplicated = p.IsDuplicated,
                Province = p.Province,
                PostalCode = p.PostalCode
            });

            return response;
        }


        public async Task<bool> SendEmailToRecipients(int[] selectValues, string URL, string form, int instituteId = -1)
        {
            var result = from ic in _evolvedtaxContext.Tbl_1098_C
                         where selectValues.Contains(ic.Id) && !string.IsNullOrEmpty(ic.RcpEmail)
                         select new Tbl_1098_C
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
        public async Task<MessageResponseModel> Upload1098_Data(IFormFile file, int InstId, int entityId, string UserId)
        {
            bool Status = false;
            var response = new List<Tbl_1098>();
            var AList = new List<Tbl_1098>();
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

                    var request = new Tbl_1098
                    {
                        // ProRataBasisChkbx = (excelRow.GetCell(columnMapping["Ammended"])?.ToString() != null && (bool)excelRow.GetCell(columnMapping["Ammended"])?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        // Ammended_No = excelRow.GetCell(columnMapping["Ammended #"])?.ToString() ?? string.Empty,
                        //UniqueFormID = Convert.ToInt32(excelRow.GetCell(columnMapping["UNIQUE FORM IDENTIFIER"])?.ToString() ?? "0"),
                        // WHACh3Status = excelRow.GetCell(columnMapping["WHA Ch3 Exemption Code"])?.ToString() ?? string.Empty,
                        // WHACh4Status = excelRow.GetCell(columnMapping["WHA Ch4 Exemption Code"])?.ToString() ?? string.Empty,
                        RcpTIN = excelRow.GetCell(columnMapping["RcpTIN"])?.ToString() ?? string.Empty,
                        LastNameCompany = excelRow.GetCell(columnMapping["LastNameCompany"])?.ToString() ?? string.Empty,
                        FirstName = excelRow.GetCell(columnMapping["FirstName"])?.ToString() ?? string.Empty,
                        NameLine2 = excelRow.GetCell(columnMapping["NameLine2"])?.ToString() ?? string.Empty,
                        AddressType = excelRow.GetCell(columnMapping["AddressType"])?.ToString() ?? string.Empty,
                        AddressDelivStreet = excelRow.GetCell(columnMapping["AddressDelivStreet"])?.ToString() ?? string.Empty,
                        AddressAptSuite = excelRow.GetCell(columnMapping["AddressAptSuite"])?.ToString() ?? string.Empty,
                        City = excelRow.GetCell(columnMapping["City"])?.ToString() ?? string.Empty,
                        State = excelRow.GetCell(columnMapping["State"])?.ToString() ?? string.Empty,
                        Zip = excelRow.GetCell(columnMapping["Zip"])?.ToString() ?? string.Empty,
                        Country = excelRow.GetCell(columnMapping["Country"])?.ToString() ?? string.Empty,
                        RcpAccount = excelRow.GetCell(columnMapping["RcpAccount"])?.ToString() ?? string.Empty,
                        RcpEmail = excelRow.GetCell(columnMapping["RcpEmail"])?.ToString() ?? string.Empty,
                        Box1Amount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Box1Amount"])?.ToString() ?? "0.0"),
                        Box2Amount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Box2Amount"])?.ToString() ?? "0.0"),
                        Box3DATETIME = excelRow.GetCell(columnMapping["Box3DATETIME"])?.ToString() == null ? null : Convert.ToDateTime(excelRow.GetCell(columnMapping["Box3DATETIME"]).ToString()),
                        Box4Amount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Box4Amount"])?.ToString() ?? "0.0"),
                        Box5Amount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Box5Amount"])?.ToString() ?? "0.0"),
                        Box6Amount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Box6Amount"])?.ToString() ?? "0.0"),
                        Box7Checkbox = (excelRow.GetCell(columnMapping["Box7Checkbox"])?.ToString() != null && (bool)excelRow.GetCell(columnMapping["Box7Checkbox"])?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        Box8AllLines = excelRow.GetCell(columnMapping["Box8AllLines"])?.ToString() ?? string.Empty,
                        Box9Number = Convert.ToInt32(excelRow.GetCell(columnMapping["Box9Number"])?.ToString() ?? "0"),
                        Box10AllLines = excelRow.GetCell(columnMapping["Box10AllLines"])?.ToString() ?? string.Empty,
                        Box11DATETIME = excelRow.GetCell(columnMapping["Box11DATETIME"])?.ToString() == null ? null : Convert.ToDateTime(excelRow.GetCell(columnMapping["Box11DATETIME"]).ToString()),

                        FormCategory = excelRow.GetCell(columnMapping["FormCategory"])?.ToString() ?? string.Empty,
                        TaxState = excelRow.GetCell(columnMapping["TaxState"])?.ToString() ?? string.Empty,
                        Status=1,
                        Created_Date = DateTime.Now,
                        Created_By = UserId,
                        InstID = InstId,
                        EntityId = entityId,
                        UserId = UserId,
                    };
                    string clientEmailEINNumber = request.RcpTIN ?? string.Empty;
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
                    if (await _evolvedtaxContext.Tbl_1098.AnyAsync(p => p.RcpTIN == request.RcpTIN && p.EntityId == request.EntityId && p.Created_Date != null &&
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
                await _evolvedtaxContext.Tbl_1098.AddRangeAsync(AList);
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

            String ClientName = response.RcpFirstAndMI + " " + response.RcpNameLine2?.Replace(": ", "");

            if (!String.IsNullOrEmpty(Page))
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
            //var request = _evolvedtaxContext.Tbl_5498_SA.FirstOrDefault(p => p.Id == Id);
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
            //var request = _evolvedtaxContext.Tbl_5498_SA.FirstOrDefault(p => p.Id == Id);
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

            var recordToDelete = _evolvedtaxContext.Tbl_1098.First(p => p.Id == id);
            if (recordToDelete != null)
            {
                _evolvedtaxContext.Tbl_1098.Remove(recordToDelete);
                await _evolvedtaxContext.SaveChangesAsync();
                return new MessageResponseModel { Status = true };
            }

            return new MessageResponseModel { Status = false };
        }

        public async Task<MessageResponseModel> KeepRecord(int id)
        {

            var recordToUpdate = _evolvedtaxContext.Tbl_1098.First(p => p.Id == id);
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
