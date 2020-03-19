using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using PDFDataExtraction.Configuration;
using PDFDataExtraction.Generic;
using PDFDataExtraction.PDF2Txt;
using PDFDataExtraction.PDF2Txt.Models;
using PDFDataExtraction.Tests.Common;
using PDFDataExtraction.Tests.PDF2Txt.TestFiles;
using Xunit;

namespace PDFDataExtraction.Tests.PDF2Txt
{
    public class PDF2TxtOutputMappingTests
    {
        [Fact]
        public void MapsSimpleTwoPageDocCorrectly()
        {
            var inputFile = PDF2TxtTestFilesHelper.SimpleTwoPageDoc;
            var xmlSerializer = new XmlSerializer(typeof(Pages));

            Pages deserializedDoc = null; 
            
            using (var fileStream = File.OpenRead(inputFile))
                deserializedDoc = (Pages) xmlSerializer.Deserialize(fileStream);

            var producedDocument = PDF2TxtMapper.MapToDocument(deserializedDoc, new DocElementConstructionConfiguration());
            
            // Check the amount of elements
            var numberOfPagesExpected = 2;
            var numberOfPagesActual = producedDocument.Pages.Length;
            Assert.Equal(numberOfPagesExpected, numberOfPagesActual);

            var lines = producedDocument.Pages.SelectMany(p => p.Lines).ToArray();
            
            var numberOfLinesExpected = 5;
            var numberOfLinesActual = lines.Length;
            Assert.Equal(numberOfLinesExpected, numberOfLinesActual);

            var words = lines.SelectMany(l => l.Words).ToArray();
            
            var numberOfWordsExpected = 12;
            var numberOfWordsActual = producedDocument.Pages.Sum(p => p.Lines.Sum(l => l.Words.Length));
            Assert.Equal(numberOfWordsExpected, numberOfWordsActual);
            
           // Check the contents
           Assert.Equal("Test title header", DocumentChecker.LineToString(lines[0]));
           Assert.Equal("Test test", DocumentChecker.LineToString(lines[1]));
           Assert.Equal("Teeeeeeeest", DocumentChecker.LineToString(lines[2]));
           Assert.Equal("Super much test wow", DocumentChecker.LineToString(lines[3]));
           Assert.Equal("Second page!", DocumentChecker.LineToString(lines[4]));
           
           // Check the bounding boxes
           // The coordinate systems of the bounding boxes are local for each page 
           DocumentChecker.CheckBoundingBoxesPositioning(producedDocument);
        }


    }
}