

namespace POEMgr.Application.TransferModels
{
    public class AuditPoeRequestRequest
    {
        public List<ApprovePoeRequestRequest_FileIntegrity> AuditStatus { get; set; }

        public List<ApprovePoeRequestRequest_SubscriptionStatus> SubscriptionStatus { get; set; }

        public string Status { get; set; }

        public string Reason { get; set; }

        public string DeadLineDate { get; set; }
    }

    public class ApprovePoeRequestRequest_FileIntegrity
    {
        public string Id { get; set; }
        public string Status { get; set; }
    }

    public class ApprovePoeRequestRequest_SubscriptionStatus
    {
        public string SubscriptionId { get; set; }
        public string Status { get; set; }
    }
}
