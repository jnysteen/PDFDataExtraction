using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PDFDataExtraction.Generic.Models
{
    [DataContract]
    public class Word
    {
        /// <summary>
        ///     The ID of the word. The ID is unique in the context of the document.
        /// </summary>
        [JsonProperty("id")]
        [DataMember(Order = 1)]
        public string Id => $"word-{WordNumberInDocument}";
        
        /// <summary>
        ///     The bounding box stretching from the first to the last character of the word
        /// </summary>
        [JsonProperty("bbox")]
        [DataMember(Order = 2)]
        public BoundingBox BoundingBox { get; set; } 
        
        /// <summary>
        ///     The characters making up this word
        /// </summary>
        [JsonProperty("chars")]
        [DataMember(Order = 3)]
        public Character[] Characters { get; set; }
        
        /// <summary>
        ///     The number of line that this word is in
        /// </summary>
        [JsonProperty("lineId")]
        [DataMember(Order = 4)]
        public string LineId { get; set; }

        /// <summary>
        ///     This word's number in the document
        /// </summary>
        [JsonProperty("wordNumberInDocument")]
        [DataMember(Order = 5)]
        public int WordNumberInDocument { get; set; }
        
        /// <summary>
        ///     The word as a string
        /// </summary>
        [JsonProperty("text")]
        [DataMember(Order = 6)]
        public string Text => string.Join("", Characters.Select(c => c.Text));
        
        public override string ToString()
        {
            return Text;
        }
    }
}