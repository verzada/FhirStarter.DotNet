using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

//https://stackoverflow.com/questions/49304972/asp-net-core-2-0-how-to-return-custom-json-or-xml-response-from-middleware
namespace FhirStarter.STU3.Instigator.DotNet.Extension
{
  public  class HeaderValidation
  {
      private const string accept = "accept";

        private readonly RequestDelegate _next;
        public HeaderValidation(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            // How to return a json or xml formatted custom message with a http status code?
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
