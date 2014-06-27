using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Onboarding.Models
{
    public class OnboardingRequest
    {
        [Key]
        public int RequestId { get; set; }

        // Need this?
        public string DisplayName { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        //public RequestState RequestState { get; set; }

        //public RequestType RequestType { get; set; }

        public byte[] Blob { get; set; }

        public int ChangelistNumber { get; set; }

        public int BuildNumber { get; set; }

        public int RTONumber { get; set; }

        public enum RequestType
        {
            CreateSPT,
            UpdateSPT,
            CreateApplication,
            UpdateApplication,
            AddCertToKeyGroup
        };

        public enum RequestState
        {
            Created,
            PendingApproval,
            Approved,
            CheckedIn,
            BuildQueued,
            BuildFinished,
            RTDQueued,
            RTDApproved,
            Completed
        };
    }
}