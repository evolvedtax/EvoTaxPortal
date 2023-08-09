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
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.Ocsp;
using EvolvedTax.Common.Constants;

namespace EvolvedTax.Business.Services.W8BenFormService
{
    public class W8BenFormService : IW8BenFormService
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly IGeneralQuestionareService _generalQuestionareService;
        public W8BenFormService(EvolvedtaxContext evolvedtaxContext, IGeneralQuestionareService generalQuestionareService)
        {
            _generalQuestionareService = generalQuestionareService;
            _evolvedtaxContext = evolvedtaxContext;
        }
        public string Save(FormRequest request)
        {
            var model = new TblW8benform
            {
                NameOfIndividual = request.NameOfIndividual,
                City = string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode),
                ArticleAndParagraph = request.ArticleAndParagraph,
                CheckIfFtinNotLegallyRequiredYN = request.CheckIfFtinNotLegallyRequiredYN,
                Country = request.PCountry,
                CountryOfCitizenship = request.CountryOfCitizenship,
                DateOfBirthMmDdYyyy = request.DateOfBirthMmDdYyyy,
                ForeignTaxIdentifyingNumber = request.ForeignTaxIdentifyingNumber?.Replace("&nbsp;", ""),
                MailingAddress = string.Concat(request.MAddress1, " ", request.MAddress2),
                MCity = string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode),
                MCountry = request.MCountry,
                PermanentResidenceAddress = string.Concat(request.PAddress1, " ", request.PAddress2),
                Rate = request.Rate,
                PrintNameOfSigner = request.PrintNameOfSigner,
                ReferenceNumberS = request.ReferenceNumberS,
                ResidentCertification = request.Country,
                SignatureDateMmDdYyyy = DateTime.Now.ToString("MM-dd-yyyy"),
                SpecifyTypeOfIncome = request.SpecifyTypeOfIncome,
                SsnOrItin = request.Ssnitnein,
                Status = "1",
                W8benemailAddress = request.EmailId,
                W8benonBehalfName = request.W8BENOnBehalfName
            };
            if (request.EligibleForTheRateOfWithholding != "")
            {
                model.EligibleForTheRateOfWithholding = "1";
                request.EligibleForTheRateOfWithholding = "1";
            }
            else
            {
                model.EligibleForTheRateOfWithholding = "0";
                request.EligibleForTheRateOfWithholding = "0";
            }
            if (_evolvedtaxContext.TblW8benforms.Any(p => p.W8benemailAddress == request.EmailId))
            {
                return Update(request);
            }
            _evolvedtaxContext.TblW8benforms.Add(model);
            _evolvedtaxContext.SaveChanges();
            if (request.IsPartialSave)
            {
                return AppConstants.FormPartiallySave;
            }
            return W8BenCreation(request);
        }
        protected static string W8BenCreation(FormRequest request)
        {
            string templatefile = request.TemplateFilePath;
            string fileName = string.Concat(request.NameOfIndividual?.Replace(" ", "_"), "_", "Form_", AppConstants.W8BENForm, "_", Guid.NewGuid(), "_temp.pdf");
            string newFile = Path.Combine(request.BasePath, fileName);
            PdfReader pdfReader = new PdfReader(templatefile);
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;


            pdfFormFields.SetField("topmostSubform[0].Page1[0].f_1[0]", request.NameOfIndividual);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f_2[0]", request.CountryOfCitizenship);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f_3[0]", string.Concat(request.PAddress1, " ", request.PAddress2));
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f_4[0]", string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode));
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f_5[0]", request.PCountry);
            if (request.PAddress1 != request.MAddress1 && request.PAddress2 != request.MAddress2)
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].f_6[0]", string.Concat(request.MAddress1, " ", request.MAddress2));
                pdfFormFields.SetField("topmostSubform[0].Page1[0].f_7[0]", string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode));
               
            }
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f_8[0]", request.MCountry);


            pdfFormFields.SetField("topmostSubform[0].Page1[0].f_9[0]", request.Ssnitnein ?? "");
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f_10[0]", request.ForeignTaxIdentifyingNumber?.Replace("&nbsp;", ""));
            if (request.CheckIfFtinNotLegallyRequiredYN == true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_01[0]", "0");
            }

            pdfFormFields.SetField("topmostSubform[0].Page1[0].f_11[0]", request.ReferenceNumberS);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f_12[0]", request.DateOfBirthMmDdYyyy);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f_13[0]", request.Country);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f_14[0]", request.ArticleAndParagraph);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f_15[0]", request.Rate);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f_16[0]", request.SpecifyTypeOfIncome);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f_17[0]", request.EligibleForTheRateOfWithholding);
            //pdfFormFields.SetField("topmostSubform[0].Page1[0].Date[0]", DateTime.Now.ToString("MM-dd-yyyy"));
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f_21[0]", request.PrintNameOfSigner);
            string sTmp = "W-8 BEN Completed for " + request.NameOfIndividual + " " + request.Ssnitnein;
            // flatten the form To remove editting options, set it to false  
            // to leave the form open to subsequent manual edits  
            pdfStamper.FormFlattening = true;
            // close the pdf  
            //      pdfStamper.Close();

            // I certify check
            pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_4[0]", "1");

            PdfContentByte overContent = pdfStamper.GetOverContent(1);
            Rectangle rectangle = new Rectangle(130, 75, 380, 97, 0);
            rectangle.BackgroundColor = BaseColor.LIGHT_GRAY;
            overContent.Rectangle(rectangle);
            //for date
            PdfContentByte overContent1 = pdfStamper.GetOverContent(1);
            iTextSharp.text.Rectangle rectangle1 = new iTextSharp.text.Rectangle(460, 75, 510, 90, 0);
            rectangle1.BackgroundColor = BaseColor.LIGHT_GRAY;
            overContent1.Rectangle(rectangle1);
            PdfAnnotation annotation1;

            bool IsDate = true;
            string methodName = "Index?IsDate=" + IsDate.ToString();
            annotation1 = PdfAnnotation.CreateLink(pdfStamper.Writer, rectangle1, PdfAnnotation.HIGHLIGHT_INVERT, new PdfAction(Path.Combine(request.Host, "Certification", methodName)));
            //annotation1 = PdfAnnotation.CreateLink(pdfStamper.Writer, rectangle1, PdfAnnotation.HIGHLIGHT_INVERT, new PdfAction(Path.Combine(request.Host, "Certification", "Index")));
            pdfStamper.AddAnnotation(annotation1, 1);

            var src1 = Path.Combine(Directory.GetCurrentDirectory(), "signature-image.png");
            Image image1 = Image.GetInstance(src1);
            PdfImage stream1 = new PdfImage(image1, "", null);
            stream1.Put(new PdfName("ITXT_SpecialId"), new PdfName("123456789"));
            PdfIndirectObject ref1 = pdfStamper.Writer.AddToBody(stream1);
            image1.SetAbsolutePosition(145, 75);
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
            image2.SetAbsolutePosition(450, 75);
            PdfContentByte over2 = pdfStamper.GetOverContent(1);
            over2.AddImage(image2);
            #endregion
            PdfAnnotation annotation;

            annotation = PdfAnnotation.CreateLink(pdfStamper.Writer, rectangle, PdfAnnotation.HIGHLIGHT_INVERT, new PdfAction(Path.Combine(request.Host, "Certification", "Index")));
            pdfStamper.AddAnnotation(annotation, 1);

            pdfStamper.Close();
            pdfReader.Close();
            //var dd = File.ReadAllBytes(newFile);
            //byte[] pngByte = Freeware.Pdf2Png.Convert(dd, 1);
            //File.WriteAllBytes(Path.Combine(request.BasePath, "dd.png"), pngByte);
            //Response.Redirect("~/" + GlobalVariables.Globals.Uploaded_File1);
            return fileName;
        }
        public async Task<bool> UpdateByClientEmailId(string ClientEmail, PdfFormDetailsRequest request)
        {
            var response = _evolvedtaxContext.TblW8benforms.Where(p => p.W8benemailAddress == ClientEmail).FirstOrDefault();
            response.UploadedFile = request.FileName;
            response.W8benentryDate = request.EntryDate;
            response.Status = "3";
            response.W8benfontName = request.FontFamily;
            response.W8benprintName = request.PrintName;
            response.W8benprintSize = Convert.ToInt32(request.FontSize);
            _evolvedtaxContext.TblW8benforms.Update(response);
            await _evolvedtaxContext.SaveChangesAsync();
            return true;
        }

        public FormRequest GetDataByClientEmailId(string ClientEmailId)
        {
            var gQuestionData = _generalQuestionareService.GetDataByClientEmail(ClientEmailId);
            var w8BenData = _evolvedtaxContext.TblW8benforms.FirstOrDefault(p => p.W8benemailAddress == ClientEmailId);
            gQuestionData.NameOfIndividual = w8BenData?.NameOfIndividual;
            gQuestionData.ArticleAndParagraph = w8BenData?.ArticleAndParagraph;
            gQuestionData.CheckIfFtinNotLegallyRequiredYN = w8BenData?.CheckIfFtinNotLegallyRequiredYN;
            gQuestionData.DateOfBirthMmDdYyyy = w8BenData?.DateOfBirthMmDdYyyy;
            gQuestionData.ForeignTaxIdentifyingNumber = w8BenData?.ForeignTaxIdentifyingNumber;
            gQuestionData.Rate = w8BenData?.Rate;
            gQuestionData.PrintNameOfSigner = w8BenData?.PrintNameOfSigner;
            gQuestionData.ReferenceNumberS = w8BenData?.ReferenceNumberS;
            gQuestionData.ResidentCertification = w8BenData?.ResidentCertification;
            gQuestionData.SignatureDateMmDdYyyy = w8BenData?.SignatureDateMmDdYyyy;
            gQuestionData.SpecifyTypeOfIncome = w8BenData?.SpecifyTypeOfIncome;
            gQuestionData.Ssnitnein = w8BenData?.SsnOrItin;
            gQuestionData.EmailId = w8BenData?.W8benemailAddress ?? ClientEmailId;
            return gQuestionData;
        }
        public string Update(FormRequest request)
        {
            var response = _evolvedtaxContext.TblW8benforms.First(p => p.W8benemailAddress == request.EmailId);
            response.NameOfIndividual = request.NameOfIndividual;
            response.City = string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode);
            response.ArticleAndParagraph = request.ArticleAndParagraph;
            response.CheckIfFtinNotLegallyRequiredYN = request.CheckIfFtinNotLegallyRequiredYN;
            response.Country = request.PCountry;
            response.CountryOfCitizenship = request.CountryOfCitizenship;
            response.DateOfBirthMmDdYyyy = request.DateOfBirthMmDdYyyy;
            response.ForeignTaxIdentifyingNumber = request.ForeignTaxIdentifyingNumber?.Replace("&nbsp;", "");
            response.MailingAddress = string.Concat(request.MAddress1, " ", request.MAddress2);
            response.MCity = string.Concat(request.PCity, ", ", request.PState ?? request.PProvince, ", ", request.PZipCode);
            response.MCountry = request.MCountry;
            response.PermanentResidenceAddress = string.Concat(request.PAddress1, " ", request.PAddress2);
            response.Rate = request.Rate;
            response.PrintNameOfSigner = request.PrintNameOfSigner;
            response.ReferenceNumberS = request.ReferenceNumberS;
            response.ResidentCertification = request.Country;
            response.SignatureDateMmDdYyyy = DateTime.Now.ToString("MM-dd-yyyy");
            response.SpecifyTypeOfIncome = request.SpecifyTypeOfIncome;
            response.SsnOrItin = request.Ssnitnein;
            response.Status = "1";
            response.W8benemailAddress = request.EmailId;
            response.W8benonBehalfName = request.W8BENOnBehalfName;

            if (request.EligibleForTheRateOfWithholding != "")
            {
                response.EligibleForTheRateOfWithholding = "1";
                request.EligibleForTheRateOfWithholding = "1";
            }
            else
            {
                response.EligibleForTheRateOfWithholding = "0";
                request.EligibleForTheRateOfWithholding = "0";
            }
            _evolvedtaxContext.TblW8benforms.Update(response);
            _evolvedtaxContext.SaveChanges();
            if (request.IsPartialSave)
            {
                return AppConstants.FormPartiallySave;
            }
            return W8BenCreation(request);
        }
    }
}
