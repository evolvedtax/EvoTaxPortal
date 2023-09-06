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
using EvolvedTax.Data.Models.DTOs.Response;

namespace EvolvedTax.Business.Services.Form1099Services
{
    public class Form1099_NEC_Service : IForm1099_NEC_Service
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
                UserId = Guid.NewGuid().ToString();
                for (int row = 1; row <= sheet.LastRowNum; row++) // Starting from the second row
                {
                    IRow excelRow = sheet.GetRow(row);

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
                        Second_TIN_Notice = excelRow.GetCell(13)?.ToString(),
                        Box_1_Amount = excelRow.GetCell(14) != null ? Convert.ToDecimal(excelRow.GetCell(14).ToString()) : (decimal?)null,
                        Box_2_Checkbox = excelRow.GetCell(15)?.ToString(),
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
                        UserId = UserId,
                        Created_Date = DateTime.Now.Date,
                        Created_By = InstId.ToString()

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
                    }
                    else
                    {
                        necList.Add(entity);
                     
                    }
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
            var request = _evolvedtaxContext.Tbl1099_NEC.FirstOrDefault(p => p.Id == Id);
            var requestInstitue = _evolvedtaxContext.InstituteMasters.FirstOrDefault(p => p.InstId == request.InstID);
            string templatefile = TemplatefilePath;
            string newFile1 = string.Empty;





            newFile1 = string.Concat(request.First_Name.Replace(" ", "_"), "_", "Form_", AppConstants.NEC1099Form, "_", request.Id);
            string FilenameNew ="/1099NEC/" + newFile1+".pdf";
            string newFileName = newFile1 + ".pdf"; // Add ".pdf" extension to the file name

            string newFilePath = Path.Combine(SaveFolderPath, newFileName);

            PdfReader pdfReader = new PdfReader(templatefile);
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFilePath, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;

