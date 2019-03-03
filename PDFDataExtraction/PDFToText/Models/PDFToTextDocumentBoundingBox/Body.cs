using System.Xml.Serialization;

namespace PDFDataExtraction.PDFToText.Models.PDFToTextDocumentBoundingBox
{
    [XmlRoot(ElementName="body", Namespace="http://www.w3.org/1999/xhtml")]
    public class Body {
        [XmlElement(ElementName="doc", Namespace="http://www.w3.org/1999/xhtml")]
        public Doc Doc { get; set; }
    }
}