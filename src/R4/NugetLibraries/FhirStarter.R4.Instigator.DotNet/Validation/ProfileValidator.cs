using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Validation;
using Microsoft.Extensions.Logging;

namespace FhirStarter.R4.Instigator.DotNet.Validation
{
    public class ProfileValidator:IProfileValidator
    {
        private readonly ILogger _log;
        private static Validator _validator;
        
        public ProfileValidator(Validator validator, ILogger logger)
        {
            _validator ??= validator;
            _log = logger;
        }

        public OperationOutcome Validate(Resource resource, bool onlyErrors=true, bool threadedValidation=true)
        {
            OperationOutcome result;
            if (!(resource is Bundle) || !threadedValidation)
            {
                var xmlSerializer = new FhirXmlSerializer();
                using var reader = XDocument.Parse(xmlSerializer.SerializeToString(resource)).CreateReader();
                result =  RunValidation(onlyErrors, reader);
            }
            else
            {
                var bundle = (Bundle)resource;
                result =  RunBundleValidation(onlyErrors, bundle);
            }

            if (result.Issue.Count <= 0) return result;
            _log.LogWarning("Validation failed");
            _log.LogWarning("Request: " + XDocument.Parse(new FhirXmlSerializer().SerializeToString(resource)));
            _log.LogWarning("Response:" + XDocument.Parse(new FhirXmlSerializer().SerializeToString(result)));
            return result;
        }

        private static OperationOutcome RunBundleValidation(bool onlyErrors, Bundle bundle)
        {
            var operationOutcome = new OperationOutcome();

            var itemsRun = new List<string>();
            var serialItems = new List<Resource>();
            var parallelItems = new List<Resource>();
            foreach (var item in bundle.Entry)
            {
                if (itemsRun.Contains(item.Resource.TypeName))
                {
                    parallelItems.Add(item.Resource);
                }
                else
                {
                    serialItems.Add(item.Resource);
                    itemsRun.Add(item.Resource.TypeName);
                }
            }
            RunSerialValidation(onlyErrors, serialItems, operationOutcome);
            RunParallelValidation(onlyErrors, parallelItems, operationOutcome);
            return operationOutcome;
        }

        private static void RunParallelValidation(bool onlyErrors, IReadOnlyCollection<Resource> parallelItems, OperationOutcome operationOutcome)
        {
            var xmlSerializer = new FhirXmlSerializer();
            if (parallelItems.Count > 0)
            {
                Parallel.ForEach(parallelItems, new ParallelOptions {MaxDegreeOfParallelism = parallelItems.Count},
                    loopedResource =>
                    {
                        using var reader = XDocument.Parse(xmlSerializer.SerializeToString(loopedResource))
                            .CreateReader();
                        var localOperationOutCome = RunValidation(onlyErrors, reader);

                        operationOutcome.Issue.AddRange(localOperationOutCome.Issue);
                    });
            }
        }

        private static void RunSerialValidation(bool onlyErrors, IEnumerable<Resource> serialItems, OperationOutcome operationOutcome)
        {
            var xmlSerializer = new FhirXmlSerializer();
            foreach (var item in serialItems)
            {
                var localOperationOutCome = RunValidation(onlyErrors,
                    XDocument.Parse(xmlSerializer.SerializeToString(item)).CreateReader());
                operationOutcome.Issue.AddRange(localOperationOutCome.Issue);
            }
        }

        private static OperationOutcome RunValidation(bool onlyErrors, XmlReader reader)
        {
            var result = _validator.Validate(reader);
            if (!onlyErrors)
            {
                return result;
            }
            var invalidItems = (from item in result.Issue
                                let error = item.Severity != null && item.Severity.Value == OperationOutcome.IssueSeverity.Error
                                where error
                                select item).ToList();

            result.Issue = invalidItems;
            return result;
            
        }
    }
}
