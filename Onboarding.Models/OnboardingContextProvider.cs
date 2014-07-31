using System;
using System.Globalization;
using System.Reflection;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using log4net;

namespace Onboarding.Models
{
    public class OnboardingRequestContextProvider : EFContextProvider<OnboardingDbContext>
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
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

                    // Write string formatted xml into binary and assign it to Blob field.
                    if ((onboardingRequest.Type == RequestType.CreateSPT 
                        || onboardingRequest.Type == RequestType.UpdateSPT)
                        && onboardingRequest.TempXmlStore != null)
                    {
                        onboardingRequest.Blob = SystemHelpers.GenerateBlobFromString(onboardingRequest.TempXmlStore);
                    }
                    Logger.Info("A new OnboardingRequest is creating on " + onboardingRequest.DisplayCreatedDate);
                }
                else if (entityInfo.EntityState == EntityState.Modified)
                {
                    entityInfo.OriginalValuesMap.Add("ModifiedDate", onboardingRequest.ModifiedDate);
                    entityInfo.OriginalValuesMap.Add("DisplayModifiedDate", onboardingRequest.DisplayModifiedDate);
                    SetModifiedTimeOnchange(onboardingRequest);
                    Logger.Info("An OnboardingRequest has been modified on " + onboardingRequest.DisplayModifiedDate + " with new state " + onboardingRequest.State);
                }
                return true;
            }
            else
            {
                Logger.Fatal("Fatal error happens when saving the request, entity type is " + entityInfo.Entity);
                return false;
            }
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