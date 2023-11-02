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

namespace EvolvedTax.Business.Services.W8ECIFormService
{
    public class W8ECIFormService : IW8ECIFormService
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly IGeneralQuestionareService _generalQuestionareService;
        readonly IGeneralQuestionareEntityService _generalQuestionareEntityService;
        public W8ECIFormService(EvolvedtaxContext evolvedtaxContext, IGeneralQuestionareService generalQuestionareService, IGeneralQuestionareEntityService generalQuestionareEntityService)
        {
            _generalQuestionareService = generalQuestionareService;
            _evolvedtaxContext = evolvedtaxContext;
            _generalQuestionareEntityService = generalQuestionareEntityService;
        }

        public string SaveForIndividual(FormRequest request)
        {
            string Items1stLine = string.Empty;
            string Items2ndLine = string.Empty;
            string Items3rdLine = string.Empty;
            string items = request.Items ?? string.Empty;
            int startindex = 0;
            string newline = items.Substring(startindex, items.Length - startindex);
            int line1 = 0;
            int line2 = 0;
            int line3 = 0;
            int Endline1 = 0;
            int Endline2 = 0;
            int Endline3 = 0;
            int finallength = items.Length;
            string newline2;
            string newline3;

            if (finallength < 75)
            {
                Items1stLine = items.Replace("&nbsp;", "") ?? string.Empty;
                Items2ndLine = "";
                Items3rdLine = "";
            }
            if (finallength > 74)
            {
                Endline1 = 76;
                do
                {
                    line1 = startindex;
                    newline = items.Substring(startindex, items.Length - startindex);
                    startindex = startindex + newline.IndexOf(" ");
                    startindex++;
                } while (startindex < Endline1);
                Items1stLine = items.Substring(0, line1);

                if ((finallength - line1 > 132) || (finallength - line1 < 0))
                {
                    startindex = 0;
                    Endline2 = 134;

                }
                else
                {
                    startindex = 0;
                    Endline2 = finallength - line1;
                }
                do
                {
                    line2 = startindex;
                    newline2 = newline.Substring(line2, newline.Length - startindex);
                    startindex = startindex + newline2.IndexOf(" ");
                    startindex++;
                } while (startindex < Endline2);
                Items2ndLine = items?.Substring(line1, line2) ?? string.Empty;

                if (finallength - line1 - line2 > 132)
                {
                    startindex = 0;
                    Endline3 = 134;
                }
                else
                {
                    startindex = 0;
                    Endline3 = finallength - line1 - line2 + 1;
                }
                do
                {
                    line3 = startindex;
                    newline3 = items.Substring(line3, newline2.Length - startindex);
                    startindex = startindex + newline.IndexOf(" ");
                    startindex++;
                } while (startindex < Endline3);
                Items3rdLine = items.Substring(line1 + line2, line3) ?? string.Empty;
            }

            var model = new TblW8eciform
            {
                NameOfIndividual = request.NameOfIndividualW8ECI,
                CountryOfIncorporation = "",
                City = string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode),
                DisregardedEntity = "",
                TypeOfEntity = "8",
                CheckIfFtinNotLegallyRequiredYN = request.CheckIfFtinNotLegallyRequiredYN,
                Country = request.PCountry,
                Ssnitnein = "0",
                DateOfBirthMmDdYyyy = request.DateOfBirthMmDdYyyyW8ECI,
                ForeignTaxIdentifyingNumber = request.ForeignTaxIdentifyingNumber?.Replace("&nbsp;", ""),
                MailingAddress = string.Concat(request.MAddress1, " ", request.MAddress2),
                MCity = string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode),
                PermanentResidenceAddress = string.Concat(request.PAddress1, " ", request.PAddress2),
                PrintNameOfSigner = request.PrintNameOfSignerW8ECI,
                ReferenceNumberS = request.ReferenceNumberSW8ECI,
                Items = items,
                SignatureDateMmDdYyyy = DateTime.Now.ToString("MM-dd-yyyy"),
                SsnOrItin = request.Ssnitnein,
                DealerCertification = true,
                Status = "1",
                W8eciemailAddress = request.EmailId,
                W8ecionBehalfName = request.W8ECIOnBehalfName
            };
            if (_evolvedtaxContext.TblW8eciforms.Any(p => p.W8eciemailAddress == request.EmailId))
            {
                var data = _evolvedtaxContext.TblW8eciforms.FirstOrDefault(p => p.W8eciemailAddress == request.EmailId);
                request.Id = data.Id;
                return UpdateForIndividual(request);
            }
            _evolvedtaxContext.TblW8eciforms.Add(model);
            _evolvedtaxContext.SaveChanges();
            request.Id = model.Id;
            if (request.IsPartialSave)
            {
                return AppConstants.FormPartiallySave;
            }
            return W8ECICreation(request, Items1stLine, Items2ndLine, Items3rdLine);
        }
        public string SaveForEntity(FormRequest request)
        {
            string Items1stLine = string.Empty;
            string Items2ndLine = string.Empty;
            string Items3rdLine = string.Empty;
            string items = request.Items ?? string.Empty;
            int startindex = 0;
            string newline = items.Substring(startindex, items.Length - startindex);
            int line1 = 0;
            int line2 = 0;
            int line3 = 0;
            int Endline1 = 0;
            int Endline2 = 0;
            int Endline3 = 0;
            int finallength = items.Length;
            string newline2;
            string newline3;

            if (finallength < 75)
            {
                Items1stLine = items.Replace("&nbsp;", "") ?? string.Empty;
                Items2ndLine = "";
                Items3rdLine = "";
            }
            if (finallength > 74)
            {
                Endline1 = 76;
                do
                {
                    line1 = startindex;
                    newline = items.Substring(startindex, items.Length - startindex);
                    startindex = startindex + newline.IndexOf(" ");
                    startindex++;
                } while (startindex < Endline1);
                Items1stLine = items.Substring(0, line1);

                if ((finallength - line1 > 132) || (finallength - line1 < 0))
                {
                    startindex = 0;
                    Endline2 = 134;

                }
                else
                {
                    startindex = 0;
                    Endline2 = finallength - line1;
                }
                do
                {
                    line2 = startindex;
                    newline2 = newline.Substring(line2, newline.Length - startindex);
                    startindex = startindex + newline2.IndexOf(" ");
                    startindex++;
                } while (startindex < Endline2);
                Items2ndLine = items?.Substring(line1, line2) ?? string.Empty;

                if (finallength - line1 - line2 > 132)
                {
                    startindex = 0;
                    Endline3 = 134;
                }
                else
                {
                    startindex = 0;
                    Endline3 = finallength - line1 - line2 + 1;
                }
                do
                {
                    line3 = startindex;
                    newline3 = items.Substring(line3, newline2.Length - startindex);
                    startindex = startindex + newline.IndexOf(" ");
                    startindex++;
                } while (startindex < Endline3);
                Items3rdLine = items.Substring(line1 + line2, line3) ?? string.Empty;
            }

            var model = new TblW8eciform
            {
                NameOfIndividual = request.GQOrgName,
                CountryOfIncorporation = request.CountryOfIncorporation,
                City = string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode),
                DisregardedEntity = request.DEOwnerNameW8ECI,
                TypeOfEntity = request.TypeOfEntityForW8ECI,
                CheckIfFtinNotLegallyRequiredYN = request.CheckIfFtinNotLegallyRequiredYNW8ECI,
                Country = request.PCountry,
                Ssnitnein = request.Ssnitnein,
                DateOfBirthMmDdYyyy = "",
                ForeignTaxIdentifyingNumber = request.ForeignTaxIdentifyingNumberW8ECI?.Replace("&nbsp;", ""),
                MailingAddress = string.Concat(request.MAddress1, " ", request.MAddress2),
                MCity = string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode),
                PermanentResidenceAddress = string.Concat(request.PAddress1, " ", request.PAddress2),
                PrintNameOfSigner = request.PrintNameOfSignerW8ECI,
                ReferenceNumberS = request.ReferenceNumberSW8ECI,
                Items = items,
                SignatureDateMmDdYyyy = DateTime.Now.ToString("MM-dd-yyyy"),
                SsnOrItin = request.Ssnitnein,
                DealerCertification = true,
                Status = "1",
                W8eciemailAddress = request.EmailId,
                W8ecionBehalfName = request.W8ECIOnBehalfName,
                ActiveTabIndex = request.activeTabIndex,
                IsActive=true
            };
            if (_evolvedtaxContext.TblW8eciforms.Any(p => p.W8eciemailAddress == request.EmailId))
            {
                return UpdateForEntity(request);
            }
            _evolvedtaxContext.TblW8eciforms.Add(model);
            _evolvedtaxContext.SaveChanges();
            request.Id = model.Id;
            if (request.IsPartialSave)
            {
                return AppConstants.FormPartiallySave;
            }
            return W8ECICreationForEntity(request, Items1stLine, Items2ndLine, Items3rdLine);
        }
        protected static string W8ECICreation(FormRequest request, string Items1stLine, string Items2ndLine, string Items3rdLine)
        {
            request.TypeOfEntity = "8";
            string templatefile = request.TemplateFilePath;
            string fileName = string.Concat(request.NameOfIndividualW8ECI?.Replace(" ", "_"), "_", "Form_", AppConstants.W8ECIForm, "_", request.Id, "_temp.pdf");
            string newFile = Path.Combine(request.BasePath, fileName);
            PdfReader pdfReader = new PdfReader(templatefile);
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;
            // set form pdfFormFields  

            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_1[0]", request.NameOfIndividualW8ECI);
            //pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_2[0]", "");
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_2[0]", request.GQCountry);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_3[0]", request.DisregardedEntity);
            if (request.TypeOfEntity == "1")
            {
                //           pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
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
            }
            else if (request.TypeOfEntity == "2")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
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
            }
            else if (request.TypeOfEntity == "3")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                //'pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
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
            }
            else if (request.TypeOfEntity == "4")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                //'pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
            }
            else if (request.TypeOfEntity == "5")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
            }
            else if (request.TypeOfEntity == "6")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
            }
            else if (request.TypeOfEntity == "7")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
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
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
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
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
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
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
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
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
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
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
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
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
            }

            pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", request.NameOfIndividualW8ECI);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_4[0]", string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode));
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_5[0]", request.City);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_6[0]", request.MCountry);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_7[0]", request.MAddress1 + " " + request.MAddress2);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_8[0]", request.City);
            //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", GlobalVariables.Globals.SSNITNEIN1);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");




            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_9[0]", request.Ssnitnein);

            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_10[0]", request.ForeignTaxIdentifyingNumber);

            if (request.CheckIfFtinNotLegallyRequiredYN != false)
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_3[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_11[0]", request.ReferenceNumberS);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_12[0]", request.DateOfBirthMmDdYyyyW8ECI);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_13[0]", Items1stLine);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_14[0]", Items2ndLine);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_15[0]", Items3rdLine);

            if (request.DealerCertification == false)
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_4[0]", "0");
            }

           // pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_4[0]" );
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_16[0]", request.PrintNameOfSignerW8ECI);
            //pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_17[0]", DateTime.Now.ToString("MM-dd-yyyy"));
            // I certify check
            //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_4[0]", "1");
            string sTmp = "W8-ECI Completed for " + request.NameOfIndividualW8ECI + " " + request.Ssnitnein;
            // flatten the form To remove editting options, set it to false  
            // to leave the form open to subsequent manual edits  
            pdfStamper.FormFlattening = true;
            // close the pdf  
            PdfContentByte overContent = pdfStamper.GetOverContent(1);
            Rectangle rectangle = new Rectangle(110, 50, 350, 72, 0);
            rectangle.BackgroundColor = BaseColor.LIGHT_GRAY;
            overContent.Rectangle(rectangle);

            var src1 = Path.Combine(Directory.GetCurrentDirectory(), "signature-image.png");
            Image image1 = Image.GetInstance(src1);
            PdfImage stream1 = new PdfImage(image1, "", null);
            stream1.Put(new PdfName("ITXT_SpecialId"), new PdfName("123456789"));
            PdfIndirectObject ref1 = pdfStamper.Writer.AddToBody(stream1);
            image1.SetAbsolutePosition(130, 51);
            PdfContentByte over1 = pdfStamper.GetOverContent(1);
            over1.AddImage(image1);
            //for date
            PdfContentByte overContent1 = pdfStamper.GetOverContent(1);
            iTextSharp.text.Rectangle rectangle1 = new iTextSharp.text.Rectangle(500, 52, 560, 70, 0);
            rectangle1.BackgroundColor = BaseColor.LIGHT_GRAY;
            overContent1.Rectangle(rectangle1);
            PdfAnnotation annotation1;
            bool IsDate = true;
            string methodName = "Index?IsDate=" + IsDate.ToString();
            annotation1 = PdfAnnotation.CreateLink(pdfStamper.Writer, rectangle1, PdfAnnotation.HIGHLIGHT_INVERT, new PdfAction(Path.Combine(request.Host, "Certification", methodName)));
            pdfStamper.AddAnnotation(annotation1, 1);
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
            image2.SetAbsolutePosition(485, 51);
            PdfContentByte over2 = pdfStamper.GetOverContent(1);
            over2.AddImage(image2);
            #endregion
            PdfAnnotation annotation;
            annotation = PdfAnnotation.CreateLink(pdfStamper.Writer, rectangle, PdfAnnotation.HIGHLIGHT_INVERT, new PdfAction(Path.Combine(request.Host, "Certification", "Index")));
            pdfStamper.AddAnnotation(annotation, 1);

            pdfStamper.Close();
            pdfReader.Close();
            //var dd = System.IO.File.ReadAllBytes(newFile);
            //byte[] pngByte = Freeware.Pdf2Png.Convert(dd, 1);
            //System.IO.File.WriteAllBytes(Path.Combine(Request.PhysicalApplicationPath, "dd.png"), pngByte);

            //pdfStamper.Close();
            //myconnect.Close();
            //Response.Redirect("~/" + GlobalVariables.Globals.Uploaded_File1);
            return fileName;
        }
        protected static string W8ECICreationForEntity(FormRequest request, string Items1stLine, string Items2ndLine, string Items3rdLine)
        {
            string templatefile = request.TemplateFilePath;
            string fileName = string.Concat(request.GQOrgName?.Replace(" ", "_"), "_", "Form_", AppConstants.W8ECIForm, "_", request.Id, "_temp.pdf");
            string newFile = Path.Combine(request.BasePath, fileName);
            PdfReader pdfReader = new PdfReader(templatefile);
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;
            // set form pdfFormFields  

            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_1[0]", request.GQOrgName);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_2[0]", request.CountryOfIncorporation);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_3[0]", request.DEOwnerNameW8ECI);
            if (request.TypeOfEntityForW8ECI == "1")
            {
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
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
            }
            else if (request.TypeOfEntityForW8ECI == "2")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
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
            }
            else if (request.TypeOfEntityForW8ECI == "3")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                //'pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
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
            }
            else if (request.TypeOfEntityForW8ECI == "4")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                //'pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
            }
            else if (request.TypeOfEntityForW8ECI == "5")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
            }
            else if (request.TypeOfEntityForW8ECI == "6")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
            }
            else if (request.TypeOfEntityForW8ECI == "7")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
            }
            else if (request.TypeOfEntityForW8ECI == "8")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
            }
            else if (request.TypeOfEntityForW8ECI == "9")
            {

                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[5]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[6]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[7]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[8]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
            }
            else if (request.TypeOfEntityForW8ECI == "10")
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
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[9]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
            }
            else if (request.TypeOfEntityForW8ECI == "11")
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
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[10]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
            }
            else if (request.TypeOfEntityForW8ECI == "12")
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
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[11]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
            }
            else if (request.TypeOfEntityForW8ECI == "13")
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
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[12]", "0");
            }

            //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[0]", request.PrintNameOfSignerW8ECI);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_4[0]", string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode));
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_5[0]", request.PCity);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_6[0]", request.PCountry);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_7[0]", request.MAddress1 + " " + request.MAddress2);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_8[0]", request.MCity);
            if (request.TypeofTaxNumber == "S" || request.TypeofTaxNumber == "I")
            {
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
            }
            else
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[0]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_2[1]", "0");
            }

            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_9[0]", request.Ssnitnein ?? "");

            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_10[0]", request.ForeignTaxIdentifyingNumberW8ECI);

            if (request.CheckIfFtinNotLegallyRequiredYNW8ECI != false)
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_3[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_11[0]", request.ReferenceNumberSW8ECI);
            //pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_12[0]", request.DateOfBirthMmDdYyyyW8ECI);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_13[0]", Items1stLine);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_14[0]", Items2ndLine);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_15[0]", Items3rdLine);

            //     pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_4[0]", GlobalVariables.Globals.DealerCertification1);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_16[0]", request.PrintNameOfSignerW8ECI);
            //pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_17[0]", DateTime.Now.ToString("MM-dd-yyyy"));
            // I certify check
            pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_4[0]", "1");
            string sTmp = "W8-ECI Completed for " + request.NameOfIndividualW8ECI + " " + request.Ssnitnein;
            // flatten the form To remove editting options, set it to false  
            // to leave the form open to subsequent manual edits  
            pdfStamper.FormFlattening = true;
            // close the pdf  
            PdfContentByte overContent = pdfStamper.GetOverContent(1);
            Rectangle rectangle = new Rectangle(110, 50, 350, 72, 0);
            rectangle.BackgroundColor = BaseColor.LIGHT_GRAY;
            overContent.Rectangle(rectangle);

            var src1 = Path.Combine(Directory.GetCurrentDirectory(), "signature-image.png");
            Image image1 = Image.GetInstance(src1);
            PdfImage stream1 = new PdfImage(image1, "", null);
            stream1.Put(new PdfName("ITXT_SpecialId"), new PdfName("123456789"));
            PdfIndirectObject ref1 = pdfStamper.Writer.AddToBody(stream1);
            image1.SetAbsolutePosition(130, 51);
            PdfContentByte over1 = pdfStamper.GetOverContent(1);
            over1.AddImage(image1);
            //for date
            PdfContentByte overContent1 = pdfStamper.GetOverContent(1);
            iTextSharp.text.Rectangle rectangle1 = new iTextSharp.text.Rectangle(500, 52, 560, 70, 0);
            rectangle1.BackgroundColor = BaseColor.LIGHT_GRAY;
            overContent1.Rectangle(rectangle1);
            PdfAnnotation annotation1;
            annotation1 = PdfAnnotation.CreateLink(pdfStamper.Writer, rectangle1, PdfAnnotation.HIGHLIGHT_INVERT, new PdfAction(Path.Combine(request.Host, "Certification", "Index")));
            pdfStamper.AddAnnotation(annotation1, 1);
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
            image2.SetAbsolutePosition(485, 51);
            PdfContentByte over2 = pdfStamper.GetOverContent(1);
            over2.AddImage(image2);
            #endregion
            PdfAnnotation annotation;
            annotation = PdfAnnotation.CreateLink(pdfStamper.Writer, rectangle, PdfAnnotation.HIGHLIGHT_INVERT, new PdfAction(Path.Combine(request.Host, "Certification", "Index")));
            pdfStamper.AddAnnotation(annotation, 1);

            pdfStamper.Close();
            pdfReader.Close();
            //var dd = System.IO.File.ReadAllBytes(newFile);
            //byte[] pngByte = Freeware.Pdf2Png.Convert(dd, 1);
            //System.IO.File.WriteAllBytes(Path.Combine(Request.PhysicalApplicationPath, "dd.png"), pngByte);

            //pdfStamper.Close();
            //myconnect.Close();
            //Response.Redirect("~/" + GlobalVariables.Globals.Uploaded_File1);
            return fileName;
        }
        public async Task<bool> UpdateByClientEmailId(string ClientEmail, PdfFormDetailsRequest request)
        {
            var response = _evolvedtaxContext.TblW8eciforms.Where(p => p.W8eciemailAddress == ClientEmail).FirstOrDefault();
            response.UploadedFile = request.FileName;
            response.W8ecientryDate = request.EntryDate;
            response.Status = "3";
            response.W8ecifontName = request.FontFamily;
            response.W8eciprintName = request.PrintName;
            response.W8eciprintSize = Convert.ToInt32(request.FontSize);
            _evolvedtaxContext.TblW8eciforms.Update(response);
            await _evolvedtaxContext.SaveChangesAsync();
            return true;
        }

        public FormRequest GetIndividualDataByClientEmailId(string ClientEmailId)
        {
            var gQuestionData = _generalQuestionareService.GetDataByClientEmail(ClientEmailId);
            var w8ECIData = _evolvedtaxContext.TblW8eciforms.FirstOrDefault(p => p.W8eciemailAddress == ClientEmailId);
            gQuestionData.NameOfIndividualW8ECI = w8ECIData?.NameOfIndividual;
            gQuestionData.CheckIfFtinNotLegallyRequiredYNW8ECI = w8ECIData?.CheckIfFtinNotLegallyRequiredYN ?? false;
            gQuestionData.DateOfBirthMmDdYyyyW8ECI = w8ECIData?.DateOfBirthMmDdYyyy;
            gQuestionData.ForeignTaxIdentifyingNumberW8ECI = w8ECIData?.ForeignTaxIdentifyingNumber;
            gQuestionData.PrintNameOfSignerW8ECI = w8ECIData?.PrintNameOfSigner;
            gQuestionData.ReferenceNumberSW8ECI = w8ECIData?.ReferenceNumberS;
            gQuestionData.SignatureDateMmDdYyyyW8ECI = w8ECIData?.SignatureDateMmDdYyyy;
            gQuestionData.EmailId = w8ECIData?.W8eciemailAddress ?? ClientEmailId;
            gQuestionData.Items = w8ECIData?.Items;
            gQuestionData.DealerCertification = (bool)w8ECIData.DealerCertification;
            gQuestionData.W8ECIOnBehalfName = (bool)w8ECIData.W8ecionBehalfName;
            return gQuestionData;
        }
        public FormRequest GetEntityDataByClientEmailId(string ClientEmailId)
        {
            var gQuestionData = _generalQuestionareEntityService.GetDataByClientEmail(ClientEmailId);
            var w8ECIData = _evolvedtaxContext.TblW8eciforms.FirstOrDefault(p => p.W8eciemailAddress == ClientEmailId);
            if (w8ECIData != null)
            {
                gQuestionData.GQOrgName = w8ECIData?.NameOfIndividual;
                gQuestionData.DEW8ECI = !string.IsNullOrEmpty(w8ECIData.DisregardedEntity);
                gQuestionData.DEOwnerNameW8ECI = w8ECIData.DisregardedEntity;
                gQuestionData.CountryOfIncorporation = w8ECIData.CountryOfIncorporation;
                gQuestionData.TypeOfEntityForW8ECI = w8ECIData.TypeOfEntity;
                gQuestionData.W8ECIOnBehalfName = w8ECIData?.W8ecionBehalfName ?? false;
                gQuestionData.CheckIfFtinNotLegallyRequiredYNW8ECI = w8ECIData?.CheckIfFtinNotLegallyRequiredYN ?? false;
                gQuestionData.DateOfBirthMmDdYyyyW8ECI = w8ECIData?.DateOfBirthMmDdYyyy;
                gQuestionData.ForeignTaxIdentifyingNumberW8ECI = w8ECIData?.ForeignTaxIdentifyingNumber;
                gQuestionData.PrintNameOfSignerW8ECI = w8ECIData?.PrintNameOfSigner;
                gQuestionData.ReferenceNumberSW8ECI = w8ECIData?.ReferenceNumberS;
                gQuestionData.SignatureDateMmDdYyyyW8ECI = w8ECIData?.SignatureDateMmDdYyyy;
                gQuestionData.EmailId = w8ECIData?.W8eciemailAddress ?? ClientEmailId;
                gQuestionData.Items = w8ECIData?.Items;
                gQuestionData.DealerCertification = (bool)w8ECIData.DealerCertification;
                gQuestionData.activeTabIndex = w8ECIData.ActiveTabIndex;
            }
            return gQuestionData;
        }
        public string UpdateForIndividual(FormRequest request)
        {
            var response = _evolvedtaxContext.TblW8eciforms.FirstOrDefault(p => p.W8eciemailAddress == request.EmailId);
            if (response != null)
            {
                string Items1stLine = string.Empty;
                string Items2ndLine = string.Empty;
                string Items3rdLine = string.Empty;
                string items = request.Items ?? string.Empty;
                int startindex = 0;
                string newline = items.Substring(startindex, items.Length - startindex);
                int line1 = 0;
                int line2 = 0;
                int line3 = 0;
                int Endline1 = 0;
                int Endline2 = 0;
                int Endline3 = 0;
                int finallength = items.Length;
                string newline2;
                string newline3;

                if (finallength < 75)
                {
                    Items1stLine = items.Replace("&nbsp;", "") ?? string.Empty;
                    Items2ndLine = "";
                    Items3rdLine = "";
                }
                if (finallength > 74)
                {
                    Endline1 = 76;
                    do
                    {
                        line1 = startindex;
                        newline = items.Substring(startindex, items.Length - startindex);
                        startindex = startindex + newline.IndexOf(" ");
                        startindex++;
                    } while (startindex < Endline1);
                    Items1stLine = items.Substring(0, line1);

                    if ((finallength - line1 > 132) || (finallength - line1 < 0))
                    {
                        startindex = 0;
                        Endline2 = 134;

                    }
                    else
                    {
                        startindex = 0;
                        Endline2 = finallength - line1;
                    }
                    do
                    {
                        line2 = startindex;
                        newline2 = newline.Substring(line2, newline.Length - startindex);
                        startindex = startindex + newline2.IndexOf(" ");
                        startindex++;
                    } while (startindex < Endline2);
                    Items2ndLine = items?.Substring(line1, line2) ?? string.Empty;

                    if (finallength - line1 - line2 > 132)
                    {
                        startindex = 0;
                        Endline3 = 134;
                    }
                    else
                    {
                        startindex = 0;
                        Endline3 = finallength - line1 - line2 + 1;
                    }
                    do
                    {
                        line3 = startindex;
                        newline3 = items.Substring(line3, newline2.Length - startindex);
                        startindex = startindex + newline.IndexOf(" ");
                        startindex++;
                    } while (startindex < Endline3);
                    Items3rdLine = items.Substring(line1 + line2, line3) ?? string.Empty;
                }
                request.Id = response.Id;
                response.NameOfIndividual = request.NameOfIndividualW8ECI;
                response.CountryOfIncorporation = "";
                response.City = string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode);
                response.DisregardedEntity = "";
                response.TypeOfEntity = "8";
                response.CheckIfFtinNotLegallyRequiredYN = request.CheckIfFtinNotLegallyRequiredYN;
                response.Country = request.PCountry;
                response.Ssnitnein = "0";
                response.DateOfBirthMmDdYyyy = request.DateOfBirthMmDdYyyyW8ECI;
                response.ForeignTaxIdentifyingNumber = request.ForeignTaxIdentifyingNumber?.Replace("&nbsp;", "");
                response.MailingAddress = string.Concat(request.MAddress1, " ", request.MAddress2);
                response.MCity = string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode);
                response.PermanentResidenceAddress = string.Concat(request.PAddress1, " ", request.PAddress2);
                response.PrintNameOfSigner = request.NameOfIndividualW8ECI;
                response.ReferenceNumberS = request.ReferenceNumberSW8ECI;
                response.Items = items;
                response.SignatureDateMmDdYyyy = DateTime.Now.ToString("MM-dd-yyyy");
                response.SsnOrItin = request.Ssnitnein;
                response.DealerCertification = true;
                response.Status = "1";
                response.W8eciemailAddress = request.EmailId;
                response.W8ecionBehalfName = request.W8ECIOnBehalfName;

                _evolvedtaxContext.TblW8eciforms.Update(response);
                _evolvedtaxContext.SaveChanges();
                if (request.IsPartialSave)
                {
                    return AppConstants.FormPartiallySave;
                }
                return W8ECICreation(request, Items1stLine, Items2ndLine, Items3rdLine);
            }
            return SaveForIndividual(request);
        }
        public string UpdateForEntity(FormRequest request)
        {
            var response = _evolvedtaxContext.TblW8eciforms.FirstOrDefault(p => p.W8eciemailAddress == request.EmailId);
            if (response != null)
            {
                string Items1stLine = string.Empty;
                string Items2ndLine = string.Empty;
                string Items3rdLine = string.Empty;
                string items = request.Items ?? string.Empty;
                int startindex = 0;
                string newline = items.Substring(startindex, items.Length - startindex);
                int line1 = 0;
                int line2 = 0;
                int line3 = 0;
                int Endline1 = 0;
                int Endline2 = 0;
                int Endline3 = 0;
                int finallength = items.Length;
                string newline2;
                string newline3;

                if (finallength < 75)
                {
                    Items1stLine = items.Replace("&nbsp;", "") ?? string.Empty;
                    Items2ndLine = "";
                    Items3rdLine = "";
                }
                if (finallength > 74)
                {
                    Endline1 = 76;
                    do
                    {
                        line1 = startindex;
                        newline = items.Substring(startindex, items.Length - startindex);
                        startindex = startindex + newline.IndexOf(" ");
                        startindex++;
                    } while (startindex < Endline1);
                    Items1stLine = items.Substring(0, line1);

                    if ((finallength - line1 > 132) || (finallength - line1 < 0))
                    {
                        startindex = 0;
                        Endline2 = 134;

                    }
                    else
                    {
                        startindex = 0;
                        Endline2 = finallength - line1;
                    }
                    do
                    {
                        line2 = startindex;
                        newline2 = newline.Substring(line2, newline.Length - startindex);
                        startindex = startindex + newline2.IndexOf(" ");
                        startindex++;
                    } while (startindex < Endline2);
                    Items2ndLine = items?.Substring(line1, line2) ?? string.Empty;

                    if (finallength - line1 - line2 > 132)
                    {
                        startindex = 0;
                        Endline3 = 134;
                    }
                    else
                    {
                        startindex = 0;
                        Endline3 = finallength - line1 - line2 + 1;
                    }
                    do
                    {
                        line3 = startindex;
                        newline3 = items.Substring(line3, newline2.Length - startindex);
                        startindex = startindex + newline.IndexOf(" ");
                        startindex++;
                    } while (startindex < Endline3);
                    Items3rdLine = items.Substring(line1 + line2, line3) ?? string.Empty;
                }
                request.Id = response.Id;
                response.NameOfIndividual = request.GQOrgName;
                response.CountryOfIncorporation = request.CountryOfIncorporation;
                response.City = string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode);
                response.DisregardedEntity = request.DEOwnerNameW8ECI;
                response.TypeOfEntity = request.TypeOfEntityForW8ECI;
                response.CheckIfFtinNotLegallyRequiredYN = request.CheckIfFtinNotLegallyRequiredYNW8ECI;
                response.Country = request.PCountry;
                response.Ssnitnein = request.Ssnitnein;
                response.DateOfBirthMmDdYyyy = "";
                response.ForeignTaxIdentifyingNumber = request.ForeignTaxIdentifyingNumberW8ECI?.Replace("&nbsp;", "");
                response.MailingAddress = string.Concat(request.MAddress1, " ", request.MAddress2);
                response.MCity = string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode);
                response.PermanentResidenceAddress = string.Concat(request.PAddress1, " ", request.PAddress2);
                response.PrintNameOfSigner = request.PrintNameOfSignerW8ECI;
                response.ReferenceNumberS = request.ReferenceNumberSW8ECI;
                response.Items = items;
                response.SignatureDateMmDdYyyy = DateTime.Now.ToString("MM-dd-yyyy");
                response.SsnOrItin = request.Ssnitnein;
                response.DealerCertification = true;
                response.Status = "1";
                response.W8eciemailAddress = request.EmailId;
                response.W8ecionBehalfName = request.W8ECIOnBehalfName;
                response.ActiveTabIndex = request.activeTabIndex;
                _evolvedtaxContext.TblW8eciforms.Update(response);
                _evolvedtaxContext.SaveChanges();
                if (request.IsPartialSave)
                {
                    return AppConstants.FormPartiallySave;
                }
                return W8ECICreationForEntity(request, Items1stLine, Items2ndLine, Items3rdLine);
            }
            return SaveForEntity(request);
        }

        public void ActivateRecord(string ClientEmail)
        {
            var record = _evolvedtaxContext.TblW8eciforms.FirstOrDefault(e => e.W8eciemailAddress == ClientEmail && e.IsActive == false);
            if (record != null)
            {
                record.IsActive = true;
                _evolvedtaxContext.SaveChanges();
            }
        }
    }

   
}
