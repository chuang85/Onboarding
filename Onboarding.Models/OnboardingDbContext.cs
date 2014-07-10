﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Onboarding.Models
{
    public class OnboardingDbContext : DbContext
    {
        public DbSet<OnboardingRequest> OnboardingRequests { get; set; }

        public OnboardingDbContext()
            : base("OnboardingConnection")
		{
            // Uncomment for data migration
            //Database.SetInitializer(new OnboardingDbContextInitializer());
		}
    }
}