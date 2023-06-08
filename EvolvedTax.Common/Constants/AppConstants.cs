
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

        public const string Entity = "Entity";
        public const string Client = "Client";

        public const string IndividualStatus = "IndividualStatus";
        public const string EntityStatus = "EntityStatus";
        //-----------FILE TEMPLATES----------------------//
        public const string W9TemplateFileName = @"Form_W9.pdf";
        public const string W8BENTemplateFileName = @"Form_W8Ben.pdf";
        public const string W8ECITemplateFileName = @"Form_W8ECI.pdf";
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
        

    }
}