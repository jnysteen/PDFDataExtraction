using System;
using System.ComponentModel;
// ReSharper disable StringLiteralTypo

namespace PDFDataExtraction.Models
{
    public class PDFToTextArgs
    {
        # region Args with values
        
        /// <summary>
        /// first page to convert
        /// </summary>
        [DisplayName("f")]
        public int? FirstPageToConvert { get; set; }
        
        /// <summary>
        /// last page to convert
        /// </summary>
        [DisplayName("l")]
        public int? LastPageToConvert { get; set; }
        
        /// <summary>
        /// resolution, in DPI (default is 72)
        /// </summary>
        [DisplayName("r")]
        public double? Resolution { get; set; }
        
        /// <summ
        /// <summary>
        /// output text encoding name
        /// </summary>
        [DisplayName("enc")]
        public string Encoding { get; set; }
        
        /// <summary>
        /// assume fixed-pitch (or tabular) text
        /// </summary>
        [DisplayName("fixed")]
        public double? FixedPitch { get; set; }
        
        /// <summary>
        /// output end-of-line convention (unix, dos, or mac)
        /// </summary>
        [DisplayName("eol")]
        public string EndOfLine { get; set; }
        
        /// <summary>
        /// owner password (for encrypted files)
        /// </summary>
        [DisplayName("opw")]
        public string OwnerPassword { get; set; }
        
        /// <summary>
        /// user password (for encrypted files)
        /// </summary>
        [DisplayName("upw")]
        public string UserPassword { get; set; }

        #endregion
        
        
        #region Flags (boolean args)
        
        /// <summary>
        /// keep strings in content stream order  
        /// </summary>
        [DisplayName("raw")]
        public bool RawContentStreamOrder { get; set;}     

        /// <summary>
        /// don't insert page breaks between pages
        /// </summary>
        [DisplayName("nopgbrk")]
        public bool DontInsertPageBreaks { get; set;}     

        /// <summary>
        /// output bounding box for each word and page size to html.
        /// </summary>
        [DisplayName("bbox")]
        public bool OutputBoundingBox { get; set;}     

        /// <summary>
        /// like -bbox but with extra layout bounding box data.
        /// </summary>
        [DisplayName("bbox-layout")]
        public bool OutputBoundingBoxLayout { get; set;}    

        /// <summary>
        /// maintain original physical layout
        /// </summary>
        [DisplayName("layout")]
        public bool MaintainLayout { get; set;}     

        #endregion
        
        public string GetArgsAsString()
        {
            throw new NotImplementedException();
        }
        
    }
}