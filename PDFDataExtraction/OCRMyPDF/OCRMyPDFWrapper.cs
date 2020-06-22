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
            {
                if(Enum.TryParse<OCRMyPDFErrorCode>(statusCode.ToString(), out var mappedErrorCode))
                    throw new PDFTextExtractionException($"{applicationName} exited with status code: {mappedErrorCode.ToString()}");
                throw new PDFTextExtractionException($"{applicationName} exited with status code: {statusCode}");
            }
        } 
    }

    /// <summary>
    ///     Codes have been taken from the OCRMyPDF docs: https://ocrmypdf.readthedocs.io/en/latest/advanced.html#return-code-policy
    /// </summary>
    public enum OCRMyPDFErrorCode
    {
        /// <summary>
        ///     Everything worked as expected.
        /// </summary>
        ok = 0,
        /// <summary>
        ///     Invalid arguments, exited with an error.
        /// </summary>
        bad_args = 1,
        /// <summary>
        ///     The input file does not seem to be a valid PDF.
        /// </summary>
        input_file = 2,
        /// <summary>
        ///     An external program required by OCRmyPDF is missing.
        /// </summary>
        missing_dependency = 3,
        /// <summary>
        ///     An output file was created, but it does not seem to be a valid PDF. The file will be available.
        /// </summary>
        invalid_output_pdf = 4,
        /// <summary>
        ///     The user running OCRmyPDF does not have sufficient permissions to read the input file and write the output file.
        /// </summary>
        file_access_error = 5,
        /// <summary>
        ///     The file already appears to contain text so it may not need OCR. See output message.
        /// </summary>
        already_done_ocr = 6,
        /// <summary>
        ///     An error occurred in an external program (child process) and OCRmyPDF cannot continue.
        /// </summary>
        child_process_error = 7,
        /// <summary>
        ///     The input PDF is encrypted. OCRmyPDF does not read encrypted PDFs. Use another program such as qpdf to remove encryption.
        /// </summary>
        encrypted_pdf = 8,
        /// <summary>
        ///     A custom configuration file was forwarded to Tesseract using --tesseract-config, and Tesseract rejected this file.
        /// </summary>
        invalid_config = 9,
        /// <summary>
        ///     A valid PDF was created, PDF/A conversion failed. The file will be available.
        /// </summary>
        pdfa_conversion_failed = 10,
        /// <summary>
        ///     Some other error occurred.
        /// </summary>
        other_error = 15,
        /// <summary>
        ///     The program was interrupted by pressing Ctrl+C.
        /// </summary>
        ctrl_c = 130
    }
}