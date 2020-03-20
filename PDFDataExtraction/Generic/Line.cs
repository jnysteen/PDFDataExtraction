using System.Linq;

namespace PDFDataExtraction.Generic
{
    public class Line
    {
        /// <summary>
        ///     The words in the line, ordered in a left-to-right reading direction
        /// </summary>
        public Word[] Words { get; set; }

        public override string ToString()
        {
            return string.Join(" ", Words.Select(w => w.ToString()));
        }
    }
}