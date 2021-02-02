using System;
using System.IO;
using System.Reflection;

namespace PDFDataExtraction.Tests.PDF2Txt.TestFiles
{
    public static class PDF2TxtTestFilesHelper
    {
        public static string SimpleTwoPageDoc => GetTestFilesFolder() + "/simple-two-page-doc.pdf2txt.xml";

        public static string GetTestFilesFolder()
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().Location);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);
            return Path.Combine(dirPath, "PDF2Txt/TestFiles");
        }
    }
}