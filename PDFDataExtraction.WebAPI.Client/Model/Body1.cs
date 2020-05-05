/* 
 * PDF Data Extraction
 *
 * No description provided (generated by Swagger Codegen https://github.com/swagger-api/swagger-codegen)
 *
 * OpenAPI spec version: v1
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SwaggerDateConverter = PDFDataExtraction.WebAPI.Client.Client.SwaggerDateConverter;

namespace PDFDataExtraction.WebAPI.Client.Model
{
    /// <summary>
    /// Body1
    /// </summary>
    [DataContract]
        public partial class Body1 :  IEquatable<Body1>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Body1" /> class.
        /// </summary>
        /// <param name="_file">The PDF to extract text from.</param>
        public Body1(byte[] _file = default(byte[]))
        {
            this.File = _file;
        }
        
        /// <summary>
        /// The PDF to extract text from
        /// </summary>
        /// <value>The PDF to extract text from</value>
        [DataMember(Name="file", EmitDefaultValue=false)]
        public byte[] File { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Body1 {\n");
            sb.Append("  File: ").Append(File).Append("\n");
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
            return this.Equals(input as Body1);
        }

        /// <summary>
        /// Returns true if Body1 instances are equal
        /// </summary>
        /// <param name="input">Instance of Body1 to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Body1 input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.File == input.File ||
                    (this.File != null &&
                    this.File.Equals(input.File))
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
                if (this.File != null)
                    hashCode = hashCode * 59 + this.File.GetHashCode();
                return hashCode;
            }
        }
    }
}
