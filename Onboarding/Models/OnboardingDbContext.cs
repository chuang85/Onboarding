using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Onboarding.Models
{
    public class OnboardingDbContext : DbContext
    {

        public DbSet<ServicePrincipalTemplate> ServicePrincipalTemplates { get; set; }

        public OnboardingDbContext()
            : base("OnboardingConnection")
		{
            Database.SetInitializer<OnboardingDbContext>(new OnboardingDbContextInitializer());
		}
    }
}