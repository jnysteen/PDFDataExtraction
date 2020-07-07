using Newtonsoft.Json;

namespace PDFDataExtraction.Generic.Models
{
    public class Character
    {
        /// <summary>
        ///     The ID of the character. The ID is unique in the context of the document.
        /// </summary>
        [JsonProperty("id")]
        public string Id => $"char-{CharNumberInDocument}";
        
        /// <summary>
        ///     A box that fits the character
        /// </summary>
        [JsonProperty("bbox")]
        public BoundingBox BoundingBox { get; set; }

        /// <summary>
        ///     The character itself
        /// </summary>
        [JsonProperty("text")] 
        public string Text { get; set; }

        /// <summary>
        ///     The ID of the word that this character is in
        /// </summary>
        [JsonProperty("wordId")]
        public string WordId { get; set; }

        /// <summary>
        ///     This character's number in the document 
        /// </summary>
        public int CharNumberInDocument { get; set; }
    }
}