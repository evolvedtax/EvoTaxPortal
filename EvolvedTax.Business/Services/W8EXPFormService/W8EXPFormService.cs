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

namespace EvolvedTax.Business.Services.W8EXPFormService
{
    public class W8EXPFormService : IW8EXPFormService
    {
        readonly EvolvedtaxContext _evolvedtaxContext;

        public W8EXPFormService(EvolvedtaxContext evolvedtaxContext)
        {
           
            _evolvedtaxContext = evolvedtaxContext;
        }
        public int Save(FormRequest request)
        {
            var model = new TblW8expform
            {
                NameOfOrganization =  request.GQOrgName,
                TypeOfEntity = request.TypeOfEntity,
                CountryOfIncorporation = request.CountryOfIncorporation,
                FatcaStatus = request.W8EXPFatca,
                _10a = request._10a,
                _10b = request._10b,
                _10bText=request._10b_Text,
                _10c = request._10c,
                _10cText = request._10c_Text,
                _11 = request._11,
                _12 = request._12,
                _13a = request._13a,
                _13b = request._13b,
                _13c = request._13c,
                _13d = request._13d,
                _14 = request._14,
                _16 = request._16,
                _17 = request._17,
                _18 = request._18,
                _19 = request._19,
                _20a = request._20a,
                _20b = request._20b,
                _20c = request._20c,
                _21 = request._21,
                _21Text = request._21_Text,
             
            };

          
            _evolvedtaxContext.TblW8expforms.Add(model);
            _evolvedtaxContext.SaveChanges();
      
            return model.Id;
            //return W9Creation(request);
        }

        protected static string W9Creation(FormRequest request)
        {
            string templatefile = request.TemplateFilePath;
            string newFile1 = string.Empty;
            if (request.IndividualOrEntityStatus == AppConstants.IndividualStatus)
            {
                newFile1 = string.Concat(string.Concat(request.GQFirstName, " ", request.GQLastName).Replace(" ", "_"), "_", "Form_", AppConstants.W9Form, "_", Guid.NewGuid(), "_temp.pdf");
            }
            else
            {
                newFile1 = string.Concat(request.GQOrgName.Replace(" ", "_"), "_", "Form_", AppConstants.W9Form, "_", Guid.NewGuid(), "_temp.pdf");
            }
            string newFile = Path.Combine(request.BasePath, newFile1);

            PdfReader pdfReader = new PdfReader(templatefile);
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;
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
                pdfFormFields.SetField("topmostSubform[0].Page1[0].FederalClassification[0].c1_1[0]", "0");
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
                pdfFormFields.SetField("topmostSubform[0].Page1[0].Exemptions[0].f1_6[0]", request.W8EXPFatca);

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


            pdfStamper.Close();
            pdfReader.Close();
            return newFile1;
        }

    }
}
