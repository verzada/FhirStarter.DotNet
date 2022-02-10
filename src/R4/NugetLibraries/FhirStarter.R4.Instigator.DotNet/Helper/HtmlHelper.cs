using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace FhirStarter.R4.Instigator.DotNet.Helper
{
    public static class HtmlHelper
    {
        public static string GetDisplayUrlFromRequest(HttpRequest request)
        {
            var displayUrl = request.GetDisplayUrl();
            return displayUrl;
        }
    }
}
