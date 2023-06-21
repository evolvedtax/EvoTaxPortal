using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Business.Services.CommonService
{
    public interface ICommonService
    {
        string AssignSignature(PdfFormDetailsRequest request, string filePath);
        public string RemoveAnnotations(string filePath);
        MemoryStream DownloadFile(string filePath);

    }
}
