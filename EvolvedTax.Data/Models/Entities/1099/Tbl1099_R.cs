using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EvolvedTax.Data.Models.Entities._1099
{
    public partial class Tbl1099_R
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

        public decimal? Box_1_Amount { get; set; }

        public decimal? Box_2a_Amount { get; set; }

        public string? Box_2b_Checkbox1 { get; set; }

        public string? Box_2b_Checkbox2 { get; set; }

        public decimal? Box_3_Amount { get; set; }

        public decimal? Box_4_Amount { get; set; }

        public decimal? Box_5_Amount { get; set; }

        public decimal? Box_6_Amount { get; set; }

        public string? Box_7_Code { get; set; }

        public string? Box_7_Checkbox { get; set; }

        public decimal? Box_8_Amount { get; set; }

        public int? Box_8_Number { get; set; }

        public int? Box_9a_Number { get; set; }

        public decimal? Box_9b_Amount { get; set; }

        public decimal? Box_10_Amount { get; set; }

        public int? Box_11__Roth_Year { get; set; }

        public string? Box_12_FATCA_Check { get; set; }

        public DateTime? Box_13_DATE { get; set; }

        public decimal? Box_14_Amount { get; set; }

        public string? Box_15_State { get; set; }

        public int? Box_15_IDNumber { get; set; }

        public decimal? Box_16_Amount { get; set; }

        public decimal? Box_17_Amount { get; set; }

        public string? Box_18_Name { get; set; }

        public decimal? Box_19_Amount { get; set; }

        public string? Form_Category { get; set; }

        public string? Form_Source { get; set; }

        public int? Batch_ID { get; set; }

        public string? Tax_State { get; set; }

        public string? Uploaded_File { get; set; }

        public int? Status { get; set; }

        public string? Created_By { get; set; }

        public DateTime? Created_Date { get; set; }

        public string? UserId { get; set; }

        public int? InstID { get; set; }

    }



}
