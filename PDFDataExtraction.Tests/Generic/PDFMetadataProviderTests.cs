using System;
using System.Collections.Generic;
using PDFDataExtraction.Generic;
using Xunit;

namespace PDFDataExtraction.Tests.Generic
{
    public class PDFMetadataProviderTests
    {
        [Theory]
        [MemberData(nameof(EmbeddedDateParsingTestData))]
        public void CanParseEmbeddedDates(string inputDateString, DateTimeOffset? expectedOutput)
        {
            var parsed = PDFEmbeddedMetadata.SafeParseDateTimeOffset(inputDateString);
            Assert.Equal(expectedOutput, parsed);
        }
        
        public static IEnumerable<object[]> EmbeddedDateParsingTestData => new List<object[]>
            {
                new object[] { null, new DateTimeOffset?()},
                new object[] { "D:20200217232824Z", new DateTimeOffset(2020, 02, 17, 23, 28, 24, TimeSpan.Zero)},
                new object[] { "D:20161127184129+00'00'", new DateTimeOffset(2016, 11, 27, 18, 41, 29, TimeSpan.Zero)},
                new object[] { "D:20150211064040-05'00'", new DateTimeOffset(2015, 02, 11, 06, 40, 40, TimeSpan.FromHours(-5))},
                new object[] { "D:20150211064040+05'00'", new DateTimeOffset(2015, 02, 11, 06, 40, 40, TimeSpan.FromHours(+5))},
                new object[] { "D:20150211064040+05'30'", new DateTimeOffset(2015, 02, 11, 06, 40, 40, TimeSpan.FromHours(+5).Add(TimeSpan.FromMinutes(30)))},
            };
        
    }
}