using EvolvedTax.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.DTOs.Response
{
    public class InstituteRequestNameChangeResponse : InstituteRequestNameChange
    {
        public string UserName { get; set; } = string.Empty;
    }
}
