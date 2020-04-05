using System.Linq;
using System.Text.Json.Serialization;

namespace PDFDataExtraction.Generic
{
    public class Word
    {
        [JsonPropertyName("bbox")]
        public BoundingBox BoundingBox { get; set; } 
        
        [JsonPropertyName("chars")]
        public Character[] Characters { get; set; }
        
        /// <summary>
        ///     The line that this word is in
        /// </summary>
        [JsonPropertyName("line")]
        public Line Line { get; set; }
        

        public string Text => string.Join("", Characters.Select(c => c.Text));
        
        public override string ToString()
        {
            return Text;
        }
    }
}