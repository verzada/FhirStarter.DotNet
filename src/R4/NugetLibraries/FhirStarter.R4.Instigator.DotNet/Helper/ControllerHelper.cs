using System;
using System.Collections.Generic;
using System.Linq;
using FhirStarter.R4.Detonator.DotNet.Interface;
using FhirStarter.R4.Detonator.DotNet.SparkEngine.Service.FhirServiceExtensions;
using FhirStarter.R4.Instigator.DotNet.Model;
using FhirStarter.R4.Instigator.DotNet.Validation;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FhirStarter.R4.Instigator.DotNet.Helper
{
    public static class ControllerHelper
    {
        public static IFhirService GetFhirService(string type, IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(
                    $"The service provider cannot be null when looking for a {nameof(IFhirService)}");
            }

            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentNullException(
                    $"The parameter {type} cannot be null, it must contain a valid {nameof(Hl7)} reference to a valid {nameof(Resource)} such as {nameof(Patient)}");
            }

            var request = serviceProvider.GetService<IEnumerable<IFhirService>>()
                .FirstOrDefault(p => p.GetServiceResourceReference().Equals(type));
            return request;
        }

        public static IEnumerable<IFhirService> GetFhirServices(IServiceProvider serviceProvider)
        {
            var request = serviceProvider?.GetService<IEnumerable<IFhirService>>();
            return request;
        }

        public static CapabilityStatement CreateMetaData(IEnumerable<IFhirService> services, IConfigurationRoot appSettings, HttpRequest request)
        {
            var enumerableFhirServices = services as IFhirService[] ?? services.ToArray();
            var fhirServices = services as IFhirService[] ?? enumerableFhirServices.ToArray();
            if (!fhirServices.Any())
            {
                return new CapabilityStatement();
            }

            var serviceName = MetaDataName(fhirServices);
            var fhirVersion = GetFhirVersion(ModelInfo.Version);

            var publisher = GetFhirStarterSettingString(appSettings, "FhirPublisher");
            var capabilityStatement =
                CapabilityStatementBuilder.CreateServer(serviceName, publisher, fhirVersion);

            capabilityStatement.AddSearchTypeInteractionForResources();
            capabilityStatement = capabilityStatement.AddCoreSearchParamsAllResources(enumerableFhirServices);
            capabilityStatement.Experimental = false;
            capabilityStatement.Format = new List<string>{"xml+fhir", "json+fhir"};

            var fhirDescription = GetFhirStarterSettingString(appSettings, "FhirDescription");
            if (!string.IsNullOrEmpty(fhirDescription))
            {
                capabilityStatement.Description = new Markdown(fhirDescription);
            }

            capabilityStatement.Rest =
                AddProfile(capabilityStatement.Rest, enumerableFhirServices);

            return capabilityStatement;

        }

        private static List<CapabilityStatement.RestComponent> AddProfile(
            List<CapabilityStatement.RestComponent> restComponents, IEnumerable<IFhirService> services)
        {

            if (restComponents.Any())
            {
                var structureDefinitions = ValidationHelper.GetStructureDefinitions();

                var definitions = structureDefinitions as StructureDefinition[] ?? structureDefinitions.ToArray();
                var fhirServices = services as IFhirService[] ?? services.ToArray();
                foreach (var restComponent in restComponents)
                {
                    var resources = restComponent.Resource;
                    foreach (var resource in resources)
                    {
                        var hl7ModelType = resource.Type;
                        if (hl7ModelType != null)
                        {
                            var type = hl7ModelType.Value.ToString();
                            var resourceService =
                                fhirServices.FirstOrDefault(p => p.GetServiceResourceReference() == type);
                            if (resourceService != null)
                            {
                                var profile = resourceService.GetStructureDefinitionNameForResourceProfile();

                                var structureDefinition =
                                    definitions.FirstOrDefault(p => p.Name == profile);

                                if (structureDefinition != null)
                                {
                                    resource.Profile = structureDefinition.Url;
                                }
                            }
                        }
                    }
                }

            }

            return restComponents;
        }

        private static string MetaDataName(IEnumerable<IFhirService> services)
        {
            var fhirServices = services as IFhirService[] ?? services.ToArray();
            var serviceName = fhirServices.Length > 1 ? "The following services are available: " : "The following service is available: ";

            for (var i = 0; i < fhirServices.Length; i++)
            {
                serviceName += fhirServices[i].GetServiceResourceReference();
                if (i < fhirServices.Length - 1)
                {
                    serviceName += " ";
                }
            }
            return serviceName;
        }

        private static FHIRVersion GetFhirVersion(string version)
        {
            switch (version)
            {
                case "4.0.0":
                    return FHIRVersion.N0_4_0;
                default:
                    return FHIRVersion.N0_4_0;
            }
        }

        public static bool GetFhirStarterSettingBool(IConfigurationRoot appSettings, string key)
        {
            var keyValue = appSettings.GetSection($"FhirStarterSettings:{key}");
            if (!string.IsNullOrEmpty(keyValue.Value))
            {
                bool.TryParse(keyValue.Value, out var keyBool);
                return keyBool;
            }
            throw new ArgumentException($"The setting {key} must be defined in the {nameof(FhirStarterSettings)} section of appSettings with a true / false value");
        }

        public static string GetFhirStarterSettingString(IConfigurationRoot appSettings, string key)
        {
            var keyValue = appSettings.GetSection($"FhirStarterSettings:{key}");
            if (!string.IsNullOrEmpty(keyValue.Value))
            {
                return keyValue.Value;
            }
            throw new ArgumentException($"The setting {key} must be defined in the {nameof(FhirStarterSettings)} section of appSettings with a value");
        }
    }
}