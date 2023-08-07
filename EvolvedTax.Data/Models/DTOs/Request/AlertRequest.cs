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
    public class AlertRequest
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
  
        public int InstituteID { get; set; }

        public string AlertText { get; set; }

        public DateTime CreatedDate { get; set; }
        public string Title { get; set; }
        public int EntityID { get; set; }

    }
}
