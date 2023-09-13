﻿using Azure.Core;
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
using EvolvedTax.Data.Models.DTOs.Response;
using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax.Business.Services.Form1099Services
{
    public class Form1099_NEC_Service :IForm1099_NEC_Service
    {
        readonly EvolvedtaxContext _evolvedtaxContext;


        public Form1099_NEC_Service(EvolvedtaxContext evolvedtaxContext)
        {
            _evolvedtaxContext = evolvedtaxContext;
        }
        public async Task<MessageResponseModel> Upload1099_NEC_Data(IFormFile file, int InstId, string UserId)
        {
            bool Status = false;
            var response = new List<Tbl1099_NEC>();
            var necList = new List<Tbl1099_NEC>();
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

                    string cell_value_15 = excelRow.GetCell(15)?.ToString();
                    string Box_2_Checkbox = string.IsNullOrEmpty(cell_value_15) ? "0" : (cell_value_15.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");


                    string cell_value_27 = excelRow.GetCell(27)?.ToString();
                    string Corrected = string.IsNullOrEmpty(cell_value_27) ? "0" : (cell_value_27.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0");

                    var entity = new Tbl1099_NEC
                    {
                        Rcp_TIN = excelRow.GetCell(0)?.ToString(),
                        Last_Name_Company = excelRow.GetCell(1)?.ToString(),
                        First_Name = excelRow.GetCell(2)?.ToString(),
                        Name_Line2 = excelRow.GetCell(3)?.ToString(),
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
                        Box_1_Amount = excelRow.GetCell(14) != null ? Convert.ToDecimal(excelRow.GetCell(14).ToString()) : (decimal?)null,
                        Box_2_Checkbox = Box_2_Checkbox,
                        Box_4_Amount = excelRow.GetCell(16) != null ? Convert.ToDecimal(excelRow.GetCell(16).ToString()) : (decimal?)null,
                        Box_5_Amount = excelRow.GetCell(17) != null ? Convert.ToDecimal(excelRow.GetCell(17).ToString()) : (decimal?)null,
                        Box_6_IDNumber = excelRow.GetCell(18)?.ToString(),
                        Box_6_State = excelRow.GetCell(19)?.ToString(),
                        Box_7_Amount = excelRow.GetCell(20) != null ? Convert.ToDecimal(excelRow.GetCell(20).ToString()) : (decimal?)null,
                        OptRcpTextLine1 = excelRow.GetCell(21)?.ToString(),
                        OptRcpTextLine2 = excelRow.GetCell(22)?.ToString(),
                        Form_Category = excelRow.GetCell(23)?.ToString(),
                        Form_Source = excelRow.GetCell(24)?.ToString(),
                        BatchID = excelRow.GetCell(25)?.ToString(),
                        Tax_State = excelRow.GetCell(26)?.ToString(),
                        InstID = InstId,
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
                    if (await _evolvedtaxContext.Tbl1099_NEC.AnyAsync(p =>
                        p.Rcp_TIN == entity.Rcp_TIN))
                    {
                        response.Add(entity);
                        Status = true;
                        entity.IsDuplicated = true;
                    }
                    else
                    {
                        entity.IsDuplicated = false;
                        

                    }
                    necList.Add(entity);
                }

                await _evolvedtaxContext.Tbl1099_NEC.AddRangeAsync(necList);
                await _evolvedtaxContext.SaveChangesAsync();


                return new MessageResponseModel { Status = Status, Message = response, Param = "Entity" };
            }

        }

        public IQueryable<Tbl1099_NEC> GetRecodByInstId(int InstId)
        {
            return _evolvedtaxContext.Tbl1099_NEC
            .Where(record => record.InstID == InstId)
            .AsQueryable();
        }


        public string GeneratePdf(int Id, string TemplatefilePath, string SaveFolderPath)
        {
            return CreatePdf(Id, TemplatefilePath, SaveFolderPath, false);

        }

        public string CreatePdf(int Id, string TemplatefilePath, string SaveFolderPath,bool IsAll, string Page = "")
        {
            var request = _evolvedtaxContext.Tbl1099_NEC.FirstOrDefault(p => p.Id == Id);
            var requestInstitue = _evolvedtaxContext.InstituteMasters.FirstOrDefault(p => p.InstId == request.InstID);
            string templatefile = TemplatefilePath;
            string newFile1 = string.Empty;

            String ClientName = request.First_Name + " " + request.Name_Line2;

            if (!String.IsNullOrEmpty(Page))
            {
                newFile1 = string.Concat(ClientName, "_", "Form_", AppConstants.NEC1099Form, "_", request.Id, "_Page_", Page);
            }
            else
            {
                newFile1 = string.Concat(ClientName, "_", "Form_", AppConstants.NEC1099Form, "_", request.Id);
            }

      
            string FilenameNew = "/1099NEC/" + newFile1 + ".pdf";
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

            pdfFormFields.SetField("topmostSubform[0].CopyA[0].PgHeader[0].CalendarYear[0].f1_1[0]", currentYear);   //CalYear
            if (request.Corrected.ToString() == "1")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyA[0].PgHeader[0].c1_1[0]", "0");   //VOID
            }
            if (request.Corrected.ToString() == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyA[0].PgHeader[0].c1_1[1]", "0");   //CORRECTED
            }
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_2[0]", PayData);   //PAYER’S name, street address, city or town, state or province, country, ZIP or foreign postal code, and telephone no
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_3[0]", requestInstitue.Idnumber);   //Payer TIN (IDNYMber
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_4[0]", request.Rcp_TIN);   //Recepients TIN
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_5[0]", request.First_Name + " " + request.Name_Line2);   //RECIPIENT’S name
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_6[0]", RecipentAddress);   //Street address (including apt. no.)
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_7[0]", RecipentCity);   //City or town, state or province, country, and ZIP or foreign postal code
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_8[0]", request.Rcp_Account);   //Account number (see instructions)
            if (request.Second_TIN_Notice != "1")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].c1_2[0]", "1");   //2nd TIN not.

            }
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_9[0]", request.Box_1_Amount.HasValue ? request.Box_1_Amount.Value.ToString("0.00") : string.Empty);   //1 Nonemployee compensation

            if (request.Box_2_Checkbox.ToString() != "1")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].c1_3[0]", "1");   //2 CheckBox Payer made direct sales totaling $5,000 or more of consumer products to recipient for resale
            }
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_10[0]", request.Box_4_Amount.HasValue ? request.Box_4_Amount.Value.ToString("0.00") : string.Empty);   //4 Federal income tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_11[0]", request.Box_5_Amount.HasValue ? request.Box_5_Amount.Value.ToString("0.00") : string.Empty);   //5 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_12[0]", request.Box_5_Amount.HasValue ? request.Box_5_Amount.Value.ToString("0.00") : string.Empty);   //5 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_13[0]", request.Box_6_State ?? string.Empty);   //6 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_14[0]", request.Box_6_State ?? string.Empty);   //6 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_15[0]", request.Box_7_Amount.HasValue ? request.Box_7_Amount.Value.ToString("0.00") : string.Empty);   //7 State income
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_16[0]", request.Box_7_Amount.HasValue ? request.Box_7_Amount.Value.ToString("0.00") : string.Empty);   //7 State income


            pdfFormFields.SetField("topmostSubform[0].Copy1[0].Copy1Header[0].CalendarYear[0].f2_1[0]", currentYear);   //CalYear
            if (request.Corrected.ToString() == "1")
            {
                pdfFormFields.SetField("topmostSubform[0].Copy1[0].Copy1Header[0].c2_1[0]", "1");   //VOID
            }
            if (request.Corrected.ToString() == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].Copy1[0].Copy1Header[0].c2_1[1]", "1");   //CORRECTED
            }
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].LeftColumn[0].f2_2[0]", PayData);   //PAYER’S name, street address, city or town, state or province, country, ZIP or foreign postal code, and telephone no
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].LeftColumn[0].f2_3[0]", requestInstitue.Idnumber);   //Payer TIN
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].LeftColumn[0].f2_4[0]", request.Rcp_TIN);   //Recepients TIN
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].LeftColumn[0].f2_5[0]", request.First_Name + " " + request.Name_Line2);   //RECIPIENT’S name
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].LeftColumn[0].f2_6[0]", RecipentAddress);   //Street address (including apt. no.)
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].LeftColumn[0].f2_7[0]", RecipentCity);   //City or town, state or province, country, and ZIP or foreign postal code
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].LeftColumn[0].f2_8[0]", request.Rcp_Account);   //Account number (see instructions)
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_9[0]", request.Box_1_Amount.HasValue ? request.Box_1_Amount.Value.ToString() : string.Empty);   //1 Nonemployee compensation

            if (request.Box_2_Checkbox != "1")
            {
                pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].c2_3[0]", "1");   //2 CheckBox Payer made direct sales totaling $5,000 or more of consumer products to recipient for resale

            }
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_10[0]", request.Box_4_Amount.HasValue ? request.Box_4_Amount.Value.ToString("0.00") : string.Empty);   //4 Federal income tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_11[0]", request.Box_5_Amount.HasValue ? request.Box_5_Amount.Value.ToString("0.00") : string.Empty);   //5 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_12[0]", request.Box_5_Amount.HasValue ? request.Box_5_Amount.Value.ToString("0.00") : string.Empty);   //5 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_13[0]", request.Box_6_State ?? string.Empty);   //6 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_14[0]", request.Box_6_State ?? string.Empty);   //6 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_15[0]", request.Box_7_Amount.HasValue ? request.Box_7_Amount.Value.ToString("0.00") : string.Empty);   //7 State income
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_16[0]", request.Box_7_Amount.HasValue ? request.Box_7_Amount.Value.ToString("0.00") : string.Empty);   //7 State income


            pdfFormFields.SetField("topmostSubform[0].CopyB[0].CopyBHeader[0].CalendarYear[0].f2_1[0]", currentYear);   //CalYear
            if (request.Corrected.ToString() != "1")
            {

                pdfFormFields.SetField("topmostSubform[0].CopyB[0].CopyBHeader[0].c2_1[0]", "0");   //CORRECTED
            }
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftColumn[0].f2_2[0]", PayData);   //PAYER’S name, street address, city or town, state or province, country, ZIP or foreign postal code, and telephone no
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftColumn[0].f2_3[0]", requestInstitue.Idnumber);   //Payer TIN
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftColumn[0].f2_4[0]", request.Rcp_TIN);   //Recepients TIN
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftColumn[0].f2_5[0]", request.First_Name + " " + request.Name_Line2);   //RECIPIENT’S name
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftColumn[0].f2_6[0]", RecipentAddress);   //Street address (including apt. no.)
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftColumn[0].f2_7[0]", RecipentCity);   //City or town, state or province, country, and ZIP or foreign postal code
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftColumn[0].f2_8[0]", request.Rcp_Account);   //Account number (see instructions)
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_9[0]", request.Box_1_Amount.HasValue ? request.Box_1_Amount.Value.ToString("0.00") : string.Empty);   //1 Nonemployee compensation
            if (request.Box_2_Checkbox != "1")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].c2_3[0]", "1");   //2 CheckBox Payer made direct sales totaling $5,000 or more of consumer products to recipient for resale

            }
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_10[0]", request.Box_4_Amount.HasValue ? request.Box_4_Amount.Value.ToString("0.00") : string.Empty);   //4 Federal income tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_11[0]", request.Box_5_Amount.HasValue ? request.Box_5_Amount.Value.ToString("0.00") : string.Empty);   //5 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_12[0]", request.Box_5_Amount.HasValue ? request.Box_5_Amount.Value.ToString("0.00") : string.Empty);   //5 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_13[0]", request.Box_6_State ?? string.Empty);   //6 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_14[0]", request.Box_6_State ?? string.Empty);   //6 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_15[0]", request.Box_7_Amount.HasValue ? request.Box_7_Amount.Value.ToString("0.00") : string.Empty);   //7 State income
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_16[0]", request.Box_7_Amount.HasValue ? request.Box_7_Amount.Value.ToString("0.00") : string.Empty);   //7 State income


            pdfFormFields.SetField("topmostSubform[0].Copy2[0].CopyCHeader[0].CalendarYear[0].f2_1[0]", currentYear);   //CalYear
            if (request.Corrected.ToString() == "1")
            {
                pdfFormFields.SetField("topmostSubform[0].Copy2[0].CopyCHeader[0].c2_1[0]", "1");   //VOID
            }
            if (request.Corrected.ToString() == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].Copy2[0].CopyCHeader[0].c2_1[1]", "1");   //CORRECTED
            }
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].LeftColumn[0].f2_2[0]", PayData);   //PAYER’S name, street address, city or town, state or province, country, ZIP or foreign postal code, and telephone no
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].LeftColumn[0].f2_3[0]", requestInstitue.Idnumber);   //Payer TIN
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].LeftColumn[0].f2_4[0]", request.Rcp_TIN);   //Recepients TIN
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].LeftColumn[0].f2_5[0]", request.First_Name + " " + request.Name_Line2);   //RECIPIENT’S name
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].LeftColumn[0].f2_6[0]", RecipentAddress);   //Street address (including apt. no.)
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].LeftColumn[0].f2_7[0]", RecipentCity);   //City or town, state or province, country, and ZIP or foreign postal code
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].LeftColumn[0].f2_8[0]", request.Rcp_Account);   //Account number (see instructions)
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_9[0]", request.Box_1_Amount.HasValue ? request.Box_1_Amount.Value.ToString() : string.Empty);   //1 Nonemployee compensation
            if (request.Box_2_Checkbox != "1")
            {
                pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].c2_3[0]", "1");   //2 CheckBox Payer made direct sales totaling $5,000 or more of consumer products to recipient for resale

            }
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_10[0]", request.Box_4_Amount.HasValue ? request.Box_4_Amount.Value.ToString("0.00") : string.Empty);   //4 Federal income tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_11[0]", request.Box_5_Amount.HasValue ? request.Box_5_Amount.Value.ToString("0.00") : string.Empty);   //5 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_12[0]", request.Box_5_Amount.HasValue ? request.Box_5_Amount.Value.ToString("0.00") : string.Empty);   //5 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_13[0]", request.Box_6_State ?? string.Empty);   //6 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_14[0]", request.Box_6_State ?? string.Empty);   //6 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_15[0]", request.Box_7_Amount.HasValue ? request.Box_7_Amount.Value.ToString("0.00") : string.Empty);   //7 State income
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_16[0]", request.Box_7_Amount.HasValue ? request.Box_7_Amount.Value.ToString("0.00") : string.Empty);   //7 State income

            pdfFormFields.SetField("topmostSubform[0].CopyC[0].CopyCHeader[0].CalendarYear[0].f2_1[0]", currentYear);   //CalYear


            if (request.Corrected.ToString() == "1")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyC[0].CopyCHeader[0].c2_1[0]", "1");   //VOID
            }
            if (request.Corrected.ToString() == "0")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyC[0].CopyCHeader[0].c2_1[1]", "1");   //CORRECTED
            }

            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_2[0]", PayData);   //PAYER’S name, street address, city or town, state or province, country, ZIP or foreign postal code, and telephone no
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_3[0]", requestInstitue.Idnumber);   //Payer TIN
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_4[0]", request.Rcp_TIN);   //Recepients TIN
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_5[0]", request.First_Name + " " + request.Name_Line2);   //RECIPIENT’S name
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_6[0]", RecipentAddress);   //Street address (including apt. no.)
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_7[0]", RecipentCity);   //City or town, state or province, country, and ZIP or foreign postal code
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_8[0]", request.Rcp_Account);   //Account number (see instructions)
            if (request.Second_TIN_Notice != "1")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].c2_2[0]", "1");   //2nd TIN not.               

            }
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_9[0]", request.Box_1_Amount.HasValue ? request.Box_1_Amount.Value.ToString("0.00") : string.Empty);   //1 Nonemployee compensation


            if (request.Box_2_Checkbox != "1")
            {
                pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].c2_3[0]", "1");   //2 CheckBox Payer made direct sales totaling $5,000 or more of consumer products to recipient for resale             

            }
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_10[0]", request.Box_4_Amount.HasValue ? request.Box_4_Amount.Value.ToString("0.00") : string.Empty);   //4 Federal income tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_11[0]", request.Box_5_Amount.HasValue ? request.Box_5_Amount.Value.ToString("0.00") : string.Empty);   //5 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_12[0]", request.Box_5_Amount.HasValue ? request.Box_5_Amount.Value.ToString() : string.Empty);   //5 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_13[0]", request.Box_6_State ?? string.Empty);   //6 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_14[0]", request.Box_6_State ?? string.Empty);   //6 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_15[0]", request.Box_7_Amount.HasValue ? request.Box_7_Amount.Value.ToString("0.00") : string.Empty);   //7 State income
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_16[0]", request.Box_7_Amount.ToString());   //7 State income

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



        public string GenerateAndZipPdfs(List<int> ids,  string SaveFolderPath, List<string> selectedPages, string RootPath)
        {
            var pdfPaths = new List<string>();

            foreach (var id in ids)
            {
               
                    string TemplatePathFile  = Path.Combine(RootPath, "Forms", AppConstants.NEC_1099_TemplateFileName);
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
            var request = _evolvedtaxContext.Tbl1099_NEC.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.First_Name + " " + request.Name_Line2;
            newFile1 = string.Concat(ClientName, "_", "Form_", AppConstants.NEC1099Form, "_", Id);
            string FilenameNew = "/1099NEC/" + newFile1 + ".pdf";
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

        public string GeneratePdForSpecificType(int Id, string TemplatefilePath, string SaveFolderPath,string selectedPage)
        {
            string newFile1 = string.Empty;
            var request = _evolvedtaxContext.Tbl1099_NEC.FirstOrDefault(p => p.Id == Id);
            String ClientName = request.First_Name + " " + request.Name_Line2;
           
            newFile1 = string.Concat(ClientName, "_", "Form_", AppConstants.NEC1099Form, "_", request.Id, "_Page_", selectedPage);
            string FilenameNew = "/1099NEC/" + newFile1 + ".pdf";
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
            string TemplatePathFile = Path.Combine(RootPath, "Forms", AppConstants.NEC_1099_TemplateFileName);
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
            else { 
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


        public async Task<MessageResponseModel> DeletePermeant(int id)
        {

            var recordToDelete = _evolvedtaxContext.Tbl1099_NEC.First(p => p.Id == id);
            if (recordToDelete != null)
            {
                _evolvedtaxContext.Tbl1099_NEC.Remove(recordToDelete);
                await _evolvedtaxContext.SaveChangesAsync();
                return new MessageResponseModel { Status = true };
            }

            return new MessageResponseModel { Status = false };
        }

        public async Task<MessageResponseModel> KeepRecord(int id)
        {

            var recordToUpdate = _evolvedtaxContext.Tbl1099_NEC.First(p => p.Id == id);
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
