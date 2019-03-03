using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PDFDataExtraction.Exceptions;
using PDFDataExtraction.Models;

// ReSharper disable StringLiteralTypo

namespace PDFDataExtraction
{
    public class PDFToTextWrapper : IPDFToTextWrapper
    {
        public async Task<string> ExtractTextFromPDF(string inputFilePath, PDFToTextArgs pdfToTextArgs)
        {
            var otherArgsAsString = pdfToTextArgs.GetArgsAsString();

            var outputFilePath = "-"; // If the output file is "-", the output is redirected to stdout
            
            var cmd = $" {otherArgsAsString} {inputFilePath} {outputFilePath}";
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "pdftotext",
                    Arguments = escapedArgs,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            
            process.Start();
            var pdfToTextOutput = await process.StandardOutput.ReadToEndAsync();
            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new PDFToTextException($"pdftotext exited with status code: {process.ExitCode}");

            return pdfToTextOutput;
        }
        
        public async Task<Models.PDFToTextDocumentBoundingBoxLayout.Html> ExtractTextFromPDFBoundingBoxLayout(string inputFilePath, PDFToTextArgs pdfToTextArgs)
        {
            pdfToTextArgs.OutputBoundingBoxLayout = true;
            var extractedXml = await ExtractTextFromPDF(inputFilePath, pdfToTextArgs);
            
            var xmlSerializer = new XmlSerializer(typeof(Models.PDFToTextDocumentBoundingBoxLayout.Html));

            using (var reader = new StringReader(extractedXml))
            {
                var deserializedDoc = (Models.PDFToTextDocumentBoundingBoxLayout.Html) xmlSerializer.Deserialize(reader);
                return deserializedDoc;
            }
        }
        
        public async Task<Models.PDFToTextDocumentBoundingBox.Html> ExtractTextFromPDFBoundingBox(string inputFilePath, PDFToTextArgs pdfToTextArgs)
        {
            pdfToTextArgs.OutputBoundingBox = true;
            var extractedXml = await ExtractTextFromPDF(inputFilePath, pdfToTextArgs);
            
            var xmlSerializer = new XmlSerializer(typeof(Models.PDFToTextDocumentBoundingBox.Html));

            using (var reader = new StringReader(extractedXml))
            {
                var deserializedDoc = (Models.PDFToTextDocumentBoundingBox.Html) xmlSerializer.Deserialize(reader);
                return deserializedDoc;
            }
        }
    }
}