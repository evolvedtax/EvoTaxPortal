using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.DTOs.Response;
using EvolvedTax.Data.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Business.Services.InstituteService
{
    public interface IInstituteService
    {
        IQueryable<InstituteMasterResponse> GetMaster();
        IQueryable<InstituteEntitiesResponse> GetEntitiesByInstId(int InstId);
        IQueryable<InstituteClientResponse> GetClientByEntityId(int InstId, int EntityId);
        List<InstituteClientResponse> GetClientInfoByClientId(int[] ClientId);
        InstituteClientResponse GetClientDataByClientEmailId(string ClientEmailId);
        Task<bool> UploadEntityData(IFormFile file, int InstId, string InstituteName);
        Task<bool> UploadClientData(IFormFile file, int InstId, int EntityId);
        Task<bool> UpdateClientByClientEmailId(string ClientEmail, PdfFormDetailsRequest request);
        Task<bool> UpdateClientStatusByClientEmailId(string ClientEmail, short status);
        InstituteMaster GetInstituteDataByClientEmailId(string clientEmailId);
        InstituteEntity GetEntityDataByClientEmailId(string clientEmail);
    }
}
