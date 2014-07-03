﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Breeze.WebApi2;
using Onboarding.Models;

namespace Onboarding.Data
{
    public class OnboardingRequestContextProvider : EFContextProvider<OnboardingDbContext>
    {
        protected override bool BeforeSaveEntity(EntityInfo entityInfo)
        {
            OnboardingRequest onboardingRequest = (OnboardingRequest)entityInfo.Entity;
            onboardingRequest.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);
            //DateTime timeUtc = DateTime.UtcNow;
            //TimeZoneInfo ptZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            //onboardingRequest.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, ptZone);

            //onboardingRequest.Blob = GetBytes(onboardingRequest.TempXmlStore);
            return true;
        }

        private byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}