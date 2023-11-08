﻿using EvolvedTax.Data.Models.DTOs.Response;
using EvolvedTax.Data.Models.Entities;
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
        public Tbl1099_ReminderDaysRequest Tbl1099_ReminderDaysRequest { get; set; } = new Tbl1099_ReminderDaysRequest();
        public AnnouncementRequest AnnouncementRequest { get; set; } = new AnnouncementRequest();
        public FormAccessRequest FormAccessRequest { get; set; } = new FormAccessRequest();
        public InstituteRequestNameChange InstituteRequestNameChange { get; set; } = new InstituteRequestNameChange();
        public InstituteEmailTemplate InstituteEmailTemplate { get; set; } = new InstituteEmailTemplate();
        public IQueryable<InstituteRequestNameChangeResponse> InstituteRequestNameChangeResponses { get; set; } = new List<InstituteRequestNameChangeResponse>().AsQueryable();
    }
}
