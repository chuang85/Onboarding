using System.Text;
using System.Xml;
using Onboarding.Models;

namespace Onboarding.Utils
{
    public class DbHelpers
    {
        public XmlDocument GetXmlFromBlob(OnboardingRequest onboardingRequest)
        {
            var doc = new XmlDocument();
            string xml = Encoding.UTF8.GetString(onboardingRequest.Blob);
            doc.LoadXml(xml);
            return doc;
        }
    }
}