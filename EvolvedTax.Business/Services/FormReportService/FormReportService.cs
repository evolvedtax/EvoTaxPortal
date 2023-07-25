using AutoMapper;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Enums;
using EvolvedTax.Data.Models.DTOs.Response;
using EvolvedTax.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Business.Services.FormReport
{
    public class FormReportService : IFormReportService
    {

        readonly private EvolvedtaxContext _evolvedtaxContext;
        readonly private IMapper _mapper;

        public FormReportService(EvolvedtaxContext evolvedtaxContext, IMapper mapper)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _mapper = mapper;
        }

        //public IQueryable<InstituteClientResponse> GetClientByInstituteId(int InstId, string formType = null)
        //{
        //    throw new NotImplementedException();
        //}

        public IQueryable<InstituteClientResponse> GetClientByInstituteId(int InstId, string formType = null)
        {
            // Fetch all MasterClientStatus records
            var clientStatuses = _evolvedtaxContext.MasterClientStatuses.ToDictionary(cs => cs.StatusId);

            var response = _evolvedtaxContext.InstitutesClients
                .Where(p => p.InstituteId == InstId && p.IsActive == RecordStatusEnum.Active && p.ClientStatus == AppConstants.ClientStatusFormSubmitted);

            if (!string.IsNullOrEmpty(formType))
            {
                response = response.Where(p =>  p.FormName == formType);
            }

            return response.Select(p => new InstituteClientResponse
            {
                Address1 = p.Address1,
                Address2 = p.Address2,
                City = p.City,
                ClientId = p.ClientId,
                ClientEmailId = p.ClientEmailId,
                ClientStatusDate = p.ClientStatusDate,
                ClientStatus = p.ClientStatus,
                Country = p.Country,
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
        }

        //public IQueryable<InstituteClientResponse> GetClientByInstituteId(int InstId)
        //{
        //    // Fetch all MasterClientStatus records
        //    var clientStatuses = _evolvedtaxContext.MasterClientStatuses.ToDictionary(cs => cs.StatusId);

        //    var response = _evolvedtaxContext.InstitutesClients
        //        .Where(p => p.InstituteId == InstId && p.IsActive == RecordStatusEnum.Active && p.ClientStatus == AppConstants.ClientStatusFormSubmitted)
        //        .Select(p => new InstituteClientResponse
        //        {
        //            Address1 = p.Address1,
        //            Address2 = p.Address2,
        //            City = p.City,
        //            ClientId = p.ClientId,
        //            ClientEmailId = p.ClientEmailId,
        //            ClientStatusDate = p.ClientStatusDate,
        //            ClientStatus = p.ClientStatus,
        //            Country = p.Country,
        //            EntityName = p.EntityName,
        //            FileName = p.FileName,
        //            InstituteId = p.InstituteId,
        //            PartnerName1 = p.PartnerName1,
        //            PartnerName2 = p.PartnerName2,
        //            PhoneNumber = p.PhoneNumber,
        //            Province = p.Province,
        //            State = p.State,
        //            FormName = p.FormName,
        //            Zip = p.Zip,
        //            IsActive = p.IsActive,
        //            IsLocked = p.IsLocked,
        //            StatusName = clientStatuses[(short)p.ClientStatus].StatusName ?? ""
        //        });
        //    return response;
        //}




    }
}

