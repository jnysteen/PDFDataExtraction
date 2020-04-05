using System.Threading.Tasks;
using PDFDataExtraction.Configuration;
using PDFDataExtraction.Generic.Models;
using PDFDataExtraction.PdfImageConversion;

namespace PDFDataExtraction.Generic
{
    public interface IPDFTextExtractor
    {
        Task<Document> ExtractTextFromPDF(string inputFilePath,
            DocElementConstructionConfiguration docElementConstructionConfiguration, PageAsImage[] pagesAsImages);
    }
}