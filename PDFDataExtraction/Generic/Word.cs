using System.Linq;

namespace PDFDataExtraction.Generic
{
    public class Word
    {
        public double XMin { get; set; }
        public double XMax { get; set; }
        
        public double YMin { get; set; }
        public double YMax { get; set; }
        
        public Character[] Characters { get; set; }

        public string Text => string.Join("", Characters.Select(c => c.Text));
    }
}