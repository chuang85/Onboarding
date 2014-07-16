using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Models
{
    public class Scope
    {
        [Key]
        public string ScopeId { get; set; }

        public string ScopeName { get; set; }
    }
}
