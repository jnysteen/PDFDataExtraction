using System.Linq;
using Newtonsoft.Json;

namespace PDFDataExtraction.Generic
{
    public class Word
    {
        [JsonProperty("bbox")]
        public BoundingBox BoundingBox { get; set; } 
        
        [JsonProperty("chars")]
        public Character[] Characters { get; set; }

        public string Text => string.Join("", Characters.Select(c => c.Text));
        
        public override string ToString()
        {
            return Text;
        }
    }
}