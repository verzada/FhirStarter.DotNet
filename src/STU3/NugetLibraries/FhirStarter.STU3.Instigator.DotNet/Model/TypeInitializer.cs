using System;

namespace FhirStarter.STU3.Instigator.DotNet.Model
{
    public class TypeInitializer
    {
        public bool MustBeInitialized { get; }
        public Type ServiceType { get; }
        public string Name { get; }

        public TypeInitializer(bool mustBeInitialized, Type serviceType, string name)
        {
            MustBeInitialized = mustBeInitialized;
            ServiceType = serviceType;
            Name = name;
        }
    }
}
