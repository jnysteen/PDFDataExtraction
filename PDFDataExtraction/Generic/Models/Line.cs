using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PDFDataExtraction.Generic.Models
{
    [DataContract]
    public class Line
    {
        /// <summary>
        ///     The ID of the line. The ID is unique in the context of the document.
        /// </summary>
        [JsonProperty("id")]
        [DataMember(Order = 2)]
        public string Id => $"line-{LineNumberInDocument}";
        
        /// <summary>
        ///     The words in the line, ordered in a left-to-right reading direction
        /// </summary>
        [JsonProperty("words")]
        [DataMember(Order = 3)]
        public Word[] Words { get; set; }

        /// <summary>
        ///     The line's number on its page (starting from 1)
        /// </summary>
        [JsonProperty("lineNumberInPage")]
        [Required]
        [DataMember(Order = 4)]
        public int LineNumberInPage { get; set; }

        /// <summary>
        ///     The line's number in the document (starting from 1)
        /// </summary>
        [JsonProperty("lineNumberInDocument")]
        [Required]
        [DataMember(Order = 5)]
        public int LineNumberInDocument { get; set; }

        /// <summary>
        ///     The ID of the page that this line is on
        /// </summary>
        [JsonProperty("pageId")]
        [DataMember(Order = 6)]
        public string PageId { get; set; }

        public override string ToString()
        {
            return string.Join(" ", Words.Select(w => w.ToString()));
        }
    }
}