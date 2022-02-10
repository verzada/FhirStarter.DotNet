using System;
using System.Collections.Generic;
using FhirStarter.STU3.Detonator.DotNetCore3.Interface;
using FhirStarter.STU3.Detonator.DotNetCore3.SparkEngine.Core;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FhirStarter.STU3.Twisted.DotNetCore3.Services
{
    public class PatientService : IFhirService
    {
        public string GetServiceResourceReference()
        {
            return nameof(Patient);
        }


        public Base Create(IKey key, Base resource)
        {
            throw new Exception("SHould not get here");
            //return new Patient
            //{
            //    Active =  true,
            //    Id =  "12345"
            //};
        }

        public Base Create(IKey key,Resource resource)
        {
           
            var test = (Patient) resource;
            return test;
        }

        public Base Read(SearchParams searchParams)
        {
            var patient = new Patient {Name = new List<HumanName>()};
            var humanName = new HumanName { Family = "yo", Given = new List<string> { "yo", "next"}};
            patient.Name.Add(humanName);
            return patient;
        }

        public Base Read(string id)
        {
            if (id.Equals("Fail"))
            {
                throw new ArgumentException($"Could not find patient by id {id}");
            }
            var patient = new Patient { Name = new List<HumanName>() };
            var humanName = new HumanName { Family = "yo", Given = new List<string> { "yo", "next" } };
            patient.Name.Add(humanName);
            return patient;
        }

        public ActionResult Update(IKey key, Resource resource)
        {
            throw new System.NotImplementedException();
        }

        public ActionResult Delete(IKey key)
        {
            throw new System.NotImplementedException();
        }

        public ActionResult Patch(IKey key, Resource resource)
        {
            throw new System.NotImplementedException();
        }

        public CapabilityStatement.RestComponent GetRestDefinition()
        {
            throw new System.NotImplementedException();
        }

        public OperationDefinition GetOperationDefinition(HttpRequest request)
        {
            throw new System.NotImplementedException();
        }

        public string GetStructureDefinitionNameForResourceProfile()
        {
            throw new System.NotImplementedException();
        }
    }
}
