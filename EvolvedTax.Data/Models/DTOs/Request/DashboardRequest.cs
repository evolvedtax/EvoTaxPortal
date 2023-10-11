using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.DTOs.Request
{
    public class DashboardRequest
    {
        public EmailNotSendCountRequest EmailNotSentCount { get; set; } = new EmailNotSendCountRequest();
        public FormSubmittedCountRequest FormSubmittedCount { get; set; } = new FormSubmittedCountRequest();
        public FormPendingCountRequest FormPendingCount { get; set; } = new FormPendingCountRequest();
    }
    public class EmailNotSendCountRequest
    {
        public int W9_Counts { get; set; }
        public int W8_BEN_Counts { get; set; }
        public int W8_BEN_E_Counts { get; set; }
        public int W8_IMY_Counts { get; set; }
        public int W8_ECI_Counts { get; set; }
        public int W8_EXP_Counts { get; set; }
        public int _1099_A_Counts { get; set; }
        public int _1099_B_Counts { get; set; }
        public int _1099_C_Counts { get; set; }
        public int _1099_CAP_Counts { get; set; }
        public int _1099_G_Counts { get; set; }
        public int _1099_INT_Counts { get; set; }
        public int _1099_R_Counts { get; set; }
        public int _1099_S_Counts { get; set; }
        public int _1099_SA_Counts { get; set; }
        public int _1099_SB_Counts { get; set; }
        public int _1099_DIV_Counts { get; set; }
        public int _1099_K_Counts { get; set; }
        public int _1099_LS_Counts { get; set; }
        public int _1099_MISC_Counts { get; set; }
        public int _1099_OID_Counts { get; set; }
        public int _1099_Q_Counts { get; set; }
        public int _1099_NEC_Counts { get; set; }
        public int _1099_PATR_Counts { get; set; }
        public int _1099_LTC_Counts { get; set; }
    }
    public class FormSubmittedCountRequest
    {
        public int W9_Counts { get; set; }
        public int W8_BEN_Counts { get; set; }
        public int W8_BEN_E_Counts { get; set; }
        public int W8_IMY_Counts { get; set; }
        public int W8_ECI_Counts { get; set; }
        public int W8_EXP_Counts { get; set; }
        public int _1099_A_Counts { get; set; }
        public int _1099_B_Counts { get; set; }
        public int _1099_C_Counts { get; set; }
        public int _1099_CAP_Counts { get; set; }
        public int _1099_G_Counts { get; set; }
        public int _1099_INT_Counts { get; set; }
        public int _1099_R_Counts { get; set; }
        public int _1099_S_Counts { get; set; }
        public int _1099_SA_Counts { get; set; }
        public int _1099_SB_Counts { get; set; }
        public int _1099_DIV_Counts { get; set; }
        public int _1099_K_Counts { get; set; }
        public int _1099_LS_Counts { get; set; }
        public int _1099_MISC_Counts { get; set; }
        public int _1099_OID_Counts { get; set; }
        public int _1099_Q_Counts { get; set; }
        public int _1099_NEC_Counts { get; set; }
        public int _1099_PATR_Counts { get; set; }
        public int _1099_LTC_Counts { get; set; }
    }
    public class FormPendingCountRequest
    {
        public int W9_Counts { get; set; }
        public int W8_BEN_Counts { get; set; }
        public int W8_BEN_E_Counts { get; set; }
        public int W8_IMY_Counts { get; set; }
        public int W8_ECI_Counts { get; set; }
        public int W8_EXP_Counts { get; set; }
        public int _1099_A_Counts { get; set; }
        public int _1099_B_Counts { get; set; }
        public int _1099_C_Counts { get; set; }
        public int _1099_CAP_Counts { get; set; }
        public int _1099_G_Counts { get; set; }
        public int _1099_INT_Counts { get; set; }
        public int _1099_R_Counts { get; set; }
        public int _1099_S_Counts { get; set; }
        public int _1099_SA_Counts { get; set; }
        public int _1099_SB_Counts { get; set; }
        public int _1099_DIV_Counts { get; set; }
        public int _1099_K_Counts { get; set; }
        public int _1099_LS_Counts { get; set; }
        public int _1099_MISC_Counts { get; set; }
        public int _1099_OID_Counts { get; set; }
        public int _1099_Q_Counts { get; set; }
        public int _1099_NEC_Counts { get; set; }
        public int _1099_PATR_Counts { get; set; }
        public int _1099_LTC_Counts { get; set; }
    }
}
