using System.Xml.Serialization;

namespace PDFDataExtraction.PDFToText.Models.PDFToTextDocumentBoundingBox
{
    [XmlRoot(ElementName="word", Namespace="http://www.w3.org/1999/xhtml")]
    public class Word {
        [XmlAttribute(AttributeName="xMin")]
        public double XMin { get; set; }
        [XmlAttribute(AttributeName="yMin")]
        public double YMin { get; set; }
        [XmlAttribute(AttributeName="xMax")]
        public double XMax { get; set; }
        [XmlAttribute(AttributeName="yMax")]
        public double YMax { get; set; }
        [XmlText]
        public string Text { get; set; }
    }
}