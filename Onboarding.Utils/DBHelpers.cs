﻿using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Xml;
using Onboarding.Models;

namespace Onboarding.Utils
{
    public class DbHelpers
    {
        public static IQueryable<OnboardingRequest> UncompletedRequests(OnboardingDbContext db)
        {
            return
                from d in db.OnboardingRequests
                where !d.State.Equals("Completed")
                select d;
        }

        public static IQueryable<OnboardingRequest> RequestsCreated(OnboardingDbContext db)
        {
            return
                from d in db.OnboardingRequests
                where d.Type.Equals("CreateSPT") &&
                d.State.Equals("Created")
                select d;
        }

        public static IQueryable<OnboardingRequest> RequestsPendingReview(OnboardingDbContext db)
        {
            return
                from d in db.OnboardingRequests
                where d.Type.Equals("CreateSPT") &&
                d.State.Equals("PendingReview")
                select d;
        }

        public static void SaveCodeFlowId(OnboardingDbContext db, OnboardingRequest request, string codeFlowId)
        {
            request.CodeFlowId = codeFlowId;
            db.OnboardingRequests.Attach(request);
            db.Entry(request).State = EntityState.Modified;
        }

        public static XmlDocument GetXmlFromBlob(OnboardingRequest onboardingRequest)
        {
            var doc = new XmlDocument();
            string xml = Encoding.UTF8.GetString(onboardingRequest.Blob);
            doc.LoadXml(xml);
            return doc;
        }
    }
}