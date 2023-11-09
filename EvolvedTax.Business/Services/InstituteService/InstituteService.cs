using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Data.Models.DTOs.Response;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using EvolvedTax.Data.Models.DTOs.Request;
using Microsoft.EntityFrameworkCore;
using EvolvedTax.Data.Models.DTOs;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Enums;
using Microsoft.Data.SqlClient;
using SkiaSharp;
using System.Data;
using Azure;
using ICSharpCode.SharpZipLib.Zip;
using System.Linq;
using NPOI.OpenXmlFormats.Dml;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace EvolvedTax.Business.Services.InstituteService
{
    public class InstituteService : IInstituteService
    {
        readonly private EvolvedtaxContext _evolvedtaxContext;
        readonly private IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public InstituteService(EvolvedtaxContext evolvedtaxContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<InstituteMasterResponse> GetMaster()
        {
            var s = _evolvedtaxContext.InstituteMasters.ToList();
            return _mapper.Map<List<InstituteMasterResponse>>(_evolvedtaxContext.InstituteMasters).AsQueryable();
        }

        public IQueryable<InstituteEntitiesResponse> GetEntitiesByInstId(int InstId)
        {
            string userId = "";
            HttpContext httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                userId = httpContext.Session.GetString("UserId");

            }


            var userEntityIds = _evolvedtaxContext.EntitiesUsers
                                .Where(ue => ue.UserId == userId)
                                .Select(ue => ue.EntityId)
                                .ToList();

            var query = from p in _evolvedtaxContext.InstituteEntities
                            //where userEntityIds.Contains(p.EntityId) && p.InstituteId == InstId && p.IsActive == RecordStatusEnum.Active
                        where (userEntityIds.Count == 0 || userEntityIds.Contains(p.EntityId))
                && p.InstituteId == InstId
                && p.IsActive == RecordStatusEnum.Active
                        select new InstituteEntitiesResponse
                        {
                            Address1 = p.Address1,
                            Address2 = p.Address2,
                            City = p.City,
                            Country = p.Country,
                            Ein = p.Ein,
                            EntityId = p.EntityId,
                            EntityName = p.EntityName,
                            EntityRegistrationDate = p.EntityRegistrationDate,
                            InstituteId = p.InstituteId,
                            InstituteName = p.InstituteName,
                            LastUpdatedDate = p.LastUpdatedDate,
                            Province = p.Province,
                            State = p.State,
                            Zip = p.Zip,
                            IsActive = p.IsActive,
                            IsLocked = p.IsLocked,
                            EmailFrequency = p.EmailFrequency,
                            LastUpdatedByName = _evolvedtaxContext.InstituteMasters.FirstOrDefault(x => x.InstId == p.LastUpdatedBy).InstitutionName ?? string.Empty,
                            Role = _evolvedtaxContext.EntitiesUsers.FirstOrDefault(ue => ue.EntityId == p.EntityId && ue.UserId == userId).Role.Trim() ?? string.Empty,
                        };

            return query;
        }

        public IQueryable<InstituteClientResponse> GetClientByEntityId(int InstId, int EntityId)
        {
            // Fetch all MasterClientStatus records
            var clientStatuses = _evolvedtaxContext.MasterClientStatuses.ToDictionary(cs => cs.StatusId);

            var response = _evolvedtaxContext.InstitutesClients
                .Where(p => p.EntityId == EntityId && p.IsActive == RecordStatusEnum.Active && p.InstituteId == InstId)
                .OrderByDescending(p => p.IsDuplicated)
                .Select(p => new InstituteClientResponse
                {
                    Address1 = p.Address1,
                    Address2 = p.Address2,
                    City = p.City,
                    ClientId = p.ClientId,
                    ClientEmailId = p.ClientEmailId,
                    ClientStatusDate = p.ClientStatusDate,
                    ClientStatus = p.ClientStatus,
                    Country = p.Country,
                    EntityId = EntityId,
                    EntityName = p.EntityName,
                    FileName = p.FileName,
                    InstituteId = p.InstituteId,
                    PartnerName1 = p.PartnerName1,
                    PartnerName2 = p.PartnerName2,
                    PhoneNumber = p.PhoneNumber,
                    Province = p.Province,
                    State = p.State,
                    FormName = p.FormName,
                    Zip = p.Zip,
                    IsActive = p.IsActive,
                    IsLocked = p.IsLocked,
                    IsDuplicated = p.IsDuplicated,
                    StatusName = clientStatuses[(short)p.ClientStatus].StatusName ?? "",
                    LastUpdatedOn = p.LastUpdatedOn,
                    LastUpdatedByName = _evolvedtaxContext.InstituteMasters.FirstOrDefault(x => x.InstId == p.LastUpdatedBy).InstitutionName ?? string.Empty,
                });
            return response;
        }

        public List<InstituteClientResponse> GetClientInfoByClientId(int[] ClientId)
        {
            var result = from ic in _evolvedtaxContext.InstitutesClients
                         join ie in _evolvedtaxContext.InstituteEntities on ic.EntityId equals ie.EntityId
                         where ClientId.Contains(ic.ClientId)
                         select new InstituteClientResponse
                         {
                             ClientEmailId = ic.ClientEmailId,
                             InstituteUserName = ic.PartnerName1 + " " + ic.PartnerName2 ?? "",
                             InstituteName = ie.EntityName ?? "",
                             EntityId = ie.EntityId,
                             ClientStatus = ic.ClientStatus
                         };

            return result.ToList();
        }

        public async Task<MessageResponseModel> UploadEntityData(IFormFile file, int InstId, string InstituteName)
        {
            bool Status = false;
            var response = new List<InstituteEntity>();
            var entityList = new List<InstituteEntity>();
            try
            {
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
                            EntityName = excelRow.GetCell(0)?.ToString()?.Trim(),
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
                            InstituteName = InstituteName,
                            IsActive = RecordStatusEnum.Active,
                            IsLocked = false,
                            LastUpdatedDate = DateTime.Now.Date,
                            LastUpdatedBy = InstId
                        };
                        string clientEmailEINNumber = entity.Ein ?? string.Empty;
                        string entityNameExcel = entity.EntityName ?? string.Empty;
                        if (entity.EntityRegistrationDate > DateTime.Now)
                        {
                            // This entity is a duplicate within the Excel sheet
                            Status = false;
                            return new MessageResponseModel { Status = Status, Message = new { Title = "Invalid Date", TagLine = "There are invalid date(s) in the records. Please correct and upload again." }, Param = "Entity" };
                        }
                        if (Regex.IsMatch(entity.Ein ?? "", AppConstants.EINPattern))
                        {
                            // This entity is a duplicate within the Excel sheet
                            Status = false;
                            return new MessageResponseModel { Status = Status, Message = new { Title = "Invalid EIN", TagLine = "There are invalid EIN(s) in the records. Please correct and upload again." }, Param = "Entity" };
                        }
                        if (uniqueEINNumber.Contains(clientEmailEINNumber) || uniqueEntityNames.Contains(entityNameExcel))
                        {
                            // This entity is a duplicate within the Excel sheet
                            Status = false;
                            return new MessageResponseModel { Status = Status, Message = new { Title = "Duplication Record In Excel", TagLine = "Record not uploaded due to duplication record in excel" }, Param = "Entity" };
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
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                return new MessageResponseModel { ErrorStatus = true, Message = ex.ToString(), Param = "Entity" };
            }
            return new MessageResponseModel { Status = Status, Message = response, Param = "Entity" };
        }
        public async Task<MessageResponseModel> UploadClientData(IFormFile file, int InstId, int EntityId, string entityName)
        {
            bool Status = false;
            var response = new List<InstitutesClient>();
            var clientList = new List<InstitutesClient>();
            using (var stream = file.OpenReadStream())
            {
                IWorkbook workbook = new XSSFWorkbook(stream);
                ISheet sheet = workbook.GetSheetAt(0); // Assuming the data is in the first sheet

                HashSet<string> uniqueClientEmailIds = new HashSet<string>();
                HashSet<string> uniqueEntityNames = new HashSet<string>();

                for (int row = 1; row <= sheet.LastRowNum; row++) // Starting from the second row
                {
                    IRow excelRow = sheet.GetRow(row);

                    var client = new InstitutesClient
                    {
                        EntityName = entityName,
                        PartnerName1 = excelRow.GetCell(0)?.ToString() ?? string.Empty,
                        PartnerName2 = excelRow.GetCell(1)?.ToString() ?? string.Empty,
                        Address1 = excelRow.GetCell(2)?.ToString() ?? string.Empty,
                        Address2 = excelRow.GetCell(3)?.ToString() ?? string.Empty,
                        City = excelRow.GetCell(4)?.ToString() ?? string.Empty,
                        State = excelRow.GetCell(5)?.ToString() ?? string.Empty,
                        Province = excelRow.GetCell(6)?.ToString() ?? string.Empty,
                        Zip = excelRow.GetCell(7)?.ToString() ?? string.Empty,
                        Country = excelRow.GetCell(8)?.ToString() ?? string.Empty,
                        PhoneNumber = excelRow.GetCell(9)?.ToString() ?? string.Empty,
                        ClientEmailId = excelRow.GetCell(10)?.ToString()?.Trim() ?? string.Empty,
                        ClientStatus = 1,
                        FileName = "",
                        InstituteId = (short)InstId,
                        EntityId = EntityId,
                        IsActive = RecordStatusEnum.Active,
                        IsLocked = false,
                        LastUpdatedBy = InstId,
                        LastUpdatedOn = DateTime.Now.Date
                    };
                    client.FileUploadTime = DateTime.Now;
                    string clientEmailId = client.ClientEmailId;
                    string entityNameExcel = client.EntityName;
                    //if (entityNameExcel != entityName)
                    //{
                    //    Status = false;
                    //    return new MessageResponseModel { Status = Status, Message = new { Title = entityNameExcel + " " + "entity does not appear", TagLine = "Name of the selected entity does not appear in the uploaded excel sheet" }, Param = "Client" };
                    //}
                    // Check for duplicate records within the Excel sheet
                    if (uniqueClientEmailIds.Contains(clientEmailId))
                    {
                        // This client is a duplicate within the Excel sheet
                        Status = false;
                        return new MessageResponseModel { Status = Status, Message = new { Title = "Duplication Record In Excel", TagLine = "Record not uploaded due to duplication record in excel" }, Param = "Client" };
                    }
                    else
                    {
                        // Add the values to the HashSet to track duplicates
                        uniqueClientEmailIds.Add(clientEmailId);
                        //uniqueEntityNames.Add(entityNameExcel);
                        //clientList.Add(client);
                    }
                    // Check for duplicate records based on ClientEmailId in the database
                    if (await _evolvedtaxContext.InstitutesClients.AnyAsync(p =>
                        p.ClientEmailId == client.ClientEmailId &&
                        p.InstituteId == client.InstituteId &&
                        p.EntityId == client.EntityId
                       // && p.EntityName == client.EntityName
                        ))
                    {
                        //response.Add(client);
                        client.IsDuplicated = true;
                    }
                    else
                    {
                        //clientList.Add(client);
                        client.IsDuplicated = false;
                    }
                    clientList.Add(client);
                }

                await _evolvedtaxContext.InstitutesClients.AddRangeAsync(clientList);
                // Locking record of Entity
                var entityResponse = _evolvedtaxContext.InstituteEntities.FirstOrDefault(p => p.EntityId == EntityId);
                if (entityResponse != null)
                {
                    entityResponse.IsLocked = true;
                    _evolvedtaxContext.InstituteEntities.Update(entityResponse);
                }
                await _evolvedtaxContext.SaveChangesAsync();
            }
            return new MessageResponseModel { Status = Status, Message = response, Param = "Client" };

        }
        public async Task<bool> UpdateClientByClientEmailId(string ClientEmail, PdfFormDetailsRequest request)
        {
            var response = _evolvedtaxContext.InstitutesClients.Where(p => p.ClientEmailId == ClientEmail).FirstOrDefault();
            response.FileName = request.FileName;
            response.ClientStatusDate = request.EntryDate;
            response.ClientStatus = AppConstants.ClientStatusFormSubmitted;
            response.IsLocked = true;
            response.FormName = request.FormName;
            _evolvedtaxContext.InstitutesClients.Update(response);
            await _evolvedtaxContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateClientStatusByClientEmailId(string ClientEmail, short status)
        {
            var response = _evolvedtaxContext.InstitutesClients.Where(p => p.ClientEmailId == ClientEmail).FirstOrDefault();
            response.ClientStatus = status;
            if (status == AppConstants.ClientStatusFormSubmitted)
            {
                response.IsLocked = true;
            }
            _evolvedtaxContext.InstitutesClients.Update(response);
            await _evolvedtaxContext.SaveChangesAsync();
            return true;
        }

        public InstituteClientResponse? GetClientDataByClientEmailId(string ClientEmailId)
        {
            return _evolvedtaxContext.InstitutesClients.Where(p => p.ClientEmailId == ClientEmailId).Select(p => new InstituteClientResponse
            {
                FileName = p.FileName,
                ClientStatusDate = p.ClientStatusDate,
                ClientStatus = p.ClientStatus,
                FormName = p.FormName,
                Address1 = p.Address1,
                Address2 = p.Address2,
                City = p.City,
                ClientEmailId = p.ClientEmailId,
                ClientId = p.ClientId,
                Country = p.Country,
                EntityId = p.EntityId,
                EntityName = p.EntityName,
                InstituteId = p.InstituteId,
                PartnerName1 = p.PartnerName1,
                PartnerName2 = p.PartnerName2,
                PhoneNumber = p.PhoneNumber,
                Province = p.Province,
                State = p.State,
                Zip = p.Zip,
                IsActive = p.IsActive,
                IsLocked = p.IsLocked,
                OtpexpiryDate = p.OtpexpiryDate,
                //Otp = p.OtpexpiryDate >= DateTime.Now ? p.Otp : ""
                Otp = p.Otp 
            }).FirstOrDefault();
        }

        public InstituteMaster GetInstituteDataByClientEmailId(string clientEmailId)
        {
            var institute = _evolvedtaxContext.InstitutesClients.FirstOrDefault(p => p.ClientEmailId == clientEmailId);

            var institutionName = institute != null
                ? _evolvedtaxContext.InstituteMasters
                    .FirstOrDefault(p => p.InstId == institute.InstituteId)
                : null;

            return institutionName;
        }
        public InstituteEntity GetEntityDataByClientEmailId(string clientEmail)
        {
            try
            {
                var entity = _evolvedtaxContext.InstitutesClients.FirstOrDefault(p => p.ClientEmailId == clientEmail);

                var entityName = entity != null
                    ? _evolvedtaxContext.InstituteEntities
                        .FirstOrDefault(p => p.EntityId == entity.EntityId)
                    : null;

                return entityName;
            }
            catch (Exception ex)
            {

                Console.WriteLine("An error occurred: " + ex.Message);
                return null;
            }
        }
        public async Task<MessageResponseModel> UpdateEntity(InstituteEntityRequest request)
        {
            var result = await _evolvedtaxContext.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC UpdateInstituteEntity
            {request.EntityId},
            {request.EntityName},
            {request.Ein},
            {request.EntityRegistrationDate},
            {request.Address1},
            {request.Address2},
            {request.City},
            {request.State},
            {request.Province},
            {request.Zip},
            {request.Country},
            {DateTime.Now.Date},
            {request.InstituteId}
            ");
            if (result > 0)
            {
                return new MessageResponseModel { Status = true };
            }
            return new MessageResponseModel { Status = false };
        }
        public async Task<MessageResponseModel> DeleteEntity(int EntityId, RecordStatusEnum RecordStatus)
        {
            //if (_evolvedtaxContext.InstitutesClients.Any(p => p.EntityId == EntityId))
            //{
            //    return new MessageResponseModel { Status = false, Message = "Please delete child records first associated with this record." };
            //}
            var result = await _evolvedtaxContext.Database.ExecuteSqlInterpolatedAsync($@"EXEC DeleteInstituteEntity {EntityId},{RecordStatus},{DateTime.Now.Date}");
            if (result > 0)
            {
                return new MessageResponseModel { Status = true };
            }
            return new MessageResponseModel { Status = false };
        }
        public async Task<MessageResponseModel> LockUnlockEntity(int[] selectedValues, bool isLocked)
        {
            var result = 0;
            foreach (var item in selectedValues)
            {
                result = await _evolvedtaxContext.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC LockUnlockEntity
                    {item},
                    {isLocked}");
            }
            if (result > 0)
            {
                var respModel = new MessageResponseModel();
                respModel.Status = true;
                var message = "";
                if (isLocked)
                {
                    if (selectedValues.Count() > 1) { message = "Records are locked"; } else { message = "Record is locked"; }
                    respModel.Message = message;
                }
                else
                {
                    if (selectedValues.Count() > 1) { message = "Records are unlocked"; } else { message = "Record is unlocked"; }
                    respModel.Message = message;
                }
                return respModel;
            }
            return new MessageResponseModel { Status = false };
        }
        public IQueryable<InstituteEntitiesResponse> GetRecyleBinEntitiesByInstId(int instId)
        {
            var query = from p in _evolvedtaxContext.InstituteEntities
                        where p.InstituteId == instId && p.IsActive == RecordStatusEnum.Trash && p.InActiveDate <= DateTime.Now.Date.AddMonths(1)
                        select new InstituteEntitiesResponse
                        {
                            Address1 = p.Address1,
                            Address2 = p.Address2,
                            City = p.City,
                            Country = p.Country,
                            Ein = p.Ein,
                            EntityId = p.EntityId,
                            EntityName = p.EntityName,
                            EntityRegistrationDate = p.EntityRegistrationDate,
                            InstituteId = p.InstituteId,
                            InstituteName = p.InstituteName,
                            LastUpdatedDate = p.LastUpdatedDate,
                            Province = p.Province,
                            State = p.State,
                            Zip = p.Zip,
                            IsActive = p.IsActive,
                            IsLocked = p.IsLocked
                        };
            return query;
        }
        public MessageResponseModel RestoreEntities(int[] selectedValues)
        {
            var response = new List<InstituteEntity>();
            foreach (var item in selectedValues)
            {
                var result = _evolvedtaxContext.InstituteEntities.First(p => p.EntityId == item);
                result.IsActive = RecordStatusEnum.Active;
                response.Add(result);
            }
            _evolvedtaxContext.UpdateRange(response);
            _evolvedtaxContext.SaveChanges();
            return new MessageResponseModel { Status = true, Message = "Records restored." };
        }
        public IQueryable<InstituteClientResponse> GetRecyleBinClientsByEntityId(int instId, int entityId)
        {
            // Fetch all MasterClientStatus records
            var clientStatuses = _evolvedtaxContext.MasterClientStatuses.ToDictionary(cs => cs.StatusId);

            var response = _evolvedtaxContext.InstitutesClients
                .Where(p => p.EntityId == entityId && p.IsActive == RecordStatusEnum.Trash && p.InActiveDate <= DateTime.Now.Date.AddMonths(1))
                .Select(p => new InstituteClientResponse
                {
                    Address1 = p.Address1,
                    Address2 = p.Address2,
                    City = p.City,
                    ClientId = p.ClientId,
                    ClientEmailId = p.ClientEmailId,
                    ClientStatusDate = p.ClientStatusDate,
                    ClientStatus = p.ClientStatus,
                    Country = p.Country,
                    EntityId = entityId,
                    EntityName = p.EntityName,
                    FileName = p.FileName,
                    InstituteId = p.InstituteId,
                    PartnerName1 = p.PartnerName1,
                    PartnerName2 = p.PartnerName2,
                    PhoneNumber = p.PhoneNumber,
                    Province = p.Province,
                    State = p.State,
                    FormName = p.FormName,
                    Zip = p.Zip,
                    IsActive = p.IsActive,
                    IsLocked = p.IsLocked,
                    StatusName = clientStatuses[(short)p.ClientStatus].StatusName ?? ""
                });
            return response;
        }
        public MessageResponseModel RestoreClients(int[] selectedValues)
        {
            var response = new List<InstitutesClient>();
            foreach (var item in selectedValues)
            {
                var result = _evolvedtaxContext.InstitutesClients.First(p => p.ClientId == item);
                result.IsActive = RecordStatusEnum.Active;
                response.Add(result);
            }
            _evolvedtaxContext.UpdateRange(response);
            _evolvedtaxContext.SaveChanges();
            return new MessageResponseModel { Status = true, Message = "Records restored." };
        }
        public async Task<MessageResponseModel> DeleteClient(int id, RecordStatusEnum RecordStatus)
        {
            var result = await _evolvedtaxContext.Database.ExecuteSqlInterpolatedAsync($@"EXEC DeleteInstituteClient {id},{RecordStatus},{DateTime.Now.Date}");
            if (result > 0)
            {
                return new MessageResponseModel { Status = true };
            }
            return new MessageResponseModel { Status = false };
        }

        public async Task<MessageResponseModel> DeleteMultipleClient(int[] id, RecordStatusEnum RecordStatus)
        {
            bool allUpdated = true;

            foreach (var clientId in id)
            {
                var result = await _evolvedtaxContext.Database.ExecuteSqlInterpolatedAsync($@"EXEC DeleteInstituteClient {clientId},{RecordStatus},{DateTime.Now.Date}");
                if (result <= 0)
                {
                    allUpdated = false;
                }
            }

            if (allUpdated)
            {
                return new MessageResponseModel { Status = true, Message = "The record has been deleted" };
            }
            else
            {
                return new MessageResponseModel { Status = false, Message = "Oops! something wrong" };
            }
        }

        public async Task<MessageResponseModel> DeleteRecentRecordClient(int timeInterval, int instId)
        {

            // Calculate the retention cutoff time based on the selected time interval
            var currentTime = DateTime.Now;

            var cutoffTimestamp = currentTime.AddMinutes(-timeInterval);
            cutoffTimestamp = new DateTime(
                cutoffTimestamp.Year, cutoffTimestamp.Month, cutoffTimestamp.Day,
                cutoffTimestamp.Hour, cutoffTimestamp.Minute, 0);

            var recordsToDelete = _evolvedtaxContext.InstitutesClients
             .Where(r => r.InstituteId == instId && r.FileUploadTime > cutoffTimestamp)
             .ToList();

            if (recordsToDelete.Count > 0)
            {
                 _evolvedtaxContext.InstitutesClients.RemoveRange(recordsToDelete);
                 _evolvedtaxContext.SaveChanges();
                return new MessageResponseModel { Status = true, Message= "The record has been successfully deleted" };
            }
            else
            {
                return new MessageResponseModel { Status = true, Message = "No records were found within the specified time interval",ErrorStatus=true };
            }
            
            return new MessageResponseModel { Status = false };

         
        }

        public async Task<MessageResponseModel> DeleteClientPermeant(int id)
        {

            var recordToDelete = _evolvedtaxContext.InstitutesClients.First(p => p.ClientId == id);
            if (recordToDelete != null)
            {
                _evolvedtaxContext.InstitutesClients.Remove(recordToDelete);
                await _evolvedtaxContext.SaveChangesAsync();
                return new MessageResponseModel { Status = true };
            }

            return new MessageResponseModel { Status = false };
        }

        public async Task<MessageResponseModel> KeepClienRecord(int id)
        {

            var recordToUpdate = _evolvedtaxContext.InstitutesClients.First(p => p.ClientId == id);
            if (recordToUpdate != null)
            {
                recordToUpdate.IsDuplicated = false;
                await _evolvedtaxContext.SaveChangesAsync();
                return new MessageResponseModel { Status = true, Message = "The record has been kept" };
            }

            return new MessageResponseModel { Status = false, Message = "Oops! something wrong" };
        }


        public async Task<MessageResponseModel> UpdateClient(InstituteClientRequest request)
        {
            var result = await _evolvedtaxContext.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC UpdateInstituteClient
            {request.ClientId},
            {request.PartnerName1},
            {request.PartnerName2},
            {request.Address1},
            {request.Address2},
            {request.City},
            {request.State},
            {request.Province},
            {request.Zip},
            {request.Country},
            {request.PhoneNumber},
            {DateTime.Now.Date},
            {request.LastUpdatedBy},
            {request.ClientEmailId}
            ");
            if (result > 0)
            {
                return new MessageResponseModel { Status = true };
            }
            return new MessageResponseModel { Status = false };
        }
        public async Task<MessageResponseModel> LockUnlockClient(int[] selectedValues, bool isLocked)
        {
            var result = 0;
            foreach (var item in selectedValues)
            {
                result = await _evolvedtaxContext.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC LockUnlockClient
                        {item},
                        {isLocked}");
            }
            if (result > 0)
            {
                var respModel = new MessageResponseModel();
                respModel.Status = true;
                var message = "";
                if (isLocked)
                {
                    if (selectedValues.Count() > 1) { message = "Records are locked"; } else { message = "Record is locked"; }
                    respModel.Message = message;
                }
                else
                {
                    if (selectedValues.Count() > 1) { message = "Records are unlocked"; } else { message = "Record is unlocked"; }
                    respModel.Message = message;
                }
                return respModel;
            }
            return new MessageResponseModel { Status = false };
        }
        public async Task<MessageResponseModel> TrashEmptyClient(int[] selectedValues, RecordStatusEnum recordStatusEnum)
        {
            var count = 0;
            foreach (var item in selectedValues)
            {
                count = await _evolvedtaxContext.Database.ExecuteSqlInterpolatedAsync($@"EXEC DeleteInstituteClient {item},{recordStatusEnum},{DateTime.Now.Date}");
            }
            if (count > 0)
            {
                return new MessageResponseModel { Status = true, Message = "Record has been deleted permanently." };
            }
            return new MessageResponseModel { Status = false };
        }
        public async Task<MessageResponseModel> TrashEmptyEntity(int[] selectedValues, RecordStatusEnum recordStatusEnum)
        {
            var count = 0;
            foreach (var item in selectedValues)
            {
                count = await _evolvedtaxContext.Database.ExecuteSqlInterpolatedAsync($@"EXEC DeleteInstituteEntity {item},{recordStatusEnum},{DateTime.Now.Date}");
            }
            if (count > 0)
            {
                return new MessageResponseModel { Status = true, Message = "Record has been deleted permanently." };
            }
            return new MessageResponseModel { Status = false };
        }
        public async Task<bool> CheckIfClientRecordExist(string clientEmail, string entityId)
        {
            var query = from client in _evolvedtaxContext.InstitutesClients
                        join entity in _evolvedtaxContext.InstituteEntities on client.EntityId equals entity.EntityId
                        where client.ClientEmailId == clientEmail && client.EntityId.ToString() == entityId
                        select new { Client = client, Entity = entity };

            var result = await query.FirstOrDefaultAsync();

            if (result != null && (result.Client.IsActive == RecordStatusEnum.EmptyTrash || result.Entity.IsActive == RecordStatusEnum.EmptyTrash))
            {
                return true;
            }

            return false;
        }
        public async Task<MessageResponseModel> UpdateEmailFrequncy(int EntityId, int emailFrequency)
        {
            var response = await _evolvedtaxContext.InstituteEntities.FirstOrDefaultAsync(p => p.EntityId == EntityId);
            if (response != null)
            {
                response.EmailFrequency = emailFrequency;
                _evolvedtaxContext.InstituteEntities.Update(response);
                await _evolvedtaxContext.SaveChangesAsync();
                return new MessageResponseModel { Status = true };
            }
            return new MessageResponseModel { Status = false };
        }
        public InstituteMasterResponse GetInstituteDataById(int instId)
        {
            return _mapper.Map<InstituteMasterResponse>(_evolvedtaxContext.InstituteMasters.FirstOrDefault(p => p.InstId == instId));
        }
        public bool UpdateInstituteMaster(InstituteMasterRequest request)
        {
            var response = _evolvedtaxContext.InstituteMasters.FirstOrDefault(p => p.InstId == request.InstId);
            response.FirstName = request.FirstName;
            response.LastName = request.LastName;
            response.InstituteLogo = request.InstituteLogo;
            response.Madd1 = request.Madd1;
            response.Madd2 = request.Madd2;
            response.Mcity = request.Mcity;
            response.Mcountry = request.Mcountry;
            response.Mprovince = request.Mprovince;
            response.Mstate = request.Mstate;
            response.Phone = request.Phone;
            response.Mzip = request.Mzip;
            response.DateFormat = request.DateFormat;
            response.Position = request.Position;
            response.Timezone = request.Timezone;

            _evolvedtaxContext.InstituteMasters.Update(response);
            _evolvedtaxContext.SaveChanges();
            return true;
        }
        public async Task<MessageResponseModel> AddClient(InstituteClientRequest request)
        {
            if (_evolvedtaxContext.InstitutesClients.Any(p => p.ClientEmailId.Trim() == request.ClientEmailId.Trim()))
            {
                return new MessageResponseModel { Status = false, Message = "Data already exist against given email." };
            }
            var model = new InstitutesClient
            {
                PartnerName1 = request.PartnerName1,
                PartnerName2 = request.PartnerName2,
                Address1 = request.Address1,
                Address2 = request.Address2,
                City = request.City,
                State = request.State,
                Province = request.Province,
                Zip = request.Zip,
                Country = request.Country,
                PhoneNumber = request.PhoneNumber,
                ClientEmailId = request.ClientEmailId,
                ClientStatus = 1,
                EntityId = request.EntityId,
                EntityName = request.EntityName,
                InstituteId = request.InstituteId,
                IsActive = RecordStatusEnum.Active,
                LastUpdatedOn = DateTime.Now.Date,
                LastUpdatedBy = request.LastUpdatedBy
            };
            await _evolvedtaxContext.AddAsync(model);
            await _evolvedtaxContext.SaveChangesAsync();
            return new MessageResponseModel { Status = true, Message = "Record inserted" };
        }
        public async Task<MessageResponseModel> AddEntity(InstituteEntityRequest request)
        {
            if (_evolvedtaxContext.InstituteEntities.Any(p => p.Ein.Trim() == request.Ein.Trim()))
            {
                return new MessageResponseModel { Status = false, Message = "Data already exist against given EIN." };
            }
            var model = new InstituteEntity
            {
                EntityName = request.EntityName,
                Ein = request.Ein,
                EntityRegistrationDate = request.EntityRegistrationDate,
                Address1 = request.Address1,
                Address2 = request.Address2,
                City = request.City,
                State = request.State,
                Province = request.Province,
                Zip = request.Zip,
                Country = request.Country,
                InstituteId = request.InstituteId,
                InstituteName = request.InstituteName,
                IsActive = RecordStatusEnum.Active,
                LastUpdatedBy = request.LastUpdatedBy,
                LastUpdatedDate = DateTime.Now.Date,
            };
            await _evolvedtaxContext.AddAsync(model);
            await _evolvedtaxContext.SaveChangesAsync();
            return new MessageResponseModel { Status = true, Message = "Record inserted" };
        }
        public bool IsEntityNameExist(string entityName, int entityId, int institueId)
        {
            if (entityId == 0 || !_evolvedtaxContext.InstituteEntities.Any(p => p.InstituteId == institueId && p.EntityId == entityId && p.EntityName == entityName.Trim()))
            {
                return !_evolvedtaxContext.InstituteEntities.Any(p => p.InstituteId == institueId && p.EntityName == entityName.Trim());
            }
            else
            {
                return true;
            }
        }
        public bool IsEINExist(string ein, int entityId, int institueId)
        {
            if (entityId == 0 || !_evolvedtaxContext.InstituteEntities.Any(p => p.InstituteId == institueId && p.EntityId == entityId && p.Ein == ein.Trim()))
            {
                return !_evolvedtaxContext.InstituteEntities.Any(p => p.InstituteId == institueId && p.Ein == ein.Trim());
            }
            else
            {
                return true;
            }
        }

        public bool SetEmailReminder(InstituteMasterRequest request)
        {
            var responseMaster = _evolvedtaxContext.InstituteMasters.FirstOrDefault(p => p.InstId == request.InstId);
            if (responseMaster != null)
            {
                responseMaster.EmailFrequency = request.EmailFrequency;
                responseMaster.IsEmailFrequency = request.IsEmailFrequency;
                _evolvedtaxContext.InstituteMasters.Update(responseMaster);

                if ((bool)responseMaster.IsEmailFrequency)
                {
                    var responses = _evolvedtaxContext.InstituteEntities.Where(p => p.InstituteId == request.InstId).ToList();
                    if (responses.Any())
                    {
                        foreach (var response in responses)
                        {
                            response.EmailFrequency = request.EmailFrequency;
                            _evolvedtaxContext.InstituteEntities.Update(response);
                        }
                    }
                }

            }

            _evolvedtaxContext.SaveChanges();
            return true;
        }

        public List<AlertRequest> GetAlertsNotification(int instituteId, bool IsuperAdmin)
        {
            List<AlertRequest> alerts = new List<AlertRequest>();

            if (IsuperAdmin)
            {
                alerts = _evolvedtaxContext.Alert
              .Where(a => a.IsRead == false)
                 .OrderByDescending(a => a.Id)
                   .Take(20)
              .Select(a => new AlertRequest
              {
                  Id = a.Id,
                  Title = a.Title,
                  AlertText = a.AlertText,
                  CreatedDate = (DateTime)a.CreatedDate
              })
              .ToList();
            }
            else
            {

                alerts = _evolvedtaxContext.Alert
               .Where(a => a.InstituteID == instituteId && a.IsRead == false)
                  .OrderByDescending(a => a.Id)
                    .Take(20)
               .Select(a => new AlertRequest
               {
                   Id = a.Id,
                   Title = a.Title,
                   AlertText = a.AlertText,
                   CreatedDate = (DateTime)a.CreatedDate
               })
               .ToList();
            }


            return alerts;
        }


        public bool MarkAlertAsRead(int id)
        {
            var response = _evolvedtaxContext.Alert.FirstOrDefault(p => p.Id == id);
            if (response != null)
            {
                response.IsRead = true;
                _evolvedtaxContext.Alert.Update(response);
                _evolvedtaxContext.SaveChanges();
                return true;

            }
            return false;


        }

        public bool MarkAllAlertsAsRead(int instituteId)
        {

            var unreadAlerts = _evolvedtaxContext.Alert.Where(a => !a.IsRead && a.InstituteID == instituteId).ToList();
            if (unreadAlerts != null)
            {
                foreach (var alert in unreadAlerts)
                {
                    alert.IsRead = true;
                }
                _evolvedtaxContext.SaveChanges();
                return true;
            }
            return false;

        }


        public List<AnnouncementRequest> GetAnnouncements()
        {

            var currentDate = DateTime.Now;
            //valid starting from 7 days after their CreatedDate and until the EndDate.
            var announcements = _evolvedtaxContext.Announcements
            .Where(a => a.EndDate >= currentDate && currentDate <= a.CreatedDate.Value.AddDays(7))
                .OrderByDescending(a => a.Id)
                .Take(20)
                .Select(a => new AnnouncementRequest
                {
                    Id = a.Id,
                    Title = a.Title,
                    Message = a.Message,
                    EndDate = (DateTime)a.EndDate,
                    CreatedDate = (DateTime)a.CreatedDate
                })
                .ToList();

            return announcements;
        }
        public IQueryable<InstituteEntitiesResponse> GetEntitiesByInstIdRole(int value, string UserId)
        {
            var result = (from eu in _evolvedtaxContext.EntitiesUsers
                          join ie in _evolvedtaxContext.InstituteEntities on eu.EntityId equals ie.EntityId
                          where eu.UserId == UserId && ie.InstituteId == value
                          select ie);

            return _mapper.Map<IQueryable<InstituteEntitiesResponse>>(result);
        }
        public async Task<MessageResponseModel> LogClientButtonClicked(string CreatedBy, string buttonText, int EntityId, string Category)
        {
            var auditLog = new AuditLog
            {
                TableName = "",
                Action = buttonText,
                CreatedAt = DateTime.Now,
                CreatedBy = CreatedBy,
                EntityId = EntityId,
                Category = Category
            };

            _evolvedtaxContext.AuditLog.Add(auditLog);
            _evolvedtaxContext.SaveChanges();
            return new MessageResponseModel { Status = true };

        }

        public (string, string) GetPayeeData(int instId)
        {
            var requestInstitue = GetInstituteDataById(instId);

            string Institution_CountryCode = "";
            if (requestInstitue.Mcountry != "United States")
            {
                var country = _evolvedtaxContext.MstrCountries.FirstOrDefault(c => c.Country == requestInstitue.Mcountry);
                if (country != null)
                {
                    Institution_CountryCode = country.CountryId;
                }
            }
            string PayData = string.Concat(
                 requestInstitue.InstitutionName, "\r\n",
                 requestInstitue.Madd1, "\r\n",
                 requestInstitue.Madd2, "\r\n",
                 requestInstitue.Mcity, ", ",
                 requestInstitue.Mstate, " ",
                 requestInstitue.Mprovince, ", ",
                 !string.IsNullOrWhiteSpace(Institution_CountryCode) ? Institution_CountryCode + ", " : "",
                 requestInstitue.Mzip, ", ",
                 requestInstitue.Phone
             );

            return (requestInstitue.Idnumber ?? string.Empty, PayData);
        }
        public DashboardRequest DashboardDataByInstituteId(int instituteId)
        {
            var instituteData = _evolvedtaxContext.InstitutesClients.Where(p => p.InstituteId == instituteId);
            var emailSubmittedRequest = new EmailNotSendCountRequest
            {
                W8_BEN_Counts = instituteData.Count(p => p.FormName == AppConstants.W8BENForm && p.ClientStatus == 1),
                W8_BEN_E_Counts = instituteData.Count(p => p.FormName == AppConstants.W8BENEForm && p.ClientStatus == 1),
                W8_ECI_Counts = instituteData.Count(p => p.FormName == AppConstants.W8ECIForm && p.ClientStatus == 1),
                W8_EXP_Counts = instituteData.Count(p => p.FormName == AppConstants.W8EXPForm && p.ClientStatus == 1),
                W8_IMY_Counts = instituteData.Count(p => p.FormName == AppConstants.W8IMYForm && p.ClientStatus == 1),
                W9_Counts = instituteData.Count(p => p.FormName == AppConstants.W9Form && p.ClientStatus == 1),
            };
            var formPendingRequest = new FormPendingCountRequest
            {
                W8_BEN_Counts = instituteData.Count(p => p.FormName == AppConstants.W8BENForm && p.ClientStatus == 2),
                W8_BEN_E_Counts = instituteData.Count(p => p.FormName == AppConstants.W8BENEForm && p.ClientStatus == 2),
                W8_ECI_Counts = instituteData.Count(p => p.FormName == AppConstants.W8ECIForm && p.ClientStatus == 2),
                W8_EXP_Counts = instituteData.Count(p => p.FormName == AppConstants.W8EXPForm && p.ClientStatus == 2),
                W8_IMY_Counts = instituteData.Count(p => p.FormName == AppConstants.W8IMYForm && p.ClientStatus == 2),
                W9_Counts = instituteData.Count(p => p.FormName == AppConstants.W9Form && p.ClientStatus == 2),
            };
            var formSubmittedRequest = new FormSubmittedCountRequest
            {
                W8_BEN_Counts = instituteData.Count(p => p.FormName == AppConstants.W8BENForm && p.ClientStatus == 3),
                W8_BEN_E_Counts = instituteData.Count(p => p.FormName == AppConstants.W8BENEForm && p.ClientStatus == 3),
                W8_ECI_Counts = instituteData.Count(p => p.FormName == AppConstants.W8ECIForm && p.ClientStatus == 3),
                W8_EXP_Counts = instituteData.Count(p => p.FormName == AppConstants.W8EXPForm && p.ClientStatus == 3),
                W8_IMY_Counts = instituteData.Count(p => p.FormName == AppConstants.W8IMYForm && p.ClientStatus == 3),
                W9_Counts = instituteData.Count(p => p.FormName == AppConstants.W9Form && p.ClientStatus == 3),
            };
            var request = new DashboardRequest();
            request.EmailNotSentCount = emailSubmittedRequest;
            request.FormPendingCount = formPendingRequest;
            request.FormSubmittedCount = formSubmittedRequest;

            return request;
        }
    }
}