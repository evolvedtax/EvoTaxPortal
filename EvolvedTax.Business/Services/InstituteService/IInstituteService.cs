using EvolvedTax.Data.Enums;
using EvolvedTax.Data.Models.DTOs;
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
        InstituteClientResponse? GetClientDataByClientEmailId(string ClientEmailId);
        Task<MessageResponseModel> UploadEntityData(IFormFile file, int InstId, string InstituteName);
        Task<MessageResponseModel> UploadClientData(IFormFile file, int InstId, int EntityId, string entityName);
        Task<bool> UpdateClientByClientEmailId(string ClientEmail, PdfFormDetailsRequest request);
        Task<bool> UpdateClientStatusByClientEmailId(string ClientEmail, short status);
        InstituteMaster GetInstituteDataByClientEmailId(string clientEmailId);
        InstituteEntity GetEntityDataByClientEmailId(string clientEmail);
        Task<MessageResponseModel> UpdateEntity(InstituteEntityRequest request);
        Task<MessageResponseModel> DeleteEntity(int id, RecordStatusEnum RecordStatus);
        Task<MessageResponseModel> LockUnlockEntity(int[] selectedValues, bool isLocked);
        IQueryable<InstituteEntitiesResponse> GetRecyleBinEntitiesByInstId(int instId);
        MessageResponseModel RestoreEntities(int[] selectedValues);
        IQueryable<InstituteClientResponse> GetRecyleBinClientsByEntityId(int instId, int entityId);
        MessageResponseModel RestoreClients(int[] selectedValues);
        Task<MessageResponseModel> DeleteClient(int id, RecordStatusEnum RecordStatus);
        Task<MessageResponseModel> DeleteClientPermeant(int id);
        Task<MessageResponseModel> KeepClienRecord(int id);
        Task<MessageResponseModel> UpdateClient(InstituteClientRequest request);
        Task<MessageResponseModel> LockUnlockClient(int[] selectedValues, bool isLocked);
        Task<MessageResponseModel> TrashEmptyClient(int[] selectedValues, RecordStatusEnum recordStatusEnum);
        Task<MessageResponseModel> TrashEmptyEntity(int[] selectedValues, RecordStatusEnum recordStatusEnum);
        Task<bool> CheckIfClientRecordExist(string clientEmail, string user);
        Task<MessageResponseModel> UpdateEmailFrequncy(int EntityId, int emailFrequency);
        InstituteMasterResponse GetInstituteDataById(int instId);
        bool UpdateInstituteMaster(InstituteMasterRequest request);
        Task<MessageResponseModel> AddClient(InstituteClientRequest request);
        Task<MessageResponseModel> AddEntity(InstituteEntityRequest request);
        bool IsEntityNameExist(string entityName, int institueId, int institueId1);
        bool IsEINExist(string ein, int institueId, int institueId1);

        bool SetEmailReminder(InstituteMasterRequest request);

        public List<AlertRequest> GetAlertsNotification(int instituteId);
        bool MarkAlertAsRead(int id);
        public bool MarkAllAlertsAsRead(int instituteId);

        public List<AnnouncementRequest> GetAnnouncements();

    }
}
