using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JNysteen.FileTypeIdentifier.Interfaces;
using JNysteen.FileTypeIdentifier.MagicNumbers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PDFDataExtraction.Exceptions;
using PDFDataExtraction.Generic;
using PDFDataExtraction.PDF2Txt;
using PDFDataExtraction.WebAPI.Helpers;
using PDFDataExtraction.WebAPI.Models;

namespace PDFDataExtraction.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PDFTextExtractionController : ControllerBase
    {
        private readonly IPDF2TxtWrapper _pdfTextExtractor;

        public PDFTextExtractionController(IPDF2TxtWrapper pdfTextExtractor)
        {
            _pdfTextExtractor = pdfTextExtractor;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            return new OkObjectResult("Test");
        }
        
        [HttpPost("detailed")]
        [ServiceFilter(typeof(ValidateInputPDFAttribute))]
        [Produces("application/json")]
        public async Task<IActionResult> DetailedExtraction(IFormFile file)
        {
            var result = new PDFTextExtractionResult();
            
            Func<string, Task<Document>> extractor = _pdfTextExtractor.ExtractTextFromPDF;
            
            try
            {
                var extractedText = await ExtractText(file, extractor);
                result.ExtractedData = extractedText;
                return new OkObjectResult(extractedText);
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                return new BadRequestObjectResult(result);
            }
        }
                
        [HttpPost("simple")]
        [ServiceFilter(typeof(ValidateInputPDFAttribute))]
        [Produces("application/json")]
        public async Task<IActionResult> SimpleExtraction(IFormFile file)
        {
            Func<string, Task<Document>> extractor = _pdfTextExtractor.ExtractTextFromPDF;
            
            try
            {
                var extractedText = await ExtractText(file, extractor);
                var documentAsString = extractedText.GetAsString();
                return new OkObjectResult(documentAsString);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }
        
        private static async Task<T> ExtractText<T>(IFormFile file, Func<string, Task<T>> extractor)
        {                
            var fileId = Guid.NewGuid().ToString();
            var inputFilePath = $"./uploaded-files/{fileId}.pdf";

            try
            {
                // Save file to disk
                using (var fileStream = new FileStream(inputFilePath, FileMode.CreateNew, FileAccess.Write))
                    await file.CopyToAsync(fileStream);

                var result = await extractor(inputFilePath);
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