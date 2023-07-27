using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.DTOs.ViewModels
{
    public class InstituteEntityViewModel
    {
        public IQueryable<InstituteEntitiesResponse> InstituteEntitiesResponse { get; set; } = new List<InstituteEntitiesResponse>().AsQueryable();
        public InstituteEntityRequest InstituteEntityRequest { get; set; } = new InstituteEntityRequest();
    }
}
