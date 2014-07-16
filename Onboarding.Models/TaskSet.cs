using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Models
{
    public class TaskSet
    {
        [Key]
        public string TaskSetId { get; set; }

        public string TaskSetName { get; set; }
    }
}
