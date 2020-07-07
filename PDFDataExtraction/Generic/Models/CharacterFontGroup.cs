using System.Collections.Generic;

namespace PDFDataExtraction.Generic.Models
{
    public class CharacterFontGroup
    {
        /// <summary>
        ///     The ID of the font. The ID is unique in the context of the document. 
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        ///     The name of the font as described in the document
        /// </summary>
        public List<string> CharacterIds { get; set; }
    }
}