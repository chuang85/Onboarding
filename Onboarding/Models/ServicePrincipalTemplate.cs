using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Onboarding.Models
{
    public class ServicePrincipalTemplate
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public string Environment { get; set; }
    }
}