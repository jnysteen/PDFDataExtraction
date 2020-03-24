using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PDFDataExtraction.Configuration;
using PDFDataExtraction.Generic;
using PDFDataExtraction.GhostScript;
using PDFDataExtraction.PDF2Txt;
using PDFDataExtraction.PdfImageConversion;
using PDFDataExtraction.WebAPI.Helpers;
using PDFDataExtraction.WebAPI.Models;

namespace PDFDataExtraction.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PDFTextExtractionController : ControllerBase
    {
        private readonly IPDF2TxtWrapper _pdfTextExtractor;
        private readonly IPdfToImagesConverter _pdfToImagesConverter;

        public PDFTextExtractionController(IPDF2TxtWrapper pdfTextExtractor, IPdfToImagesConverter pdfToImagesConverter)
        {
            _pdfTextExtractor = pdfTextExtractor;
            _pdfToImagesConverter = pdfToImagesConverter;
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
        /// <returns>Detailed text, including text position, size and font, from the provided PDF</returns>
        [HttpPost("detailed")]
        [ServiceFilter(typeof(ValidateInputPDFAttribute))]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(PDFTextExtractionResult))]
        [ProducesResponseType(500, Type = typeof(PDFTextExtractionResult))]
        public async Task<ActionResult<PDFTextExtractionResult>> DetailedExtraction(IFormFile file, [FromQuery] int? wordDiff, [FromQuery] double? whiteSpaceFactor)
        {
            var result = new PDFTextExtractionResult();
            
            Func<string, DocElementConstructionConfiguration, PageAsImage[], Task<Document>> extractor = (s, configuration, pagesAsImages) => _pdfTextExtractor.ExtractTextFromPDF(s, configuration, pagesAsImages);

            var conf = new DocElementConstructionConfiguration();
            
            if (wordDiff.HasValue)
                conf.MaxPixelDifferenceInWordsInTheSameLine = wordDiff.Value;
            if (whiteSpaceFactor.HasValue)
                conf.WhiteSpaceSizeAsAFactorOfMedianCharacterWidth = whiteSpaceFactor.Value;
            
            try
            {
                var processFileResult = await ProcessFile(file, conf, extractor, _pdfToImagesConverter.ConvertPdfToPngs);
                result.ExtractedData = processFileResult.extractedData;
                result.PagesAsPNGs = processFileResult.extractedImages;
                return result;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                return new BadRequestObjectResult(result);
            }
        }
        
        /// <summary>
        ///     Extract simple text from a PDF
        /// </summary>
        /// <param name="file">The PDF to extract text from</param>
        /// <returns>Text from the provided PDF</returns>
        [HttpPost("simple")]
        [ServiceFilter(typeof(ValidateInputPDFAttribute))]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        [Produces("application/json")]
        public async Task<ActionResult<string>> SimpleExtraction(IFormFile file, [FromQuery] int? wordDiff, [FromQuery] double? whiteSpaceFactor)
        {
            var conf = new DocElementConstructionConfiguration();
            
            if (wordDiff.HasValue)
                conf.MaxPixelDifferenceInWordsInTheSameLine = wordDiff.Value;
            if (whiteSpaceFactor.HasValue)
                conf.WhiteSpaceSizeAsAFactorOfMedianCharacterWidth = whiteSpaceFactor.Value;
            
            try
            {
                var extractionResult = await ProcessFile(file, conf, (s, configuration, pagesAsImages) => _pdfTextExtractor.ExtractTextFromPDF(s, conf, null) , null);
                var documentAsString = extractionResult.extractedData.GetAsString();
                return documentAsString;
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        private static async Task<(T extractedData, PageAsImage[] extractedImages)> ProcessFile<T>(IFormFile file, DocElementConstructionConfiguration config, Func<string, DocElementConstructionConfiguration, PageAsImage[], Task<T>> dataExtractor, Func<string, Task<PageAsImage[]>> imageConverter)
        {                
            var fileId = Guid.NewGuid().ToString();
            var inputFilesDir = Path.Combine(Directory.GetCurrentDirectory(), $"uploaded-files/");

            if (!Directory.Exists(inputFilesDir))
                Directory.CreateDirectory(inputFilesDir);
            
            var inputFilePath = Path.Combine(inputFilesDir, $"{fileId}.pdf");

            try
            {
                // Save file to disk
                using (var fileStream = new FileStream(inputFilePath, FileMode.CreateNew, FileAccess.Write))
                    await file.CopyToAsync(fileStream);
                
                PageAsImage[] convertedImages = null;
                if(imageConverter != null)
                    convertedImages = await imageConverter(inputFilePath);

                var extractedData = await dataExtractor(inputFilePath, config, convertedImages);
                return (extractedData, convertedImages);
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