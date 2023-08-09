using EvolvedTax.Business.Services.GeneralQuestionareService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.Entities;
using iTextSharp.text.pdf;
using iTextSharp.text;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvolvedTax.Data.Models.DTOs.Request;
using System.Net.Mail;

namespace EvolvedTax.Business.Services.W8EXPFormService
{
    public class W8EXPFormService : IW8EXPFormService
    {
        readonly EvolvedtaxContext _evolvedtaxContext;

        public W8EXPFormService(EvolvedtaxContext evolvedtaxContext)
        {

            _evolvedtaxContext = evolvedtaxContext;
        }


      
        public string Save(FormRequest request)
        {
            if (_evolvedtaxContext.TblW8expforms.Any(p => p.EmailAddress == request.EmailId))
            {
                return Update(request);
            }
            var model = new TblW8expform
            {
                NameOfOrganization = request.GQOrgName,
                TypeOfEntity = request.TypeOfEntity,
                CountryOfIncorporation = request.CountryOfIncorporation,
                FatcaStatus = request.W8EXPFatca,
                PrintNameOfSigner = request.AuthSignatoryName,
                Gin = request.GIN,
                SsnOrItin = request.SsnOrItin,
                CheckIfFtinNotLegallyRequiredYN = request.LegallyRequired,
                ForeignTaxIdentifyingNumber = request.ForeignTaxIdentifyingNumber,
                ReferenceNumberS = request.Referencenumber,
                _10a = request._10a,
                _10b = request._10b,
                _10bText = request._10b_Text,
                _10c = request._10c,
                _10cText = request._10c_Text,
                _11 = request._11,
                _12 = request._12,
                _13a = request._13a,
                _13b = request._13b,
                _13c = request._13c,
                _13d = request._13d,
                _14 = request._14,
                _15 = request._15,
                _15Text1=request._15_Text1,
                _15Text2=request._15_Text2,
                _15Text3=request._15_Text3,
                _16 = request._16,
                _17 = request._17,
                _18 = request._18,
                _19 = request._19,
                _20a = request._20a,
                _20b = request._20b,
                _20c = request._20c,
                _21 = request._21,
                _21Text = request._21_Text,
                EmailAddress = request.EmailId

            };



            _evolvedtaxContext.TblW8expforms.Add(model);
            _evolvedtaxContext.SaveChanges();

            //var resultList = new List<object>();
            // resultList.Add(model.Id);
            request.W8ExpId = model.Id;
            //resultList.Add(W8CreationEXP(request));
            return W8CreationEXP(request);
            // return resultList;

        }

        public int SavePartial(FormRequest request)
        {
            if (_evolvedtaxContext.TblW8expforms.Any(p => p.EmailAddress == request.EmailId))
            {
                return UpdatePartial(request);
            }
            var model = new TblW8expform
            {
                NameOfOrganization = request.GQOrgName,
                TypeOfEntity = request.TypeOfEntity,
                CountryOfIncorporation = request.CountryOfIncorporation,
                FatcaStatus = request.W8EXPFatca,
                PrintNameOfSigner = request.AuthSignatoryName,
                Gin = request.GIN,
                SsnOrItin = request.SsnOrItin,
                CheckIfFtinNotLegallyRequiredYN = request.LegallyRequired,
                ForeignTaxIdentifyingNumber = request.ForeignTaxIdentifyingNumber,
                ReferenceNumberS = request.Referencenumber,
                _10a = request._10a,
                _10b = request._10b,
                _10bText = request._10b_Text,
                _10c = request._10c,
                _10cText = request._10c_Text,
                _11 = request._11,
                _12 = request._12,
                _13a = request._13a,
                _13b = request._13b,
                _13c = request._13c,
                _13d = request._13d,
                _14 = request._14,
                _15 = request._15,
                _15Text1 = request._15_Text1,
                _15Text2 = request._15_Text2,
                _15Text3 = request._15_Text3,
                _16 = request._16,
                _17 = request._17,
                _18 = request._18,
                _19 = request._19,
                _20a = request._20a,
                _20b = request._20b,
                _20c = request._20c,
                _21 = request._21,
                _21Text = request._21_Text,
                EmailAddress = request.EmailId,
                ActiveTabIndex = request.activeTabIndex

            };

            _evolvedtaxContext.TblW8expforms.Add(model);
            _evolvedtaxContext.SaveChanges();

            request.W8ExpId = model.Id;
            return model.Id;
        }

