using System;
using System.Collections.Generic;
using System.Linq;
using PDFDataExtraction.Generic;
using PDFDataExtraction.Helpers;
using PDFDataExtraction.PDF2Txt.Models;
using Line = PDFDataExtraction.Generic.Line;
using Page = PDFDataExtraction.Generic.Page;

namespace PDFDataExtraction.PDFToText
{
    internal static class PDFToTextMapper
    {
        public static Document MapToDocument(Models.PDFToTextDocumentBoundingBox.Html html)
        {
            var pages = html.Body.Doc.Page;

            return new Document()
            {
                Pages = pages
                    .Select(page => new Page
                    {
                        Width = page.Width, 
                        Height = page.Height, 
                        Lines = ConstructLinesFromWords(page.Word)
                    }).ToArray()
            };
        }

        private static Line[] ConstructLinesFromWords(List<Models.PDFToTextDocumentBoundingBox.Word> words)
        {
            Func<Word, Word, bool> lineGroupingCondition =
                (thisWord, thatWord) => Math.Abs(thisWord.BoundingBox.MaxY - thatWord.BoundingBox.MaxY) < 0.1;

            Func<IEnumerable<Word>, Line> lineCreator = wordsInLine => new Line() {Words = wordsInLine.ToArray()};

            var mappedWords = words.Select(MapFromWord);
            var wordsInReadingOrder =
                mappedWords.OrderBy(w => w.BoundingBox.MinY).ThenBy(w => w.BoundingBox.MinX).ToList();

            var constructedLines = Grouper.GroupByCondition(wordsInReadingOrder, lineGroupingCondition, lineCreator);

            return constructedLines.ToArray();
        }

        private static Character[] FakeCharactersFromString(Models.PDFToTextDocumentBoundingBox.Word oldWord)
        {
            var charsInWord = oldWord.Text.ToCharArray();
            var oldWordWidth = oldWord.XMax - oldWord.XMin;

            var widthPerCharacter = oldWordWidth / charsInWord.Length;

            var nextCharStartX = oldWord.XMin;

            var characters = new Character[charsInWord.Length];

            for (var i = 0; i < charsInWord.Length; i++)
            {
                var charInWord = charsInWord[i];

                var character = new Character()
                {
                    Text = charInWord.ToString(),
                    BoundingBox = new BoundingBox()
                    {
                        TopLeftCorner = new Point()
                        {
                            X = nextCharStartX,
                            Y = oldWord.YMin
                        },
                        BottomRightCorner = new Point()
                        {
                            X = nextCharStartX + widthPerCharacter,
                            Y = oldWord.YMax
                        }
                    }
                };

                nextCharStartX += widthPerCharacter;
                characters[i] = character;
            }

            return characters;
        }

        private static Word MapFromWord(Models.PDFToTextDocumentBoundingBox.Word oldWord)
        {
            return new Word()
            {
                BoundingBox = new BoundingBox()
                {
                    TopLeftCorner = new Point()
                    {
                        X = oldWord.XMin,
                        Y = oldWord.YMin
                    },
                    BottomRightCorner = new Point()
                    {
                        X = oldWord.XMax,
                        Y = oldWord.YMax
                    }
                },
                Characters = FakeCharactersFromString(oldWord)
            };
        }
    }
}