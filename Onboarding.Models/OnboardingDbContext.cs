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

        public DbSet<ServiceType> ServiceTypes { get; set; }

        public DbSet<TaskSet> TaskSets { get; set; }

        public DbSet<Scope> Scopes { get; set; }
    }
}