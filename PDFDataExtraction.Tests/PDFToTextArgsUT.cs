using System;
using PDFDataExtraction.PDFToText.Models;
using Xunit;

namespace PDFDataExtraction.Tests
{
    public class PDFToTextArgsUT
    {
        [Fact]
        public void CanPassArgsCorrectly_NoArgsSpecified()
        {
            var pdfToTextArgs = new PDFToTextArgs();

            var argsAsString = pdfToTextArgs.GetArgsAsString();
            Assert.Equal(0, argsAsString.Length);
        }
        
        [Fact]
        public void CanPassArgsCorrectly_DoubleValueNoDecimals()
        {
            var pdfToTextArgs = new PDFToTextArgs()
            {
                Resolution = 300
            };
 
            var argsAsString = pdfToTextArgs.GetArgsAsString();
            
            var expectedOutput = "-r 300";
            Assert.Equal(expectedOutput, argsAsString);
        }
        
        [Fact]
        public void CanPassArgsCorrectly_DoubleValueWithDecimals()
        {
            var pdfToTextArgs = new PDFToTextArgs()
            {
                Resolution = 123.4569123874
            };
 
            var argsAsString = pdfToTextArgs.GetArgsAsString();
            
            var expectedOutput = "-r 123.4569123874";
            Assert.Equal(expectedOutput, argsAsString);
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
        public void CanPassArgsCorrectly_ParametersWithValuesAndFlagSet()
        {
            var pdfToTextArgs = new PDFToTextArgs()
            {
                Encoding = "test",
                OutputBoundingBox = true,
                Resolution = 123.4569123874
            };

            var argsAsString = pdfToTextArgs.GetArgsAsString();

            var expectedOutput = "-r 123.4569123874 -enc test -bbox";
            Assert.Equal(expectedOutput, argsAsString);
        }
    }
}