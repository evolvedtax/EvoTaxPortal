using EvolvedTax.Data.Models.DTOs;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.DTOs.Response;
using EvolvedTax.Data.Models.DTOs.Response.Form1099;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Data.Models.Entities._1099;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Business.Services.Form1099Services
{
    public interface IForm1099_SB_Service
    {
        Task<MessageResponseModel> Upload1099_Data(IFormFile file, int EntityId, int instituteId, string UserId);
        //IQueryable<Tbl1099_NEC> GetRecodByInstId(int InstId);
        public string GeneratePdf(int Id, string TemplatefilePath, string SaveFolderPath);
        public string GenerateAndZipPdfs(List<int> ids, string SaveFolderPath, List<string> selectedPages, string RootPath);
        public string DownloadOneFile(List<int> ids, string SaveFolderPath, List<string> selectedPages, string RootPath);
        Task<MessageResponseModel> KeepRecord(int id);
        Task<MessageResponseModel> DeletePermeant(int id);
        Task<bool> SendEmailToRecipients(int[] selectedValues, string uRL, string form1099NEC, int instituteId = -1);
        IEnumerable<Tbl1099_SB> GetForm1099List();
        IEnumerable<Form1099SBResponse> GetCSVForm1099List(int entityId, int instId, List<string> selectedRows);
        string GenerateCsvContent(IEnumerable<Form1099SBResponse> data);
    }
        
}
