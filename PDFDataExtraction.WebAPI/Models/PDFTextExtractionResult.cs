using PDFDataExtraction.Generic;
using PDFDataExtraction.GhostScript;
// ReSharper disable InconsistentNaming

namespace PDFDataExtraction.WebAPI.Models
{
    public class PDFTextExtractionResult
    {
        public string ErrorMessage { get; set; }
        public bool IsSuccessful => ErrorMessage == null;
        public Document ExtractedData { get; set; }
        public PageAsImage[] PagesAsPNGs { get; set; }
    }
}