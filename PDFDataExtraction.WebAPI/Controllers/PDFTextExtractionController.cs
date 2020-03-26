using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PDFDataExtraction.Configuration;
using PDFDataExtraction.Generic;
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
        private readonly IPDFToImagesConverter _ipdfToImagesConverter;
        private readonly IPDFMetadataProvider _ipdfMetadataProvider;

        public PDFTextExtractionController(IPDFTextExtractor pdfTextExtractor, IPDFToImagesConverter ipdfToImagesConverter, IPDFMetadataProvider ipdfMetadataProvider)
        {
            _pdfTextExtractor = pdfTextExtractor;
            _ipdfToImagesConverter = ipdfToImagesConverter;
            _ipdfMetadataProvider = ipdfMetadataProvider;
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
        /// <param name="file">The PDF to extract text from</param>
        /// <param name="extractionParameters">Parameters for the extraction process</param>
        /// <param name="convertPdfToImages">Set this to `true` if the PDF should be converted to PNGs and included in the returned result. Default is `false`</param>
        /// <returns>Detailed text, including text position, size and font, from the provided PDF</returns>
        [HttpPost("detailed", Name = "DetailedTextExtraction")]
        [ServiceFilter(typeof(ValidateInputPDFAttribute))]
        [Produces("application/json")]
        [ProducesResponseType(500, Type = typeof(PDFTextExtractionResult))]
        public async Task<ActionResult<PDFTextExtractionResult>> DetailedExtraction(IFormFile file, [FromQuery] ExtractionParameters extractionParameters, [FromQuery] bool? convertPdfToImages)
        {
            var conf = new DocElementConstructionConfiguration();
            UseUserProvidedParameters(extractionParameters, conf);

            try
            {
                var extractionResult = await ProcessFile(file, conf, convertPdfToImages ?? false);
                return extractionResult;
            }
            catch (Exception e)
            {
                var result = new PDFTextExtractionResult {ErrorMessage = e.Message};
                return new BadRequestObjectResult(result);
            }
        }

        private static void UseUserProvidedParameters(ExtractionParameters extractionParameters,
            DocElementConstructionConfiguration conf)
        {
            if (extractionParameters == null)
                return;
            
            conf.MaxPixelDifferenceInWordsInTheSameLine = extractionParameters.MaxPixelDifferenceInWordsInTheSameLine;
            conf.WhiteSpaceSizeAsAFactorOfMedianCharacterWidth = extractionParameters.WhiteSpaceSizeAsAFactorOfMedianCharacterWidth;
        }

        /// <summary>
        ///     Extract simple text from a PDF
        /// </summary>
        /// <param name="file">The PDF to extract text from</param>
        /// <param name="extractionParameters">Parameters for the extraction process</param>
        /// <returns>Text from the provided PDF</returns>
        [HttpPost("simple", Name = "SimpleTextExtraction")]
        [ServiceFilter(typeof(ValidateInputPDFAttribute))]
        [ProducesResponseType(500, Type = typeof(string))]
        [Produces("application/json")]
        public async Task<ActionResult<string>> SimpleExtraction(IFormFile file, [FromQuery] ExtractionParameters extractionParameters)
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
                return new BadRequestObjectResult(e.Message);
            }
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
                    convertedImages = await _ipdfToImagesConverter.ConvertPDFToPNGs(inputFilePath);

                var extractedData = await _pdfTextExtractor.ExtractTextFromPDF(inputFilePath, config, convertedImages);
                var embeddedMetaData = await _ipdfMetadataProvider.GetPDFMetadata(inputFilePath);
                var fileMd5 = _ipdfMetadataProvider.GetFileMd5(inputFilePath);
                var textMd5 = _ipdfMetadataProvider.GetDocumentTextMd5(extractedData);

                return new PDFTextExtractionResult()
                {
                    ExtractedData = extractedData,
                    FileMetaData = new PDFFileMetadata()
                    {
                        FileMd5 = fileMd5,
                        FileName = file.FileName ?? file.Name,
                        TextMd5 = textMd5,
                    },
                    PDFEmbeddedMetadata = embeddedMetaData
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

        public class ExtractionParameters
        {
            /// <summary>
            ///     TODO write docs for WordDiff. Default is 10
            ///     The amount of pixels
            /// </summary>
            [FromQuery(Name = "wordLineDiff")]
            public int MaxPixelDifferenceInWordsInTheSameLine { get; set; } = 10;
                
            /// <summary>
            ///     TODO write docs for WhiteSpaceFactor. Default is 0.2
            /// </summary>
            [FromQuery(Name = "whiteSpaceFactor")]
            public double WhiteSpaceSizeAsAFactorOfMedianCharacterWidth { get; set; } = 0.2;
        }
    }
}