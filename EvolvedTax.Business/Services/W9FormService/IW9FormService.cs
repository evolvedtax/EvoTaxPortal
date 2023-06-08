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
        public string Save(FormRequest model);
        public string Update(FormRequest model);
        public FormRequest GetDataByClientEmailId(string ClientEmailId);
        public Task<bool> UpdateByClientEmailId(string ClientId, PdfFormDetailsRequest request);
    }
}