            string PayData= string.Concat(requestInstitue.FirstName,",", requestInstitue.Madd1,",", requestInstitue.Mcity, ",",requestInstitue.Mcountry,",",requestInstitue.Phone);
            string RecipentCIty=string.Concat(request.City,",",request.State,",",request.Zip,",",request.Country);
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].PgHeader[0].CalendarYear[0].f1_1[0]", "23");   //CalYear
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].PgHeader[0].c1_1[0]", "VOID");   //VOID
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].PgHeader[0].c1_1[1]", "CORRECTED");   //CORRECTED
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_2[0]", PayData);   //PAYER’S name, street address, city or town, state or province, country, ZIP or foreign postal code, and telephone no
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_3[0]", requestInstitue.Idnumber);   //Payer TIN (IDNYMber
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_4[0]", request.Rcp_TIN);   //Recepients TIN
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_5[0]", request.First_Name);   //RECIPIENT’S name
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_6[0]", request.Address_Apt_Suite);   //Street address (including apt. no.)
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_7[0]", RecipentCIty);   //City or town, state or province, country, and ZIP or foreign postal code
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].f1_8[0]",  request.Rcp_Account);   //Account number (see instructions)
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].LeftColumn[0].c1_2[0]", request.Second_TIN_Notice);   //2nd TIN not.
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_9[0]", request.Box_1_Amount.ToString());   //1 Nonemployee compensation
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].c1_3[0]", request.Box_2_Checkbox.ToString());   //2 CheckBox Payer made direct sales totaling $5,000 or more of consumer products to recipient for resale
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_10[0]", request.Box_4_Amount.ToString());   //4 Federal income tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_11[0]", request.Box_5_Amount.ToString());   //5 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_12[0]", request.Box_5_Amount.ToString());   //5 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_13[0]", request.Box_6_State.ToString());   //6 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_14[0]", request.Box_6_State.ToString());   //6 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_15[0]", request.Box_7_Amount.ToString());   //7 State income
            pdfFormFields.SetField("topmostSubform[0].CopyA[0].RightColumn[0].f1_16[0]", request.Box_7_Amount.ToString());   //7 State income

            pdfFormFields.SetField("topmostSubform[0].Copy1[0].Copy1Header[0].CalendarYear[0].f2_1[0]", "CalYear");   //CalYear
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].Copy1Header[0].c2_1[0]", "VOID");   //VOID
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].Copy1Header[0].c2_1[1]", "CORRECTED");   //CORRECTED
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].LeftColumn[0].f2_2[0]", PayData);   //PAYER’S name, street address, city or town, state or province, country, ZIP or foreign postal code, and telephone no
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].LeftColumn[0].f2_3[0]", "PAYTIN");   //Payer TIN
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].LeftColumn[0].f2_4[0]", "RCPTIN");   //Recepients TIN
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].LeftColumn[0].f2_5[0]", "RCPName");   //RECIPIENT’S name
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].LeftColumn[0].f2_6[0]", "RCPAdd");   //Street address (including apt. no.)
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].LeftColumn[0].f2_7[0]", "RCPCityStateCountryZIPPC");   //City or town, state or province, country, and ZIP or foreign postal code
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].LeftColumn[0].f2_8[0]", "RCTAccountNo");   //Account number (see instructions)
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_9[0]", "NonEmployeeComp");   //1 Nonemployee compensation
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].c2_3[0]", "Payer5k");   //2 CheckBox Payer made direct sales totaling $5,000 or more of consumer products to recipient for resale
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_10[0]", "FedITWithHeld");   //4 Federal income tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_11[0]", "StatTax1");   //5 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_12[0]", "StatTax2");   //5 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_13[0]", "StateNo1");   //6 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_14[0]", "StateNo2");   //6 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_15[0]", "StateIncome1");   //7 State income
            pdfFormFields.SetField("topmostSubform[0].Copy1[0].RightColumn[0].f2_16[0]", "StateIncome2");   //7 State income

            pdfFormFields.SetField("topmostSubform[0].CopyB[0].CopyBHeader[0].CalendarYear[0].f2_1[0]", "CalYear");   //CalYear
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].CopyBHeader[0].c2_1[0]", "CORRECTED");   //CORRECTED
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftColumn[0].f2_2[0]", "PayData");   //PAYER’S name, street address, city or town, state or province, country, ZIP or foreign postal code, and telephone no
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftColumn[0].f2_3[0]", "PAYTIN");   //Payer TIN
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftColumn[0].f2_4[0]", "RCPTIN");   //Recepients TIN
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftColumn[0].f2_5[0]", "RCPName");   //RECIPIENT’S name
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftColumn[0].f2_6[0]", "RCPAdd");   //Street address (including apt. no.)
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftColumn[0].f2_7[0]", "RCPCityStateCountryZIPPC");   //City or town, state or province, country, and ZIP or foreign postal code
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].LeftColumn[0].f2_8[0]", "RCTAccountNo");   //Account number (see instructions)
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_9[0]", "NonEmployeeComp");   //1 Nonemployee compensation
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].c2_3[0]", "Payer5k");   //2 CheckBox Payer made direct sales totaling $5,000 or more of consumer products to recipient for resale
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_10[0]", "FedITWithHeld");   //4 Federal income tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_11[0]", "StatTax1");   //5 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_12[0]", "StatTax2");   //5 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_13[0]", "StateNo1");   //6 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_14[0]", "StateNo2");   //6 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_15[0]", "StateIncome1");   //7 State income
            pdfFormFields.SetField("topmostSubform[0].CopyB[0].RightColumn[0].f2_16[0]", "StateIncome2");   //7 State income

            pdfFormFields.SetField("topmostSubform[0].Copy2[0].CopyCHeader[0].CalendarYear[0].f2_1[0]", "CalYear");   //CalYear
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].CopyCHeader[0].c2_1[0]", "VOID");   //VOID
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].CopyCHeader[0].c2_1[1]", "CORRECTED");   //CORRECTED
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].LeftColumn[0].f2_2[0]", "PayData");   //PAYER’S name, street address, city or town, state or province, country, ZIP or foreign postal code, and telephone no
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].LeftColumn[0].f2_3[0]", "PAYTIN");   //Payer TIN
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].LeftColumn[0].f2_4[0]", "RCPTIN");   //Recepients TIN
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].LeftColumn[0].f2_5[0]", "RCPName");   //RECIPIENT’S name
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].LeftColumn[0].f2_6[0]", "RCPAdd");   //Street address (including apt. no.)
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].LeftColumn[0].f2_7[0]", "RCPCityStateCountryZIPPC");   //City or town, state or province, country, and ZIP or foreign postal code
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].LeftColumn[0].f2_8[0]", "RCTAccountNo");   //Account number (see instructions)
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_9[0]", "NonEmployeeComp");   //1 Nonemployee compensation
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].c2_3[0]", "Payer5k");   //2 CheckBox Payer made direct sales totaling $5,000 or more of consumer products to recipient for resale
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_10[0]", "FedITWithHeld");   //4 Federal income tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_11[0]", "StatTax1");   //5 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_12[0]", "StatTax2");   //5 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_13[0]", "StateNo1");   //6 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_14[0]", "StateNo2");   //6 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_15[0]", "StateIncome1");   //7 State income
            pdfFormFields.SetField("topmostSubform[0].Copy2[0].RightColumn[0].f2_16[0]", "StateIncome2");   //7 State income

            pdfFormFields.SetField("topmostSubform[0].CopyC[0].CopyCHeader[0].CalendarYear[0].f2_1[0]", "CalYear");   //CalYear
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].CopyCHeader[0].c2_1[0]", "VOID");   //VOID
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].CopyCHeader[0].c2_1[1]", "CORRECTED");   //CORRECTED
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_2[0]", "PayData");   //PAYER’S name, street address, city or town, state or province, country, ZIP or foreign postal code, and telephone no
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_3[0]", "PAYTIN");   //Payer TIN
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_4[0]", "RCPTIN");   //Recepients TIN
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_5[0]", "RCPName");   //RECIPIENT’S name
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_6[0]", "RCPAdd");   //Street address (including apt. no.)
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_7[0]", "RCPCityStateCountryZIPPC");   //City or town, state or province, country, and ZIP or foreign postal code
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].f2_8[0]", "RCTAccountNo");   //Account number (see instructions)
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].LeftColumn[0].c2_2[0]", "2ndTin");   //2nd TIN not.
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_9[0]", "NonEmployeeComp");   //1 Nonemployee compensation
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].c2_3[0]", "Payer5k");   //2 CheckBox Payer made direct sales totaling $5,000 or more of consumer products to recipient for resale
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_10[0]", "FedITWithHeld");   //4 Federal income tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_11[0]", "StatTax1");   //5 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_12[0]", "StatTax2");   //5 State tax withheld
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_13[0]", "StateNo1");   //6 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_14[0]", "StateNo2");   //6 State/Payer’s state no.
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_15[0]", "StateIncome1");   //7 State income
            pdfFormFields.SetField("topmostSubform[0].CopyC[0].RightColumn[0].f2_16[0]", "StateIncome2");   //7 State income

            pdfStamper.FormFlattening = true;
            pdfStamper.Close();
            pdfReader.Close();
            return FilenameNew;
            //return Path.Combine("1099NEC", newFileName); // Return the relative path to the generated PDF
        }

    }
}
