using System.Linq;
using PDFDataExtraction.Generic;
using PDFDataExtraction.Generic.Models;
using Xunit;

namespace PDFDataExtraction.Tests.Common
{
    public static class DocumentChecker
    {
        internal static void CheckBoundingBoxesPositioning(Document producedDocument)
        {
            foreach (var producedDocumentPage in producedDocument.Pages)
            {
                var lastMinYOfWord = double.MinValue;

                var linesOnPage = producedDocumentPage.Lines;
                foreach (var line in linesOnPage)
                {
                    var lastMinXOfWord = double.MinValue;
                    foreach (var word in line.Words)
                    {
                        var thisMinY = word.BoundingBox.MinY;
                        Assert.True(thisMinY >= lastMinYOfWord);
                        lastMinYOfWord = thisMinY;

                        var thisMinX = word.BoundingBox.MinX;
                        Assert.True(thisMinX >= lastMinXOfWord);
                        lastMinXOfWord = thisMinX;
                    }
                }
            }
        }

        internal static string LineToString(Line line)
        {
            return string.Join(' ', line.Words.Select(w => w.Text));
        }
    }
}