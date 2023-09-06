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

namespace EvolvedTax.Business.Services.Form1099Services
{
    public class Form1099_MISC_Service : IForm1099_MISC_Service
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        public Form1099_MISC_Service(EvolvedtaxContext evolvedtaxContext)
        {
            _evolvedtaxContext = evolvedtaxContext;
        }
        public async Task<MessageResponseModel> Upload1099_MISC_Data(IFormFile file, int InstId, string UserId)
        {
            bool Status = false;
            var response = new List<InstituteEntity>();
            var entityList = new List<InstituteEntity>();
            using (var stream = file.OpenReadStream())
            {
                IWorkbook workbook = new XSSFWorkbook(stream);
                ISheet sheet = workbook.GetSheetAt(0); // Assuming the data is in the first sheet

                HashSet<string> uniqueEINNumber = new HashSet<string>();
                HashSet<string> uniqueEntityNames = new HashSet<string>();

                for (int row = 1; row <= sheet.LastRowNum; row++) // Starting from the second row
                {
                    IRow excelRow = sheet.GetRow(row);

                    var entity = new InstituteEntity
                    {
                        EntityName = excelRow.GetCell(0)?.ToString(),
                        Ein = excelRow.GetCell(1)?.ToString(),
                        EntityRegistrationDate = DateTime.Parse(excelRow.GetCell(2)?.ToString() ?? "01/01/0001"),
                        Address1 = excelRow.GetCell(3)?.ToString(),
                        Address2 = excelRow.GetCell(4)?.ToString(),
                        City = excelRow.GetCell(5)?.ToString(),
                        State = excelRow.GetCell(6)?.ToString(),
                        Province = excelRow.GetCell(7)?.ToString(),
                        Zip = excelRow.GetCell(8)?.ToString(),
                        Country = excelRow.GetCell(9)?.ToString(),
                        InstituteId = InstId,
                        InstituteName = UserId,
                        IsActive = RecordStatusEnum.Active,
                        IsLocked = false,
                        LastUpdatedDate = DateTime.Now.Date,
                        LastUpdatedBy = InstId
                    };
                    string clientEmailEINNumber = entity.Ein ?? string.Empty;
                    string entityNameExcel = entity.EntityName ?? string.Empty;
                    if (uniqueEINNumber.Contains(clientEmailEINNumber) || uniqueEntityNames.Contains(entityNameExcel))
                    {
                        // This entity is a duplicate within the Excel sheet
                        Status = false;
                        return new MessageResponseModel { Status = Status, Message = new { Title = "Duplication Record In Excel", TagLine = "Record not uploaded due to duplication record in excel" }, Param = "Client" };
                    }
                    else
                    {
                        // Add the values to the HashSet to track dzuplicates
                        uniqueEINNumber.Add(clientEmailEINNumber);
                        uniqueEntityNames.Add(entityNameExcel);
                    }
                    // Check for duplicate records based on entityName in the database
                    if (await _evolvedtaxContext.InstituteEntities.AnyAsync(p =>
                        p.Ein == entity.Ein &&
                        p.EntityName == entity.EntityName &&
                        p.InstituteId == entity.InstituteId))
                    {
                        response.Add(entity);
                        Status = true;
                    }
                    else
                    {
                        entityList.Add(entity);
                    }
                }
                await _evolvedtaxContext.InstituteEntities.AddRangeAsync(entityList);
                await _evolvedtaxContext.SaveChangesAsync();
            }
            return new MessageResponseModel { Status = Status, Message = response, Param = "Entity" };
        }
    }
}
