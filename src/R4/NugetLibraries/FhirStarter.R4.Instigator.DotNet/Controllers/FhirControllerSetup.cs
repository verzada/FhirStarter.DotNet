using System;
using System.Collections.Generic;
using FhirStarter.R4.Detonator.DotNet.Interface;
using FhirStarter.R4.Instigator.DotNet.Helper;
using FhirStarter.R4.Instigator.DotNet.Validation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FhirStarter.R4.Instigator.DotNet.Controllers
{
    [Route("fhir"), EnableCors]
    public partial class FhirController
    {

        private ILogger<IFhirService> _log;
        private readonly IConfigurationRoot _appSettings;
        private readonly IEnumerable<IFhirService> _fhirServices;
        private readonly IProfileValidator _profileValidator;

        private readonly bool _validationEnabled;
        private readonly bool _returnValidatedResource;

        private bool _isMockupEnabled;

        public FhirController(ILogger<IFhirService> loggerFactory, IConfigurationRoot fhirStarterSettings,
            IServiceProvider serviceProvider, IProfileValidator profileValidator)
        {
            _log = loggerFactory;
            _appSettings = fhirStarterSettings;

            _validationEnabled = ControllerHelper.GetFhirStarterSettingBool(_appSettings, "EnableValidation");
            _isMockupEnabled = ControllerHelper.GetFhirStarterSettingBool(_appSettings, "MockupEnabled");
            _returnValidatedResource =
                ControllerHelper.GetFhirStarterSettingBool(_appSettings, "ReturnValidatedResource");

            _fhirServices = ControllerHelper.GetFhirServices(serviceProvider);
            _profileValidator = profileValidator;
        }
    }
}
