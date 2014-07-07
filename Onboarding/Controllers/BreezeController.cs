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
using Onboarding.Utils;
using System.Net.Mail;

namespace Onboarding.Controllers
{
    [BreezeController]
    public class BreezeController : ApiController
    {
        //readonly EFContextProvider<OnboardingDbContext> _contextProvider = new EFContextProvider<OnboardingDbContext>();
        readonly OnboardingRequestContextProvider _contextProvider = new OnboardingRequestContextProvider();

        // ~/breeze/Breeze/Metadata 
        [HttpGet]
        public string Metadata()
        {
            try
            {
                return _contextProvider.Metadata();
            }
            catch (Exception e)
            {
                return e.Message + System.Environment.NewLine + System.Environment.NewLine + e.StackTrace;
            }
            
        }

        // ~/breeze/Breeze/SaveChanges
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

        // ~/breeze/Breeze/OnboardingRequests
        [HttpGet]
        public IQueryable<OnboardingRequest> OnboardingRequests()
        {
            return _contextProvider.Context.OnboardingRequests;
        }
    }
}
