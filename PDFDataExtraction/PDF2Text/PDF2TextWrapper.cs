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
        private readonly XmlSerializer _xmlSerializer;

        public PDF2TextWrapper()
        {
            _xmlSerializer = new XmlSerializer(typeof(Pages));
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

            using (var reader = new StringReader(stdOutput))
            {
                var deserializedDoc = (Pages) _xmlSerializer.Deserialize(reader);
                return deserializedDoc;
            }
        }

        public async Task<Document> ExtractTextFromPDF(string inputFilePath)
        {
            var extractedXml = await ExtractText(inputFilePath);
            var mapped = PDF2TextMapper.MapToDocument(extractedXml);
            return mapped;
        }
    }
}