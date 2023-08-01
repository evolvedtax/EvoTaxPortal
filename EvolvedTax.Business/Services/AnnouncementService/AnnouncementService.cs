﻿using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EvolvedTax.Business.Services.AnnouncementService
{
    public class AnnouncementService : IAnnouncementService
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        public AnnouncementService(EvolvedtaxContext evolvedtaxContext)
        {
            _evolvedtaxContext = evolvedtaxContext;
        }

        public List<AnnouncementRequest> GetAnnouncements()
        {
          
            var currentDate = DateTime.Now;
            var announcements = _evolvedtaxContext.Announcements
                .Where(a => a.EndDate >= currentDate)
                   .OrderByDescending(a => a.Id)
                .Select(a => new AnnouncementRequest
                {
                    Id = a.Id,
                    Title = a.Title,
                    Message = a.Message,
                    EndDate = (DateTime)a.EndDate
                })
                .ToList();

            return announcements;
        }
        public Announcement GetAnnouncementByID(int id)
        {

            var announcement = _evolvedtaxContext.Announcements.FirstOrDefault(a => a.Id == id);

            return announcement;
        }



        public void SaveAnnouncement(AnnouncementRequest request)
        {

            var model = new Announcement
            {
                Title = request.Title,
                Message = request.Message,
                EndDate = request.EndDate,

            };

            _evolvedtaxContext.Announcements.Add(model);
            _evolvedtaxContext.SaveChanges();
        }
    }
}
