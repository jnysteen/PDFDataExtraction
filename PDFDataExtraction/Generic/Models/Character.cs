using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PDFDataExtraction.Generic.Models
{
    [DataContract]
    public class Character
    {
        /// <summary>
        ///     The ID of the character. The ID is unique in the context of the document.
        /// </summary>
        [JsonProperty("id")]
        [DataMember(Order = 1)]
        public string Id => $"char-{CharNumberInDocument}";
        
        /// <summary>
        ///     A box that fits the character
        /// </summary>
        [JsonProperty("bbox")]
        [DataMember(Order = 2)]
        public BoundingBox BoundingBox { get; set; }

        /// <summary>
        ///     The character itself
        /// </summary>
        [JsonProperty("text")]
        [DataMember(Order = 3)]
        public string Text { get; set; }

        /// <summary>
        ///     The font used to render the character
        /// </summary>
        [JsonProperty("font")]
        [DataMember(Order = 4)]
        public string Font { get; set; }

        /// <summary>
        ///     The ID of the word that this character is in
        /// </summary>
        [JsonProperty("wordId")]
        [DataMember(Order = 5)]
        public string WordId { get; set; }

        /// <summary>
        ///     This character's number in the document 
        /// </summary>
        [JsonProperty("charNumberInDocument")]
        [DataMember(Order = 6)]
        public int CharNumberInDocument { get; set; }
    }
}