using System;
using System.Globalization;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;

namespace Onboarding.Models
{
    public class OnboardingRequestContextProvider : EFContextProvider<OnboardingDbContext>
    {
        private const string DateFormat = "M/d/yyyy h:mm tt";

        protected override bool BeforeSaveEntity(EntityInfo entityInfo)
        {
            if (entityInfo.EntityState == EntityState.Added &&
                entityInfo.Entity is OnboardingRequest)
            {
                // Get the instance of the OnboardingRequest class.
                var onboardingRequest = (OnboardingRequest) entityInfo.Entity;

                // Set CreatedDate & ModifiedDate to Now.
                SetCreatedTimeOnInitialization(onboardingRequest);

                // Set RequestState to "Created".
                //onboardingRequest.State = RequestState.Created;
                onboardingRequest.State = "Created";

                // Write string formatted xml into binary and assign it to Blob field.
                onboardingRequest.Blob = SystemHelpers.GenerateBlobFromString(onboardingRequest.TempXmlStore);

                SystemHelpers.SaveXmlToDisk(onboardingRequest);
                SystemHelpers.AddFileToDepotAndPack(SystemHelpers.GenerateFilename(onboardingRequest));
                return true;
            }
            return false;
        }

        private static void SetCreatedTimeOnInitialization(OnboardingRequest onboardingRequest)
        {
            onboardingRequest.CreatedDate = DateTime.UtcNow;
            onboardingRequest.DisplayCreatedDate = onboardingRequest.CreatedDate.ToLocalTime().ToString(DateFormat,
                CultureInfo.CreateSpecificCulture("en-US"));
            onboardingRequest.ModifiedDate = onboardingRequest.CreatedDate;
            onboardingRequest.DisplayModifiedDate = onboardingRequest.DisplayCreatedDate;
        }

        
    }
}