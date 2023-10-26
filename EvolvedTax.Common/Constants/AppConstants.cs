
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
        public const string W8FormTypes = @"W8FormType";



        public const string Entity = "Entity";
        public const string Client = "Client";
        
        public const string IndividualEntityType = "Individual";
        public const string BusinessEntityType = "Business";

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
        public const string NEC_1099_TemplateFileName = @"Form_1099_NEC.pdf";
        public const string INT_1099_TemplateFileName = @"Form_1099_INT.pdf";
        public const string B_1099_TemplateFileName = @"Form_1099_B.pdf";
        public const string C_1099_TemplateFileName = @"Form_1099_C.pdf";
        public const string CAP_1099_TemplateFileName = @"Form_1099_CAP.pdf";
        public const string G_1099_TemplateFileName = @"Form_1099_G.pdf";
        public const string LTC_1099_TemplateFileName = @"Form_1099_LTC.pdf";
        public const string PATR_1099_TemplateFileName = @"Form_1099_PATR.pdf";
        public const string R_1099_TemplateFileName = @"Form_1099_R.pdf";
        public const string SA_1099_TemplateFileName = @"Form_1099_SA.pdf";
        public const string SB_1099_TemplateFileName = @"Form_1099_SB.pdf";
        public const string NEC_1099_Page2_TemplateFileName = @"Form_1099_NEC_2.pdf";
        public const string InstituteEntityTemplate = "InstituteEntitiesTemplate.xlsx";
        public const string InstituteClientTemplate = "InstituteClientsTemplate.xlsx";

        //----------1099 TEMPLATES----------//
        //public const string Form1099MISCTemplateFileName = @"Forms/f1099msc.pdf";
        public const string Form1099MISCTemplateFileName = @"f1099msc.pdf";
        public const string Form1099ATemplateFileName = @"f1099a.pdf";
        public const string Form1099DIVTemplateFileName = @"f1099div.pdf";
        public const string Form1099LSTemplateFileName = @"f1099ls.pdf";
        public const string Form1099KTemplateFileName = @"f1099k.pdf";
        public const string Form1099OIDTemplateFileName = @"f1099oid.pdf";
        public const string Form1099QTemplateFileName = @"f1099q.pdf";
        public const string Form1099STemplateFileName = @"f1099s.pdf";

        public const string Form1099MISCExcelTemplate = "Form1099MISCTemplate.xlsx";
        public const string Form1099AExcelTemplate = "Form1099ATemplate.xlsx";
        public const string Form1099NECExcelTemplate = "Form1099NECTemplate.xlsx";
        public const string Form1099INTExcelTemplate = "Form1099INTTemplate.xlsx";
        public const string Form1099DIVExcelTemplate = "Form1099DIVTemplate.xlsx";
        public const string Form1099LSExcelTemplate = "Form1099LSTemplate.xlsx";
        public const string Form1099KExcelTemplate = "Form1099Kemplate.xlsx";
        public const string Form1099OIDExcelTemplate = "Form1099OIDemplate.xlsx";
        public const string Form1099QExcelTemplate = "Form1099Qemplate.xlsx";
        public const string Form1099SExcelTemplate = "Form1099Semplate.xlsx";
        public const string Form1099_B_ExcelTemplate = "Form1099_B_Template.xlsx";
        public const string Form1099_C_ExcelTemplate = "Form1099_C_Template.xlsx";
        public const string Form1099_CAP_ExcelTemplate = "Form1099_CAP_Template.xlsx";
        public const string Form1099_G_ExcelTemplate = "Form1099_G_Template.xlsx";
        public const string Form1099_LTC_ExcelTemplate = "Form1099_LTC_Template.xlsx";
        public const string Form1099_PATR_ExcelTemplate = "Form1099_PATR_Template.xlsx";
        public const string Form1099_R_ExcelTemplate = "Form1099_R_Template.xlsx";
        public const string Form1099_SA_ExcelTemplate = "Form1099_SA_Template.xlsx";
        public const string Form1099_SB_ExcelTemplate = "Form1099_SB_Template.xlsx";


        public const string Form1099MISC = @"Form1099MISC";
        public const string Form1099A = @"Form1099A";
        public const string Form1099NEC = @"Form1099NEC";
        public const string Form1099INT = @"Form1099INT";
        public const string Form1099DIV = @"Form1099DIV";
        public const string Form1099LS = @"Form1099LS";
        public const string Form1099K = @"Form1099K";
        public const string Form1099OID = @"Form1099OID";
        public const string Form1099Q = @"Form1099Q";
        public const string Form1099S = @"Form1099S";
        public const string Form1099B = @"Form1099B";
        public const string Form1099C = @"Form1099C";
        public const string Form1099CAP = @"Form1099CAP";
        public const string Form1099G = @"Form1099G";
        public const string Form1099LTC = @"Form1099LTC";
        public const string Form1099PATR = @"Form1099PATR";
        public const string Form1099R = @"Form1099R";
        public const string Form1099SA = @"Form1099SA";
        public const string Form1099SB = @"Form1099SB";

        public const string NEC1099Form = @"Form_1099NEC";
        public const string INT1099Form = @"Form_1099INT";
        public const string B1099Form = @"Form_1099B";
        public const string C1099Form = @"Form_1099C";
        public const string CAP1099Form = @"Form_1099CAP";
        public const string G1099Form = @"Form_1099G";
        public const string LTC1099Form = @"Form_1099LTC";
        public const string PATR1099Form = @"Form_1099PATR";
        public const string R1099Form = @"Form_1099R";
        public const string SA1099Form = @"Form_1099SA";
        public const string SB1099Form = @"Form_1099SB";
        public const string FormPartiallySave = @"Form partially saved";

        //-----------BUTTON CONSTANTS--------------------//
        //public const string F_Family_PalaceScriptMT = "Palace Script MT";
        //public const string F_Family_VladimirScript = "Vladimir Script";
        //public const string F_Family_FrenchScriptMT = "French Script MT";
        //public const string F_Family_SegoeScript = "Segoe Script";
        //public const string F_Family_BlackadderITC = "Blackadder ITC";
        public const string F_Family_DancingScript_Bold = "DancingScript-Bold";
        public const string F_Family_Yellowtail_Regular = "Yellowtail-Regular";
        public const string F_Family_VLADIMIR = "VLADIMIR";
        public const string F_Family_SegoeScript = "Segoe Script";
        public const string F_Family_Sugar_Garden = "Sugar Garden";
        //-----------EMAIL TEMPLATE---------------------//
        public static string EmailToClient = @"Dear {{Name}},
