using System.Threading.Tasks;

namespace PDFDataExtraction.Generic
{
    public interface IPDFTextExtractor
    {
        Task<Document> ExtractTextFromPDF(string inputFilePath);
    }
}