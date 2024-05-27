

namespace POEMgr.Application.TransferModels
{
    public class PoeRequest_save_req
    {
        public List<PoeRequest_save_req_requestStatus> RequestStatus { get; set; }

        public List<PoeRequest_save_req_SubscriptionStatus> SubscriptionStatus { get; set; }

        public List<string> RequestFiles { get; set; }

        public string Status { get; set; }
    }

    public class PoeRequest_save_req_requestStatus
    {
        public string Id { get; set; }
        public string Status { get; set; }
    }

    public class PoeRequest_save_req_SubscriptionStatus
    {
        public string SubscriptionId { get; set; }
        public string Status { get; set; }
    }
}
