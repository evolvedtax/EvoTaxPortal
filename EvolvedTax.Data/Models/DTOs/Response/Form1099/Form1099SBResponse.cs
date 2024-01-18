using EvolvedTax.Data.Models.Entities._1099;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.DTOs.Response.Form1099
{
    public class Form1099SBResponse : Tbl1099_SB
    {
        public int TaxYear { get; set; }
        public string FormType { get; set; }

        //testing
        public string CountryCode { get; set; }
        public string RecipentCity { get; set; }
    }
}
