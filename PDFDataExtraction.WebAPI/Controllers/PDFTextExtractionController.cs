using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PDFDataExtraction.Exceptions;
using PDFDataExtraction.Models;

namespace PDFDataExtraction.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PDFTextExtractionController : ControllerBase
    {
        private readonly IPDFToTextWrapper _pdfToTextWrapper;

        public PDFTextExtractionController(IPDFToTextWrapper pdfToTextWrapper)
        {
            _pdfToTextWrapper = pdfToTextWrapper;
        }
        
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] {"value1", "value2"};
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] IFormFile file)
        {
            // TODO Check if actually a PDF file
            
            // Save file to disk
            var fileId = Guid.NewGuid().ToString();
            var inputFilePath = $"./uploaded-files/{fileId}.pdf";

            using (var fileStream = new FileStream(inputFilePath, FileMode.CreateNew, FileAccess.Write))
                await file.CopyToAsync(fileStream);
            
            var result = new PDFTextExtractionResult();

            var extractionArgs = new PDFToTextArgs()
            {
                OutputBoundingBoxLayout = true
            };
            
            // Process file
            try
            {
                var pdfToTextResult = await _pdfToTextWrapper.ExtractTextFromPDF(inputFilePath, extractionArgs);
                result.ExtractedData = pdfToTextResult;

                return new OkObjectResult(pdfToTextResult);
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                return new BadRequestObjectResult(result);
            }
            finally
            {
                try
                {
                    System.IO.File.Delete(inputFilePath);
                }
                catch (IOException e)
                {
                }
            }
        }
    }
}