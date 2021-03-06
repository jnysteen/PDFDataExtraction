using System;
using System.Collections.Generic;
using System.Linq;
using PDFDataExtraction.Configuration;
using PDFDataExtraction.Exceptions;
using PDFDataExtraction.Generic;
using PDFDataExtraction.Generic.Models;
using PDFDataExtraction.Helpers;
using PDFDataExtraction.PDF2Txt.Models;
using Line = PDFDataExtraction.Generic.Models.Line;
using Page = PDFDataExtraction.Generic.Models.Page;

namespace PDFDataExtraction.PDF2Txt
{
    internal static class PDF2TxtMapper
    {
        public static Document MapToDocument(Pages pages, DocElementConstructionConfiguration docElementConstructionConfiguration)
        {
            var outputPages = new List<Page>();

            var allNonEmptyTextParts = 
                pages?
                    .Page?
                    .SelectMany(p => p.Textbox)?
                    .SelectMany(tb => tb.Textlines)?
                    .SelectMany(tl => tl.TextParts)?
                    .Where(t => !string.IsNullOrEmpty(t.Text))?
                    .ToList();
            if (allNonEmptyTextParts == null || !allNonEmptyTextParts.Any())
                throw new NoDataExtractedException("PDF2Txt could not extract any data from the PDF");
            var characterOfMeanSize = allNonEmptyTextParts.OrderBy(t => t.Size).Skip(allNonEmptyTextParts.Count() / 2).First();
            var whitespaceSize = GetBoundingBoxFromString(characterOfMeanSize.Bbox).Width * docElementConstructionConfiguration.WhiteSpaceSizeAsAFactorOfMedianCharacterWidth;            

            Func<IEnumerable<Character>, Word> wordCreator = GetWordFromCharacters;

            var allCharsWithFonts = new List<(Character character, string font)>();
            
            var nextLineNumber = 1;
            var nextWordNumber = 1;
            var nextCharNumber = 1;

            for (var pageIndex = 0; pageIndex < pages.Page.Count; pageIndex++)
            {
                var page = pages.Page[pageIndex];
                var pageBoundingBox = GetBoundingBoxFromString(page.Bbox);

                var outputPage = new Page()
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
                        var textPartsAsCharactersAndFonts =
                            nonEmptyTextParts.Select(t => GetCharacterFromTextNode(t, outputPage));
                    
                        textPartsAsCharactersAndFonts = textPartsAsCharactersAndFonts.OrderBy(w => w.character.BoundingBox.MaxY).ThenBy(w => w.character.BoundingBox.MinX).ToList();
                        allCharsWithFonts.AddRange(textPartsAsCharactersAndFonts);

                        var textPartsAsCharacters = textPartsAsCharactersAndFonts.Select(t => t.character); 

                        var wordsInLine = Grouper.GroupByCondition(textPartsAsCharacters,
                            (thisCharacter, theCharacterAfterThisCharacter) =>
                                CharactersToWordGroupingCondition(thisCharacter, theCharacterAfterThisCharacter,
                                    whitespaceSize), wordCreator);

                        allWordsOnPage.AddRange(wordsInLine);
                    }
                }
                
                foreach (var figure in page.Figure)
                {
                    var nonEmptyTextParts = figure.TextParts.Where(t => !string.IsNullOrEmpty(t.Text));
                    var textPartsAsCharactersAndFonts =
                        nonEmptyTextParts.Select(t => GetCharacterFromTextNode(t, outputPage));
                    
                    textPartsAsCharactersAndFonts = textPartsAsCharactersAndFonts.OrderBy(w => w.character.BoundingBox.MaxY).ThenBy(w => w.character.BoundingBox.MinX).ToList();
                    allCharsWithFonts.AddRange(textPartsAsCharactersAndFonts);

                    var textPartsAsCharacters = textPartsAsCharactersAndFonts.Select(t => t.character); 
                    
                    var wordsInLine = Grouper.GroupByCondition(textPartsAsCharacters,
                        (thisCharacter, theCharacterAfterThisCharacter) =>
                            CharactersToWordGroupingCondition(thisCharacter, theCharacterAfterThisCharacter,
                                whitespaceSize), wordCreator);

                    allWordsOnPage.AddRange(wordsInLine);
                }

