namespace PDFDataExtraction.WebAPI.Models
{
    public class DetailedPDFTextExtractionResult
    {
        public string ErrorMessage { get; set; }
        public bool IsSuccessful => ErrorMessage == null;
        public string ExtractedData { get; set; }
    }
}