using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Policy;

namespace Onboarding.Models
{
    public class Environment
    {
        public ICollection<string> Hostnames { get; set; }

        public ICollection<string> AdditionalServicePrincipalNames { get; set; }

        public ICollection<Url> AppAddress { get; set; }
    }
}