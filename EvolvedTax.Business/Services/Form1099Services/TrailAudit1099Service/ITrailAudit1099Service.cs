using EvolvedTax.Data.Models.DTOs;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.DTOs.Response;
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
    public interface ITrailAudit1099Service
    {
        Task<bool> CheckIfRecipientRecordExist(string s, string e);
        Task AddUpdateRecipientAuditDetails(AuditTrail1099 request);
        AuditTrail1099 GetRecipientDataByEmailId(string RecipientEmail);
        Task<AuditTrail1099> UpdateRecipientStatus(AuditTrail1099 request);
        Task UpdateOTPStatus(AuditTrail1099 request);

    }
}
