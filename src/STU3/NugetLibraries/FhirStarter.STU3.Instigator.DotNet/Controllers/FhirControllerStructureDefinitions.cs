using System.Linq;
using System.Net;
using System.Net.Http;
using FhirStarter.STU3.Instigator.DotNet.Validation;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Mvc;

namespace FhirStarter.STU3.Instigator.DotNet.Controllers
{
   public partial class FhirController
    {
        [HttpGet, Route("StructureDefinition/{nspace}/{id}")]
        public HttpResponseMessage GetStructureDefinition(string nspace, string id)
        {
            var structureDefinition = Load(false, id, nspace);
            if (structureDefinition == null)
                throw new FhirOperationException($"{nameof(StructureDefinition)} for {nspace}/{id} not found",
                    HttpStatusCode.InternalServerError);
            return SendResponse(structureDefinition);
        }

        [HttpGet, Route("StructureDefinition/{id}")]
        public HttpResponseMessage GetStructureDefinition(string id)
        {
            var structureDefinition = Load(false, id);
            if (structureDefinition == null)
                throw new FhirOperationException($"{nameof(StructureDefinition)} for {id} not found",
                    HttpStatusCode.InternalServerError);
            return SendResponse(structureDefinition);
        }

        private static StructureDefinition Load(bool excactMatch, string id, string nspace = null)
        {
            string lookup;
            if (string.IsNullOrEmpty(nspace))
            {
                lookup = id;
            }
            else
            {
                lookup = nspace + "/" + id;
            }

            var structureDefinitions = ValidationHelper.GetStructureDefinitions();
            return excactMatch
                ? structureDefinitions.FirstOrDefault(definition => definition.Type.Equals(lookup))
                : structureDefinitions.FirstOrDefault(definition => definition.Url.EndsWith(lookup));
        }
    }
}
