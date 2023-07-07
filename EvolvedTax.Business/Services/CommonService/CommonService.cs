using EvolvedTax.Data.Models.Entities;
using iTextSharp.text.pdf;
using iTextSharp.text;
using EvolvedTax.Data.Models.DTOs.Request;
using SkiaSharp;
using EvolvedTax.Common.Constants;
using Microsoft.AspNetCore.Http;

namespace EvolvedTax.Business.Services.CommonService
{
    public class CommonService : ICommonService
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        private string BaseUrl;
        private readonly HttpContext _httpContext;
        public CommonService(EvolvedtaxContext evolvedtaxContext, IHttpContextAccessor httpContextAccessor)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _httpContext = httpContextAccessor.HttpContext;
            BaseUrl = _httpContext.Session.GetString("BaseURL") ?? string.Empty;
        }
        public string AssignSignature(PdfFormDetailsRequest request, string filePath)
        {
            string imageFilePath = Path.Combine(Directory.GetCurrentDirectory(), "pictureText.bmp");

            // Load the image file using SkiaSharp
            using (SKBitmap bitmap = SKBitmap.Decode(imageFilePath))
            {
                using (SKCanvas canvas = new SKCanvas(bitmap))
                {
                    using (SKPaint paint = new SKPaint())
                    {
                        paint.Color = SKColors.Black;
                        paint.TextSize = (float)Convert.ToDecimal(request.FontSize);

                        string text = request.Text;
                        paint.Typeface = SKTypeface.FromFamilyName(request.FontFamily);
                        paint.IsAntialias = true; // Enable anti-aliasing
                        SKRect textBounds = new SKRect();
                        paint.MeasureText(text, ref textBounds);

                        // Calculate the position to center the text horizontally and align it vertically in the middle
                        float x = (bitmap.Width - textBounds.Width) / 2;
                        float y = (bitmap.Height - textBounds.Height) / 2 + textBounds.Height;

                        // Draw the text at the calculated position
                        canvas.DrawText(text, x, y, paint);
                    }
                }

                // Save the modified image as PNG
                using (SKData encoded = bitmap.Encode(SKEncodedImageFormat.Png, 100))
                {
                    string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "picture1.png");
                    File.WriteAllBytes(outputPath, encoded.ToArray());
                }
            }
            // Load the image file using SkiaSharp
            using (SKBitmap bitmap = SKBitmap.Decode(imageFilePath))
            {
                using (SKCanvas canvas = new SKCanvas(bitmap))
                {
                    using (SKPaint paint = new SKPaint())
                    {
                        paint.Color = SKColors.Black;
                        paint.TextSize = 9;

                        string text = request.EntryDate?.ToString("MM-dd-yyyy") ?? string.Empty;
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
                    string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "picture2.png");
                    File.WriteAllBytes(outputPath, encoded.ToArray());
                }
            }
            string newFile = Path.Combine(request.BaseUrl, filePath);
            string fileName = filePath.Replace("_temp", "");

            using (PdfReader pdfReader = new PdfReader(newFile))
            {
                int numberOfPages = pdfReader.NumberOfPages;
        
                using (PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(Path.Combine(request.BaseUrl, fileName), FileMode.Create)))
                {
                    AcroFields pdfFormFields = pdfStamper.AcroFields;
                    string src1 = Path.Combine(Directory.GetCurrentDirectory(), "picture1.png");
                    Image image1 = Image.GetInstance(src1);
                    PdfImage stream1 = new PdfImage(image1, "", null);
                    stream1.Put(new PdfName("ITXT_SpecialId"), new PdfName("123456789"));
                    PdfIndirectObject ref1 = pdfStamper.Writer.AddToBody(stream1);

                    string src2 = Path.Combine(Directory.GetCurrentDirectory(), "picture2.png");
                    Image image2 = Image.GetInstance(src2);
                    PdfImage stream2 = new PdfImage(image2, "", null);
                    stream2.Put(new PdfName("ITXT_SpecialId"), new PdfName("1234567"));
                    PdfIndirectObject ref2 = pdfStamper.Writer.AddToBody(stream2);

                    if (AppConstants.W8BENForm == request.FormName)
                    {
                        image1.SetAbsolutePosition(147, 76);
                        if (request.EntryDate != null)
                        {
                            image2.SetAbsolutePosition(450, 75);
                        }
                    }
                    else if (AppConstants.W8ECIForm == request.FormName)
                    {
                        image1.SetAbsolutePosition(132, 51);
                        if (request.EntryDate != null)
                        {
                            image2.SetAbsolutePosition(485, 51);
                        }
                    }
                    else if (AppConstants.W9Form == request.FormName)
                    {
                        image1.SetAbsolutePosition(147, 230);
                        if (request.EntryDate != null)
                        {
                            image2.SetAbsolutePosition(430, 230);
                        }
                    }
                    else if (AppConstants.W8EXPForm == request.FormName)
                        {
                        image1.SetAbsolutePosition(72, 530);
                        if (request.EntryDate != null)
                        {
                            image2.SetAbsolutePosition(450, 530);
                        }
                    }
                    else if (AppConstants.W8IMYForm == request.FormName)
                    {
                        image1.SetAbsolutePosition(107, 185);
                        if (request.EntryDate != null)
                        {
                            image2.SetAbsolutePosition(495, 185);
                        }
                    }

                    PdfContentByte over1 = pdfStamper.GetOverContent(numberOfPages);
                    over1.AddImage(image1);
                    if (request.EntryDate != null)
                    {
                        PdfContentByte over2 = pdfStamper.GetOverContent(numberOfPages);
                        over1.AddImage(image2);
                    }
                    // Flatten the form fields to apply the changes
                    pdfStamper.FormFlattening = true;

                    pdfStamper.Close();
                }

                pdfReader.Close();
            }
            return fileName;
        }


        public string RemoveAnnotations(string filePath)
        {
            string inputFilePath = Path.Combine(BaseUrl, filePath);
            string outputFileName = Path.GetFileNameWithoutExtension(filePath) + "_new" + Path.GetExtension(filePath);
            string outputFilePath = Path.Combine(Path.GetDirectoryName(inputFilePath), outputFileName);

            // Load the PDF document
            using (PdfReader reader = new PdfReader(inputFilePath))
            {
                // Iterate through each page of the document
                for (int pageNum = 1; pageNum <= reader.NumberOfPages; pageNum++)
                {
                    // Get the page dictionary
                    PdfDictionary pageDict = reader.GetPageN(pageNum);

                    // Remove the annotations from the page
                    pageDict.Remove(PdfName.ANNOTS);
                }

                // Save the modified PDF document without annotations
                using (FileStream outputStream = new FileStream(outputFilePath, FileMode.Create))
                {
                    using (PdfStamper stamper = new PdfStamper(reader, outputStream))
                    {
                        // No need to flatten the form fields since we are only removing annotations
                    }
                }
            }

            return outputFileName;
        }

        //public string RemoveAnnotations (string filePath)
        //{

        //    string FilePath = Path.Combine(BaseUrl, filePath);
        //    // Load the PDF document
        //    using (PdfReader reader = new PdfReader(FilePath))
        //    {
        //        // Iterate through each page of the document
        //        for (int pageNum = 1; pageNum <= reader.NumberOfPages; pageNum++)
        //        {
        //            // Get the page dictionary
        //            PdfDictionary pageDict = reader.GetPageN(pageNum);

        //            // Remove the annotations from the page
        //            pageDict.Remove(PdfName.ANNOTS);
        //        }



        //        // Save the modified PDF document without annotations
        //        string outputFilePath = Path.Combine(Path.GetDirectoryName(FilePath), New_FileName);
        //        using (FileStream outputStream = new FileStream(outputFilePath, FileMode.Create))
        //        {
        //            using (PdfStamper stamper = new PdfStamper(reader, outputStream))
        //            {
        //                // No need to flatten the form fields since we are only removing annotations
        //            }
        //        }
        //        return outputFilePath;

        //            /*
        //            using (FileStream fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
        //            {
        //                // Forcefully close the file
        //                fileStream.Close();
        //            }

        //            // Overwrite the original PDF file with the modified PDF without annotations
        //            try
        //            {
        //                using (FileStream outputStream = new FileStream(FilePath, FileMode.Create))
        //                {
        //                    using (PdfStamper stamper = new PdfStamper(reader, outputStream))
        //                    {
        //                        // No need to flatten the form fields since we are only removing annotations
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                // Log or handle the exception
        //                Console.WriteLine($"An error occurred while removing annotations: {ex.Message}");
        //            }

        //            */
        //        }
        //    }
        public MemoryStream DownloadFile(string filePath)
        {
            var memoryStream = new MemoryStream();
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                fileStream.CopyTo(memoryStream);
            }
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
