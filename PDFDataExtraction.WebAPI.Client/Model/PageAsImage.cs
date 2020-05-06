/* 
 * PDF Data Extraction
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: v1
 * 
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using OpenAPIDateConverter = PDFDataExtraction.WebAPI.Client.Client.OpenAPIDateConverter;

namespace PDFDataExtraction.WebAPI.Client.Model
{
    /// <summary>
    /// PageAsImage
    /// </summary>
    [DataContract]
    public partial class PageAsImage :  IEquatable<PageAsImage>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageAsImage" /> class.
        /// </summary>
        /// <param name="contents">contents.</param>
        /// <param name="pageNumber">pageNumber.</param>
        /// <param name="imageHeight">imageHeight.</param>
        /// <param name="imageWidth">imageWidth.</param>
        public PageAsImage(byte[] contents = default(byte[]), int pageNumber = default(int), int imageHeight = default(int), int imageWidth = default(int))
        {
            this.Contents = contents;
            this.PageNumber = pageNumber;
            this.ImageHeight = imageHeight;
            this.ImageWidth = imageWidth;
        }
        
        /// <summary>
        /// Gets or Sets Contents
        /// </summary>
        [DataMember(Name="contents", EmitDefaultValue=true)]
        public byte[] Contents { get; set; }

        /// <summary>
        /// Gets or Sets PageNumber
        /// </summary>
        [DataMember(Name="pageNumber", EmitDefaultValue=false)]
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or Sets ImageHeight
        /// </summary>
        [DataMember(Name="imageHeight", EmitDefaultValue=false)]
        public int ImageHeight { get; set; }

        /// <summary>
        /// Gets or Sets ImageWidth
        /// </summary>
        [DataMember(Name="imageWidth", EmitDefaultValue=false)]
        public int ImageWidth { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class PageAsImage {\n");
            sb.Append("  Contents: ").Append(Contents).Append("\n");
            sb.Append("  PageNumber: ").Append(PageNumber).Append("\n");
            sb.Append("  ImageHeight: ").Append(ImageHeight).Append("\n");
            sb.Append("  ImageWidth: ").Append(ImageWidth).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as PageAsImage);
        }

        /// <summary>
        /// Returns true if PageAsImage instances are equal
        /// </summary>
        /// <param name="input">Instance of PageAsImage to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(PageAsImage input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Contents == input.Contents ||
                    (this.Contents != null &&
                    this.Contents.Equals(input.Contents))
                ) && 
                (
                    this.PageNumber == input.PageNumber ||
                    this.PageNumber.Equals(input.PageNumber)
                ) && 
                (
                    this.ImageHeight == input.ImageHeight ||
                    this.ImageHeight.Equals(input.ImageHeight)
                ) && 
                (
                    this.ImageWidth == input.ImageWidth ||
                    this.ImageWidth.Equals(input.ImageWidth)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.Contents != null)
                    hashCode = hashCode * 59 + this.Contents.GetHashCode();
                hashCode = hashCode * 59 + this.PageNumber.GetHashCode();
                hashCode = hashCode * 59 + this.ImageHeight.GetHashCode();
                hashCode = hashCode * 59 + this.ImageWidth.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}
