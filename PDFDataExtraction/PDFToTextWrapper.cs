using System;
using System.Diagnostics;
// ReSharper disable StringLiteralTypo

namespace PDFDataExtraction
{
    public class PDFToTextWrapper
    {
        private string _workingDirectory;

        public PDFToTextWrapper(string workingDirectory)
        {
            _workingDirectory = workingDirectory;
        }

        public string PDFToText(string inputFilePath, string outputFilePath, string[] otherArgs)
        {
            var otherArgsAsString = string.Join(" ", otherArgs);

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
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
    }
}