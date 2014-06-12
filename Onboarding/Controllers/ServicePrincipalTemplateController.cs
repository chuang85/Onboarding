using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Onboarding.Controllers
{
    public class ServicePrincipalTemplateController : ApiController
    {
        // GET api/serviceprincipaltemplate
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/serviceprincipaltemplate/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/serviceprincipaltemplate
        public void Post([FromBody]string value)
        {
        }

        // PUT api/serviceprincipaltemplate/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/serviceprincipaltemplate/5
        public void Delete(int id)
        {
        }
    }
}
