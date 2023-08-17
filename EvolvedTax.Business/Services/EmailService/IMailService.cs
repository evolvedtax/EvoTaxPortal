using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.DTOs.Response;

namespace EvolvedTax.Business.MailService
{
    public interface IMailService
    {
        Task SendEmailAsync(List<InstituteClientResponse> instituteClientResponses, string subject, string content, string URL);
        Task SendEmailToInstituteAsync(string UserFullName, String Email, string subject, string content, string URL);
        Task EmailVerificationAsync(string UserFullName, String Email, string subject, string token, string URL);
        Task SendOTPAsync(string OTP, string Email, string subject,string UserName, string URL);
        void SendResetPassword(string emailAddress, string str, string resetUrl);
        Task SendInvitaionEmail(List<InvitationEmailDetalsRequest> invitationEmailDetails, string uRL, int InstituteId, string subject, string administrator);
        Task SendShareInvitaionEmailSignUp(string invitationEmails, string uRL, string userId, string subject, string administrator, string businessName, string nameOfEntity, string role);
        Task SendShareInvitaionEmail(string email, string uRL, string userId, string subject, string administrator, string businessName, string nameOfEntity, string role);
        Task SendEmailForExpireSignUp(string email, string entityEmail, string Entity, string Role, DateTime? InviteDate, string InviteeName,string LoginUrl);
    }
}
