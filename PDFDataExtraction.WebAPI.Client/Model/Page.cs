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
    /// Page
    /// </summary>
    [DataContract]
    public partial class Page :  IEquatable<Page>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Page" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected Page() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Page" /> class.
        /// </summary>
        /// <param name="lines">lines.</param>
        /// <param name="width">width (required).</param>
        /// <param name="height">height (required).</param>
        /// <param name="pageNumber">pageNumber (required).</param>
        public Page(List<Line> lines = default(List<Line>), double width = default(double), double height = default(double), int pageNumber = default(int))
        {
            this.Width = width;
            this.Height = height;
            this.PageNumber = pageNumber;
            this.Lines = lines;
        }
        
        /// <summary>
        /// Gets or Sets Lines
        /// </summary>
        [DataMember(Name="lines", EmitDefaultValue=true)]
        public List<Line> Lines { get; set; }

        /// <summary>
        /// Gets or Sets Width
        /// </summary>
        [DataMember(Name="width", EmitDefaultValue=false)]
        public double Width { get; set; }

        /// <summary>
        /// Gets or Sets Height
        /// </summary>
        [DataMember(Name="height", EmitDefaultValue=false)]
        public double Height { get; set; }

        /// <summary>
        /// Gets or Sets PageNumber
        /// </summary>
        [DataMember(Name="pageNumber", EmitDefaultValue=false)]
        public int PageNumber { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Page {\n");
            sb.Append("  Lines: ").Append(Lines).Append("\n");
            sb.Append("  Width: ").Append(Width).Append("\n");
            sb.Append("  Height: ").Append(Height).Append("\n");
            sb.Append("  PageNumber: ").Append(PageNumber).Append("\n");
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
            return this.Equals(input as Page);
        }

        /// <summary>
        /// Returns true if Page instances are equal
        /// </summary>
        /// <param name="input">Instance of Page to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Page input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Lines == input.Lines ||
                    this.Lines != null &&
                    input.Lines != null &&
                    this.Lines.SequenceEqual(input.Lines)
                ) && 
                (
                    this.Width == input.Width ||
                    this.Width.Equals(input.Width)
                ) && 
                (
                    this.Height == input.Height ||
                    this.Height.Equals(input.Height)
                ) && 
                (
                    this.PageNumber == input.PageNumber ||
                    this.PageNumber.Equals(input.PageNumber)
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
                if (this.Lines != null)
                    hashCode = hashCode * 59 + this.Lines.GetHashCode();
                hashCode = hashCode * 59 + this.Width.GetHashCode();
                hashCode = hashCode * 59 + this.Height.GetHashCode();
                hashCode = hashCode * 59 + this.PageNumber.GetHashCode();
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
