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
using Page = PDFDataExtraction.Generic.Page;

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

            Func<IEnumerable<Character>, Word> wordCreator = GetWordFromCharacters;

            foreach (var page in pages.Page)
            {
                var pageBoundingBox = GetBoundingBoxFromString(page.Bbox);
                
                var outputPage = new Generic.Page()
                {
                    Width = pageBoundingBox.Width,
                    Height = pageBoundingBox.Height,
                };
                
                var allWordsOnPage = new List<Word>();

                foreach (var textBox in page.Textbox)
                {
                    foreach (var textline in textBox.Textlines)
                    {
                        var nonEmptyTextParts = textline.TextParts.Where(t => !string.IsNullOrEmpty(t.Text));
                        var textPartsAsCharacters = nonEmptyTextParts.Select(t => GetCharacterFromTextNode(t, outputPage));
                        
                        var wordsInLine = Grouper.GroupByCondition(textPartsAsCharacters, (thisCharacter, theCharacterAfterThisCharacter) => CharactersToWordGroupingCondition(thisCharacter, theCharacterAfterThisCharacter, whitespaceSize), wordCreator);

                        allWordsOnPage.AddRange(wordsInLine);
                    }
                }

                var wordsInReadingOrder = allWordsOnPage.OrderBy(w => w.BoundingBox.MaxY).ThenBy(w => w.BoundingBox.MinX);
                var createdLines = ConstructLinesFromWords(wordsInReadingOrder);

                outputPage.Lines = createdLines.ToArray();

                outputPages.Add(outputPage);
            }

            return new Document()
            {
                Pages = outputPages.ToArray()
            };
        }
        
        private static Line[] ConstructLinesFromWords(IEnumerable<Word> wordsInReadingOrder)
        {
            var maxPixelDifferenceInWordsInTheSameLine = 1;
            var constructedLines = Grouper.GroupByCondition(wordsInReadingOrder, (thisWord, thatWord) => WordsToLinesGroupingCondition(thisWord, thatWord, maxPixelDifferenceInWordsInTheSameLine), WordsToLinesGroupCreator);

            return constructedLines.ToArray();
        }

        public static bool CharactersToWordGroupingCondition(Character thisCharacter, Character theCharacterAfterThisCharacter, double whitespaceSize)
        {
            return theCharacterAfterThisCharacter.BoundingBox.MinX - thisCharacter.BoundingBox.MaxX <=
                   whitespaceSize;
        }
        
        public static bool WordsToLinesGroupingCondition(Word thisWord, Word nextWord, double toleratedDifference)
        {
            return Math.Abs(thisWord.BoundingBox.MaxY - nextWord.BoundingBox.MaxY) < toleratedDifference;
        }
        
        public static Line WordsToLinesGroupCreator(IEnumerable<Word> wordsInLine)
        {
            var wordsInNaturalReadingOrder = wordsInLine.OrderBy(w => w.BoundingBox.MinX);
            return new Line() {Words = wordsInNaturalReadingOrder.ToArray()}; 
        }

        
        private static BoundingBox GetBoundingBoxFromString(string bbox, double? pageHeight = null)
        {
            var splitOnComma = bbox.Split(',');

            if (splitOnComma.Length != 4)
                throw new PDFTextExtractionException(
                    $"Too few or too many coordinates for a bounding box encountered!");

            var topLeftX = double.Parse(splitOnComma[0]);
            var topLeftY = double.Parse(splitOnComma[1]);
            var bottomRightX = double.Parse(splitOnComma[2]);
            var bottomRightY = double.Parse(splitOnComma[3]);

            // Flip the Y-coordinates to get the points into the right coordinate system (where the Y-axis points downwards)
            if (pageHeight.HasValue)
            {
                topLeftY = pageHeight.Value - topLeftY;
                bottomRightY = pageHeight.Value - bottomRightY;

                var temp = topLeftY;
                topLeftY = bottomRightY;
                bottomRightY = temp;
            }
            
            return new BoundingBox()
            {
                TopLeftCorner = new Point() {X = topLeftX, Y = topLeftY},
                BottomRightCorner = new Point() {X = bottomRightX, Y = bottomRightY}
            };
        }

        private static Character GetCharacterFromTextNode(TextNode textNode, Page originatesFromPage)
        {
            return new Character()
            {
                Font = textNode.Font,
                Text = textNode.Text,
                BoundingBox = GetBoundingBoxFromString(textNode.Bbox, originatesFromPage.Height)
            };
        }

        private static Word GetWordFromCharacters(IEnumerable<Character> characters)
        {
            var enumeratedCharacters = characters.ToArray();
            
            var firstChar = enumeratedCharacters.First();
            var lastChar = enumeratedCharacters.Last();

            var wordBoundingBox = new BoundingBox()
            {
                TopLeftCorner = firstChar.BoundingBox.TopLeftCorner,
                BottomRightCorner = lastChar.BoundingBox.BottomRightCorner
            };

            return new Word()
            {
                Characters = enumeratedCharacters.ToArray(),
                BoundingBox = wordBoundingBox
            };
        }
    }
}