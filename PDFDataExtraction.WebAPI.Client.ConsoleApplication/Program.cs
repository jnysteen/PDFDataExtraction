using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PDFDataExtraction.WebAPI.Client.ConsoleApplication
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var apiEndpoint = "http://localhost:6000";
            var pdfExtractionClient = new PDFDataExtractionClient(apiEndpoint);

            while (true)
            {
                Console.WriteLine("Input PDF file path (or empty input to exit the application)");
                var rawInputFilePath = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(rawInputFilePath))
                    break;

                var formattedInputFilePath = rawInputFilePath.StartsWith("\"") && rawInputFilePath.EndsWith("\"")
                    ? rawInputFilePath.Substring(1, rawInputFilePath.Length - 2) : rawInputFilePath;

                formattedInputFilePath = formattedInputFilePath.Trim();

                if (!File.Exists(formattedInputFilePath))
                {
                    Console.WriteLine($"File at provided path '{rawInputFilePath}' does not exist!");
                    continue;
                }

                var inputFileDirectory = Path.GetDirectoryName(formattedInputFilePath);
                var inputFileName = Path.GetFileNameWithoutExtension(formattedInputFilePath);

                var outputBase = Path.Combine(inputFileDirectory, inputFileName);
                var outputFilePath = $"{outputBase}-extracted-data.json";
                var outputFilePathSimple = $"{outputBase}-extracted-data.simple.txt";
                var outputImageFileBase = $"{outputBase}-extracted-data";

                using (var inputFileStream = File.OpenRead(formattedInputFilePath))
                {
                    Console.WriteLine("Sending request to API now...");

                    var extractedDocument = await pdfExtractionClient.ExtractDocumentFromPDF(inputFileStream);

                    var extractedTextSimple = extractedDocument.ExtractedData.GetAsString();
                    Console.WriteLine(extractedTextSimple);

                    var extractedText = JsonConvert.SerializeObject(extractedDocument, Formatting.Indented);
                    // var extractedText = await pdfExtractionClient.ExtractTextFromPDF(inputFileStream);
                    
                    Console.WriteLine($"Response received, saving result to '{outputFilePath}' now...");
                    File.WriteAllText(outputFilePath, extractedText);
                    File.WriteAllText(outputFilePathSimple, extractedTextSimple);

                    foreach (var pagesAsPnG in extractedDocument.PagesAsPNGs)
                    {
                        var bytes = System.Convert.FromBase64String(pagesAsPnG.Base64EncodedContents);
                        var fileName = $"{outputImageFileBase}-{pagesAsPnG.PageNumber}.png";
                        File.WriteAllBytes(fileName, bytes);
                    }
                }
            }
        }
    }
}