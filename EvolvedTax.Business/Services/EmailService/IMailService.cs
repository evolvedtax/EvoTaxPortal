
using EvolvedTax.Data.Models.DTOs.Response;

namespace EvolvedTax.Business.MailService
{
    public interface IMailService
    {
        Task SendEmailAsync(List<InstituteClientResponse> instituteClientResponses, string subject, string content, string URL);
    }
}
