using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Business.Services.W8ECIFormService
{
    public interface IW8ECIFormService
    {
        public string SaveForIndividual(FormRequest model);
        public string SaveForEntity(FormRequest model);
        public string UpdateForIndividual(FormRequest model);
        public string UpdateForEntity(FormRequest model);
        public FormRequest GetDataByClientEmailId(string ClientEmailId);
        Task<bool> UpdateByClientEmailId(string ClientEmail, PdfFormDetailsRequest request);
    }
}
