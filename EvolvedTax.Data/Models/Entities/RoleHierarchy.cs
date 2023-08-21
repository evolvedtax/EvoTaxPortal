using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.Entities
{
    public class RoleHierarchy
    {
        public string RoleName { get; set; }
        public string AllowedRoles { get; set; }
    }
}
