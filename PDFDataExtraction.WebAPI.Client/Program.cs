using System;
using System.IO;
using System.Threading.Tasks;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

namespace PDFDataExtraction.WebAPI.Client
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting PDF Data Extraction Client Generator...");
            var inputSwaggerJsonFile = args[0];
            var json = File.ReadAllText(inputSwaggerJsonFile);
            var schema = await JsonSchema.FromJsonAsync(json);
            
            var errors = schema.Validate(json);

            foreach (var error in errors)
                Console.WriteLine(error.Path + ": " + error.Kind);
            
            var generator = new CSharpGenerator(schema);
            var generatedFile = generator.GenerateFile();

            var outputDirectory = Path.GetDirectoryName(inputSwaggerJsonFile);
            var outputFileName = "PdfDataExtractionClient.cs";
            var outputFilePath = Path.Combine(outputDirectory, outputFileName);
            File.WriteAllText(outputFilePath, generatedFile);
            
            Console.WriteLine("Client successfully generated");
        }
    }
}