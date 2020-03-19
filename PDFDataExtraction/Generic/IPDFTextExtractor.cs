using System.Threading.Tasks;
using PDFDataExtraction.Configuration;

namespace PDFDataExtraction.Generic
{
    public interface IPDFTextExtractor
    {
        Task<Document> ExtractTextFromPDF(string inputFilePath, DocElementConstructionConfiguration docElementConstructionConfiguration);
    }
}