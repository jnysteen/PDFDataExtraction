using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace PDFDataExtraction.Generic
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