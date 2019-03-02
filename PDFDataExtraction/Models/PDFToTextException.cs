using System;
using System.Runtime.Serialization;

namespace PDFDataExtraction.Models
{
    public class PDFToTextException : Exception
    {
        public PDFToTextException()
        {
        }

        protected PDFToTextException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public PDFToTextException(string message) : base(message)
        {
        }

        public PDFToTextException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}