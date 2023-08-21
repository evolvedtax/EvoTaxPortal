using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.DTOs.Request
{
    public class SettingRequest
    {
        public InstituteMasterRequest InstituteMasterRequest { get; set; } = new InstituteMasterRequest();
        public UserManagementRequest UserManagementRequest { get; set; } = new UserManagementRequest();
        public EmailSettingRequest EmailSettingRequest { get; set; } = new EmailSettingRequest();
        public AnnouncementRequest AnnouncementRequest { get; set; } = new AnnouncementRequest();
    }
}
