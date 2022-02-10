using System;

namespace FhirStarter.STU3.Instigator.DotNet.Validation.Exceptions
{
    public class ValidateInputException : ArgumentException
    {
        public ValidateInputException(string message) : base(message)
        {
        }

        public ValidateInputException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
