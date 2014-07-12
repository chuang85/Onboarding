using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Xml;
using Onboarding.Models;

namespace Onboarding.Utils
{
    public class DbHelpers
    {
        public static IQueryable<OnboardingRequest> SelectAllRequest(OnboardingDbContext db)
        {
            return
                from d in db.OnboardingRequests
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