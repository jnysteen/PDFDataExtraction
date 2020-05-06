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
        private readonly IPDFTextExtractor _pdfTextExtractor;
        private readonly IPDFToImagesConverter _pdfToImagesConverter;
        private readonly IPDFMetadataProvider _pdfMetadataProvider;

        public PDFTextExtractionController(IPDFTextExtractor pdfTextExtractor, IPDFToImagesConverter pdfToImagesConverter, IPDFMetadataProvider pdfMetadataProvider)
        {
            _pdfTextExtractor = pdfTextExtractor;
            _pdfToImagesConverter = pdfToImagesConverter;
            _pdfMetadataProvider = pdfMetadataProvider;
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
            var conf = new DocElementConstructionConfiguration();
            UseUserProvidedParameters(extractionParameters, conf);

            try
            {
                var extractionResult = await ProcessFile(file, conf, extractionParameters.ConvertPdfToImages ?? false);
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
            var conf = new DocElementConstructionConfiguration();
            UseUserProvidedParameters(extractionParameters, conf);
            
            try
            {
                var extractionResult = await ProcessFile(file, conf, false);
                var documentAsString = extractionResult.ExtractedData?.GetAsString();
                return documentAsString;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        private static void UseUserProvidedParameters(ExtractionParameters extractionParameters,
            DocElementConstructionConfiguration conf)
        {
            if (extractionParameters == null)
                return;
            
            conf.MaxDifferenceInWordsInTheSameLine = extractionParameters.MaxDifferenceInWordsInTheSameLine;
            conf.WhiteSpaceSizeAsAFactorOfMedianCharacterWidth = extractionParameters.WhiteSpaceSizeAsAFactorOfMedianCharacterWidth;
        }
        
        private async Task<PDFTextExtractionResult> ProcessFile(IFormFile file, DocElementConstructionConfiguration config, bool convertPdfToPngs)
        {                
            var fileId = Guid.NewGuid().ToString();
            var inputFilesDir = Path.Combine(Directory.GetCurrentDirectory(), $"uploaded-files/");

            if (!Directory.Exists(inputFilesDir))
                Directory.CreateDirectory(inputFilesDir);
            
            var inputFilePath = Path.Combine(inputFilesDir, $"{fileId}.pdf");

            try
            {
                // Save file to disk
                await using (var fileStream = new FileStream(inputFilePath, FileMode.CreateNew, FileAccess.Write))
                    await file.CopyToAsync(fileStream);
                
                PageAsImage[] convertedImages = null;
                if(convertPdfToPngs)
                    convertedImages = await _pdfToImagesConverter.ConvertPDFToPNGs(inputFilePath);

                var extractedData = await _pdfTextExtractor.ExtractTextFromPDF(inputFilePath, config, convertedImages);
                var embeddedMetaData = await _pdfMetadataProvider.GetPDFMetadata(inputFilePath);
                var fileMd5 = _pdfMetadataProvider.GetFileMd5(inputFilePath);
                var textMd5 = _pdfMetadataProvider.GetDocumentTextMd5(extractedData);

                return new PDFTextExtractionResult()
                {
                    ExtractedData = extractedData,
                    FileMetaData = new PDFFileMetadata()
                    {
                        FileMd5 = fileMd5,
                        FileName = file.FileName ?? file.Name,
                        TextMd5 = textMd5,
                    },
                    PDFEmbeddedMetadata = embeddedMetaData,
                    PagesAsPNGs = convertedImages
                };
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
            ///     Set this to `true` if the PDF should be converted to PNGs and included in the returned result. Default is `false`
            /// </summary>
            [FromQuery(Name = "convertPdfToImages")]
            public bool? ConvertPdfToImages { get; set; }
            
            /// <summary>
            ///     The allowed difference in Y coordinates for words in the same line as a factor of the average character height.
            ///     The default value is 0.05
            /// </summary>
            [FromQuery(Name = "wordLineDiff")]
            public double MaxDifferenceInWordsInTheSameLine { get; set; } = 0.05;
                
            /// <summary>
            ///    How wide the spacing between characters can be before the spacing is considered to be a whitespace, as a factor of the median character width.
            ///    The default value is 0.2
            /// </summary>
            [FromQuery(Name = "whiteSpaceFactor")]
            public double WhiteSpaceSizeAsAFactorOfMedianCharacterWidth { get; set; } = 0.2;
        }
    }
}