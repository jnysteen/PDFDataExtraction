namespace PDFDataExtraction.WebAPI.Models
{
    public class SimplePDFTextExtractionResult
    {
        public string ErrorMessage { get; set; }
        public bool IsSuccessful => ErrorMessage == null;
        public string ExtractedData { get; set; }
    }
}