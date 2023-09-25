using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EvolvedTax.Data.Models.Entities._1099
{
    public partial class Tbl1099_K
    {
        [Key]
        public int Id { get; set; }
        public string? Corrected { get; set; }
        public int? EntityId { get; set; }
        public string Rcp_TIN { get; set; }

        public string? Last_Name_Company { get; set; }

        public string? First_Name { get; set; }

        public string? Name_Line2 { get; set; }

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

        public string? Payment_Card_Chkbox { get; set; }

        public string? Third_Party_Chkbox { get; set; }

        public decimal? Box_1a_Amount { get; set; }

        public decimal? Box_1b_Amount { get; set; }

        public int? Box_2_MCC { get; set; }

        public decimal? Box_3_Number { get; set; }

        public decimal? Box_4_Amount { get; set; }

        public decimal? Box_5a_Amount { get; set; }

        public decimal? Box_5b_Amount { get; set; }

        public decimal? Box_5c_Amount { get; set; }

        public decimal? Box_5d_Amount { get; set; }

        public decimal? Box_5e_Amount { get; set; }

        public decimal? Box_5f_Amount { get; set; }

        public decimal? Box_5g_Amount { get; set; }

        public decimal? Box_5h_Amount { get; set; }

        public decimal? Box_5i_Amount { get; set; }

        public decimal? Box_5j_Amount { get; set; }

        public decimal? Box_5k_Amount { get; set; }

        public decimal? Box_5l_Amount { get; set; }

        public string? Box_6_State { get; set; }

        public string? Box_7_IDNumber { get; set; }

        public decimal? Box_8_Amount { get; set; }

        public string? Form_Category { get; set; }

        public string? Form_Source { get; set; }

        public string? Tax_State { get; set; }

        public string? Uploaded_File { get; set; }

        public int? Status { get; set; }

        public string? Created_By { get; set; }

        public DateTime? Created_Date { get; set; }

        public string? UserId { get; set; }

        public int? InstID { get; set; }
        public bool IsDuplicated { get; set; }
        public string? PSE_Checkbox { get; set; }
        public string? Other_3rd_Party_Checkbox { get; set; }
        public string? PSE_Name_Telephone_Number { get; set; }
        public string? Province { get; set; }
        public string? PostalCode { get; set; }

    }



}
