using Hl7.Fhir.Model;

namespace FhirStarter.R4.Instigator.DotNet.Validation
{

    public interface IProfileValidator
    {
        OperationOutcome Validate(Resource resource, bool onlyErrors = true, bool threadedValidation = true);
    }
}
