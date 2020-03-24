using System.Text.Json.Serialization;

namespace PDFDataExtraction.Generic
{
    /// <summary>
    ///     A rectangular box
    /// </summary>
    public class BoundingBox
    {
        /// <summary>
        ///     The top left corner of the box
        /// </summary>
        [JsonPropertyName("topLeft")]
        public Point TopLeftCorner { get; set; }

        /// <summary>
        ///     The bottom right corner of the box
        /// </summary>
        [JsonPropertyName("bottomRight")]
        public Point BottomRightCorner { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
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