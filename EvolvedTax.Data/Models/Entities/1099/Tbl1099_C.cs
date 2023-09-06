﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EvolvedTax.Data.Models.Entities._1099
{
    public partial class Tbl1099_C
    {
        [Key]
        public int Id { get; set; }
        public string? Corrected { get; set; }
        public int? EntityId { get; set; }
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

        public DateTime? Box_1_Date { get; set; }

        public decimal? Box_2_Amount { get; set; }

        public decimal? Box_3_Amount { get; set; }

        public string? Box_4_All_Lines { get; set; }

        public string? Box_5_Checkbox { get; set; }

        public string? Box_6_Code { get; set; }

        public decimal? Box_7_Amount { get; set; }

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
