using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FhirStarter.R4.Instigator.DotNet.Extension
{
    /// <summary>
    /// https://stackoverflow.com/questions/49304972/asp-net-core-2-0-how-to-return-custom-json-or-xml-response-from-middleware
    /// </summary>
    public class HeaderValidation
  {
      private readonly RequestDelegate _next;
        public HeaderValidation(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var request = httpContext.Request;
            var acceptXml = request.Headers.FirstOrDefault(p => p.Key.ToLower() == "application/xml");
            var acceptJson = request.Headers.FirstOrDefault(p => p.Key.ToLower() == "application/json");
            
            await _next.Invoke(httpContext);

            if (acceptXml.Key != null)
            {

            }

            if (acceptJson.Key != null)
            {

            }
        }
    }
}
