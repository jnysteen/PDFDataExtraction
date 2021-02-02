using System;
using System.IO;
using System.Reflection;

namespace PDFDataExtraction.Tests
{
    public static class TestHelper
    {
        public static string GetTestFilesFolder()
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().Location);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);
            var result = Path.Join(dirPath, "TestFiles");
            return result;
        }
    }
}