                var wordsInReadingOrder = allWordsOnPage.OrderBy(w => w.BoundingBox.MaxY).ThenBy(w => w.BoundingBox.MinX);
                var createdLines = ConstructLinesFromWords(wordsInReadingOrder, docElementConstructionConfiguration, ref nextLineNumber);
                

                foreach (var createdLine in createdLines)
                {
                    createdLine.PageId = outputPage.Id;
                    foreach (var word in createdLine.Words)
                    {
                        word.WordNumberInDocument = nextWordNumber++;
                        word.LineId = createdLine.Id;

                        foreach (var character in word.Characters)
                        {
                            character.CharNumberInDocument = nextCharNumber++;
                            character.WordId = word.Id;
                        }
                    }
                }
                outputPage.Lines = createdLines;
                outputPages.Add(outputPage);
            }

            var fontGroups = allCharsWithFonts.GroupBy(c => c.font, c => c.character.Id).Select(g =>
                new CharacterFontGroup()
                {
                    Name = g.Key,
                    CharacterIds = g.ToList()
                }).ToArray();

            return new Document()
            {
                Pages = outputPages.ToArray(),
                Fonts = fontGroups
            };
        }
        
        private static Line[] ConstructLinesFromWords(IEnumerable<Word> wordsInReadingOrder,
            DocElementConstructionConfiguration docElementConstructionConfiguration, ref int currentDocumentLineNumber)
        {
            var maxPixelDifferenceInWordsInTheSameLine = docElementConstructionConfiguration.MaxDifferenceInWordsInTheSameLine;
            var constructedLines = 
                Grouper.GroupByCondition(wordsInReadingOrder, 
                    (thisWord, thatWord) =>
                    {
                        var allowedYDifference = maxPixelDifferenceInWordsInTheSameLine * thisWord.BoundingBox.Height;
                        return WordsToLinesGroupingCondition(thisWord, thatWord, allowedYDifference);
                    }, WordsToLinesGroupCreator)
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
            var allowedDiffInY = whitespaceSize * 0.02;

            var yDiff = Math.Abs(thisCharacter.BoundingBox.MinY - theCharacterAfterThisCharacter.BoundingBox.MinY); 
            
            return yDiff <= allowedDiffInY && theCharacterAfterThisCharacter.BoundingBox.MinX - thisCharacter.BoundingBox.MaxX <=
                   whitespaceSize;
        }
        
        private static bool WordsToLinesGroupingCondition(Word thisWord, Word nextWord, double toleratedDifference)
        {
            return Math.Abs(thisWord.BoundingBox.MaxY - nextWord.BoundingBox.MaxY) <= toleratedDifference;
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
                throw new PDFDataExtractionException(
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

        private static (Character character, string font) GetCharacterFromTextNode(TextNode textNode, Page originatesFromPage)
        {
            return (new Character()
            {
                Text = textNode.Text,
                BoundingBox = GetBoundingBoxFromString(textNode.Bbox, originatesFromPage.Height)
            }, textNode.Font);
        }

        private static Word GetWordFromCharacters(IEnumerable<Character> characters)
        {
            var enumeratedCharacters = characters.ToArray();
            
            var firstChar = enumeratedCharacters.First();
            var lastChar = enumeratedCharacters.Last();

            var wordBoundingBox = new BoundingBox()
            {
                TopLeftCorner = new Point()
                {
                    X = firstChar.BoundingBox.TopLeftCorner.X,
                    Y = firstChar.BoundingBox.TopLeftCorner.Y
                } ,
                BottomRightCorner = new Point()
                {
                    X = lastChar.BoundingBox.BottomRightCorner.X,
                    Y = lastChar.BoundingBox.BottomRightCorner.Y,
                } 
            };

            return new Word()
            {
                Characters = enumeratedCharacters.ToArray(),
                BoundingBox = wordBoundingBox
            };
        }
    }
}