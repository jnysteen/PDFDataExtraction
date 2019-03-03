using System.Collections.Generic;
using System.Xml.Serialization;

namespace PDFDataExtraction.Models.PDFToTextDocumentBoundingBoxLayout
{
    [XmlRoot(ElementName="line", Namespace="http://www.w3.org/1999/xhtml")]
    public class Line {
        [XmlElement(ElementName="word", Namespace="http://www.w3.org/1999/xhtml")]
        public List<Word> Words { get; set; }
        [XmlAttribute(AttributeName="xMin")]
        public double XMin { get; set; }
        [XmlAttribute(AttributeName="yMin")]
        public double YMin { get; set; }
        [XmlAttribute(AttributeName="xMax")]
        public double XMax { get; set; }
        [XmlAttribute(AttributeName="yMax")]
        public double YMax { get; set; }
    }
}