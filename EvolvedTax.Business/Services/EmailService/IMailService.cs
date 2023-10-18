using EvolvedTax.Data.Models.DTOs;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.DTOs.Response;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Data.Models.Entities._1099;

namespace EvolvedTax.Business.MailService
{
    public interface IMailService
    {
        Task SendEmailAsync(List<InstituteClientResponse> instituteClientResponses, string subject, string content, string URL, string ActionText,string userName, int instituteId = -1);
        Task SendEmailToInstituteAsync(string UserFullName, String Email, string subject, string content, string URL);
        Task EmailVerificationAsync(string UserFullName, String Email, string subject, string token, string URL, int InstituteId = -1);
        Task SendOTPAsync(string OTP, string Email, string subject, string UserName, string URL, int InstituteId = -1);
        void SendResetPassword(string emailAddress, string str, string resetUrl);
        Task SendInvitaionEmail(List<InvitationEmailDetalsRequest> invitationEmailDetails, string uRL, int InstituteId, string subject, string administrator, int instituteId = -1);
        Task SendShareInvitaionEmailSignUp(string invitationEmails, string uRL, string userId, string subject, string administrator, string businessName, string nameOfEntity, string role, int instituteId = -1);
        Task SendShareInvitaionEmail(string email, string uRL, string userId, string subject, string administrator, string businessName, string nameOfEntity, string role, int instituteId = -1);
        Task SendEmailForExpireSignUp(string email, string entityEmail, string Entity, string Role, DateTime? InviteDate, string InviteeName, string LoginUrl, int InstituteId = -1);
        Task SendEmailForChangeInstituteNameRequest(string oldInstituteName, string newInstituteName, string adminUser, string acceptLink, string rejectLink, string Comments);
        Task SendOTPToRecipientAsync(string otp, string Email, string Subject, string URL, int InstituteId = -1);
        Task SendElectronicAcceptanceEmail(string email, int EntityId, string body, string subject, string url, string form, int InstituteId = -1);
        Task<string> SendConfirmationEmailToRecipient(IpInfo? ipInfo,string email, string subject, VerifyModel model);
    }
}
