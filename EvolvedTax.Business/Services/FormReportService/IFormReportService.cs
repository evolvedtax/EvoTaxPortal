using EvolvedTax.Data.Enums;
using EvolvedTax.Data.Models.DTOs;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.DTOs.Response;
using EvolvedTax.Data.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Business.Services.FormReport
{
    public interface IFormReportService
    {
        IQueryable<InstituteClientResponse> GetClientByInstituteId(int InstId, string formType = null,string Entities = null, string Status = null);
    }
}
