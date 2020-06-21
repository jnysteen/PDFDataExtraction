using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDFDataExtraction.Exceptions;
using PDFDataExtraction.Helpers;

namespace PDFDataExtraction.OCRMyPDF
{
    public interface IOCRPDFWrapper
    {
        Task OCRPDF(string inputFilePath, string outputFilePath, List<string> languages);
    }

    public class OCRPDFWrapper : IOCRPDFWrapper
    {
        public async Task OCRPDF(string inputFilePath, string outputFilePath, List<string> languages)
        {
            if (inputFilePath == null) throw new ArgumentNullException(nameof(inputFilePath));
            if (outputFilePath == null) throw new ArgumentNullException(nameof(outputFilePath));

            var argsBuilder = new List<string>();
            if (languages?.Any() == true)
                argsBuilder.Add($"-l {string.Join("+", languages)}");
            
            argsBuilder.Add("--skip-text"); // Prevents OCR'ing of any pages that already contains text
            // argsBuilder.Add("--rotate-pages"); // fix misrotated pages
            // argsBuilder.Add("--deskew"); // deskew crooked pages
            
            argsBuilder.Add(inputFilePath);
            argsBuilder.Add(outputFilePath);
            
            var applicationName = "ocrmypdf";
            var args = string.Join(" ", argsBuilder);

            var (statusCode, stdOutput) = await CmdLineHelper.Run(applicationName, args);
            
            if (statusCode != 0)
                throw new PDFTextExtractionException($"{applicationName} exited with status code: {statusCode}");
        } 
    }
}