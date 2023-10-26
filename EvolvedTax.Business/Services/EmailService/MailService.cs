using EvolvedTax.Business.Services.Form1099Services;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.DTOs.Response;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Data.Models.Entities._1099;
using EvolvedTax.Helpers;
using System;
using System.Data.SqlTypes;
using System.Net;
using System.Net.Mail;
using System.Security.Policy;
using System.Text;

namespace EvolvedTax.Business.MailService
{
    public class MailService : IMailService
    {
        private readonly IInstituteService _instituteService;
        private readonly ITrailAudit1099Service _trailAudit1099Service;
        private EmailSetting emailSetting;
        private InstituteMaster instituteMaster;
        private readonly EvolvedtaxContext _evolvedtaxContext;
        private string SupportEmailForInstitute = "";
        private string NameForInstitute = "";
        private string ClientName = "";
        private string SupportEmailForTechnology = "technology@evolvedtax.com";


        public MailService(IInstituteService instituteService, EvolvedtaxContext evolvedtaxContext, ITrailAudit1099Service trailAudit1099Service, int instituteId = -1)
        {
            _instituteService = instituteService;
            _evolvedtaxContext = evolvedtaxContext;
            _trailAudit1099Service = trailAudit1099Service;
            //emailSetting = _evolvedtaxContext.EmailSetting.FirstOrDefault(es => es.InstID == instituteId);
        }

