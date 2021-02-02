using System;
using System.Runtime.Serialization;

namespace PDFDataExtraction.Exceptions
{
    public class NoDataExtractedException : PDFDataExtractionException
    {
        public NoDataExtractedException()
        {
        }

        protected NoDataExtractedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public NoDataExtractedException(string message) : base(message)
        {
        }

        public NoDataExtractedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}