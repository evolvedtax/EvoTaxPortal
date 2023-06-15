using EvolvedTax.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Business.Services.W8EXPFormService
{
    public interface IW8EXPFormService
    {
        public int Save(FormRequest request);
    }
}
