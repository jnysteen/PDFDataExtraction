using System.Threading.Tasks;
using PDFDataExtraction.Models;

namespace PDFDataExtraction
{
    public interface IPDFToTextWrapper
    {
        Task<string> ExtractTextFromPDF(string inputFilePath, PDFToTextArgs pdfToTextArgs);

        Task<Models.PDFToTextDocumentBoundingBoxLayout.Html> ExtractTextFromPDFBoundingBoxLayout(string inputFilePath,
            PDFToTextArgs pdfToTextArgs);

        Task<Models.PDFToTextDocumentBoundingBox.Html> ExtractTextFromPDFBoundingBox(string inputFilePath,
            PDFToTextArgs pdfToTextArgs);
    }
}