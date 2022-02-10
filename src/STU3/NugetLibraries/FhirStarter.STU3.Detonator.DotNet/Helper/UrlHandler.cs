using System;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Http;

namespace FhirStarter.STU3.Detonator.DotNetCore3.Helper
{
    public static class UrlHandler
    {
        public static string GetUrlForOperationDefinition(HttpContext context, string fhirUrlParameter, string alias)
        {
            return GetParentUrl(context, fhirUrlParameter) + fhirUrlParameter + nameof(OperationDefinition) + "/" + alias;
        }

        public static string GetUrlForOperationDefinition(string urlPath, string fhirUrlParameter, string alias)
        {
            urlPath = urlPath.Replace("metadata","");
            var combinedUrl = $"{urlPath}{fhirUrlParameter}{nameof(OperationDefinition)}/{alias}";
            return combinedUrl;
        }

        private static string GetParentUrl(HttpContext context, string fhirUrlParameter)
        {
            var originalPath = GetContextRequestUrl(context);

            if (originalPath.Contains(fhirUrlParameter))
            {
                var subString = originalPath.Substring(0,
                    GetContextRequestUrl(context).IndexOf(fhirUrlParameter, 0, StringComparison.Ordinal));
                return subString;
            }
            return originalPath;
        }

        private static string GetContextRequestUrl(HttpContext context)
        {
            var currentUrl = context.Request.Path;
            var path = currentUrl.Value;

            var uri = new Uri(path);

            var originalString = uri.OriginalString;
            return originalString;
        }
    }
}
