﻿using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Business.Services.AnnouncementService
{
    public interface IAnnouncementService
    {
        public void SaveAnnouncement(AnnouncementRequest request);
        public List<AnnouncementRequest> GetAnnouncements();
        public Announcement GetAnnouncementByID(int id);
    }
}
