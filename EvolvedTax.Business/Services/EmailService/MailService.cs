using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.DTOs.Response;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Helpers;
using System.Data.SqlTypes;
using System.Net;
using System.Net.Mail;
using System.Security.Policy;

namespace EvolvedTax.Business.MailService
{
    public class MailService : IMailService
    {
        private readonly IInstituteService _instituteService;
        private readonly EvolvedtaxContext _evolvedtaxContext;

        public MailService(IInstituteService instituteService, EvolvedtaxContext evolvedtaxContext)
        {
            _instituteService = instituteService;
            _evolvedtaxContext = evolvedtaxContext;
        }

        public async Task SendEmailAsync(List<InstituteClientResponse> instituteClientResponses, string subject, string content, string URL, string ActionText, string userName)
        {
            var FromEmail = _evolvedtaxContext.EmailSetting.First().EmailDoamin;
            var FromPassword = _evolvedtaxContext.EmailSetting.First().Password;
            var Host = _evolvedtaxContext.EmailSetting.First().SMTPServer;
            var Port = _evolvedtaxContext.EmailSetting.First().SMTPPort;
            foreach (var email in instituteClientResponses)
            {
                content = AppConstants.EmailToClient
                    .Replace("{{Name}}", email.InstituteUserName)
                    .Replace("{{InstituteName}}", email.InstituteName)
                    .Replace("{{link}}", string.Concat(URL, "?s=", EncryptionHelper.Encrypt(email.ClientEmailId), "&e=", EncryptionHelper.Encrypt(email.EntityId.ToString())));
                try
                {
                    MailMessage message = new MailMessage();
                    SmtpClient smtp = new SmtpClient();
                    message.From = new MailAddress(FromEmail);
                    message.To.Add(new MailAddress(email.ClientEmailId));
                    message.Subject = subject;
                    message.IsBodyHtml = true; //to make message body as html
                    message.Body = content;
                    smtp.Port = Port;
                    smtp.Host = Host;
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(FromEmail, FromPassword);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    await smtp.SendMailAsync(message);

                    await _instituteService.UpdateClientStatusByClientEmailId(email.ClientEmailId, 2);

                    var EntityName = _evolvedtaxContext.InstituteEntities.FirstOrDefault(p => p.EntityId == Convert.ToInt32(email.EntityId))?.EntityName.Trim();
                    string Message = ActionText.Replace("{Email}", email.ClientEmailId).Replace("{EntityName}", EntityName);
                    //Message+= ActionText.Replace("{EntityName}", EntityName);
                    await _instituteService.LogClientButtonClicked(userName, Message, email.EntityId,"Email Send");
                }
                catch (Exception ex)
                {
                    // Exception Details
                }
            }
        }
        public async Task EmailVerificationAsync(string UserFullName, String Email, string subject, string token, string URL)
        {
            var FromEmail = _evolvedtaxContext.EmailSetting.First().EmailDoamin;
            var FromPassword = _evolvedtaxContext.EmailSetting.First().Password;
            var Host = _evolvedtaxContext.EmailSetting.First().SMTPServer;
            var Port = _evolvedtaxContext.EmailSetting.First().SMTPPort;;
            var content = AppConstants.EmailForEmailVerification
                .Replace("{{Name}}", UserFullName)
                .Replace("{{email}}", Email)
                .Replace("{{link}}", URL);

            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(FromEmail);
                message.To.Add(new MailAddress(Email));
                message.Subject = subject;
                message.IsBodyHtml = true; //to make message body as html
                message.Body = content;
                smtp.Port = Port;
                smtp.Host = Host;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(FromEmail, FromPassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                // Exception Details
            }

        }
        public async Task SendEmailToInstituteAsync(string UserFullName, String Email, string subject, string content, string URL)
        {
            var FromEmail = _evolvedtaxContext.EmailSetting.First().EmailDoamin;
            var FromPassword = _evolvedtaxContext.EmailSetting.First().Password;
            var Host = _evolvedtaxContext.EmailSetting.First().SMTPServer;
            var Port = _evolvedtaxContext.EmailSetting.First().SMTPPort;;
            // Email = "mr.owaisalibaig@gmail.com";
            content = AppConstants.EmailToInstitute
                .Replace("{{Name}}", UserFullName)
                .Replace("{{email}}", Email)
                .Replace("{{link}}", string.Concat(URL, EncryptionHelper.Encrypt(Email)));
            // .Replace("{{link}}", string.Concat(URL, "?clientEmail=", email.ClientEmailId));

            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(FromEmail);
                message.To.Add(new MailAddress(Email));
                message.Subject = subject;
                message.IsBodyHtml = true; //to make message body as html
                message.Body = content;
                smtp.Port = Port;
                smtp.Host = Host;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(FromEmail, FromPassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                // Exception Details
            }

        }
        public async Task SendOTPAsync(string OTP, string Email, string subject, string Username, string URL)
        {
            var FromEmail = _evolvedtaxContext.EmailSetting.First().EmailDoamin;
            var FromPassword = _evolvedtaxContext.EmailSetting.First().Password;
            var Host = _evolvedtaxContext.EmailSetting.First().SMTPServer;
            var Port = _evolvedtaxContext.EmailSetting.First().SMTPPort;;
            var content = AppConstants.LoginOTP
                .Replace("{{UserName}}", Username)
                .Replace("{{OTP}}", OTP);
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(FromEmail);
                message.To.Add(new MailAddress(Email));
                message.Subject = subject;
                message.IsBodyHtml = true; //to make message body as html
                message.Body = content;
                smtp.Port = Port;
                smtp.Host = Host;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(FromEmail, FromPassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                // Exception Details
            }

        }
        public void SendResetPassword(string emailAddress, string subject, string resetUrl)
        {
            var FromEmail = _evolvedtaxContext.EmailSetting.First().EmailDoamin;
            var FromPassword = _evolvedtaxContext.EmailSetting.First().Password;
            var Host = _evolvedtaxContext.EmailSetting.First().SMTPServer;
            var Port = _evolvedtaxContext.EmailSetting.First().SMTPPort;;
            var content = AppConstants.ResetPassword
                //.Replace("{{UserName}}", Username)
                .Replace("{{ResetUrl}}", resetUrl);
            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            message.From = new MailAddress(FromEmail);
            message.To.Add(new MailAddress(emailAddress));
            message.Subject = subject;
            message.IsBodyHtml = true; //to make message body as html
            message.Body = content;
            smtp.Port = Port;
            smtp.Host = Host;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(FromEmail, FromPassword);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(message);

        }
        public async Task SendInvitaionEmail(List<InvitationEmailDetalsRequest> invitationEmailDetails, string uRL, int InstituteId, string subject, string administrator)
        {
            var FromEmail = _evolvedtaxContext.EmailSetting.First().EmailDoamin;
            var FromPassword = _evolvedtaxContext.EmailSetting.First().Password;
            var Host = _evolvedtaxContext.EmailSetting.First().SMTPServer;
            var Port = _evolvedtaxContext.EmailSetting.First().SMTPPort;;
            foreach (var email in invitationEmailDetails)
            {
                var content = AppConstants.InvitationEmailForSignUp
                    .Replace("{{Name}}", "User")
                    .Replace("{{administrator}}", administrator)
                    .Replace("{{InstituteName}}", "Default")
                    .Replace("{{link}}", uRL.Replace("id", EncryptionHelper.Encrypt(InstituteId.ToString())).Replace("email", EncryptionHelper.Encrypt(email.InvitaionEmail)));
                try
                {
                    MailMessage message = new MailMessage();
                    SmtpClient smtp = new SmtpClient();
                    message.From = new MailAddress(FromEmail);
                    message.To.Add(new MailAddress(email.InvitaionEmail));
                    message.Subject = subject;
                    message.IsBodyHtml = true; //to make message body as html
                    message.Body = content;
                    smtp.Port = Port;
                    smtp.Host = Host;
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(FromEmail, FromPassword);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    await smtp.SendMailAsync(message);
                }
                catch (Exception ex)
                {
                    // Exception Details
                }
            }
        }
        public async Task SendShareInvitaionEmailSignUp(string email, string uRL, string userId, string subject, string administrator, string businessName, string nameOfEntity, string role)
        {
            var FromEmail = _evolvedtaxContext.EmailSetting.First().EmailDoamin;
            var FromPassword = _evolvedtaxContext.EmailSetting.First().Password;
            var Host = _evolvedtaxContext.EmailSetting.First().SMTPServer;
            var Port = _evolvedtaxContext.EmailSetting.First().SMTPPort;;
            var content = AppConstants.InvitationEmailForShareSignUp
                .Replace("{{Name}}", "User")
                .Replace("{{administrator}}", administrator)
                .Replace("{{business}}", businessName)
                .Replace("{{entity}}", nameOfEntity)
                .Replace("{{role}}", role)
                .Replace("{{InstituteName}}", "Default")
                .Replace("{{link}}", uRL.Replace("id", userId).Replace("email", EncryptionHelper.Encrypt(email)));
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(FromEmail);
                message.To.Add(new MailAddress(email));
                message.Subject = subject;
                message.IsBodyHtml = true; //to make message body as html
                message.Body = content;
                smtp.Port = Port;
                smtp.Host = Host;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(FromEmail, FromPassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                // Exception Details
            }
        }
        public async Task SendShareInvitaionEmail(string email, string uRL, string userId, string subject, string administrator, string businessName, string nameOfEntity, string role)
        {
            var FromEmail = _evolvedtaxContext.EmailSetting.First().EmailDoamin;
            var FromPassword = _evolvedtaxContext.EmailSetting.First().Password;
            var Host = _evolvedtaxContext.EmailSetting.First().SMTPServer;
            var Port = _evolvedtaxContext.EmailSetting.First().SMTPPort;;
            var content = AppConstants.InvitationEmailForShare
                .Replace("{{Name}}", userId)
                .Replace("{{administrator}}", administrator)
                .Replace("{{business}}", businessName)
                .Replace("{{entity}}", nameOfEntity)
                .Replace("{{role}}", role)
                .Replace("{{InstituteName}}", "Default")
                .Replace("{{link}}", uRL);
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(FromEmail);
                message.To.Add(new MailAddress(email));
                message.Subject = subject;
                message.IsBodyHtml = true; //to make message body as html
                message.Body = content;
                smtp.Port = Port;
                smtp.Host = Host;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(FromEmail, FromPassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                // Exception Details
            }
        }

