using System.Linq;
using System.Runtime.Serialization;

namespace PDFDataExtraction.Generic.Models
{
    [DataContract]
    public class Document
    {
        /// <summary>
        ///     The pages of the document
        /// </summary>
        [DataMember(Order = 1)]
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