using System.Threading.Tasks;

namespace PDFDataExtraction.GhostScript
{
    public interface IPdfToImagesConverter
    {
        Task<PageAsImage[]> ConvertPdfToPngs(string inputFilePath);
    }
}