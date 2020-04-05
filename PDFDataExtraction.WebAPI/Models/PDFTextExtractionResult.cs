using PDFDataExtraction.Generic;
using PDFDataExtraction.Generic.Models;
using PDFDataExtraction.PdfImageConversion;

// ReSharper disable InconsistentNaming

namespace PDFDataExtraction.WebAPI.Models
{
    public class PDFTextExtractionResult
    {
        /// <summary>
        ///     The text extracted from the PDF file
        /// </summary>
        public Document ExtractedData { get; set; }
        
        /// <summary>
        ///     The PDF file converted into PNGs, one PNG per page
        /// </summary>
        public PageAsImage[] PagesAsPNGs { get; set; }

        /// <summary>
        ///     Metadata about the provided PDF file
        /// </summary>
        public PDFFileMetadata FileMetaData { get; set; }

        /// <summary>
        ///     Metadata embedded in the provided PDF file
        /// </summary>
        public PDFEmbeddedMetadata PDFEmbeddedMetadata { get; set; }
        
        /// <summary>
        ///     `true`, if the extraction was successful - `false` if it failed 
        /// </summary>
        public bool IsSuccessful => ErrorMessage == null;
        
        /// <summary>
        ///     If the extraction failed, this error message will describe what went wrong
        /// </summary>
        public string ErrorMessage { get; set; }

    }

}