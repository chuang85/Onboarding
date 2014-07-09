using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Onboarding.Models;
using Onboarding.DashboardService;
using Onboarding.ReviewService;

namespace Onboarding.Utils
{
    public static class CodeFlowHelpers
    {
        public const string ProjectShortName = "MSODS";
        public const string EmailDomain = "@microsoft.com";

        /// <summary>
        /// Step 1 - Create a review. An id will be generated, but not pushlished.
        /// </summary>
        /// <param name="client">An instance of <see cref="ReviewServiceClient"/>.</param>
        /// <param name="authorName">Author's alias.</param>
        /// <param name="authorDisplayName">Author's display name.</param>
        /// <param name="emailAddress">Author's email address.</param>
        /// <param name="reviewName">The title of review.</param>
        /// <param name="projectShortName">The name of the project that review belongs to. e.g. "MSODS"</param>
        /// <returns>Review Id</returns>
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
        /// <returns>An instance of <see cref="CodePackage"/>.</returns>
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

        /// <summary>
        /// Monitor status for an active review, given the key.
        /// A review is regarded as completed when at least one reviewer is signed off.
        /// </summary>
        /// <param name="client">An instance of <see cref="ReviewDashboardServiceClient"/>.</param>
        /// <param name="key">Key of the request to be monitored. Can be accessed from DB.</param>
        /// <param name="author">Author of the review.</param>
        /// <returns>True if at least one reviewer is signed off.</returns>
        public static bool ReviewCompleted(ReviewDashboardServiceClient client, string key, string author)
        {
            var response = client.Query(new CodeReviewQuery
            {
                Authors = new string[] { author },
                ReviewStatuses = new DashboardService.CodeReviewStatus[] { DashboardService.CodeReviewStatus.Active },
                UserAgent = author
            });
            if (response != null) {
                foreach (var review in response.Reviews)
                {
                    if (review.Key.Equals(key))
                    {
                        foreach (var reviewer in review.Reviewers)
                        {
                            if (reviewer.Status == DashboardService.ReviewerStatus.SignedOff)
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                    throw new Exception("Given key not exist.");
                }
            }
            throw new Exception("No response found.");
        }
    }
}