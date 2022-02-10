using System.Text;
using FhirStarter.STU3.Detonator.DotNetCore3.MediaTypeHeaders;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Task = System.Threading.Tasks.Task;

namespace FhirStarter.STU3.Detonator.DotNetCore3.Formatters
{
   public class JsonFhirFormatterDotNetCore3:TextOutputFormatter
    {
        public JsonFhirFormatterDotNetCore3()
        {
            SupportedEncodings.Add(new UTF8Encoding());
            SupportedMediaTypes.Add(FhirMediaTypeHeaderValues.ApplicationJson);
            SupportedMediaTypes.Add(FhirMediaTypeHeaderValues.ApplicationJsonFhir);
            SupportedMediaTypes.Add(FhirMediaTypeHeaderValues.TextJson);
            SupportedMediaTypes.Add(FhirMediaTypeHeaderValues.TextJsonFhir);
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var fhirResource = context.Object as Base;
            var fhirJsonSerializer = new FhirJsonSerializer();
            var json = fhirJsonSerializer.SerializeToString(fhirResource);

            return context.HttpContext.Response.WriteAsync(json);
        }
    }
}
