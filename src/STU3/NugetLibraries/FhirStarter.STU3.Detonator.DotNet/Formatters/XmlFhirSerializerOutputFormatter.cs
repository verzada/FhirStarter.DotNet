using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using MediaTypeHeaderValue = Microsoft.Net.Http.Headers.MediaTypeHeaderValue;
using Task = System.Threading.Tasks.Task;

namespace FhirStarter.R4.Detonator.Core.Formatters
{
    /// <summary>
    /// https://asp.net-hacker.rocks/2018/10/11/customizing-aspnetcore-07-outputformatter.html
    /// </summary>
    public class XmlFhirSerializerOutputFormatter:FhirMediaTypeFormatter
 {
        public string ContentType { get; }

        public XmlFhirSerializerOutputFormatter()
        {
            foreach (var mediaType in Hl7.Fhir.Rest.ContentType.XML_CONTENT_HEADERS)
            {
                SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(mediaType));
            }
            SupportedEncodings.Add(Encoding.UTF8);
        }

        // optional, but makes sense to restrict to a specific condition
        protected override bool CanWriteType(Type type)
        {
            if (typeof(Base).IsAssignableFrom(type) 
                || typeof(IEnumerable<Base>).IsAssignableFrom(type))
            {
                return base.CanWriteType(type);
            }
            return false;
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var fhirResource = context.Object as Base;
            var fhirXmlSerializer = new FhirXmlSerializer();
            var xml = fhirXmlSerializer.SerializeToString(fhirResource);

            return context.HttpContext.Response.WriteAsync(xml);

        }
 }
}
