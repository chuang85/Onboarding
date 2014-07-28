using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Onboarding.Models
{
    public class OnboardingRequest
    {
        [Key]
        public int RequestId { get; set; }

        // Need this?
        [Required]
        public string RequestSubject { get; set; }

        public string TempXmlStore { get; set; }

        public DateTime CreatedDate { get; set; }

        public string DisplayCreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string DisplayModifiedDate { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        public RequestState State { get; set; }

        public RequestType Type { get; set; }

        public byte[] Blob { get; set; }

        public string CodeFlowId { get; set; }

        public int ChangelistNumber { get; set; }

        public int BuildNumber { get; set; }

        public int RTONumber { get; set; }
    }

    public enum RequestType
    {
        [Description("Create SPT")]
        CreateSPT = 5,
        [Description("Update SPT")]
        UpdateSPT = 10,
        [Description("Create Application")]
        CreateApplication = 15,
        [Description("Update Application")]
        UpdateApplication = 20,
        [Description("Add Cert To KeyGroup")]
        AddCertToKeyGroup = 25
    }

    public enum RequestState
    {
        [Description("Created")]
        Created = 100,
        [Description("Pending Review")]
        PendingReview = 110,
        [Description("Review Completed")]
        ReviewCompleted = 120,
        [Description("RTD Queued")]
        RTDQueued = 130,
        [Description("RTD Approved")]
        RTDApproved = 140,
        [Description("Completed")]
        Completed = 150,
        [Description("Canceled")]
        Canceled = 160
    }
}