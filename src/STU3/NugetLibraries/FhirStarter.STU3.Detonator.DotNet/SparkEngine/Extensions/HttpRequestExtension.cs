/* 
 * Copyright (c) 2018, Firely (info@fire.ly) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.github.com/furore-fhir/spark/master/LICENSE
 */


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Http;

namespace FhirStarter.STU3.Detonator.DotNetCore3.SparkEngine.Extensions
{
    public static class HttpRequestExtension
    {
       private static List<Tuple<string, string>> TupledParameters(this HttpRequest request)
        {
            var list = new List<Tuple<string, string>>();
            var query = request.Query;
            foreach (var pair in query)
            {
                list.AddRange(pair.Value.Select(splitValue => new Tuple<string, string>(pair.Key, splitValue)));
            }
            return list;
        }

        public static SearchParams GetSearchParams(this HttpRequest request)
        {
            var parameters = request.TupledParameters().Where(tp => tp.Item1 != "_format");
            var searchCommand = SearchParams.FromUriParamList(parameters);
            return searchCommand;
        }
    }
}
