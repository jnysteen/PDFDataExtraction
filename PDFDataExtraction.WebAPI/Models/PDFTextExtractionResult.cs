using PDFDataExtraction.Generic;

namespace PDFDataExtraction.WebAPI.Models
{
    public class PDFTextExtractionResult
    {
        public string ErrorMessage { get; set; }
        public bool IsSuccessful => ErrorMessage == null;
        public Document ExtractedData { get; set; }
    }
}