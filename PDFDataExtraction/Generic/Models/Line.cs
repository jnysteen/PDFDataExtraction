using System.Linq;

namespace PDFDataExtraction.Generic
{
    public class Line
    {
        /// <summary>
        ///     The words in the line, ordered in a left-to-right reading direction
        /// </summary>
        public Word[] Words { get; set; }

        /// <summary>
        ///     The line's number on its page (starting from 1)
        /// </summary>
        public int LineNumberInPage { get; set; }

        /// <summary>
        ///     The line's number in the document (starting from 1)
        /// </summary>
        public int LineNumberInDocument { get; set; }

        public override string ToString()
        {
            return string.Join(" ", Words.Select(w => w.ToString()));
        }
    }
}