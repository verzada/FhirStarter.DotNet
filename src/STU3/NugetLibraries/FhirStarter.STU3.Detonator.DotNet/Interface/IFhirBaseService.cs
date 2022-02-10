using FhirStarter.STU3.Detonator.DotNetCore3.SparkEngine.Core;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Mvc;

namespace FhirStarter.STU3.Detonator.DotNetCore3.Interface
{
    public interface IFhirBaseService
    {
        // The name of the Resource you can query (earlier called GetAlias)
        string GetServiceResourceReference();
        Base Create(IKey key, Resource resource);
        Base Read(SearchParams searchParams);
        Base Read(string id);
        ActionResult Update(IKey key, Resource resource);
        ActionResult Delete(IKey key);
        ActionResult Patch(IKey key, Resource resource);
    }
}
