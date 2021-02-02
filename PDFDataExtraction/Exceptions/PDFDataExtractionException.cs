using System;
using System.Runtime.Serialization;

namespace PDFDataExtraction.Exceptions
{
    public class PDFDataExtractionException : Exception
    {
        public PDFDataExtractionException()
        {
        }

        protected PDFDataExtractionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public PDFDataExtractionException(string message) : base(message)
        {
        }

        public PDFDataExtractionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}