<br/><br/>
We hope this e-mail finds you well. As you are aware, {{InstituteName}} is required to comply with tax laws and regulations of the United States.
<br/><br/>
To ensure compliance with these regulations, we kindly request that you provide us with your tax information as soon as possible using our online portal, EvoTax.
<br/>
EvoTax is an online portal which makes it easy for you to securely provide us with your tax information online.
<br/><br/>
To access the EvoTax Portal, please follow the instructions below:
<br/><br/>
<!--[if mso]>
  <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" style=""margin: 0 auto;"">
    <tr>
      <td align=""center"" bgcolor=""#1ab394"" style=""border-radius: 4px;"">
        <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{{link}}"" style=""height: 40px; v-text-anchor: middle; width: 200px;"" arcsize=""10%"" stroke=""f"" fillcolor=""#1ab394"">
          <w:anchorlock/>
          <center style=""color: #ffffff; font-family: Arial, sans-serif; font-size: 16px; font-weight: bold;"">
            GET STARTED
          </center>
        </v:roundrect>
      </td>
    </tr>
  </table>
<![endif]-->
<!--[if !mso]><!-->
  <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" style=""margin: 0 auto;"">
    <tr>
      <td align=""center"" bgcolor=""#1ab394"" style=""border-radius: 4px;"">
        <a href=""{{link}}"" target=""_blank"" style=""font-size: 16px; font-family: Arial, sans-serif; color: #FFFFFF; text-decoration: none; display: inline-block; padding: 10px 20px;"">
          GET STARTED
        </a>
      </td>
    </tr>
  </table>
<!--<![endif]-->

