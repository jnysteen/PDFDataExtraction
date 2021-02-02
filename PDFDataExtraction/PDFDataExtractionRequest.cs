using System.ComponentModel.DataAnnotations;

namespace PDFDataExtraction
{
    public class PDFDataExtractionRequest
    {
        public byte[] FileContents { get; set; }
        public double MaxDifferenceInWordsInTheSameLine { get; set; }
        public double WhiteSpaceSizeAsAFactorOfMedianCharacterWidth { get; set; }
        public bool ProduceImagesOfPages { get; set; }
        
        /// <summary>
        ///     The maximum allowed time spent on extracting data from the PDF
        /// </summary>
        [Range(1, int.MaxValue)]
        public int MaxProcessingTimeInMilliseconds { get; set; }
    }
}