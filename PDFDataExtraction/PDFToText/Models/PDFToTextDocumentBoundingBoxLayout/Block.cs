using System.Collections.Generic;
using System.Xml.Serialization;

namespace PDFDataExtraction.PDFToText.Models.PDFToTextDocumentBoundingBoxLayout
{
    [XmlRoot(ElementName="block", Namespace="http://www.w3.org/1999/xhtml")]
    public class Block {
        [XmlElement(ElementName="line", Namespace="http://www.w3.org/1999/xhtml")]
        public List<Line> Lines { get; set; }
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