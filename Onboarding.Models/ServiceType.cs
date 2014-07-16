using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Models
{
    public class ServiceType
    {
        [Key]
        public string ServiceTypeId { get; set; } // Same as AppPrincipalId

        public string ServiceTypeName { get; set; }
    }
}
