using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Business.Services.W9FormService
{
    public interface IW9FormService
    {
        public string SaveForIndividual(FormRequest model);
        public string UpdateForIndividual(FormRequest model);
        public string SaveForEntity(FormRequest model);
        public string UpdateForEntity(FormRequest model);
        public FormRequest GetDataByClientEmailId(string ClientEmailId);
        public Task<bool> UpdateByClientEmailId(string ClientId, PdfFormDetailsRequest request);
    }
}
