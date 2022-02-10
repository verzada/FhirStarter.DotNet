using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FhirStarter.STU3.Instigator.DotNet.Model;
using Microsoft.Extensions.Configuration;

namespace FhirStarter.STU3.Instigator.DotNet.Helper
{
    public static class FhirStarterSettingsHelper
    {
        private const string FhirStarterSettingsFhirServiceAssemblies = "FhirStarterSettings:FhirServiceAssemblies";

        public static ICollection<Assembly> GetFhirServiceAssemblies(IConfigurationRoot fhirStarterSettings)
        {
            var assemblyNames = GetArrayFromSettings(fhirStarterSettings);
            if (assemblyNames.Any())
            {
                return  assemblyNames.Select(assemblyName => Assembly.Load(assemblyName.Trim())).ToList();
            }
            throw new ArgumentException($"The setting {FhirStarterSettingsFhirServiceAssemblies} in FhirStarterSettings.json is empty");
        }

        private static ICollection<string> GetArrayFromSettings(IConfigurationRoot fhirStarterSettings)
        {
            var settings = GetFhirStarterSettings(fhirStarterSettings);
            
            var assemblyNames = settings.FhirServiceAssemblies;
            if (assemblyNames != null && assemblyNames.Any())
            {
                return assemblyNames;
            }
            return new List<string>();
        }

        private static FhirStarterSettings GetFhirStarterSettings(IConfigurationRoot fhirStarterSettings)
        {
            var sectionData = fhirStarterSettings.GetSection(nameof(FhirStarterSettings));
            var section = new FhirStarterSettings();
            sectionData.Bind(section);
            return section;
        }
     }
}
