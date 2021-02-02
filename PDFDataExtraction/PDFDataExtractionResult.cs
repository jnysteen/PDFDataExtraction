using PDFDataExtraction.Generic.Models;
using PDFDataExtraction.PdfImageConversion;

namespace PDFDataExtraction
{
    public class PDFDataExtractionResult
    {
        public Document ExtractedData { get; set; }
        public PageAsImage[] PagesAsImages { get; set; }
        public string FileMd5 { get; set; }
        public string TextMd5 { get; set; }
    }
}