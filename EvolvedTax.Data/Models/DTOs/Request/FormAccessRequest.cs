using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.DTOs.Request
{
    public class FormAccessRequest
    {
        public int Id { get; set; }
        public string[] SelectedFormNames { get; set; }
        public string Form_Name { get; set; }
        public int InstituteID { get; set; }
        public DateTime CreatedDate { get; set; }= DateTime.Now;

    }
}