<br/><br/>
Follow the prompts to complete your tax information.
<br/><br/>
Please note that you will need to have your tax documents ready to input the required information.
<br/><br/>
The deadline to complete this process is 7 Days, and we highly encourage you to do so as soon as possible to avoid any delays in processing of your information.
<br/><br/>
Should you have any questions or concerns, please do not hesitate to reach out to our support team at {{SupportEmailForInstitute}}.
<br/><br/>
Thank you for your cooperation in this matter.
<br/><br/>
We appreciate your prompt attention to this important task.
<br/><br/>
Regards,
<br/><br/>
{{NameForInstitute}}";
        public static string InvitationEmailForSignUp = @"Dear {{Name}},
                                                        <br /><br />
                                                        We hope this e-mail finds you well. <strong>{{administrator}}</strong> invited you to register with EvoTax Portal.
                                                        <br /><br />
                                                        To access the EvoTax Portal, please follow the instructions below:
                                                        <br /><br />
                                                        <!--[if mso]>
                                                          <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" style=""margin: 0 auto;"">
                                                            <tr>
                                                              <td align=""center"" bgcolor=""#1ab394"" style=""border-radius: 4px;"">
                                                                <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{{link}}"" style=""height: 40px; v-text-anchor: middle; width: 200px;"" arcsize=""10%"" stroke=""f"" fillcolor=""#1ab394"">
                                                                  <w:anchorlock/>
                                                                  <center style=""color: #ffffff; font-family: Arial, sans-serif; font-size: 16px; font-weight: bold;"">
                                                                    Accept Invite
                                                                  </center>
                                                                </v:roundrect>
                                                              </td>
                                                            </tr>
                                                          </table>
                                                        <![endif]-->
                                                        <!--[if !mso]><!-->
                                                        <table cellspacing="" 0"" cellpadding="" 0"" border="" 0"" align="" center"" style="" margin: 0 auto;"">
                                                            <tr>
                                                                <td align="" center"" bgcolor="" #1ab394"" style="" border-radius: 4px;"">
                                                                    <a href="" {{link}}"" target="" _blank"" style="" font-size: 16px; font-family: Arial, sans-serif; color:
                                                                        #FFFFFF; text-decoration: none; display: inline-block; padding: 10px 20px;"">
                                                                        Accept Invite
                                                                    </a>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <!--<![endif]-->
                                                        
                                                        <br /><br />
                                                        Follow the prompts to complete your registration.
                                                        <br /><br />
                                                        Should you have any questions or concerns, please do not hesitate to reach out to our support team at
                                                       {{SupportEmailForInstitute}}.
                                                        <br /><br />
                                                        Thank you for your cooperation in this matter.
                                                        <br /><br />
                                                        Regards,
                                                        <br /><br />
                                                        {{NameForInstitute}}";
        public static string InvitationEmailForShareSignUp = @"Dear {{Name}},
                                                        <br /><br />
                                                        We hope this e-mail finds you well. <strong>{{administrator}}</strong> from <strong>{{business}}</strong> invited you to register with EvoTax Portal and access <strong>{{entity}}</strong> as <strong>{{role}}</strong>.
                                                        <br /><br />
                                                        To access the EvoTax Portal, please follow the instructions below:
                                                        <br /><br />
                                                        <!--[if mso]>
                                                          <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" style=""margin: 0 auto;"">
                                                            <tr>
                                                              <td align=""center"" bgcolor=""#1ab394"" style=""border-radius: 4px;"">
                                                                <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{{link}}"" style=""height: 40px; v-text-anchor: middle; width: 200px;"" arcsize=""10%"" stroke=""f"" fillcolor=""#1ab394"">
                                                                  <w:anchorlock/>
                                                                  <center style=""color: #ffffff; font-family: Arial, sans-serif; font-size: 16px; font-weight: bold;"">
                                                                    Accept Invite
                                                                  </center>
                                                                </v:roundrect>
                                                              </td>
                                                            </tr>
                                                          </table>
                                                        <![endif]-->
                                                        <!--[if !mso]><!-->
                                                        <table cellspacing="" 0"" cellpadding="" 0"" border="" 0"" align="" center"" style="" margin: 0 auto;"">
                                                            <tr>
                                                                <td align="" center"" bgcolor="" #1ab394"" style="" border-radius: 4px;"">
                                                                    <a href="" {{link}}"" target="" _blank"" style="" font-size: 16px; font-family: Arial, sans-serif; color:
                                                                        #FFFFFF; text-decoration: none; display: inline-block; padding: 10px 20px;"">
                                                                        Accept Invite
                                                                    </a>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <!--<![endif]-->
                                                        
                                                        <br /><br />
                                                        Follow the prompts to complete your registration.
                                                        <br /><br />
                                                        Should you have any questions or concerns, please do not hesitate to reach out to our support team at
                                                        {{SupportEmailForInstitute}}.
                                                        <br /><br />
                                                        Thank you for your cooperation in this matter.
                                                        <br /><br />
                                                        Regards,
                                                        <br /><br />
                                                        {{NameForInstitute}}";
        public static string InvitationEmailForShare = @"Dear {{Name}},
                                                        <br /><br />
                                                        We hope this e-mail finds you well. <strong>{{administrator}}</strong> from <strong>{{business}}</strong> invited you to access <strong>{{entity}}</strong> as <strong>{{role}}</strong>.
                                                        <br /><br />
                                                        To access the EvoTax Portal, please follow the instructions below:
                                                        <br /><br />
                                                        <!--[if mso]>
                                                          <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" style=""margin: 0 auto;"">
                                                            <tr>
                                                              <td align=""center"" bgcolor=""#1ab394"" style=""border-radius: 4px;"">
                                                                <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{{link}}"" style=""height: 40px; v-text-anchor: middle; width: 200px;"" arcsize=""10%"" stroke=""f"" fillcolor=""#1ab394"">
                                                                  <w:anchorlock/>
                                                                  <center style=""color: #ffffff; font-family: Arial, sans-serif; font-size: 16px; font-weight: bold;"">
                                                                    Click to Access
                                                                  </center>
                                                                </v:roundrect>
                                                              </td>
                                                            </tr>
                                                          </table>
                                                        <![endif]-->
                                                        <!--[if !mso]><!-->
                                                        <table cellspacing="" 0"" cellpadding="" 0"" border="" 0"" align="" center"" style="" margin: 0 auto;"">
                                                            <tr>
                                                                <td align="" center"" bgcolor="" #1ab394"" style="" border-radius: 4px;"">
                                                                    <a href="" {{link}}"" target="" _blank"" style="" font-size: 16px; font-family: Arial, sans-serif; color:
                                                                        #FFFFFF; text-decoration: none; display: inline-block; padding: 10px 20px;"">
                                                                        Click to Access
                                                                    </a>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <!--<![endif]-->
                                                        
                                                        <br /><br />
                                                        Should you have any questions or concerns, please do not hesitate to reach out to our support team at
                                                        {{SupportEmailForInstitute}}.
                                                        <br /><br />
                                                        Regards,
                                                        <br /><br />
                                                        {{NameForInstitute}}";

        public static string EmailToInstitute = @"Dear {{Name}},<br/><br/> Thank you for registering for the EvoTax Platform. We are excited to have you on board and we appreciate your interest in our platform. <br/> As a security measure, we require all users to verify their registration before they can access the full features of the platform. This helps us ensure that only authorized individuals are able to access our services.<br/><br/><br/> To verify your identity, kindly click on the verification link provided below. You will be directed to a secure login page where you will need to enter your login credentials.<br/><br/> <!--[if mso]>
  <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" style=""margin: 0 auto;"">
    <tr>
      <td align=""center"" bgcolor=""#1ab394"" style=""border-radius: 4px;"">
        <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{{link}}"" style=""height: 40px; v-text-anchor: middle; width: 200px;"" arcsize=""10%"" stroke=""f"" fillcolor=""#1ab394"">
          <w:anchorlock/>
          <center style=""color: #ffffff; font-family: Arial, sans-serif; font-size: 16px; font-weight: bold;"">
            GET STARTED
          </center>
        </v:roundrect>
      </td>
    </tr>
  </table>
