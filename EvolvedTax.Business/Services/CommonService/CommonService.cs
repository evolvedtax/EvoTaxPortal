using EvolvedTax.Data.Models.Entities;
using iTextSharp.text.pdf;
using iTextSharp.text;
using EvolvedTax.Data.Models.DTOs.Request;
using SkiaSharp;
using EvolvedTax.Common.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Crypto;

namespace EvolvedTax.Business.Services.CommonService
{
    public class CommonService : ICommonService
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        private string BaseUrl;
        private readonly HttpContext _httpContext;
        private readonly IHostingEnvironment _env;

        public CommonService(EvolvedtaxContext evolvedtaxContext, IHttpContextAccessor httpContextAccessor, IHostingEnvironment env=null)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _httpContext = httpContextAccessor.HttpContext;
            BaseUrl = _httpContext.Session.GetString("BaseURL") ?? string.Empty;
            _env = env;
        }
        public string AssignSignature(PdfFormDetailsRequest request, string filePath)
        {
            string imageFilePath = Path.Combine(Directory.GetCurrentDirectory(), "pictureText.bmp");
            string fontsFolderPath = Path.Combine(_env.WebRootPath, "fonts");
            string ext = ".ttf";
            string fontPath = string.Empty;

            int tempIndex = filePath.IndexOf("_temp");
            string result = filePath.Substring(0, tempIndex);
            string newSignFileName = $"{result}.png"; // New file name with .png extension

            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            string newSignDateFileName = $"{result}_{currentDate}.png";


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
                        switch (request.FontFamily)
                        {
                            case AppConstants.F_Family_DancingScript_Bold:
                                fontPath = Path.Combine(fontsFolderPath, AppConstants.F_Family_DancingScript_Bold + ext);
                                break;
                            case AppConstants.F_Family_Yellowtail_Regular:
                                fontPath = Path.Combine(fontsFolderPath, AppConstants.F_Family_Yellowtail_Regular+ext);
                                break;
                            case AppConstants.F_Family_VLADIMIR:
                                fontPath = Path.Combine(fontsFolderPath, AppConstants.F_Family_VLADIMIR+ext);
                                break;
                     
                            case AppConstants.F_Family_SegoeScript:
                                fontPath = Path.Combine(fontsFolderPath, AppConstants.F_Family_SegoeScript + ext);

                                break;

                            case AppConstants.F_Family_Sugar_Garden:
                                fontPath = Path.Combine(fontsFolderPath, AppConstants.F_Family_Sugar_Garden + ext);

                                break;


                            default:
                                break;
                        }

                        if (!string.IsNullOrEmpty(fontPath))
                        {
                            paint.Typeface = SKTypeface.FromFile(fontPath, 0);
                       
                        }
                        
                
                        //paint.Typeface = SKTypeface.FromFamilyName(request.FontFamily);
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
                   

                    // outputPath = Path.Combine(Directory.GetCurrentDirectory(), "picture1.png");
                    string outputPath = Path.Combine(Directory.GetCurrentDirectory(), newSignFileName);
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
                        fontPath = Path.Combine(fontsFolderPath, "Times New Roman.TTF");
                        //paint.Typeface = SKTypeface.FromFamilyName("Times New Roman");
                        paint.Typeface = SKTypeface.FromFile(fontPath, 0);
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
                   // string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "picture2.png");
                    string outputPath = Path.Combine(Directory.GetCurrentDirectory(), newSignDateFileName);
                    File.WriteAllBytes(outputPath, encoded.ToArray());
                }
            }
            string newFile = Path.Combine(request.BaseUrl, filePath);
            string fileName = filePath.Replace("_temp", "");
            string SignImgFilePath = "";

            using (PdfReader pdfReader = new PdfReader(newFile))
            {
                int numberOfPages = pdfReader.NumberOfPages;
        
                using (PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(Path.Combine(request.BaseUrl, fileName), FileMode.Create)))
                {
                    AcroFields pdfFormFields = pdfStamper.AcroFields;
                   // string src1 = Path.Combine(Directory.GetCurrentDirectory(), "picture1.png");
                    string src1 = SignImgFilePath= Path.Combine(Directory.GetCurrentDirectory(), newSignFileName);
                    Image image1 = Image.GetInstance(src1);
                    PdfImage stream1 = new PdfImage(image1, "", null);
                    stream1.Put(new PdfName("ITXT_SpecialId"), new PdfName("123456789"));
                    PdfIndirectObject ref1 = pdfStamper.Writer.AddToBody(stream1);

                  //  string src2 = Path.Combine(Directory.GetCurrentDirectory(), "picture2.png");
                    string src2 = Path.Combine(Directory.GetCurrentDirectory(), newSignDateFileName);
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
                        //image1.SetAbsolutePosition(132, 51);
                        image1.SetAbsolutePosition(122, 51);
                        if (request.EntryDate != null)
                        {
                            image2.SetAbsolutePosition(485, 51);
                        }
                    }
                    else if (AppConstants.W9Form == request.FormName)
                    {
                        image1.SetAbsolutePosition(145, 195);
                        //image1.SetAbsolutePosition(132, 231);
                        if (request.EntryDate != null)
                        {
                            image2.SetAbsolutePosition(430, 195);
                        }

                        PdfContentByte over3= pdfStamper.GetOverContent(1);
                        over3.AddImage(image1);
                        if (request.EntryDate != null)
                        {
                            PdfContentByte over2 = pdfStamper.GetOverContent(1);
                            over3.AddImage(image2);
                        }

                        #region Remove Pages 


                         

                      


                            //string compileFileName;

                          

                            //string outputFilePath = Path.Combine(SaveFolderPath, compileFileName);
                            //CompilepdfPaths.Add(outputFilePath);

                            //// Create a Document object
                            //Document document = new Document();

                            //// Create a PdfCopy object to write the output PDF
                            //PdfCopy pdfCopy = new PdfCopy(document, new FileStream(outputFilePath, FileMode.Create));

                            //// Open the document for writing
                            //document.Open();

                            
                            //    // Open each input PDF file
                            //    PdfReader pdfReader_local = new PdfReader(pdfFilePath);

                            //    // Iterate through the pages of the input PDF and add them to the output PDF
                            //    for (int pageNum = 1; pageNum <= pdfReader_local.NumberOfPages; pageNum++)
                            //    {
                            //        PdfImportedPage page = pdfCopy.GetImportedPage(pdfReader_local, pageNum);
                            //        pdfCopy.AddPage(page);
                            //    }

                            //pdfReader_local.Close();
                            


                            //document.Close();
                            //pdfCopy.Close();
                         

                            #endregion
                        }
                           
                        
                    else if (AppConstants.W8EXPForm == request.FormName)
                        {
                       // image1.SetAbsolutePosition(72, 530);
                        image1.SetAbsolutePosition(62, 530);
                        if (request.EntryDate != null)
                        {
                            image2.SetAbsolutePosition(450, 530);
                        }
                    }
                    else if (AppConstants.W8BENEForm == request.FormName)
                        {
                        //image1.SetAbsolutePosition(120, 110);
                        image1.SetAbsolutePosition(100, 110);
                      
                        if (request.EntryDate != null)
                        {
                            image2.SetAbsolutePosition(470, 110);
                        }
                    }
                    else if (AppConstants.W8IMYForm == request.FormName)
                    {
                        //image1.SetAbsolutePosition(107, 185);
                        image1.SetAbsolutePosition(102, 185);
                        if (request.EntryDate != null)
                        {
                            image2.SetAbsolutePosition(495, 185);
                        }
                    }
                    if (AppConstants.W9Form != request.FormName)
                    {
                        PdfContentByte over1 = pdfStamper.GetOverContent(numberOfPages);
                        over1.AddImage(image1);
                        if (request.EntryDate != null)
                        {
                            PdfContentByte over2 = pdfStamper.GetOverContent(numberOfPages);
                            over1.AddImage(image2);
                        }
                    }
                    // Flatten the form fields to apply the changes
                    pdfStamper.FormFlattening = true;

                    pdfStamper.Close();
                }

                pdfReader.Close();
            }
            //Delete The SignImage from folder 
            File.Delete(SignImgFilePath);
            File.Delete(newSignDateFileName);
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
                    //pageDict.Remove(PdfName.RECT);







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
