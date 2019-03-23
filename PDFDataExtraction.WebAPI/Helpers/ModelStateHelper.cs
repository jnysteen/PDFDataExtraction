using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PDFDataExtraction.WebAPI.Helpers
{
    public static class ModelStateHelper
    {
        public static string PrettyPrint(this ModelStateDictionary.ValueEnumerable valueEnumerable)
        {
            var sb = new StringBuilder();

            foreach (var v in valueEnumerable)
            {
                sb.Append($"Errors: ");
                for (var i = 0; i < v.Errors.Count; i++)
                {
                    var e = v.Errors[i];
                    sb.Append($"'{e.ErrorMessage}'");

                    if (i < v.Errors.Count - 1)
                        sb.Append(", ");
                }
            }

            return sb.ToString();
        }
    }
}