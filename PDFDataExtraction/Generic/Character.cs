using Newtonsoft.Json;

namespace PDFDataExtraction.Generic
{
    public class Character
    {
        [JsonProperty("bbox")] 
        public BoundingBox BoundingBox { get; set; }
        public string Text { get; set; }
        public string Font { get; set; }
    }
}