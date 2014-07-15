using System;
using System.Web.Optimization;

namespace Onboarding
{
    public class DurandalBundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();
            AddDefaultIgnorePatterns(bundles.IgnoreList);

            bundles.Add(
                new ScriptBundle("~/Scripts/vendor")
                    .Include("~/Scripts/jquery-{version}.js")
                    .Include("~/Scripts/bootstrap.js")
                    .Include("~/Scripts/bootstrap-switch.js")
                    .Include("~/Scripts/bootstrap-select.js")
                    .Include("~/Scripts/knockout-{version}.js")
                    .Include("~/Scripts/toastr.js")
                    .Include("~/scripts/Q.js")
                    .Include("~/scripts/breeze.debug.js")
                );

            bundles.Add(
                new StyleBundle("~/Content/css")
                    .Include("~/Content/ie10mobile.css")
                    .Include("~/Content/bootstrap.min.css")
                    .Include("~/Content/bootstrap-responsive.min.css")
                    .Include("~/Content/bootstrap-switch/bootstrap3/bootstrap-switch.min.css")
                    .Include("~/Content/bootstrap-select.min.css")
                    .Include("~/Content/font-awesome.min.css")
                    .Include("~/Content/durandal.css")
                    .Include("~/Content/starterkit.css")
                    .Include("~/Content/toastr.css")
                    .Include("~/Content/spt.css")
                );
        }

        public static void AddDefaultIgnorePatterns(IgnoreList ignoreList)
        {
            if (ignoreList == null)
            {
                throw new ArgumentNullException("ignoreList");
            }

            ignoreList.Ignore("*.intellisense.js");
            ignoreList.Ignore("*-vsdoc.js");
            ignoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
            //ignoreList.Ignore("*.min.js", OptimizationMode.WhenDisabled);
            //ignoreList.Ignore("*.min.css", OptimizationMode.WhenDisabled);
        }
    }
}