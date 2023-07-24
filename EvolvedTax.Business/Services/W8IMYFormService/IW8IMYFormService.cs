using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Business.Services.W8IMYFormService
{
    public interface IW8IMYFormService
    {
        public string Save(FormRequest request);
        public int SavePartial(FormRequest request);
        public int UpdatePartial(FormRequest request);
        public Task<bool> UpdateByClientEmailId(string ClientEmail, PdfFormDetailsRequest request);
        public FormRequest? GetDataByClientEmail(string ClientEmail);
        public string Update(FormRequest model);

        public void ActivateRecord(string ClientEmail);

    }
}
