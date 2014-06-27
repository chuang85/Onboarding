using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Onboarding.Models
{
    public class OnboardingDbContextInitializer : DropCreateDatabaseAlways<OnboardingDbContext>
    {
        protected override void Seed(OnboardingDbContext context)
        {
            base.Seed(context);

            //context.ServicePrincipalTemplates.Add(new ServicePrincipalTemplate
            //{
            //    DisplayName = "Office 365",
            //    AppClass = "Office365Portal",
            //    AppPrincipalID = Guid.NewGuid(),
            //    Environment = "grn001"
            //});

            //context.ServicePrincipalTemplates.Add(new ServicePrincipalTemplate
            //{
            //    DisplayName = "Exchange",
            //    AppClass = "Exchange",
            //    AppPrincipalID = Guid.NewGuid(),
            //    Environment = "grnppe"
            //});

            context.SaveChanges();
        }
    }
}