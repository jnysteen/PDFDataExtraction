using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PDFDataExtraction.Exceptions;
using PDFDataExtraction.Generic;
using PDFDataExtraction.Helpers;

namespace PDFDataExtraction.PdfImageConversion
{
    public class GhostScriptWrapper : IPDFToImagesConverter
    {
        private readonly ILogger<GhostScriptWrapper> _logger;

        public GhostScriptWrapper(ILogger<GhostScriptWrapper> logger)
        {
            _logger = logger;
        }
        
        public async Task<PageAsImage[]> ConvertPDFToPNGs(string inputFilePath, CancellationToken cancellationToken)
        {
            var tempWorkingFolder = CreateTemporaryDirectory();

            try
            {
                var applicationName = "gs";
                var args = $"-dTextAlphaBits=4 -dGraphicsAlphaBits=4 -r300 -sDEVICE=png16m -o {Path.Combine(tempWorkingFolder, "%03d.png")} {inputFilePath}";

                var (statusCode, stdOutput, errorOutput) = await CmdLineHelper.Run(applicationName, args, cancellationToken);

                if (statusCode != 0)
                    throw new PDFDataExtractionException($"{applicationName} exited with status code: {statusCode}, error output: '{errorOutput}'");

                var extractedImageFiles = Directory.GetFiles(tempWorkingFolder).OrderBy(e => e).ToArray();

                var extractedImages = new PageAsImage[extractedImageFiles.Length];
                for (var i = 0; i < extractedImageFiles.Length; i++)
                {
                    var extractedImageFile = extractedImageFiles[i];
                    var imageBytes = File.ReadAllBytes(extractedImageFile);

                    int imageHeight = 0;
                    int imageWidth = 0;

                    await using (var ms = new MemoryStream(imageBytes))
                    {
                        using var image = Image.FromStream(ms);
                        imageHeight = image.Height;
                        imageWidth = image.Width;
                    }

                    extractedImages[i] = new PageAsImage()
                    {
                        Contents = imageBytes,
                        PageNumber = i + 1,
                        ImageHeight = imageHeight,
                        ImageWidth = imageWidth
                    };
                }

                return extractedImages;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Conversion of PDF to PNGs failed");
                throw;
            }
            finally
            {
                Directory.Delete(tempWorkingFolder, true);
            }
        }
        
        private string CreateTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}