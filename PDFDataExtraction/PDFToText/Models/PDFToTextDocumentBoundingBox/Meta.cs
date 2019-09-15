﻿using System.Xml.Serialization;

namespace PDFDataExtraction.PDFToText.Models.PDFToTextDocumentBoundingBox
{
	[XmlRoot(ElementName="meta", Namespace="http://www.w3.org/1999/xhtml")]
	public class Meta {
		[XmlAttribute(AttributeName="name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName="content")]
		public string Content { get; set; }
	}
}