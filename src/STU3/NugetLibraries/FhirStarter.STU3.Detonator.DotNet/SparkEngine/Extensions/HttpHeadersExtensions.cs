/* 
 * Copyright (c) 2018, Firely (info@fire.ly) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.github.com/furore-fhir/spark/master/LICENSE
 */

using System.Net.Http;
using System.Net.Http.Headers;

namespace FhirStarter.STU3.Detonator.DotNetCore3.SparkEngine.Extensions
{
    public static class HttpHeadersExtensions
    {
        public static void Replace(this HttpHeaders headers, string header, string value)
        {
            //if (headers.Exists(header)) 
            headers.Remove(header);
            headers.Add(header, value);
        }

        public static string GetParameter(this HttpRequestMessage request, string key)
        {
            //foreach (var param in request.GetQueryNameValuePairs())
            foreach (var param in request.Properties)
            {
                if (param.Key == key) return param.Value.ToString();
            }
            return null;
        }
    }
}

