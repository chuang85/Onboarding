﻿using System;
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

            context.ServicePrincipalTemplates.Add(new ServicePrincipalTemplate
            {
                Name = "Office 365",
                Environment = "grn001"
            });

            context.ServicePrincipalTemplates.Add(new ServicePrincipalTemplate
            {
                Name = "Exchange",
                Environment = "grnppe"
            });

            context.SaveChanges();
        }
    }
}