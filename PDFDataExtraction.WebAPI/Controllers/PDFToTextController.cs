using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JNysteen.FileTypeIdentifier.Interfaces;
using JNysteen.FileTypeIdentifier.MagicNumbers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PDFDataExtraction.Exceptions;
using PDFDataExtraction.PDFToText;
using PDFDataExtraction.PDFToText.Models;
using PDFDataExtraction.WebAPI.Models;

namespace PDFDataExtraction.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PDFToTextController : ControllerBase
    {
        private readonly IPDFToTextWrapper _pdfToTextWrapper;
        private readonly IFileTypeIdentifier _fileTypeIdentifier;

        public PDFToTextController(IPDFToTextWrapper pdfToTextWrapper, IFileTypeIdentifier fileTypeIdentifier)
        {
            _pdfToTextWrapper = pdfToTextWrapper;
            _fileTypeIdentifier = fileTypeIdentifier;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            return new OkObjectResult("Test");
        }
        
        [HttpPost("simple")]
        public async Task<IActionResult> SimpleExtraction([FromForm] IFormFile file)
        {
            var result = new PDFToTextResult();
            
            string fileType = null;
            using(var fileStream = file.OpenReadStream())
            {
                fileType = _fileTypeIdentifier.GetFileType(fileStream);
            }

            if (fileType == null || fileType != DocumentMagicNumbers.PDF)
            {
                result.ErrorMessage = "The provided file was not a PDF!";
                return new BadRequestObjectResult(result);
            }

            var pdfToTextArgs = new PDFToTextArgs();

            Func<string, PDFToTextArgs, Task<string>> extractor = _pdfToTextWrapper.ExtractTextFromPDF;
            
            try
            {
                var extractedText = await ExtractText(file, pdfToTextArgs, extractor);
                result.ExtractedData = extractedText;
                return new OkObjectResult(extractedText);
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                return new BadRequestObjectResult(result);
            }
        }
        
        [HttpPost("detailed")]
        public async Task<IActionResult> DetailedExtraction([FromForm] IFormFile file)
        {
            var result = new PDFToTextResult();
            
            string fileType = null;
            using(var fileStream = file.OpenReadStream())
            {
                fileType = _fileTypeIdentifier.GetFileType(fileStream);
            }

            if (fileType == null || fileType != DocumentMagicNumbers.PDF)
            {
                result.ErrorMessage = "The provided file was not a PDF!";
                return new BadRequestObjectResult(result);
            }

            var pdfToTextArgs = new PDFToTextArgs()
            {
                OutputBoundingBox = true,
                OutputBoundingBoxLayout = true
            };

            
            Func<string, PDFToTextArgs, Task<string>> extractor = _pdfToTextWrapper.ExtractTextFromPDF;
            
            try
            {
                var extractedText = await ExtractText(file, pdfToTextArgs, extractor);
                result.ExtractedData = extractedText;
                return new OkObjectResult(extractedText);
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                return new BadRequestObjectResult(result);
            }
        }

        private static async Task<T> ExtractText<T>(IFormFile file, PDFToTextArgs pdfToTextArgs, Func<string, PDFToTextArgs, Task<T>> extractor)
        {                
            var fileId = Guid.NewGuid().ToString();
            var inputFilePath = $"./uploaded-files/{fileId}.pdf";

            try
            {
                // Save file to disk
                using (var fileStream = new FileStream(inputFilePath, FileMode.CreateNew, FileAccess.Write))
                    await file.CopyToAsync(fileStream);

                var result = await extractor(inputFilePath, pdfToTextArgs);
                return result;
            }
            finally
            {
                try
                {
                    System.IO.File.Delete(inputFilePath);
                }
                catch (IOException)
                {
                    
                }
            }
        }
    }
}