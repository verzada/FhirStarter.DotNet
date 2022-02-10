using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Task = System.Threading.Tasks.Task;

namespace FhirStarter.R4.Detonator.Core.Formatters
{
    public class JsonFhirFormatter:FhirMediaTypeFormatter
    {
        public string ContentType { get; }

        public JsonFhirFormatter()
        {
            foreach (var mediaType in Hl7.Fhir.Rest.ContentType.JSON_CONTENT_HEADERS)
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
            var fhirJsonSerializer = new FhirJsonSerializer();
            var json = fhirJsonSerializer.SerializeToString(fhirResource);

            return context.HttpContext.Response.WriteAsync(json);

        }
    }
}
