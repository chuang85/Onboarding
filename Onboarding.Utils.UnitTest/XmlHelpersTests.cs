using System;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Onboarding.Utils.UnitTest
{
    [TestClass]
    public class XmlHelpersTests
    {
        private const string NotXml0 = @"..\..\dummyXmls\NotXml0.txt";
        private const string NotXml1 = @"..\..\dummyXmls\NotXml1.txt";
        private const string NotSpt0 = @"..\..\dummyXmls\NotSpt0.txt";
        private const string NotSpt1 = @"..\..\dummyXmls\NotSpt1.txt";
        private const string IsSpt = @"..\..\dummyXmls\IsSpt0.txt";

        [TestMethod]
        public void IsValidXmlTest()
        {
            var xmlDoc = new XmlDocument();
            Assert.IsFalse(XmlHelpers.IsValidXml(xmlDoc, ReadFileToString(NotXml0)));
            Assert.IsFalse(XmlHelpers.IsValidXml(xmlDoc, ReadFileToString(NotXml1)));
            Assert.IsTrue(XmlHelpers.IsValidXml(xmlDoc, ReadFileToString(NotSpt0)));
            Assert.IsTrue(XmlHelpers.IsValidXml(xmlDoc, ReadFileToString(NotSpt1)));
        }

        [TestMethod]
        public void IsValidSptTest()
        {
            Assert.IsFalse(XmlHelpers.IsValidSpt(ReadFileToString(NotSpt0)));
            Assert.IsFalse(XmlHelpers.IsValidSpt(ReadFileToString(NotSpt1)));
            Assert.IsTrue(XmlHelpers.IsValidSpt(ReadFileToString(IsSpt)));
        }

        [TestMethod]
        public void GetFileNameFromPathTest()
        {
            Assert.AreEqual("NotSpt0", XmlHelpers.GetFileNameFromPath(NotSpt0));
        }

        private string ReadFileToString(string path)
        {
            var text = string.Empty;
            using (var sr = new StreamReader(path, Encoding.UTF8))
            {
                text = sr.ReadToEnd();
            }
            return text;
        }
    }
}
