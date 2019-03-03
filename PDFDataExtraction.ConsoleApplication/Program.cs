using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PDFDataExtraction.PDFToText;
using PDFDataExtraction.PDFToText.Models;
using PDFDataExtraction.PDFToText.Models.PDFToTextDocumentBoundingBoxLayout;

namespace PDFDataExtraction.ConsoleApplication
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var inputFile = args[0];

            var pdfToTextService = new PDFToTextWrapper();

            Console.WriteLine($"Extracting text from PDF...");

            var pdfToTextArgs = new PDFToTextArgs()
            {
                OutputBoundingBoxLayout = true,
            };
            
            var extractedText = await pdfToTextService.ExtractTextFromPDF(inputFile, pdfToTextArgs);

            Console.WriteLine($"Parsing output to XML...");

            var xmlSerializer = new XmlSerializer(typeof(Html));

            using (var reader = new StringReader(extractedText))
            {
                var deserializedDoc = (Html)xmlSerializer.Deserialize(reader);
                PrintDocument(deserializedDoc);
            }
        }

        private static void PrintDocument(Html html)
        {
            var pages = html.Body.Doc.Pages;

            foreach(var page in pages)
            {
                foreach (var flow in page.Flows)
                {
                    foreach (var block in flow.Blocks)
                    {
                        foreach (var line in block.Lines)
                        {
                            foreach (var word in line.Words)
                            {
                                Console.Write($"{word.Text} ");
                            }
                            Console.WriteLine();
                        }
                    }
                }
            }
        }
    }
}