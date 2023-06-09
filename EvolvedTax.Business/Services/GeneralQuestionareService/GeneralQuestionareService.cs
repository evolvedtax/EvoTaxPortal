using Azure.Core;
using Azure;
using EvolvedTax.Data.Models.Entities;
using iTextSharp.text.pdf;
using System.Diagnostics.Metrics;
using System.Globalization;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Ocsp;
using EvolvedTax.Common.Constants;
using NPOI.OpenXmlFormats.Dml.Diagram;

namespace EvolvedTax.Business.Services.GeneralQuestionareService
{
    public class GeneralQuestionareService : IGeneralQuestionareService
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        public GeneralQuestionareService(EvolvedtaxContext evolvedtaxContext)
        {
            _evolvedtaxContext = evolvedtaxContext;
        }

        public FormRequest? GetDataByClientEmail(string ClientEmail)
        {
            return _evolvedtaxContext.GeneralQuestionIndividuals.Where(p => p.UserName == ClientEmail).Select(p => new FormRequest
            {
                GQFirstName = p.FirstName ?? string.Empty,
                GQLastName = p.LastName ?? string.Empty,
                GQCountry = p.CountryofCitizenship ?? string.Empty,
                MAddress1 = p.MailingAddress1 ?? string.Empty,
                MAddress2 = p.MailingAddress2 ?? string.Empty,
                MCity = p.MailingCity ?? string.Empty,
                MCountry = p.MailingCountry ?? string.Empty,
                MProvince = p.MailingProvince ?? string.Empty,
                MState = p.MailingState ?? string.Empty,
                MZipCode = p.MailingZip ?? string.Empty,
                PAddress1 = p.PermanentAddress1 ?? string.Empty,
                PAddress2 = p.PermanentAddress2 ?? string.Empty,
                PCity = p.PermanentCity ?? string.Empty,
                PCountry = p.PermanentCountry ?? string.Empty,
                PProvince = p.PermanentProvince ?? string.Empty,
                PState = p.PermanentState ?? string.Empty,
                PZipCode = p.PermanentZip ?? string.Empty,
                TypeofTaxNumber = p.TypeofTaxNumber ?? string.Empty,
                UserName = p.UserName ?? string.Empty,
                USCitizen = p.Uscitizen ?? string.Empty,
                US1 = p.Us1 ?? string.Empty,
            }).FirstOrDefault();
        }

        public int Save(FormRequest request)
        {
            var model = new GeneralQuestionIndividual
            {
                FirstName = request.GQFirstName,
                LastName = request.GQLastName,
                CountryofCitizenship = request.GQCountry,
                MailingAddress1 = request.MAddress1,
                MailingAddress2 = request.MAddress2,
                MailingCity = request.MCity,
                MailingCountry = request.MCountry,
                MailingProvince = request.MProvince,
                MailingState = request.MState,
                MailingZip = request.MZipCode,
                PermanentAddress1 = request.PAddress1,
                PermanentAddress2 = request.PAddress2,
                PermanentCity = request.PCity,
                PermanentCountry = request.PCountry,
                PermanentProvince = request.PProvince,
                PermanentState = request.PState,
                PermanentZip = request.PZipCode,
                TypeofTaxNumber = request.TypeofTaxNumber,
                UserName = request.UserName,
                Uscitizen = request.USCitizen,
                Us1 = request.US1
            };

            _evolvedtaxContext.GeneralQuestionIndividuals.Add(model);
            _evolvedtaxContext.SaveChanges();
            return model.Id;
        }
        public int Update(FormRequest model)
        {
            var response = _evolvedtaxContext.GeneralQuestionIndividuals.FirstOrDefault(p => p.UserName == model.EmailId);
            response.FirstName = model.GQFirstName;
            response.LastName = model.GQLastName;
            response.CountryofCitizenship = model.GQCountry;
            response.MailingAddress1 = model.MAddress1;
            response.MailingAddress2 = model.MAddress2;
            response.MailingCity = model.MCity;
            response.MailingCountry = model.MCountry;
            response.MailingProvince = model.MProvince;
            response.MailingState = model.MState;
            response.MailingZip = model.MZipCode;
            response.PermanentAddress1 = model.PAddress1;
            response.PermanentAddress2 = model.PAddress2;
            response.PermanentCity = model.PCity;
            response.PermanentCountry = model.PCountry;
            response.PermanentProvince = model.PProvince;
            response.PermanentState = model.PState;
            response.PermanentZip = model.PZipCode;
            response.TypeofTaxNumber = model.TypeofTaxNumber;
            response.Uscitizen = model.USCitizen;
            response.Us1 = model.US1;
            _evolvedtaxContext.Update(response);
            return _evolvedtaxContext.SaveChanges();
        }
        public bool IsClientAlreadyExist(string ClientEmailId)
        {
            return _evolvedtaxContext.GeneralQuestionIndividuals.Any(p=>p.UserName == ClientEmailId);
        }
    }
}
