using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs.Response;
using EvolvedTax.Helpers;
using System.Net;
using System.Net.Mail;

namespace EvolvedTax.Business.MailService
{
    public class MailService : IMailService
    {
        private readonly IInstituteService _instituteService;

        public MailService(IInstituteService instituteService)
        {
            _instituteService = instituteService;
        }

        public async Task SendEmailAsync(List<InstituteClientResponse> instituteClientResponses, string subject, string content, string URL)
        {
            var FromEmail = "technology@evolvedtax.com";
            var FromPassword = "rme*E3&9PI@4c!f6aZng1cTc";
            var Host = "smtp.office365.com";
            var Port = 587;
            foreach (var email in instituteClientResponses)
            {
                content = AppConstants.EmailToClient
                    .Replace("{{Name}}", email.InstituteUserName)
                    .Replace("{{InstituteName}}", email.InstituteName)
                    .Replace("{{Link}}", string.Concat(URL, "?s=", EncryptionHelper.Encrypt(email.ClientEmailId), "&e=", EncryptionHelper.Encrypt(email.EntityId.ToString())));
                try
                {
                    MailMessage message = new MailMessage();
                    SmtpClient smtp = new SmtpClient();
                    message.From = new MailAddress(FromEmail);
                    message.To.Add(new MailAddress(email.ClientEmailId));
                    message.Subject = subject;
                    message.IsBodyHtml = false; //to make message body as html
                    message.Body = content;
                    smtp.Port = Port;
                    smtp.Host = Host;
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(FromEmail, FromPassword);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    await smtp.SendMailAsync(message);

                    await _instituteService.UpdateClientStatusByClientEmailId(email.ClientEmailId, 2);
                }
                catch (Exception ex)
                {
                    // Exception Details
                }
            }
        }
        public async Task SendEmailToInstituteAsync(string UserFullName, String Email, string subject, string content, string URL)
        {
            var FromEmail = "technology@evolvedtax.com";
            var FromPassword = "rme*E3&9PI@4c!f6aZng1cTc";
            var Host = "smtp.office365.com";
            var Port = 587;
            // Email = "mr.owaisalibaig@gmail.com";
            content = AppConstants.EmailToInstitute
                .Replace("{{Name}}", UserFullName)
                .Replace("{{link}}", URL);
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
            var FromEmail = "technology@evolvedtax.com";
            var FromPassword = "rme*E3&9PI@4c!f6aZng1cTc";
            var Host = "smtp.office365.com";
            var Port = 587;
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
            var FromEmail = "technology@evolvedtax.com";
            var FromPassword = "rme*E3&9PI@4c!f6aZng1cTc";
            var Host = "smtp.office365.com";
            var Port = 587;
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
    }
}