        public int UpdatePartial(FormRequest request)
        {
            var response = _evolvedtaxContext.TblW8expforms.FirstOrDefault(p => p.EmailAddress == request.EmailId);
            if (response != null)
            {
                request.W8ExpId = response.Id;
                response.NameOfOrganization = request.GQOrgName;
                response.TypeOfEntity = request.TypeOfEntity;
                response.CountryOfIncorporation = request.CountryOfIncorporation;
                response.FatcaStatus = request.W8EXPFatca;
                response.PrintNameOfSigner = request.AuthSignatoryName;
                response.Gin = request.GIN;
                response.SsnOrItin = request.SsnOrItin;
                response.CheckIfFtinNotLegallyRequiredYN = request.LegallyRequired;
                response.ForeignTaxIdentifyingNumber = request.ForeignTaxIdentifyingNumber;
                response.ReferenceNumberS = request.Referencenumber;
                response._10a = request._10a;
                response._10b = request._10b;
                response._10bText = request._10b_Text;
                response._10c = request._10c;
                response._10cText = request._10c_Text;
                response._11 = request._11;
                response._12 = request._12;
                response._13a = request._13a;
                response._13b = request._13b;
                response._13c = request._13c;
                response._13d = request._13d;
                response._14 = request._14;
                response._15 = request._15;
                response._15Text1 = request._15_Text1;
                response._15Text2 = request._15_Text2;
                response._15Text3 = request._15_Text3;
                response._16 = request._16;
                response._17 = request._17;
                response._18 = request._18;
                response._19 = request._19;
                response._20a = request._20a;
                response._20b = request._20b;
                response._20c = request._20c;
                response._21 = request._21;
                response._21Text = request._21_Text;
                response.EmailAddress = request.EmailId;
                response.ActiveTabIndex = request.activeTabIndex;
                _evolvedtaxContext.TblW8expforms.Update(response);
                _evolvedtaxContext.SaveChanges();
                return response.Id;

            }
            return SavePartial(request);
        }


        public string Update(FormRequest request)
        {
            var response = _evolvedtaxContext.TblW8expforms.FirstOrDefault(p => p.EmailAddress == request.EmailId);
            if (response != null)
            {
                request.W8ExpId = response.Id;
                response.NameOfOrganization = request.GQOrgName;
                response.TypeOfEntity = request.TypeOfEntity;
                response.CountryOfIncorporation = request.CountryOfIncorporation;
                response.FatcaStatus = request.W8EXPFatca;
                response.PrintNameOfSigner = request.AuthSignatoryName;
                response.Gin = request.GIN;
                response.SsnOrItin = request.SsnOrItin;
                response.CheckIfFtinNotLegallyRequiredYN = request.LegallyRequired;
                response.ForeignTaxIdentifyingNumber = request.ForeignTaxIdentifyingNumber;
                response.ReferenceNumberS = request.Referencenumber;
                response._10a = request._10a;
                response._10b = request._10b;
                response._10bText = request._10b_Text;
                response._10c = request._10c;
                response._10cText = request._10c_Text;
                response._11 = request._11;
                response._12 = request._12;
                response._13a = request._13a;
                response._13b = request._13b;
                response._13c = request._13c;
                response._13d = request._13d;
                response._14 = request._14;
                response._15 = request._15;
                response._15Text1 = request._15_Text1;
                response._15Text2 = request._15_Text2;
                response._15Text3 = request._15_Text3;
                response._16 = request._16;
                response._17 = request._17;
                response._18 = request._18;
                response._19 = request._19;
                response._20a = request._20a;
                response._20b = request._20b;
                response._20c = request._20c;
                response._21 = request._21;
                response._21Text = request._21_Text;
                response.EmailAddress = request.EmailId;
                _evolvedtaxContext.TblW8expforms.Update(response);
                _evolvedtaxContext.SaveChanges();
                return W8CreationEXP(request);
            }
            return Save(request);
        }

