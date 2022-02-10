using System;

namespace FhirStarter.STU3.Instigator.DotNet.Validation.Exceptions
{
    public class ValidateOutputException : ArgumentException
    {

        public ValidateOutputException(string message) : base(message)
        {
        }

        public ValidateOutputException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
