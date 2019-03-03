using System;
using PDFDataExtraction.Models;
using Xunit;

namespace PDFDataExtraction.Tests
{
    public class PDFToTextArgsUT
    {
        [Fact]
        public void CanPassArgsCorrectly_NoArgs()
        {
            var pdfToTextArgs = new PDFToTextArgs()
            {
                
            };

            var argsAsString = pdfToTextArgs.GetArgsAsString();
            Assert.Equal(0, argsAsString.Length);
        }
        
        [Fact]
        public void CanPassArgsCorrectly_ParameterWithValueSet()
        {
            var pdfToTextArgs = new PDFToTextArgs()
            {
                Encoding = "test"
            };

            var argsAsString = pdfToTextArgs.GetArgsAsString();

            var expectedOutput = "-enc test";
            Assert.Equal(expectedOutput, argsAsString);
        }
        
        [Fact]
        public void CanPassArgsCorrectly_FlagSet()
        {
            var pdfToTextArgs = new PDFToTextArgs()
            {
                OutputBoundingBox = true
            };

            var argsAsString = pdfToTextArgs.GetArgsAsString();

            var expectedOutput = "-bbox";
            Assert.Equal(expectedOutput, argsAsString);
        }
        
        [Fact]
        public void CanPassArgsCorrectly_ParameterWithValueAndFlagSet()
        {
            var pdfToTextArgs = new PDFToTextArgs()
            {
                Encoding = "test",
                OutputBoundingBox = true
            };

            var argsAsString = pdfToTextArgs.GetArgsAsString();

            var expectedOutput = "-enc test -bbox";
            Assert.Equal(expectedOutput, argsAsString);
        }
    }
}