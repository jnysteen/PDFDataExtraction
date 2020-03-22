using System.Threading.Tasks;
using PDFDataExtraction.Configuration;
using PDFDataExtraction.GhostScript;
using PDFDataExtraction.PdfImageConversion;

namespace PDFDataExtraction.Generic
{
    public interface IPDFTextExtractor
    {
        Task<Document> ExtractTextFromPDF(string inputFilePath,
            DocElementConstructionConfiguration docElementConstructionConfiguration, PageAsImage[] pagesAsImages);
    }
}