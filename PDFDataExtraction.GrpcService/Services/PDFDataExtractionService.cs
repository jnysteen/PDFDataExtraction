using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Grpc.Core;
using PDFDataExtraction.Configuration;
using PDFDataExtraction.Generic;
using PDFDataExtraction.Generic.Models;

namespace PDFDataExtraction.GrpcService.Services
{
    public class PDFDataExtractionService : Pdfdataextraction.PdfdataextractionBase
    {
        private readonly IPDFTextExtractor _pdfTextExtractor;

        public PDFDataExtractionService(IPDFTextExtractor pdfTextExtractor)
        {
            _pdfTextExtractor = pdfTextExtractor;
        }
        
        public override async Task<PDFDataExtractionResultSimple> ExtractSimple(PDFDataExtractionRequest request, ServerCallContext context)
        {
            var extractedData = await ProcessFile(request);
            var documentAsString = extractedData?.GetAsString();
            
            return new PDFDataExtractionResultSimple()
            {
                ExtractedText = documentAsString
            };
        }

        private async Task<Generic.Models.Document> ProcessFile(PDFDataExtractionRequest request)
        {
            var fileId = Guid.NewGuid().ToString();
            var inputFilesDir = Path.Combine(Directory.GetCurrentDirectory(), $"uploaded-files/");

            if (!Directory.Exists(inputFilesDir))
                Directory.CreateDirectory(inputFilesDir);

            var inputFilePath = Path.Combine(inputFilesDir, $"{fileId}.pdf");
            await File.WriteAllBytesAsync(inputFilePath, request.FileContents.ToByteArray());

            var config = new DocElementConstructionConfiguration()
            {
                MaxDifferenceInWordsInTheSameLine = 0.15,
                WhiteSpaceSizeAsAFactorOfMedianCharacterWidth = 0.2
            };

            var extractedData = await _pdfTextExtractor.ExtractTextFromPDF(inputFilePath, config, null);
            return extractedData;
        }

        public override async Task<PDFDataExtractionResultDetailed> ExtractDetailed(PDFDataExtractionRequest request, ServerCallContext context)
        {
            var extractedData = await ProcessFile(request);
            
            var extractedDoc = new Document();
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
                                Font = c.Font,
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
                        Words = { words}
                    };
                    return newLine;
                });
                
                var page = new Document.Types.Page()
                {
                    Id = p.Id,
                    Lines = { lines }
                };
                return page;
            }));

            var pdfDataExtractionResultDetailed = new PDFDataExtractionResultDetailed()
            {
                ExtractedDocument = extractedDoc
            };

            return pdfDataExtractionResultDetailed;
        }

        public static Document.Types.BoundingBox MapBoundingBox(BoundingBox boundingBox)
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