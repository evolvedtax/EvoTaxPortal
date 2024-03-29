﻿using Azure.Core;
using Azure;
using EvolvedTax.Data.Models.Entities;
using iTextSharp.text.pdf;
using System.Diagnostics.Metrics;
using System.Globalization;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using iTextSharp.text.html.simpleparser;
using EvolvedTax.Data.Models.DTOs.Request;
using Microsoft.AspNetCore.Hosting.Server;
using System.Drawing;
using System.Xml.Linq;
using SkiaSharp;
using static iTextSharp.text.Font;
using EvolvedTax.Common.Constants;
using EvolvedTax.Business.Services.GeneralQuestionareService;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.Ocsp;
using EvolvedTax.Business.Services.GeneralQuestionareEntityService;
using System.Linq.Expressions;

namespace EvolvedTax.Business.Services.W9FormService
{
    public class W9FormService : IW9FormService
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly IGeneralQuestionareService _generalQuestionareService;
        readonly IGeneralQuestionareEntityService _generalQuestionareEntityService;
        public W9FormService(EvolvedtaxContext evolvedtaxContext, IGeneralQuestionareService generalQuestionareService, IGeneralQuestionareEntityService generalQuestionareEntityService)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _generalQuestionareService = generalQuestionareService;
            _generalQuestionareEntityService = generalQuestionareEntityService;
        }
        public string SaveForIndividual(FormRequest request)
        {
            var model = new TblW9form
            {
                Name = string.Concat(request.GQFirstName, " ", request.GQLastName),
                BusinessEntity = "",
                FederalTaxClassification = "",
                Address = string.Concat(request.MAddress1, " ", request.MAddress2),
                City = string.Concat(request.MCity, ", ", request.MState ?? request.MProvince, ", ", request.MZipCode),
                Country = request.MCountry,
                Address1 = string.Concat(request.PAddress1, " ", request.PAddress2),
                City1 = string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode),
                Country1 = request.PCountry,
                ListofAccounts = " ",
                Exemptions = " ",
                Fatca = " ",
                SsnTin = request.Ssnitnein,
                W9emailAddress = request.EmailId,
                W9entryDate = DateTime.Now,
            };
            if (_evolvedtaxContext.TblW9forms.Any(p => p.W9emailAddress == request.EmailId))
            {
                return UpdateForIndividual(request);
            }
            _evolvedtaxContext.TblW9forms.Add(model);
            _evolvedtaxContext.SaveChanges();
            request.Id = model.Id;
            if (request.IsPartialSave)
            {
                return AppConstants.FormPartiallySave;
            }
            return W9Creation(request);
        }

        public string SaveForEntity(FormRequest request)
        {
            var model = new TblW9form
            {
                Name = "",
                BusinessEntity = request.GQOrgName,
                FederalTaxClassification = "",
                Address = string.Concat(request.MAddress1, " ", request.MAddress2),
                City = string.Concat(request.MCity, ", ", request.MState ?? request.MProvince, ", ", request.MZipCode),
                Country = request.MCountry,
                Address1 = string.Concat(request.PAddress1, " ", request.PAddress2),
                City1 = string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode),
                Country1 = request.PCountry,
                ListofAccounts = " ",
                Exemptions = request.Payeecode,
                Fatca = request.W9Fatca,
                SsnTin = request.Ssnitnein,
                W9emailAddress = request.EmailId,
                W9entryDate = DateTime.Now,
                IsActive = true,
            };

            if (_evolvedtaxContext.TblW9forms.Any(p => p.W9emailAddress == request.EmailId))
            {
                return UpdateForEntity(request);
            }
            _evolvedtaxContext.TblW9forms.Add(model);
            _evolvedtaxContext.SaveChanges();

            request.Id = model.Id;
            if (request.IsPartialSave)
            {
                return AppConstants.FormPartiallySave;
            }
            return W9Creation(request);
        }
        protected static string W9Creation(FormRequest request)
        {
            string templatefile = request.TemplateFilePath;
            string newFile1 = string.Empty;
            if (request.IndividualOrEntityStatus == AppConstants.IndividualStatus)
            {
                newFile1 = string.Concat(string.Concat(request.GQFirstName, " ", request.GQLastName).Replace(" ", "_"), "_", "Form_", AppConstants.W9Form, "_", request.Id, "_temp.pdf");
            }
            else
            {
                newFile1 = string.Concat(request.GQOrgName.Replace(" ", "_"), "_", "Form_", AppConstants.W9Form, "_", request.Id, "_temp.pdf");
            }
            string newFile = Path.Combine(request.BasePath, newFile1);

            PdfReader pdfReader = new PdfReader(templatefile);
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;
            try
            {
           
                // set form pdfFormFields  
                // The first worksheet and W-9 form
                if (request.IndividualOrEntityStatus == AppConstants.IndividualStatus)
                {
                    // 1 Name (as shown on your income tax return). Name is required on this line; do not leave this line blank.
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_1[0]", string.Concat(request.GQFirstName, " ", request.GQLastName));
                    // Social security number
                    if (!string.IsNullOrEmpty(request.Ssnitnein))
                    {
                        pdfFormFields.SetField("topmostSubform[0].Page1[0].SSN[0].f1_11[0]", request?.Ssnitnein?.Substring(0, 3));
                        pdfFormFields.SetField("topmostSubform[0].Page1[0].SSN[0].f1_12[0]", request?.Ssnitnein?.Substring(4, 2));
                        pdfFormFields.SetField("topmostSubform[0].Page1[0].SSN[0].f1_13[0]", request?.Ssnitnein?.Substring(7, 4));
                    }
                    // following seven boxes.
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[6]", "0");
                }
                else
                {
                    // 2 Business name/disregarded entity name, if different from above
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_2[0]", request.GQOrgName);
                    // 'Employer identification number
                    // ElseIf Mid(SSN_TIN1, 3, 1) = "-" Then
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].EmployerID[0].f1_14[0]", request?.Ssnitnein?.Substring(0, 2));
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].EmployerID[0].f1_15[0]", request?.Ssnitnein?.Substring(3, 7));
                    // Exempt payee code (if any)
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Exemptions[0].f1_5[0]", request.Payeecode);
                    // Exemption From FATCA reporting code(If any)
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].Exemptions[0].f1_6[0]", request.W9Fatca);

                    // 3. Check appropriate box for federal tax classification of the person whose name is entered on line 1. Check only one of the 
                    switch (request?.EntityType)
                    {
                        case "1": //Individual
                                  //pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[0]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[1]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[2]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[3]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[4]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[5]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[6]", "0");
                            break;
                        case "2": // C Corporation
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[0]", "0");
                            //pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[1]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[2]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[3]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[4]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[5]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[6]", "0");
                            break;
                        case "3": // S Corporation
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[0]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[1]", "0");
                            //pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[2]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[3]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[4]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[5]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[6]", "0");
                            break;
                        case "4": // Partnership
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[0]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[1]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[2]", "0");
                            //pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[3]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[4]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[5]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[6]", "0");
                            break;
                        case "5": // Trust/estate
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[0]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[1]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[2]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[3]", "0");
                            //pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[4]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[5]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[6]", "0");
                            break;
                        case "6": // Limited liability company
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[0]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[1]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[2]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[3]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[4]", "0");
                            //pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[5]", "0");
                            pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[6]", "0");
                            break;
                    }
                }

                // pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].f1_3[0]", TextBox3.Text)
                // pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].f1_4[0]", "4")
                // 4. Exemptions(codes apply only to certain entities, Not individuals; see instructions on page 3)
                // Exempt payee code (if any)
                // pdfFormFields.SetField("topmostSubform[0].Page1[0].Exemptions[0].f1_5[0]", Exemptions1)
                // Exemption From FATCA reporting code(If any)
                // pdfFormFields.SetField("topmostSubform[0].Page1[0].Exemptions[0].f1_6[0]", FATCA1)
                // 5. Address (number, street, and apt. or suite no.) See instructions.

                pdfFormFields.SetField("topmostSubform[0].Page1[0].Address[0].f1_7[0]", string.Concat(request.MAddress1, " ", request.MAddress2));
                // 6 City, state, and ZIP code
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Address[0].f1_8[0]", string.Concat(request.MCity, " ", request.MState, " ", request.MZipCode));
                // pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_9[0]", "9")
                // 7 List account number(s) here (optional)
                // pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_10[0]", ListofAccounts1)



                // Else
                // MsgBox("Please Check the Tax Number Or Social Security")
                // Exit Sub
                // End If
                // report by reading values from completed PDF  
                //pdfFormFields.SetField("EntryDate", DateTime.Now.ToString("MM-dd-yyyy"));
                pdfFormFields.SetField("Signature", "Form_W9.png");


                string sTmp = "W-9 Completed for " + pdfFormFields.GetField("f1_9(0)") + " " + pdfFormFields.GetField("f1_10(0)");
                // flatten the form To remove editting options, set it to false  
                // to leave the form open to subsequent manual edits  
                pdfStamper.FormFlattening = true;
                // close the pdf  
                //    pdfStamper.Close();



                PdfContentByte overContent = pdfStamper.GetOverContent(1);
                iTextSharp.text.Rectangle rectangle = new iTextSharp.text.Rectangle(130, 230, 350, 250, 0);
                rectangle.BackgroundColor = BaseColor.LIGHT_GRAY;
                overContent.Rectangle(rectangle);
                //for date
                PdfContentByte overContent1 = pdfStamper.GetOverContent(1);
                iTextSharp.text.Rectangle rectangle1 = new iTextSharp.text.Rectangle(440, 230, 510, 250, 0);
                rectangle1.BackgroundColor = BaseColor.LIGHT_GRAY;
                overContent1.Rectangle(rectangle1);
                // For pasting image of signature
                var src1 = Path.Combine(Directory.GetCurrentDirectory(), "signature-image.png");
                iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance(src1);
                PdfImage stream1 = new PdfImage(image1, "", null);
                stream1.Put(new PdfName("ITXT_SpecialId"), new PdfName("123456789"));
                PdfIndirectObject ref1 = pdfStamper.Writer.AddToBody(stream1);
                image1.SetAbsolutePosition(145, 230);
                PdfContentByte over1 = pdfStamper.GetOverContent(1);
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
                image2.SetAbsolutePosition(430, 230);
                PdfContentByte over2 = pdfStamper.GetOverContent(1);
                over2.AddImage(image2);
                #endregion



                PdfAnnotation annotation;
                annotation = PdfAnnotation.CreateLink(pdfStamper.Writer, rectangle, PdfAnnotation.HIGHLIGHT_INVERT, new PdfAction(Path.Combine(request.Host, "Certification", "Index")));
                pdfStamper.AddAnnotation(annotation, 1);

                PdfAnnotation annotation1;
                bool IsDate = true;
                string methodName = "Index?IsDate=" + IsDate.ToString();
                annotation1 = PdfAnnotation.CreateLink(pdfStamper.Writer, rectangle1, PdfAnnotation.HIGHLIGHT_INVERT, new PdfAction(Path.Combine(request.Host, "Certification", methodName)));
                pdfStamper.AddAnnotation(annotation1, 1);

                pdfStamper.Close();
                pdfReader.Close();
            }
            catch (Exception ex)
            {

                pdfStamper.Close();
                pdfReader.Close();
            }
            return newFile1;
        }

        public async Task<bool> UpdateByClientEmailId(string ClientEmail, PdfFormDetailsRequest request)
        {
            var response = _evolvedtaxContext.TblW9forms.Where(p => p.W9emailAddress == ClientEmail).FirstOrDefault();
            response.UploadedFile = request.FileName;
            response.W9entryDate = request.EntryDate;
            response.Status = "3";
            response.W9fontName = request.FontFamily;
            response.W9printName = request.PrintName;
            response.W9printSize = Convert.ToInt32(request.FontSize);
            _evolvedtaxContext.TblW9forms.Update(response);
            await _evolvedtaxContext.SaveChangesAsync();
            return true;
        }

        public FormRequest GetDataForIndividualByClientEmailId(string ClientEmailId)
        {
            var gQuestionData = _generalQuestionareService.GetDataByClientEmail(ClientEmailId);
            var w9Data = _evolvedtaxContext.TblW9forms.FirstOrDefault(p => p.W9emailAddress == ClientEmailId);
            gQuestionData.Ssnitnein = w9Data?.SsnTin;
            gQuestionData.EmailId = w9Data?.W9emailAddress ?? ClientEmailId;
            gQuestionData.W9PrintName = w9Data?.W9printName;
            return gQuestionData;
        }

        public FormRequest GetDataForEntityByClientEmailId(string ClientEmailId)
        {
            var gQuestionData = _generalQuestionareEntityService.GetDataByClientEmail(ClientEmailId);
            var w9Data = _evolvedtaxContext.TblW9forms.FirstOrDefault(p => p.W9emailAddress == ClientEmailId);
            gQuestionData.Ssnitnein = w9Data?.SsnTin;
            gQuestionData.EmailId = w9Data?.W9emailAddress ?? ClientEmailId;
            gQuestionData.W9PrintName = w9Data?.W9printName;
            return gQuestionData;
        }

        public string GetPrintNameByClientEmailId(string ClientEmailId)
        {
            return _evolvedtaxContext?.TblW9forms?.FirstOrDefault(p => p.W9emailAddress == ClientEmailId)?.W9printName ?? "";
        }
        public string UpdateForIndividual(FormRequest request)
        {
            var response = _evolvedtaxContext.TblW9forms.FirstOrDefault(p => p.W9emailAddress == request.EmailId);
            if (response != null)
            {
                request.Id = response.Id;
                response.Name = string.Concat(request.GQFirstName, " ", request.GQLastName);
                response.BusinessEntity = "";
                response.FederalTaxClassification = "";
                response.Address = string.Concat(request.MAddress1, " ", request.MAddress2);
                response.City = string.Concat(request.MCity, ", ", request.MState ?? request.MProvince, ", ", request.MZipCode);
                response.Country = request.MCountry;
                response.Address1 = string.Concat(request.PAddress1, " ", request.PAddress2);
                response.City1 = string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode);
                response.Country1 = request.PCountry;
                response.ListofAccounts = " ";
                response.Exemptions = " ";
                response.Fatca = " ";
                response.SsnTin = request.Ssnitnein;
                response.W9emailAddress = request.EmailId;
                _evolvedtaxContext.TblW9forms.Update(response);
                _evolvedtaxContext.SaveChanges();
                if (request.IsPartialSave)
                {
                    return AppConstants.FormPartiallySave;
                }
                return W9Creation(request);
            }
            return SaveForIndividual(request);
        }
        public string UpdateForEntity(FormRequest request)
        {
            var response = _evolvedtaxContext.TblW9forms.FirstOrDefault(p => p.W9emailAddress == request.EmailId);
            if (response != null)
            {
                request.Id = response.Id;
                response.Name = "";
                response.BusinessEntity = request.GQOrgName;
                response.FederalTaxClassification = "";
                response.Address = string.Concat(request.MAddress1, " ", request.MAddress2);
                response.City = string.Concat(request.MCity, ", ", request.MState ?? request.MProvince, ", ", request.MZipCode);
                response.Country = request.MCountry;
                response.Address1 = string.Concat(request.PAddress1, " ", request.PAddress2);
                response.City1 = string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode);
                response.Country1 = request.PCountry;
                response.ListofAccounts = "";
                response.Exemptions = request.Payeecode;
                response.Fatca = request.W9Fatca;
                response.SsnTin = request.Ssnitnein;
                response.W9emailAddress = request.EmailId;
                response.IsActive=true;
                _evolvedtaxContext.TblW9forms.Update(response);
                _evolvedtaxContext.SaveChanges();
                if (request.IsPartialSave)
                {
                    return AppConstants.FormPartiallySave;
                }
                return W9Creation(request);
            }
            return SaveForIndividual(request);
        }

        public void ActivateRecord(string ClientEmail)
        {
            var record = _evolvedtaxContext.TblW9forms.FirstOrDefault(e => e.W9emailAddress == ClientEmail && e.IsActive == false);
            if (record != null)
            {
                record.IsActive = true;
                _evolvedtaxContext.SaveChanges();
            }
        }
    }
}
