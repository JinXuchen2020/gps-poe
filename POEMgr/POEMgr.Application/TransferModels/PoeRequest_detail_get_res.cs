

using System.ComponentModel.DataAnnotations;

namespace POEMgr.Application.TransferModels
{
    public class PoeRequest_detail_get_res
    {
        public string Id { get; set; }
        public Incentive_detail_get_res Incentive { get; set; }
        public Partner_detail_get_res Partner { get; set; }
        public Customer_detail_get_res Customer { get; set; }
        public string Status { get; set; }
        public string DeadLineDate { get; set; }
        public List<PoeRequest_detail_get_res_requestFile> RequestFiles { get; set; }
        public List<PoeRequest_detail_get_res_checkPoints> RequestCheckPoints { get; set; }
        public List<PoeRequest_detail_get_res_checkPoints> AuditCheckPoints { get; set; }
        public List<PoeRequest_detail_get_res_subscriptionCheck> Subscriptions { get; set; }
        public List<PoeRequest_detail_get_res_auditLog> Logs { get; set; }
    }

    public class PoeRequest_detail_get_res_checkPoints
    {
        public string Id { get; set; }
        public string Status { get; set; }
    }

    public class Partner_detail_get_res
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class Customer_detail_get_res
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Subscriptions { get; set; }
    }

    public class PoeRequest_detail_get_res_requestFile
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public string Path { get; set; }
    }

    public class PoeRequest_detail_get_res_subscriptionCheck
    {
        public string SubscriptionId { get; set; }
        public string Status { get; set; }
    }

    public class PoeRequest_detail_get_res_auditLog
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Reason { get; set; }

        public DateTime? CreatedTime { get; set; }
    }
}
