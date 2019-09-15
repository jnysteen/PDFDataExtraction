using Newtonsoft.Json;

namespace PDFDataExtraction.Generic
{
    public class BoundingBox
    {
        public Point TopLeftCorner { get; set; }
        public Point BottomRightCorner { get; set; }

        [JsonIgnore]
        public double Height => BottomRightCorner.Y -TopLeftCorner.Y;
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