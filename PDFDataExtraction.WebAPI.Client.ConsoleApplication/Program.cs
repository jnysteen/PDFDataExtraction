using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PdfDataExtraction;
using PdfDataExtraction.WebAPI.Client;

namespace PDFDataExtraction.WebAPI.Client.ConsoleApplication
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var apiEndpoint = "http://localhost:4000";
            using var httpClient = new HttpClient();
            var pdfExtractionClient = new PdfDataExtractionClient(apiEndpoint, httpClient);

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
                
                

                // await using (var inputFileStream = File.Open(formattedInputFilePath, FileMode.Open))
                var t = File.ReadAllBytes(formattedInputFilePath);
                using var ms = new MemoryStream(t);
                {
                    Console.WriteLine("Sending request to API now...");
                    
                    var fileParameter = new FileParameter(ms);

                    var extractedDocument = await pdfExtractionClient.DetailedTextExtractionAsync(true, null, null, fileParameter);

                    var extractedTextSimple = GetAsString(extractedDocument.ExtractedData);
                    Console.WriteLine(extractedTextSimple);

                    var extractedText = JsonConvert.SerializeObject(extractedDocument, Formatting.Indented);
                    // var extractedText = await pdfExtractionClient.ExtractTextFromPDF(inputFileStream);
                    
                    Console.WriteLine($"Response received, saving result to '{outputFilePath}' now...");
                    File.WriteAllText(outputFilePath, extractedText);
                    File.WriteAllText(outputFilePathSimple, extractedTextSimple);

                    if (extractedDocument.PagesAsPNGs != null)
                    {
                        foreach (var pagesAsPnG in extractedDocument.PagesAsPNGs)
                        {
                            var bytes = pagesAsPnG.Contents;
                            var fileName = $"{outputImageFileBase}-{pagesAsPnG.PageNumber}.png";
                            File.WriteAllBytes(fileName, bytes);
                        }
                    }
                }
            }
        }

        public static string GetAsString(Document document)
        {
            return string.Join("\n", document.Pages.Select(GetAsString));
        }
        
        public static string GetAsString(Page page)
        {
            return string.Join("\n", page.Lines.Select(GetAsString));
        }
        
        public static string GetAsString(Line line)
        {
            return string.Join(" ", line.Words.Select(GetAsString));
        }
        
        public static string GetAsString(Word word)
        {
            return string.Join("", word.Chars.Select(c => c.Text));
        }
    }
}