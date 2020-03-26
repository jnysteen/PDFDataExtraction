namespace PDFDataExtraction.Configuration
{
    public class DocElementConstructionConfiguration
    {
        public int MaxPixelDifferenceInWordsInTheSameLine { get; set; } = 10;
        public double WhiteSpaceSizeAsAFactorOfMedianCharacterWidth { get; set; } = 0.2;
    }
}