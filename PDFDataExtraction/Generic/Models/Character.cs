using Newtonsoft.Json;

namespace PDFDataExtraction.Generic.Models
{
    public class Character
    {
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
        ///     The font used to render the character
        /// </summary>
        [JsonProperty("font")] 
        public string Font { get; set; }

        /// <summary>
        ///     The word that this character is in
        /// </summary>
        [JsonProperty("word")]
        public Word Word { get; set; }
    }
}