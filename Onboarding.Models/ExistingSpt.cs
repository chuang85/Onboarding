using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Models
{
    public class ExistingSpt
    {
        [Key]
        public string Name { get; set; }

        public string XmlContent { get; set; }

        public string ServiceType { get; set; }
    }
}
