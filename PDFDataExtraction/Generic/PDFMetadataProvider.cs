using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PDFDataExtraction.Generic
{
    public class PDFMetadataProvider : IPDFMetadataProvider
    {
        public string GetFileMd5(string filePath)
        {
            using var fileStream = File.OpenRead(filePath);
            using var md5Hasher = MD5.Create();
            var hash = md5Hasher.ComputeHash(fileStream);
            var base64String = Convert.ToBase64String(hash);
            return base64String;
        }

        public string GetDocumentTextMd5(Document document)
        {
            var documentTextAsBytes = Encoding.UTF8.GetBytes(document.GetAsString());
            using var md5Hasher = MD5.Create();
            var hash = md5Hasher.ComputeHash(documentTextAsBytes);
            var base64String = Convert.ToBase64String(hash);
            return base64String;
        }
    }
}