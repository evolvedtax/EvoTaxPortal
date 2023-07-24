using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Business.Services.W8BEN_E_FormService
{
    public interface IW8BEN_E_FormService
    {
        public string SaveForEntity(W8BENERequest model);
        public string UpdateForEntity(W8BENERequest model);
        public W8BENERequest GetEntityDataByClientEmailId(string ClientEmailId);
        Task<bool> UpdateByClientEmailId(string ClientEmail, PdfFormDetailsRequest request);
        public void ActivateRecord(string ClientEmail);
    }
}
