using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;

namespace PDFDataExtraction.Generic.Models
{
    public class Line
    {
        /// <summary>
        ///     The ID of the line. The ID is unique in the context of the document.
        /// </summary>
        [JsonProperty("id")]
        public string Id => $"line-{LineNumberInDocument}";
        
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
        ///     The ID of the page that this line is on
        /// </summary>
        [JsonProperty("pageId")]
        public string PageId { get; set; }

        public override string ToString()
        {
            return string.Join(" ", Words.Select(w => w.ToString()));
        }
    }
}