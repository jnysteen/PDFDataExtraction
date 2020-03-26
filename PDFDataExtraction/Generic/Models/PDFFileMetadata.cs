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

        /// <summary>
        ///     The name of the input PDF file
        /// </summary>
        public string FileName { get; set; }
        
        /// <summary>
        ///     The MD5 hash of the textual contents of the PDF file, as extracted by this version
        /// </summary>
        public string TextMd5 { get; set; }
    }
}