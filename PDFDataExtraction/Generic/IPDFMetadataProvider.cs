using System.IO;
using System.Threading.Tasks;
using PDFDataExtraction.Generic.Models;

namespace PDFDataExtraction.Generic
{
    public interface IPDFMetadataProvider
    {
        string GetFileMd5(string filePath);
        string GetDocumentTextMd5(Document document);
        Task<PDFEmbeddedMetadata> GetPDFMetadata(string inputFilePath);
    }
}