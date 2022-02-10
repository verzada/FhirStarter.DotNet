using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace FhirStarter.STU3.Detonator.DotNetCore3.Filter
{
    public class FhirExceptionFilter:IExceptionFilter
    {
        private readonly ILogger<FhirExceptionFilter> _logger;

        public FhirExceptionFilter(ILogger<FhirExceptionFilter> logger)
        {
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, context.Exception.Message);
            var operationOutCome = new OperationOutcome { Issue = new List<OperationOutcome.IssueComponent>() };
            var issue = new OperationOutcome.IssueComponent
            {
                Severity = OperationOutcome.IssueSeverity.Fatal,
                Code = OperationOutcome.IssueType.NotFound,
                Details = new CodeableConcept("StandardException", context.Exception.GetType().ToString(), context.Exception.Message),
                Diagnostics = context.Exception.StackTrace
            };

            operationOutCome.Issue.Add(issue);
            context.Result = new ObjectResult(operationOutCome);
            context.ExceptionHandled = true;
            
        }
    }
}
