using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

// ReSharper disable StringLiteralTypo

namespace PDFDataExtraction.PDFToText.Models
{
    public class PDFToTextArgs
    {
        private static (Func<PDFToTextArgs, object>, string)[] _argsProperties;
        private static (Func<PDFToTextArgs, object>, string)[] ArgsProperties => _argsProperties ?? (_argsProperties = GetArgsProperties());
        
        
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
        
        internal string GetArgsAsString()
        {
            var stringBuilder = new StringBuilder();
            
            foreach (var argsProperty in ArgsProperties)
            {
                var (propertyValueGetter, displayName) = argsProperty;
                
                var propertyValue = propertyValueGetter(this);

                switch (propertyValue)
                {
                    case null:
                        continue;
                    case bool propertyValueBool:
                    {
                        if (propertyValueBool)
                        {
                            stringBuilder.Append($" -{displayName}");
                        }

                        break;
                    }
                    default:
                        stringBuilder.Append($" -{displayName} {propertyValue}");
                        break;
                }
            }

            return stringBuilder.ToString().Trim();
        }

        private static (Func<PDFToTextArgs, object>, string)[] GetArgsProperties()
        {
            var properties = typeof(PDFToTextArgs).GetProperties();

            var argsProperties = new List<(Func<PDFToTextArgs, object>, string)>();

            foreach (var propertyInfo in properties)
            {
                var attribute = Attribute.GetCustomAttribute(propertyInfo, typeof(DisplayNameAttribute));

                if (!(attribute is DisplayNameAttribute displayNameAttribute)) 
                    continue;
                
                Func<PDFToTextArgs, object> propertyValueGetter = propertyInfo.GetValue;
                var displayName = displayNameAttribute.DisplayName;
                
                argsProperties.Add((propertyValueGetter, displayName));
            }

            return argsProperties.ToArray();
        }
        
    }
}