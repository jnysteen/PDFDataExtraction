namespace PDFDataExtraction.PdfImageConversion
{
    public class PageAsImage
    {
        public byte[] Contents { get; set; }
        public int PageNumber { get; set; }
        public int ImageHeight { get; set; }
        public int ImageWidth { get; set; }
    }
}