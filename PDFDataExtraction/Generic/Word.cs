using System.Linq;

namespace PDFDataExtraction.Generic
{
    public class Word
    {
        public BoundingBox BoundingBox { get; set; } 
        
        public Character[] Characters { get; set; }

        public string Text => string.Join("", Characters.Select(c => c.Text));
    }
}