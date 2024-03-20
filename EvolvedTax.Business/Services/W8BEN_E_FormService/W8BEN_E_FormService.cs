using Azure.Core;
using Azure;
using EvolvedTax.Data.Models.Entities;
using iTextSharp.text.pdf;
using System.Diagnostics.Metrics;
using System.Globalization;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using EvolvedTax.Business.Services.W8BenFormService;
using SkiaSharp;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Business.Services.GeneralQuestionareService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Business.Services.GeneralQuestionareEntityService;
using AutoMapper;
using iTextSharp.text.pdf.qrcode;

namespace EvolvedTax.Business.Services.W8BEN_E_FormService
{
    public class W8BEN_E_FormService : IW8BEN_E_FormService
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly IGeneralQuestionareService _generalQuestionareService;
        readonly IGeneralQuestionareEntityService _generalQuestionareEntityService;
        readonly IMapper _mapper;
        public W8BEN_E_FormService(EvolvedtaxContext evolvedtaxContext, IGeneralQuestionareService generalQuestionareService, IGeneralQuestionareEntityService generalQuestionareEntityService, IMapper mapper)
        {
            _generalQuestionareService = generalQuestionareService;
            _evolvedtaxContext = evolvedtaxContext;
            _generalQuestionareEntityService = generalQuestionareEntityService;
            _mapper = mapper;
        }

