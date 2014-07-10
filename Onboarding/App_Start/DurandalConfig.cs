using System.Web.Optimization;
using Onboarding;
using WebActivator;

[assembly: PostApplicationStartMethod(
    typeof (DurandalConfig), "PreStart")]

namespace Onboarding
{
    public static class DurandalConfig
    {
        public static void PreStart()
        {
            // Add your start logic here
            DurandalBundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}