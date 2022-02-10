using System;
using System.Collections.Generic;
using FhirStarter.STU3.Detonator.DotNetCore3.Interface;
using FhirStarter.STU3.Instigator.DotNet.Helper;
using FhirStarter.STU3.Instigator.DotNet.Validation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FhirStarter.STU3.Instigator.DotNet.Controllers
{
    [Route("fhir"), EnableCors]
    public partial class FhirController
    {

        private ILogger<IFhirService> _log;
        private readonly IConfigurationRoot _appSettings;
        private readonly IEnumerable<IFhirService> _fhirServices;
        private readonly IProfileValidator _profileValidator;

        private readonly bool _validationEnabled;

        private bool _isMockupEnabled;
        //private AcceptHeaderAttribute _acceptHeaderAttributes;

        public FhirController(ILogger<IFhirService> loggerFactory, IConfigurationRoot fhirStarterSettings,
            IServiceProvider serviceProvider, IProfileValidator profileValidator)
        {
            _log = loggerFactory;
            _appSettings = fhirStarterSettings;

            _validationEnabled = ControllerHelper.GetFhirStarterSettingBool(_appSettings, "EnableValidation");
            _isMockupEnabled = ControllerHelper.GetFhirStarterSettingBool(_appSettings, "MockupEnabled");
            _fhirServices = ControllerHelper.GetFhirServices(serviceProvider);
            _profileValidator = profileValidator;
        }
    }
}
