using System.Collections.Generic;
using System.Xml.Serialization;

namespace PDFDataExtraction.PDFToText.Models.PDFToTextDocumentBoundingBoxLayout
{
    [XmlRoot(ElementName="flow", Namespace="http://www.w3.org/1999/xhtml")]
    public class Flow {
        [XmlElement(ElementName="block", Namespace="http://www.w3.org/1999/xhtml")]
        public List<Block> Blocks { get; set; }
    }
}