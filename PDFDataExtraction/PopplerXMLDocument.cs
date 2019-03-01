using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace PDFDataExtraction
{
	[XmlRoot(ElementName="meta", Namespace="http://www.w3.org/1999/xhtml")]
	public class Meta {
		[XmlAttribute(AttributeName="name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName="content")]
		public string Content { get; set; }
	}

	[XmlRoot(ElementName="head", Namespace="http://www.w3.org/1999/xhtml")]
	public class Head {
		[XmlElement(ElementName="title", Namespace="http://www.w3.org/1999/xhtml")]
		public string Title { get; set; }
		[XmlElement(ElementName="meta", Namespace="http://www.w3.org/1999/xhtml")]
		public List<Meta> Meta { get; set; }
	}

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

	[XmlRoot(ElementName="flow", Namespace="http://www.w3.org/1999/xhtml")]
	public class Flow {
		[XmlElement(ElementName="block", Namespace="http://www.w3.org/1999/xhtml")]
		public List<Block> Blocks { get; set; }
	}

	[XmlRoot(ElementName="page", Namespace="http://www.w3.org/1999/xhtml")]
	public class Page {
		[XmlElement(ElementName="flow", Namespace="http://www.w3.org/1999/xhtml")]
		public List<Flow> Flows { get; set; }
		[XmlAttribute(AttributeName="width")]
		public double Width { get; set; }
		[XmlAttribute(AttributeName="height")]
		public double Height { get; set; }
	}

	[XmlRoot(ElementName="doc", Namespace="http://www.w3.org/1999/xhtml")]
	public class Doc {
		[XmlElement(ElementName="page", Namespace="http://www.w3.org/1999/xhtml")]
		public List<Page> Pages { get; set; }
	}

	[XmlRoot(ElementName="body", Namespace="http://www.w3.org/1999/xhtml")]
	public class Body {
		[XmlElement(ElementName="doc", Namespace="http://www.w3.org/1999/xhtml")]
		public Doc Doc { get; set; }
	}

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
