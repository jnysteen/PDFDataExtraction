using System.Collections.Generic;
using System.Xml.Serialization;

namespace PDFDataExtraction.PDFToText.Models.PDFToTextDocumentBoundingBoxLayout
{
    [XmlRoot(ElementName="page", Namespace="http://www.w3.org/1999/xhtml")]
    public class Page {
        [XmlElement(ElementName="flow", Namespace="http://www.w3.org/1999/xhtml")]
        public List<Flow> Flows { get; set; }
        [XmlAttribute(AttributeName="width")]
        public double Width { get; set; }
        [XmlAttribute(AttributeName="height")]
        public double Height { get; set; }
    }
}