        protected static string W8CreationEXP(FormRequest request)
        {
            string templatefile = request.TemplateFilePath;
            string newFile1 = string.Empty;
            if (request.IndividualOrEntityStatus == AppConstants.IndividualStatus)
            {
                newFile1 = string.Concat(string.Concat(request.GQFirstName, " ", request.GQLastName).Replace(" ", "_"), "_", "Form_", AppConstants.W8EXPForm, "_", request.W8ExpId, "_temp.pdf");
            }
            else
            {
                newFile1 = string.Concat(request.GQOrgName.Replace(" ", "_"), "_", "Form_", AppConstants.W8EXPForm, "_", request.W8ExpId, "_temp.pdf");
            }
            string newFile = Path.Combine(request.BasePath, newFile1);

            PdfReader pdfReader = new PdfReader(templatefile);
            int numberOfPages = pdfReader.NumberOfPages;
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;

            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_01[0]", request.GQOrgName);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_02[0]", request.CountryOfIncorporation);


            switch (request?.TypeOfEntity)
            {
                case "1": //Individual
                          //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].ForeignCentral[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                    break;
                case "2": // C Corporation
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].ForeignCentral[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                    break;
                case "3": // S Corporation
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].ForeignCentral[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                    break;
                case "4": // Partnership
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].ForeignCentral[0].c1_1[0]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                    break;
                case "5": // Trust/estate
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].ForeignCentral[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                    break;
                case "6": // Limited liability company
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].ForeignCentral[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                    break;
            }





