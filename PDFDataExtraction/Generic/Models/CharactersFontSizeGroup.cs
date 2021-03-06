using System.Collections.Generic;
using System.Linq;

namespace PDFDataExtraction.Generic.Models
{
    internal class CharactersFontSizeGroup
    {
        public HashSet<Character> Characters { get; set; }
        public double AverageHeight => Characters.Average(w => w.BoundingBox.Height);
    }
}