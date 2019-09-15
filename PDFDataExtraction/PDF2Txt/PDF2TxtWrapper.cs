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
using PDFDataExtraction.PDF2Txt.Models;
using PDFDataExtraction.PDFToText.Models;
using Line = PDFDataExtraction.Generic.Line;
using Page = PDFDataExtraction.Generic.Page;

namespace PDFDataExtraction.PDF2Txt
{
    public class PDF2TxtWrapper : IPDF2TxtWrapper
    {
        private readonly XmlSerializer _xmlSerializer;

        public PDF2TxtWrapper()
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

            // Certain documents contain the character 0x00, which ends up in the XML - and that char is not allowed in XML, breaking the serializer.
            // The lines below removes the invalid char
            var invalidChar = (char) 0x00; 
            var invalidCharAsString = invalidChar.ToString();
            stdOutput = stdOutput.Replace(invalidCharAsString, "");
            
            using (var reader = new StringReader(stdOutput))
            {
                var deserializedDoc = (Pages) _xmlSerializer.Deserialize(reader);
                return deserializedDoc;
            }
        }

        public async Task<Document> ExtractTextFromPDF(string inputFilePath)
        {
            var extractedXml = await ExtractText(inputFilePath);
            var mapped = PDF2TxtMapper.MapToDocument(extractedXml);
            return mapped;
        }
    }
}