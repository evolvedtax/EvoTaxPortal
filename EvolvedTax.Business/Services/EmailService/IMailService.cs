
using EvolvedTax.Data.Models.DTOs.Response;

namespace EvolvedTax.Business.MailService
{
    public interface IMailService
    {
        Task SendEmailAsync(List<InstituteClientResponse> instituteClientResponses, string subject, string content, string URL);
        Task SendEmailToInstituteAsync(string UserFullName, String Email, string subject, string content, string URL);
        Task SendOTPAsync(string OTP, string Email, string subject,string UserName, string URL);
    }
}
