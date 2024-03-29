﻿using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FhirStarter.R4.Detonator.DotNet.MediaTypeHeaders;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.IO;

namespace FhirStarter.R4.Detonator.DotNet.Formatters
{
    /// <summary>
    /// https://github.com/dotnet/AspNetCore.Docs/blob/master/aspnetcore/web-api/advanced/custom-formatters/sample/Formatters/VcardInputFormatter.cs
    /// </summary>
    public class XmlFhirSerializerInputFormatterDotNetCore3 : TextInputFormatter
    {
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public XmlFhirSerializerInputFormatterDotNetCore3()
        {
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedMediaTypes.Add(FhirMediaTypeHeaderValues.ApplicationXml);
            SupportedMediaTypes.Add(FhirMediaTypeHeaderValues.ApplicationXmlFhir);
            SupportedMediaTypes.Add(FhirMediaTypeHeaderValues.TextXml);
            SupportedMediaTypes.Add(FhirMediaTypeHeaderValues.TextXmlFhir);
            SupportedMediaTypes.Add("text/plain");

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
                throw new ArgumentException("{context} is null", nameof(context));
            }

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
                    readChunkLength = await reader.ReadBlockAsync(readChunk, 0, readChunkBufferLength);
                    await textWriter.WriteAsync(readChunk, 0, readChunkLength);
                } while (readChunkLength > 0);

                var result = textWriter.ToString();
                if (!string.IsNullOrEmpty(result))
                {
                    var xmlFhirParser = new FhirXmlParser();
                    var resource = xmlFhirParser.ParseAsync(result);

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
