using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dasync.Collections;
using Google.Protobuf;
using Grpc.Net.Client;

namespace PDFDataExtraction.GrpcService.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var httpHandler = new HttpClientHandler(); // Return `true` to allow certificates that are untrusted/invalid
            httpHandler.ServerCertificateCustomValidationCallback = 
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            var channel = GrpcChannel.ForAddress("https://localhost:5001",
                new GrpcChannelOptions { HttpHandler = httpHandler });
            var client = new Pdfdataextraction.PdfdataextractionClient(channel);

            var inputFilePath = "/home/jesper/Documents/git/PDFDataExtraction/PDFDataExtraction.Tests/Common/TestFiles/simple-two-page-doc.pdf";
            var fileExtension = Path.GetExtension(inputFilePath);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputFilePath);
            await using var fileStream = File.OpenRead(inputFilePath);
            
            var pdfDataExtractionRequest = new PDFDataExtractionRequest()
            {
                FileContents = await ByteString.FromStreamAsync(fileStream),
                FileExtension = fileExtension,
                FileName = fileNameWithoutExtension
            };
            
            var messages = Enumerable.Range(1, 100);
            await messages.ParallelForEachAsync(async msg =>
                {
                    var reply = await client.ExtractDetailedAsync(pdfDataExtractionRequest);
                    Console.WriteLine(msg + ": " + reply.ExtractedDocument.Pages.Count);
                }, maxDegreeOfParallelism: 10);
        }
    }
}