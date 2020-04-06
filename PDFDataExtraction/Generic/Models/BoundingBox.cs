using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PDFDataExtraction.Generic.Models
{
    /// <summary>
    ///     A rectangular box
    /// </summary>
    public class BoundingBox
    {
        /// <summary>
        ///     The top left corner of the box
        /// </summary>
        [JsonProperty("topLeft")]
        public Point TopLeftCorner { get; set; }

        /// <summary>
        ///     The bottom right corner of the box
        /// </summary>
        [JsonProperty("bottomRight")]
        public Point BottomRightCorner { get; set; }
        
        [JsonIgnore]
        public double Height => BottomRightCorner.Y - TopLeftCorner.Y;
        
        [JsonIgnore]
        public double Width => BottomRightCorner.X - TopLeftCorner.X;
        
        [JsonIgnore]
        public double MinX => TopLeftCorner.X;
        
        [JsonIgnore]
        public double MaxX => BottomRightCorner.X;
        
        [JsonIgnore]
        public double MinY => TopLeftCorner.Y;
        
        [JsonIgnore]
        public double MaxY => BottomRightCorner.Y;
    }
}