<![endif]-->
<!--[if !mso]><!-->
  <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" style=""margin: 0 auto;"">
    <tr>
      <td align=""center"" bgcolor=""#1ab394"" style=""border-radius: 4px;"">
        <a href=""{{link}}"" target=""_blank"" style=""font-size: 16px; font-family: Arial, sans-serif; color: #FFFFFF; text-decoration: none; display: inline-block; padding: 10px 20px;"">
          GET STARTED
        </a>
      </td>
    </tr>
  </table>
<!--<![endif]--> <br/><br/>User Name: {{email}}<br/><br/> Once your registration has been verified, you will be able to access the full features of the EvoTax Portal, including the ability to upload, manage and track your data.<br/> If you have any questions or concerns about the verification process or our platform, please do not hesitate to contact us.<br/><br/><br/>Thank you for choosing EvoTax Portal.<br/><br/> Best regards,<br/><br/> Technology Team at Evolved LLC";
        public static string EmailForEmailVerification = @"Dear {{Name}},<br/><br/> Thank you for registering for the EvoForms. We are excited to have you on board and we appreciate your interest in our platform. <br/> As a security measure, we require all users to verify their account. This helps us ensure that only authorized individuals are able to access our services.<br/><br/><br/> To verify your account, kindly click on the verification link provided below.<br/><br/> <!--[if mso]>
  <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""left"" style=""margin: 0 auto;"">
    <tr>
      <td align=""left"" bgcolor=""#1ab394"" style=""border-radius: 4px;"">
        <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{{link}}"" style=""height: 40px; v-text-anchor: middle; width: 200px;"" arcsize=""10%"" stroke=""f"" fillcolor=""#1ab394"">
          <w:anchorlock/>
          <center style=""color: #ffffff; font-family: Arial, sans-serif; font-size: 14px; font-weight: bold;"">
           Verify
          </center>
        </v:roundrect>
      </td>
    </tr>
  </table>
