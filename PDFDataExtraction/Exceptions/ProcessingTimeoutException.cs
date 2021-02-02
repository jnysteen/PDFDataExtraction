using System;
using System.Runtime.Serialization;

namespace PDFDataExtraction.Exceptions
{
    public class ProcessingTimeoutException : PDFDataExtractionException
    {
        public ProcessingTimeoutException()
        {
        }

        protected ProcessingTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ProcessingTimeoutException(string message) : base(message)
        {
        }

        public ProcessingTimeoutException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}