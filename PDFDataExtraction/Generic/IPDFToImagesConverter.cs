using System.Threading;
using System.Threading.Tasks;
using PDFDataExtraction.PdfImageConversion;
// ReSharper disable InconsistentNaming

namespace PDFDataExtraction.Generic
{
    public interface IPDFToImagesConverter
    {
        Task<PageAsImage[]> ConvertPDFToPNGs(string inputFilePath, CancellationToken cancellationToken);
    }
}