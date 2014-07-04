using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Breeze.WebApi2;
using Onboarding.DashboardService;
using Onboarding.ReviewService;
using Onboarding.Models;
using System.Xml;
using System.Text;
using System.Diagnostics;

namespace Onboarding.Utils
{
    public class OnboardingRequestContextProvider : EFContextProvider<OnboardingDbContext>
    {
        private const string CmdPath = @"C:\WINDOWS\system32\cmd.exe";
        private const string CmdConfigArgs = @"/k set inetroot=e:\cumulus_main&set corextbranch=main&e:\cumulus_main\tools\path1st\myenv.cmd";
        private const string SourcePath = @"C:\Users\t-chehu\Source\Repos\Onboarding\Onboarding\App_Data\";
        private const string DestPath = @"E:\CUMULUS_MAIN\sources\dev\RestServices\GraphService\Tools\";
        private const string SavingPathXml = @"../../App_Data/";

        public ReviewDashboardServiceClient qClient = new ReviewDashboardServiceClient();
        public ReviewServiceClient rClient = new ReviewServiceClient();

        protected override bool BeforeSaveEntity(EntityInfo entityInfo)
        {
            // Convert time format from UTC to local.
            OnboardingRequest onboardingRequest = (OnboardingRequest)entityInfo.Entity;
            onboardingRequest.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);

            // Write string formatted xml in to an xml file and assign it to Blob field.
            string filename = onboardingRequest.DisplayName + "_" + onboardingRequest.CreatedBy + ".xml";
            onboardingRequest.Blob = SystemHelpers.SavesStringToXml(onboardingRequest.TempXmlStore, SavingPathXml, filename);
            
            // Handle file copy and add to (local) source depot operations.
            RunCmd(filename);

            // Step 1 - Create a code review
            // Assign ReviewId to the corresponding field in OnboardingRequest
            onboardingRequest.CodeFlowId = CodeFlowHelpers.CreateReview(rClient, onboardingRequest.CreatedBy, "Chengkan Huang",
                onboardingRequest.CreatedBy + "@microsoft.com", "Testing integration", "MSODS");

            // Step 2 - Create a code package and add it to the review
            CodeFlowHelpers.AddCodePackage(rClient, onboardingRequest.CodeFlowId, CodeFlowHelpers.CreateCodePackage("testing pack", onboardingRequest.CreatedBy, onboardingRequest.CreatedBy, CodePackageFormat.SourceDepotPack, new Uri(DestPath + filename)));
            //CodeFlowHelpers.AddCodePackage(rClient, onboardingRequest.CodeFlowId, CodeFlowHelpers.CreateCodePackage("testing pack", onboardingRequest.CreatedBy, onboardingRequest.CreatedBy, new Uri(DestPath + filename)));


            // Step 3 - Add reviewers to the review
            CodeFlowHelpers.AddReviewers(rClient, onboardingRequest.CodeFlowId, new ReviewService.Reviewer[]
                {
                    CodeFlowHelpers.CreateReviewer(onboardingRequest.CreatedBy, "Chengkan Huang", onboardingRequest.CreatedBy + "@microsoft.com", true)
                });

            // Step 4 - Publish the review
            CodeFlowHelpers.PublishReview(rClient, onboardingRequest.CodeFlowId, "meesage from author");

            // Close clients
            rClient.Close();
            qClient.Close();

            return true;
        }

        public void RunCmd(string filename)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(CmdPath);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = CmdConfigArgs 
                + SystemHelpers.CmdCopyArgs(SourcePath, DestPath, filename) 
                + SystemHelpers.CmdAddToDepotArgs(DestPath, filename);
            Process process = Process.Start(startInfo);
        }
    }
}