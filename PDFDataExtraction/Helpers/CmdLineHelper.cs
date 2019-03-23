using System.Diagnostics;
using System.Threading.Tasks;
using PDFDataExtraction.Exceptions;

namespace PDFDataExtraction.Helpers
{
    public static class CmdLineHelper
    {
        public static async Task<(int exitCode, string stdOutput)> Run(string applicationName, string args)
        {
            var escapedArgs = args.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = applicationName,
                    Arguments = escapedArgs,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            var pdfToTextOutput = await process.StandardOutput.ReadToEndAsync();
            process.WaitForExit();

            return (process.ExitCode, pdfToTextOutput);
        }
    }
}