using System.Linq;
using Newtonsoft.Json;

namespace PDFDataExtraction.Generic.Models
{
    public class Word
    {
        /// <summary>
        ///     The ID of the word. The ID is unique in the context of the document.
        /// </summary>
        [JsonProperty("id")]
        public string Id => $"word-{WordNumberInDocument}";
        
        /// <summary>
        ///     The bounding box stretching from the first to the last character of the word
        /// </summary>
        [JsonProperty("bbox")]
        public BoundingBox BoundingBox { get; set; } 
        
        /// <summary>
        ///     The characters making up this word
        /// </summary>
        [JsonProperty("chars")]
        public Character[] Characters { get; set; }
        
        /// <summary>
        ///     The number of line that this word is in
        /// </summary>
        [JsonProperty("lineId")]
        public string LineId { get; set; }

        /// <summary>
        ///     This word's number in the document
        /// </summary>
        [JsonProperty("wordNumberInDocument")]
        public int WordNumberInDocument { get; set; }
        
        /// <summary>
        ///     The word as a string
        /// </summary>
        public string Text => string.Join("", Characters.Select(c => c.Text));
        
        public override string ToString()
        {
            return Text;
        }
    }
}