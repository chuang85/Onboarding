using System;
using Onboarding.Models;
using Onboarding.Utils.DashboardService;
using Onboarding.Utils.ReviewService;
using Author = Onboarding.Utils.ReviewService.Author;
using CodeReviewStatus = Onboarding.Utils.DashboardService.CodeReviewStatus;
using Reviewer = Onboarding.Utils.ReviewService.Reviewer;
using ReviewerStatus = Onboarding.Utils.DashboardService.ReviewerStatus;

namespace Onboarding.Utils
{
    public static class CodeFlowHelpers
    {
        /// <summary>
        ///     Step 1 - Create a review. An id will be generated, but not pushlished.
        /// </summary>
        /// <param name="client">An instance of <see cref="ReviewServiceClient" />.</param>
        /// <param name="authorName">Author's alias.</param>
        /// <param name="authorDisplayName">Author's display name.</param>
        /// <param name="emailAddress">Author's email address.</param>
        /// <param name="reviewName">The title of review.</param>
        /// <param name="projectShortName">The name of the project that review belongs to. e.g. "MSODS"</param>
        /// <returns>Review Id</returns>
        public static string CreateReview(ReviewServiceClient client, string authorName, string authorDisplayName,
            string emailAddress, string reviewName, string projectShortName)
        {
            return client.CreateReview(
                new Author
                {
                    Name = authorName,
                    DisplayName = authorDisplayName,
                    EmailAddress = emailAddress
                }, reviewName, projectShortName).Key;
        }

        /// <summary>
        ///     Step 2.1 - Create a code package.
        /// </summary>
        /// <param name="name">Code package name.</param>
        /// <param name="author">Author's alias.</param>
        /// <param name="userAgent"></param>
        /// <param name="format">Code package format, dpk/tfs shelve set.</param>
        /// <param name="location">The uri location of the package.</param>
        /// <returns>An instance of <see cref="CodePackage" />.</returns>
        public static CodePackage CreateCodePackage(string name, string author, string userAgent,
            CodePackageFormat format, Uri location)
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
        ///     Step 2.2 - Add code package to review.
        /// </summary>
        /// <param name="client">An instance of <see cref="ReviewServiceClient" />.</param>
        /// <param name="reviewId">Id of the review to be added.</param>
        /// <param name="codePackage">Code package to be added.</param>
        public static void AddCodePackage(ReviewServiceClient client, string reviewId, CodePackage codePackage)
        {
            client.AddCodePackage(reviewId, codePackage);
        }

        /// <summary>
        ///     Step 3.1 - Create a review.
        /// </summary>
        /// <param name="name">Reviewer's alias.</param>
        /// <param name="displayName">Reviewer's display name.</param>
        /// <param name="emailAddress">Reviewer's emailAddress.</param>
        /// <param name="required">Required or optional.</param>
        /// <returns>An instance of <see cref="ReviewService.Reviewer" />.</returns>
        public static Reviewer CreateReviewer(string name, string displayName, string emailAddress, bool required)
        {
            return new Reviewer
            {
                Name = name,
                DisplayName = displayName,
                EmailAddress = emailAddress,
                Required = required
            };
        }

        /// <summary>
        ///     Step 3.2 - Add a list of reviewers to the review.
        /// </summary>
        /// <param name="client">An instance of <see cref="ReviewServiceClient" />.</param>
        /// <param name="reviewId">Id of the review to be added.</param>
        /// <param name="reviewers">A list of reviewers.</param>
        public static void AddReviewers(ReviewServiceClient client, string reviewId, Reviewer[] reviewers)
        {
            client.AddReviewers(reviewId, reviewers);
        }

        /// <summary>
        ///     Step 4 - Final step, publish the review.
        /// </summary>
        /// <param name="client">An instance of <see cref="ReviewServiceClient" />.</param>
        /// <param name="reviewId">Id of the review to be added.</param>
        /// <param name="messageFromAuthor"></param>
        public static void PublishReview(ReviewServiceClient client, string reviewId, string messageFromAuthor)
        {
            client.PublishReview(reviewId, messageFromAuthor);
        }

        /// <summary>
        ///     Monitor status for an active review, given the key.
        ///     A review is regarded as completed when at least one reviewer is signed off.
        /// </summary>
        /// <param name="client">An instance of <see cref="ReviewDashboardServiceClient" />.</param>
        /// <param name="key">Key of the request to be monitored. Can be accessed from DB.</param>
        /// <param name="author">Author of the review.</param>
        /// <returns>True if at least one reviewer is signed off.</returns>
        public static bool ReviewCompleted(ReviewDashboardServiceClient client, string key, string author)
        {
            var response = client.Query(new CodeReviewQuery
            {
                Authors = new[] { author },
                ReviewStatuses = new[] { CodeReviewStatus.Active },
                UserAgent = author
            });
            if (response != null)
            {
                foreach (var review in response.Reviews)
                {
                    if (review.Key.Equals(key))
                    {
                        foreach (var reviewer in review.Reviewers)
                        {
                            if (reviewer.Status == ReviewerStatus.SignedOff)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}