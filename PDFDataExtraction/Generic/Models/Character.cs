using System.Text.Json.Serialization;

namespace PDFDataExtraction.Generic
{
    public class Character
    {
        /// <summary>
        ///     A box that fits the character
        /// </summary>
        [JsonPropertyName("bbox")] 
        public BoundingBox BoundingBox { get; set; }

        /// <summary>
        ///     The character itself
        /// </summary>
        [JsonPropertyName("text")] 
        public string Text { get; set; }

        /// <summary>
        ///     The font used to render the character
        /// </summary>
        [JsonPropertyName("font")] 
        public string Font { get; set; }

        /// <summary>
        ///     The word that this character is in
        /// </summary>
        [JsonPropertyName("word")]
        public Word Word { get; set; }
    }
}