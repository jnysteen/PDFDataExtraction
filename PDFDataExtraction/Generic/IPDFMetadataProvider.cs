using System.IO;

namespace PDFDataExtraction.Generic
{
    public interface IMetadataProvider
    {
        string GetFileMd5(string filePath);
        string GetDocumentTextMd5(Document document);
    }
}