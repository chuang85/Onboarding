using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Onboarding.Models;
using System.Xml;
using System.Text;

namespace Onboarding.Utils
{
    public class DBHelpers
    {
        public XmlDocument GetXmlFromBlob(OnboardingRequest onboardingRequest)
        {
            XmlDocument doc = new XmlDocument();
            string xml = Encoding.UTF8.GetString(onboardingRequest.Blob);
            doc.LoadXml(xml);
            return doc;
        }
    }
}