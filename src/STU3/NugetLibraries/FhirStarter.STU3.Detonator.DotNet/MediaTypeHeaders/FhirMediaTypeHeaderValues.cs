using Microsoft.Net.Http.Headers;

namespace FhirStarter.STU3.Detonator.DotNetCore3.MediaTypeHeaders
{
    public static class FhirMediaTypeHeaderValues
    {
        public static readonly MediaTypeHeaderValue ApplicationJson = MediaTypeHeaderValue.Parse("application/json").CopyAsReadOnly();
        public static readonly MediaTypeHeaderValue ApplicationJsonFhir = MediaTypeHeaderValue.Parse("application/json+fhir").CopyAsReadOnly();
        public static readonly MediaTypeHeaderValue TextJson = MediaTypeHeaderValue.Parse("text/json").CopyAsReadOnly();
        public static readonly MediaTypeHeaderValue TextJsonFhir = MediaTypeHeaderValue.Parse("text/json+fhir").CopyAsReadOnly();

        public static readonly MediaTypeHeaderValue ApplicationXml = MediaTypeHeaderValue.Parse("application/xml").CopyAsReadOnly();
        public static readonly MediaTypeHeaderValue ApplicationXmlFhir = MediaTypeHeaderValue.Parse("application/xml+fhir").CopyAsReadOnly();
        public static readonly MediaTypeHeaderValue TextXml = MediaTypeHeaderValue.Parse("text/xml").CopyAsReadOnly();
        public static readonly MediaTypeHeaderValue TextXmlFhir = MediaTypeHeaderValue.Parse("text/xml+fhir").CopyAsReadOnly();
    }
}
