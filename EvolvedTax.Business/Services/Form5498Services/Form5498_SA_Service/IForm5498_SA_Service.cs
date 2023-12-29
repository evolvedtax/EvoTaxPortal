﻿using EvolvedTax.Data.Models.DTOs;
using Microsoft.AspNetCore.Http;
using EvolvedTax.Data.Models.DTOs.Response.Form1042;
using EvolvedTax.Data.Models.DTOs.Response.Form5498;

namespace EvolvedTax.Business.Services.Form5498ervices
{
    public interface IForm5498_SA_Service
    {
        IQueryable<Form5498SAResponse> GetForm5498SASList();
        Task<MessageResponseModel> Upload5498_SA_Data(IFormFile file, int InstId,int entityId, string UserId);
       // string GeneratePdf(int id, string BasePath);
        public string GeneratePdf(int Id, string TemplatefilePath, string SaveFolderPath, int entityId);
        public string GenerateAndZipPdfs(List<int> ids, string SaveFolderPath, List<string> selectedPages, string RootPath);
        public string DownloadOneFile(List<int> ids, string SaveFolderPath, List<string> selectedPages, string RootPath);
        Task<MessageResponseModel> KeepRecord(int id);
        Task<MessageResponseModel> DeletePermeant(int id);
        Task<bool> SendEmailToRecipients(int[] selectValues, string URL, string form1042S, int instituteId = -1);
        //tring GeneratePdf(int id, string BasePath);
    }
}
