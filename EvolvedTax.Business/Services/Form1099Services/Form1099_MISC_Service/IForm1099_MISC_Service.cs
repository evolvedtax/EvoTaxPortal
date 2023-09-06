using EvolvedTax.Data.Models.DTOs;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Business.Services.Form1099Services
{
    public interface IForm1099_MISC_Service
    {
        Task<MessageResponseModel> Upload1099_MISC_Data(IFormFile file, int InstId, string UserId);
    }
}
