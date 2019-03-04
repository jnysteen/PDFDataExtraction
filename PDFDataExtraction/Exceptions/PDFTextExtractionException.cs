using System;
using System.Runtime.Serialization;

namespace PDFDataExtraction.Exceptions
{
    public class PDFTextExtractionException : Exception
    {
        public PDFTextExtractionException()
        {
        }

        protected PDFTextExtractionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public PDFTextExtractionException(string message) : base(message)
        {
        }

        public PDFTextExtractionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}