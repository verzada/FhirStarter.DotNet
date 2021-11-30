using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using FhirStarter.R4.Detonator.DotNetCore3.Filter;
using FhirStarter.R4.Detonator.DotNetCore3.Formatters;
using FhirStarter.R4.Instigator.DotNetCore3.Configuration;
using FhirStarter.R4.Instigator.DotNetCore3.Diagnostics;
using FhirStarter.R4.Instigator.DotNetCore3.Helper;
using FhirStarter.R4.Instigator.DotNetCore3.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FhirStarter.R4.Twisted.DotNetCore3
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            FhirDotnet3Setup(services);
        }

        // copy this method to your Startup
        public void FhirDotnet3Setup(IServiceCollection services)
        {
            var appSettings =
                StartupConfigHelper.BuildConfigurationFromJson(AppContext.BaseDirectory, "appsettings.json");
            FhirStarterConfig.SetupFhir(services, appSettings);

            var detonator = FhirStarterConfig.GetDetonatorAssembly(appSettings["FhirStarterSettings:FhirDetonatorAssembly"]);
            var instigator = FhirStarterConfig.GetInstigatorAssembly(appSettings["FhirStarterSettings:FhirInstigatorAssembly"]);

            services.Configure<FhirStarterSettings>(appSettings.GetSection(nameof(FhirStarterSettings)));
            services.AddRouting();

            services.Configure<MvcOptions>(options =>
            {
                //https://stackoverflow.com/questions/32942608/mvc-6-change-return-content-type

                // input
                options.InputFormatters.Clear();
                options.InputFormatters.Add(new XmlFhirSerializerInputFormatterDotNetCore3());
                options.InputFormatters.Add(new JsonFhirInputFormatter());


                // output
                options.OutputFormatters.Clear();
                options.OutputFormatters.Insert(0, new JsonFhirFormatterDotNetCore3());
                options.OutputFormatters.Insert(1, new XmlFhirSerializerOutputFormatterDotNetCore3());

                options.FormatterMappings.SetMediaTypeMappingForFormat("xml+fhir", "application/xml+fhir");
                options.FormatterMappings.SetMediaTypeMappingForFormat("json+fhir", "application/json+fhir");

                options.RespectBrowserAcceptHeader = true;

            });
            services.AddControllers(controllers => {
                controllers.Filters.Add(typeof(FhirExceptionFilter));
                }).AddApplicationPart(instigator).AddApplicationPart(detonator).AddControllersAsServices();
            services.AddHttpContextAccessor();

            services.AddSingleton<DiagnosticObserver>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            DiagnosticListener diagnosticListenerSource, DiagnosticObserver diagnosticObserver)
        {
            diagnosticListenerSource.Subscribe(diagnosticObserver);
            app.UseRouting();
            app.UseAuthorization();
            app.UseCors();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }
    }
}
