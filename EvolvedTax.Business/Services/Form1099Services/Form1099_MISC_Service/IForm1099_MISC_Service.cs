using EvolvedTax.Data.Models.DTOs;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.DTOs.Response.Form1099;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Data.Models.Entities._1099;
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
        IQueryable<Form1099MISCResponse> GetForm1099MISCList();
        Task<MessageResponseModel> Upload1099_MISC_Data(IFormFile file, int InstId, string UserId);
        string GeneratePdf(string id, string BasePath);
    }
}
