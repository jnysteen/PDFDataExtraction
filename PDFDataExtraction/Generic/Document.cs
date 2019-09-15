using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PDFDataExtraction.Generic
{
    public class Document
    {
        public Page[] Pages { get; set; }

        public string GetAsString()
        {
            var sb = new StringBuilder();

            for (var i = 0; i < Pages.Length; i++)
            {
                var page = Pages[i];

                sb.AppendLine($"-------------- Page {i + 1}/{Pages.Length} --------------");

                foreach (var line in page.Lines)
                {
                    foreach (var lineWord in line.Words)
                    {
                        sb.Append(lineWord.Text);
                        sb.Append(" ");
                    }

                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }
    }
}