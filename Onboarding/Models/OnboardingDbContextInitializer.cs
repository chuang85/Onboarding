using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Onboarding.Utils;

namespace Onboarding.Models
{
    public class OnboardingDbContextInitializer : 
        DropCreateDatabaseAlways<OnboardingDbContext>
    {
        protected override void Seed(OnboardingDbContext context)
        {
            base.Seed(context);

            context.OnboardingRequests.Add(new OnboardingRequest
            {
                DisplayName = "Office 365",
                CreatedBy = "hck",
                //State = RequestState.Created
            });

            context.SaveChanges();
        }
    }
}