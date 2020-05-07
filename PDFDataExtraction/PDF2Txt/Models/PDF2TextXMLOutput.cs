/* 
    Licensed under the Apache License, Version 2.0
    
    http://www.apache.org/licenses/LICENSE-2.0
    */

using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace PDFDataExtraction.PDF2Txt.Models
{
    [XmlRoot(ElementName = "text")]
    public class TextNode
    {
        [XmlAttribute(AttributeName = "font")]
        public string Font { get; set; }

        [XmlAttribute(AttributeName = "bbox")]
        public string Bbox { get; set; }

        [XmlAttribute(AttributeName = "colourspace")]
        public string Colourspace { get; set; }

        [XmlAttribute(AttributeName = "ncolour")]
        public string Ncolour { get; set; }

        [XmlAttribute(AttributeName = "size")]
        public double Size { get; set; }

        [XmlText]
        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }

    [XmlRoot(ElementName = "textline")]
    public class Textline
    {
        [XmlElement(ElementName = "text")]
        public List<TextNode> TextParts { get; set; }

        [XmlAttribute(AttributeName = "bbox")]
        public string Bbox { get; set; }

        public override string ToString()
        {
            return (TextParts != null && TextParts.Any()) ? string.Join("", TextParts.Select(t => t.ToString())) : "";
        }
    }

    [XmlRoot(ElementName = "textbox")]
    public class TextBox
    {
        [XmlElement(ElementName = "textline")]
        public List<Textline> Textlines { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "bbox")]
        public string Bbox { get; set; }

        public override string ToString()
        {
            return (Textlines != null && Textlines.Any()) ? string.Join("\n", Textlines.Select(t => t.ToString())) : "";
        }
    }

    [XmlRoot(ElementName = "curve")]
    public class Curve
    {
        [XmlAttribute(AttributeName = "linewidth")]
        public double Linewidth { get; set; }

        [XmlAttribute(AttributeName = "bbox")]
        public string Bbox { get; set; }

        [XmlAttribute(AttributeName = "pts")]
        public string Pts { get; set; }
    }

    [XmlRoot(ElementName = "rect")]
    public class Rect
    {
        [XmlAttribute(AttributeName = "linewidth")]
        public string Linewidth { get; set; }

        [XmlAttribute(AttributeName = "bbox")]
        public string Bbox { get; set; }
    }

    [XmlRoot(ElementName = "textgroup")]
    public class Textgroup
    {
        [XmlElement(ElementName = "textbox")]
        public List<TextBox> Textbox { get; set; }

        [XmlAttribute(AttributeName = "bbox")]
        public string Bbox { get; set; }

        [XmlElement(ElementName = "textgroup")]
        public List<Textgroup> Textgroups { get; set; }

        public override string ToString()
        {
            return (Textbox != null && Textbox.Any()) ? string.Join("\n", Textbox.Select(t => t.ToString())) : "";
        }
    }

    [XmlRoot(ElementName = "layout")]
    public class Layout
    {
        [XmlElement(ElementName = "textgroup")]
        public Textgroup Textgroup { get; set; }
    }

    [XmlRoot(ElementName = "page")]
    public class Page
    {
        [XmlElement(ElementName = "textbox")]
        public List<TextBox> Textbox { get; set; }

        [XmlElement(ElementName = "curve")]
        public List<Curve> Curve { get; set; }

        [XmlElement(ElementName = "rect")]
        public List<Rect> Rect { get; set; }

        [XmlElement(ElementName = "layout")]
        public Layout Layout { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "bbox")]
        public string Bbox { get; set; }

        [XmlAttribute(AttributeName = "rotate")]
        public string Rotate { get; set; }

        [XmlElement(ElementName = "line")]
        public List<Line> Line { get; set; }

        [XmlElement(ElementName = "figure")]
        public List<Figure> Figure { get; set; }

        public override string ToString()
        {
            return (Textbox != null && Textbox.Any()) ? string.Join("\n", Textbox.Select(t => t.ToString())) : "";
        }
    }

    [XmlRoot(ElementName = "line")]
    public class Line
    {
        [XmlAttribute(AttributeName = "linewidth")]
        public string Linewidth { get; set; }

        [XmlAttribute(AttributeName = "bbox")]
        public string Bbox { get; set; }
    }

    [XmlRoot(ElementName = "image")]
    public class Image
    {
        [XmlAttribute(AttributeName = "width")]
        public string Width { get; set; }

        [XmlAttribute(AttributeName = "height")]
        public string Height { get; set; }
    }

    [XmlRoot(ElementName = "figure")]
    public class Figure
    {
        [XmlElement(ElementName = "image")]
        public Image Image { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "bbox")]
        public string Bbox { get; set; }
        
        [XmlElement(ElementName = "text")]
        public List<TextNode> TextParts { get; set; }
    }

    [XmlRoot(ElementName = "pages")]
    public class Pages
    {
        [XmlElement(ElementName = "page")]
        public List<Page> Page { get; set; }
    }
}