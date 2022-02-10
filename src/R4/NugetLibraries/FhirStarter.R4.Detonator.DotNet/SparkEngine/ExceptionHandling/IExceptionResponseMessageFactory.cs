using System;
using System.Net.Http;

namespace FhirStarter.STU3.Detonator.DotNetCore.SparkEngine.ExceptionHandling
{
    public interface IExceptionResponseMessageFactory
    {
        HttpResponseMessage GetResponseMessage(Exception exception, HttpRequestMessage reques);
    }
}
