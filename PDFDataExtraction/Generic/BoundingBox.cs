namespace PDFDataExtraction.Generic
{
    public class BoundingBox
    {
        public Point TopLeftCorner { get; set; }
        public Point BottomRightCorner { get; set; }

        public double Height => BottomRightCorner.Y -TopLeftCorner.Y; 
        public double Width => BottomRightCorner.X - TopLeftCorner.X;

        public double MinX => TopLeftCorner.X;
        public double MaxX => BottomRightCorner.X;
        public double MinY => TopLeftCorner.Y;
        public double MaxY => BottomRightCorner.Y;
    }
}