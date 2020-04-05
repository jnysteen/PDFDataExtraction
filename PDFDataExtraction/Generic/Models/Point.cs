using System.ComponentModel.DataAnnotations;

namespace PDFDataExtraction.Generic.Models
{
    /// <summary>
    ///     A 2D point
    /// </summary>
    public class Point
    {
        /// <summary>
        ///     The X coordinate of the point
        /// </summary>
        [Required]
        public double X { get; set; }

        /// <summary>
        ///     The Y coordinate of the point
        /// </summary>
        [Required]
        public double Y { get; set; }
    }
}