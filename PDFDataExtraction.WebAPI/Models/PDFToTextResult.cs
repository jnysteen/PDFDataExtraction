namespace PDFDataExtraction.WebAPI.Models
{
    public class PDFToTextResult
    {
        public string ErrorMessage { get; set; }
        public bool IsSuccessful => ErrorMessage == null;
        public string ExtractedData { get; set; }
    }
}