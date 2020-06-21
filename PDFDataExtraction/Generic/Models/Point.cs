using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace PDFDataExtraction.Generic.Models
{
    /// <summary>
    ///     A 2D point
    /// </summary>
    [DataContract]
    public class Point
    {
        /// <summary>
        ///     The X coordinate of the point
        /// </summary>
        [Required]
        [DataMember(Order = 1)]
        public double X { get; set; }

        /// <summary>
        ///     The Y coordinate of the point
        /// </summary>
        [Required]
        [DataMember(Order = 2)]
        public double Y { get; set; }
    }
}