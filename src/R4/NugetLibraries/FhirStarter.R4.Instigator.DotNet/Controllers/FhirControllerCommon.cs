using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using FhirStarter.R4.Detonator.DotNet.MediaTypeHeaders;
using FhirStarter.R4.Instigator.DotNet.Validation.Exceptions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Http;

namespace FhirStarter.R4.Instigator.DotNet.Controllers
{
    public partial class FhirController
    {
        private HttpResponseMessage SendResponse(Base resource)
        {
            var returnJson = ReturnJson(Request.Headers);
            if (_validationEnabled && !(resource is OperationOutcome))
            {
                resource = ValidateResource((Resource) resource, false);
            }

            StringContent httpContent;
            if (!returnJson)
            {
                var xmlSerializer = new FhirXmlSerializer();
                httpContent =
                    GetHttpContent(xmlSerializer.SerializeToString(resource), FhirMediaTypeHeaderValues.ApplicationXmlFhir.ToString());
            }
            else
            {
                var jsonSerializer = new FhirJsonSerializer();
                httpContent =
                    GetHttpContent(jsonSerializer.SerializeToString(resource), FhirMediaTypeHeaderValues.ApplicationJsonFhir.ToString());
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK) {Content = httpContent};
            if (resource is OperationOutcome)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
            }

            return response;
        }



        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.iheaderdictionary.item?view=aspnetcore-2.2
        /// https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.headerdictionaryextensions.getcommaseparatedvalues?view=aspnetcore-2.2 
        /// </summary>
        /// <param name="headerDictionary"></param>
        /// <returns></returns>
        private static bool ReturnJson(IHeaderDictionary headerDictionary)
        {

            var acceptHeaders =
                headerDictionary.GetCommaSeparatedValues("Accept");

            return acceptHeaders.Contains("application/json");
        }

        private static StringContent GetHttpContent(string serializedValue, string resourceType)
        {
            return new StringContent(serializedValue, Encoding.UTF8,
                resourceType);
        }

        private Base ValidateResource(Resource resource, bool isInput)
        {

            if (_profileValidator == null) return resource;
            if (resource is OperationOutcome) return resource;
            {
                var resourceName = resource.TypeName;
                var structureDefinition = Load(true, resourceName);
                if (structureDefinition != null)
                {
                    var found = resource.Meta != null && resource.Meta.ProfileElement.Count == 1 &&
                                resource.Meta.ProfileElement[0].Value.Equals(structureDefinition.Url);
                    if (!found)
                    {
                        var message = $"Profile for {resourceName} must be set to: {structureDefinition.Url}";
                        if (isInput)
                        {
                            throw new ValidateInputException(message);
                        }

                        throw new ValidateOutputException(message);

                    }
                }

            }
            var validationResult = _profileValidator.Validate(resource, true, false);
            if (validationResult.Issue.Count > 0)
            {
                resource = validationResult;
            }

            return resource;
        }

    }
}



