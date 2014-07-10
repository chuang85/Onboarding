using Onboarding.Service.ReviewService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Onboarding.Service
{
    public partial class RequestHandler : ServiceBase
    {
        public static ReviewServiceClient rClient = new ReviewServiceClient();
        public Timer tmrExecutor = new Timer();

        public RequestHandler()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //tmrExecutor.Elapsed += new ElapsedEventHandler(DoWork); // adding Event
            //tmrExecutor.Interval = 5000; // Set your time here 
            //tmrExecutor.Enabled = true;
            //tmrExecutor.Start();
        }

        protected override void OnStop()
        {
        }

        private void DoWork(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Step 1 - Create a code review
            string reviewId = CreateReview(rClient, "t-chehu", "Chengkan Huang", "t-chehu@microsoft.com", "Testing service", "MSODS");

            // Step 2 - Create a code package and add it to the review
            AddCodePackage(rClient, reviewId, CreateCodePackage("testing pack", "t-chehu", "t-chehu",
                CodePackageFormat.SourceDepotPack, new Uri(@"E:\CUMULUS_MAIN\sources\dev\RestServices\GraphService\Tools\qwefd_t-chehu.xml.dpk")));

            // Step 3 - Add reviewers to the review
            AddReviewers(rClient, reviewId, new ReviewService.Reviewer[]
            {
                CreateReviewer("t-chehu", "Chengkan Huang", "t-chehu@microsoft.com", true)
            });

            // Step 4 - Publish the review
            PublishReview(rClient, reviewId, "meesage from author");
        }

        /// <summary>
        /// Step 1 for submiting a code review, create a review.
        /// </summary>
        /// <param name="client">An instance of <see cref="ReviewServiceClient"/>.</param>
        /// <param name="authorName">Author's alias.</param>
        /// <param name="authorDisplayName">Author's display name.</param>
        /// <param name="emailAddress">Author's email address.</param>
        /// <param name="reviewName">The title of review.</param>
        /// <param name="projectShortName">The name of the project that review belongs to. e.g. "MSODS"</param>
        /// <returns>Review Id.</returns>
        public static string CreateReview(ReviewServiceClient client, string authorName, string authorDisplayName, string emailAddress, string reviewName, string projectShortName)
        {
            return client.CreateReview(
                new ReviewService.Author
                {
                    Name = authorName,
                    DisplayName = authorDisplayName,
                    EmailAddress = emailAddress
                }, reviewName, projectShortName).Key;
        }

        /// <summary>
        /// Step 2.1 - Create a code package.
        /// </summary>
        /// <param name="name">Code package name.</param>
        /// <param name="author">Author's alias.</param>
        /// <param name="userAgent"></param>
        /// <param name="format">Code package format, dpk/tfs shelve set.</param>
        /// <param name="location">The uri location of the package.</param>
        /// <returns>Review Id</returns>
        public static CodePackage CreateCodePackage(string name, string author, string userAgent, CodePackageFormat format, Uri location)
        {
            return new CodePackage
            {
                Name = name,
                Author = author,
                UserAgent = userAgent,
                Format = format,
                Location = location
            };
        }

        /// <summary>
        /// Step 2.2 - Add code package to review.
        /// </summary>
        /// <param name="client">An instance of <see cref="ReviewServiceClient"/>.</param>
        /// <param name="reviewId">Id of the review to be added.</param>
        /// <param name="authorDisplayName">Author's display name.</param>
        /// <param name="emailAddress">Author's email address.</param>
        /// <param name="reviewName">The title of review.</param>
        /// <returns>Review Id</returns>
        public static void AddCodePackage(ReviewServiceClient client, string reviewId, CodePackage codePackage)
        {
            client.AddCodePackage(reviewId, codePackage);
        }

        /// <summary>
        /// Step 3.1 - Create a review.
        /// </summary>
        /// <param name="name">Reviewer's alias.</param>
        /// <param name="displayName">Reviewer's display name.</param>
        /// <param name="userAgent">Reviewer's emailAddress.</param>
        /// <param name="required">Required or optional.</param>
        /// <returns>An instance of <see cref="ReviewService.Reviewer"/>.</returns>
        public static ReviewService.Reviewer CreateReviewer(string name, string displayName, string emailAddress, bool required)
        {
            return new ReviewService.Reviewer
            {
                Name = name,
                DisplayName = displayName,
                EmailAddress = emailAddress,
                Required = required
            };
        }

        /// <summary>
        /// Step 3.2 - Add a list of reviewers to the review.
        /// </summary>
        /// <param name="client">An instance of <see cref="ReviewServiceClient"/>.</param>
        /// <param name="reviewId">Id of the review to be added.</param>
        /// <param name="reviewers">A list of reviewers.</param>
        public static void AddReviewers(ReviewServiceClient client, string reviewId, ReviewService.Reviewer[] reviewers)
        {
            client.AddReviewers(reviewId, reviewers);
        }

        /// <summary>
        /// Step 4 - Final step, publish the review.
        /// </summary>
        /// <param name="client">An instance of <see cref="ReviewServiceClient"/>.</param>
        /// <param name="reviewId">Id of the review to be added.</param>
        /// <param name="messageFromAuthor"></param>
        public static void PublishReview(ReviewServiceClient client, string reviewId, string messageFromAuthor)
        {
            client.PublishReview(reviewId, messageFromAuthor);
        }
    }
}
