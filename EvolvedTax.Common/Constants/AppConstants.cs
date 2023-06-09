﻿
using System.Xml.Linq;

namespace EvolvedTax.Common.Constants
{
    public static class AppConstants
    {
        public const string AppName = @"EvolvedTax Portal";
        public const string W9Form = @"W-9";
        public const string W8Form = @"W-8";
        public const string W8BENForm = @"W-8BEN";
        public const string W8ECIForm = @"W-8ECI";
        public const string W8BENEForm = @"W-8BEN-E";
        public const string W8IMYForm = @"W-8IMY";
        public const string W8EXPForm = @"W-8EXP";
        public const string W8FormTypes= @"W8FormType";
        public const string FormPartiallySave = @"Form partially saved";

        public const string Entity = "Entity";
        public const string Client = "Client";

        public const string IndividualStatus = "IndividualStatus";
        public const string EntityStatus = "EntityStatus";

        public const short ClientStatusActive = 1;
        public const short ClientStatusEmailSent = 2;
        public const short ClientStatusFormSubmitted = 3;
        public const short ClientStatusUpdateRequest = 4;
        //-----------FILE TEMPLATES----------------------//
        public const string W9TemplateFileName = @"Form_W9.pdf";
        public const string W8BENETemplateFileName = @"Form_W8BEN-E.pdf";
        public const string W8BENTemplateFileName = @"Form_W8Ben.pdf";
        public const string W8ECITemplateFileName = @"Form_W8ECI.pdf";
        public const string W8IMYTemplateFileName = @"Form_W8IMY.pdf";
        public const string W8EXPTemplateFileName = @"Form_W-8EXP.pdf";
        public const string InstituteEntityTemplate = "InstituteEntitiesTemplate.xlsx";
        public const string InstituteClientTemplate = "InstituteClientsTemplate.xlsx";
        //-----------BUTTON CONSTANTS--------------------//
        public const string F_Family_PalaceScriptMT = "Palace Script MT";
        public const string F_Family_VladimirScript = "Vladimir Script";
        public const string F_Family_FrenchScriptMT = "French Script MT";
        public const string F_Family_SegoeScript = "Segoe Script";
        public const string F_Family_BlackadderITC = "Blackadder ITC";
        //-----------EMAIL TEMPLATE---------------------//
        public static string EmailToClient = @"Dear {{Name}},

We hope this email finds you well. As you are aware, {{InstituteName}} is required to comply with tax laws and regulations of the United States.

To ensure compliance with these regulations, we kindly request that you provide us with your tax information as soon as possible using our online portal, EvoTax.

EvoTax is an online portal which makes it easy for you to securely provide us with your tax information online.

To access the EvoTax Portal, please follow the instructions below:

Go to {{Link}}

Follow the prompts to complete your tax information.

Please note that you will need to have your tax documents ready to input the required information.

The deadline to complete this process is 7 Days, and we highly encourage you to do so as soon as possible to avoid any delays in processing of your information.

Should you have any questions or concerns, please do not hesitate to reach out to our support team at technology@evolvedtax.com.

Thank you for your cooperation in this matter.

We appreciate your prompt attention to this important task.

Regards,

Technology Team at Evolved LLC";

        public static string EmailToInstitute = @"Dear {{Name}},<br/><br/> Thank you for registering for the EvoTax Platform. We are excited to have you on board and we appreciate your interest in our platform. <br/> As a security measure, we require all users to verify their registration before they can access the full features of the platform. This helps us ensure that only authorized individuals are able to access our services.<br/><br/><br/> To verify your identity, kindly click on the verification link provided below. You will be directed to a secure login page where you will need to enter your login credentials.<br/>{{link}}<br/><br/><br/> Once your registration has been verified, you will be able to access the full features of the EvoTax Portal, including the ability to upload, manage and track your data.<br/> If you have any questions or concerns about the verification process or our platform, please do not hesitate to contact us.<br/><br/><br/>Thank you for choosing EvoTax Portal.<br/><br/> Best regards,<br/><br/> Technology Team at Evolved LLC";
        public static string LoginOTP = @"Dear {{UserName}},<br/><br/>Please find below your one time Password (OTP) to be used in the EvoTax Portal for further process. <br/><br/> {{OTP}} <br/><br/>Please check your email. The validity of this OTP is 60 minutes <br/><br/>Should you have any questions or concerns, please do not hesitate to reach out to our support team at<br/><br/>technology@evolvedtax.com<br/><br/>Thank you for your cooperation in this matter.<br/><br/>Regards,<br/>Technology Team at Evolved LLC";
        public static string ResetPassword = @"Dear User,<br/><br/>Please find below your reset password link, please click on the link to reset your password. <br/><br/> {{ResetUrl}} <br/><br/>The validity of this link is 60 minutes <br/><br/>Should you have any questions or concerns, please do not hesitate to reach out to our support team at<br/><br/>technology@evolvedtax.com<br/><br/>Thank you for your cooperation in this matter.<br/><br/>Regards,<br/>Technology Team at Evolved LLC";
    }
}