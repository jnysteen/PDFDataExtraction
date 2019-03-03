using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public PDFToTextController(IPDFToTextWrapper pdfToTextWrapper)
        {
            _pdfToTextWrapper = pdfToTextWrapper;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            return new OkObjectResult("Test");
        }
        
        [HttpPost("simple")]
        public async Task<IActionResult> SimpleExtraction([FromForm] IFormFile file)
        {
            // TODO Check if actually a PDF file

            var pdfToTextArgs = new PDFToTextArgs();

            var result = new PDFToTextResult();
            
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
            // TODO Check if actually a PDF file

            var pdfToTextArgs = new PDFToTextArgs()
            {
                OutputBoundingBox = true,
                OutputBoundingBoxLayout = true
            };

            var result = new PDFToTextResult();
            
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