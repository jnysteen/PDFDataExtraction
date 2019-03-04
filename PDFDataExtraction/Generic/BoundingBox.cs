namespace PDFDataExtraction.Generic
{
    public class BoundingBox
    {
        public Point TopLeftCorner { get; set; }
        public Point BottomRightCorner { get; set; }

        public double Height => TopLeftCorner.Y - BottomRightCorner.Y; 
        public double Width => BottomRightCorner.X - TopLeftCorner.X; 
    }
}