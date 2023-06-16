using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Business.Services.W8EXPFormService
{
    public interface IW8EXPFormService
    {

    
        public string Save(FormRequest request);
        public Task<bool> UpdateByClientEmailId(string ClientId, PdfFormDetailsRequest request);
        public FormRequest? GetDataByClientEmail(string ClientEmail);
        public string Update(FormRequest model);
    }
}
