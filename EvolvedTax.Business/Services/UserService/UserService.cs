using Azure.Core;
using Azure;
using EvolvedTax.Data.Models.Entities;
using iTextSharp.text.pdf;
using System.Diagnostics.Metrics;
using System.Globalization;
using iTextSharp.text;
using EvolvedTax.Data.Models.DTOs.Request;
using Org.BouncyCastle.Asn1.Ocsp;

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
        public bool UpdateInstituteMasterOTP(string emailId, string otp, DateTime expiryDate)
        {
            var response = _evolvedtaxContext.InstituteMasters.FirstOrDefault(p => p.EmailAddress == emailId);
            if (response != null)
            {
                response.OtpexpiryDate = expiryDate;
                response.Otp = otp;
                _evolvedtaxContext.InstituteMasters.Update(response);
                _evolvedtaxContext.SaveChanges();
                return true;
            }
            return false;
        }
        public bool UpdateInstituteClientOTP(string emailId, string otp, DateTime expiryDate)
        {
            var response = _evolvedtaxContext.InstitutesClients.FirstOrDefault(p => p.ClientEmailId == emailId);
            if (response != null)
            {
                response.OtpexpiryDate = expiryDate;
                response.Otp = otp;
                _evolvedtaxContext.InstitutesClients.Update(response);
                _evolvedtaxContext.SaveChanges();
                return true;
            }
            return false;
        }

        public string Save(UserRequest model)
        {
            throw new NotImplementedException();
        }
        public UserRequest GetUserbyEmailId(string emailId)
        {
            var request = new UserRequest();
            var response = _evolvedtaxContext.InstituteMasters.FirstOrDefault(p => p.EmailAddress == emailId);
            if (response != null)
            {
                request.UserName = response.FirstName + " " + response.LastName;
                request.InstId = response.InstId;
                request.EmailId = response.EmailAddress ?? string.Empty;
                request.InstituteName = response.InstitutionName ?? string.Empty;
                request.IsLoggedIn = true;
                request.ExpiryDate = response.OtpexpiryDate;
                request.OTP = response.OtpexpiryDate >= DateTime.Now ? response.Otp : "";
                return request;
            }
            return request;
        }
    }
}
