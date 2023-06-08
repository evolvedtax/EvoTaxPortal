using Azure.Core;
using Azure;
using EvolvedTax.Data.Models.Entities;
using iTextSharp.text.pdf;
using System.Diagnostics.Metrics;
using System.Globalization;
using iTextSharp.text;
using EvolvedTax.Data.Models.DTOs.Request;

namespace EvolvedTax.Business.Services.UserService
{
    public class UserService : IUserService
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        public UserService(EvolvedtaxContext evolvedtaxContext)
        {
            _evolvedtaxContext = evolvedtaxContext;
        }

        public UserRequest Login(LoginRequest model)
        {
            var request = new UserRequest();
            var response = _evolvedtaxContext.InstituteMasters.FirstOrDefault(p => p.EmailAddress == model.UserName && p.Password == model.Password);
            if (response != null)
            {
                request.UserName = response.FirstName + " " + response.LastName;
                request.InstId = response.InstId;
                request.EmailId = response.EmailAddress ?? string.Empty;
                request.InstituteName = response.InstitutionName ?? string.Empty;
                request.IsLoggedIn = true;
                return request;
            }
            return request;
        }

        public string Save(UserRequest model)
        {
            throw new NotImplementedException();
        }
    }
}
