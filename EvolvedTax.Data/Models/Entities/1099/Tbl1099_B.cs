using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EvolvedTax.Data.Models.Entities._1099
{
    public partial class Tbl1099_B
    {
        [Key]
        public int Id { get; set; }
        public string? Corrected { get; set; }
        public string Rcp_TIN { get; set; }

        public string? Last_Name_Company { get; set; }

        public string? First_Name { get; set; }

        public string? Name_Line_2 { get; set; }

        public string? Address_Type { get; set; }

        public string? Address_Deliv_Street { get; set; }

        public string? Address_Apt_Suite { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Zip { get; set; }

        public string? Country { get; set; }

        public string? Rcp_Account { get; set; }

        public string? Rcp_Email { get; set; }

        public string? Second_TIN_Notice { get; set; }

        public string? FATCA_Checkbox { get; set; }

        public string? CUSIP_No { get; set; }

        public string? Eight949_Code { get; set; }

        public string? Box_1a_Description { get; set; }

        public DateTime? Box_1b_Date { get; set; }

        public DateTime? Box_1c_Date { get; set; }

        public decimal? Box_1d_Amount { get; set; }

        public decimal? Box_1e_Amount { get; set; }

        public decimal? Box_1f_Amount { get; set; }

        public decimal? Box_1g_Amount { get; set; }

        public string? Box_2_Checkbox_1 { get; set; }

        public string? Box_2_Checkbox_2 { get; set; }

        public string? Box_2_Checkbox_3 { get; set; }

        public string? Box_3_Checkbox_1 { get; set; }

        public string? Box_3_Checkbox_2 { get; set; }

        public decimal? Box_4_Amount { get; set; }

        public string? Box_5_Checkbox { get; set; }

        public string? Box_6_Checkbox_1 { get; set; }

        public string? Box_6_Checkbox_2 { get; set; }

        public string? Box_7_Checkbox { get; set; }

        public decimal? Box_8_Amount { get; set; }

        public decimal? Box_9_Amount { get; set; }

        public decimal? Box_10_Amount { get; set; }

        public decimal? Box_11_Amount { get; set; }

        public string? Box_12_Checkbox { get; set; }

        public decimal? Box_13_Amount { get; set; }

        public string? Box_14_State { get; set; }

        public string? Box_15_ID_Number { get; set; }

        public decimal? Box_16_Amount { get; set; }

        public string? Form_Category { get; set; }

        public string? Form_Source { get; set; }

        public string? Tax_State { get; set; }

        public string? Uploaded_File { get; set; }

        public int? Status { get; set; }

        public string? Created_By { get; set; }

        public DateTime? Created_Date { get; set; }

        public string? UserId { get; set; }

        public int? InstID { get; set; }

    }



}
