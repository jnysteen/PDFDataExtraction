using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using PDFDataExtraction.Exceptions;
using PDFDataExtraction.Generic.Models;
using PdfDataExtraction.Grpc;
using Document = PdfDataExtraction.Grpc.Document;

namespace PDFDataExtraction.GrpcService.Services
{
    public class PDFDataExtractionGrpcService : PdfDataExtractionGrpcService.PdfDataExtractionGrpcServiceBase
    {
        private readonly IPDFDataExtractionService _pdfDataExtractionService;
        private readonly ILogger<PDFDataExtractionGrpcService> _logger;

        public PDFDataExtractionGrpcService(IPDFDataExtractionService pdfDataExtractionService, ILogger<PDFDataExtractionGrpcService> logger)
        {
            _pdfDataExtractionService = pdfDataExtractionService;
            _logger = logger;
        }

        public override async Task<PDFDataExtractionResultSimple> ExtractSimple(PDFDataExtractionGrpcRequest request, ServerCallContext context)
        {
            var mappedRequest = MapRequest(request);
            var result = await _pdfDataExtractionService.ExtractPDFData(mappedRequest);
            var documentAsString = result?.ExtractedData?.GetAsString();

            return new PDFDataExtractionResultSimple()
            {
                ExtractedText = documentAsString
            };
        }


        public override async Task<PDFDataExtractionResultDetailed> ExtractDetailed(PDFDataExtractionGrpcRequest request, ServerCallContext context)
        {
            try
            {
                var mappedRequest = MapRequest(request);
                var result = await _pdfDataExtractionService.ExtractPDFData(mappedRequest);

                var extractedDoc = MapExtractedDoc(result.ExtractedData);

                var pdfDataExtractionResultDetailed = new PDFDataExtractionResultDetailed()
                {
                    ExtractedDocument = extractedDoc,
                    FileMd5 = result.FileMd5,
                    InputTextMd5 = result.TextMd5,
                };

                return pdfDataExtractionResultDetailed;
            }
            catch (Exception e)
            {
                _logger.LogError("Exception occurred while extracting data from PDF", e);
                throw;
            }
        }

        private static Document MapExtractedDoc(Generic.Models.Document extractedData)
        {
            if (extractedData?.Pages?.Any() != true) 
                return null;
            
            Document extractedDoc = null;
            
            var characterFontGroups = extractedData.Fonts.Select(f => new CharacterFont()
            {
                Name = f.Name,
                CharacterIds = {f.CharacterIds}
            });
            extractedDoc = new Document()
            {
                CharacterFonts = {characterFontGroups}
            };

            extractedDoc.Pages.AddRange(extractedData.Pages.Select(p =>
            {
                var lines = p.Lines.Select(l =>
                {
                    var words = l.Words.Select(w =>
                    {
                        var characters = w.Characters.Select(c =>
                            new Document.Types.Page.Types.Line.Types.Word.Types.Character
                            {
                                Id = c.Id,
                                BoundingBox = MapBoundingBox(c.BoundingBox),
                                Text = c.Text,
                                WordId = c.WordId,
                                CharNumberInDocument = c.CharNumberInDocument
                            });

                        var newWord = new Document.Types.Page.Types.Line.Types.Word
                        {
                            Id = w.Id,
                            BoundingBox = MapBoundingBox(w.BoundingBox),
                            LineId = w.LineId,
                            WordNumberInDocument = w.WordNumberInDocument,
                            Text = w.Text,
                            Characters = {characters}
                        };
                        return newWord;
                    });

                    var newLine = new Document.Types.Page.Types.Line()
                    {
                        Id = l.Id,
                        PageId = l.PageId,
                        LineNumberInDocument = l.LineNumberInDocument,
                        LineNumberInPage = l.LineNumberInDocument,
                        Words = {words}
                    };
                    return newLine;
                });

                var page = new Document.Types.Page()
                {
                    Id = p.Id,
                    Lines = {lines},
                    Height = p.Height,
                    Width = p.Width,
                    PageNumber = p.PageNumber
                };
                return page;
            }));

            return extractedDoc;
        }

        private static PDFDataExtractionRequest MapRequest(PDFDataExtractionGrpcRequest request)
        {
            return new PDFDataExtractionRequest
            {
                FileContents = request.ToByteArray(),
                MaxDifferenceInWordsInTheSameLine = request.ExtractionParameters.WordLineDiff,
                WhiteSpaceSizeAsAFactorOfMedianCharacterWidth = request.ExtractionParameters.WhiteSpaceFactor,
                MaxProcessingTimeInMilliseconds = request.MaxProcessingTimeInMilliseconds,
                ProduceImagesOfPages = request.ConvertPdfToImages
            };
        }

        private static Document.Types.BoundingBox MapBoundingBox(BoundingBox boundingBox)
        {
            return new Document.Types.BoundingBox()
            {
                BottomRight = new Document.Types.BoundingBox.Types.Point()
                {
                    X = boundingBox.BottomRightCorner.X,
                    Y = boundingBox.BottomRightCorner.Y,
                },
                TopLeftCorner = new Document.Types.BoundingBox.Types.Point
                {
                    X = boundingBox.TopLeftCorner.X,
                    Y = boundingBox.TopLeftCorner.Y
                }
            };
        }
    }
}