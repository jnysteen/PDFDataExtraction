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
        private readonly XmlSerializer _xmlSerializer;

        public PDFToTextWrapper()
        {
            _xmlSerializer = new XmlSerializer(typeof(Models.PDFToTextDocumentBoundingBoxLayout.Html));
        }

        public async Task<string> ExtractTextFromPDF(string inputFilePath, PDFToTextArgs pdfToTextArgs)
        {
            var otherArgsAsString = pdfToTextArgs.GetArgsAsString();

            var outputFilePath = "-"; // If the output file is "-", the output is redirected to stdout

            var applicationName = "pdftotext";
            var args = $"{otherArgsAsString} {inputFilePath} {outputFilePath}";

            var (statusCode, stdOutput) = await CmdLineHelper.Run(applicationName, args);
            
            if (statusCode != 0)
                throw new PDFTextExtractionException($"{applicationName} exited with status code: {statusCode}");
            
            if(string.IsNullOrEmpty(stdOutput))
                throw new PDFTextExtractionException($"{applicationName} completed without outputting anything!");

            return stdOutput;
        }

        public async Task<Models.PDFToTextDocumentBoundingBoxLayout.Html> ExtractTextFromPDFBoundingBoxLayout(string inputFilePath, PDFToTextArgs pdfToTextArgs)
        {
            pdfToTextArgs.OutputBoundingBoxLayout = true;
            var extractedXml = await ExtractTextFromPDF(inputFilePath, pdfToTextArgs);

            using (var reader = new StringReader(extractedXml))
            {
                var deserializedDoc = (Models.PDFToTextDocumentBoundingBoxLayout.Html) _xmlSerializer.Deserialize(reader);
                return deserializedDoc;
            }
        }

        public async Task<Models.PDFToTextDocumentBoundingBox.Html> ExtractTextFromPDFBoundingBox(string inputFilePath, PDFToTextArgs pdfToTextArgs)
        {
            pdfToTextArgs.OutputBoundingBox = true;
            var extractedXml = await ExtractTextFromPDF(inputFilePath, pdfToTextArgs);

            using (var reader = new StringReader(extractedXml))
            {
                var deserializedDoc = (Models.PDFToTextDocumentBoundingBox.Html) _xmlSerializer.Deserialize(reader);
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
            return PDFToTextMapper.MapToDocument(extractedHtml);
        }
    }
}