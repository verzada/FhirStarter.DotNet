using System.IO;
using System.Text;
using FhirStarter.R4.Detonator.Core.SparkEngine.Core;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace FhirStarter.R4.Detonator.Core.Formatters
{
    public abstract class FhirMediaTypeFormatter:TextOutputFormatter
    {
        protected string ReadBodyFromStream(Stream readStream)
        {
            //var charset = content.Headers.ContentType.CharSet ?? Encoding.UTF8.HeaderName;
            var charset = Encoding.UTF8.HeaderName;
            var encoding = Encoding.GetEncoding(charset);

            if (!Equals(encoding, Encoding.UTF8))
                throw Error.BadRequest("FHIR supports UTF-8 encoding exclusively, not " + encoding.WebName);

            var reader = new StreamReader(readStream, Encoding.UTF8, true);
            return reader.ReadToEnd();
        }
    }
}
