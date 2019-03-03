using System.Xml.Serialization;

namespace PDFDataExtraction.Models.PDFToTextDocumentBoundingBoxLayout
{
    [XmlRoot(ElementName="html", Namespace="http://www.w3.org/1999/xhtml")]
    public class Html {
        [XmlElement(ElementName="head", Namespace="http://www.w3.org/1999/xhtml")]
        public Head Head { get; set; }
        [XmlElement(ElementName="body", Namespace="http://www.w3.org/1999/xhtml")]
        public Body Body { get; set; }
        [XmlAttribute(AttributeName="xmlns")]
        public string Xmlns { get; set; }
    }
}