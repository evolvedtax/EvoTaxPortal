using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Data.Models.DTOs.Response;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using EvolvedTax.Data.Models.DTOs.Request;
using Microsoft.EntityFrameworkCore;
using EvolvedTax.Data.Models.DTOs;
using Azure;

namespace EvolvedTax.Business.Services.InstituteService
{
    public class InstituteService : IInstituteService
    {
        readonly private EvolvedtaxContext _evolvedtaxContext;
        readonly private IMapper _mapper;

        public InstituteService(EvolvedtaxContext evolvedtaxContext, IMapper mapper)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _mapper = mapper;
        }

        public IQueryable<InstituteMasterResponse> GetMaster()
        {
            return _mapper.Map<List<InstituteMasterResponse>>(_evolvedtaxContext.InstituteMasters).AsQueryable();

        }

        public IQueryable<InstituteEntitiesResponse> GetEntitiesByInstId(int InstId)
        {
            return _mapper.Map<List<InstituteEntitiesResponse>>(_evolvedtaxContext.InstituteEntities).Where(p => p.InstituteId == InstId).AsQueryable();

        }

        public IQueryable<InstituteClientResponse> GetClientByEntityId(int InstId, int EntityId)
        {
            // Fetch all MasterClientStatus records
            var clientStatuses = _evolvedtaxContext.MasterClientStatuses.ToDictionary(cs => cs.StatusId);

            var response = _evolvedtaxContext.InstitutesClients
                .Where(p => p.EntityId == EntityId)
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
                    StatusName = clientStatuses[(short)p.ClientStatus].StatusName ?? ""
                });
            return response;
        }

        public List<InstituteClientResponse> GetClientInfoByClientId(int[] ClientId)
        {
            var result = from ic in _evolvedtaxContext.InstitutesClients
                         join ie in _evolvedtaxContext.InstituteEntities on ic.InstituteId equals ie.InstituteId
                         where ClientId.Contains(ic.ClientId)
                         select new InstituteClientResponse
                         {
                             ClientEmailId = ic.ClientEmailId,
                             InstituteUserName = ic.PartnerName1 + " " + ic.PartnerName1 ?? "",
                             InstituteName = ie.EntityName ?? "",
                         };

            return result.ToList();
        }

        public async Task<MessageResponseModel> UploadEntityData(IFormFile file, int InstId, string InstituteName)
        {
            bool Status = false;
            var response = new List<int>();
            var entityList = new List<InstituteEntity>();
            using (var stream = file.OpenReadStream())
            {
                IWorkbook workbook = new XSSFWorkbook(stream);
                ISheet sheet = workbook.GetSheetAt(0); // Assuming the data is in the first sheet

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
                        InstituteName = InstituteName,
                    };
                    // Check for duplicate records based on ClientEmailId in the database
                    if (await _evolvedtaxContext.InstituteEntities.AnyAsync(p =>
                        p.Ein == entity.Ein &&
                        p.InstituteId == entity.InstituteId))
                    {
                        response.Add(row);
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
            return new MessageResponseModel { Status = Status, Message = response };
        }
        public async Task<MessageResponseModel> UploadClientData(IFormFile file, int InstId, int EntityId)
        {
            bool Status = false;
            var response = new List<int>();
            var clientList = new List<InstitutesClient>();
            using (var stream = file.OpenReadStream())
            {
                IWorkbook workbook = new XSSFWorkbook(stream);
                ISheet sheet = workbook.GetSheetAt(0); // Assuming the data is in the first sheet

                for (int row = 1; row <= sheet.LastRowNum; row++) // Starting from the second row
                {
                    IRow excelRow = sheet.GetRow(row);

                    var client = new InstitutesClient
                    {
                        EntityName = excelRow.GetCell(0)?.ToString() ?? string.Empty,
                        PartnerName1 = excelRow.GetCell(1)?.ToString() ?? string.Empty,
                        PartnerName2 = excelRow.GetCell(2)?.ToString() ?? string.Empty,
                        Address1 = excelRow.GetCell(3)?.ToString() ?? string.Empty,
                        Address2 = excelRow.GetCell(4)?.ToString() ?? string.Empty,
                        City = excelRow.GetCell(5)?.ToString() ?? string.Empty,
                        State = excelRow.GetCell(6)?.ToString() ?? string.Empty,
                        Province = excelRow.GetCell(7)?.ToString() ?? string.Empty,
                        Zip = excelRow.GetCell(8)?.ToString() ?? string.Empty,
                        Country = excelRow.GetCell(9)?.ToString() ?? string.Empty,
                        PhoneNumber = excelRow.GetCell(10)?.ToString() ?? string.Empty,
                        ClientEmailId = excelRow.GetCell(11)?.ToString() ?? string.Empty,
                        ClientStatus = 1,
                        FileName = "",
                        InstituteId = (short)InstId,
                        EntityId = EntityId,
                    };

                    // Check for duplicate records based on ClientEmailId in the database
                    if (await _evolvedtaxContext.InstitutesClients.AnyAsync(p =>
                        p.ClientEmailId == client.ClientEmailId &&
                        p.InstituteId == client.InstituteId &&
                        p.EntityId == client.EntityId))
                    {
                        response.Add(row);
                        Status = true;
                    }
                    else
                    {
                        clientList.Add(client);
                    }
                }

                await _evolvedtaxContext.InstitutesClients.AddRangeAsync(clientList);
                await _evolvedtaxContext.SaveChangesAsync();
            }
            return new MessageResponseModel { Status = Status, Message = response };

        }
        public async Task<bool> UpdateClientByClientEmailId(string ClientEmail, PdfFormDetailsRequest request)
        {
            var response = _evolvedtaxContext.InstitutesClients.Where(p => p.ClientEmailId == ClientEmail).FirstOrDefault();
            response.FileName = request.FileName;
            response.ClientStatusDate = request.EntryDate;
            response.ClientStatus = 3;
            response.FormName = request.FormName;
            _evolvedtaxContext.InstitutesClients.Update(response);
            await _evolvedtaxContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateClientStatusByClientEmailId(string ClientEmail, short status)
        {
            var response = _evolvedtaxContext.InstitutesClients.Where(p => p.ClientEmailId == ClientEmail).FirstOrDefault();
            response.ClientStatus = status;
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
            var entity = _evolvedtaxContext.InstitutesClients.FirstOrDefault(p => p.ClientEmailId == clientEmail);

            var entityName = entity != null
                ? _evolvedtaxContext.InstituteEntities
                    .FirstOrDefault(p => p.EntityId == entity.EntityId)
                : null;

            return entityName;
        }
    }
}