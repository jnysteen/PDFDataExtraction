using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dasync.Collections;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using PdfDataExtraction.Grpc;

namespace PDFDataExtraction.GrpcService.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // HACK to accepted unencrypted traffic to the GRPC service
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            
            var httpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }; // Return `true` to allow certificates that are untrusted/invalid
            var channel = GrpcChannel.ForAddress("http://localhost:5001", new GrpcChannelOptions
            {
                HttpHandler = httpHandler,
                Credentials = ChannelCredentials.Insecure,
                MaxReceiveMessageSize = int.MaxValue,
                MaxSendMessageSize = int.MaxValue,
            });
            var client = new PdfDataExtractionGrpcService.PdfDataExtractionGrpcServiceClient(channel);

            var inputPath = args[0];

            var files = Directory.GetFiles(inputPath, "*.pdf", SearchOption.AllDirectories);

            Console.WriteLine($"Found {files.Length} PDF files, processing them now");
            
            await files.ParallelForEachAsync(async inputFilePath =>
                {
                    var fileExtension = Path.GetExtension(inputFilePath);
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputFilePath);
                    
                    try
                    {
                        await using var fileStream = File.OpenRead(inputFilePath);
            
                        var pdfDataExtractionRequest = new PDFDataExtractionGrpcRequest()
                        {
                            FileContents = await ByteString.FromStreamAsync(fileStream),
                            FileExtension = fileExtension,
                            FileName = fileNameWithoutExtension,
                            ExtractionParameters = new ExtractionParameters()
                            {
                                WhiteSpaceFactor = 0.2,
                                WordLineDiff = 0.15
                            },
                            MaxProcessingTimeInMilliseconds = 5000,
                            ConvertPdfToImages = false
                        };
                    
                        var stopwatch = new Stopwatch();
                        stopwatch.Start();
                        var reply = await client.ExtractDetailedAsync(pdfDataExtractionRequest);
                        stopwatch.Stop();
                        if(reply.ExtractedDocument == null)
                            throw new Exception();
                        Console.WriteLine(fileNameWithoutExtension + $" extracted in {stopwatch.ElapsedMilliseconds} ms");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Failed on doc {inputFilePath}");
                    }
                }, maxDegreeOfParallelism: 1);
        }
        
                
        public static string GetAsString(Document document)
        {
            return string.Join("\n", document.Pages.Select(GetAsString));
        }
        
        public static string GetAsString(Document.Types.Page page)
        {
            return string.Join("\n", page.Lines.Select(GetAsString));
        }
        
        public static string GetAsString(Document.Types.Page.Types.Line line)
        {
            return string.Join(" ", line.Words.Select(GetAsString));
        }
        
        public static string GetAsString(Document.Types.Page.Types.Line.Types.Word word)
        {
            return string.Join("", word.Characters.Select(c => c.Text));
        }
    }
}