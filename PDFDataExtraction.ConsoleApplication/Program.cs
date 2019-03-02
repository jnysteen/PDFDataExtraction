using System;
using System.IO;
using System.Xml.Serialization;
using PDFDataExtraction.Models;

namespace PDFDataExtraction.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputFile = args[0];

            var pdfToTextService = new PDFToTextWrapper(".");

            Console.WriteLine($"Extracting text from PDF...");

            var inputFileWithoutExtension = Path.GetFileNameWithoutExtension(inputFile);

            var outputFile = $"{inputFileWithoutExtension}-extracted-text.xml";

            pdfToTextService.PDFToText(inputFile, outputFile, new string[] { "-bbox-layout" });


            Console.WriteLine($"Reading XML file {outputFile}");

            XmlSerializer serializer = new XmlSerializer(typeof(Html));

            using (var reader = new StreamReader(outputFile))
            {
                var deserializedDoc = (Html)serializer.Deserialize(reader);
                PrintDocument(deserializedDoc);
            }
        }

        static void PrintDocument(Html html)
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