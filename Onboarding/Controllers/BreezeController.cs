using System;
using System.Linq;
using System.Web.Http;
using Breeze.ContextProvider;
using Breeze.WebApi2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Onboarding.Models;

namespace Onboarding.Controllers
{
    [BreezeController]
    public class BreezeController : ApiController
    {
        //readonly EFContextProvider<OnboardingDbContext> _contextProvider = new EFContextProvider<OnboardingDbContext>();
        private readonly OnboardingRequestContextProvider _contextProvider = new OnboardingRequestContextProvider();

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
                return e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace;
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
            JsonSerializerSettings serializerSettings = BreezeConfig.Instance.GetJsonSerializerSettings();
            JsonSerializer jsonSerializer = JsonSerializer.Create(serializerSettings);
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