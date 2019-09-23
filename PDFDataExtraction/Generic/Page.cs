namespace PDFDataExtraction.Generic
{
    public class Page
    {
        /// <summary>
        ///     The lines in the page, ordered in a top-to-bottom reading direction
        /// </summary>
        public Line[] Lines { get; set; }

        /// <summary>
        ///     The width of the page in //TODO WHAT UNIT?
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        ///     The height of the page in //TODO WHAT UNIT?
        /// </summary>
        public double Height { get; set; }
    }
}