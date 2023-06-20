using EvolvedTax.Data.Models.DTOs.Response;
using EvolvedTax.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Business.Services.GeneralQuestionareService
{
    public interface IGeneralQuestionareService
    {
        public int Save(FormRequest model);
        public int Update(FormRequest model);
        public FormRequest? GetDataByClientEmail(string ClientEmail);
        public bool IsClientAlreadyExist(string ClientEmailId);
        TaxPayerInfo? GetTaxpayerInfoByEmailId(string emailId);
    }
}
