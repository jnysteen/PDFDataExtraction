using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PDFDataExtraction.Configuration;
using PDFDataExtraction.Exceptions;
using PDFDataExtraction.Generic;
using PDFDataExtraction.Generic.Models;
using PDFDataExtraction.PdfImageConversion;

namespace PDFDataExtraction
{
    public interface IPDFDataExtractionService
    {
        Task<PDFDataExtractionResult> ExtractPDFData(PDFDataExtractionRequest request);
    }
    
    public class PDFDataExtractionService : IPDFDataExtractionService
    {
        private readonly IPDFTextExtractor _pdfTextExtractor;
        private readonly IPDFToImagesConverter _pdfToImagesConverter;
        private readonly IPDFMetadataProvider _pdfMetadataProvider;
        private readonly ILogger<PDFDataExtractionService> _logger;

        public PDFDataExtractionService(IPDFTextExtractor pdfTextExtractor, IPDFMetadataProvider pdfMetadataProvider, ILogger<PDFDataExtractionService> logger, IPDFToImagesConverter pdfToImagesConverter)
        {
            _pdfTextExtractor = pdfTextExtractor;
            _pdfMetadataProvider = pdfMetadataProvider;
            _logger = logger;
            _pdfToImagesConverter = pdfToImagesConverter;
        }

        public async Task<PDFDataExtractionResult> ExtractPDFData(PDFDataExtractionRequest request)
        {
            var fileId = Guid.NewGuid().ToString();
            var inputFilesDir = Path.Combine(Directory.GetCurrentDirectory(), $"uploaded-files/");

            if (!Directory.Exists(inputFilesDir))
                Directory.CreateDirectory(inputFilesDir);

            var inputFilePath = Path.Combine(inputFilesDir, $"{fileId}.pdf");
            
            try
            {
                var result = await ExtractPDFDataInner(request, inputFilePath, _pdfTextExtractor, _pdfToImagesConverter);
                return result;
            }
            finally
            {
                var fileName = Path.GetFileName(inputFilePath);
                try
                {
                    _logger.LogDebug("Processing of file '{fileName}' completed, removing it...", fileName); 
                    File.Delete(inputFilePath);
                    _logger.LogDebug("File '{fileName}' removed successfully", fileName);
                        
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Could not remove file '{fileName}' after ended processing", fileName);
                }
            }
        }

        private async Task<PDFDataExtractionResult> ExtractPDFDataInner(
            PDFDataExtractionRequest request, 
            string inputFilePath, 
            IPDFTextExtractor textExtractor, 
            IPDFToImagesConverter pdfToImagesConverter
            )
        {
            var fileContentsAsByteArray = request.FileContents;
            _logger.LogDebug("Received PDF of {PDFSizeInBytes} bytes", fileContentsAsByteArray.Length);
            await File.WriteAllBytesAsync(inputFilePath, fileContentsAsByteArray);

            var config = new DocElementConstructionConfiguration()
            {
                MaxDifferenceInWordsInTheSameLine = request.MaxDifferenceInWordsInTheSameLine,
                WhiteSpaceSizeAsAFactorOfMedianCharacterWidth = request.WhiteSpaceSizeAsAFactorOfMedianCharacterWidth,
            };

            try
            {
                try
                {
                    using var cancellationTokenSource = new CancellationTokenSource();
                    cancellationTokenSource.CancelAfter(request.MaxProcessingTimeInMilliseconds);

                    PageAsImage[] pagesAsImages = null;
                    if (request.ProduceImagesOfPages)
                        pagesAsImages = await pdfToImagesConverter.ConvertPDFToPNGs(inputFilePath, cancellationTokenSource.Token);
                    
                    var extractedData = await textExtractor.ExtractTextFromPDF(inputFilePath, config, pagesAsImages, cancellationTokenSource.Token);
                    var fileMd5 = _pdfMetadataProvider.GetFileMd5(inputFilePath);
                    var textMd5 = _pdfMetadataProvider.GetDocumentTextMd5(extractedData);

                    if (extractedData?.Pages == null || extractedData.Pages.Length == 0)
                        throw new NoDataExtractedException("Could not extract any data from the PDF");

                    return new PDFDataExtractionResult
                    {
                        ExtractedData = extractedData,
                        PagesAsImages = pagesAsImages,
                        FileMd5 = fileMd5,
                        TextMd5 = textMd5
                    };
                }
                catch (TaskCanceledException e)
                {
                    throw new ProcessingTimeoutException($"Processing of the PDF exceeded the time limit of {request.MaxProcessingTimeInMilliseconds} ms", e);
                }
            }
            catch (PDFDataExtractionException e)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new PDFDataExtractionException("Exception occurred while extracting data from PDF", e);
            }
        }
    }
}