        public string SaveForEntity(W8BENERequest request)
        {
            var model = _mapper.Map<TblW8ebeneform>(request);
            model.City = string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode);
            model.MailingAddress = string.Concat(request.MAddress1, " ", request.MAddress2);
            model.MCity = string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode);
            model.PermanentResidenceAddress = string.Concat(request.PAddress1, " ", request.PAddress2);
            model.SignatureDateMmDdYyyy = DateTime.Now.ToString("MM-dd-yyyy");
            model.W8beneemailAddress = request.EmailId;
            model.IsActive = true;
            //var model = new TblW8ebeneform
            //{
            //    NameOfOrganization = request.NameOfOrganization,
            //    CountryOfIncorporation = request.CountryOfIncorporation,
            //    City = string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode),
            //    NameOfDiregardedEntity = request.NameOfDiregardedEntity,
            //    TypeOfEntity = request.TypeOfEntity,
            //    FtinCheck = request.FtinCheck,
            //    Country = request.PCountry,
            //    ForeignTaxIdentifyingNumber = request.ForeignTaxIdentifyingNumber,
            //    FatcaStatus = request.FatcaStatus,
            //};
            if (_evolvedtaxContext.TblW8ebeneforms.Any(p => p.W8beneemailAddress == request.EmailId))
            {
                return UpdateForEntity(request);
            }
            _evolvedtaxContext.TblW8ebeneforms.Add(model);
            _evolvedtaxContext.SaveChanges();
            request.Id = model.Id;

            if (request.IsPartialSave)
            {
                return AppConstants.FormPartiallySave;
            }
            return W8BENECreationForEntity(request);
        }
        protected static string W8BENECreationForEntity(W8BENERequest request)
        {
            string templatefile = request.TemplateFilePath;
            string fileName = string.Concat(request.NameOfIndividual?.Replace(" ", "_"), "_", "Form_", AppConstants.W8BENEForm, "_", request.Id, "_temp.pdf");
            string newFile = Path.Combine(request.BasePath, fileName);
            PdfReader pdfReader = new PdfReader(templatefile);
            int numberOfPages = pdfReader.NumberOfPages;
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;
            string s1 = string.Empty;
            string s2 = string.Empty;
            string s3 = string.Empty;

            if (request._15cTb4?.Length >= 256)
            {
                s1 = request._15cTb4.Substring(0, 24);
                s2 = request._15cTb4.Substring(24, 116);
                s3 = request._15cTb4.Substring(24 + 116, 116);
            }
            else if (request._15cTb4?.Length >= 24)
            {
                s1 = request._15cTb4.Substring(0, 24);
                if (request._15cTb4?.Length >= 140)
                {
                    s2 = request._15cTb4.Substring(24, 116);
                    s3 = request._15cTb4.Substring(24 + 116, 116);
                }
                else
                {
                    s2 = request._15cTb4 != null ? request._15cTb4.Substring(24) : "";
                }
            }
            else
            {
                s1 = request._15cTb4 != null ? request._15cTb4 : "";
            }
            // set form pdfFormFields  
            // The first worksheet and W8-BEN-E form

            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_1[0]", request.NameOfOrganization);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_2[0]", request.CountryOfIncorporation);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_3[0]", request.NameOfDiregardedEntity);
            if (request.TypeOfEntity == "1")
            {
                //     pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                //      pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");

            }
            else if (request.TypeOfEntity == "2")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                //     pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
            }
            else if (request.TypeOfEntity == "3")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                //     pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
            }
            else if (request.TypeOfEntity == "4")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                //     pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                //      pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
            }
            else if (request.TypeOfEntity == "5")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                //     pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                //      pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
            }
            else if (request.TypeOfEntity == "6")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                //    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                //      pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
            }
            else if (request.TypeOfEntity == "7")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                //    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                //      pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
            }
            else if (request.TypeOfEntity == "8")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                //    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                //      pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
            }
            else if (request.TypeOfEntity == "9")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                //   pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                //      pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
            }
            else if (request.TypeOfEntity == "10")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                //   pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                //      pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
            }
            else if (request.TypeOfEntity == "11")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                //   pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
            }
            else if (request.TypeOfEntity == "12")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                // pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
            }
            else if (request.TypeOfEntity == "13")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                // pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                //      pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
            }
            if (request.FatcaStatus == "1")
            {
                //  pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "2")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                //    pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "3")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                //    pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "4")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                //        pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "5")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                //         pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "6")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                //        pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "7")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                //       pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "8")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                //       pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "9")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                //          pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "10")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                //       pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "11")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                //        pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "12")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                //        pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "13")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                //       pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "14")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                //        pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");
            }
            else if (request.FatcaStatus == "15")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                //        pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "16")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                //         pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "17")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                //        pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "18")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                //            pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "19")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                //         pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "20")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                //         pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "21")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                //         pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "22")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                //         pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "23")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                //         pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "24")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                //        pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "25")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                //         pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "26")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                //        pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "27")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                //        pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "28")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                //        pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "29")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                //         pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "30")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                //        pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "31")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                //        pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            else if (request.FatcaStatus == "32")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col1[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[12]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[13]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[14]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[15]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[16]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[17]", "0");
                //        pdfFormFields.SetField("topmostSubform[0].Page1[0].Col2[0].c1_3[18]", "0");

            }
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_4[0]", request.PermanentResidenceAddress);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_5[0]", request.City);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_6[0]", request.Country);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_7[0]", request.MailingAddress);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_8[0]", request.MCity);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_9[0]", request.MCountry);
            pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_1[0]", request.UsTin);
            pdfFormFields.SetField("topmostSubform[0].Page2[0].Line9a_ReadOrder[0].f2_2[0]", request.GIN);
            pdfFormFields.SetField("topmostSubform[0].Page2[0].Line9b_ReadOrder[0].f2_3[0]", request.ForeignTaxIdentifyingNumber);
            if (request.FtinCheck != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_4[0]", request.ReferenceNumberS);
            if (request._11fatcaCb == "1")
            {
                //pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesCol1_ReadOrder[0].c2_2[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesCol1_ReadOrder[0].c2_2[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesCol2_ReadOrder[0].c2_2[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesCol2_ReadOrder[0].c2_2[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_2[0]", "0");
            }
            else if (request._11fatcaCb == "2")
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesCol1_ReadOrder[0].c2_2[0]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesCol1_ReadOrder[0].c2_2[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesCol2_ReadOrder[0].c2_2[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesCol2_ReadOrder[0].c2_2[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_2[0]", "0");
            }
            else if (request._11fatcaCb == "3")
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesCol1_ReadOrder[0].c2_2[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesCol1_ReadOrder[0].c2_2[1]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesCol2_ReadOrder[0].c2_2[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesCol2_ReadOrder[0].c2_2[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_2[0]", "0");
            }
            else if (request._11fatcaCb == "4")
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesCol1_ReadOrder[0].c2_2[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesCol1_ReadOrder[0].c2_2[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesCol2_ReadOrder[0].c2_2[0]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesCol2_ReadOrder[0].c2_2[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_2[0]", "0");
            }
            else if (request._11fatcaCb == "5")
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesCol1_ReadOrder[0].c2_2[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesCol1_ReadOrder[0].c2_2[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesCol2_ReadOrder[0].c2_2[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesCol2_ReadOrder[0].c2_2[1]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_2[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_5[0]", request._12mailingAddress);
            pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_6[0]", string.Concat(request._12City, ", ", request._12State ?? request._12Province, ", ", request._12ZipPostCode));
            pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_7[0]", request._12Country);
            pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_8[0]", request._13gin);
            #region Part_3
            if (string.IsNullOrEmpty(request._14aTb))
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_3[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_9[0]", request._14aTb);
            //if (GlobalVariables.Globals.a14b1 == "1")
            //{
            //    //                            pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_4[0]", "0");
            //    //                          pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[5]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_10[0]", "");
            //}
            //else if (GlobalVariables.Globals.a14b1 == "2")
            //{
            //    //                        pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_4[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[0]", "0");
            //    //                      pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[5]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_10[0]", "");
            //}
            //else if (GlobalVariables.Globals.a14b1 == "3")
            //{
            //    //              pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_4[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[1]", "0");
            //    //                pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[5]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_10[0]", "");
            //}
            //else if (GlobalVariables.Globals.a14b1 == "4")
            //{
            //    //                 pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_4[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[2]", "0");
            //    //               pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[5]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_10[0]", "");
            //}
            //else if (GlobalVariables.Globals.a14b1 == "5")
            //{
            //    //                     pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_4[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[3]", "0");
            //    //                   pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[5]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_10[0]", "");
            //}
            //else if (GlobalVariables.Globals.a14b1 == "6")
            //{
            //    //                         pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_4[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[4]", "0");
            //    //                       pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[5]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_10[0]", "");
            //}
            //else if (GlobalVariables.Globals.a14b1 == "7")
            //{
            //    //                         pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_4[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[0]", "0");
            //    //pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[5]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_10[0]", "");
            //}
            //else if (GlobalVariables.Globals.a14b1 == "8")
            //{
            //    //                         pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_4[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[1]", "0");
            //    //pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[5]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_10[0]", "");
            //}
            //else if (GlobalVariables.Globals.a14b1 == "9")
            //{
            //    //                         pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_4[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[2]", "0");
            //    //pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[5]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_10[0]", "");
            //}
            //else if (GlobalVariables.Globals.a14b1 == "10")
            //{
            //    //                         pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_4[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[3]", "0");
            //    //pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[5]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_10[0]", "");
            //}
            //else if (GlobalVariables.Globals.a14b1.Substring(0, 2) == "11")
            //{
            //    //                         pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_4[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[4]", "0");
            //    //pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[5]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_10[0]", GlobalVariables.Globals.a14b1.Substring(2, GlobalVariables.Globals.a14b1.Length - 2));
            //}
            //else
            //{
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_4[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].CheckboxesLine14b_ReadOrder[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[0]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[1]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[2]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[3]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[4]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[5]", "0");
            //    pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_10[0]", "");
            //}

            if (!request._14cCb)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_6[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_11[0]", request._15cTb1);
            pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_12[0]", request._15cTb2);
            pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_13[0]", request._15cTb3);
            pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_14[0]", s1);
            pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_15[0]", s2);
            pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_16[0]", s3);
            #endregion
            #region Part_4

            pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_17[0]", request._16tb);
            if (!request._17cb1)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_7[0]", "0");
            }
            if (!request._17cb2)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_8[0]", "0");
            }
            #endregion
            #region Part_5

            if (!request._18cb)
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_1[0]", "0");
            }
            #endregion
            #region Part_6

            if (!request._19cb) { pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_2[0]", "0"); }
            #endregion
            #region Part_7
            pdfFormFields.SetField("topmostSubform[0].Page3[0].f3_1[0]", request._20tb);
            if (!request._21cb) { pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_3[0]", "0"); }
            #endregion
            #region Part_8
            if (!request._22cb) { pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_4[0]", "0"); }
            #endregion
            #region Part_9
            if (!request._23cb) { pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_5[0]", "0"); }
            #endregion
            #region Part_10
            if (!request._24aCb) { pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_6[0]", "0"); }
            if (request._24borcCb != "1")
            {
                pdfFormFields.SetField("topmostSubform[0].Page4[0].c4_1[0]", "0");
            }
            else if (request._24borcCb != "2")
            {
                pdfFormFields.SetField("topmostSubform[0].Page4[0].c4_1[1]", "0");
            }
            if (!request._24dCb) { pdfFormFields.SetField("topmostSubform[0].Page4[0].c4_2[0]", "0"); }
            #endregion
            #region Part_11
            if (!request._25aCb) { pdfFormFields.SetField("topmostSubform[0].Page4[0].c4_3[0]", "0"); }
            if (request._25bcCb != "1")
            {
                pdfFormFields.SetField("topmostSubform[0].Page4[0].c4_4[0]", "0");
            }
            else if (request._25bcCb != "2")
            {
                pdfFormFields.SetField("topmostSubform[0].Page4[0].c4_4[1]", "0");
            }
            #endregion
            #region Part_12
            if (!request._26cb1) { pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_1[0]", "0"); }

            pdfFormFields.SetField("topmostSubform[0].Page5[0].BulletedList1[0].Bullet1[0].f5_01[0]", request._26tb1);
            if (request._26cb2or3 == "1")
            {
                //  pdfFormFields.SetField("topmostSubform[0].Page5[0].BulletedList1[0].Bullet1[0].c5_2[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page5[0].BulletedList1[0].Bullet1[0].c5_2[1]", "0");
            }
            else if (request._26cb2or3 == "2")
            {
                pdfFormFields.SetField("topmostSubform[0].Page5[0].BulletedList1[0].Bullet1[0].c5_2[0]", "0");
                //   pdfFormFields.SetField("topmostSubform[0].Page5[0].BulletedList1[0].Bullet1[0].c5_2[1]", "0");
            }
            else
            {
                pdfFormFields.SetField("topmostSubform[0].Page5[0].BulletedList1[0].Bullet1[0].c5_2[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page5[0].BulletedList1[0].Bullet1[0].c5_2[1]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].Page5[0].BulletedList1[0].Bullet1[0].f5_2[0]", request._26tb2);
            pdfFormFields.SetField("topmostSubform[0].Page5[0].BulletedList1[0].Bullet2[0].f5_03[0]", request._26tb3);
            if (request._26cb4or5 == "1")
            {
                //    pdfFormFields.SetField("topmostSubform[0].Page5[0].BulletedList1[0].Bullet2[0].c5_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page5[0].BulletedList1[0].Bullet2[0].c5_3[1]", "0");
            }
            else if (request._26cb4or5 == "2")
            {
                pdfFormFields.SetField("topmostSubform[0].Page5[0].BulletedList1[0].Bullet2[0].c5_3[0]", "0");
                //    pdfFormFields.SetField("topmostSubform[0].Page5[0].BulletedList1[0].Bullet2[0].c5_3[1]", "0");
            }
            else
            {
                pdfFormFields.SetField("topmostSubform[0].Page5[0].BulletedList1[0].Bullet2[0].c5_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page5[0].BulletedList1[0].Bullet2[0].c5_3[1]", "0");
            }
            #endregion
            #region Part_13
            if (!request._27cb) { pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_4[0]", "0"); }
            #endregion
            #region Part_14
            if (request._28aorbCb != "1")
            {
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_5[0]", "0");
            }
            else if (request._28aorbCb != "2")
            {
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_5[1]", "0");
            }
            #endregion
            #region Part_15
            if (request._29cb == "1")
            {
                //pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_6[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_6[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_6[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c5_6[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c5_6[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c5_6[2]", "0");
            }
            else if (request._29cb == "2")
            {
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_6[0]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_6[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_6[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c5_6[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c5_6[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c5_6[2]", "0");
            }
            else if (request._29cb == "3")
            {
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_6[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_6[1]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_6[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c5_6[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c5_6[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c5_6[2]", "0");
            }
            else if (request._29cb == "4")
            {
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_6[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_6[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_6[2]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page6[0].c5_6[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c5_6[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c5_6[2]", "0");
            }
            else if (request._29cb == "5")
            {
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_6[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_6[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_6[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c5_6[0]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page6[0].c5_6[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c5_6[2]", "0");
            }
            else if (request._29cb == "6")
            {
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_6[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_6[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_6[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c5_6[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c5_6[1]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page6[0].c5_6[2]", "0");
            }
            #endregion
            #region Part_16
            if (!request._30cb) { pdfFormFields.SetField("topmostSubform[0].Page6[0].c6_1[0]", "0"); }
            #endregion
            #region Part_17
            if (!request._31cb) { pdfFormFields.SetField("topmostSubform[0].Page6[0].c6_2[0]", "0"); }
            #endregion
            #region Part_18
            if (!request._32cb) { pdfFormFields.SetField("topmostSubform[0].Page6[0].c6_3[0]", "1"); }
            #endregion
            #region Part_19
            pdfFormFields.SetField("topmostSubform[0].Page6[0].BulletList4[0].Bullet1[0].f6_1[0]", request._33tb);
            if (string.IsNullOrEmpty(request._33tb?.Trim()))
            {
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c6_4[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page6[0].BulletList4[0].Bullet1[0].f6_1[0]", "");
            }
            #endregion
            #region Part_20
            pdfFormFields.SetField("topmostSubform[0].Page6[0].BulletList5[0].Bullet1[0].f6_2[0]", request._34tb);
            if (string.IsNullOrEmpty(request._34tb?.Trim()))
            {
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c6_5[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page6[0].BulletList5[0].Bullet1[0].f6_2[0]", "");
            }
            #endregion
            #region Part_21
            pdfFormFields.SetField("topmostSubform[0].Page7[0].BulletList1[0].Bullet1[0].f7_1[0]", request._35tb);
            if (string.IsNullOrEmpty(request._35tb?.Trim()))
            {
                pdfFormFields.SetField("topmostSubform[0].Page7[0].c7_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page7[0].BulletList1[0].Bullet1[0].f7_1[0]", "");
            }
            #endregion
            #region Part_22
            if (!request._36cb)
            {
                pdfFormFields.SetField("topmostSubform[0].Page7[0].c7_2[0]", "0");
            }
            #endregion
            #region Part_23
            pdfFormFields.SetField("topmostSubform[0].Page7[0].BulletList3[0].Bullet2[0].f7_2[0]", request._37aTb);

            pdfFormFields.SetField("topmostSubform[0].Page7[0].BulletList4[0].Bullet3[0].f7_3[0]", request._37bTb1);
            pdfFormFields.SetField("topmostSubform[0].Page7[0].BulletList4[0].Bullet4[0].f7_4[0]", request._37bTb2);

            if (string.IsNullOrEmpty(request._37aTb?.Trim()))
            {
                pdfFormFields.SetField("topmostSubform[0].Page7[0].c7_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page7[0].BulletList3[0].Bullet2[0].f7_2[0]", "");
            }
            else if (string.IsNullOrEmpty(request._37bTb1?.Trim()) || string.IsNullOrEmpty(request._37bTb2?.Trim()))
            {
                pdfFormFields.SetField("topmostSubform[0].Page7[0].c7_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page7[0].BulletList4[0].Bullet3[0].f7_3[0]", "");
                pdfFormFields.SetField("topmostSubform[0].Page7[0].BulletList4[0].Bullet4[0].f7_4[0]", "");
            }
            #endregion
            #region Part_24
            if (!request._38cb)
            {
                pdfFormFields.SetField("topmostSubform[0].Page7[0].c7_4[0]", "0");
            }
            #endregion
            #region Part_25
            if (!request._39cb)
            {
                pdfFormFields.SetField("topmostSubform[0].Page7[0].c7_5[0]", "0");
            }
            #endregion
            #region Part_26
            if (!request._40aCb)
            {
                pdfFormFields.SetField("topmostSubform[0].Page7[0].c7_6[0]", "0");
            }
            if (request._40borcCb != "1")
            {
                pdfFormFields.SetField("topmostSubform[0].Page7[0].c7_7[0]", "0");
            }
            if (request._40borcCb != "2")
            {
                pdfFormFields.SetField("topmostSubform[0].Page7[0].c7_7[1]", "0");
            }
            #endregion
            #region Part_27
            if (!request._41cb)
            {
                pdfFormFields.SetField("topmostSubform[0].Page8[0].c8_1[0]", "0");
            }
            #endregion
            #region Part_28
            pdfFormFields.SetField("topmostSubform[0].Page8[0].f8_1[0]", request._42tb);
            if (!request._43cb) { pdfFormFields.SetField("topmostSubform[0].Page8[0].c8_2[0]", "0"); }
            #endregion
            #region Part_29
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow1[0].f8_3[0]", request.NameRow1 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow1[0].f8_4[0]", request.AddressRow1 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow1[0].f8_5[0]", request.Tinrow1 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow2[0].f8_6[0]", request.NameRow2 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow2[0].f8_7[0]", request.AddressRow2 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow2[0].f8_8[0]", request.Tinrow2 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow3[0].f8_9[0]", request.NameRow3 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow3[0].f8_10[0]", request.AddressRow3 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow3[0].f8_11[0]", request.Tinrow3 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow4[0].f8_12[0]", request.NameRow4 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow4[0].f8_13[0]", request.AddressRow4 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow4[0].f8_14[0]", request.Tinrow4 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow5[0].f8_15[0]", request.NameRow5 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow5[0].f8_16[0]", request.AddressRow5 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow5[0].f8_17[0]", request.Tinrow5 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow6[0].f8_18[0]", request.NameRow6 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow6[0].f8_19[0]", request.AddressRow6 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow6[0].f8_20[0]", request.Tinrow6 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow7[0].f8_21[0]", request.NameRow7 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow7[0].f8_22[0]", request.AddressRow7 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow7[0].f8_23[0]", request.Tin1row7 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow8[0].f8_24[0]", request.NameRow8 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow8[0].f8_25[0]", request.AddressRow8 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow8[0].f8_26[0]", request.Tinrow8 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow9[0].f8_27[0]", request.NameRow9 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow9[0].f8_28[0]", request.AddressRow9 ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page8[0].Table_Part29[0].BodyRow9[0].f8_29[0]", request.Tinrow9 ?? "");
            #endregion
            //       pdfFormFields.SetField("topmostSubform[0].Page8[0].c8_3[0]", "1");
            //pdfFormFields.SetField("topmostSubform[0].Page8[0].f8_30[0]", GlobalVariables.Globals.Printnameofsigner1.Replace("&nbsp;", ""));
            pdfFormFields.SetField("topmostSubform[0].Page8[0].f8_31[0]", request.PrintNameOfSigner);
            //pdfFormFields.SetField("topmostSubform[0].Page8[0].f8_32[0]", GlobalVariables.Globals.SignatureDate1);

            PdfContentByte overContent = pdfStamper.GetOverContent(numberOfPages);
            iTextSharp.text.Rectangle rectangle = new iTextSharp.text.Rectangle(100, 110, 250, 130, 0);
            //rectangle.BackgroundColor = BaseColor.LIGHT_GRAY;
            overContent.Rectangle(rectangle);

            //for date
            PdfContentByte overContent1 = pdfStamper.GetOverContent(numberOfPages);
            iTextSharp.text.Rectangle rectangle1 = new iTextSharp.text.Rectangle(465, 110, 610, 130, 0);
            rectangle1.BackgroundColor = BaseColor.LIGHT_GRAY;
            overContent1.Rectangle(rectangle1);

            // For pasting image of signature
            var src1 = Path.Combine(Directory.GetCurrentDirectory(), "signature-image.png");
            iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance(src1);
            PdfImage stream1 = new PdfImage(image1, "", null);
            stream1.Put(new PdfName("ITXT_SpecialId"), new PdfName("123456789"));
            PdfIndirectObject ref1 = pdfStamper.Writer.AddToBody(stream1);
            image1.SetAbsolutePosition(100, 110);
            PdfContentByte over1 = pdfStamper.GetOverContent(numberOfPages);
            over1.AddImage(image1);


            #region Pasting Date Picture
            // Load the image file using SkiaSharp
            using (SKBitmap bitmap = SKBitmap.Decode(Path.Combine(Directory.GetCurrentDirectory(), "pictureText.bmp")))
            {
                using (SKCanvas canvas = new SKCanvas(bitmap))
                {
                    using (SKPaint paint = new SKPaint())
                    {
                        paint.Color = SKColors.Black;
                        paint.TextSize = 9;

                        string text = DateTime.Now.ToString("MM-dd-yyyy");
                        paint.Typeface = SKTypeface.FromFamilyName("Times New Roman");
                        paint.IsAntialias = true; // Enable anti-aliasing
                        SKRect textBounds = new SKRect();
                        paint.MeasureText(text, ref textBounds);

                        // Calculate the position to center the text horizontally and align it vertically in the middle
                        float x = (bitmap.Width - textBounds.Width) / 4;
                        float y = (bitmap.Height - textBounds.Height) / 2 + textBounds.Height;

                        // Draw the text at the calculated position
                        canvas.DrawText(text, x, y, paint);
                    }
                }

                // Save the modified image as PNG
                using (SKData encoded = bitmap.Encode(SKEncodedImageFormat.Png, 100))
                {
                    string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "date2.png");
                    File.WriteAllBytes(outputPath, encoded.ToArray());
                }
            }
            // For pasting date as image
            var src2 = Path.Combine(Directory.GetCurrentDirectory(), "date2.png");
            iTextSharp.text.Image image2 = iTextSharp.text.Image.GetInstance(src2);
            PdfImage stream2 = new PdfImage(image2, "", null);
            stream2.Put(new PdfName("ITXT_SpecialId"), new PdfName("1234567"));
            PdfIndirectObject ref2 = pdfStamper.Writer.AddToBody(stream2);
            image2.SetAbsolutePosition(465, 110);
            PdfContentByte over2 = pdfStamper.GetOverContent(numberOfPages);
            over2.AddImage(image2);
            #endregion
            PdfAnnotation annotation;
            annotation = PdfAnnotation.CreateLink(pdfStamper.Writer, rectangle, PdfAnnotation.HIGHLIGHT_INVERT, new PdfAction(Path.Combine(request.Host, "Certification", "Index")));
            pdfStamper.AddAnnotation(annotation, numberOfPages);

            PdfAnnotation annotation1;
            bool IsDate = true;
            string methodName = "Index?IsDate=" + IsDate.ToString();
            annotation1 = PdfAnnotation.CreateLink(pdfStamper.Writer, rectangle1, PdfAnnotation.HIGHLIGHT_INVERT, new PdfAction(Path.Combine(request.Host, "Certification", methodName)));
            pdfStamper.AddAnnotation(annotation1, numberOfPages);


            pdfStamper.Close();
            pdfReader.Close();
            //string sTmp = "W8-BEN-E Completed for " + GlobalVariables.Globals.Printnameofsigner1 + " " + GlobalVariables.Globals.NameofOrganization1;
            //// flatten the form To remove editting options, set it to false  
            //// to leave the form open to subsequent manual edits  
            //pdfStamper.FormFlattening = true;
            //// close the pdf  
            return fileName;
        }
        public async Task<bool> UpdateByClientEmailId(string ClientEmail, PdfFormDetailsRequest request)
        {
            var response = _evolvedtaxContext.TblW8ebeneforms.Where(p => p.W8beneemailAddress == ClientEmail).FirstOrDefault();
            if (response != null)
            {
                response.UploadedFile = request.FileName;
                response.W8beneentryDate = request.EntryDate?.ToString("MM-dd-yyyy");
                response.Status = "3";
                response.W8benefontName = request.FontFamily;
                response.PrintNameOfSigner = request.PrintName;
                response.W8beneprintSize = request.FontSize;
                _evolvedtaxContext.TblW8ebeneforms.Update(response);
                await _evolvedtaxContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public W8BENERequest GetEntityDataByClientEmailId(string ClientEmailId)
        {
            var gQuestionData = _generalQuestionareEntityService.GetDataByClientEmail(ClientEmailId);
            var w8BENEData = _evolvedtaxContext.TblW8ebeneforms.FirstOrDefault(p => p.W8beneemailAddress == ClientEmailId);
            var model = _mapper.Map<W8BENERequest>(gQuestionData);

            if (w8BENEData != null)
            {
                model.NameOfDiregardedEntity = w8BENEData.NameOfDiregardedEntity;
                model.W8beneemailAddress = ClientEmailId;
                model.AddressRow1 = w8BENEData.AddressRow1;
                model.AddressRow2 = w8BENEData.AddressRow2;
                model.AddressRow3 = w8BENEData.AddressRow3;
                model.AddressRow4 = w8BENEData.AddressRow4;
                model.AddressRow5 = w8BENEData.AddressRow5;
                model.AddressRow6 = w8BENEData.AddressRow6;
                model.AddressRow7 = w8BENEData.AddressRow7;
                model.AddressRow8 = w8BENEData.AddressRow8;
                model.AddressRow9 = w8BENEData.AddressRow9;
                model.FatcaStatus = w8BENEData.FatcaStatus ?? "";
                model.FtinCheck = w8BENEData.FtinCheck ?? false;
                model.Gin = w8BENEData.Gin;
                model.CountryOfIncorporation = w8BENEData.CountryOfIncorporation;
                model.TypeOfEntity = w8BENEData.TypeOfEntity;
                model.UsTin = w8BENEData.UsTin;
                model.ForeignTaxIdentifyingNumber = w8BENEData.ForeignTaxIdentifyingNumber;
                model.ReferenceNumberS = w8BENEData.ReferenceNumberS;
                model._11fatcaCb = w8BENEData._11fatcaCb;
                model.NameRow1 = w8BENEData.NameRow1;
                model.NameRow2 = w8BENEData.NameRow2;
                model.NameRow3 = w8BENEData.NameRow3;
                model.NameRow4 = w8BENEData.NameRow4;
                model.NameRow5 = w8BENEData.NameRow5;
                model.NameRow6 = w8BENEData.NameRow6;
                model.NameRow7 = w8BENEData.NameRow7;
                model.NameRow8 = w8BENEData.NameRow8;
                model.NameRow9 = w8BENEData.NameRow9;
                model.PrintNameOfSigner = w8BENEData.PrintNameOfSigner;
                model.SignatureDateMmDdYyyy = w8BENEData.SignatureDateMmDdYyyy;
                model.NameOfOrganization = w8BENEData.NameOfOrganization;
                model.Tin1row7 = w8BENEData.Tin1row7;
                model.Tinrow1 = w8BENEData.Tinrow1;
                model.Tinrow2 = w8BENEData.Tinrow2;
                model.Tinrow3 = w8BENEData.Tinrow3;
                model.Tinrow4 = w8BENEData.Tinrow4;
                model.Tinrow5 = w8BENEData.Tinrow5;
                model.Tinrow6 = w8BENEData.Tinrow6;
                model.Tinrow8 = w8BENEData.Tinrow8;
                model.Tinrow9 = w8BENEData.Tinrow9;
                model.W8beneentryDate = w8BENEData.W8beneentryDate;
                model.W8benefontName = w8BENEData.W8benefontName;
                model.W8beneonBehalfName = w8BENEData.W8beneonBehalfName;
                model.W8beneprintName = w8BENEData.W8beneprintName;
                model.W8beneprintSize = w8BENEData.W8beneprintSize;
                model._12City = w8BENEData._12City;
                model._12Country = w8BENEData._12Country;
                model._12mailingAddress = w8BENEData._12mailingAddress;
                model._13gin = w8BENEData._13gin;
                model._14aCb = w8BENEData._14aCb ?? false;
                model._14aTb = w8BENEData._14aTb;
                model._14bCb1 = w8BENEData._14bCb1 ?? false;
                model._14bCb2others = w8BENEData._14bCb2others;
                model._14bTb = w8BENEData._14bTb;
                model._14cCb = w8BENEData._14cCb ?? false;
                model._15cTb1 = w8BENEData._15cTb1;
                model._15cTb2 = w8BENEData._15cTb2;
                model._15cTb3 = w8BENEData._15cTb3;
                model._15cTb4 = w8BENEData._15cTb4;
                model._16tb = w8BENEData._16tb;
                model._17cb1 = w8BENEData._17cb1 ?? false;
                model._17cb2 = w8BENEData._17cb2 ?? false;
                model._18cb = w8BENEData._18cb ?? false;
                model._19cb = w8BENEData._19cb ?? false;
                model._20tb = w8BENEData._20tb;
                model._21cb = w8BENEData._21cb ?? false;
                model._22cb = w8BENEData._22cb ?? false;
                model._23cb = w8BENEData._23cb ?? false;
                model._24aCb = w8BENEData._24aCb ?? false;
                model._24borcCb = w8BENEData._24borcCb;
                model._24dCb = w8BENEData._24dCb ?? false;
                model._25aCb = w8BENEData._25aCb ?? false;
                model._25bcCb = w8BENEData._25bcCb;
                model._26cb1 = w8BENEData._26cb1 ?? false;
                model._26cb2or3 = w8BENEData._26cb2or3;
                model._26cb4or5 = w8BENEData._26cb4or5;
                model._26tb1 = w8BENEData._26tb1;
                model._26tb2 = w8BENEData._26tb2;
                model._26tb3 = w8BENEData._26tb3;
                model._27cb = w8BENEData._27cb ?? false;
                model._28aorbCb = w8BENEData._28aorbCb;
                model._29cb = w8BENEData._29cb;
                model._30cb = w8BENEData._30cb ?? false;
                model._31cb = w8BENEData._31cb ?? false;
                model._32cb = w8BENEData._32cb ?? false;
                model._33cb = w8BENEData._33cb ?? false;
                model._33tb = w8BENEData._33tb;
                model._34cb = w8BENEData._34cb ?? false;
                model._34tb = w8BENEData._34tb;
                model._35cb = w8BENEData._35cb ?? false;
                model._35tb = w8BENEData._35tb;
                model._36cb = w8BENEData._36cb ?? false;
                model._37aorbCb = w8BENEData._37aorbCb;
                model._37aTb = w8BENEData._37aTb;
                model._37bTb1 = w8BENEData._37bTb1;
                model._37bTb2 = w8BENEData._37bTb2;
                model._38cb = w8BENEData._38cb ?? false;
                model._39cb = w8BENEData._39cb ?? false;
                model._40aCb = w8BENEData._40aCb ?? false;
                model._40borcCb = w8BENEData._40borcCb;
                model._41cb = w8BENEData._41cb ?? false;
                model._42tb = w8BENEData._42tb;
                model._43cb = w8BENEData._43cb ?? false;
                model.activeTabIndex = w8BENEData.ActiveTabIndex ?? "";
            }
            return model;
        }
        public string UpdateForEntity(W8BENERequest request)
        {
            var response = _evolvedtaxContext.TblW8ebeneforms.FirstOrDefault(p => p.W8beneemailAddress == request.EmailId);
            if (response != null)
            {
                request.Id = response.Id;
                response.City = string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode);
                response.MailingAddress = string.Concat(request.MAddress1, " ", request.MAddress2);
                response.MCity = string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode);
                response.PermanentResidenceAddress = string.Concat(request.PAddress1, " ", request.PAddress2);
                response.NameOfDiregardedEntity = request.NameOfDiregardedEntity;
                response.AddressRow1 = request.AddressRow1;
                response.AddressRow2 = request.AddressRow2;
                response.AddressRow3 = request.AddressRow3;
                response.AddressRow4 = request.AddressRow4;
                response.AddressRow5 = request.AddressRow5;
                response.AddressRow6 = request.AddressRow6;
                response.AddressRow7 = request.AddressRow7;
                response.AddressRow8 = request.AddressRow8;
                response.AddressRow9 = request.AddressRow9;
                response.FatcaStatus = request.FatcaStatus;
                response.FtinCheck = request.FtinCheck;
                response.Gin = request.Gin;
                response.CountryOfIncorporation = request.CountryOfIncorporation;
                response.TypeOfEntity = request.TypeOfEntity;
                response.UsTin = request.UsTin;
                response.ForeignTaxIdentifyingNumber = request.ForeignTaxIdentifyingNumber;
                response.ReferenceNumberS = request.ReferenceNumberS;
                response._11fatcaCb = request._11fatcaCb;
                response.NameRow1 = request.NameRow1;
                response.NameRow2 = request.NameRow2;
                response.NameRow3 = request.NameRow3;
                response.NameRow4 = request.NameRow4;
                response.NameRow5 = request.NameRow5;
                response.NameRow6 = request.NameRow6;
                response.NameRow7 = request.NameRow7;
                response.NameRow8 = request.NameRow8;
                response.NameRow9 = request.NameRow9;
                response.PrintNameOfSigner = request.PrintNameOfSigner;
                response.SignatureDateMmDdYyyy = request.SignatureDateMmDdYyyy;
                response.NameOfOrganization = request.NameOfOrganization;
                response.Tin1row7 = request.Tin1row7;
                response.Tinrow1 = request.Tinrow1;
                response.Tinrow2 = request.Tinrow2;
                response.Tinrow3 = request.Tinrow3;
                response.Tinrow4 = request.Tinrow4;
                response.Tinrow5 = request.Tinrow5;
                response.Tinrow6 = request.Tinrow6;
                response.Tinrow8 = request.Tinrow8;
                response.Tinrow9 = request.Tinrow9;
                response.W8beneentryDate = request.W8beneentryDate;
                response.W8benefontName = request.W8benefontName;
                response.W8beneonBehalfName = request.W8beneonBehalfName;
                response.W8beneprintName = request.W8beneprintName;
                response.W8beneprintSize = request.W8beneprintSize;
                response._12City = request._12City;
                response._12Country = request._12Country;
                response._12mailingAddress = request._12mailingAddress;
                response._13gin = request._13gin;
                response._14aCb = request._14aCb;
                response._14aTb = request._14aTb;
                response._14bCb1 = request._14bCb1;
                response._14bCb2others = request._14bCb2others;
                response._14bTb = request._14bTb;
                response._14cCb = request._14cCb;
                response._15cTb1 = request._15cTb1;
                response._15cTb2 = request._15cTb2;
                response._15cTb3 = request._15cTb3;
                response._15cTb4 = request._15cTb4;
                response._16tb = request._16tb;
                response._17cb1 = request._17cb1;
                response._17cb2 = request._17cb2;
                response._18cb = request._18cb;
                response._19cb = request._19cb;
                response._20tb = request._20tb;
                response._21cb = request._21cb;
                response._22cb = request._22cb;
                response._23cb = request._23cb;
                response._24aCb = request._24aCb;
                response._24borcCb = request._24borcCb;
                response._24dCb = request._24dCb;
                response._25aCb = request._25aCb;
                response._25bcCb = request._25bcCb;
                response._26cb1 = request._26cb1;
                response._26cb2or3 = request._26cb2or3;
                response._26cb4or5 = request._26cb4or5;
                response._26tb1 = request._26tb1;
                response._26tb2 = request._26tb2;
                response._26tb3 = request._26tb3;
                response._27cb = request._27cb;
                response._28aorbCb = request._28aorbCb;
                response._29cb = request._29cb;
                response._30cb = request._30cb;
                response._31cb = request._31cb;
                response._32cb = request._32cb;
                response._33cb = request._33cb;
                response._33tb = request._33tb;
                response._34cb = request._34cb;
                response._34tb = request._34tb;
                response._35cb = request._35cb;
                response._35tb = request._35tb;
                response._36cb = request._36cb;
                response._37aorbCb = request._37aorbCb;
                response._37aTb = request._37aTb;
                response._37bTb1 = request._37bTb1;
                response._37bTb2 = request._37bTb2;
                response._38cb = request._38cb;
                response._39cb = request._39cb;
                response._40aCb = request._40aCb;
                response._40borcCb = request._40borcCb;
                response._41cb = request._41cb;
                response._42tb = request._42tb;
                response._43cb = request._43cb;
                response.ActiveTabIndex = request.activeTabIndex;
                _evolvedtaxContext.TblW8ebeneforms.Update(response);
                _evolvedtaxContext.SaveChanges();
                if (request.IsPartialSave)
                {
                    return AppConstants.FormPartiallySave;
                }
                return W8BENECreationForEntity(request);
            }
            return SaveForEntity(request);
        }

        public void ActivateRecord(string ClientEmail)
        {
            var record = _evolvedtaxContext.TblW8ebeneforms.FirstOrDefault(e => e.W8beneemailAddress == ClientEmail && e.IsActive == false);
            if (record != null)
            {
                record.IsActive = true;
                _evolvedtaxContext.SaveChanges();
            }
        }
    }
}
