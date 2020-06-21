using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PDFDataExtraction.Generic.Models
{
    [DataContract]
    public class Page
    {
        /// <summary>
        ///     The ID of the page. The ID is unique in the context of the document.
        /// </summary>
        [JsonProperty("id")]
        [DataMember(Order = 1)]
        public string Id => $"page-{PageNumber}";
        
        /// <summary>
        ///     The lines in the page, ordered in a top-to-bottom reading direction
        /// </summary>
        [DataMember(Order = 2)]
        public Line[] Lines { get; set; }

        /// <summary>
        ///     The width of the page
        /// </summary>
        [Required]
        [DataMember(Order = 3)]
        public double Width { get; set; }

        /// <summary>
        ///     The height of the page
        /// </summary>
        [Required]
        [DataMember(Order = 4)]
        public double Height { get; set; }

        /// <summary>
        ///     The number of the page in the document (starting from 1)
        /// </summary>
        [Required]
        [DataMember(Order = 5)]
        public int PageNumber { get; set; }
        
        public override string ToString()
        {
            return string.Join("\n", Lines.Select(l => l.ToString()));
        }
    }
}