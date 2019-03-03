using System.Collections.Generic;
using System.Xml.Serialization;

namespace PDFDataExtraction.PDFToText.Models.PDFToTextDocumentBoundingBox
{
    [XmlRoot(ElementName="doc", Namespace="http://www.w3.org/1999/xhtml")]
    public class Doc {
        [XmlElement(ElementName="page", Namespace="http://www.w3.org/1999/xhtml")]
        public List<Page> Page { get; set; }
    }
}