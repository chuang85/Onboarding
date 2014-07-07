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
        //[Required]
        public string DisplayName { get; set; }

        public string TempXmlStore { get; set; }

        public DateTime CreatedDate { get; set; }

        public string DisplayCreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string DisplayModifiedDate { get; set; }

        //[Required]
        public string CreatedBy { get; set; }

        //public RequestState State { get; set; }

        //public RequestType Type { get; set; }

        public string State { get; set; }

        public string Type { get; set; }

        public byte[] Blob { get; set; }

        public string CodeFlowId { get; set; }

        public int ChangelistNumber { get; set; }

        public int BuildNumber { get; set; }

        public int RTONumber { get; set; } 
    }

    public enum RequestType
    {
        CreateSPT,
        UpdateSPT,
        CreateApplication,
        UpdateApplication,
        AddCertToKeyGroup
    }

    public enum RequestState
    {
        Created = 1,
        PendingApproval,
        Approved,
        CheckedIn,
        BuildQueued,
        BuildFinished,
        RTDQueued,
        RTDApproved,
        Completed
    }
}