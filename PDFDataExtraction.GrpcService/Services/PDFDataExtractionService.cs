using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
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
        private readonly IPDFToImagesConverter _pdfToImagesConverter;
        private readonly IPDFMetadataProvider _pdfMetadataProvider;

        public PDFDataExtractionService(IPDFTextExtractor pdfTextExtractor, IPDFToImagesConverter pdfToImagesConverter, IPDFMetadataProvider pdfMetadataProvider)
        {
            _pdfTextExtractor = pdfTextExtractor;
            _pdfToImagesConverter = pdfToImagesConverter;
            _pdfMetadataProvider = pdfMetadataProvider;
        }
        
        public override async Task<PDFDataExtractionResultSimple> ExtractSimple(PDFDataExtractionRequest request, ServerCallContext context)
        {
            var (extractedData, _, _, _) = await ProcessFile(request);
            var documentAsString = extractedData?.GetAsString();
            
            return new PDFDataExtractionResultSimple()
            {
                ExtractedText = documentAsString
            };
        }

        private async Task<(Generic.Models.Document extractedData, PdfImageConversion.PageAsImage[] pngs, string fileMd5, string textMd5)> ProcessFile(PDFDataExtractionRequest request)
        {
            var fileId = Guid.NewGuid().ToString();
            var inputFilesDir = Path.Combine(Directory.GetCurrentDirectory(), $"uploaded-files/");

            if (!Directory.Exists(inputFilesDir))
                Directory.CreateDirectory(inputFilesDir);

            var inputFilePath = Path.Combine(inputFilesDir, $"{fileId}.pdf");
            await File.WriteAllBytesAsync(inputFilePath, request.FileContents.ToByteArray());

            var config = new DocElementConstructionConfiguration()
            {
                MaxDifferenceInWordsInTheSameLine = request.ExtractionParameters.WordLineDiff,
                WhiteSpaceSizeAsAFactorOfMedianCharacterWidth = request.ExtractionParameters.WhiteSpaceFactor,
            };
            
            PdfImageConversion.PageAsImage[] pngs = null;
            if(request.ConvertPdfToImages)
                pngs = await _pdfToImagesConverter.ConvertPDFToPNGs(inputFilePath);

            var extractedData = await _pdfTextExtractor.ExtractTextFromPDF(inputFilePath, config, pngs);
            
            var fileMd5 = _pdfMetadataProvider.GetFileMd5(inputFilePath);
            var textMd5 = _pdfMetadataProvider.GetDocumentTextMd5(extractedData);
            
            return (extractedData, pngs, fileMd5, textMd5);
        }

        public override async Task<PDFDataExtractionResultDetailed> ExtractDetailed(PDFDataExtractionRequest request, ServerCallContext context)
        {
            var (extractedData, pngs, fileMd5, textMd5) = await ProcessFile(request);

            var characterFontGroups = extractedData.Fonts.Select(f => new CharacterFont()
            {
                Name = f.Name,
                CharacterIds = {f.CharacterIds}
            });
            var extractedDoc = new Document()
            {
                CharacterFonts = { characterFontGroups }
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

            var mappedPngs = pngs?.Select(png => new PageAsImage
            {
                Contents = ByteString.CopyFrom(png.Contents),
                PageNumber = png.PageNumber,
                ImageHeight = png.ImageHeight,
                ImageWidth = png.ImageWidth
            });
            
            var pdfDataExtractionResultDetailed = new PDFDataExtractionResultDetailed()
            {
                ExtractedDocument = extractedDoc,
                FileMd5 = fileMd5,
                InputTextMd5 = textMd5
            };
            
            if(mappedPngs != null)
                pdfDataExtractionResultDetailed.PagesAsPNGs.AddRange(mappedPngs);

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