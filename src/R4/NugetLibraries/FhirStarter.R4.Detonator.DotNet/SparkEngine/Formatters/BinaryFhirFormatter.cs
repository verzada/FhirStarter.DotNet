/* 
 * Copyright (c) 2018, Firely (info@fire.ly) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.github.com/furore-fhir/spark/master/LICENSE
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FhirStarter.R4.Detonator.Core.SparkEngine.Core;
using Hl7.Fhir.Model;
using Task = System.Threading.Tasks.Task;

namespace FhirStarter.R4.Detonator.Core.SparkEngine.Formatters
{
     public class BinaryFhirFormatter : FhirMediaTypeFormatter
    {
        public BinaryFhirFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(FhirMediaType.BinaryResource));
       //     MediaTypeMappings.Add(new MatchBinaryPathTypeMapping());
        }

        public override bool CanReadType(Type type)
        {
            return type == typeof(Resource);
        }

        public override bool CanWriteType(Type type)
        {
            return type == typeof(Binary);
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            return Task.Factory.StartNew(() =>
            {
                var stream = new MemoryStream();
                readStream.CopyTo(stream);

                var success = content.Headers.TryGetValues("X-Content-Type", out var xContentHeader);

                if (!success)
                {
                    throw Error.BadRequest("POST to binary must provide a Content-Type header");
                }

                var contentType = xContentHeader.FirstOrDefault();

                var binary = new Binary
                {
                    Data = stream.ToArray(),
                    ContentType = contentType
                };

                //ResourceEntry entry = ResourceEntry.Create(binary);
                //entry.Tags = content.Headers.GetFhirTags();
                return (object)binary;
            });
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, System.Net.TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                
                var binary = (Binary)value;
                var stream = new MemoryStream(binary.Data);

                stream.CopyTo(writeStream);

                stream.Flush();
            });
        }
    }
}
