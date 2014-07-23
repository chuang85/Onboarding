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
            if (entityInfo.Entity is OnboardingRequest)
            {
                var onboardingRequest = (OnboardingRequest) entityInfo.Entity;
                if (entityInfo.EntityState == EntityState.Added)
                {
                    // Set CreatedDate & ModifiedDate to Now.
                    SetCreatedTimeOnInitialization(onboardingRequest);

                    // Set RequestState to "Created".
                    //onboardingRequest.State = RequestState.Created;
                    onboardingRequest.State = "Created";

                    // Write string formatted xml into binary and assign it to Blob field.
                    if (onboardingRequest.TempXmlStore != null)
                    {
                        onboardingRequest.Blob = SystemHelpers.GenerateBlobFromString(onboardingRequest.TempXmlStore);
                    }

                    //SystemHelpers.SaveXmlToDisk(onboardingRequest);
                    //SystemHelpers.AddFileToDepotAndPack(SystemHelpers.GenerateFilename(onboardingRequest));
                }
                else if (entityInfo.EntityState == EntityState.Modified)
                {
                    entityInfo.OriginalValuesMap.Add("ModifiedDate", onboardingRequest.ModifiedDate);
                    entityInfo.OriginalValuesMap.Add("DisplayModifiedDate", onboardingRequest.DisplayModifiedDate);
                    SetModifiedTimeOnchange(onboardingRequest);
                    entityInfo.OriginalValuesMap.Add("TempXmlStore", onboardingRequest.TempXmlStore);
                }
            }
            return true;
        }

        private static void SetCreatedTimeOnInitialization(OnboardingRequest onboardingRequest)
        {
            onboardingRequest.CreatedDate = DateTime.UtcNow;
            onboardingRequest.DisplayCreatedDate = onboardingRequest.CreatedDate.ToLocalTime().ToString(DateFormat,
                CultureInfo.CreateSpecificCulture("en-US"));
            onboardingRequest.ModifiedDate = onboardingRequest.CreatedDate;
            onboardingRequest.DisplayModifiedDate = onboardingRequest.DisplayCreatedDate;
        }

        private static void SetModifiedTimeOnchange(OnboardingRequest onboardingRequest)
        {
            onboardingRequest.ModifiedDate = DateTime.UtcNow;
            onboardingRequest.DisplayModifiedDate = onboardingRequest.ModifiedDate.ToLocalTime().ToString(DateFormat,
                CultureInfo.CreateSpecificCulture("en-US"));
        }
        
    }
}