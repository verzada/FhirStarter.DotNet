using System;
using Microsoft.Extensions.Configuration;

namespace FhirStarter.STU3.Instigator.DotNet.Helper
{
    public static class StartupConfigHelper
    {
        public static IConfigurationRoot BuildConfigurationFromJson(string basePath, string settingsFilename)
        {

            if (!string.IsNullOrEmpty(basePath) && !string.IsNullOrEmpty(settingsFilename))
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile(settingsFilename)
                    .Build();

                return configuration;
            }

            if (string.IsNullOrEmpty(settingsFilename))
            {
                throw new ArgumentException($"Add {settingsFilename} to the FHIR server project. Check https://github.com/verzada/FhirStarter.DotNetCore for more details");
            }
            throw new ArgumentNullException($"{nameof(basePath)} or {nameof(settingsFilename)} input to {nameof(BuildConfigurationFromJson)} cannot be null or empty.");
        }
    }
}
