using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FhirStarter.R4.Twisted.DotNet
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static void Main()
        {
            var host = Host.CreateDefaultBuilder().ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                webBuilder.ConfigureLogging((hostingContext, logging) => { logging.AddLog4Net("log4net.config"); });
                webBuilder.UseIISIntegration();
                webBuilder.UseStartup<Startup>();

            }).Build();
            host.Run();
        }
    }
}
