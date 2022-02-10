using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FhirStarter.STU3.Detonator.DotNetCore3.MediaTypeHeaders;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.IO;

namespace FhirStarter.STU3.Detonator.DotNetCore3.Formatters
{
    //https://github.com/dotnet/AspNetCore.Docs/blob/master/aspnetcore/web-api/advanced/custom-formatters/sample/Formatters/VcardInputFormatter.cs
   public class JsonFhirInputFormatter : TextInputFormatter
    {
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public JsonFhirInputFormatter()
        {
            SupportedEncodings.Add(new UTF8Encoding());
            SupportedMediaTypes.Add(FhirMediaTypeHeaderValues.ApplicationJson);
            SupportedMediaTypes.Add(FhirMediaTypeHeaderValues.ApplicationJsonFhir);
            SupportedMediaTypes.Add(FhirMediaTypeHeaderValues.TextJson);
            SupportedMediaTypes.Add(FhirMediaTypeHeaderValues.TextJsonFhir);

            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        #region canreadtype

        protected override bool CanReadType(Type type)
        {
            if (type.IsSubclassOf(typeof(Resource)) || type == typeof(Resource))
            {
                return base.CanReadType(type);
            }

            return false;
        }

        #endregion

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context,
            Encoding encoding)
        {
            if (context == null)
            {
                throw new ArgumentException(nameof(context));
            }

            //var request = context.HttpContext.Request;
            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.HttpContext.Request.Body.CopyToAsync(requestStream);

            try
            {
                requestStream.Seek(0, SeekOrigin.Begin);

                const int readChunkBufferLength = 4096;
                await using var textWriter = new StringWriter();
                using var reader = new StreamReader(requestStream);

                var readChunk = new char[readChunkBufferLength];
                int readChunkLength;

                do
                {
                    readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
                    textWriter.Write(readChunk, 0, readChunkLength);
                } while (readChunkLength > 0);

                var result = textWriter.ToString();
                if (!string.IsNullOrEmpty(result))
                {
                    var jsonParser = new FhirJsonParser();
                    var resource = jsonParser.Parse(result);

                    return await InputFormatterResult.SuccessAsync(resource);
                }
            }
            catch
            {
                return await InputFormatterResult.FailureAsync();
            }

            return null;
        }
    }
}
