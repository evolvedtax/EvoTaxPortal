using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EvolvedTax.Business.Services.SignupService
{
    public class SignupQuestionareService : ISignupQuestionareService
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        public SignupQuestionareService(EvolvedtaxContext evolvedtaxContext)
        {
            _evolvedtaxContext = evolvedtaxContext;
        }
        public int Save(InstituteSignUpFormRequest request)
        {
            if (request.SURegistrationExpiryDate == null)
            {
                request.SURegistrationExpiryDate = request.SURegistrationDate.AddDays(365);
            }

            var model = new InstituteMaster
            {
                FirstName = request.SUFirstName,
                LastName = request.SULastName,
                InstitutionName = request.SUInstitutionName,
                //RegistrationDate = request.SURegistrationDate,
                //RegistrationExpiryDate = request.SURegistrationExpiryDate,

                EmailAddress = request.SUEmailAddress,
                SupportEmail=request.SupportEmailAddress,
                Password = request.SUPassword,
                PasswordSecuredQ1 = request.SUPasswordSecuredQ1,
                PasswordSecuredA1 = request.SUPasswordSecuredA1,
                PasswordSecuredQ2 = request.SUPasswordSecuredQ2,
                PasswordSecuredA2 = request.SUPasswordSecuredA2,
                PasswordSecuredQ3 = request.SUPasswordSecuredQ3,
                PasswordSecuredA3 = request.SUPasswordSecuredA3,
                Idtype = request.SUIDType,
                Idnumber = request.SUIDNumber,
                TypeofEntity = request.SUTypeofEntity,

                Mcountry = request.SUMCountry,
                Madd1 = request.SUMMAdd1,
                Madd2 = request.SUMMAdd2,
                Mcity = request.SUMCity,
                Mstate = request.SUMState,
                Mzip = request.SUMZip,
                Mprovince = request.SUMProvince,


                Pcountry = request.SUPCountry,
                Padd1 = request.SUMPAdd1,
                Padd2 = request.SUPPAdd2,
                Pcity = request.SUPCity,
                Pstate = request.SUPState,
                Pzip = request.SUPZip,
                Pprovince = request.SUPProvince,

                Ftin = request.SUFTIN,
                Gin = request.SUGIN,
                CountryOfIncorporation = request.SUCountryOfIncorporation,
                Status = "1",
                StatusDate = DateTime.Now,
                Phone=request.Phone




            };

            _evolvedtaxContext.InstituteMasters.Add(model);
            _evolvedtaxContext.SaveChanges();
            return model.InstId;
        }
    }
}
