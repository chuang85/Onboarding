using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onboarding.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Onboarding.Models.UnitTest
{
    [TestClass()]
    public class SystemHelpersTests
    {
        private OnboardingRequest _request;
        private const string DummyXml = @"<TestXml><DisplayName><Value>ListUsers</Value></DisplayName><TaskName><Value>0</Value></TaskName><TaskId><Value>6b043a6f-1162-4fae-8eb5-38410e151ddb</Value></TaskId></TestXml>";

        [TestInitialize()]
        public void InitializeCreatedRequest()
        {
            byte[] blob = SystemHelpers.GenerateBlobFromString(DummyXml);
            _request = new OnboardingRequest
            {
                RequestId = 15,
                RequestSubject = "CreatedRequest",
                CreatedBy = @"REDMOND\t-chehu",
                State = RequestState.Created,
                Type = RequestType.CreateSPT,
                Blob = blob
            };
        }

        [TestMethod()]
        public void GenerateBlobFromStringTest()
        {
            byte[] blob = SystemHelpers.GenerateBlobFromString(DummyXml);
            Assert.AreEqual(DummyXml, SystemHelpers.GenerateStringFromBlob(blob));
        }

        [TestMethod()]
        public void SaveXmlToDiskTest()
        {
            var filePath = Constants.DepotPath + SystemHelpers.GenerateFilename(_request);
            Assert.IsFalse(File.Exists(filePath));
            SystemHelpers.SaveXmlToDisk(_request);
            Assert.IsTrue(File.Exists(filePath));
            File.Delete(filePath);
        }

        [TestMethod()]
        public void AddFileToDepotAndPackTest()
        {
            var xmlPath = Constants.DepotPath + SystemHelpers.GenerateFilename(_request);
            var dpkPath = Constants.DepotPath + SystemHelpers.GenerateFilename(_request) + ".dpk";
            Assert.IsFalse(File.Exists(dpkPath));
            SystemHelpers.SaveXmlToDisk(_request);
            SystemHelpers.AddFileToDepotAndPack(SystemHelpers.GenerateFilename(_request));
            Assert.IsTrue(File.Exists(dpkPath));
            while (true)
            {
                if (File.Exists(xmlPath) && File.Exists(dpkPath))
                {
                    File.Delete(xmlPath);
                    File.Delete(dpkPath);
                    break;
                }
            }
            SystemHelpers.RevertFile(SystemHelpers.GenerateFilename(_request));
        }

        [TestMethod()]
        public void GenerateReivewNameTest()
        {
            Assert.AreEqual("CreateSPT-RequestId-15", SystemHelpers.GenerateReivewName(_request));
        }

        [TestMethod()]
        public void GenerateFilenameTest()
        {
            Assert.AreEqual("15_t-chehu.xml", SystemHelpers.GenerateFilename(_request));
        }

    }
}
