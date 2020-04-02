namespace PDFDataExtraction.Configuration
{
    public class DocElementConstructionConfiguration
    {
        public int MaxPixelDifferenceInWordsInTheSameLine { get; set; } = 4;
        public double WhiteSpaceSizeAsAFactorOfMedianCharacterWidth { get; set; } = 0.2;
    }
}