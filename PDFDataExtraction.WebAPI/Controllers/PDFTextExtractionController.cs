using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PDFDataExtraction.Configuration;
using PDFDataExtraction.Generic;
using PDFDataExtraction.Generic.Models;
using PDFDataExtraction.PdfImageConversion;
using PDFDataExtraction.WebAPI.Helpers;
using PDFDataExtraction.WebAPI.Models;

namespace PDFDataExtraction.WebAPI.Controllers
{
    /// <summary>
    ///     Offers various methods for extracting text embedded in a PDF file
    /// </summary>
    [Route("api/PDFTextExtraction")]
    [ApiController]
    public class PDFTextExtractionController : ControllerBase
    {
        private readonly IPDFDataExtractionService _pdfDataExtractionService;

        public PDFTextExtractionController(IPDFDataExtractionService pdfDataExtractionService)
        {
            _pdfDataExtractionService = pdfDataExtractionService;
        }
        
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi =true)]
        public IActionResult Get()
        {
            return new OkObjectResult("Test");
        }

        /// <summary>
        ///     Extract detailed text, including text position, size and font, from a PDF
        /// </summary>
        /// ///
        /// <param name="file">The PDF to extract text from</param>
        /// <param name="extractionParameters">Parameters for the extraction process</param>
        /// <returns>Detailed text, including text position, size and font, from the provided PDF</returns>
        [HttpPost("detailed", Name = "DetailedTextExtraction")]
        [ServiceFilter(typeof(ValidateInputPDFAttribute))]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(PDFTextExtractionResult))]
        [ProducesResponseType(500, Type = typeof(PDFTextExtractionResult))]
        public async Task DetailedExtraction([Required] IFormFile file, [FromQuery] ExtractionParameters extractionParameters)
        {
            try
            {
                var extractionResult = await ProcessFile(file, extractionParameters);
                await WriteJsonResponse(extractionResult, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                var result = new PDFTextExtractionResult {ErrorMessage = e.Message};
                await WriteJsonResponse(result, HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        ///     Extract simple text from a PDF
        /// </summary>
        /// <param name="file">The PDF to extract text from</param>
        /// <param name="extractionParameters">Parameters for the extraction process</param>
        /// <returns>Text from the provided PDF</returns>
        [HttpPost("simple", Name = "SimpleTextExtraction")]
        [ServiceFilter(typeof(ValidateInputPDFAttribute))]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        [Produces("text/plain")]
        public async Task<ActionResult<string>> SimpleExtraction([Required] IFormFile file, [FromQuery] ExtractionParameters extractionParameters)
        {
            try
            {
                var extractionResult = await ProcessFile(file, extractionParameters);
                var documentAsString = extractionResult.ExtractedData?.GetAsString();
                return documentAsString;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        private async Task<PDFTextExtractionResult> ProcessFile(IFormFile file, ExtractionParameters extractionParameters)
        {
            var mappedRequest = await MapRequest(file, extractionParameters);
            
            var result = await _pdfDataExtractionService.ExtractPDFData(mappedRequest);

            return new PDFTextExtractionResult()
            {
                ExtractedData = result.ExtractedData,
                FileMetaData = new PDFFileMetadata()
                {
                    FileMd5 = result.FileMd5,
                    TextMd5 = result.TextMd5,
                    FileName = file.Name
                },
                PagesAsPNGs = result.PagesAsImages
            };
        }

        private static async Task<PDFDataExtractionRequest> MapRequest(IFormFile file, ExtractionParameters extractionParameters)
        {
            byte[] fileBytes;
            await using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                fileBytes = ms.ToArray();
            }
            
            var mappedRequest = new PDFDataExtractionRequest
            {
                FileContents = fileBytes,
                MaxDifferenceInWordsInTheSameLine = extractionParameters.MaxDifferenceInWordsInTheSameLine,
                WhiteSpaceSizeAsAFactorOfMedianCharacterWidth = extractionParameters.WhiteSpaceSizeAsAFactorOfMedianCharacterWidth,
                ProduceImagesOfPages = extractionParameters.ConvertPdfToImages ?? false,
                MaxProcessingTimeInMilliseconds = extractionParameters.MaxProcessingTimeInMilliseconds
            };

            return mappedRequest;
        }

        /// <summary>
        ///     Using this fixes the concurrency issues of Newtonsoft.Json, where ID assignment can go wrong when the API is under heavy load
        /// </summary>
        private async Task WriteJsonResponse(object o, HttpStatusCode statusCode)
        {
            var jsonSerializer = new JsonSerializer
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };
        
            await using var ms = new MemoryStream();
            await using var streamWriter = new StreamWriter(ms);
            jsonSerializer.Serialize(streamWriter, o);
            await streamWriter.FlushAsync();
        
            var jsonText = Encoding.UTF8.GetString(ms.ToArray());

            Response.ContentType = "application/json";
            Response.ContentLength = Encoding.UTF8.GetBytes(jsonText).Length;
            Response.StatusCode = (int) statusCode;
            await Response.WriteAsync(jsonText);
        }

        public class ExtractionParameters
        {
            /// <summary>
            ///     Set this to `true` if the PDF should be converted to PNGs and included in the returned result.
            ///     Default is `false`.
            /// </summary>
            [FromQuery(Name = "convertPdfToImages")]
            public bool? ConvertPdfToImages { get; set; }
            
            /// <summary>
            ///     The allowed difference in Y coordinates for words in the same line as a factor of the average character height.
            ///     The default value is 0.15.
            /// </summary>
            [FromQuery(Name = "wordLineDiff")]
            [Range(0, double.MaxValue)]
            public double MaxDifferenceInWordsInTheSameLine { get; set; } = 0.15;
                
            /// <summary>
            ///    How wide the spacing between characters can be before the spacing is considered to be a whitespace, as a factor of the median character width.
            ///    The default value is 0.2.
            /// </summary>
            [FromQuery(Name = "whiteSpaceFactor")]
            [Range(0, double.MaxValue)]
            public double WhiteSpaceSizeAsAFactorOfMedianCharacterWidth { get; set; } = 0.2;
            
            /// <summary>
            ///     The maximum amount of milliseconds the server will spend on extracting data from the PDF before aborting.
            ///     The default value is 5000 milliseconds.
            /// </summary>
            [FromQuery(Name = "maxProcessingTime")]
            [Range(0, int.MaxValue)]
            public int MaxProcessingTimeInMilliseconds { get; set; } = 5000;
        }
    }
}