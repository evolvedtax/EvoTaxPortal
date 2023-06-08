using EvolvedTax.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Business.Services.GeneralQuestionareEntityService
{
    public interface IGeneralQuestionareEntityService
    {
        public int Save(FormRequest model);
        public int Update(FormRequest model);
        public FormRequest GetDataByClientEmail(string ClientEmail);
        public bool IsClientAlreadyExist(string ClientEmailId);
    }
}
