using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PDFDataExtraction.Generic.Models
{
    /// <summary>
    ///     Metadata from the input PDF file
    /// </summary>
    [DataContract]
    public class PDFFileMetadata
    {
        /// <summary>
        ///     The MD5 hash of the PDF file
        /// </summary>
        [JsonProperty("fileMd5")]
        [DataMember(Order = 1)]
        public string FileMd5 { get; set; }

        /// <summary>
        ///     The name of the input PDF file
        /// </summary>
        [JsonProperty("fileName")]
        [DataMember(Order = 2)]
        public string FileName { get; set; }
        
        /// <summary>
        ///     The MD5 hash of the textual contents of the PDF file, as extracted by this version
        /// </summary>
        [JsonProperty("textMd5")]
        [DataMember(Order = 3)]
        public string TextMd5 { get; set; }
    }
}