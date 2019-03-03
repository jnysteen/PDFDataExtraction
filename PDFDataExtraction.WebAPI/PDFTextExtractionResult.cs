namespace PDFDataExtraction.WebAPI
{
    public class PDFTextExtractionResult
    {
        public string ErrorMessage { get; set; }
        public bool IsSuccessful => ErrorMessage == null;
        public string ExtractedData { get; set; }
    }
}