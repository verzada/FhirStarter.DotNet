using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Validation;

namespace FhirStarter.R4.Instigator.DotNet.Validation
{
    public static class ValidationHelper
    {
        private const string StructureDefinitionsPath = @"\Resources\StructureDefinitions";

        public static Validator GetValidator()
        {
            var structureDefinitions = GetStructureDefinitionsPath();
            var includeSubDirectories = new DirectorySourceSettings { IncludeSubDirectories = true };
            var directorySource = new DirectorySource(structureDefinitions, includeSubDirectories);

            var cachedResolver = new CachedResolver(directorySource);
            var coreSource = new CachedResolver(ZipSource.CreateValidationSource());
            var combinedSource = new MultiResolver(cachedResolver, coreSource);
            var settings = new ValidationSettings
            {
                EnableXsdValidation = true,
                GenerateSnapshot = true,
                Trace = true,
                ResourceResolver = combinedSource,
                ResolveExternalReferences = true,
                SkipConstraintValidation = false
            };
            var validator = new Validator(settings);
            return validator;
        }

        private static string GetStructureDefinitionsPath()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var location = new Uri(assembly.GetName().CodeBase);
            var directoryInfo = new FileInfo(location.AbsolutePath).Directory;
            var structureDefinitions = directoryInfo?.FullName + StructureDefinitionsPath;
            return structureDefinitions;
        }

        public static IEnumerable<StructureDefinition> GetStructureDefinitions()
        {
            var xmlParser = new FhirXmlParser();
            var structureDefinitionsPath = GetStructureDefinitionsPath();
            if (!Directory.Exists(structureDefinitionsPath)) yield break;
            var files = Directory.GetFiles(GetStructureDefinitionsPath());
            foreach (var file in files)
            {
                using (var stream = new FileStream(file, FileMode.Open))
                {
                    var xDocument = XDocument.Load(stream);
                    var structureDefinition = xmlParser.Parse<StructureDefinition>(xDocument.ToString());
                    yield return structureDefinition;
                }
            }

        }
    }
}
