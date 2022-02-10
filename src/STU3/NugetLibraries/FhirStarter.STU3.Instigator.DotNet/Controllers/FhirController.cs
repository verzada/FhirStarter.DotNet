using System;
using System.Linq;
using FhirStarter.STU3.Detonator.DotNetCore3.Interface;
using FhirStarter.STU3.Detonator.DotNetCore3.SparkEngine.Core;
using FhirStarter.STU3.Detonator.DotNetCore3.SparkEngine.Extensions;
using FhirStarter.STU3.Instigator.DotNet.Helper;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace FhirStarter.STU3.Instigator.DotNet.Controllers
{
    public partial class FhirController : Controller
    {
        #region HelperMethods
        private ActionResult HandleServiceReadWithSearchParams(string type)
        {
            var searchParams = Request.GetSearchParams();
            var service = ControllerHelper.GetFhirService(type, HttpContext.RequestServices);
            if (service == null) throw new ArgumentException($"The resource {type} service does not exist!");
            var result = service.Read(searchParams);
            return HandleResult(result);
        }

        private ActionResult HandleServiceRead(string type, string id)
        {
            var service = ControllerHelper.GetFhirService(type, HttpContext.RequestServices);
            if (service == null) throw new ArgumentException($"The resource {type} service does not exist!");
            var result = service.Read(id);
            return HandleResult(result);
        }

        private ActionResult HandleResult(Base result)
        {
            if (_validationEnabled)
            {
                var validation = _profileValidator.Validate((Resource) result);
                if (!validation.Success)
                {
                 
                    validation.Text = new Narrative
                    {
                        Div = new FhirXmlSerializer().SerializeToDocument(result).ToString()
                    };
                    result = validation;
                }
            }

            //Setting default contenttype to text/xml
            Response.ContentType = !string.IsNullOrEmpty(Request.ContentType) ? Request.ContentType : "text/xml";
            if (result is OperationOutcome)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        private ActionResult ResourceCreate(string type, Resource resource, IFhirBaseService service)
        {
            var headerAccept = Request.Headers.FirstOrDefault(t => t.Key.Equals("Accept"));
            if (headerAccept.Value.Equals("*/*"))
            {
                var contentType = Request.ContentType;
                Response.ContentType = contentType;
            }
            
            if (service == null || string.IsNullOrEmpty(type) || resource == null)
                return BadRequest($"Service for resource {nameof(resource)} must exist.");
            var key = Key.Create(type);
            var result = service.Create(key, resource);
            switch (result)
            {
                case null:
                    return BadRequest($"Service for resource {nameof(resource)} must exist.");
                case OperationOutcome _:
                    return BadRequest(result);
                default:
                    return Ok(result);
            }
        }

        public ActionResult ResourceUpdate(string type, string id, Resource resource, IFhirBaseService service)
        {
            if (service == null || string.IsNullOrEmpty(type) || resource == null || string.IsNullOrEmpty(id))
                throw new ArgumentException("Service is null, cannot update resource of type " + type);
            var key = Key.Create(type, id);
            var result = service.Update(key, resource);
            if (result != null)
                return Ok(result);
            return BadRequest($"Service is null, cannot update resource of {type}");
        }

        public ActionResult ResourceDelete(string type, Key key, IFhirBaseService service)
        {
            if (service == null) return BadRequest($"Service is null, cannot update resource of {type}");
            var result = service.Delete(key);
            return result;
        }

        public ActionResult ResourcePatch(string type, IKey key, Resource resource, IFhirBaseService service)
        {
            if (service == null) return BadRequest($"Service is null, cannot update resource of {type}");
            var result = service.Patch(key, resource);
            return result;
        }

        #endregion HelperMethods

        [HttpGet, Route("{type}/{id}"), Route("{type}/identifier/{id}")]
        public ActionResult Read(string type, string id)
        {
            return HandleServiceRead(type, id);
        }

        [HttpGet, Route("fhir/{type}"), Route("{type}")]
        public ActionResult Read(string type)
        {
            return HandleServiceReadWithSearchParams(type);
        }

        [HttpGet, Route("")]
        // ReSharper disable once InconsistentNaming
        public ActionResult Query(string type, string _query)
        {
            return HandleServiceReadWithSearchParams(type);
        }

        // return 201 when created
        // return 202 when takes too long
        [HttpPost, Route("{type}")]
        public ActionResult Create(string type,[FromBody] Resource resource)
        {
            if (_validationEnabled)
            {
                resource = (Resource)ValidateResource(resource, true);
            }
            if (resource is OperationOutcome)
            {
                return BadRequest(resource);
            }
            var service = ControllerHelper.GetFhirService(type, HttpContext.RequestServices);
            return ResourceCreate(type, resource, service);
        }

        // return 201 when created
        // return 202 when takes too long
        [HttpPut, Route("{type}/{id}")]
        public ActionResult Update(string type, string id, [FromBody] Resource resource)
        {
            if (_validationEnabled)
            {
                resource = (Resource) ValidateResource(resource, true);
            }
            if (resource is OperationOutcome)
            {
                return BadRequest(resource);
            }
            var service = ControllerHelper.GetFhirService(type, HttpContext.RequestServices);
            return ResourceUpdate(type, id, resource, service);
        }

        // return 201 when created
        // return 202 when takes too long
        [HttpDelete, Route("{type}/{id}")]
        public ActionResult Delete(string type, string id)
        {
            var service = ControllerHelper.GetFhirService(type, HttpContext.RequestServices);
            if (service != null)
            {
                var result = ResourceDelete(type, Key.Create(id), service);
                return result;
            }
            return BadRequest($"Service is null, cannot delete resource of {type} and id {id}");
        }

        // return 201 when created
        // return 202 when takes too long
        [HttpPatch, Route("{type}/{id}")]
        public ActionResult Patch(string type, string id, [FromBody]Resource resource)
        {
            var service = ControllerHelper.GetFhirService(type, HttpContext.RequestServices);
            if (service != null)
            {
                var result = ResourcePatch(type, Key.Create(id), resource, service);
                return result;
            }
            return BadRequest($"Service is null, cannot delete resource of {type} and id {id}");
        }

        [HttpGet, Route("metadata")]
        public ActionResult MetaData()
        {
            var httpRequest = HttpContext.Request;

            var metaData = ControllerHelper.CreateMetaData(_fhirServices, _appSettings, httpRequest);
            return Ok(metaData);
        }
    }
}
