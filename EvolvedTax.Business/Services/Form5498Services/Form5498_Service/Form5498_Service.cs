﻿using AutoMapper;
using EvolvedTax.Business.MailService;
using EvolvedTax.Business.Services.Form1099Services;
using EvolvedTax.Business.Services.Form5498ervices;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs;
using EvolvedTax.Data.Models.DTOs.Response.Form1042;
using EvolvedTax.Data.Models.DTOs.Response.Form5498;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Data.Models.Entities._1042;
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

namespace EvolvedTax.Business.Services.Form5498Services
{
    public class Form5498_Service : IForm5498_Service
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly IInstituteService _instituteService;
        readonly IMailService _mailService;
        readonly ITrailAudit1099Service _trailAudit1099Service;
        readonly IMapper _mapper;
        public Form5498_Service(EvolvedtaxContext evolvedtaxContext, IMapper mapper, IInstituteService instituteService, IMailService mailService = null, ITrailAudit1099Service trailAudit1099Service = null)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _mapper = mapper;
            _instituteService = instituteService;
            _mailService = mailService;
            _trailAudit1099Service = trailAudit1099Service;
        }

        public IQueryable<Form5498Response> GetForm5498List()
        {
            var response = _evolvedtaxContext.Tbl_5498.Select(p => new Form5498Response
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
                Box3Amount = p.Box3Amount,
                Box4Amount = p.Box4Amount,
                Box5Amount = p.Box5Amount,
                Box6Amount = p.Box6Amount,
                Box7Checkbox1 = p.Box7Checkbox1,
                Box7Checkbox2 = p.Box7Checkbox2,
                Box7Checkbox3 = p.Box7Checkbox3,
                Box7Checkbox4 = p.Box7Checkbox4,
                Box8Amount = p.Box8Amount,
                Box9Amount = p.Box9Amount,
                Box10Amount = p.Box10Amount,
                Box11Checkbox = p.Box11Checkbox,
                Box12aDate = p.Box12aDate,
                Box12bAmount = p.Box12bAmount,
                Box13aAmount = p.Box13aAmount,
                Box13bYear = p.Box13bYear,
                Box13cCode = p.Box13cCode,
                Box14aAmount = p.Box14aAmount,
                Box14bCode = p.Box14bCode,
                Box15aAmount = p.Box15aAmount,
                Box15bCode = p.Box15bCode,
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
            var result = from ic in _evolvedtaxContext.Tbl_5498_SA
                         where selectValues.Contains(ic.Id) && !string.IsNullOrEmpty(ic.RcpEmail)
                         select new Tbl_5498_SA
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
        public async Task<MessageResponseModel> Upload5498_Data(IFormFile file, int InstId, int entityId, string UserId)
        {
            bool Status = false;
            var response = new List<Tbl_5498>();
            var AList = new List<Tbl_5498>();
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

                    var request = new Tbl_5498
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
                        Box3Amount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Box3Amount"])?.ToString() ?? "0.0"),
                        Box4Amount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Box4Amount"])?.ToString() ?? "0.0"),
                        Box5Amount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Box5Amount"])?.ToString() ?? "0.0"),
                        Box6Amount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Box6Amount"])?.ToString() ?? "0.0"),
                        Box7Checkbox1 = (excelRow.GetCell(columnMapping["Box7Checkbox1"])?.ToString() != null && (bool)excelRow.GetCell(columnMapping["Box7Checkbox1"])?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        Box7Checkbox2 = (excelRow.GetCell(columnMapping["Box7Checkbox2"])?.ToString() != null && (bool)excelRow.GetCell(columnMapping["Box7Checkbox2"])?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        Box7Checkbox3 = (excelRow.GetCell(columnMapping["Box7Checkbox3"])?.ToString() != null && (bool)excelRow.GetCell(columnMapping["Box7Checkbox3"])?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        Box7Checkbox4 = (excelRow.GetCell(columnMapping["Box7Checkbox4"])?.ToString() != null && (bool)excelRow.GetCell(columnMapping["Box7Checkbox4"])?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        Box8Amount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Box8Amount"])?.ToString() ?? "0.0"),
                        Box9Amount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Box9Amount"])?.ToString() ?? "0.0"),
                        Box10Amount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Box10Amount"])?.ToString() ?? "0.0"),
                        Box11Checkbox = (excelRow.GetCell(columnMapping["Box11Checkbox"])?.ToString() != null && (bool)excelRow.GetCell(columnMapping["Box11Checkbox"])?.ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase)) ? "1" : "0",
                        Box12aDate = excelRow.GetCell(columnMapping["Box12aDate"])?.ToString() == null ? null : Convert.ToDateTime(excelRow.GetCell(columnMapping["Box12aDate"]).ToString()),
                        Box12bAmount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Box12bAmount"])?.ToString() ?? "0.0"),
                        Box13aAmount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Box13aAmount"])?.ToString() ?? "0.0"),
                        Box13bYear = Convert.ToInt32(excelRow.GetCell(columnMapping["Box13bYear"])?.ToString() ?? "0"),
                        Box13cCode = excelRow.GetCell(columnMapping["Box13cCode"])?.ToString() ?? string.Empty,
                        Box14aAmount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Box14aAmount"])?.ToString() ?? "0.0"),
                        Box14bCode = excelRow.GetCell(columnMapping["Box14bCode"])?.ToString() ?? string.Empty,
                        Box15aAmount = Convert.ToDecimal(excelRow.GetCell(columnMapping["Box15aAmount"])?.ToString() ?? "0.0"),
                        Box15bCode = excelRow.GetCell(columnMapping["Box15bCode"])?.ToString() ?? string.Empty,

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
                    if (await _evolvedtaxContext.Tbl_5498.AnyAsync(p => p.RcpTIN == request.RcpTIN && p.EntityId == request.EntityId && p.Created_Date != null &&
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
                await _evolvedtaxContext.Tbl_5498.AddRangeAsync(AList);
                await _evolvedtaxContext.SaveChangesAsync();
            }
            return new MessageResponseModel { Status = Status, Message = response, Param = "Database" };
            //return new MessageResponseModel { Status = Status, Message = response, Param = "Entity" };
        }


        #region PDF Methods
        public string GeneratePdf(int Id, string TemplatefilePath, string SaveFolderPath, int entityId)
        {
            return CreatePdf(Id, TemplatefilePath, SaveFolderPath, entityId, false);

        }

        public string CreatePdf(int Id, string TemplatefilePath, string SaveFolderPath, int entityId, bool IsAll, string Page = "")
        {
            var response = _evolvedtaxContext.Tbl_3921.FirstOrDefault(p => p.Id == Id);
            //var requestInstitue = _instituteService.GetPayeeData((int)response.InstID);
            var entityData = _instituteService.GetEntityDataById(entityId);
            string templatefile = TemplatefilePath;
            string newFile1 = string.Empty;

            string ClientName = response.FirstName + " " + response.LastNameCompany?.Replace(": ", "");

            if (!string.IsNullOrEmpty(Page))
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.Form5498, "_", response.Id, "_Page_", Page);
            }
            else
            {
                newFile1 = string.Concat(ClientName, "_", AppConstants.Form5498, "_", response.Id);
            }


            string FilenameNew = "/Form5498/" + newFile1 + ".pdf";
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
            if (response.IsCorrected == 0)
            {
                //pdfFormFields.SetField("topmostSubform.CopyA.CopyHeader.c1_1", "0");
                pdfFormFields.SetField("efield1_topmostSubform.CopyA.CopyHeader.c1_1", "0");
            }
            else
            {
                pdfFormFields.SetField("topmostSubform.CopyA.CopyHeader.c1_1", "0");
                // pdfFormFields.SetField("efield1_topmostSubform.CopyA.CopyHeader.c1_1", "1");
            }
            pdfFormFields.SetField("topmostSubform.CopyA.LftCol.f1_1", string.Concat(entityData.EntityName, " ", entityData.Address1, " ", entityData.Address2, " ", entityData.City, " ", entityData.State, entityData.Province, " ", entityData.Zip));
            pdfFormFields.SetField("topmostSubform.CopyA.LftCol.f1_2", entityData.Ein);
            pdfFormFields.SetField("topmostSubform.CopyA.LftCol.f1_3", response.RcpTIN);
            pdfFormFields.SetField("topmostSubform.CopyA.LftCol.f1_4", response.FirstName + response.NameLine2);
            pdfFormFields.SetField("topmostSubform.CopyA.LftCol.f1_5", response.AddressDelivStreet + " " + response.AddressAptSuite);
            pdfFormFields.SetField("topmostSubform.CopyA.LftCol.f1_6", response.City + " " + response.State + " " + response.Zip + " " + response.Country);
            pdfFormFields.SetField("topmostSubform.CopyA.LftCol.f1_7", response.RcpAccount);
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.f1_8", response.Box1Date?.ToString("MM/dd/yyyy"));
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.f1_9", response.Box2Date?.ToString("MM/dd/yyyy"));
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.f1_10", response.Box3Amount?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.f1_11", response.Box4Amount?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.f1_12", response.Box5Number?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyA.RghtCol.f1_13", response.Box6AllLines);
            if (response.IsCorrected == 0)
            {
                pdfFormFields.SetField("topmostSubform.CopyB.CopyBHeader.c2_1", "1");
            }
            else
            {
                pdfFormFields.SetField("topmostSubform.CopyB.CopyBHeader.c2_1", "0");
            }
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.f2_1", string.Concat(entityData.EntityName, " ", entityData.Address1, " ", entityData.Address2, " ", entityData.City, " ", entityData.State, entityData.Province, " ", entityData.Zip));
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.f2_2", entityData.Ein);
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.f2_3", response.RcpTIN);
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.f2_4", response.FirstName + response.NameLine2);
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.f2_5", response.AddressDelivStreet + " " + response.AddressAptSuite);
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.f2_6", response.City + " " + response.State + " " + response.Zip + " " + response.Country);
            pdfFormFields.SetField("topmostSubform.CopyB.LeftColumn.f2_7", response.RcpAccount);
            pdfFormFields.SetField("topmostSubform.CopyB.RghtCol.f2_8", response.Box1Date?.ToString("MM/dd/yyyy"));
            pdfFormFields.SetField("topmostSubform.CopyB.RghtCol.f2_9", response.Box2Date?.ToString("MM/dd/yyyy"));
            pdfFormFields.SetField("topmostSubform.CopyB.RghtCol.f2_10", response.Box3Amount?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyB.RghtCol.f2_11", response.Box4Amount?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyB.RghtCol.f2_12", response.Box5Number?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyB.RghtCol.f2_13", response.Box6AllLines);
            if (response.IsCorrected == 0)
            {
                pdfFormFields.SetField("topmostSubform.CopyC.CopyCHeader.c2_1", "1");
            }
            else
            {
                pdfFormFields.SetField("topmostSubform.CopyC.CopyCHeader.c2_1", "0");
            }
            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.f2_1", string.Concat(entityData.EntityName, " ", entityData.Address1, " ", entityData.Address2, " ", entityData.City, " ", entityData.State, entityData.Province, " ", entityData.Zip));
            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.f2_2", entityData.Ein);
            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.f2_3", response.RcpTIN);
            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.f2_4", response.FirstName + response.NameLine2);
            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.f2_5", response.AddressDelivStreet + " " + response.AddressAptSuite);
            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.f2_6", response.City + " " + response.State + " " + response.Zip + " " + response.Country);
            pdfFormFields.SetField("topmostSubform.CopyC.LeftColumn.f2_7", response.RcpAccount);
            pdfFormFields.SetField("topmostSubform.CopyC.RghtCol.f2_8", response.Box1Date?.ToString("MM/dd/yyyy"));
            pdfFormFields.SetField("topmostSubform.CopyC.RghtCol.f2_9", response.Box2Date?.ToString("MM/dd/yyyy"));
            pdfFormFields.SetField("topmostSubform.CopyC.RghtCol.f2_10", response.Box3Amount?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyC.RghtCol.f2_11", response.Box4Amount?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyC.RghtCol.f2_12", response.Box5Number?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyC.RghtCol.f2_13", response.Box6AllLines);
            if (response.IsCorrected == 0)
            {
                //pdfFormFields.SetField("topmostSubform.CopyD.CopyDHeader.c2_1", "Not(IsCorrected)");
                pdfFormFields.SetField("efield44_topmostSubform.CopyD.CopyDHeader.c2_1", "0");
            }
            else
            {
                pdfFormFields.SetField("topmostSubform.CopyD.CopyDHeader.c2_1", "0");
                //pdfFormFields.SetField("efield44_topmostSubform.CopyD.CopyDHeader.c2_1", "IsCorrected");
            }

            pdfFormFields.SetField("topmostSubform.CopyD.LeftColumn.f2_1", string.Concat(entityData.EntityName, " ", entityData.Address1, " ", entityData.Address2, " ", entityData.City, " ", entityData.State, entityData.Province, " ", entityData.Zip));
            pdfFormFields.SetField("topmostSubform.CopyD.LeftColumn.f2_2", entityData.Ein);
            pdfFormFields.SetField("topmostSubform.CopyD.LeftColumn.f2_3", response.RcpTIN);
            pdfFormFields.SetField("topmostSubform.CopyD.LeftColumn.f2_4", response.FirstName + response.NameLine2);
            pdfFormFields.SetField("topmostSubform.CopyD.LeftColumn.f2_5", response.AddressDelivStreet + " " + response.AddressAptSuite);
            pdfFormFields.SetField("topmostSubform.CopyD.LeftColumn.f2_6", response.City + " " + response.State + " " + response.Zip + " " + response.Country);
            pdfFormFields.SetField("topmostSubform.CopyD.LeftColumn.f2_7", response.RcpAccount);
            pdfFormFields.SetField("topmostSubform.CopyD.RghtCol.f2_8", response.Box1Date?.ToString("MM/dd/yyyy"));
            pdfFormFields.SetField("topmostSubform.CopyD.RghtCol.f2_9", response.Box2Date?.ToString("MM/dd/yyyy"));
            pdfFormFields.SetField("topmostSubform.CopyD.RghtCol.f2_10", response.Box3Amount?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyD.RghtCol.f2_11", response.Box4Amount?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyD.RghtCol.f2_12", response.Box5Number?.ToString());
            pdfFormFields.SetField("topmostSubform.CopyD.RghtCol.f2_13", response.Box6AllLines);
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

        public string GeneratePdfForSpecificPage(int Id, string TemplatefilePath, string SaveFolderPath, List<string> selectedPages, int entityId)
        {
            string newFile1 = string.Empty;
            var request = _evolvedtaxContext.Tbl_3921.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.FirstName + " " + request.NameLine2?.Replace(": ", "");
            newFile1 = string.Concat(ClientName, "_", AppConstants.Form5498, "_", Id);
            string FilenameNew = "/Form5498/" + newFile1 + ".pdf";
            string newFileName = newFile1 + ".pdf";



            var newFilePath = CreatePdf(Id, TemplatefilePath, SaveFolderPath, entityId, true);

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
            //return null;
        }
        public string GeneratePdForSpecificType(int Id, string TemplatefilePath, string SaveFolderPath, string selectedPage, int entityId = 0)
        {
            string newFile1 = string.Empty;
            var request = _evolvedtaxContext.Tbl_3921.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.FirstName + " " + request.NameLine2?.Replace(": ", "");

            newFile1 = string.Concat(ClientName, "_", AppConstants.Form5498, "_", request.Id, "_Page_", selectedPage);
            string FilenameNew = "/Form5498/" + newFile1 + ".pdf";
            string newFileName = newFile1 + ".pdf";

            var newFilePath = CreatePdf(Id, TemplatefilePath, SaveFolderPath, entityId, true, selectedPage);

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
            //return null;
        }
        public string DownloadOneFile(List<int> ids, string SaveFolderPath, List<string> selectedPages, string RootPath, int entityId = 0)
        {
            var pdfPaths = new List<string>();
            var CompilepdfPaths = new List<string>();
            string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.Form5498TemplateFileName);
            bool containsAll = selectedPages.Contains("All");

            if (containsAll)
            {
                foreach (var id in ids)
                {
                    var pdfPath = CreatePdf(id, TemplatePathFile, SaveFolderPath, entityId, true);
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
                        var pdfPath = GeneratePdForSpecificType(id, TemplatePathFile, SaveFolderPath, selectedPage, entityId);
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
                            compileFileName = "For Borrower.pdf";
                            break;
                        case "5":
                            compileFileName = "For Lender.pdf";
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
            //return null;
        }
        public string GenerateAndZipPdfs(List<int> ids, string SaveFolderPath, List<string> selectedPages, string RootPath, int entityId)
        {
            var pdfPaths = new List<string>();

            foreach (var id in ids)
            {

                string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.Form5498TemplateFileName);
                bool containsAll = selectedPages.Contains("All");

                if (containsAll)
                {
                    var pdfPath = CreatePdf(id, TemplatePathFile, SaveFolderPath, entityId, true);
                    pdfPaths.Add(pdfPath);
                }
                else
                {
                    var pdfPath = GeneratePdfForSpecificPage(id, TemplatePathFile, SaveFolderPath, selectedPages, entityId);
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
            //return null;
        }
        #endregion

        public async Task<MessageResponseModel> DeletePermeant(int id)
        {

            var recordToDelete = _evolvedtaxContext.Tbl_5498_SA.First(p => p.Id == id);
            if (recordToDelete != null)
            {
                _evolvedtaxContext.Tbl_5498_SA.Remove(recordToDelete);
                await _evolvedtaxContext.SaveChangesAsync();
                return new MessageResponseModel { Status = true };
            }

            return new MessageResponseModel { Status = false };
        }

        public async Task<MessageResponseModel> KeepRecord(int id)
        {

            var recordToUpdate = _evolvedtaxContext.Tbl_5498_SA.First(p => p.Id == id);
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
