/* 
 * Copyright (c) 2018, Firely (info@fire.ly) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.github.com/furore-fhir/spark/master/LICENSE
 */

using System.Net.Http;
using FhirStarter.R4.Detonator.Core.SparkEngine.Core;
using FhirStarter.R4.Detonator.Core.SparkEngine.Service;
using Hl7.Fhir.Rest;

namespace FhirStarter.R4.Detonator.Core.SparkEngine.Extensions
{
    public static class HttpRequestFhirExtensions
    {
        public static Entry GetEntry(this HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey(Const.RESOURCE_ENTRY))
                return request.Properties[Const.RESOURCE_ENTRY] as Entry;
            return null;
        }

        public static SummaryType RequestSummary(this HttpRequestMessage request)
        {
            return request.GetParameter("_summary") == "true" ? SummaryType.True : SummaryType.False;
        }
    }
}
