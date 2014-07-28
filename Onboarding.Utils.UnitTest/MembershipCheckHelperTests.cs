using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onboarding.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Onboarding.Utils.UnitTest
{
    [TestClass()]
    public class MembershipCheckHelperTests
    {
        private const string User0 = @"REDMOND\t-chehu";
        private const string User1 = @"fareast\sriramd";

        [TestMethod()]
        public void GetNameTest()
        {
            Assert.AreEqual("Chengkan Huang", MembershipCheckHelpers.GetName(User0));
            Assert.AreEqual("Sriram Dhanasekaran", MembershipCheckHelpers.GetName(User1));
        }

        [TestMethod()]
        public void GetEmailAddressTest()
        {
            Assert.AreEqual("t-chehu@microsoft.com", MembershipCheckHelpers.GetEmailAddress(User0));
            Assert.AreEqual("sriramd@microsoft.com", MembershipCheckHelpers.GetEmailAddress(User1));
        }
    }
}
