using EvolvedTax.Data.Models.DTOs;
using Microsoft.AspNetCore.Http;
using EvolvedTax.Data.Models.DTOs.Response.Form1042;

namespace EvolvedTax.Business.Services.Form1042Services
{
    public interface IForm1042_S_Service
    {
        IQueryable<Form1042SResponse> GetForm1042SList();
        Task<MessageResponseModel> Upload1042_S_Data(IFormFile file, int InstId,int entityId, string UserId);
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
