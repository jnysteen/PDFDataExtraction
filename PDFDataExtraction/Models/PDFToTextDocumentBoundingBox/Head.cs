using System.Collections.Generic;
using System.Xml.Serialization;

namespace PDFDataExtraction.Models.PDFToTextDocumentBoundingBox
{
    [XmlRoot(ElementName="head", Namespace="http://www.w3.org/1999/xhtml")]
    public class Head {
        [XmlElement(ElementName="title", Namespace="http://www.w3.org/1999/xhtml")]
        public string Title { get; set; }
        [XmlElement(ElementName="meta", Namespace="http://www.w3.org/1999/xhtml")]
        public List<Meta> Meta { get; set; }
    }
}