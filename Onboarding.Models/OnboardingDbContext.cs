using System.Data.Entity;

namespace Onboarding.Models
{
    public class OnboardingDbContext : DbContext
    {
        public OnboardingDbContext()
            : base("OnboardingConnection")
        {
            // Uncomment for data migration
            //Database.SetInitializer(new OnboardingDbContextInitializer());
        }

        public DbSet<OnboardingRequest> OnboardingRequests { get; set; }
    }
}