using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PDFDataExtraction.Exceptions;
using PDFDataExtraction.Generic;
using PDFDataExtraction.Helpers;
using PDFDataExtraction.PDF2Text.Models;
using PDFDataExtraction.PDFToText.Models;
using Line = PDFDataExtraction.Generic.Line;
using Page = PDFDataExtraction.PDF2Text.Models.Page;

namespace PDFDataExtraction.PDF2Text
{
    public class PDF2TextWrapper : IPDF2TextWrapper
    {
        private async Task<Pages> ExtractText(string inputFilePath)
        {
            var applicationName = "pdf2txt.py";
            var args = $"-t xml -c UTF-8 {inputFilePath}";

            var (statusCode, stdOutput) = await CmdLineHelper.Run(applicationName, args);

            if (statusCode != 0)
                throw new PDFTextExtractionException($"pdf2text.py exited with status code: {statusCode}");

            var xmlSerializer = new XmlSerializer(typeof(Pages));

            using (var reader = new StringReader(stdOutput))
            {
                var deserializedDoc = (Pages) xmlSerializer.Deserialize(reader);
                return deserializedDoc;
            }
        }

        public async Task<Document> ExtractTextFromPDF(string inputFilePath)
        {
            var extractedXml = await ExtractText(inputFilePath);

            var mapped = MapToDocument(extractedXml);

            return mapped;
        }

        public static Document MapToDocument(Pages pages)
        {
            var outputPages = new List<Generic.Page>();

            var allNonEmptyTextParts = pages.Page.SelectMany(p => p.Textbox).SelectMany(tb => tb.Textlines).SelectMany(tl => tl.TextParts).Where(t => !string.IsNullOrEmpty(t.Text)).ToList();

            var characterOfMeanSize = allNonEmptyTextParts.OrderBy(t => t.Size).Skip(allNonEmptyTextParts.Count() / 2).First();
            
            // TODO The width of a whitespace depends on the font size - this does not take that fact into account.
            var whitespaceSize = GetBoundingBoxFromString(characterOfMeanSize.Bbox).Width * 0.3;            

            
            
            Func<TextNode, TextNode, bool> wordGroupingCondition =
                (thisWord, thatWordAfterThisWord) =>
                {
                    var thisBoundingBox = GetBoundingBoxFromString(thisWord.Bbox);
                    var thatBoundingBox = GetBoundingBoxFromString(thatWordAfterThisWord.Bbox);

                    return Math.Abs(thatBoundingBox.TopLeftCorner.X - thisBoundingBox.BottomRightCorner.X) <=
                           whitespaceSize;
                };

            Func<IEnumerable<TextNode>, Word> wordCreator = GetWordFromTextNodes;

            foreach (var page in pages.Page)
            {
                var pageBoundingBox = GetBoundingBoxFromString(page.Bbox);

                var lines = new List<Line>();

                foreach (var textBox in page.Textbox)
                {
                    foreach (var textline in textBox.Textlines)
                    {
                        var nonEmptyTextParts = textline.TextParts.Where(t => !string.IsNullOrEmpty(t.Text));
                        
                        var wordsInLine =
                            Grouper.GroupByCondition(nonEmptyTextParts, wordGroupingCondition, wordCreator);

                        var line = new Line()
                        {
                            Words = wordsInLine.ToArray()
                        };

                        lines.Add(line);
                    }
                }


                var outputPage = new Generic.Page()
                {
                    Width = pageBoundingBox.Width,
                    Height = pageBoundingBox.Height,
                    Lines = lines.ToArray()
                };

                outputPages.Add(outputPage);
            }

            return new Document()
            {
                Pages = outputPages.ToArray()
            };
        }

        private static BoundingBox GetBoundingBoxFromString(string bbox)
        {
            var splitOnComma = bbox.Split(',');

            if (splitOnComma.Length != 4)
                throw new PDFTextExtractionException(
                    $"Too few or too many coordinates for a bounding box encountered!");

            var topLeftX = double.Parse(splitOnComma[0]);
            var topLeftY = double.Parse(splitOnComma[1]);
            var bottomRightX = double.Parse(splitOnComma[2]);
            var bottomRightY = double.Parse(splitOnComma[3]);

            return new BoundingBox()
            {
                TopLeftCorner = new Point() {X = topLeftX, Y = topLeftY},
                BottomRightCorner = new Point() {X = bottomRightX, Y = bottomRightY}
            };
        }

        private static Word GetWordFromTextNodes(IEnumerable<TextNode> textNodes)
        {
            var characters = textNodes.Select(t => new Character()
            {
                Font = t.Font,
                Text = t.Text,
                BoundingBox = GetBoundingBoxFromString(t.Bbox)
            });

            return new Word()
            {
                Characters = characters.ToArray()
            };
        }
    }
}