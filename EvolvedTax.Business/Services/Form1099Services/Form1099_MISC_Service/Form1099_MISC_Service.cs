using Azure.Core;
using Azure;
using EvolvedTax.Data.Models.Entities;
using iTextSharp.text.pdf;
using System.Diagnostics.Metrics;
using System.Globalization;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using iTextSharp.text.html.simpleparser;
using EvolvedTax.Data.Models.DTOs.Request;
using Microsoft.AspNetCore.Hosting.Server;
using System.Drawing;
using System.Xml.Linq;
using SkiaSharp;
using static iTextSharp.text.Font;
using EvolvedTax.Common.Constants;
using EvolvedTax.Business.Services.GeneralQuestionareService;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.Ocsp;
using EvolvedTax.Business.Services.GeneralQuestionareEntityService;
using System.Linq.Expressions;
using EvolvedTax.Data.Enums;
using EvolvedTax.Data.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using EvolvedTax.Data.Models.Entities._1099;
using EvolvedTax.Data.Models.DTOs.Response.Form1099;
using AutoMapper;
using EvolvedTax.Business.Services.InstituteService;

namespace EvolvedTax.Business.Services.Form1099Services
{
    public class Form1099_MISC_Service : IForm1099_MISC_Service
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly IInstituteService _instituteService;
        readonly IMapper _mapper;
        public Form1099_MISC_Service(EvolvedtaxContext evolvedtaxContext, IMapper mapper, IInstituteService instituteService)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _mapper = mapper;
            _instituteService = instituteService;
        }

        public IQueryable<Form1099MISCResponse> GetForm1099MISCList()
        {
            var response = _evolvedtaxContext.Tbl1099_MISC.Select(p => new Form1099MISCResponse
            {
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
                Rcp_Email = p.Rcp_Email,
                Rcp_TIN = p.Rcp_TIN,
                Second_TIN_Notice = p.Second_TIN_Notice,
                State = p.State,
                Status = p.Status,
                Tax_State = p.Tax_State,
                Uploaded_File = p.Uploaded_File,
                UserId = p.UserId,
                Zip = p.Zip
            });
            return response;
        }

        public async Task<MessageResponseModel> Upload1099_MISC_Data(IFormFile file, int InstId, string UserId)
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
                        FATCA_Checkbox = excelRow.GetCell(14)?.ToString() ?? string.Empty,
                        Box_1_Amount = excelRow.GetCell(15)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(15)?.ToString()) : 0,
                        Box_2_Amount = excelRow.GetCell(16)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(16)?.ToString()) : 0,
                        Box_3_Amount = excelRow.GetCell(17)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(17)?.ToString()) : 0,
                        Box_4_Amount = excelRow.GetCell(18)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(18)?.ToString()) : 0,
                        Box_5_Amount = excelRow.GetCell(19)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(19)?.ToString()) : 0,
                        Box_6_Amount = excelRow.GetCell(20)?.ToString() != null ? Convert.ToDecimal(excelRow.GetCell(20)?.ToString()) : 0,
                        Box_7_Checkbox = excelRow.GetCell(21)?.ToString() ?? string.Empty,
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
                        Created_By = UserId,
                        Created_Date = DateTime.Now,
                        InstID = InstId,
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
                    if (await _evolvedtaxContext.Tbl1099_MISC.AnyAsync(p => p.Rcp_TIN == request.Rcp_TIN))
                    {
                        response.Add(request);
                        Status = true;
                    }
                    else
                    {
                        mISCList.Add(request);
                    }
                }
                await _evolvedtaxContext.Tbl1099_MISC.AddRangeAsync(mISCList);
                await _evolvedtaxContext.SaveChangesAsync();
            }
            return new MessageResponseModel { Status = Status, Message = response, Param = "Database" };
        }

        public string GeneratePdf(string Id, string BasePath)
        {
            var mISCresponse = _evolvedtaxContext.Tbl1099_MISC.FirstOrDefault(p => p.Rcp_TIN.Trim() == Id.Trim());
            var instResponse = _instituteService.GetInstituteDataById((int)mISCresponse.InstID);
            string templatefile = BasePath + "/" + AppConstants.Form1099MISCTemplateFileName;
            string newFile = "/Form1099MISC/" + mISCresponse.First_Name + "_" + mISCresponse.Name_Line_2 + "_F1099MISC.pdf";
            PdfReader pdfReader = new PdfReader(templatefile);
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(BasePath + newFile, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;


            pdfFormFields.SetField("topmostSubform[0].CopyA[0].CopyAHeader[0].CalendarYear[0].f1_1[0]", "2023");   //CalYear
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].CopyAHeader[0].c1_1[0]", "");   //VOID
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].CopyAHeader[0].c1_1[1]", "");   //CORRECTED
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_2[0]", string.Concat(instResponse.InstitutionName, ", ", instResponse.Madd1, ", ", instResponse.Madd2, ", ", instResponse.Mcity, ", ", instResponse.Mstate, instResponse.Mprovince, ", ", instResponse.Mcountry, ", ", instResponse.Mzip, ", ", instResponse.Phone));   //PAYER’S name, street address, city or town, state or province, country, ZIP or foreign postal code, and telephone no
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_3[0]", instResponse.Ftin);   //Payer TIN
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_4[0]", mISCresponse.Rcp_TIN);   //Recepients TIN
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_5[0]", mISCresponse.First_Name + " " + mISCresponse.Name_Line_2);   //RECIPIENT’S name
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_6[0]", mISCresponse.Address_Deliv_Street + " " + mISCresponse.Address_Apt_Suite);   //Street address (including apt. no.)
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_7[0]", mISCresponse.City + " " + mISCresponse.State + " " + mISCresponse.Country + " " + mISCresponse.Zip);   //City or town, state or province, country, and ZIP or foreign postal code
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_8[0]", mISCresponse.Rcp_Account);   //Account number (see instructions)
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].c1_2[0]", mISCresponse.Second_TIN_Notice);   //2nd TIN not.
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_9[0]", mISCresponse.Box_1_Amount.ToString());   //1 Rents
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_10[0]", mISCresponse.Box_2_Amount.ToString());   //2 Royalties
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_11[0]", mISCresponse.Box_3_Amount.ToString());   //3 Other income
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_12[0]", mISCresponse.Box_4_Amount.ToString());   //4 Federal income tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_13[0]", mISCresponse.Box_5_Amount.ToString());   //5 Fishing boat proceeds
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_14[0]", mISCresponse.Box_6_Amount.ToString());   //6 Medical and health care payments
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].c1_4[0]", mISCresponse.Box_7_Checkbox);   //7 CheckBox Payer made direct sales totaling $5,000 or more of consumer products to recipient for resale
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_15[0]", mISCresponse.Box_8_Amount.ToString());   //8 Substitute payments in lieu of dividends or interest
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_16[0]", mISCresponse.Box_9_Amount.ToString());   //9 Crop insurance proceeds
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_17[0]", mISCresponse.Box_10_Amount.ToString());   //10 Gross proceeds paid to an attorney
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_18[0]", mISCresponse.Box_11_Amount.ToString());   //11 Fish purchased for resale
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_19[0]", mISCresponse.Box_12_Amount.ToString());   //12 Section 409A deferrals
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].TagCorrectingSubform[0].c1_3[0]", "");   //13 FATCA filing requirement
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].Box14_ReadOrder[0].f1_20[0]", mISCresponse.Box_14_Amount.ToString()); //14 Excess golden parachute payments
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_21[0]", mISCresponse.Box_15_Amount.ToString());   //15 Nonqualified deferred compensation
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].Box16_ReadOrder[0].f1_22[0]", mISCresponse.Box_16_Amount.ToString());   //16 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].Box16_ReadOrder[0].f1_23[0]", mISCresponse.Box_16_Amount.ToString());   //16 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].Box17_ReadOrder[0].f1_24[0]", mISCresponse.Box_17_ID_Number);   //17 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].Box17_ReadOrder[0].f1_25[0]", mISCresponse.Box_17_State);   //17 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].f1_26[0]", mISCresponse.Box_18_Amount.ToString());   //18 State income
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].f1_27[0]", mISCresponse.Box_18_Amount.ToString());   //18 State income
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].Copy1Header[0].CalendarYear[0].f2_1[0]", "2023");   //CalYear
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].Copy1Header[0].c2_1[0]", "");   //VOID
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].Copy1Header[0].c2_1[1]", "");   //CORRECTED
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
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].c2_4[0]", mISCresponse.Box_7_Checkbox);   //7 CheckBox Payer made direct sales totaling $5,000 or more of consumer products to recipient for resale
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_15[0]", mISCresponse.Box_8_Amount.ToString());   //8 Substitute payments in lieu of dividends or interest
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_16[0]", mISCresponse.Box_9_Amount.ToString());   //9 Crop insurance proceeds
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_17[0]", mISCresponse.Box_10_Amount.ToString());   //10 Gross proceeds paid to an attorney
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_18[0]", mISCresponse.Box_11_Amount.ToString());   //11 Fish purchased for resale
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_19[0]", mISCresponse.Box_12_Amount.ToString());   //12 Section 409A deferrals
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].TagCorrectingSubform[0].c2_3[0]", "");   //13 FATCA filing requirement
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].Box14_ReadOrder[0].f2_20[0]", mISCresponse.Box_14_Amount.ToString());   //12 Section 409A deferrals);   //14 Excess golden parachute payments
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_21[0]", mISCresponse.Box_15_Amount.ToString());   //15 Nonqualified deferred compensation
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].Box16_ReadOrder[0].f2_22[0]", mISCresponse.Box_16_Amount.ToString());   //16 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].Box16_ReadOrder[0].f2_23[0]", mISCresponse.Box_16_Amount.ToString());   //16 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].Box17_ReadOrder[0].f2_24[0]", mISCresponse.Box_17_ID_Number);   //17 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].Box17_ReadOrder[0].f2_25[0]", mISCresponse.Box_17_State);   //17 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].f2_26[0]", "StateIncome1");   //18 State income
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].f2_27[0]", "StateIncome2");   //18 State income
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].CopyBHeader[0].CalendarYear[0].f2_1[0]", "2023");   //CalYear
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].CopyBHeader[0].c2_1[0]", "");   //CORRECTED
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
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].c2_4[0]", mISCresponse.Box_7_Checkbox);   //7 CheckBox Payer made direct sales totaling $5,000 or more of consumer products to recipient for resale
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_15[0]", mISCresponse.Box_8_Amount.ToString());   //8 Substitute payments in lieu of dividends or interest
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_16[0]", mISCresponse.Box_9_Amount.ToString());   //9 Crop insurance proceeds
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_17[0]", mISCresponse.Box_10_Amount.ToString());   //10 Gross proceeds paid to an attorney
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_18[0]", mISCresponse.Box_11_Amount.ToString());   //11 Fish purchased for resale
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_19[0]", mISCresponse.Box_12_Amount.ToString());   //12 Section 409A deferrals
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].TagCorrectingSubform[0].c2_3[0]", "");   //13 FATCA filing requirement
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].Box14_ReadOrder[0].f2_20[0]", mISCresponse.Box_14_Amount.ToString());   //12 Section 409A deferrals);   //14 Excess golden parachute payments
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_21[0]", mISCresponse.Box_15_Amount.ToString());   //15 Nonqualified deferred compensation
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].Box16_ReadOrder[0].f2_22[0]", mISCresponse.Box_16_Amount.ToString());   //16 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].Box16_ReadOrder[0].f2_23[0]", mISCresponse.Box_16_Amount.ToString());   //16 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].Box17_ReadOrder[0].f2_24[0]", mISCresponse.Box_17_ID_Number);   //17 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].Box17_ReadOrder[0].f2_25[0]", mISCresponse.Box_17_State);   //17 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].f2_26[0]", "StateIncome1");   //18 State income
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].f2_27[0]", "StateIncome2");   //18 State income
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].Copy2Header[0].CalendarYear[0].f2_1[0]", "2023");   //CalYear
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].Copy2Header[0].c2_1[0]", "");   //CORRECTED
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
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].c2_4[0]", mISCresponse.Box_7_Checkbox);   //7 CheckBox Payer made direct sales totaling $5,000 or more of consumer products to recipient for resale
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_15[0]", mISCresponse.Box_8_Amount.ToString());   //8 Substitute payments in lieu of dividends or interest
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_16[0]", mISCresponse.Box_9_Amount.ToString());   //9 Crop insurance proceeds
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_17[0]", mISCresponse.Box_10_Amount.ToString());   //10 Gross proceeds paid to an attorney
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_18[0]", mISCresponse.Box_11_Amount.ToString());   //11 Fish purchased for resale
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_19[0]", mISCresponse.Box_12_Amount.ToString());   //12 Section 409A deferrals
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].TagCorrectingSubform[0].c2_3[0]", "");   //13 FATCA filing requirement
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].Box14_ReadOrder[0].f2_20[0]", mISCresponse.Box_14_Amount.ToString());   //12 Section 409A deferrals);   //14 Excess golden parachute payments
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_21[0]", mISCresponse.Box_15_Amount.ToString());   //15 Nonqualified deferred compensation
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].Box16_ReadOrder[0].f2_22[0]", mISCresponse.Box_16_Amount.ToString());   //16 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].Box16_ReadOrder[0].f2_23[0]", mISCresponse.Box_16_Amount.ToString());   //16 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].Box17_ReadOrder[0].f2_24[0]", mISCresponse.Box_17_ID_Number);   //17 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].Box17_ReadOrder[0].f2_25[0]", mISCresponse.Box_17_State);   //17 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].f2_26[0]", "StateIncome1");   //18 State income
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].f2_27[0]", "StateIncome2");   //18 State income
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].CopyCHeader[0].CalendarYear[0].f2_1[0]", "2023");   //CalYear
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].CopyCHeader[0].c2_1[0]", "");   //VOID
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].CopyCHeader[0].c2_1[1]", "");   //CORRECTED
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_2[0]", string.Concat(instResponse.InstitutionName, ", ", instResponse.Madd1, ", ", instResponse.Madd2, ", ", instResponse.Mcity, ", ", instResponse.Mstate, instResponse.Mprovince, ", ", instResponse.Mcountry, ", ", instResponse.Mzip, ", ", instResponse.Phone));   //PAYER’S name, street address, city or town, state or province, country, ZIP or foreign postal code, and telephone no
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_3[0]", instResponse.Ftin);   //Payer TIN
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_4[0]", mISCresponse.Rcp_TIN);   //Recepients TIN
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_5[0]", mISCresponse.First_Name + " " + mISCresponse.Name_Line_2);   //RECIPIENT’S name
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_6[0]", mISCresponse.Address_Deliv_Street + " " + mISCresponse.Address_Apt_Suite);   //Street address (including apt. no.)
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_7[0]", mISCresponse.City + " " + mISCresponse.State + " " + mISCresponse.Country + " " + mISCresponse.Zip);   //City or town, state or province, country, and ZIP or foreign postal code
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_8[0]", mISCresponse.Rcp_Account);   //Account number (see instructions)
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].c2_2[0]", mISCresponse.Second_TIN_Notice);   //2nd TIN not.
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_9[0]", mISCresponse.Box_1_Amount.ToString());   //1 Rents
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_10[0]", mISCresponse.Box_2_Amount.ToString());   //2 Royalties
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_11[0]", mISCresponse.Box_3_Amount.ToString());   //3 Other income
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_12[0]", mISCresponse.Box_4_Amount.ToString());   //4 Federal income tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_13[0]", mISCresponse.Box_5_Amount.ToString());   //5 Fishing boat proceeds
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_14[0]", mISCresponse.Box_6_Amount.ToString());   //6 Medical and health care payments
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].c2_4[0]", mISCresponse.Box_7_Checkbox);   //7 CheckBox Payer made direct sales totaling $5,000 or more of consumer products to recipient for resale
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_15[0]", mISCresponse.Box_8_Amount.ToString());   //8 Substitute payments in lieu of dividends or interest
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_16[0]", mISCresponse.Box_9_Amount.ToString());   //9 Crop insurance proceeds
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_17[0]", mISCresponse.Box_10_Amount.ToString());   //10 Gross proceeds paid to an attorney
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_18[0]", mISCresponse.Box_11_Amount.ToString());   //11 Fish purchased for resale
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_19[0]", mISCresponse.Box_12_Amount.ToString());   //12 Section 409A deferrals
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].TagCorrectingSubform[0].c2_3[0]", "");   //13 FATCA filing requirement
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].Box14_ReadOrder[0].f2_20[0]", mISCresponse.Box_14_Amount.ToString());   //12 Section 409A deferrals);   //14 Excess golden parachute payments
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_21[0]", mISCresponse.Box_15_Amount.ToString());   //15 Nonqualified deferred compensation
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].Box16_ReadOrder[0].f2_22[0]", mISCresponse.Box_16_Amount.ToString());   //16 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].Box16_ReadOrder[0].f2_23[0]", mISCresponse.Box_16_Amount.ToString());   //16 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].Box17_ReadOrder[0].f2_24[0]", mISCresponse.Box_17_ID_Number);   //17 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].Box17_ReadOrder[0].f2_25[0]", mISCresponse.Box_17_State);   //17 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].f2_26[0]", "StateIncome1");   //18 State income
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].f2_27[0]", "StateIncome2");   //18 State income

            pdfStamper.FormFlattening = true;
            pdfStamper.Close();
            return newFile;
        }
    }
}