<![endif]-->
<!--[if !mso]><!-->
  <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""left"" style=""margin: 0 auto;"">
    <tr>
      <td align=""left"" bgcolor=""#1ab394"" style=""border-radius: 4px;"">
        <a href=""{{link}}"" target=""_blank"" style=""font-size: 14px; font-family: Arial, sans-serif; color: #FFFFFF; text-decoration: none; display: inline-block; padding: 10px 20px;"">
         Verify
        </a>
      </td>
    </tr>
  </table>
<!--<![endif]--> <br/><br/>User Name: {{email}}<br/><br/> Once your account has been verified, you will be able to access the full features of the EvoForms, including the ability to upload, manage and track your data.<br/> If you have any questions or concerns about the verification process or our platform, please do not hesitate to contact us.<br/><br/><br/>Thank you for choosing EvoForms.<br/><br/> Best regards,<br/><br/> Technology Team at Evolved LLC";
        public static string LoginOTP = @"Dear {{UserName}},<br/><br/>Please find below your one time Password (OTP) to be used for logging into EvoForms. <br/><br/> {{OTP}} <br/><br/> The validity of this OTP is 60 minutes <br/><br/>Should you have any questions or concerns, please do not hesitate to reach out to our support team at<br/><br/>{{SupportEmailForInstitute}}<br/><br/>Thank you for your cooperation in this matter.<br/><br/>Regards,<br/>{{NameForInstitute}}";
        public static string ResetPassword = @"Dear User,<br/><br/>Please find below your reset password link, please click on the link to reset your password. <br/><br/> {{ResetUrl}} <br/><br/>The validity of this link is 60 minutes <br/><br/>Should you have any questions or concerns, please do not hesitate to reach out to our support team at<br/><br/>technology@evolvedtax.com<br/><br/>Thank you for your cooperation in this matter.<br/><br/>Regards,<br/>Technology Team at Evolved LLC";
        //public static string SendLinkToRecipient = @"Dear User,<br/><br/>Please give us permission to send you an electronic copy of 1099 form through your e-mail. Please find below the link to accept or reject. <br/><br/> <a href=""{{link}}"">Click here</a> <br/><br/>The validity of this link is 60 minutes <br/><br/>Should you have any questions or concerns, please do not hesitate to reach out to our support team at<br/><br/>technology@evolvedtax.com<br/><br/>Thank you for your cooperation in this matter.<br/><br/>Regards,<br/>Technology Team at Evolved LLC";
        public static string SendLinkToRecipient = "<p>Dear {{ClientName}},</p>\n" +
     "<p>I hope this message finds you well. As the tax season approaches, we want to ensure a smooth and efficient process for our valued associates and contractors. To this end, we are reaching out to inquire about your preferred method of receiving your Form 1099.</p>\n" +
     "<p>You have the option to receive your Form 1099 electronically via email or in the traditional paper format through regular mail. To make your choice, please click on the link provided below, which will direct you to an EvoForms, secure platform where you can specify your delivery preference:</p>\n" +
     "<p><a href=\"{{link}}\">Click here</a></p>\n" +
     "<p>We understand that privacy and security are paramount when dealing with sensitive tax-related documents. Please rest assured that we have taken every precaution to safeguard your information. Our electronic delivery method is secure and ensures timely access to your tax documents.</p>\n" +
     "<p>However, if you prefer to receive a hard copy via mail, we will promptly send it to the address on file, so please make sure your contact information is up to date. You can update your address information by contacting our HR or Finance team if needed.</p>\n" +
     "<p>To ensure that your Form 1099 is delivered according to your preference, please make your selection via the provided link by {{DeadLinedDate}}.</p>\n" +
     "<p>If you have any questions or concerns regarding this process or require assistance with your choice, please do not hesitate to contact our dedicated support team at {{SupportEmailForInstitute}}.</p>\n" +
     "<p>Your cooperation in this matter is greatly appreciated, as it will help us streamline our processes and better serve your needs during this tax season.</p>\n" +
     "<p>Thank you for your continued collaboration, and we look forward to assisting you with your Form 1099 delivery.</p>\n" +
     "<p>Regards,<br/>{{NameForInstitute}}</p>";


        public static string RequestForChangeInstituteName = @"Dear Administrator(s), 
                                                        <br /><br />
                                                        We hope this e-mail finds you well. <strong>{{adminUser}}</strong> of <strong>{{institute}}</strong> requested to change the name of the institute from ""{{oldInstituteName}}"" to ""{{newInstituteName}}"".
                                                        <br /><br />
                                                        Your approval is required in this regard.
                                                        <br /><br />
                                                        Below is the reason submitted by the user for the request of change name.
                                                        <br/>
                                                        ({{reason}})
                                                        <br /><br />
                                                        <!--[if mso]>
                                                          <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" style=""margin: 0 auto;"">
                                                            <tr>
                                                              <td align=""center"" bgcolor=""#1ab394"" style=""border-radius: 4px;"">
                                                                <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{{acceptLink}}"" style=""height: 40px; v-text-anchor: middle; width: 200px;"" arcsize=""10%"" stroke=""f"" fillcolor=""#1ab394"">
                                                                  <w:anchorlock/>
                                                                  <center style=""color: #ffffff; font-family: Arial, sans-serif; font-size: 16px; font-weight: bold;"">
                                                                    Approve
                                                                  </center>
                                                                </v:roundrect>
                                                              </td>
                                                              <td align=""center"" bgcolor=""#FF0000"" style=""border-radius: 4px;"">
                                                                <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{{rejectLink}}"" style=""height: 40px; v-text-anchor: middle; width: 200px;"" arcsize=""10%"" stroke=""f"" fillcolor=""#FF0000"">
                                                                  <w:anchorlock/>
                                                                  <center style=""color: #ffffff; font-family: Arial, sans-serif; font-size: 16px; font-weight: bold;"">
                                                                    Reject
                                                                  </center>
                                                                </v:roundrect>
                                                              </td>
                                                            </tr>
                                                          </table>
                                                        <![endif]-->
                                                        <!--[if !mso]><!-->
                                                        <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" style=""margin: 0 auto;"">
                                                            <tr>
                                                                <td align=""center"" bgcolor=""#1ab394"" style=""border-radius: 4px;"">
                                                                    <a href=""{{acceptLink}}"" target=""_blank"" style=""font-size: 16px; font-family: Arial, sans-serif; color:
                                                                        #FFFFFF; text-decoration: none; display: inline-block; padding: 10px 20px;"">
                                                                        Approve
                                                                    </a>
                                                                </td>
                                                                <td align=""center"" bgcolor=""#FF0000"" style=""border-radius: 4px;"">
                                                                    <a href=""{{rejectLink}}"" target=""_blank"" style=""font-size: 16px; font-family: Arial, sans-serif; color:
                                                                        #FFFFFF; text-decoration: none; display: inline-block; padding: 10px 20px;"">
                                                                        Reject
                                                                    </a>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <!--<![endif]-->
                                                        
                                                        <br /><br />
                                                        Thank you for your cooperation in this matter. 
                                                        <br /><br />
                                                        We appreciate your prompt attention to this important task. 
                                                        <br /><br />
                                                        Regards,
                                                        <br /><br />
                                                        Technology Team at Evolved LLC";

        public const string EmailForExpireSignUp = @"
        Dear {{InviteeName}},
        <br><br>
        We hope this e-mail finds you well. Below user(s) invite links have expired due to non-acceptance of the invites:
        <br><br>
        <table border='1'>
            <tr>
                <th>Name of the User/E-mail</th>
                <th>Name of the Entity</th>
                <th>Role</th>
                <th>Invite Date</th>
            </tr>
            {{UserInviteeData}}
        </table>
        <br><br>
        Please send them new invites for accessing the Evo Tax Portal.
        <br><br>
        <a href='{{LoginUrl}}' style='padding: 10px 20px; background-color: #1ab394; color: white; text-decoration: none; border-radius: 5px;'>Login to EvoTax Portal</a>
        <br><br>
        Should you have any questions or concerns, please do not hesitate to reach out to our support team at technology@evolvedtax.com.
        <br><br>
        Regards,
        <br><br>
        Technology Team at Evolved LLC
    ";

        //public static string ConfirmationEmailToRecipient = @"Dear User,<br/><br/>
        //                        We received your confirmation for 1099 Forms via e-mail as follows:<br/><br/>

        //                        <table border='1'>
        //                            <tr>
        //                                <th style='width:20%'>Form</th>
        //                                <th style='width:20%'>Status</th>
        //                            </tr>
        //                          {{tds}}
        //                        </table>
        //                        <br/><br/>
        //                        If it was not you, please contact us at technology@evolvedtax.com immediately.
        //                        <br/><br/>
        //                        Thank you for your cooperation in this matter.<br/><br/>
        //                        Regards,<br/>
        //                        Technology Team at Evolved LLC";

        public static string ConfirmationEmailToRecipient = @"Dear {{ClientName}},

    <p>We hope this message finds you well. We are writing to confirm that your choice to receive the following Form 1099s via e-mail or regular mail has been successfully recorded, and we will ensure that your tax document is delivered to you as per your preference.</p>

        <table border='1'>
                                    <tr>
                                        <th style='width:20%'>Form</th>
                                        <th style='width:20%'>Selected method of delivery</th>
                                    </tr>
                                  {{tds}}
                                </table>
								    <br/><br/>

    <p>If you choose to receive a soft copy via email, please keep an eye on your inbox in the coming weeks for the electronic version of your Form 1099.</p>

    <p>For those who have opted to receive a paper copy, please ensure that your contact information, including your mailing address, is up to date in our records. If any updates are required, please contact at {{SupportEmailForInstitute}}.</p>

    <p>If this selection was not made by you, we kindly request that you contact us immediately to rectify the situation. Your security and privacy are of utmost importance to us, and we want to ensure that your Form 1099 is delivered to the correct recipient.</p>

    <p>We appreciate your prompt response, as it helps us tailor our services to better meet your needs during this tax season. If you have any further questions or require additional assistance, please do not hesitate to reach out to our support team at {{SupportEmailForInstitute}}.</p>
    <p>If you face any technical difficulties in submitting your response, please reach out to us at {{SupportEmailForTechnology}}.</p>
    <p>Thank you for your cooperation, and we look forward to continuing to serve you effectively.</p>

    <p>Warm regards,</p>
 <br/>
{{NameForInstitute}}	
";

        public static string PdfEmailTempForElecAccep = @"
                                <!DOCTYPE html>
                                <html>
                                <body>
                                <strong>From:</strong> {{fromEmail}}
                                <br/>
                                <strong>Sent:</strong> {{sentTime}}
                                <br/>    
                                <strong>To:</strong> {{toEmail}}
                                <br/>
                                <strong>Subject:</strong> {{subject}}
                                <br/><br/>
                                Dear User,<br/><br/>
                                We received your confirmation for 1099 Forms via e-mail as follows:<br/><br/>
                                
                                <table border='1'>
                                    <tr>
                                        <th style='width:20%'>Form</th>
                                     <th style='width:20%'>Selected method of delivery</th>
                                    </tr>
                                  {{tds}}
                                </table>
                                <br/><br/>
                                If it was not you, please contact us at technology@evolvedtax.com immediately.
                                <br/><br/>
                                Thank you for your cooperation in this matter.<br/><br/>
                                Regards,<br/>
                                Technology Team at Evolved LLC
                                </body>
                                </html>";

    }
}