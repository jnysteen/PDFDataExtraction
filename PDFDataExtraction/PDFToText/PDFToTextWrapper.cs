using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PDFDataExtraction.Exceptions;
using PDFDataExtraction.Generic;
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

            var cmd = $" {otherArgsAsString} {inputFilePath} {outputFilePath}";
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "pdftotext",
                    Arguments = escapedArgs,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            var pdfToTextOutput = await process.StandardOutput.ReadToEndAsync();
            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new PDFToTextException($"pdftotext exited with status code: {process.ExitCode}");

            return pdfToTextOutput;
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
            var constructedLines = new List<Line>();

            var wordsInReadingOrder = words.OrderBy(w => w.YMin).ThenBy(w => w.XMin);

            var wordsToBePutInLines = new Queue<Models.PDFToTextDocumentBoundingBox.Word>();
            foreach (var word in wordsInReadingOrder)
                wordsToBePutInLines.Enqueue(word);

            var lineUnderConstruction = new Stack<Word>();

            while (wordsToBePutInLines.Any())
            {
                var rawWordToBeExamined = wordsToBePutInLines.Dequeue();
                var wordToBeExamined = MapFromWord(rawWordToBeExamined);

                if (!lineUnderConstruction.Any())
                {
                    lineUnderConstruction.Push(wordToBeExamined);
                    continue;
                }

                var latestAddedWord = lineUnderConstruction.Peek();

                if (Math.Abs(wordToBeExamined.YMax - latestAddedWord.YMax) < 0.1)
                {
                    lineUnderConstruction.Push(wordToBeExamined);
                    continue;
                }

                constructedLines.Add(MapFromWordStack(lineUnderConstruction));
                lineUnderConstruction.Clear();
                lineUnderConstruction.Push(wordToBeExamined);
            }

            if (lineUnderConstruction.Any())
                constructedLines.Add(MapFromWordStack(lineUnderConstruction));

            return constructedLines.ToArray();
        }

        private static Word MapFromWord(Models.PDFToTextDocumentBoundingBox.Word oldWord)
        {
            return new Word()
            {
                XMin = oldWord.XMin,
                XMax = oldWord.XMax,
                YMin = oldWord.YMin,
                YMax = oldWord.YMax,
                Text = oldWord.Text
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