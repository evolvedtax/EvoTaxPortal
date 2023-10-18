using EvolvedTax.Data.Models.DTOs;
using EvolvedTax.Data.Models.DTOs.Request;
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
    public interface IForm1099_LS_Service
    {
        IQueryable<Form1099LSResponse> GetForm1099LSList();
        Task<MessageResponseModel> Upload1099_LS_Data(IFormFile file, int InstId,int entityId, string UserId);
       // string GeneratePdf(int id, string BasePath);
        public string GeneratePdf(int Id, string TemplatefilePath, string SaveFolderPath);
        public string GenerateAndZipPdfs(List<int> ids, string SaveFolderPath, List<string> selectedPages, string RootPath);
        public string DownloadOneFile(List<int> ids, string SaveFolderPath, List<string> selectedPages, string RootPath);
        Task<MessageResponseModel> KeepRecord(int id);
        Task<MessageResponseModel> DeletePermeant(int id);
        Task<bool> SendEmailToRecipients(int[] selectValues, string URL, string form1099LS, int instituteId = -1);
        //tring GeneratePdf(int id, string BasePath);
    }
}
