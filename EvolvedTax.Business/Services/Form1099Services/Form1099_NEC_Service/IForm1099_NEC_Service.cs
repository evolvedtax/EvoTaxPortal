using EvolvedTax.Data.Models.DTOs;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.DTOs.Response;
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
    public interface IForm1099_NEC_Service
    {
        Task<MessageResponseModel> Upload1099_NEC_Data(IFormFile file, int InstId, string UserId);
        IQueryable<Tbl1099_NEC> GetRecodByInstId(int InstId);
        public string GeneratePdf(int Id, string TemplatefilePath, string SaveFolderPath);
    }
}
