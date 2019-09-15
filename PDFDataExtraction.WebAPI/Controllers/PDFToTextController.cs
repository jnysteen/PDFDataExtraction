using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JNysteen.FileTypeIdentifier.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PDFDataExtraction.Exceptions;
using PDFDataExtraction.PDFToText;
using PDFDataExtraction.PDFToText.Models;
using PDFDataExtraction.WebAPI.Helpers;
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
        [ServiceFilter(typeof(ValidateInputPDFAttribute))]
        public async Task<IActionResult> SimpleExtraction(IFormFile file)
        {
            var pdfToTextArgs = new PDFToTextArgs(); //TODO make it possible to pass args as query parameters
            Func<string, PDFToTextArgs, Task<string>> extractor = _pdfToTextWrapper.ExtractTextFromPDF;
            
            var result = new PDFToTextResult();
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
        [ServiceFilter(typeof(ValidateInputPDFAttribute))]
        public async Task<IActionResult> DetailedExtraction(IFormFile file)
        {
            var pdfToTextArgs = new PDFToTextArgs() //TODO make it possible to pass args as query parameters
            {
                OutputBoundingBox = true,
                OutputBoundingBoxLayout = true
            };

            Func<string, PDFToTextArgs, Task<string>> extractor = _pdfToTextWrapper.ExtractTextFromPDF;
            
            var result = new PDFToTextResult();
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