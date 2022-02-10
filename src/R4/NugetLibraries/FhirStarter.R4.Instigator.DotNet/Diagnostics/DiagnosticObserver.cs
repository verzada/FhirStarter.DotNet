using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FhirStarter.R4.Instigator.DotNet.Diagnostics
{
    public class DiagnosticObserver : IObserver<KeyValuePair<string, object>>
    {
        private readonly ILogger<DiagnosticObserver> _logger;
        public DiagnosticObserver(ILogger<DiagnosticObserver> logger)
        {
            _logger = logger;
        }

        public void OnCompleted() { }
        public void OnError(Exception error) { }

        public void OnNext(KeyValuePair<string, object> value)
        {
            switch (value.Key)
            {
                case "Microsoft.AspNetCore.Hosting.BeginRequest":
                {
                    var httpContext = GetHttpContext(value);
                    httpContext.Items["StartTimestampKey"] = (long)value.Value.GetType().GetProperty("timestamp").GetValue(value.Value);
                    break;
                }
                case "Microsoft.AspNetCore.Hosting.EndRequest":
                {
                    var httpContext = GetHttpContext(value);
                    if (httpContext != null)
                    {
                        var endTimestamp = (long)value.Value.GetType().GetProperty("timestamp")?.GetValue(value.Value)!;
                        var startTimestamp = (long)httpContext.Items["StartTimestampKey"]!;
                        var duration = new TimeSpan((long)((endTimestamp - startTimestamp) * TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency));
                        var method = httpContext.Request.Method;
                        var contentType = httpContext.Request.ContentType;
                        var hostname = httpContext.Connection.RemoteIpAddress;
                        var requestLength = httpContext.Request.ContentLength;
                        var path = httpContext.Request.Path;
                        _logger.LogInformation("Method: {method}, ReqLength: {requestLength}, Hostname: {hostname}, Path: {path}, Elapsed: {elapsedMs} ms, Content-Type: {contentType}", method, requestLength, hostname,path, Math.Round(duration.TotalMilliseconds), contentType);
                    }
                    
                    break;
                }
            }
        }

        private static HttpContext GetHttpContext(in KeyValuePair<string, object> value)
        {
            return (HttpContext)value.Value.GetType().GetProperty("httpContext")?.GetValue(value.Value);
        }
    }
}
