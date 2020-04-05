using System.Linq;

namespace PDFDataExtraction.Generic.Models
{
    public class Document
    {
        /// <summary>
        ///     The pages of the document
        /// </summary>
        public Page[] Pages { get; set; }

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