namespace Onboarding.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Onboarding.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<Onboarding.Models.OnboardingDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Onboarding.Models.OnboardingDbContext context)
        {
            context.ServicePrincipalTemplates.AddOrUpdate(i => i.DisplayName,
        new ServicePrincipalTemplate
        {
            DisplayName = "Office 365",
            AppClass = "Office365Portal",
            Environment = "grn001"
        },

         new ServicePrincipalTemplate
         {
             DisplayName = "Exchange",
             AppClass = "Exchange",
             Environment = "grn002"
         },

         new ServicePrincipalTemplate
         {
             DisplayName = "Yammer",
             AppClass = "Yammer",
             Environment = "grnppe"
         }
   );
        }
    }
}
