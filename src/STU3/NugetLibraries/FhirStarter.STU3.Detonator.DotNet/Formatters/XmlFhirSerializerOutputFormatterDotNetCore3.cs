using System.Text;
using FhirStarter.STU3.Detonator.DotNetCore3.MediaTypeHeaders;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Task = System.Threading.Tasks.Task;

namespace FhirStarter.STU3.Detonator.DotNetCore3.Formatters
{
    /// <summary>
    /// https://asp.net-hacker.rocks/2018/10/11/customizing-aspnetcore-07-outputformatter.html
    /// </summary>
    public class XmlFhirSerializerOutputFormatterDotNetCore3 : TextOutputFormatter
    {
        public XmlFhirSerializerOutputFormatterDotNetCore3()
        {
            SupportedEncodings.Add(new UTF8Encoding());
            SupportedMediaTypes.Add(FhirMediaTypeHeaderValues.ApplicationXml);
            SupportedMediaTypes.Add(FhirMediaTypeHeaderValues.ApplicationXmlFhir);
            SupportedMediaTypes.Add(FhirMediaTypeHeaderValues.TextXml);
            SupportedMediaTypes.Add(FhirMediaTypeHeaderValues.TextXmlFhir);
        }
        
        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var fhirResource = context.Object as Base;
            var serializerSettings = new SerializerSettings { Pretty = true };
            var fhirXmlSerializer = new FhirXmlSerializer(serializerSettings);
            var xml = fhirXmlSerializer.SerializeToString(fhirResource);
            return context.HttpContext.Response.WriteAsync(xml);
        }
    }
}
