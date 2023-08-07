using EvolvedTax.Data.Models.DTOs.Response;
using EvolvedTax.Data.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.DTOs.Request
{
    public class UserManagementRequest
    {
        public IQueryable<User> Users { get; set; } = new List<User>().AsQueryable();
        public List<IdentityRole> Roles { get; set; } = new List<IdentityRole>();
        public List<InstituteEntitiesResponse> InstituteEntities { get; set; } = new List<InstituteEntitiesResponse>();
        public List<InvitationEmailDetalsRequest> InvitationEmailDetails { get; set; } = new List<InvitationEmailDetalsRequest>();
    }
    public class InvitationEmailDetalsRequest
    {
        //[Remote("ValidateEmailDomainAddress", "UserManagement", AdditionalFields = "InvitaionEmail")]
        public string InvitaionEmail { get; set; } = string.Empty;
    }
}
