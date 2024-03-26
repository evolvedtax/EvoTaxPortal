using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.DTOs.ViewModels
{
    public class InstituteClientViewModel
    {
        public IQueryable<InstituteClientResponse> InstituteClientsResponse { get; set; } = new List<InstituteClientResponse>().AsQueryable();
        public IQueryable<SharedUsersResponse> SharedUsersResponse { get; set; } = new List<SharedUsersResponse>().AsQueryable();
        public InstituteClientRequest InstituteClientsRequest { get; set; } = new InstituteClientRequest();
        public string ClientUrl { get; set; }
    }
}