            switch (request?.W8EXPFatca)
            {
                case "1": //Individual
                          //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Registered[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Foreigngovern[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[10]", "0");
                    break;
                case "2": // C Corporation
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Registered[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Foreigngovern[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[10]", "0");
                    break;
                case "3": // S Corporation
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Registered[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Foreigngovern[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[10]", "0");
                    break;
                case "4": // Partnership
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[2]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].Registered[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Foreigngovern[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[10]", "0");
                    break;
                case "5": // Trust/estate
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Registered[0].c1_2[0]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Foreigngovern[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[10]", "0");
                    break;
                case "6": // Limited liability company
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Registered[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[3]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Foreigngovern[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[10]", "0");
                    break;

                /////////////////
                case "7": // Limited liability company
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Registered[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[4]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Foreigngovern[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[10]", "0");
                    break;
                case "8": // Limited liability company
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Registered[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[5]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].Foreigngovern[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[10]", "0");
                    break;
                case "9": // Limited liability company
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Registered[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Foreigngovern[0].c1_2[0]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[10]", "0");
                    break;
                case "10": // Limited liability company
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Registered[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Foreigngovern[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[6]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[10]", "0");
                    break;
                case "11": // Limited liability company
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Registered[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Foreigngovern[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[7]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[10]", "0");
                    break;
                case "12": // Limited liability company
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Registered[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Foreigngovern[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[8]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[10]", "0");
                    break;
                case "13": // Limited liability company
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Registered[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Foreigngovern[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[9]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[10]", "0");

                    break;
            }

            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_03[0]", request.PAddress1);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_04[0]", request.PCity);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_05[0]", request.PCountry);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_06[0]", request.MAddress1);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_07[0]", request.MCity);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_08[0]", request.MCountry);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_09[0]", request.Ssnitnein ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_10[0]", request.GIN);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_11[0]", request.ForeignTaxIdentifyingNumber);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_12[0]", request.Referencenumber);



            if (request._10a != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_3[0]", "0");
            }
            if (request._10b != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_4[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_13[0]", request._10b_Text);


            if (request._10c != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_4[1]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_14[0]", request._10c_Text);

            if (request._11 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_25[0]", "0");
            }

            if (request._12 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_5[0]", "0");
            }

            if (request._13a != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].ICertify[0].c2_1[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].Page2[0].ICertify[0].f2_01[0]", request._13a_Text);

            if (request._13b != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].IHaveAttached[0].c2_1[0]", "0");
            }

            if (request._13c != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].IfDetermination[0].c2_2[0]", "0");
            }

            if (request._13d != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].IfTheDetermination[0].c2_2[0]", "0");
            }

            if (request._14 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].ICertifyThat[0].c2_3[0]", "0");
            }

            if (request._15 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_4[0]", "0");
            }

            pdfFormFields.SetField("topmostSubform[0].Page2[0].BulletedList1[0].Bullet1[0].f2_02[0]", request._15_Text1);
            pdfFormFields.SetField("topmostSubform[0].Page2[0].BulletedList1[0].Bullet2[0].f2_03[0]", request._15_Text2);
            pdfFormFields.SetField("topmostSubform[0].Page2[0].BulletedList1[0].Bullet3[0].f2_04[0]", request._15_Text3);

            if (request._16 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].ICertifyThatThe[0].c2_5[0]", "0");
            }
            if (request._17 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].ICertifyThatTheEntity[0].c2_6[0]", "0");
            }
            if (request._18 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_7[0]", "0");
            }
            if (request._19 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].ICertifyThatThe_Ln19[0].c2_8[0]", "0");
            }
            if (request._20a != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].ICertifyThatThe_Ln20[0].c2_9[0]", "0");
            }
            if (request._20b != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_10[0]", "0");
            }
            if (request._20c != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].IFurtherCertify[0].c2_10[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_05[0]", request._21_Text);



            if (request._21 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_11[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].Page3[0].f3_2[0]", request.AuthSignatoryName);



            pdfFormFields.SetField("Signature", "Form_W8EXP.png");


            string sTmp = "W8-EXP Completed for " + pdfFormFields.GetField("f1_9(0)") + " " + pdfFormFields.GetField("f1_10(0)");
            // flatten the form To remove editting options, set it to false  
            // to leave the form open to subsequent manual edits  
            pdfStamper.FormFlattening = true;
            // close the pdf  
            //    pdfStamper.Close();



            PdfContentByte overContent = pdfStamper.GetOverContent(numberOfPages);
            iTextSharp.text.Rectangle rectangle = new iTextSharp.text.Rectangle(70, 530, 190, 550, 0);
            rectangle.BackgroundColor = BaseColor.LIGHT_GRAY;
            overContent.Rectangle(rectangle);

            //for date
            PdfContentByte overContent1 = pdfStamper.GetOverContent(numberOfPages);
            iTextSharp.text.Rectangle rectangle1 = new iTextSharp.text.Rectangle(450, 530, 610, 550, 0);
            rectangle1.BackgroundColor = BaseColor.LIGHT_GRAY;
            overContent1.Rectangle(rectangle1);

            // For pasting image of signature
            var src1 = Path.Combine(Directory.GetCurrentDirectory(), "signature-image.png");
            iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance(src1);
            PdfImage stream1 = new PdfImage(image1, "", null);
            stream1.Put(new PdfName("ITXT_SpecialId"), new PdfName("123456789"));
            PdfIndirectObject ref1 = pdfStamper.Writer.AddToBody(stream1);
            image1.SetAbsolutePosition(70, 530);
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
            image2.SetAbsolutePosition(450, 530);
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
            return newFile1;
        }


        public async Task<bool> UpdateByClientEmailId(string ClientEmail, PdfFormDetailsRequest request)
        {
            var response = _evolvedtaxContext.TblW8expforms.Where(p => p.EmailAddress == ClientEmail).FirstOrDefault();
            response.UploadedFile = request.FileName;
            response.SignatureDateMmDdYyyy = request.EntryDate.ToString();
            response.Status = "3";
            response.FontName = request.FontFamily;
            response.PrintNameOfSigner = request.PrintName;
            response.PrintSize = Convert.ToInt32(request.FontSize);
            _evolvedtaxContext.TblW8expforms.Update(response);
            await _evolvedtaxContext.SaveChangesAsync();
            return true;
        }

        public FormRequest? GetDataByClientEmail(string ClientEmail)
        {
            var query = from gq in _evolvedtaxContext.GeneralQuestionEntities
                        join w8 in _evolvedtaxContext.TblW8expforms on gq.UserName equals w8.EmailAddress
                        where gq.UserName == ClientEmail
                        select new FormRequest
                        {
                            GQOrgName = gq.OrgName ?? string.Empty,
                            EntityType = gq.EntityType ?? string.Empty,
                            Ccountry = gq.Ccountry ?? string.Empty,
                            MAddress1 = gq.MailingAddress1 ?? string.Empty,
                            MAddress2 = gq.MailingAddress2 ?? string.Empty,
                            MCity = gq.MailingCity ?? string.Empty,
                            MCountry = gq.MailingCountry ?? string.Empty,
                            MProvince = gq.MailingProvince ?? string.Empty,
                            MState = gq.MailingState ?? string.Empty,
                            MZipCode = gq.MailingZip ?? string.Empty,
                            PAddress1 = gq.PermanentAddress1 ?? string.Empty,
                            PAddress2 = gq.PermanentAddress2 ?? string.Empty,
                            PCity = gq.PermanentCity ?? string.Empty,
                            PCountry = gq.PermanentCountry ?? string.Empty,
                            PProvince = gq.PermanentProvince ?? string.Empty,
                            PState = gq.PermanentState ?? string.Empty,
                            PZipCode = gq.PermanentZip ?? string.Empty,
                            TypeofTaxNumber = gq.TypeofTaxNumber ?? string.Empty,
                            UserName = gq.UserName ?? string.Empty,
                            Payeecode = gq.Payeecode ?? string.Empty,
                            W9Fatca = gq.Fatca ?? string.Empty,
                            BackupWithHolding = gq.BackupWithHolding ?? string.Empty,
                            Ssnitnein = gq.Number,
                            DE = (bool)gq.De,
                            EnitityManagendOutSideUSA = (bool)gq.EnitityManagendOutSideUsa,
                            DEOwnerName = gq.DeownerName ?? string.Empty,
                            RetirementPlan = (bool)gq.RetirementPlan,
                            FormType = gq.FormType ?? string.Empty,
                            W8FormType = gq.W8formType ?? string.Empty,


                            // Add the fields from TblW8expforms

                            TypeOfEntity = w8.TypeOfEntity,
                            CountryOfIncorporation = w8.CountryOfIncorporation,
                            W8EXPFatca = w8.FatcaStatus,
                            AuthSignatoryName = w8.PrintNameOfSigner,
                            GIN = w8.Gin,
                            SsnOrItin = w8.SsnOrItin,
                            LegallyRequired = (bool)w8.CheckIfFtinNotLegallyRequiredYN,
                            ForeignTaxIdentifyingNumber = w8.ForeignTaxIdentifyingNumber,
                            Referencenumber = w8.ReferenceNumberS,
                            _10a = (bool)w8._10a,
                            _10b = (bool)w8._10b,
                            _10b_Text = w8._10bText,
                            _10c = (bool)w8._10c,
                            _10c_Text = w8._10cText,
                            _11 = (bool)w8._11,
                            _12 = (bool)w8._12,
                            _13a = (bool)w8._13a,
                            _13b = (bool)w8._13b,
                            _13c = (bool)w8._13c,
                            _13d = (bool)w8._13d,
                            _14 = (bool)w8._14,
                            _15 = (bool)w8._15,
                            _15_Text1 = w8._15Text1,
                            _15_Text2 = w8._15Text2,
                            _15_Text3 = w8._15Text3,
                            _16 = (bool)w8._16,
                            _17 = (bool)w8._17,
                            _18 = (bool)w8._18,
                            _19 = (bool)w8._19,
                            _20a = (bool)w8._20a,
                            _20b = (bool)w8._20b,
                            _20c = (bool)w8._20c,
                            _21 = (bool)w8._21,
                            _21_Text = w8._21Text,
                            EmailId = w8.EmailAddress,
                            activeTabIndex = w8.ActiveTabIndex,

                        };

            return query.FirstOrDefault();
        }


        public void ActivateRecord(string ClientEmail)
        {
            var record = _evolvedtaxContext.TblW8expforms.FirstOrDefault(e => e.EmailAddress == ClientEmail && e.IsActive == false);
            if (record != null)
            {
                record.IsActive = true;
                _evolvedtaxContext.SaveChanges();
            }

        }



    }
}
