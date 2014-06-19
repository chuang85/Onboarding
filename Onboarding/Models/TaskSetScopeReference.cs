using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Onboarding.Models
{
    public class TaskSetScopeReference
    {
        public Guid TaskSetId { get; set; }
        public string TaskSetName { get; set; }

        public ICollection<ScopeReference> ScopeReferences { get; set; }
    }

    public class ScopeReference
    {
        public Guid ScopeId { get; set; }
        public string ScopeName { get; set; }
    }

}