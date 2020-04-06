using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;

namespace PDFDataExtraction.Generic.Models
{
    public class Line
    {
        /// <summary>
        ///     The words in the line, ordered in a left-to-right reading direction
        /// </summary>
        [JsonProperty("words")]
        public Word[] Words { get; set; }

        /// <summary>
        ///     The line's number on its page (starting from 1)
        /// </summary>
        [JsonProperty("lineNumberInPage")]
        [Required]
        public int LineNumberInPage { get; set; }

        /// <summary>
        ///     The line's number in the document (starting from 1)
        /// </summary>
        [JsonProperty("lineNumberInDocument")]
        [Required]
        public int LineNumberInDocument { get; set; }

        /// <summary>
        ///     The page that this line is on
        /// </summary>
        [JsonProperty("page")]
        public Page Page { get; set; }

        public override string ToString()
        {
            return string.Join(" ", Words.Select(w => w.ToString()));
        }
    }
}