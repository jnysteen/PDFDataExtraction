using System.Collections.Generic;
using System.Xml.Serialization;

namespace PDFDataExtraction.PDFToText.Models.PDFToTextDocumentBoundingBox
{
    [XmlRoot(ElementName="page", Namespace="http://www.w3.org/1999/xhtml")]
    public class Page {
        [XmlElement(ElementName="word", Namespace="http://www.w3.org/1999/xhtml")]
        public List<Word> Word { get; set; }
        [XmlAttribute(AttributeName="width")]
        public double Width { get; set; }
        [XmlAttribute(AttributeName="height")]
        public double Height { get; set; }
    }
}