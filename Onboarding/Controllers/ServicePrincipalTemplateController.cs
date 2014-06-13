using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Breeze.WebApi2;
using Newtonsoft.Json.Linq;
using Onboarding.Models;

namespace Onboarding.Controllers
{
    [BreezeController]
    public class ServicePrincipalTemplateController : ApiController
    {
        readonly EFContextProvider<OnboardingDbContext> _contextProvider = new EFContextProvider<OnboardingDbContext>();

        [HttpGet]
        public string Metadata()
        {
            return _contextProvider.Metadata();
        }

        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return _contextProvider.SaveChanges(saveBundle);
        }

        [HttpGet]
        public IQueryable<ServicePrincipalTemplate> ServicePrincipalTemplates()
        {
            return _contextProvider.Context.ServicePrincipalTemplates;
        }
    }
}
