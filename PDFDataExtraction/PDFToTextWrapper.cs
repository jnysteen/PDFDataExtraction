using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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

            var cmd = $"pdftotext {otherArgsAsString} {inputFilePath} {outputFilePath}";
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
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
    }

    public interface IPDFToTextWrapper
    {
        Task<string> ExtractTextFromPDF(string inputFilePath, PDFToTextArgs pdfToTextArgs);
    }
}