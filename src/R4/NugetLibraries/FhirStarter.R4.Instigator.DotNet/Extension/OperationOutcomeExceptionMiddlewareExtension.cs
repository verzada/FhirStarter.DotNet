using System.Net;
using FhirStarter.R4.Detonator.DotNet.Interface;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FhirStarter.R4.Instigator.DotNet.Extension
{
    //https://code-maze.com/global-error-handling-aspnetcore/#builtinmiddleware
    //Global Error Handling in ASP.NET Core Web API
    public static class OperationOutcomeExceptionMiddlewareExtension
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger<IFhirService> logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    // check for header ... don't assume json
                    //context.Response.ContentType = "application/json";
                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        logger.LogError($"The request {context.Request.Path} failed: {error.Error}");
                        var exception = error.Error;
                        var exceptionType = exception.GetType();
                        var issue = new OperationOutcome.IssueComponent
                        {
                            Details = new CodeableConcept(nameof(exceptionType), exceptionType.FullName,
                              exception.Message),
                            Diagnostics = exception.StackTrace
                        };
                        var outcome = new OperationOutcome();
                        outcome.Issue.Add(issue);
                        var fhirJsonSerializer = new FhirJsonSerializer();
                        var outcomeStr = fhirJsonSerializer.SerializeToString(outcome);
                        await context.Response.WriteAsync(outcomeStr);
                    }
                });
            });
        }
    }
}
