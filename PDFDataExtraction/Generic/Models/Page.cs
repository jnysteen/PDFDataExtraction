using System.Linq;

namespace PDFDataExtraction.Generic
{
    public class Page
    {
        /// <summary>
        ///     The lines in the page, ordered in a top-to-bottom reading direction
        /// </summary>
        public Line[] Lines { get; set; }

        /// <summary>
        ///     The width of the page
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        ///     The height of the page
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        ///     The number of the page in the document (starting from 1)
        /// </summary>
        public int PageNumber { get; set; }
        
        public override string ToString()
        {
            return string.Join("\n", Lines.Select(l => l.ToString()));
        }
    }
}