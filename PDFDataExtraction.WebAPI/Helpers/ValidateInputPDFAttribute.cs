using System.ComponentModel.DataAnnotations;
using JNysteen.FileTypeIdentifier.Interfaces;
using JNysteen.FileTypeIdentifier.MagicNumbers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PDFDataExtraction.WebAPI.Helpers
{
    public class ValidateInputPDFAttribute : ActionFilterAttribute
    {
        private readonly IFileTypeIdentifier _fileTypeIdentifier;

        public ValidateInputPDFAttribute(IFileTypeIdentifier fileTypeIdentifier)
        {
            _fileTypeIdentifier = fileTypeIdentifier;
        }
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var inputFileArgumentName = "file";
            var foundFileInContext = context.ActionArguments.TryGetValue(inputFileArgumentName, out var inputFile);

            string errorMessage = null;

            if (!foundFileInContext)
                errorMessage = "Could not find a file in the request!";
            else
            {
                if (inputFile == null || !(inputFile is IFormFile asFormFile))
                {
                    errorMessage = $"Could not find a file in the request!";
                }
                else
                {
                    string identifiedFileType;

                    using (var fileStream = asFormFile.OpenReadStream())
                        identifiedFileType = _fileTypeIdentifier.GetFileType(fileStream);
    
                    if (identifiedFileType == null || identifiedFileType != DocumentMagicNumbers.PDF)
                    {
                        errorMessage = "Provided file was not a PDF!";
                    }
                }
            }

            if (errorMessage != null)
            {
                context.ModelState.AddModelError(inputFileArgumentName, errorMessage);
            }
        }
    }
}