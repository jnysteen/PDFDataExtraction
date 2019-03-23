using System;
using System.IO;
using System.Threading.Tasks;

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

                if (!File.Exists(formattedInputFilePath))
                {
                    Console.WriteLine($"File at provided path '{rawInputFilePath}' does not exist!");
                    continue;
                }

                var inputFileDirectory = Path.GetDirectoryName(formattedInputFilePath);
                var inputFileName = Path.GetFileNameWithoutExtension(formattedInputFilePath);
                var outputFilePath = $"{inputFileDirectory}\\{inputFileName}-extracted-data.txt";

                using (var inputFileStream = File.OpenRead(formattedInputFilePath))
                {
                    Console.WriteLine("Sending request to API now...");
                    var extractedText = await pdfExtractionClient.ExtractTextFromPDF(inputFileStream);

                    Console.WriteLine($"Response received, saving result to '{outputFilePath}' now...");
                    File.WriteAllText(outputFilePath, extractedText);
                }
            }
        }
    }
}