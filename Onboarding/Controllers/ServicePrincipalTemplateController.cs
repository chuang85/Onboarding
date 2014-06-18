using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Breeze.WebApi2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Onboarding.Models;
using Onboarding.Data;

namespace Onboarding.Controllers
{
    [BreezeController]
    public class ServicePrincipalTemplateController : ApiController
    {
        //readonly ServicePrincipalTemplateContextProvider _contextProvider = new ServicePrincipalTemplateContextProvider();
        readonly EFContextProvider<OnboardingDbContext> _contextProvider = new EFContextProvider<OnboardingDbContext>();

        [HttpGet]
        public string Metadata()
        {
            try
            {
                return _contextProvider.Metadata();
            }
            catch (Exception e)
            {
                return e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace;
            }
            
        }

        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return _contextProvider.SaveChanges(saveBundle);
        }

        private static JsonSerializer CreateJsonSerializer()
        {
            var serializerSettings = BreezeConfig.Instance.GetJsonSerializerSettings();
            var jsonSerializer = JsonSerializer.Create(serializerSettings);
            return jsonSerializer;
        }

        [HttpGet]
        public IQueryable<ServicePrincipalTemplate> ServicePrincipalTemplates()
        {
            return _contextProvider.Context.ServicePrincipalTemplates;
        }
    }
}
