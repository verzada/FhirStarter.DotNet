using System.Collections.Generic;

namespace FhirStarter.STU3.Instigator.DotNet.Model
{
    public class FhirStarterSettings
    {
        public List<string> FhirServiceAssemblies { get; set; }
        public bool MockupEnabled { get; set; }
        public bool EnableValidation { get; set; }
        public bool LogRequestWhenError { get; set; }
        public string FhirPublisher { get; set; }
    }
}
