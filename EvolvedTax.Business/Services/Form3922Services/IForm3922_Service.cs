using EvolvedTax.Data.Models.DTOs;
using Microsoft.AspNetCore.Http;
using EvolvedTax.Data.Models.DTOs.Response.Form1042;
using EvolvedTax.Data.Models.DTOs.Response.Form3922Response;

namespace EvolvedTax.Business.Services.Form3922Services
{
    public interface IForm3922_Service
    {
        IQueryable<Form3922Response> GetFormList();
        Task<MessageResponseModel> Upload_Data(IFormFile file, int InstId,int entityId, string UserId);
        // string GeneratePdf(int id, string BasePath);
        #region PDF Methods
        public string GeneratePdf(int Id, string TemplatefilePath, string SaveFolderPath, int entityId);
        public string GenerateAndZipPdfs(List<int> ids, string SaveFolderPath, List<string> selectedPages, string RootPath, int entityId = 0);
        public string DownloadOneFile(List<int> ids, string SaveFolderPath, List<string> selectedPages, string RootPath, int entityId = 0);
        #endregion
        Task<MessageResponseModel> KeepRecord(int id);
        Task<MessageResponseModel> DeletePermeant(int id);
        Task<bool> SendEmailToRecipients(int[] selectValues, string URL, string Form3922, int instituteId = -1);
        //tring GeneratePdf(int id, string BasePath);
    }
}
