namespace PDFDataExtraction.PdfImageConversion
{
    public class PageAsImage
    {
        public string Base64EncodedContents { get; set; }
        public int PageNumber { get; set; }
        public int ImageHeight { get; set; }
        public int ImageWidth { get; set; }
    }
}