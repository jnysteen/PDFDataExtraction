using System.IO;
using System.Linq;
using System.Xml.Serialization;
using PDFDataExtraction.PDF2Txt;
using PDFDataExtraction.PDF2Txt.Models;
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

            var producedDocument = PDF2TxtMapper.MapToDocument(deserializedDoc);
            
            // Check the amount of elements
            var numberOfPagesExpected = 2;
            var numberOfPagesActual = producedDocument.Pages.Length;
            Assert.Equal(numberOfPagesExpected, numberOfPagesActual);
            
            var numberOfLinesExpected = 5;
            var numberOfLinesActual = producedDocument.Pages.Sum(p => p.Lines.Length);
            Assert.Equal(numberOfLinesExpected, numberOfLinesActual);
            
            var numberOfWordsExpected = 12;
            var numberOfWordsActual = producedDocument.Pages.Sum(p => p.Lines.Sum(l => l.Words.Length));
            Assert.Equal(numberOfWordsExpected, numberOfWordsActual);
        }
    }
}