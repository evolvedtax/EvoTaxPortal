using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Business.Services.SignupService
{
    public interface ISignupQuestionareService
    {
        public int Save(InstituteSignUpFormRequest model);
    }
}
