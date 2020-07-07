using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDFDataExtraction.Exceptions;
using PDFDataExtraction.Generic.Models;
using PDFDataExtraction.Helpers;

namespace PDFDataExtraction.Generic
{
    public class PDFMetadataProvider : IPDFMetadataProvider
    {
        public PDFMetadataProvider()
        {
        }
        
        public string GetFileMd5(string filePath)
        {
            using var fileStream = File.OpenRead(filePath);
            using var md5Hasher = MD5.Create();
            var hash = md5Hasher.ComputeHash(fileStream);
            var base64String = ByteArrayToHex(hash);
            return base64String;
        }

        public string GetDocumentTextMd5(Document document)
        {
            var documentTextAsBytes = Encoding.UTF8.GetBytes(document.GetAsString());
            using var md5Hasher = MD5.Create();
            var hash = md5Hasher.ComputeHash(documentTextAsBytes);
            var base64String = ByteArrayToHex(hash);
            return base64String;
        }
        
        private static string ByteArrayToHex(byte[] bytes)
        {
            var result = new StringBuilder(bytes.Length * 2);

            foreach (var b in bytes)
                result.Append(b.ToString("X2"));

            return result.ToString();
        }

    }

    public class PDFEmbeddedMetadata
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Producer { get; set; }
        public string Creator { get; set; }
        public string Keywords { get; set; }
        public DateTimeOffset? CreationDate { get; set; }
        public DateTimeOffset? ModificationDate { get; set; }

        public static PDFEmbeddedMetadata CreateFromMetadataScriptOutput(Dictionary<string, string> metadataScriptOutput)
        {
            return new PDFEmbeddedMetadata()
            {
                Title = SafeTryGetParse(metadataScriptOutput, "/Title", s => s?.Trim()),
                Author = SafeTryGetParse(metadataScriptOutput, "/Author", s => s?.Trim()),
                Producer = SafeTryGetParse(metadataScriptOutput, "/Producer", s => s?.Trim()),
                Creator = SafeTryGetParse(metadataScriptOutput, "/Creator", s => s?.Trim()),
                Keywords = SafeTryGetParse(metadataScriptOutput, "/Keywords", s => s?.Trim()),
                CreationDate = SafeTryGetParse(metadataScriptOutput, "/CreationDate", SafeParseDateTimeOffset),
                ModificationDate = SafeTryGetParse(metadataScriptOutput, "/ModDate", SafeParseDateTimeOffset),
            };
        }

        private static T SafeTryGetParse<T>(Dictionary<string, string> dictionary, string keyName, Func<string, T> keyparser)
        {
            return !dictionary.TryGetValue(keyName, out var value) ? default : keyparser(value);
        }

        internal static DateTimeOffset? SafeParseDateTimeOffset(string datetimeoffsetRaw)
        {
            if (datetimeoffsetRaw == null)
                return null;

            try
            {
                var dateTimeOffsetRegex = new Regex(@"^D:(?<Year>\d{4})(?<Month>\d{2})(?<Day>\d{2})(?<Hours>\d{2})(?<Minutes>\d{2})(?<Seconds>\d{2})(?<TimeZone>Z?(((?<TimeZoneSign>(\+|\-)?)(?<TimeZoneHours>\d{2})\'(?<TimeZoneMinutes>\d{2})\'))?)", RegexOptions.ExplicitCapture);

                var match = dateTimeOffsetRegex.Match(datetimeoffsetRaw);

                if (!match.Success)
                    return null;

                var year = int.Parse(match.Groups["Year"].Value);
                var month = int.Parse(match.Groups["Month"].Value);
                var day = int.Parse(match.Groups["Day"].Value);
                var hours = int.Parse(match.Groups["Hours"].Value);
                var minutes = int.Parse(match.Groups["Minutes"].Value);
                var seconds = int.Parse(match.Groups["Seconds"].Value);
                
                var timeZone = match.Groups["TimeZone"];

                var offset = TimeSpan.Zero;
                
                if (timeZone.Value.StartsWith("Z")) 
                    return new DateTimeOffset(year, month, day, hours, minutes, seconds, offset);
                
                var timeZoneSign = match.Groups["TimeZoneSign"].Value;
                var timeZoneHours = int.Parse(match.Groups["TimeZoneHours"].Value);
                var timeZoneMinutes = int.Parse(match.Groups["TimeZoneMinutes"].Value);

                var totalMinutes = (timeZoneHours * 60) + timeZoneMinutes;

                switch (timeZoneSign)
                {
                    case "+":
                        break;
                    case "-":
                        totalMinutes *= -1;
                        break;
                    default:
                        // Regex match went wrong
                        return null;
                }
                    
                offset = TimeSpan.FromMinutes(totalMinutes);

                return new DateTimeOffset(year, month, day, hours, minutes, seconds, offset);

            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}