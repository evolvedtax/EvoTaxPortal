using EvolvedTax.Data.Models.DTOs;
using Microsoft.AspNetCore.Http;
using EvolvedTax.Data.Models.DTOs.Response.Form1042;
using EvolvedTax.Data.Models.DTOs.Response.Form5498;
using EvolvedTax.Data.Models.DTOs.Response.Form1098;

namespace EvolvedTax.Business.Services.Form1098Services
{
    public interface IForm1098_E_Service
    {
        IQueryable<Form1098EResponse> GetForm1098EList();
        Task<MessageResponseModel> Upload1098_E_Data(IFormFile file, int InstId,int entityId, string UserId);
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
