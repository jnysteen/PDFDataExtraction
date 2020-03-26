namespace PDFDataExtraction.Generic
{
    /// <summary>
    ///     Metadata from the input PDF file
    /// </summary>
    public class PDFFileMetadata
    {
        /// <summary>
        ///     The MD5 hash of the PDF file
        /// </summary>
        public string FileMd5 { get; set; }

        public string FileName { get; set; }
        
        
    }
}