        public void EmailSetting(int instituteId = -1)
        {
            emailSetting = _evolvedtaxContext.EmailSetting.FirstOrDefault(es => es.InstID == instituteId);
            instituteMaster = _evolvedtaxContext.InstituteMasters.FirstOrDefault(es => es.InstId == instituteId);
            if (instituteMaster != null)
            {
                SupportEmailForInstitute = instituteMaster.SupportEmail != null ? instituteMaster.SupportEmail : instituteMaster.EmailAddress;
                NameForInstitute = instituteMaster.InstitutionName;

            }
            if (emailSetting != null && string.IsNullOrEmpty(SupportEmailForInstitute) && string.IsNullOrEmpty(NameForInstitute))
            {
                SupportEmailForInstitute = emailSetting.EmailDoamin;
                NameForInstitute = "Technology Team at Evolved LLC";
            }
            if (emailSetting == null)
            {
                emailSetting = _evolvedtaxContext.EmailSetting.FirstOrDefault(es => es.InstID == -1);
                SupportEmailForInstitute = emailSetting.EmailDoamin;
                NameForInstitute = "Technology Team at Evolved LLC";

            }
        }
        public void SetClientName(int instituteId, String Email )
        {
            var response = _evolvedtaxContext.InstitutesClients.Where(p => p.ClientEmailId == Email && p.InstituteId== instituteId).FirstOrDefault();
     
            if (response != null)
            {
              ClientName= string.Concat(response.PartnerName1," ", response.PartnerName2);

            }
      
        }
        public async Task SendEmailAsync(List<InstituteClientResponse> instituteClientResponses, string subject, string content, string URL, string ActionText, string userName, int InstituteId = -1)
        {
            EmailSetting(InstituteId);
            var FromEmail = emailSetting.EmailDoamin;
            var FromPassword = emailSetting.Password;
            var Host = emailSetting.SMTPServer;
            var Port = emailSetting.SMTPPort;
            foreach (var email in instituteClientResponses)
            {
                content = AppConstants.EmailToClient
                    .Replace("{{Name}}", email.InstituteUserName)
                    .Replace("{{InstituteName}}", email.InstituteName)
                      .Replace("{{SupportEmailForInstitute}}", SupportEmailForInstitute)
                    .Replace("{{NameForInstitute}}", NameForInstitute)
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
                    await _instituteService.LogClientButtonClicked(userName, Message, email.EntityId, "Email Send");
                }
                catch (Exception ex)
                {
                    // Exception Details
                }
            }
        }
        public async Task EmailVerificationAsync(string UserFullName, String Email, string subject, string token, string URL, int InstituteId = -1)
        {
            EmailSetting(InstituteId);
            var FromEmail = emailSetting.EmailDoamin;
            var FromPassword = emailSetting.Password;
            var Host = emailSetting.SMTPServer;
            var Port = emailSetting.SMTPPort; ;
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
            var FromEmail = emailSetting.EmailDoamin;
            var FromPassword = emailSetting.Password;
            var Host = emailSetting.SMTPServer;
            var Port = emailSetting.SMTPPort; ;
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
        public async Task SendOTPAsync(string OTP, string Email, string subject, string Username, string URL, int InstituteId = -1)
        {
            EmailSetting(InstituteId);
            var FromEmail = emailSetting.EmailDoamin;
            var FromPassword = emailSetting.Password;
            var Host = emailSetting.SMTPServer;
            var Port = emailSetting.SMTPPort; ;
            var content = AppConstants.LoginOTP
                .Replace("{{UserName}}", Username)
                 .Replace("{{SupportEmailForInstitute}}", SupportEmailForInstitute)
                 .Replace("{{NameForInstitute}}", NameForInstitute)
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
            var FromEmail = emailSetting.EmailDoamin;
            var FromPassword = emailSetting.Password;
            var Host = emailSetting.SMTPServer;
            var Port = emailSetting.SMTPPort; ;
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
        public async Task SendInvitaionEmail(List<InvitationEmailDetalsRequest> invitationEmailDetails, string uRL, int InstituteId, string subject, string administrator, int instituteId = -1)
        {
            EmailSetting(InstituteId);
            var FromEmail = emailSetting.EmailDoamin;
            var FromPassword = emailSetting.Password;
            var Host = emailSetting.SMTPServer;
            var Port = emailSetting.SMTPPort; ;
            foreach (var email in invitationEmailDetails)
            {
                var content = AppConstants.InvitationEmailForSignUp
                    .Replace("{{Name}}", "User")
                    .Replace("{{administrator}}", administrator)
                    .Replace("{{InstituteName}}", "Default")
                    .Replace("{{SupportEmailForInstitute}}", SupportEmailForInstitute)
                    .Replace("{{NameForInstitute}}", NameForInstitute)
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
        public async Task SendShareInvitaionEmailSignUp(string email, string uRL, string userId, string subject, string administrator, string businessName, string nameOfEntity, string role, int InstituteId = -1)
        {
            EmailSetting(InstituteId);
            var FromEmail = emailSetting.EmailDoamin;
            var FromPassword = emailSetting.Password;
            var Host = emailSetting.SMTPServer;
            var Port = emailSetting.SMTPPort; ;
            var content = AppConstants.InvitationEmailForShareSignUp
                .Replace("{{Name}}", "User")
                .Replace("{{administrator}}", administrator)
                .Replace("{{business}}", businessName)
                .Replace("{{entity}}", nameOfEntity)
                .Replace("{{role}}", role)
                .Replace("{{InstituteName}}", "Default")
                .Replace("{{SupportEmailForInstitute}}", SupportEmailForInstitute)
                .Replace("{{NameForInstitute}}", NameForInstitute)
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
        public async Task SendShareInvitaionEmail(string email, string uRL, string userId, string subject, string administrator, string businessName, string nameOfEntity, string role, int instituteId = -1)
        {
            EmailSetting(instituteId);
            var FromEmail = emailSetting.EmailDoamin;
            var FromPassword = emailSetting.Password;
            var Host = emailSetting.SMTPServer;
            var Port = emailSetting.SMTPPort; ;
            var content = AppConstants.InvitationEmailForShare
                .Replace("{{Name}}", userId)
                .Replace("{{administrator}}", administrator)
                .Replace("{{business}}", businessName)
                .Replace("{{entity}}", nameOfEntity)
                .Replace("{{role}}", role)
                .Replace("{{InstituteName}}", "Default")
                .Replace("{{SupportEmailForInstitute}}", SupportEmailForInstitute)
                .Replace("{{NameForInstitute}}", NameForInstitute)
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

        public async Task SendEmailForExpireSignUp(string email, string entityEmail, string Entity, string Role, DateTime? InviteDate, string InviteeName, string LoginUrl, int InstituteId = -1)
        {
            EmailSetting(InstituteId);
            var FromEmail = emailSetting.EmailDoamin;
            var FromPassword = emailSetting.Password;
            var Host = emailSetting.SMTPServer;
            var Port = emailSetting.SMTPPort; ;


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
        public async Task SendEmailForChangeInstituteNameRequest(string oldInstituteName, string newInstituteName, string adminUser, string acceptLink, string rejectLink, string Comments)
        {
            var FromEmail = emailSetting.EmailDoamin;
            var FromPassword = emailSetting.Password;
            var Host = emailSetting.SMTPServer;
            var Port = emailSetting.SMTPPort; ;


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
        public async Task SendOTPToRecipientAsync(string otp, string s, string subject, string user, int InstituteId = -1)
        {

            EmailSetting(InstituteId);
            var FromEmail = emailSetting.EmailDoamin;
            var FromPassword = emailSetting.Password;
            var Host = emailSetting.SMTPServer;
            var Port = emailSetting.SMTPPort; ;
            var content = AppConstants.LoginOTP
                .Replace("{{UserName}}", user)
                     .Replace("{{SupportEmailForInstitute}}", SupportEmailForInstitute)
                    .Replace("{{NameForInstitute}}", NameForInstitute)
                .Replace("{{OTP}}", otp);
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(FromEmail);
                message.To.Add(new MailAddress(s));
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
        public async Task SendElectronicAcceptanceEmail(string email, int EntityId, string body, string subject, string url, string form, int instituteId = -1)
        {
            EmailSetting(instituteId);
            SetClientName(instituteId,email);
            DateTime currentDate = DateTime.Now.Date; 
            DateTime DeadLinedDate = currentDate.AddDays(7);
            var Response = _evolvedtaxContext.Tbl1099_ReminderDays.Where(p => p.InstId == instituteId).FirstOrDefault();
            if (Response != null && Response.ReminderDays > 0 )
            {
                DeadLinedDate= currentDate.AddDays(Convert.ToDouble( Response.ReminderDays));
            }
            subject = "Request for Your Form 1099 Delivery Preference";
            var FromEmail = emailSetting.EmailDoamin;
            var FromPassword = emailSetting.Password;
            var Host = emailSetting.SMTPServer;
            var Port = emailSetting.SMTPPort;
            body = AppConstants.SendLinkToRecipient
               .Replace("{{link}}",
               string.Concat(url, "?s=", EncryptionHelper.Encrypt(email), "&e=", EncryptionHelper.Encrypt(EntityId.ToString()), "&f=", EncryptionHelper.Encrypt(form), "&i=", EncryptionHelper.Encrypt(instituteId.ToString())))
                 .Replace("{{SupportEmailForInstitute}}", SupportEmailForInstitute)
                 .Replace("{{ClientName}}", ClientName)
                 .Replace("{{DeadLinedDate}}", DeadLinedDate.ToString("MM/dd/yyyy"))
                    .Replace("{{NameForInstitute}}", NameForInstitute);
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(FromEmail);
                message.To.Add(new MailAddress(email));
                message.Subject = subject;
                message.IsBodyHtml = true; //to make message body as html
                message.Body = body;
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
        public async Task<string> SendConfirmationEmailToRecipient(IpInfo? ipInfo, string email, string subject, VerifyModel model, int InstituteId)
        {
            subject = "Response Email";
            EmailSetting(InstituteId);
            SetClientName(InstituteId, email);
            var tdTemplate = "<tr><td style='text-align:center;'>{{form}}</td><td style='color:{{color}};text-align:center;'>{{status}}</td></tr>";
            var tds = new StringBuilder();

            foreach (var item in model.Items)
            {
                var row = tdTemplate
                    .Replace("{{form}}", item.FormName)
                       .Replace("{{SupportEmailForInstitute}}", SupportEmailForInstitute)
                    .Replace("{{NameForInstitute}}", NameForInstitute)
                      .Replace("{{ClientName}}", ClientName)
                  .Replace("{{status}}", item.Action == "Accept" ? "E-Mail" : "Paper Copy");
                //.Replace("{{color}}", item.Action == "Accept" ? "green" : "red");

                tds.Append(row);
            }

            var FromEmail = emailSetting.EmailDoamin;
            var FromPassword = emailSetting.Password;
            var Host = emailSetting.SMTPServer;
            var Port = emailSetting.SMTPPort;
            var content = AppConstants.ConfirmationEmailToRecipient
                .Replace("{{tds}}", tds.ToString())
                    .Replace("{{SupportEmailForInstitute}}", SupportEmailForInstitute)
                    .Replace("{{NameForInstitute}}", NameForInstitute)
                       .Replace("{{SupportEmailForTechnology}}", SupportEmailForTechnology)
                    .Replace("{{ClientName}}", ClientName);
            var psdfContent = AppConstants.PdfEmailTempForElecAccep
                .Replace("{{tds}}", tds.ToString())
                .Replace("{{fromEmail}}", FromEmail)
                .Replace("{{sentTime}}", DateTime.Now.ToString())
                .Replace("{{toEmail}}", email)
                .Replace("{{subject}}", subject
                );
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
            return psdfContent;
        }

    }
}