using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PDFDataExtraction.Configuration;
using PDFDataExtraction.Exceptions;
using PDFDataExtraction.Generic;
using PDFDataExtraction.Generic.Models;
using PDFDataExtraction.Helpers;
using PDFDataExtraction.PDF2Txt.Models;
using PDFDataExtraction.PdfImageConversion;

namespace PDFDataExtraction.PDF2Txt
{
    public class PDF2TxtWrapper : IPDFTextExtractor
    {
        private readonly XmlSerializer _xmlSerializer;

        public PDF2TxtWrapper()
        {
            _xmlSerializer = new XmlSerializer(typeof(Pages));
        }
        
        public async Task<Document> ExtractTextFromPDF(string inputFilePath,
            DocElementConstructionConfiguration docElementConstructionConfiguration, PageAsImage[] pagesAsImages)
        {
            var extractedXml = await ExtractText(inputFilePath);
            var mapped = PDF2TxtMapper.MapToDocument(extractedXml, docElementConstructionConfiguration);
            
            // If there are page images, we have to ensure that the bounding boxes of the document elements correspond to the size of the page they are on
            if (pagesAsImages != null)
                NormalizeBoundingBoxes(pagesAsImages, mapped);
            
            return mapped;
        }

        private async Task<Pages> ExtractText(string inputFilePath)
        {
            var applicationName = "pdf2txt.py";
            var args = $"-t xml -c UTF-8 {inputFilePath}";

            var (statusCode, stdOutput) = await CmdLineHelper.Run(applicationName, args);

            if (statusCode != 0)
                throw new PDFTextExtractionException($"{applicationName} exited with status code: {statusCode}");
            
            if(string.IsNullOrEmpty(stdOutput))
                throw new PDFTextExtractionException($"{applicationName} completed without outputting anything!");

            // Certain documents contain the character 0x00, which ends up in the XML - and that char is not allowed in XML, breaking the serializer.
            // The lines below removes the invalid char
            var invalidChar = ((char) 0x00).ToString();
            stdOutput = stdOutput.Replace(invalidChar, "");

            using var reader = new StringReader(stdOutput);
            var deserializedDoc = (Pages) _xmlSerializer.Deserialize(reader);
            return deserializedDoc;
        }

        private static void NormalizeBoundingBoxes(PageAsImage[] pagesAsImages, Document mapped)
        {
            var extractedPages = mapped.Pages;

            if (pagesAsImages.Length != extractedPages.Length)
                throw new Exception(
                    $"Data extraction found {extractedPages.Length} pages in the PDF, but {pagesAsImages.Length} images of pages were provided!");

            for (var i = 0; i < pagesAsImages.Length; i++)
            {
                var pageAsImage = pagesAsImages[i];
                var extractedPage = extractedPages[i];

                var heightNormalizationFactor = pageAsImage.ImageHeight / extractedPage.Height;
                var widthNormalizationFactor = pageAsImage.ImageWidth / extractedPage.Width;

                extractedPage.Height = Math.Round(extractedPage.Height * heightNormalizationFactor, 2);
                extractedPage.Width = Math.Round(extractedPage.Width* widthNormalizationFactor, 2);

                foreach (var pageLine in extractedPage.Lines)
                {
                    foreach (var word in pageLine.Words)
                    {
                        word.BoundingBox = new BoundingBox()
                        {
                            BottomRightCorner = new Point()
                            {
                                X = Math.Round(word.BoundingBox.BottomRightCorner.X * widthNormalizationFactor, 2), 
                                Y = Math.Round(word.BoundingBox.BottomRightCorner.Y * heightNormalizationFactor, 2)
                            },
                            TopLeftCorner = new Point()
                            {
                                X = Math.Round(word.BoundingBox.TopLeftCorner.X * widthNormalizationFactor, 2),
                                Y = Math.Round(word.BoundingBox.TopLeftCorner.Y * heightNormalizationFactor, 2)
                            },
                        };
                        
                        foreach (var wordCharacter in word.Characters)
                        {
                            wordCharacter.BoundingBox = new BoundingBox()
                            {
                                BottomRightCorner = new Point()
                                {
                                    X = Math.Round(wordCharacter.BoundingBox.BottomRightCorner.X * widthNormalizationFactor, 2), 
                                    Y = Math.Round(wordCharacter.BoundingBox.BottomRightCorner.Y * heightNormalizationFactor, 2)
                                },
                                TopLeftCorner = new Point()
                                {
                                    X = Math.Round(wordCharacter.BoundingBox.TopLeftCorner.X * widthNormalizationFactor, 2),
                                    Y = Math.Round(wordCharacter.BoundingBox.TopLeftCorner.Y * heightNormalizationFactor, 2)
                                },
                            };
                        }
                    }
                }
            }
        }
    }
}