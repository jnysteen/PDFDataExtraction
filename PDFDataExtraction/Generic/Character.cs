using Newtonsoft.Json;

namespace PDFDataExtraction.Generic
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
        public string Text { get; set; }

        /// <summary>
        ///     The font used to render the character
        /// </summary>
        public string Font { get; set; }
    }
}