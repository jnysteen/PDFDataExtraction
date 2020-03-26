using System;
using System.Collections.Generic;
using System.Linq;
using PDFDataExtraction.Configuration;
using PDFDataExtraction.Exceptions;
using PDFDataExtraction.Generic;
using PDFDataExtraction.Helpers;
using PDFDataExtraction.PDF2Txt.Models;
using Line = PDFDataExtraction.Generic.Line;
using Page = PDFDataExtraction.Generic.Page;

namespace PDFDataExtraction.PDF2Txt
{
    internal static class PDF2TxtMapper
    {
        public static Document MapToDocument(Pages pages, DocElementConstructionConfiguration docElementConstructionConfiguration)
        {
            var outputPages = new List<Page>();

            var allNonEmptyTextParts = pages.Page.SelectMany(p => p.Textbox).SelectMany(tb => tb.Textlines).SelectMany(tl => tl.TextParts).Where(t => !string.IsNullOrEmpty(t.Text)).ToList();

            var characterOfMeanSize = allNonEmptyTextParts.OrderBy(t => t.Size).Skip(allNonEmptyTextParts.Count() / 2).First();
            
            // TODO The width of a whitespace depends on the font size - this does not take that fact into account.
            var whitespaceSize = GetBoundingBoxFromString(characterOfMeanSize.Bbox).Width * docElementConstructionConfiguration.WhiteSpaceSizeAsAFactorOfMedianCharacterWidth;            

            Func<IEnumerable<Character>, Word> wordCreator = GetWordFromCharacters;

            var currentLineNumber = 1;

            for (var pageIndex = 0; pageIndex < pages.Page.Count; pageIndex++)
            {
                var page = pages.Page[pageIndex];
                var pageBoundingBox = GetBoundingBoxFromString(page.Bbox);

                var pageNumber = pageIndex + 1;
                var outputPage = new Generic.Page()
                {
                    Width = pageBoundingBox.Width,
                    Height = pageBoundingBox.Height,
                    PageNumber = pageIndex + 1
                };

                var allWordsOnPage = new List<Word>();

                foreach (var textBox in page.Textbox)
                {
                    foreach (var textLine in textBox.Textlines)
                    {
                        var nonEmptyTextParts = textLine.TextParts.Where(t => !string.IsNullOrEmpty(t.Text));
                        var textPartsAsCharacters =
                            nonEmptyTextParts.Select(t => GetCharacterFromTextNode(t, outputPage));

                        var wordsInLine = Grouper.GroupByCondition(textPartsAsCharacters,
                            (thisCharacter, theCharacterAfterThisCharacter) =>
                                CharactersToWordGroupingCondition(thisCharacter, theCharacterAfterThisCharacter,
                                    whitespaceSize), wordCreator);

                        allWordsOnPage.AddRange(wordsInLine);
                    }
                }

                var wordsInReadingOrder =
                    allWordsOnPage.OrderBy(w => w.BoundingBox.MaxY).ThenBy(w => w.BoundingBox.MinX);
                var createdLines = ConstructLinesFromWords(wordsInReadingOrder, docElementConstructionConfiguration, pageNumber, ref currentLineNumber);

                outputPage.Lines = createdLines;

                outputPages.Add(outputPage);
            }

            var allCharacters = outputPages.SelectMany(p => p.Lines.SelectMany(l => l.Words.SelectMany(w => w.Characters)));
            var wordsByFontSize = FontSizeGroupsCreator.FindFontSizeGroups(allCharacters);

            return new Document()
            {
                Pages = outputPages.ToArray()
            };
        }
        
        private static Line[] ConstructLinesFromWords(IEnumerable<Word> wordsInReadingOrder,
            DocElementConstructionConfiguration docElementConstructionConfiguration, int pageNumber, ref int currentDocumentLineNumber)
        {
            var maxPixelDifferenceInWordsInTheSameLine = docElementConstructionConfiguration.MaxPixelDifferenceInWordsInTheSameLine;
            var constructedLines = 
                Grouper.GroupByCondition(wordsInReadingOrder, 
                    (thisWord, thatWord) => WordsToLinesGroupingCondition(thisWord, thatWord, maxPixelDifferenceInWordsInTheSameLine), WordsToLinesGroupCreator)
                    .ToArray();

            for (var lineIndex = 0; lineIndex < constructedLines.Length; lineIndex++)
            {
                constructedLines[lineIndex].LineNumberInDocument = currentDocumentLineNumber++;
                constructedLines[lineIndex].LineNumberInPage = lineIndex+1;
            }
            return constructedLines;
        }

        private static bool CharactersToWordGroupingCondition(Character thisCharacter, Character theCharacterAfterThisCharacter, double whitespaceSize)
        {
            return theCharacterAfterThisCharacter.BoundingBox.MinX - thisCharacter.BoundingBox.MaxX <=
                   whitespaceSize;
        }
        
        private static bool WordsToLinesGroupingCondition(Word thisWord, Word nextWord, double toleratedDifference)
        {
            return Math.Abs(thisWord.BoundingBox.MaxY - nextWord.BoundingBox.MaxY) < toleratedDifference;
        }
        
        private static Line WordsToLinesGroupCreator(IEnumerable<Word> wordsInLine)
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

            var topLeftX = Math.Round(double.Parse(splitOnComma[0]), 2);
            var topLeftY = Math.Round(double.Parse(splitOnComma[1]), 2);
            var bottomRightX = Math.Round(double.Parse(splitOnComma[2]), 2);
            var bottomRightY = Math.Round(double.Parse(splitOnComma[3]), 2);

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