using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Common.ExtensionMehtods
{
    public static class AppCommonMethods
    {
        public static void GeneratePdfFromHtml(string path, string html)
        {
            try
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                   // html = "<html><body><h1>Hello, World!</h1></body></html>";

                    Document document = new Document();
                    PdfWriter writer = PdfWriter.GetInstance(document, stream);

                    if (document == null)
                    {
                        throw new Exception("Document is null.");
                    }

                    document.Open();

                    if (writer == null)
                    {
                        throw new Exception("PdfWriter is null.");
                    }

                    if (string.IsNullOrEmpty(html))
                    {
                        throw new Exception("HTML string is null or empty.");
                    }

                    using (var sr = new StringReader(html))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, sr);
                    }

                    document.Close();
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine("Error: " + ex.Message);
            }

        }
    }
}
