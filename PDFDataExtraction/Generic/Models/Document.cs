using System.Collections.Generic;
using System.Linq;

namespace PDFDataExtraction.Generic.Models
{
    public class Document
    {
        /// <summary>
        ///     The pages of the document
        /// </summary>
        public Page[] Pages { get; set; }
        
        /// <summary>
        ///     The fonts found in the document along with the IDs of the characters rendered with the specific font.
        /// </summary>
        public CharacterFontGroup[] Fonts { get; set; }

        public string GetAsString()
        {
            return string.Join("\n", Pages.Select(p => p.ToString()));
        }

        public override string ToString()
        {
            return GetAsString();
        }
    }
}