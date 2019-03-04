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
using PDFDataExtraction.PDFToText.Models;

// ReSharper disable StringLiteralTypo

namespace PDFDataExtraction.PDFToText
{
    public class PDFToTextWrapper : IPDFToTextWrapper, IPDFTextExtractor
    {
        public async Task<string> ExtractTextFromPDF(string inputFilePath, PDFToTextArgs pdfToTextArgs)
        {
            var otherArgsAsString = pdfToTextArgs.GetArgsAsString();

            var outputFilePath = "-"; // If the output file is "-", the output is redirected to stdout

            var applicationName = "pdftotext";
            var args = $"{otherArgsAsString} {inputFilePath} {outputFilePath}";

            var (statusCode, stdOutput) = await CmdLineHelper.Run(applicationName, args);
            
            if (statusCode != 0)
                throw new PDFTextExtractionException($"pdftotext exited with status code: {statusCode}");

            return stdOutput;
        }

        public async Task<Models.PDFToTextDocumentBoundingBoxLayout.Html> ExtractTextFromPDFBoundingBoxLayout(
            string inputFilePath, PDFToTextArgs pdfToTextArgs)
        {
            pdfToTextArgs.OutputBoundingBoxLayout = true;
            var extractedXml = await ExtractTextFromPDF(inputFilePath, pdfToTextArgs);

            var xmlSerializer = new XmlSerializer(typeof(Models.PDFToTextDocumentBoundingBoxLayout.Html));

            using (var reader = new StringReader(extractedXml))
            {
                var deserializedDoc =
                    (Models.PDFToTextDocumentBoundingBoxLayout.Html) xmlSerializer.Deserialize(reader);
                return deserializedDoc;
            }
        }

        public async Task<Models.PDFToTextDocumentBoundingBox.Html> ExtractTextFromPDFBoundingBox(string inputFilePath,
            PDFToTextArgs pdfToTextArgs)
        {
            pdfToTextArgs.OutputBoundingBox = true;
            var extractedXml = await ExtractTextFromPDF(inputFilePath, pdfToTextArgs);

            var xmlSerializer = new XmlSerializer(typeof(Models.PDFToTextDocumentBoundingBox.Html));

            using (var reader = new StringReader(extractedXml))
            {
                var deserializedDoc = (Models.PDFToTextDocumentBoundingBox.Html) xmlSerializer.Deserialize(reader);
                return deserializedDoc;
            }
        }

        public async Task<Document> ExtractTextFromPDF(string inputFilePath)
        {
            var args = new PDFToTextArgs()
            {
                OutputBoundingBox = true
            };

            var extractedHtml = await ExtractTextFromPDFBoundingBox(inputFilePath, args);
            return MapToDocument(extractedHtml);
        }

        private static Document MapToDocument(Models.PDFToTextDocumentBoundingBox.Html html)
        {
            var pages = html.Body.Doc.Page;

            var outputPages = new List<Page>();

            foreach (var page in pages)
            {
                var outputPage = new Page
                {
                    Width = page.Width, 
                    Height = page.Height, 
                    Lines = ConstructLinesFromWords(page.Word)
                };

                outputPages.Add(outputPage);
            }

            return new Document()
            {
                Pages = outputPages.ToArray()
            };
        }

        private static Line[] ConstructLinesFromWords(List<Models.PDFToTextDocumentBoundingBox.Word> words)
        {
            Func<Word, Word, bool> lineGroupingCondition =
                (thisWord, thatWord) => Math.Abs(thisWord.YMax - thatWord.YMax) < 0.1;
                
            Func<IEnumerable<Word>, Line> lineCreator = wordsInLine => new Line() {Words = wordsInLine.ToArray()}; 

            var mappedWords = words.Select(MapFromWord);
            var wordsInReadingOrder = mappedWords.OrderBy(w => w.YMin).ThenBy(w => w.XMin).ToList();
            
            var constructedLines = Grouper.GroupByCondition(mappedWords, lineGroupingCondition, lineCreator);

            return constructedLines.ToArray();
        }

        private static Character[] FakeCharactersFromString(Models.PDFToTextDocumentBoundingBox.Word oldWord)
        {
            var charsInWord = oldWord.Text.ToCharArray();
            var oldWordWidth = oldWord.XMax - oldWord.XMin;
            
            var widthPerCharacter = oldWordWidth / charsInWord.Length;

            var nextCharStartX = oldWord.XMin;

            var characters = new Character[charsInWord.Length];
            
            for (int i = 0; i < charsInWord.Length; i++)
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
                XMin = oldWord.XMin,
                XMax = oldWord.XMax,
                YMin = oldWord.YMin,
                YMax = oldWord.YMax,
                Characters = FakeCharactersFromString(oldWord)
            };
        }

        private static Line MapFromWordStack(Stack<Word> stack)
        {
            return new Line()
            {
                Words = stack.Reverse().ToArray()
            };
        }
    }
}