using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Breeze.WebApi2;
using Onboarding.Models;

namespace Onboarding.Data
{
    public class ServicePrincipalTemplateContextProvider : EFContextProvider<OnboardingDbContext>
    {
        protected override bool BeforeSaveEntity(EntityInfo entityInfo)
        {
            ServicePrincipalTemplate servicePrincipalTemplate = (ServicePrincipalTemplate)entityInfo.Entity;
            if (string.IsNullOrEmpty(servicePrincipalTemplate.Name))
            {
                //servicePrincipalTemplate.Name = "wtf";
            }
            return true;
        }
    }
}