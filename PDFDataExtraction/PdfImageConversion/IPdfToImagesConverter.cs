using System.Threading.Tasks;
using PDFDataExtraction.PdfImageConversion;

namespace PDFDataExtraction.GhostScript
{
    public interface IPdfToImagesConverter
    {
        Task<PageAsImage[]> ConvertPdfToPngs(string inputFilePath);
    }
}