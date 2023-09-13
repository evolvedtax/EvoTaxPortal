using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EvolvedTax.Data.Models.Entities._1099
{

    public partial class Tbl1099_NEC
    {
        [Key]
        public int Id { get; set; }
        public string Rcp_TIN { get; set; }
           public int? EntityId { get; set; }

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

        public decimal? Box_1_Amount { get; set; }

        public string? Box_2_Checkbox { get; set; }

        public decimal? Box_4_Amount { get; set; }

        public decimal? Box_5_Amount { get; set; }

        public string? Box_6_IDNumber { get; set; }

        public string? Box_6_State { get; set; }

        public decimal? Box_7_Amount { get; set; }

        public string? OptRcpTextLine1 { get; set; }

        public string? OptRcpTextLine2 { get; set; }

        public string? Form_Category { get; set; }

        public string? Form_Source { get; set; }

        public string? BatchID { get; set; }

        public string? Tax_State { get; set; }

        public string? Uploaded_File { get; set; }

        public int? Status { get; set; }

        public string? Created_By { get; set; }

        public DateTime? Created_Date { get; set; }

        public string? UserId { get; set; }

        public int? InstID { get; set; }

        public string? Corrected { get; set; }
        public bool IsDuplicated { get; set; }

    }




}