        public async Task SendEmailForExpireSignUp(string email, string entityEmail, string Entity, string Role, DateTime? InviteDate, string InviteeName, string LoginUrl)
        {
            var FromEmail = _evolvedtaxContext.EmailSetting.First().EmailDoamin;
            var FromPassword = _evolvedtaxContext.EmailSetting.First().Password;
            var Host = _evolvedtaxContext.EmailSetting.First().SMTPServer;
            var Port = _evolvedtaxContext.EmailSetting.First().SMTPPort;;


            try
            {
                var userInviteeData = $@"
            <tr>
                <td>{entityEmail}</td>
                <td>{Entity.Trim()}</td>
                <td>{Role.Trim()}</td>
              <td>{InviteDate:MM/dd/yyyy}</td>
            </tr>";

                var content = AppConstants.EmailForExpireSignUp
                    .Replace("{{InviteeName}}", InviteeName)
                    .Replace("{{UserInviteeData}}", userInviteeData)
                    .Replace("{{LoginUrl}}", LoginUrl);

                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(FromEmail);
                message.To.Add(new MailAddress(email));
                message.Subject = "Expired Sign-Up Invitation";
                message.IsBodyHtml = true; //to make message body as html
                message.Body = content;
                smtp.Port = Port;
                smtp.Host = Host;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(FromEmail, FromPassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }
        public async Task SendEmailForChangeInstituteNameRequest(string oldInstituteName, string newInstituteName,string adminUser, string acceptLink, string rejectLink, string Comments)
        {
            var FromEmail = _evolvedtaxContext.EmailSetting.First().EmailDoamin;
            var FromPassword = _evolvedtaxContext.EmailSetting.First().Password;
            var Host = _evolvedtaxContext.EmailSetting.First().SMTPServer;
            var Port = _evolvedtaxContext.EmailSetting.First().SMTPPort;;


            try
            {
                var content = AppConstants.RequestForChangeInstituteName
                    .Replace("{{adminUser}}", adminUser)
                    .Replace("{{institute}}", oldInstituteName)
                    .Replace("{{oldInstituteName}}", oldInstituteName)
                    .Replace("{{newInstituteName}}", newInstituteName)
                    .Replace("{{reason}}", Comments)
                    .Replace("{{acceptLink}}", acceptLink)
                    .Replace("{{rejectLink}}", rejectLink);

                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(FromEmail);
                message.To.Add(new MailAddress("niqbal@evolvedtax.com"));
                //message.To.Add(new MailAddress("aghazipura@evolvedtax.com"));
                //message.To.Add(new MailAddress("mjunaid@evolvedtax.com"));
                //message.To.Add(new MailAddress("mmcnally@evolvedtax.com"));
                message.Subject = "Action required for change of the Institute Name";
                message.IsBodyHtml = true; //to make message body as html
                message.Body = content;
                smtp.Port = Port;
                smtp.Host = Host;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(FromEmail, FromPassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }

    }
}